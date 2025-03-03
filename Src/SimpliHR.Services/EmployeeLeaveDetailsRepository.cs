using Dapper;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Services;

public class EmployeeLeaveDetailsRepository : DapperGenericRepository<EmployeeLeaveDetails>, IEmployeeLeaveDetailsRepository
{
   
    public EmployeeLeaveDetailsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }

    public async Task<string> SendLeaveApprovalMail(LeaveAction userAction)
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
            string sFileName = "LeaveApprovalRequest.html";
            string sTableData = string.Empty;
            try
            {
                //  userAction.EmployeeId = 164; C:\HRMS\Src\SimpliHR.WebUI\wwwroot\EmployeePofile\1
                // userAction.TicketId = "Leave_J7CHWEM5DV";
                //string pPath = userAction.Profile + "\\" + userAction.EmployeeId + ".jpg";
                string pPath= Path.Combine(actionPath, userAction.Profile);
                //LinkedResource res = new LinkedResource(@pPath, MediaTypeNames.Image.Jpeg);

                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
             
                if (userAction.EmployeeId > 0)
                {
                 // userAction.TicketId = "Leave/240711-12114389/106";
                   // EmployeeLeaveDetails employeeLeaveInfo = (await GetTableData<EmployeeLeaveDetails>(IDBConn, null, $"TicketId={userAction.TicketId}", "")).FirstOrDefault();
                   //  EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, null, $"EmployeeId={userAction.EmployeeId}", "")).FirstOrDefault();
                    EmployeeMaster managerData = (await GetQueryAll<EmployeeMaster>($"SELECT EmployeeName,OfficialEmail,EmailId FROM EmployeeMaster a WHERE EmployeeId IN(SELECT ManagerId from EmployeeMaster b WHERE EmployeeId = {userAction.EmployeeId} AND b.IsActive=1)  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                    EmployeeMaster employeeMasterInfo = (await GetQueryAll<EmployeeMaster>($"SELECT * FROM EmployeeMaster a WHERE EmployeeId  = {userAction.EmployeeId} AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                  //  EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, null, $"EmployeeId={userAction.EmployeeId}", "")).FirstOrDefault();
                    DepartmentKeyValues employeeDepartment = (await GetQueryAll<DepartmentKeyValues>($"SELECT a.DepartmentName,a.DepartmentId FROM DepartmentMaster a INNER JOIN EmployeeMaster b ON a.DepartmentId=b.DepartmentId WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                    JobTitleKeyValues employeeDesig = (await GetQueryAll<JobTitleKeyValues>($"SELECT a.JobTitle,a.JobTitleId FROM JobTitleMaster a INNER JOIN EmployeeMaster b ON a.JobTitleId=b.JobTitleId WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId}  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();

                   
                    mailTemplate = mailTemplate.Replace("#empName#", employeeMasterInfo.EmployeeName);
                    mailTemplate = mailTemplate.Replace("#empImg#", pPath);
                    mailTemplate = mailTemplate.Replace("#empCode#", employeeMasterInfo.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#empDepart#", employeeDepartment.DepartmentName);
                    mailTemplate = mailTemplate.Replace("#createdOn#", DateTime.Now.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#assignedTo#", managerData.EmployeeName);
                    mailTemplate = mailTemplate.Replace("#ticket#", userAction.TicketId);
                
                    List<EmployeeLeaveDetails> LeaveList = await GetTableData<EmployeeLeaveDetails>(IDBConn, null, $"EmployeeId={userAction.EmployeeId} AND TicketId IN({  "'"+userAction.TicketId+"'" })", "");
                    if (employeeMasterInfo != null)
                    {
                        // userAction.ManualPunchIds = "," + userAction.ManualPunchIds + ",";
                        if (LeaveList.Count > 0)
                        {
                            foreach (var LeaveDetails in LeaveList)
                            {
                                //    if (userAction.ManualPunchIds.Contains("," + manualPunch.ManualPunchId.ToString() + ","))
                                //    {
                                LeaveTypeMaster leavetype = (await GetQueryAll<LeaveTypeMaster>($"SELECT LeaveType FROM LeaveTypeMaster a WHERE LeaveTypeId = {LeaveDetails.LeaveTypeId} AND IsActive=1", IDBConn, null)).FirstOrDefault();
                                var leaveStatus= LeaveDetails.LeaveStatus == 1 ? "Pending" : LeaveDetails.LeaveStatus == 0 ? "Approved" : LeaveDetails.LeaveStatus == 98 ? "Reversal" : LeaveDetails.LeaveStatus == 99 ? "Rejected" : "Awaiting";
                                mailTemplate = mailTemplate.Replace("#leavetype#", leavetype.LeaveType);
                                mailTemplate = mailTemplate.Replace("#status#", leaveStatus);
                                mailTemplate = mailTemplate.Replace("#From#", Convert.ToDateTime(LeaveDetails.StartDate).ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#To#", Convert.ToDateTime(LeaveDetails.EndDate).ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#NoOfLeave#", Convert.ToString(LeaveDetails.NoOfLeave));
                                mailTemplate = mailTemplate.Replace("#Remark#", LeaveDetails.Remarks);
                               
                            }

                            var leaveId = CommonHelper.Encrypt( Convert.ToString(LeaveList[0].LeaveDetailsId));
                            //EmployeeEmailId= "juyalpradeep@gmail.com";
                            string approvalAction = $"{actionPath}Leave/LeaveRegularizeProcess/{leaveId}&A".Replace(" ", "");
                            string rejectionAction = $"{actionPath}Leave/LeaveRegularizeProcess/{leaveId}&R".Replace(" ", "");
                            mailTemplate = mailTemplate.Replace("#approvalAction#", approvalAction);
                            mailTemplate = mailTemplate.Replace("#rejectionAction#", rejectionAction);
                        }

                        string EmployeeEmailId = managerData.OfficialEmail;
                        if (string.IsNullOrEmpty(EmployeeEmailId))
                            EmployeeEmailId = managerData.EmailId;
                        //  mailTemplate = mailTemplate.Replace("#tableDetails#", sTableData);

                        sSubject = $"SimpliHR2.0 leave approval request";

                        AlternateView mainBody = GetEmbeddedImage("", mailTemplate);
                      //  isMailSend = MailHelper.MailSend(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com", mainBody, userAction.DisplayName);
                        isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "",null, userAction.DisplayName, userAction.EmailProvider);

                        //if (isMailSend)
                        //    await IDBConn.ExecuteAsync(@"UPDATE ManualPunches SET IsActionMailSent=@IsActionMailSent", punchesList, null);
                        return "Leave applied Successfully";
                    }
                    return "Employee details not found";
                }
                return "Leave not found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }

    public async Task<string> SendLeaveReversalMail(LeaveAction userAction)
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
            string sFileName = "LeaveReversalRequest.html";
            string sTableData = string.Empty;
            try
            {

                //  userAction.EmployeeId = 164; C:\HRMS\Src\SimpliHR.WebUI\wwwroot\EmployeePofile\1
                // userAction.TicketId = "Leave_J7CHWEM5DV";
                //string pPath = userAction.Profile + "\\" + userAction.EmployeeId + ".jpg";
                string pPath = Path.Combine(actionPath, userAction.Profile);
                //LinkedResource res = new LinkedResource(@pPath, MediaTypeNames.Image.Jpeg);

                String mailTemplate = MailHelper.GetMailTemplate(sFileName);

                if (userAction.EmployeeId > 0)
                {
                    List<EmployeeLeaveDetails> LeaveList = await GetTableData<EmployeeLeaveDetails>(IDBConn, null, $"EmployeeId={userAction.EmployeeId} AND LeaveDetailsId IN({"'" + userAction.LeaveIds + "'"})", "");
                    // userAction.TicketId = "Leave/240711-12114389/106";
                    // EmployeeLeaveDetails employeeLeaveInfo = (await GetTableData<EmployeeLeaveDetails>(IDBConn, null, $"TicketId={userAction.TicketId}", "")).FirstOrDefault();
                    //  EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, null, $"EmployeeId={userAction.EmployeeId}", "")).FirstOrDefault();
                    EmployeeMaster managerData = (await GetQueryAll<EmployeeMaster>($"SELECT EmployeeName,OfficialEmail,EmailId FROM EmployeeMaster a WHERE EmployeeId IN(SELECT ManagerId from EmployeeMaster b WHERE EmployeeId = {userAction.EmployeeId} AND b.IsActive=1)  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                    EmployeeMaster employeeMasterInfo = (await GetQueryAll<EmployeeMaster>($"SELECT * FROM EmployeeMaster a WHERE EmployeeId  = {userAction.EmployeeId} AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                    //  EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, null, $"EmployeeId={userAction.EmployeeId}", "")).FirstOrDefault();
                    DepartmentKeyValues employeeDepartment = (await GetQueryAll<DepartmentKeyValues>($"SELECT a.DepartmentName,a.DepartmentId FROM DepartmentMaster a INNER JOIN EmployeeMaster b ON a.DepartmentId=b.DepartmentId WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                    JobTitleKeyValues employeeDesig = (await GetQueryAll<JobTitleKeyValues>($"SELECT a.JobTitle,a.JobTitleId FROM JobTitleMaster a INNER JOIN EmployeeMaster b ON a.JobTitleId=b.JobTitleId WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId}  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();


                    mailTemplate = mailTemplate.Replace("#empName#", employeeMasterInfo.EmployeeName);
                    mailTemplate = mailTemplate.Replace("#empImg#", pPath);
                    mailTemplate = mailTemplate.Replace("#empCode#", employeeMasterInfo.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#empDepart#", employeeDepartment.DepartmentName);
                    mailTemplate = mailTemplate.Replace("#createdOn#", DateTime.Now.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#assignedTo#", managerData.EmployeeName);
                    mailTemplate = mailTemplate.Replace("#ticket#", LeaveList[0].TicketId);

                    //List<EmployeeLeaveDetails> LeaveList = await GetTableData<EmployeeLeaveDetails>(IDBConn, null, $"EmployeeId={userAction.EmployeeId} AND TicketId IN({"'" + userAction.TicketId + "'"})", "");
                    if (employeeMasterInfo != null)
                    {
                        // userAction.ManualPunchIds = "," + userAction.ManualPunchIds + ",";
                        if (LeaveList.Count > 0)
                        {
                            foreach (var LeaveDetails in LeaveList)
                            {
                                //    if (userAction.ManualPunchIds.Contains("," + manualPunch.ManualPunchId.ToString() + ","))
                                //    {
                                LeaveTypeMaster leavetype = (await GetQueryAll<LeaveTypeMaster>($"SELECT LeaveType FROM LeaveTypeMaster a WHERE LeaveTypeId = {LeaveDetails.LeaveTypeId} AND IsActive=1", IDBConn, null)).FirstOrDefault();
                                var leaveStatus = LeaveDetails.LeaveStatus == 1 ? "Pending" : LeaveDetails.LeaveStatus == 0 ? "Approved" : LeaveDetails.LeaveStatus == 98 ? "Rejected" : LeaveDetails.LeaveStatus == 90 ? "Reversal" : "Awaiting";
                                mailTemplate = mailTemplate.Replace("#leavetype#", leavetype.LeaveType);
                                mailTemplate = mailTemplate.Replace("#status#", leaveStatus);
                                mailTemplate = mailTemplate.Replace("#From#", Convert.ToDateTime(LeaveDetails.StartDate).ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#To#", Convert.ToDateTime(LeaveDetails.EndDate).ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#NoOfLeave#", Convert.ToString(LeaveDetails.NoOfLeave));
                                mailTemplate = mailTemplate.Replace("#Remark#", LeaveDetails.Remarks);

                            }

                            var leaveId = CommonHelper.Encrypt(Convert.ToString(LeaveList[0].LeaveDetailsId));
                            //EmployeeEmailId= "juyalpradeep@gmail.com";
                            //string approvalAction = $"{actionPath}Leave/LeaveRegularizeProcess/{leaveId}&A".Replace(" ", "");
                            //string rejectionAction = $"{actionPath}Leave/LeaveRegularizeProcess/{leaveId}&R".Replace(" ", "");
                            //mailTemplate = mailTemplate.Replace("#approvalAction#", approvalAction);
                            //mailTemplate = mailTemplate.Replace("#rejectionAction#", rejectionAction);
                        }

                        string EmployeeEmailId = managerData.OfficialEmail;
                        if (string.IsNullOrEmpty(EmployeeEmailId))
                            EmployeeEmailId = managerData.EmailId;
                        //  mailTemplate = mailTemplate.Replace("#tableDetails#", sTableData);

                        sSubject = $"SimpliHR2.0 leave Reversal approval request";

                        AlternateView mainBody = GetEmbeddedImage("", mailTemplate);
                        //  isMailSend = MailHelper.MailSend(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com", mainBody, userAction.DisplayName);
                        isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "", null, userAction.DisplayName, userAction.EmailProvider);

                        //if (isMailSend)
                        //    await IDBConn.ExecuteAsync(@"UPDATE ManualPunches SET IsActionMailSent=@IsActionMailSent", punchesList, null);
                        return "Reversal request applied Successfully";
                    }
                    return "Employee details not found";
                }
                return "Leave not found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }

    public async Task<string> SendLeaveRequalizeMail(LeaveAction userAction)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (userAction != null)
        {

            int iCtr = 0;
            //createmail
            //string actionPath = "https://localhost:7151/";
           // string actionPath = "https://simplihr2uat-web.azurewebsites.net/";
            string actionPath = "https://simplihrms.com/";
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "LeaveApprovalStatus.html";
            string sTableData = string.Empty;
            try
            {
                string pPath = Path.Combine(actionPath, userAction.Profile);
                // String pPath = userAction.Profile + "\\" + userAction.ApprovedBy +".jpg";             
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                //  List<ManualPunches> manualPunchList = new List<ManualPunches>();
                // manualPunchList = await GetTableData<ManualPunches>(IDBConn, null, $" ManualPunchId IN(SELECT value FROM STRING_SPLIT('{userAction.ManualPunchIds}',','))");
                if (Convert.ToInt32(userAction.LeaveIds) > 0)
                {
                    string EmployeeEmailId = "";
                 
              
                    List<EmployeeLeaveDetails> LeaveList = await GetTableData<EmployeeLeaveDetails>(IDBConn, null, $"LeaveDetailsId={userAction.LeaveIds}", "");
                    if (LeaveList != null)
                    {
                        // userAction.ManualPunchIds = "," + userAction.ManualPunchIds + ",";
                        if (LeaveList.Count > 0)
                        {
                            foreach (var LeaveDetails in LeaveList)
                            {

                                userAction.EmployeeId = LeaveDetails.EmployeeId;


                                EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, null, $"EmployeeId={userAction.EmployeeId}", "")).FirstOrDefault();
                                EmployeeMaster managerData = (await GetQueryAll<EmployeeMaster>($"SELECT EmployeeName,OfficialEmail FROM EmployeeMaster a WHERE EmployeeId IN(SELECT ManagerId from EmployeeMaster b WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND b.IsActive=1)  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                                DepartmentKeyValues employeeDepartment = (await GetQueryAll<DepartmentKeyValues>($"SELECT a.DepartmentName,a.DepartmentId FROM DepartmentMaster a INNER JOIN EmployeeMaster b ON a.DepartmentId=b.DepartmentId WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                                JobTitleKeyValues employeeDesig = (await GetQueryAll<JobTitleKeyValues>($"SELECT a.JobTitle,a.JobTitleId FROM JobTitleMaster a INNER JOIN EmployeeMaster b ON a.JobTitleId=b.JobTitleId WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId}  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                                //List<ShiftMaster> shiftList = (await GetQueryAll<ShiftMaster>($"SELECT a.ShiftCode,a.ShiftName FROM ShiftMaster a INNER JOIN EmployeeMaster b ON a.ShiftIdAttended=b.ShiftCode WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId} AND a.UnitId = {employeeMasterInfo.UnitId}  AND a.IsActive=1", IDBConn, trans));
                                mailTemplate = mailTemplate.Replace("#empName#", employeeMasterInfo.EmployeeName);
                                mailTemplate = mailTemplate.Replace("#empImg#", pPath);
                                mailTemplate = mailTemplate.Replace("#empCode#", employeeMasterInfo.EmployeeCode);
                                mailTemplate = mailTemplate.Replace("#empDepart#", employeeDepartment.DepartmentName);
                                mailTemplate = mailTemplate.Replace("#createdOn#", DateTime.Now.ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#assignedTo#", managerData.EmployeeName);

                                LeaveTypeMaster leavetype = (await GetQueryAll<LeaveTypeMaster>($"SELECT LeaveType FROM LeaveTypeMaster a WHERE LeaveTypeId = {LeaveDetails.LeaveTypeId} AND IsActive=1", IDBConn, null)).FirstOrDefault();
                                var leaveStatus = LeaveDetails.LeaveStatus == 1 ? "Pending" : LeaveDetails.LeaveStatus == 0 ? "Approved" : LeaveDetails.LeaveStatus == 98 ? "Rejected/Reversaled" : LeaveDetails.LeaveStatus == 99 ? "Reversal" : "Awaiting";
                                mailTemplate = mailTemplate.Replace("#leavetype#", leavetype.LeaveType);
                                mailTemplate = mailTemplate.Replace("#status#", leaveStatus);
                                mailTemplate = mailTemplate.Replace("#From#", Convert.ToDateTime(LeaveDetails.StartDate).ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#To#", Convert.ToDateTime(LeaveDetails.EndDate).ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#NoOfLeave#", Convert.ToString(LeaveDetails.NoOfLeave));
                                mailTemplate = mailTemplate.Replace("#Remark#", userAction.ActionRemarks);
                                mailTemplate = mailTemplate.Replace("#ticket#", LeaveDetails.TicketId);
                                 EmployeeEmailId = employeeMasterInfo.OfficialEmail;
                                if(string.IsNullOrEmpty(EmployeeEmailId))
                                    EmployeeEmailId = employeeMasterInfo.EmailId;
                            }
                        }
                        //  mailTemplate = mailTemplate.Replace("#tableDetails#", sTableData);
                       
                       // EmployeeEmailId= "mhasif78@gmail.com";

                        sSubject = $"HRMS Alert! Leave Request Raised";
                        AlternateView mainBody = GetEmbeddedImage(pPath, mailTemplate);
                        isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com", null, userAction.DisplayName, userAction.EmailProvider);

                        //isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com");

                        //if (isMailSend)
                        //    await IDBConn.ExecuteAsync(@"UPDATE ManualPunches SET IsActionMailSent=@IsActionMailSent", punchesList, null);
                        return "Success";
                    }
                    return "Leave not found";
                }
                return "Leave not found";
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }

    public async Task<string> SendCompoffRequalizeMail(CompOffAction userAction)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (userAction != null)
        {

            int iCtr = 0;
            //createmail
            //string actionPath = "https://localhost:7151/";
            // string actionPath = "https://simplihr2uat-web.azurewebsites.net/";
            string actionPath = "https://simplihrms.com/";
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "leave-approval-status.html";
            string sTableData = string.Empty;
            try
            {
               // String pPath = userAction.Profile + "\\" + userAction.ApprovedBy + ".jpg";
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                //  List<ManualPunches> manualPunchList = new List<ManualPunches>();
                // manualPunchList = await GetTableData<ManualPunches>(IDBConn, null, $" ManualPunchId IN(SELECT value FROM STRING_SPLIT('{userAction.ManualPunchIds}',','))");
                if (Convert.ToInt32(userAction.CompOffIds) > 0)
                {
                    string EmployeeEmailId = "";


                    List<EmployeeLeaveDetails> LeaveList = await GetTableData<EmployeeLeaveDetails>(IDBConn, null, $"LeaveDetailsId={userAction.CompOffIds}", "");
                    if (LeaveList != null)
                    {
                        // userAction.ManualPunchIds = "," + userAction.ManualPunchIds + ",";
                        if (LeaveList.Count > 0)
                        {
                            foreach (var LeaveDetails in LeaveList)
                            {

                                userAction.EmployeeId = LeaveDetails.EmployeeId;


                                EmployeeMaster employeeMasterInfo = (await GetTableData<EmployeeMaster>(IDBConn, null, $"EmployeeId={userAction.EmployeeId}", "")).FirstOrDefault();
                                EmployeeMaster managerData = (await GetQueryAll<EmployeeMaster>($"SELECT EmployeeName,OfficialEmail FROM EmployeeMaster a WHERE EmployeeId IN(SELECT ManagerId from EmployeeMaster b WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND b.IsActive=1)  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                                DepartmentKeyValues employeeDepartment = (await GetQueryAll<DepartmentKeyValues>($"SELECT a.DepartmentName,a.DepartmentId FROM DepartmentMaster a INNER JOIN EmployeeMaster b ON a.DepartmentId=b.DepartmentId WHERE EmployeeId = {employeeMasterInfo.EmployeeId} AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                                JobTitleKeyValues employeeDesig = (await GetQueryAll<JobTitleKeyValues>($"SELECT a.JobTitle,a.JobTitleId FROM JobTitleMaster a INNER JOIN EmployeeMaster b ON a.JobTitleId=b.JobTitleId WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId}  AND a.IsActive=1", IDBConn, null)).FirstOrDefault();
                                //List<ShiftMaster> shiftList = (await GetQueryAll<ShiftMaster>($"SELECT a.ShiftCode,a.ShiftName FROM ShiftMaster a INNER JOIN EmployeeMaster b ON a.ShiftIdAttended=b.ShiftCode WHERE b.EmployeeId = {employeeMasterInfo.EmployeeId} AND a.UnitId = {employeeMasterInfo.UnitId}  AND a.IsActive=1", IDBConn, trans));
                                mailTemplate = mailTemplate.Replace("#empName#", employeeMasterInfo.EmployeeName);
                                // mailTemplate = mailTemplate.Replace("#empImage#", "data:image/png;base64," + Convert.ToBase64String(employeeMasterInfo.ProfileImage, 0, employeeMasterInfo.ProfileImage.Length));
                                mailTemplate = mailTemplate.Replace("#empDesig#", employeeDesig.JobTitle);
                                mailTemplate = mailTemplate.Replace("#empDepart#", employeeDepartment.DepartmentName);
                                mailTemplate = mailTemplate.Replace("#createdOn#", DateTime.Now.ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#assignedTo#", managerData.EmployeeName);

                                LeaveTypeMaster leavetype = (await GetQueryAll<LeaveTypeMaster>($"SELECT LeaveType FROM LeaveTypeMaster a WHERE LeaveTypeId = {LeaveDetails.LeaveTypeId} AND IsActive=1", IDBConn, null)).FirstOrDefault();
                                var leaveStatus = LeaveDetails.LeaveStatus == 1 ? "Pending" : LeaveDetails.LeaveStatus == 0 ? "Approved" : LeaveDetails.LeaveStatus == 98 ? "Reversal" : LeaveDetails.LeaveStatus == 99 ? "Rejected" : "Awaiting";
                                mailTemplate = mailTemplate.Replace("#leavetype#", leavetype.LeaveType);
                                mailTemplate = mailTemplate.Replace("#status#", leaveStatus);
                                mailTemplate = mailTemplate.Replace("#From#", Convert.ToDateTime(LeaveDetails.StartDate).ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#To#", Convert.ToDateTime(LeaveDetails.EndDate).ToString("dd-MMM-yyyy"));
                                mailTemplate = mailTemplate.Replace("#NoOfLeave#", Convert.ToString(LeaveDetails.NoOfLeave));
                                mailTemplate = mailTemplate.Replace("#Remark#", userAction.ActionRemarks);
                                mailTemplate = mailTemplate.Replace("#ticket#", LeaveDetails.TicketId);
                                EmployeeEmailId = employeeMasterInfo.OfficialEmail;
                                if (string.IsNullOrEmpty(EmployeeEmailId))
                                    EmployeeEmailId = employeeMasterInfo.EmailId;
                            }
                        }
                        //  mailTemplate = mailTemplate.Replace("#tableDetails#", sTableData);

                        // EmployeeEmailId= "mhasif78@gmail.com";

                        sSubject = $"SimpliHR2.0 leave regularize status";
                        AlternateView mainBody = GetEmbeddedImage("", mailTemplate);
                        isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com", mainBody, "",1);

                        //isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com");

                        //if (isMailSend)
                        //    await IDBConn.ExecuteAsync(@"UPDATE ManualPunches SET IsActionMailSent=@IsActionMailSent", punchesList, null);
                        return "Success";
                    }
                    return "Leave not found";
                }
                return "Leave not found";
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }

    private  AlternateView GetEmbeddedImage(String filePath, string Mailbody)
    {
        //LinkedResource res = new LinkedResource(filePath);
        //res.ContentId = Guid.NewGuid().ToString();
        //string htmlBody = @"<img alt='' class='h-auto ms-0 rounded user-profile-img' style='width: 70px;border-radius: 2.375rem !important;' src='cid:" + res.ContentId + @"'/>";
        //Mailbody = Mailbody.Replace("#empImage#", htmlBody);
        AlternateView alternateView = AlternateView.CreateAlternateViewFromString(Mailbody, null, MediaTypeNames.Text.Html);
     //   alternateView.LinkedResources.Add(res);
        return alternateView;
    }

    public async Task<string> SendLeaveRequalizeTestMail(LeaveAction userAction)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (userAction != null)
        {

            int iCtr = 0;
            //createmail
            string actionPath = "https://simplihrms.com/";
            // string actionPath = "https://simplihr2uat-web.azurewebsites.net/";
            // string actionPath = "https://simplihr2.azurewebsites.net/";
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "attendance-approval-request-new.html";
            string sTableData = string.Empty;
            try
            {
                String pPath = userAction.Profile + "\\" + userAction.ApprovedBy + ".jpg";
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                //  List<ManualPunches> manualPunchList = new List<ManualPunches>();
                // manualPunchList = await GetTableData<ManualPunches>(IDBConn, null, $" ManualPunchId IN(SELECT value FROM STRING_SPLIT('{userAction.ManualPunchIds}',','))");
               
                       

                       // sSubject = $"HRMS Alert! Leave Request Raised";
                      //  AlternateView mainBody = GetEmbeddedImage(pPath, mailTemplate);
              //  isMailSend = MailHelper.MailSend(sSubject, mailTemplate.Replace('\"', '"'), "mrigaj.g@kaam.com", "simplihr97@gmail.com", mainBody);
              //  isMailSend = MailHelper.MailSend(sSubject, mailTemplate.Replace('\"', '"'), "prashantmahale76@gmail.com", "simplihr97@gmail.com", mainBody);

                        //isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), EmployeeEmailId, "simplihr97@gmail.com");

                        //if (isMailSend)
                        //    await IDBConn.ExecuteAsync(@"UPDATE ManualPunches SET IsActionMailSent=@IsActionMailSent", punchesList, null);
                        return "Success";
                   
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }
}

