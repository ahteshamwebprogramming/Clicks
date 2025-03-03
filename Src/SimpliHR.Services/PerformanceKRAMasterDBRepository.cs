using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Performace;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Services;

public class PerformanceKRAMasterDBRepository : DapperGenericRepository<PerformanceKRAMasterDB>, IPerformanceKRAMasterDBRepository
{
    public PerformanceKRAMasterDBRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }

    public async Task<string> UploadKRADB(List<PerformanceKRAMasterDBDTO> data, int unitId, int PerformanceSettingId)
    {
        IDbConnection IDBConn = DbConnection;
        string sQuery = string.Empty;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                List<string> employeeCodesInFile = data.Select(x => x.EmployeeCode).ToList();
                List<string> employeeCodesInDatabase = await GetTableData<string>("Select employeecode from employeeMaster where unitid=1 and isactive=1 and employeecode is not null", IDBConn, trans);

                var nonMatchingCodes= employeeCodesInFile.Where(code => !employeeCodesInDatabase.Contains(code)).Distinct().ToList();
                
                bool allExist = !nonMatchingCodes.Any();
                if (!allExist)
                {
                    trans.Rollback();
                    return "Invalid Employee Codes Found : " + String.Join(",", nonMatchingCodes.ToArray()) + "";
                }

                sQuery = @"DELETE FROM PerformanceKRAMasterDB WHERE unitID = " + unitId + " and Source='KRA' and PerformanceSettingId=" + PerformanceSettingId + "";
                await IDBConn.ExecuteAsync(sQuery, null, trans);

                sQuery = @"
                        insert into PerformanceKRAMasterDB(EmployeeCode,KRA,Weightage,CreatedDate,CreatedBy,IsActive,UnitId,Source,PerformanceSettingId)
                        values(@EmployeeCode, @KRA,@Weightage,@CreatedDate,@CreatedBy,@IsActive,@UnitId,@Source,@PerformanceSettingId)";
                await IDBConn.ExecuteAsync(sQuery, data, trans);

                List<EmployeeMasterDTO> employeeMasterDTOs = await GetTableData<EmployeeMasterDTO>($"Select EmployeeCode from PerformanceKRAMasterDB where UnitId={unitId} and PerformanceSettingId={PerformanceSettingId} and Source='KRA' and IsActive=1 group by EmployeeCode having sum(Weightage) < 100", IDBConn, trans);

                String.Join(",", employeeMasterDTOs.Select(x => x.EmployeeCode).ToArray());

                if (employeeMasterDTOs != null && employeeMasterDTOs.Count > 0)
                {
                    trans.Rollback();
                    return "Please review these Employee Codes for KRAs weightage : " + String.Join(",", employeeMasterDTOs.Select(x => x.EmployeeCode).ToArray()) + "";
                }

                trans.Commit();
                return "OK";




            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return ex.Message;
        }

    }


    public async Task<string> UploadBehavioralDB(List<PerformanceKRAMasterDBDTO> data, int unitId, int PerformanceSettingId)
    {
        IDbConnection IDBConn = DbConnection;
        string sQuery = string.Empty;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {

                List<string> employeeCodesInFile = data.Select(x => x.EmployeeCode).ToList();
                List<string> employeeCodesInDatabase = await GetTableData<string>("Select employeecode from employeeMaster where unitid=1 and isactive=1 and employeecode is not null", IDBConn, trans);

                var nonMatchingCodes = employeeCodesInFile.Where(code => !employeeCodesInDatabase.Contains(code)).Distinct().ToList();

                bool allExist = !nonMatchingCodes.Any();
                if (!allExist)
                {
                    trans.Rollback();
                    return "Invalid Employee Codes Found : " + String.Join(",", nonMatchingCodes.ToArray()) + "";
                }

                sQuery = @"DELETE FROM PerformanceKRAMasterDB WHERE unitID = " + unitId + " and Source='Behavioral' and PerformanceSettingId=" + PerformanceSettingId + "";
                await IDBConn.ExecuteAsync(sQuery, null, trans);

                sQuery = @"
                        insert into PerformanceKRAMasterDB(EmployeeCode,KRA,Weightage,CreatedDate,CreatedBy,IsActive,UnitId,Source,PerformanceSettingId)
                        values(@EmployeeCode, @KRA,@Weightage,@CreatedDate,@CreatedBy,@IsActive,@UnitId,@Source,@PerformanceSettingId)";
                await IDBConn.ExecuteAsync(sQuery, data, trans);

                List<EmployeeMasterDTO> employeeMasterDTOs = await GetTableData<EmployeeMasterDTO>($"Select EmployeeCode from PerformanceKRAMasterDB where UnitId={unitId} and PerformanceSettingId={PerformanceSettingId} and Source='Behavioral' and IsActive=1 group by EmployeeCode having sum(Weightage) < 100", IDBConn, trans);

                String.Join(",", employeeMasterDTOs.Select(x => x.EmployeeCode).ToArray());

                if (employeeMasterDTOs != null && employeeMasterDTOs.Count > 0)
                {
                    trans.Rollback();
                    return "Please review these Employee Codes for Behavioural weightage : " + String.Join(",", employeeMasterDTOs.Select(x => x.EmployeeCode).ToArray()) + "";
                }

                trans.Commit();
                return "OK";



            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return ex.Message;
        }

    }
}
