using Dapper;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Services.DBContext;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Mapping;
using System.Diagnostics;
using System.Text;

namespace SimpliHR.Services;

public class EmployeeSalaryTemplateMappingRepository : DapperGenericRepository<EmployeeSalaryTemplateMapping>, IEmployeeSalaryTemplateMappingRepository
{
    public EmployeeSalaryTemplateMappingRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
    public async Task<string> SaveEmployeeSalaryTemplateMapping(EmployeeSalaryTemplateMapping employeeSalaryTemplateMapping, string mappedIDs)
    {
        string sMsg = string.Empty;
        StringBuilder sData = new StringBuilder();
        string unMappedIds = string.Empty;
        string sQuery = string.Empty;
        string MappingEmployeeIds = mappedIDs;

        IDbConnection IDBConn = DbConnection;

        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (employeeSalaryTemplateMapping.SalaryTemplateId == null)
        {
            sMsg = "Select the Salary Template to be mapped";
            return sMsg;
        }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                sQuery = $"SELECT MappingName FROM EmployeeSalaryTemplateMapping a WHERE  a.MappingName = '{employeeSalaryTemplateMapping.MappingName}'";
                if (employeeSalaryTemplateMapping.EmployeeSalaryTemplateId != 0)
                    sQuery += $" AND a.EmployeeSalaryTemplateId != {employeeSalaryTemplateMapping.EmployeeSalaryTemplateId}";
                var mappingName = (await GetTableData<EmployeeSalaryTemplateMapping>(sQuery, IDBConn, trans)).Select(x => x.MappingName).FirstOrDefault();
                if (mappingName != null)
                {
                    sMsg = $"'{mappingName}' Mapping name is already exists";
                    return sMsg;
                }
                List<EmployeeSalaryTemplateDetail> unMappedEmps = new List<EmployeeSalaryTemplateDetail>();
                sQuery = $"SELECT a.EmployeeSalaryTemplateId,a.MappingName,a.SalaryTemplateId,b.EmployeeId,c.EmployeeCode,c.EmployeeName,d.DepartmentName,c.DepartmentId,e.TemplateName " +
                        $"FROM EmployeeSalaryTemplateMapping a INNER JOIN EmployeeSalaryTemplateDetail b ON a.EmployeeSalaryTemplateId = b.EmployeeSalaryTemplateId " +
                        $"INNER JOIN EmployeeMaster c ON b.EmployeeId = c.EmployeeId " +
                        $"INNER JOIN DepartmentMaster d ON d.DepartmentId = c.DepartmentId " +
                        $"INNER JOIN SalaryTemplates e ON e.SalaryTemplateId = a.EmployeeSalaryTemplateId " +
                        $"WHERE a.IsActive = 1 AND c.IsActive = 1  AND d.IsActive = 1 AND b.EmployeeId IN ({MappingEmployeeIds})";
                if (employeeSalaryTemplateMapping.EmployeeSalaryTemplateId != 0)
                    sQuery += $" AND a.EmployeeSalaryTemplateId != {employeeSalaryTemplateMapping.EmployeeSalaryTemplateId}";

                var MappedEmps = (await GetTableData<EmployeeTemplateInfo>(sQuery, IDBConn, trans));
                string[] arrUnMapped;
                string[] arrMappingEmployeeIds = MappingEmployeeIds.Split(",");
                if (MappedEmps.Count > 0)
                {

                    foreach (var item in MappedEmps)
                    {
                        MappingEmployeeIds = MappingEmployeeIds.Replace(" ", "").Replace(item.EmployeeId.ToString(), "").Replace(",,", ",");
                        sData.Append($"<tr><td>{item.MappingName}</td><td>{item.EmployeeName}({item.DepartmentName}-{item.EmployeeCode})</td><td>{item.TemplateName}</td></tr>");
                    }
                    sMsg = "Below Employees are alredy mapped with Salary Template<br><br><table><tr><th>Mapping Name</th><th>Employee</th><th>Mapped Template</th></tr><tbody>" +
                            sData.ToString() + "</tbody></table>";
                    //MappingEmployeeIds = unMappedIds;
                }
                if (MappedEmps.Count == arrMappingEmployeeIds.Length)
                    return sMsg;
                int mappingId = 0;
                List<EmployeeSalaryTemplateDetail> unMappedEmployeeMapDetailList = new List<EmployeeSalaryTemplateDetail>();

                if (employeeSalaryTemplateMapping.EmployeeSalaryTemplateId == 0)
                {
                    employeeSalaryTemplateMapping.CreatedOn = DateTime.Now;
                    employeeSalaryTemplateMapping.IsActive = true;
                    //object value = await AddAsync(employeeSalaryTemplateMapping);
                    mappingId = await ExecuteAddAsync(employeeSalaryTemplateMapping, IDBConn, trans);
                }
                else
                {
                    employeeSalaryTemplateMapping.ModifiedOn = DateTime.Now;
                    employeeSalaryTemplateMapping.IsActive = true;
                    object value = await ExecuteUpdateAsync(employeeSalaryTemplateMapping, IDBConn, trans);
                    IDBConn.Execute(@"DELETE FROM EmployeeSalaryTemplateDetail WHERE EmployeeSalaryTemplateId= " + employeeSalaryTemplateMapping.EmployeeSalaryTemplateId, null, trans);
                    mappingId = employeeSalaryTemplateMapping.EmployeeSalaryTemplateId;
                }
                foreach (var eid in MappingEmployeeIds.Split(","))
                {
                    int empId;
                    if (int.TryParse(eid, out empId))
                        unMappedEmployeeMapDetailList.Add(new EmployeeSalaryTemplateDetail() { EmployeeSalaryTemplateId = mappingId, EmployeeId = empId });
                }
                if (mappingId > 0)
                {
                    IDBConn.Execute(@"
                        insert EmployeeSalaryTemplateDetail(EmployeeSalaryTemplateId,EmployeeId)
                        values(@EmployeeSalaryTemplateId,@EmployeeId)", unMappedEmployeeMapDetailList, trans);
                    trans.Commit();
                    if (sMsg == "")
                        sMsg = "Success";
                    else
                        sMsg = sMsg + "<br><br>Rest of employee mapped successfully";
                }
                else
                {
                    sMsg = "Unable to save data";
                }

                //trans.Commit();
                return sMsg;
            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return "fail to save roster. Error occured while saving roster.";
        }


        return sMsg;
    }

