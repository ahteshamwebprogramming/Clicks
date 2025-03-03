using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Services.DBContext;
using System.Data;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SimpliHR.Services;

public class EmployeeExitResignationRepository : DapperGenericRepository<EmployeeExitResignation>, IEmployeeExitResignationRepository
{
    public EmployeeExitResignationRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
    public async Task<string> SendEmailResignationDetailsReceivedByManager(ExitEmailDTO dto)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (dto != null)
        {
            int iCtr = 0;
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "ResignationRequestReceivedByManager.html";
            string sTableData = string.Empty;
            try
            {
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                if (dto.ResignationListId > 0)
                {
                    ExitEmailDTO? exitDetails = await GetResignationData(dto.ResignationListId);

                    mailTemplate = mailTemplate.Replace("#empName#", exitDetails?.EmployeeName);
                    //mailTemplate = mailTemplate.Replace("#empImg#", pPath);
                    mailTemplate = mailTemplate.Replace("#empCode#", exitDetails?.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#empDepart#", exitDetails?.EmployeeDepartment);
                    mailTemplate = mailTemplate.Replace("#createdOn#", exitDetails?.CreationDateEmployee?.ToString("dd-MMM-yyyy"));

                    mailTemplate = mailTemplate.Replace("#ticket#", exitDetails?.TicketId);
                    mailTemplate = mailTemplate.Replace("#resignationDate#", exitDetails?.ResignationDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#lastWorkingDate#", exitDetails?.LastWorkingDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#reason#", exitDetails?.ReasonForLeaving);
                    mailTemplate = mailTemplate.Replace("#noticePeriod#", exitDetails?.NoticePeriod?.ToString() + " Days");
                    mailTemplate = mailTemplate.Replace("#empRemarks#", exitDetails?.EmployeeComments);

                    string? receiverMail = exitDetails?.ManagerEmailId;
                    string[]? ccReceiverMail = { exitDetails?.HREmailId };
                    string? emailDisplayName = exitDetails?.EmailDisplayName;
                    int? emailProvider = exitDetails?.EmailProvider;

                    sSubject = $"Resignation Request - {exitDetails?.EmployeeCode}/{exitDetails?.EmployeeName}/{exitDetails?.EmployeeDepartment}";

                    AlternateView mainBody = GetEmbeddedImage("", mailTemplate);

                    isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), receiverMail, "", null, emailDisplayName, emailProvider, null, ccReceiverMail);
                    if (isMailSend)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Error";
                    }
                }
                return "Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
    }

