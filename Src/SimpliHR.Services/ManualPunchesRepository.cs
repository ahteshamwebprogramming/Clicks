using SimpliHR.Services.DBContext;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Helper;
using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using SimpliHR.Infrastructure.Models.Employee;
using Microsoft.AspNetCore.Mvc;
using static System.Collections.Specialized.BitVector32;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Master;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SimpliHR.Services;

public class ManualPunchesRepository : DapperGenericRepository<ManualPunches>, IManualPunchesRepository
{
    public ManualPunchesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
    public async Task<string> SendManualPunchActionMail(ManualPunchesAction userAction)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;

        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (userAction != null)
        {

            int iCtr = 0;
            //createmail
            // string actionPath = "https://localhost:7151/";
            //string actionPath = "https://simplihr2uat-web.azurewebsites.net/";
            string actionPath = "https://simplihrms.com/";
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "ManualPunchesAction.html";
            string sTableData = string.Empty;
            try
            {
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                List<ManualPunches> manualPunchList = new List<ManualPunches>();
                manualPunchList = await GetTableData<ManualPunches>(IDBConn, null, $" ManualPunchId IN(SELECT value FROM STRING_SPLIT('{userAction.ManualPunchIds}',','))");
                if (manualPunchList.Count > 0)
                {

                    EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, null, $"EmployeeId={manualPunchList[0].EmployeeId}", "")).FirstOrDefault();
                    EmployeeMaster managerData = (await GetQueryAll<EmployeeMaster>($"SELECT EmployeeName,OfficialEmail FROM EmployeeMaster a WHERE EmployeeId IN(SELECT ManagerId from EmployeeMaster b WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND b.IsActive=1)  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                    DepartmentKeyValues employeeDepartment = (await GetQueryAll<DepartmentKeyValues>($"SELECT a.DepartmentName,a.DepartmentId FROM DepartmentMaster a INNER JOIN EmployeeMaster b ON a.DepartmentId=b.DepartmentId WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                    JobTitleKeyValues employeeDesig = (await GetQueryAll<JobTitleKeyValues>($"SELECT a.JobTitle,a.JobTitleId FROM JobTitleMaster a INNER JOIN EmployeeMaster b ON a.JobTitleId=b.JobTitleId WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId}  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                    //List<ShiftMaster> shiftList = (await GetQueryAll<ShiftMaster>($"SELECT a.ShiftCode,a.ShiftName FROM ShiftMaster a INNER JOIN EmployeeMaster b ON a.ShiftIdAttended=b.ShiftCode WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId} AND a.UnitId = {employeeMasterInfo.UnitId}  AND a.IsActive=1", IDBConn, trans));
                    mailTemplate = mailTemplate.Replace("#empName#", employeeMasterInfo.EmployeeName);
                    mailTemplate = mailTemplate.Replace("#empCode#", employeeMasterInfo.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#empImg#", userAction.Profile);
                    //This line is commented because gmail is deleting the messages
                    //mailTemplate = mailTemplate.Replace("#empImage#", "data:image/png;base64," + Convert.ToBase64String(employeeMasterInfo.ProfileImage, 0, employeeMasterInfo.ProfileImage.Length));
                   // mailTemplate = mailTemplate.Replace("#empDesig#", (employeeDesig == null ? "" : employeeDesig.JobTitle));
                    mailTemplate = mailTemplate.Replace("#empDepart#", employeeDepartment.DepartmentName);
                    mailTemplate = mailTemplate.Replace("#createdOn#", DateTime.Now.ToString("dd-MMM-yyyy"));
                    //mailTemplate = mailTemplate.Replace("#assignedOn#", managerData.EmployeeName);
                    string dutyDates = string.Join(",", manualPunchList.Select(x => $"'{x.ManualPunchDate.Value.ToString("yyyy-MM-dd")}'").Distinct());
                    List<ManualPunches> punchesList = await GetTableData<ManualPunches>(IDBConn, null, $"EmployeeId={manualPunchList[0].EmployeeId} AND ManualPunchDate IN({dutyDates})", "");
                    if (employeeMasterInfo != null)
                    {
                        if (punchesList.Count > 0)
                            mailTemplate = mailTemplate.Replace("#ticket#", punchesList[0].TicketId);

                        userAction.ManualPunchIds = "," + userAction.ManualPunchIds + ",";
                        foreach (var manualPunch in punchesList)
                        {
                            if (userAction.ManualPunchIds.Contains("," + manualPunch.ManualPunchId.ToString() + ","))
                            {
                                string shiftCode = "NA";
                            //    var shiftCode = attendanceHistoryData.Where(x => x.DutyDate == manualPunch.ManualPunchDate).Select(r => r.ShiftIDAttended).FirstOrDefault().ToString();
                            //    List<ShiftMaster> shiftList = (await GetQueryAll<ShiftMaster>($"SELECT ShiftId,a.ShiftCode,a.ShiftName FROM ShiftMaster a INNER JOIN AttendanceHistory b ON b.ShiftIdAttended=a.ShiftCode " +
                            //$"WHERE a.ShiftCode = '{shiftCode}' AND b.UnitId = {employeeMasterInfo.UnitId} and b.EmployeeId= {employeeMasterInfo.EmployeeId}  and b.dutydate= '{manualPunch.ManualPunchDate.Value.ToString("yyyy-MM-dd")}'  AND a.IsActive=1 ", IDBConn, trans));
                            //    shiftCode = shiftList.Where(x => x.ShiftCode == shiftCode).Select(r => r.ShiftName).FirstOrDefault().ToString();
                                iCtr = iCtr + 1;
                                var status = manualPunch.ActionType == "R" ? "Rejected" : manualPunch.ActionType == "A" ? "Approved" : "";
                                sTableData = sTableData + $"<tr style='background: #fff;'>" +
                          $"<td style='padding:10px;text-align:left;font-size:13px'>{manualPunch.ManualPunchDate.Value.ToString("dd-MM-yyyy")}</td>" +
                         $"<td style='padding:10px;text-align:left;font-size:13px'>{manualPunch.ManualPunchInTime / manualPunch.ManualPunchOutTime}</td>" +
                         $"<td style='padding:10px;text-align:left;font-size:13px'>NA</td>" +
                         $"<td style='padding:10px;text-align:left;font-size:13px'>{shiftCode}</td>" +
                         $"<td style='padding:10px;text-align:left;font-size:13px'>NA</td>" +
                           $"<td style='padding:10px;text-align:left;font-size:13px'>{manualPunch.ManualPunchReason}</td>"+
                              $"<td style='padding:10px;text-align:left;font-size:13px'>{manualPunch.ActionRemark}</td>" +
                      $"<td style='padding:10px;text-align:left;font-size:13px'>{status}</td></tr>";
                                manualPunch.IsApprovalMailSent = true;
                            }
                        }
                        string manualPunchIDs = string.Join(",", punchesList.Select(x => $"{x.ManualPunchId}").Distinct());
                        mailTemplate = mailTemplate.Replace("#tableDetails#", sTableData);
                       // string EmployeeEmailId = employeeMasterInfo.OfficialEmail;
                        string EmployeeEmailId = string.IsNullOrEmpty(employeeMasterInfo.OfficialEmail) ? employeeMasterInfo.EmailId : employeeMasterInfo.OfficialEmail;
                        //EmployeeEmailId= "juyalpradeep@gmail.com";
                        sSubject = $"SimpliHR2.0 manual punches approval request";

                        isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com", null, userAction.DisplayName, userAction.EmailProvider);

                        if (isMailSend)
                            await IDBConn.ExecuteAsync(@"UPDATE ManualPunches SET IsActionMailSent=@IsActionMailSent", punchesList, null);
                        return "Success";
                    }
                    return "Employee details not found";
                }
                return "Punches not found";
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }

    public async Task<bool> SendManualPunchesApprovalMail(List<ManualPunches> manualPunchList, List<AttendanceHistory> attendanceHistoryData, IDbConnection IDBConn, IDbTransaction trans, string profilePic, string displayName, int emailtemplate = 0)
    {
        bool isMailSend = false;
        int iCtr = 0;
        //createmail
        //  string actionPath = "https://localhost:7151/";
        //string actionPath = "https://simplihr2uat-web.azurewebsites.net/";
        string actionPath = "https://simplihrms.com/";
        string sSubject = "";
        StringBuilder mailBuilder = new StringBuilder();
        string sFileName = "AttendanceApproval.html";
        string sTableData = string.Empty;
        String mailTemplate = MailHelper.GetMailTemplate(sFileName);
        string pPath = Path.Combine(actionPath, profilePic);
        if (manualPunchList.Count > 0)
        {
            EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, trans, $"EmployeeId={manualPunchList[0].EmployeeId}", "")).FirstOrDefault();
            EmployeeMaster ManagerData = (await GetQueryAll<EmployeeMaster>($"SELECT EmployeeName,OfficialEmail,UnitId,EmailId FROM EmployeeMaster a WHERE EmployeeId IN(SELECT ManagerId from EmployeeMaster b WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND b.IsActive=1)  AND a.IsActive=1", IDBConn, trans)).FirstOrDefault();
            DepartmentKeyValues employeeDepartment = (await GetQueryAll<DepartmentKeyValues>($"SELECT a.DepartmentName,a.DepartmentId FROM DepartmentMaster a INNER JOIN EmployeeMaster b ON a.DepartmentId=b.DepartmentId WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND a.IsActive=1", IDBConn, trans)).FirstOrDefault();
            JobTitleKeyValues employeeDesig = (await GetQueryAll<JobTitleKeyValues>($"SELECT a.JobTitle,a.JobTitleId FROM JobTitleMaster a INNER JOIN EmployeeMaster b ON a.JobTitleId=b.JobTitleId WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId}  AND a.IsActive=1", IDBConn, trans)).FirstOrDefault();

            mailTemplate = mailTemplate.Replace("#empName#", employeeMasterInfo.EmployeeName);
            if (employeeMasterInfo.ProfileImage != null)
                mailTemplate = mailTemplate.Replace("#empImage#", pPath);
            mailTemplate = mailTemplate.Replace("#empCode#", employeeMasterInfo.EmployeeCode);
            mailTemplate = mailTemplate.Replace("#empDepart#", employeeDepartment.DepartmentName);
            mailTemplate = mailTemplate.Replace("#createdOn#", DateTime.Now.ToString("dd-MMM-yyyy"));
            mailTemplate = mailTemplate.Replace("#assignedOn#", ManagerData.EmployeeName);
            string dutyDates = string.Join(",", manualPunchList.Select(x => $"'{x.ManualPunchDate.Value.ToString("yyyy-MM-dd")}'").Distinct());
            List<ManualPunches> punchesList = await GetTableData<ManualPunches>(IDBConn, trans, $"EmployeeId={manualPunchList[0].EmployeeId} AND ManualPunchDate IN({dutyDates})", "");
            if (employeeMasterInfo != null)
            {
                if (punchesList.Count > 0)
                    mailTemplate = mailTemplate.Replace("#ticket#", punchesList[0].TicketId);

                foreach (var manualPunch in punchesList)
                {

                    var shiftCode = attendanceHistoryData.Where(x => x.DutyDate == manualPunch.ManualPunchDate).Select(r => r.ShiftIDAttended).FirstOrDefault().ToString();
                    List<ShiftMaster> shiftList = (await GetQueryAll<ShiftMaster>($"SELECT ShiftId,a.ShiftCode,a.ShiftName FROM ShiftMaster a INNER JOIN AttendanceHistory b ON b.ShiftIdAttended=a.ShiftCode " +
                $"WHERE a.ShiftCode = '{shiftCode}' AND b.UnitId = {employeeMasterInfo.UnitId} and b.EmployeeId= {employeeMasterInfo.EmployeeId}  and b.dutydate= '{manualPunch.ManualPunchDate.Value.ToString("yyyy-MM-dd")}'  AND a.IsActive=1 ", IDBConn, trans));
                    shiftCode = shiftList.Where(x => x.ShiftCode == shiftCode).Select(r => r.ShiftName).FirstOrDefault().ToString();
                    iCtr = iCtr + 1;
                    sTableData = sTableData + $"<tr style='background: #fff;'>" +
                         $"<td style='padding:10px;text-align:left;font-size:13px'>{manualPunch.ManualPunchDate.Value.ToString("dd-MM-yyyy")}</td>" +
                        $"<td style='padding:10px;text-align:left;font-size:13px'>{manualPunch.ManualPunchInTime / manualPunch.ManualPunchOutTime}</td>" +
                        $"<td style='padding:10px;text-align:left;font-size:13px'>NA</td>" +
                        $"<td style='padding:10px;text-align:left;font-size:13px'>{shiftCode}</td>" +
                        $"<td style='padding:10px;text-align:left;font-size:13px'>NA</td>" +
                     $"<td style='padding:10px;text-align:left;font-size:13px'>{manualPunch.ManualPunchReason}</td></tr>";
                    manualPunch.IsApprovalMailSent = true;
                }
                string manualPunchIDs = CommonHelper.Encrypt(string.Join(",", punchesList.Select(x => $"{x.ManualPunchId}").Distinct()));
                mailTemplate = mailTemplate.Replace("#tableDetails#", sTableData);

                string ManagerEmailId = string.IsNullOrEmpty(ManagerData.OfficialEmail) ? ManagerData.EmailId : ManagerData.OfficialEmail;
                // ManagerEmailId = "mhdahtesham@gmail.com";
                string approvalAction = $"{actionPath}EmployeeAttendanceUI/ProcessAttendance/{manualPunchIDs}&A".Replace(" ", "");
                string rejectionAction = $"{actionPath}EmployeeAttendanceUI/ProcessAttendance/{manualPunchIDs}&R".Replace(" ", "");
                string partialApproval = $"{actionPath}account/login".Replace(" ", "");
                mailTemplate = mailTemplate.Replace("#approvalAction#", approvalAction);
                mailTemplate = mailTemplate.Replace("#rejectionAction#", rejectionAction);
                mailTemplate = mailTemplate.Replace("#partialApproval#", partialApproval);
                sSubject = $"SimpliHR2.0 manual punches for approval of {employeeMasterInfo.EmployeeName}";
                //isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), ManagerEmailId, "simplihr97@gmail.com", profilePic);
                isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), ManagerEmailId, "simplihr97@gmail.com", null, displayName, emailtemplate);

                if (isMailSend)
                    await IDBConn.ExecuteAsync(@"UPDATE ManualPunches SET IsApprovalMailSent=@IsApprovalMailSent", punchesList, trans);
            }

            //replace all mailTemplate content
        }