    public async Task<string> DeleteEmployeeSalaryTemplateMappingInfo(int employeeSalaryTemplateId)
    {
        string result = "";
        string sQuery = "";
        string sMsg = "";
        IDbConnection IDBConn = DbConnection;

        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (employeeSalaryTemplateId == null || employeeSalaryTemplateId == 0)
        {
            sMsg = "Select the Mapping to be deleted";
            return sMsg;
        }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                sQuery = $"DELETE FROM EmployeeSalaryTemplateDetail WHERE employeeSalaryTemplateId={employeeSalaryTemplateId}";
                var isDeleted = await DeleteTableData<EmployeeSalaryTemplateDetail>(IDBConn, trans, $"employeeSalaryTemplateId={employeeSalaryTemplateId}");
                if (isDeleted)
                {
                    isDeleted = false;
                     isDeleted = await DeleteTableData<EmployeeSalaryTemplateMapping>(IDBConn, trans, $"employeeSalaryTemplateId={employeeSalaryTemplateId}");
                   
                    if (isDeleted)
                    {
                        trans.Commit();
                        sMsg = "Success";
                    }
                    return sMsg;
                }
            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return "Delete Failed<br>" + ex.Message;
        }
        return sMsg;
    }

    public class EmployeeTemplateInfo
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeCode { get; set; }
        public string TemplateName { get; set; }
        public string MappingName { get; set; }
    }
}