    public async Task<string> SendEmailResignationRequestRejectedByManager(ExitEmailDTO dto)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (dto != null)
        {
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "ResignationRequestRejectedByManager.html";
            string sTableData = string.Empty;
            try
            {
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                if (dto.ResignationListId > 0)
                {
                    ExitEmailDTO? exitDetails = await GetResignationData(dto.ResignationListId);

                    mailTemplate = mailTemplate.Replace("#empName#", exitDetails?.EmployeeName);
                    //mailTemplate = mailTemplate.Replace("#empImg#", pPath);
                    mailTemplate = mailTemplate.Replace("#empCode#", exitDetails?.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#empDepart#", exitDetails?.EmployeeDepartment);
                    mailTemplate = mailTemplate.Replace("#createdOn#", exitDetails?.CreationDateEmployee?.ToString("dd-MMM-yyyy"));

                    mailTemplate = mailTemplate.Replace("#ticket#", exitDetails?.TicketId);
                    mailTemplate = mailTemplate.Replace("#resignationDate#", exitDetails?.ResignationDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#lastWorkingDate#", exitDetails?.LastWorkingDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#reason#", exitDetails?.ReasonForLeaving);
                    mailTemplate = mailTemplate.Replace("#noticePeriod#", exitDetails?.NoticePeriod?.ToString() + " Days");
                    mailTemplate = mailTemplate.Replace("#empRemarks#", exitDetails?.EmployeeComments);
                    mailTemplate = mailTemplate.Replace("#managerRemarks#", exitDetails?.ManagerRemarks);



                    string? receiverMail = exitDetails?.EmployeeEmailId;
                    string[]? ccReceiverMail = { exitDetails?.HREmailId };
                    string? emailDisplayName = exitDetails?.EmailDisplayName;
                    int? emailProvider = exitDetails?.EmailProvider;

                    sSubject = $"Resignation Request - {exitDetails?.EmployeeCode}/{exitDetails?.EmployeeName}/{exitDetails?.EmployeeDepartment}";

                    AlternateView mainBody = GetEmbeddedImage("", mailTemplate);

                    isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), receiverMail, "", null, emailDisplayName, emailProvider, null, ccReceiverMail);
                    if (isMailSend)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Error";
                    }
                }
                return "Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }
    public async Task<string> SendEmailResignationRequestRejectedByAdmin(ExitEmailDTO dto)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (dto != null)
        {
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "ResignationRequestRejectedByAdmin.html";
            string sTableData = string.Empty;
            try
            {
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                if (dto.ResignationListId > 0)
                {
                    ExitEmailDTO? exitDetails = await GetResignationData(dto.ResignationListId);

                    mailTemplate = mailTemplate.Replace("#empName#", exitDetails?.EmployeeName);
                    //mailTemplate = mailTemplate.Replace("#empImg#", pPath);
                    mailTemplate = mailTemplate.Replace("#empCode#", exitDetails?.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#empDepart#", exitDetails?.EmployeeDepartment);
                    mailTemplate = mailTemplate.Replace("#createdOn#", exitDetails?.CreationDateEmployee?.ToString("dd-MMM-yyyy"));

                    mailTemplate = mailTemplate.Replace("#ticket#", exitDetails?.TicketId);
                    mailTemplate = mailTemplate.Replace("#resignationDate#", exitDetails?.ResignationDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#lastWorkingDate#", exitDetails?.LastWorkingDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#reason#", exitDetails?.ReasonForLeaving);
                    mailTemplate = mailTemplate.Replace("#noticePeriod#", exitDetails?.NoticePeriod?.ToString() + " Days");
                    mailTemplate = mailTemplate.Replace("#empRemarks#", exitDetails?.EmployeeComments);
                    mailTemplate = mailTemplate.Replace("#managerRemarks#", exitDetails?.ManagerRemarks);
                    mailTemplate = mailTemplate.Replace("#adminRemarks#", exitDetails?.AdminRemarks);



                    string? receiverMail = exitDetails?.EmployeeEmailId;
                    string[]? ccReceiverMail = { exitDetails?.ManagerEmailId };
                    string? emailDisplayName = exitDetails?.EmailDisplayName;
                    int? emailProvider = exitDetails?.EmailProvider;

                    sSubject = $"Resignation Request - {exitDetails?.EmployeeCode}/{exitDetails?.EmployeeName}/{exitDetails?.EmployeeDepartment}";

                    AlternateView mainBody = GetEmbeddedImage("", mailTemplate);

                    isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), receiverMail, "", null, emailDisplayName, emailProvider, null, ccReceiverMail);
                    if (isMailSend)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Error";
                    }
                }
                return "Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }
    public async Task<string> SendEmailResignationRequestReceivedByHR_ManagerApproval(ExitEmailDTO dto)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (dto != null)
        {
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "ResignationRequestReceivedByHR_ManagerApproval.html";
            string sTableData = string.Empty;
            try
            {
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                if (dto.ResignationListId > 0)
                {
                    ExitEmailDTO? exitDetails = await GetResignationData(dto.ResignationListId);

                    mailTemplate = mailTemplate.Replace("#empName#", exitDetails?.EmployeeName);
                    //mailTemplate = mailTemplate.Replace("#empImg#", pPath);
                    mailTemplate = mailTemplate.Replace("#empCode#", exitDetails?.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#managerName#", exitDetails?.ManagerName);
                    mailTemplate = mailTemplate.Replace("#managerCode#", exitDetails?.ManagerCode);
                    mailTemplate = mailTemplate.Replace("#empDepart#", exitDetails?.EmployeeDepartment);

                    mailTemplate = mailTemplate.Replace("#ticket#", exitDetails?.TicketId);
                    mailTemplate = mailTemplate.Replace("#createdOn#", exitDetails?.CreationDateEmployee?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#managerApprovalDate#", exitDetails?.ManagerApprovalDate?.ToString("dd-MMM-yyyy"));


                    mailTemplate = mailTemplate.Replace("#resignationDate#", exitDetails?.ResignationDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#lastWorkingDate#", exitDetails?.LastWorkingDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#reason#", exitDetails?.ReasonForLeaving);
                    mailTemplate = mailTemplate.Replace("#noticePeriod#", exitDetails?.NoticePeriod?.ToString() + " Days");
                    mailTemplate = mailTemplate.Replace("#empRemarks#", exitDetails?.EmployeeComments);

                    mailTemplate = mailTemplate.Replace("#resignationDateManager#", exitDetails?.ResignationDateManager?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#lastWorkingDateManager#", exitDetails?.LastWorkingDateManager?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#reasonManager#", exitDetails?.ReasonForLeavingManager);
                    mailTemplate = mailTemplate.Replace("#noticePeriodManager#", exitDetails?.NoticePeriod?.ToString() + " Days");
                    TimeSpan? difference = null;
                    string noticeTreatment = exitDetails?.NoticePeriodWaiveOff == true ? "Waived" : "Recovery";
                    if (exitDetails != null && exitDetails.LastWorkingDateManager.HasValue && exitDetails.ResignationDateManager.HasValue)
                    {
                        difference = exitDetails.LastWorkingDateManager.Value - exitDetails.ResignationDateManager.Value;
                    }
                    if (difference.HasValue)
                    {
                        int? shortfalldays = exitDetails?.NoticePeriod - difference.Value.Days;
                        mailTemplate = mailTemplate.Replace("#noticeShortfall#", shortfalldays.ToString() + " Days/" + noticeTreatment);
                    }
                    else
                    {
                        mailTemplate = mailTemplate.Replace("#noticeShortfall#", "" + noticeTreatment);
                    }
                    mailTemplate = mailTemplate.Replace("#managerRemarks#", exitDetails?.ManagerRemarks);


                    string? receiverMail = exitDetails?.HREmailId;
                    string? emailDisplayName = exitDetails?.EmailDisplayName;
                    int? emailProvider = exitDetails?.EmailProvider;

                    sSubject = $"Resignation Request - {exitDetails?.EmployeeCode}/{exitDetails?.EmployeeName}/{exitDetails?.EmployeeDepartment}";

                    AlternateView mainBody = GetEmbeddedImage("", mailTemplate);

                    isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), receiverMail, "", null, emailDisplayName, emailProvider);
                    if (isMailSend)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Error";
                    }
                }
                return "Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }
    public async Task<string> SendEmailResignationRequestReceivedByEmployee_HRApproval(ExitEmailDTO dto)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (dto != null)
        {
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "ResignationRequestReceivedByEmployee_HRApproval.html";
            string sTableData = string.Empty;
            try
            {
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                if (dto.ResignationListId > 0)
                {
                    ExitEmailDTO? exitDetails = await GetResignationData(dto.ResignationListId);

                    mailTemplate = mailTemplate.Replace("#empName#", exitDetails?.EmployeeName);
                    //mailTemplate = mailTemplate.Replace("#empImg#", pPath);
                    mailTemplate = mailTemplate.Replace("#empCode#", exitDetails?.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#managerName#", exitDetails?.ManagerName);
                    mailTemplate = mailTemplate.Replace("#managerCode#", exitDetails?.ManagerCode);
                    mailTemplate = mailTemplate.Replace("#empDepart#", exitDetails?.EmployeeDepartment);

                    mailTemplate = mailTemplate.Replace("#ticket#", exitDetails?.TicketId);
                    mailTemplate = mailTemplate.Replace("#createdOn#", exitDetails?.CreationDateEmployee?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#hrApprovalDate#", exitDetails?.AdminApprovalDate?.ToString("dd-MMM-yyyy"));


                    mailTemplate = mailTemplate.Replace("#resignationDate#", exitDetails?.ResignationDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#lastWorkingDate#", exitDetails?.LastWorkingDate?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#reason#", exitDetails?.ReasonForLeaving);
                    mailTemplate = mailTemplate.Replace("#noticePeriod#", exitDetails?.NoticePeriod?.ToString() + " Days");
                    mailTemplate = mailTemplate.Replace("#empRemarks#", exitDetails?.EmployeeComments);

                    mailTemplate = mailTemplate.Replace("#resignationDateHR#", exitDetails?.ResignationDateAdmin?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#lastWorkingDateHR#", exitDetails?.LastWorkingDateAdmin?.ToString("dd-MMM-yyyy"));
                    mailTemplate = mailTemplate.Replace("#noticePeriodHR#", exitDetails?.NoticePeriod?.ToString() + " Days");

                    TimeSpan? difference = null;
                    string noticeTreatment = exitDetails?.NoticePeriodWaiveOffAdmin == true ? "Waived" : "Recovery";
                    if (exitDetails != null && exitDetails.LastWorkingDateAdmin.HasValue && exitDetails.ResignationDateAdmin.HasValue)
                    {
                        difference = exitDetails.LastWorkingDateAdmin.Value - exitDetails.ResignationDateAdmin.Value;
                    }
                    if (difference.HasValue)
                    {
                        mailTemplate = mailTemplate.Replace("#noticeBeingServed#", difference.Value.Days.ToString() + " Days");
                        int? shortfalldays = exitDetails?.NoticePeriod - difference.Value.Days;
                        mailTemplate = mailTemplate.Replace("#noticeShortfall#", shortfalldays.ToString() + " Days/" + noticeTreatment);
                    }
                    else
                    {
                        mailTemplate = mailTemplate.Replace("#noticeBeingServed#", "");
                        mailTemplate = mailTemplate.Replace("#noticeShortfall#", "" + noticeTreatment);
                    }


                    mailTemplate = mailTemplate.Replace("#hrRemarks#", exitDetails?.AdminRemarks);


                    string? receiverMail = exitDetails?.EmployeeEmailId;
                    string[]? ccReceiverMail = { exitDetails?.ManagerEmailId };
                    string? emailDisplayName = exitDetails?.EmailDisplayName;
                    int? emailProvider = exitDetails?.EmailProvider;

                    sSubject = $"Resignation Request - {exitDetails?.EmployeeCode}/{exitDetails?.EmployeeName}/{exitDetails?.EmployeeDepartment}";

                    AlternateView mainBody = GetEmbeddedImage("", mailTemplate);

                    isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), receiverMail, "", null, emailDisplayName, emailProvider, null, ccReceiverMail);
                    if (isMailSend)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Error";
                    }
                }
                return "Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }
    public async Task<string> SendEmailExitInterviewEmailToEmployee(ExitEmailDTO dto)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (dto != null)
        {
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "ExitInterviewEmailToEmployee.html";
            string sTableData = string.Empty;
            try
            {
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                if (dto.ResignationListId > 0)
                {
                    ExitEmailDTO? exitDetails = await GetResignationData(dto.ResignationListId);

                    mailTemplate = mailTemplate.Replace("#empName#", exitDetails?.EmployeeName);

                    mailTemplate = mailTemplate.Replace("#lastWorkingDateHR#", exitDetails?.LastWorkingDateAdmin?.ToString("dd-MMM-yyyy"));

                    string? receiverMail = exitDetails?.EmployeeEmailId;
                    string? emailDisplayName = exitDetails?.EmailDisplayName;
                    int? emailProvider = exitDetails?.EmailProvider;

                    sSubject = $"Exit Interview";

                    AlternateView mainBody = GetEmbeddedImage("", mailTemplate);

                    isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), receiverMail, "", null, emailDisplayName, emailProvider, null);
                    if (isMailSend)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Error";
                    }
                }
                return "Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }

    public async Task<string> SendEmailClearanceRequestEmail(ExitEmailDTO dto)
    {
        bool isMailSend = false;
        IDbConnection IDBConn = DbConnection;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        if (dto != null)
        {
            string sSubject = "";
            StringBuilder mailBuilder = new StringBuilder();
            string sFileName = "ClearanceRequestEmail.html";
            string sTableData = string.Empty;
            try
            {
                String mailTemplate = MailHelper.GetMailTemplate(sFileName);
                if (dto.EmployeeId > 0)
                {
                    string queryGetResignationListId = "Select ResignationListId from EmployeeExitResignation where EmployeeId=@EmployeeId and AdminApproval=1";
                    var paramGetResignationListId = new { EmployeeId = dto.EmployeeId };
                    dto.ResignationListId = await GetEntityData<int>(queryGetResignationListId, paramGetResignationListId);

                    ExitEmailDTO? exitDetails = await GetResignationData(dto.ResignationListId);
                    string queryEmployeeExitClearanceOwnersList = "Select \r\nem.EmployeeId\r\n,em.EmployeeCode\r\n,em.EmployeeName\r\n,em.EmailId\r\nfrom EmployeeExitClearance eec\r\nJoin EmployeeMaster em on eec.ClearanceBy=em.EmployeeId\r\nwhere eec.EmployeeId=@EmployeeId\r\nand eec.isActive=1";
                    var paramEmployeeExitClearanceOwnersList = new { EmployeeId = dto.EmployeeId };
                    List<EmployeeKeyValues> EmployeeExitClearanceOwnersList = await GetTableData<EmployeeKeyValues>(queryEmployeeExitClearanceOwnersList, paramEmployeeExitClearanceOwnersList);

                    mailTemplate = mailTemplate.Replace("#empName#", exitDetails?.EmployeeName);
                    mailTemplate = mailTemplate.Replace("#empCode#", exitDetails?.EmployeeCode);
                    mailTemplate = mailTemplate.Replace("#empDepart#", exitDetails?.EmployeeDepartment);
                    mailTemplate = mailTemplate.Replace("#lastWorkingDateAdmin#", exitDetails?.LastWorkingDateAdmin?.ToString("dd-MMM-yyyy"));

                    string? emailDisplayName = exitDetails?.EmailDisplayName;
                    int? emailProvider = exitDetails?.EmailProvider;

                    sSubject = $"Exit Clearance Alert! {exitDetails?.EmployeeCode} / {exitDetails?.EmployeeName} / {exitDetails?.EmployeeDepartment} / LWD {exitDetails?.LastWorkingDateAdmin?.ToString("dd-MMM-yyyy")}";

                    if (EmployeeExitClearanceOwnersList != null)
                    {
                        foreach (var item in EmployeeExitClearanceOwnersList)
                        {
                            if (item != null)
                            {
                                mailTemplate = mailTemplate.Replace("#clearanceOwnerName#", item.EmployeeName);
                                string? receiverMail = item?.EmailId;
                                AlternateView mainBody = GetEmbeddedImage("", mailTemplate);
                                isMailSend = MailHelper.SendMail(sSubject, mailTemplate.Replace('\"', '"'), receiverMail, "", null, emailDisplayName, emailProvider, null);
                            }
                        }
                    }
                    if (isMailSend)
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Success";
                    }
                }
                return "Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        return "Inputs are not correct";
        //return isMailSend;
    }

    private async Task<ExitEmailDTO> GetResignationData(int ResignationId)
    {
        ExitEmailDTO? exitDetails = new ExitEmailDTO();
        var parameters = new { ResignationListId = ResignationId };
        string query = "Select  em.EmployeeName  \r\n,em.EmailId as EmployeeEmailId  \r\n,dm.DepartmentName EmployeeDepartment  \r\n\r\n,em.ManagerId  \r\n,emManager.EmployeeName as ManagerName\r\n,emManager.EmployeeCode as ManagerCode\r\n,emManager.EmailId as ManagerEmailId\r\n\r\n,cs.ClientId\r\n,cs.ClientLogo  \r\n,um.EmailDisplayName  \r\n,cs.EmailProvider  \r\n,cs.ProfileImage\r\n\r\n,um.EmailID as HREmailId,eer.*   from EmployeeExitResignation eer\r\nJoin EmployeeMaster em on eer.EmployeeId=em.EmployeeId\r\nJoin EmployeeMaster emManager on em.ManagerId=emManager.EmployeeId\r\nJoin UnitMaster um on em.UnitId=um.UnitID\r\nJoin ClientSetting cs on cs.ClientId=em.ClientId\r\nJoin DepartmentMaster dm on em.DepartmentId=dm.DepartmentId\r\nwhere eer.ResignationListId=@ResignationListId";
        var resExitDetails = await GetTableData<ExitEmailDTO>(query, parameters);

        //query = "";

        if (resExitDetails != null && resExitDetails.Count() > 0)
        {
            exitDetails = resExitDetails.FirstOrDefault();
            if (exitDetails != null)
            {
                exitDetails.TLDPath = "https://simplihrms.com/";
                string clientLogoPath = "ClientLogo/" + exitDetails?.ClientId.ToString() + "/" + exitDetails?.ClientLogo;
                exitDetails.ClientLogo = Path.Combine(exitDetails.TLDPath, clientLogoPath);
            }
        }
        return exitDetails;
    }

    private AlternateView GetEmbeddedImage(String filePath, string Mailbody)
    {
        AlternateView alternateView = AlternateView.CreateAlternateViewFromString(Mailbody, null, MediaTypeNames.Text.Html);
        return alternateView;
    }
}