        return isMailSend;
    }
    public async Task<List<AttendanceHistory>> GetAttendance(string spName, DynamicParameters param)
    {
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }

        return await GetSPData<AttendanceHistory>(IDBConn, null, "GetAttendance", param);

    }

    public async Task<string> SendManualPunchesforApproval(List<AttendanceHistory> attendanceHistoryData, string profilePic, string displayName, int emailtemplate = 0)
    {
        IDbConnection IDBConn = DbConnection;
        string sWhere = string.Empty, sOrderBy = string.Empty, sReturnMsg = "Success", status;
        TimeSpan MinimumTime, MaximumTime;
        bool isEdit = false;
        ManualPunches manualPunch = new ManualPunches();
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                List<ManualPunches> manualPunchList = new List<ManualPunches>();
                List<TicketMasterDTO> ticketMasterList = new List<TicketMasterDTO>();
                List<AttendanceSetting> attendanceSettingList = new List<AttendanceSetting>();
                string shiftCodes = string.Join(",", attendanceHistoryData.Select(x => $"'{x.ShiftIDAttended}'").Distinct());
                if (shiftCodes.Length > 0)
                {
                    string sQuery = $"SELECT b.ShiftId,b.ShiftCode,a.LegendType, a.MinimumTime, a.MaximumTime FROM AttendanceSetting a " +
                                    $"INNER JOIN ShiftMaster b ON a.ShiftCode=b.ShiftCode WHERE b.ShiftCode in({shiftCodes}) AND a.IsActive=1 and b.IsActive=1";
                    attendanceSettingList = (await GetTableData<AttendanceSetting>(sQuery, IDBConn, trans));
                }
                foreach (var attendance in attendanceHistoryData)
                {
                    //int id = await EexecuteAddAsync(punch, IDBConn, trans);
                    sWhere = $"EmployeeId={attendance.EmployeeId} AND ManualPunchDate = '{attendance.DutyDate.ToString("yyyy-MM-dd")}' AND ActionType!='R'";
                    if ((await GetTableData<ManualPunches>(IDBConn, trans, sWhere, "")).Count() > 0)
                    {
                        trans.Rollback();
                        return $"Manual Punch already  applied for date '{attendance.DutyDate}'";
                    }
                    DateTime? shiftStartAt = CommonHelper.StringToDateTime(attendance.ShiftStartTime.Value.ToShortDateString() + " " + attendance.InTime);
                    DateTime? shiftEndtAt = CommonHelper.StringToDateTime(attendance.ShiftEndTime.Value.ToShortDateString() + " " + attendance.OutTime);
                    if ((shiftEndtAt.Value - shiftStartAt.Value).TotalMinutes < 0)
                    {
                        // trans.Rollback();
                        return $"Proper time not entered for '{attendance.DutyDate}'";
                    }

                    manualPunchList.Add(new ManualPunches
                    {
                        EmployeeId = attendance.EmployeeId,
                        ManualPunchReason = attendance.ManualPunchReason,
                        ManualPunchDate = attendance.DutyDate,
                        HostName = attendance.HostName,
                        GPSLocation = attendance.GPSLocation,
                        IPAddress = attendance.IPAddress,
                        latitude = attendance.latitude,
                        longitude = attendance.longitude,
                        TicketId = attendance.TicketId,
                        ManualPunchInTime = (attendance.InTime != null ? attendance.InTime : null),
                        ManualPunchOutTime = (attendance.OutTime != null ? attendance.OutTime : null),
                        PunchType = (attendance.InTime != null && attendance.OutTime != null ? "Both" : (attendance.InTime != null && attendance.OutTime == null ? "InPunch" : (attendance.InTime == null && attendance.OutTime != null ? "OutPunch" : null))),
                        //CreatedBy = attendance.CreatedBy, Get the Logedin userid
                        IsActive = true
                    });

                    //ticketMasterList.Add(new TicketMasterDTO
                    //{

                    //    TicketCode = attendance.TicketId,
                    //    ModuleId = 0,
                    //    TicketSource = "Attendance",
                    //    CreatedBy = attendance.EmployeeId,
                    //    CreatedOn = DateTime.Now,
                    //    Status = 0,
                    //    IsActive = true,
                    //    UnitId = attendance.UnitId

                    //}) ;
                }
                if (manualPunchList.Count > 0)
                {
                    await IDBConn.ExecuteAsync(@"
                        insert ManualPunches(EmployeeID,ManualPunchReason,ManualPunchDate,ManualPunchInTime,ManualPunchOutTime,PunchType,IsActive,TicketId)
                        values(@EmployeeID, @ManualPunchReason,@ManualPunchDate,@ManualPunchInTime,@ManualPunchOutTime,@PunchType, 
                        @IsActive,@TicketId)", manualPunchList, trans);
                    await IDBConn.ExecuteAsync(@"UPDATE AttendanceHistory SET DutyDate=@ManualPunchDate, InTime=@ManualPunchInTime,OutTime=@ManualPunchOutTime,
                        HostName=@HostName,IPAddress=@IPAddress,GPSLocation=@GPSLocation,Longitude=@longitude,Latitude=@latitude
                        WHERE EmployeeId=@EmployeeID AND DutyDate=@ManualPunchDate", manualPunchList, trans);

                    //await IDBConn.ExecuteAsync(@"
                    //    insert TicketMaster(TicketId,ModuleId,TicketSource,CreatedBy,CreatedOn,[Status],IsActive,UnitId)
                    //    values(@TicketCode, @ModuleId,@TicketSource,@CreatedBy,@CreatedOn,@Status,@IsActive,@UnitId)", ticketMasterList, trans);

                    bool isApprovalMailForwarded = await SendManualPunchesApprovalMail(manualPunchList, attendanceHistoryData, IDBConn, trans, profilePic, displayName, emailtemplate);

                    //if (isApprovalMailForwarded) trans.Commit();
                    //else trans.Rollback();

                }

                //if (manualPunchList.Count == 0)
                // trans.Rollback();

                return "Success";

            }

            //ProcessAttendance
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return "fail to save roster. Error occured while saving roster.";
        }
    }



    public async Task<string> ProcessManualPunches(List<AttendanceHistory> attendanceHistoryData)
    {
        IDbConnection IDBConn = DbConnection;
        string sWhere = string.Empty, sOrderBy = string.Empty, sReturnMsg = "Success", status;
        TimeSpan MinimumTime, MaximumTime;
        bool isEdit = false;
        ManualPunches manualPunch = new ManualPunches();
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {
                List<ManualPunches> manualPunchList = new List<ManualPunches>();
                List<AttendanceSetting> attendanceSettingList = new List<AttendanceSetting>();
                string shiftCodes = string.Join(",", attendanceHistoryData.Select(x => $"'{x.ShiftIDAttended}'").Distinct());
                if (shiftCodes.Length > 0)
                {
                    string sQuery = $"SELECT b.ShiftId,b.ShiftCode,a.LegendType, a.MinimumTime, a.MaximumTime FROM AttendanceSetting a " +
                                    $"INNER JOIN ShiftMaster b ON a.ShiftCode=b.ShiftCode WHERE b.ShiftCode in({shiftCodes}) AND a.IsActive=1 and b.IsActive=1";
                    attendanceSettingList = (await GetTableData<AttendanceSetting>(sQuery, IDBConn, trans));
                }
                foreach (var attendance in attendanceHistoryData)
                {
                    //int id = await EexecuteAddAsync(punch, IDBConn, trans);
                    sWhere = $"EmployeeId={attendance.EmployeeId} AND ManualPunchDate = '{attendance.DutyDate.ToString("yyyy-MM-dd")}'";
                    if ((await GetTableData<ManualPunches>(IDBConn, trans, sWhere, "")).Count() > 0)
                    {
                        trans.Rollback();
                        return $"Manual Punch already  applied for date '{attendance.DutyDate}'";
                    }
                    DateTime? shiftStartAt = CommonHelper.StringToDateTime(attendance.ShiftStartTime.Value.ToShortDateString() + " " + attendance.InTime);
                    DateTime? shiftEndtAt = CommonHelper.StringToDateTime(attendance.ShiftEndTime.Value.ToShortDateString() + " " + attendance.OutTime);
                    if ((shiftEndtAt.Value - shiftStartAt.Value).TotalMinutes < 0)
                    {
                        trans.Rollback();
                        return $"Proper time not entered for '{attendance.DutyDate}'";
                    }

                    manualPunchList.Add(new ManualPunches
                    {
                        EmployeeId = attendance.EmployeeId,
                        ManualPunchReason = attendance.ManualPunchReason,
                        ManualPunchDate = attendance.DutyDate,
                        ManualPunchInTime = (attendance.InTime != null ? attendance.InTime : null),
                        ManualPunchOutTime = (attendance.OutTime != null ? attendance.OutTime : null),
                        PunchType = (attendance.InTime != null && attendance.OutTime != null ? "Both" : (attendance.InTime != null && attendance.OutTime == null ? "InPunch" : (attendance.InTime == null && attendance.OutTime != null ? "OutPunch" : null))),
                        //CreatedBy = attendance.CreatedBy, Get the Logedin userid
                        IsActive = true
                    });


                    TimeSpan? presentTime = shiftEndtAt.Value.Subtract(shiftStartAt.Value);
                    TimeSpan? totalShiftTime = attendance.ShiftEndTime.Value.Subtract(attendance.ShiftStartTime.Value);

                    attendance.Present = presentTime;
                    double absentTime = (totalShiftTime.Value - presentTime.Value).TotalSeconds;
                    attendance.Absent = absentTime > 0 ? totalShiftTime.Value.Subtract(presentTime.Value) : null;

                    MinimumTime = (TimeSpan)attendanceSettingList.Where(r => (r.ShiftCode == attendance.ShiftIDAttended && r.LegendType == 1)).Select(x => x.MinimumTime).FirstOrDefault();
                    MaximumTime = (TimeSpan)attendanceSettingList.Where(r => r.ShiftCode == attendance.ShiftIDAttended && r.LegendType == 1).Select(x => x.MaximumTime).FirstOrDefault();

                    status = presentTime >= MinimumTime ? "Present" : string.Empty;
                    if (status.Equals(string.Empty))
                    {
                        MinimumTime = (TimeSpan)attendanceSettingList.Where(r => (r.ShiftCode == attendance.ShiftIDAttended && r.LegendType == 2)).Select(x => x.MinimumTime).FirstOrDefault();
                        MaximumTime = (TimeSpan)attendanceSettingList.Where(r => r.ShiftCode == attendance.ShiftIDAttended && r.LegendType == 2).Select(x => x.MaximumTime).FirstOrDefault();
                        status = presentTime >= MinimumTime ? "Half" : string.Empty;
                    }
                    if (status.Equals(string.Empty))
                    {
                        MinimumTime = (TimeSpan)attendanceSettingList.Where(r => (r.ShiftCode == attendance.ShiftIDAttended && r.LegendType == 3)).Select(x => x.MinimumTime).FirstOrDefault();
                        MaximumTime = (TimeSpan)attendanceSettingList.Where(r => r.ShiftCode == attendance.ShiftIDAttended && r.LegendType == 3).Select(x => x.MaximumTime).FirstOrDefault();
                        status = presentTime >= MinimumTime && presentTime <= MaximumTime ? "Absent" : string.Empty;
                    }
                    attendance.Status = status;
                    attendance.IsManual = true;
                }
                if (manualPunchList.Count > 0)
                {
                    IDBConn.Execute(@"
                        insert ManualPunches(EmployeeID,ManualPunchReason,ManualPunchDate,ManualPunchInTime,ManualPunchOutTime,PunchType,IsActive)
                        values(@EmployeeID, @ManualPunchReason,@ManualPunchDate,@ManualPunchInTime,@ManualPunchOutTime,@PunchType, 
                        @IsActive)", manualPunchList, trans);

                }
                if (attendanceHistoryData.Count > 0)
                {

                    await IDBConn.ExecuteAsync(@"UPDATE AttendanceHistory SET InTime=@InTime,OutTime=@OutTime,Present=@Present,Absent=@Absent,IsManual=@IsManual,Status=@Status
                        WHERE EmployeeId=@EmployeeID AND DutyDate=@DutyDate", attendanceHistoryData, trans);

                }
                if (attendanceHistoryData.Count == 0 || manualPunchList.Count == 0)
                    trans.Rollback();
                else
                    trans.Commit();
                return "Success";

            }

            //ProcessAttendance
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return "fail to save roster. Error occured while saving roster.";
        }
    }

}


