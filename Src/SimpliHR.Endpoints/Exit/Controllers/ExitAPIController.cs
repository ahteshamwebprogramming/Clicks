using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.ITDeclaration;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using System.Net;
using System.Transactions;

namespace SimpliHR.Endpoints.Exit;

[Route("api/[controller]")]
[ApiController]
public class ExitAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExitAPIController> _logger;
    private readonly IMapper _mapper;
    // private readonly SimpliDbContext _simpliDbContext;

    public ExitAPIController(IUnitOfWork unitOfWork, ILogger<ExitAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        // _simpliDbContext = SimpliDbContext;
    }
    public async Task<IActionResult> GetResignationList(int? unitId, int UserId, string type)
    {
        try
        {
            List<EmployeeExitResignationDTO> outputModel = new List<EmployeeExitResignationDTO>();

            string where = "";
            string sQuery = "";
            if (type == "Manager")
            {
                where = "and em.ManagerId=" + UserId + "";
            }
            else if (type == "Admin")
            {
                //where = " and rl.ManagerApproval=1";
            }




            sQuery = "select rl.TicketId,rl.ResignationListId ,rl.EmployeeId ,rl.EmployeeCode ,em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName,rl.CreationDateEmployee ,(Case when AdminApproval=1 then rl.ResignationDateAdmin when ManagerApproval=1 then rl.ResignationDateManager else rl.ResignationDate end)ResignationDate ,(Case when AdminApproval=1 then rl.LastWorkingDateAdmin when ManagerApproval=1 then rl.LastWorkingDateManager else rl.LastWorkingDate end)LastWorkingDate  ,rl.NoticePeriod ,rl.ReasonForLeaving ,rl.EmployeeComments,rl.SettlementStatus,rl.InterviewStatus,rl.ManagerApproval,rl.AdminApproval,rl.NoticePeriodWaiveOffAdmin,rl.ActivateExitInterview,rl.ClearanceByPass,(Case when (Select count(1) from [dbo].[EmployeeExitClearance] eec where IsActive=1 and eec.EmployeeId=em.EmployeeId)=0 then 0 when (Select count(1) from [dbo].[EmployeeExitClearance] eec where IsActive=1 and eec.EmployeeId=em.EmployeeId) = (Select count(1) from [dbo].[EmployeeExitClearanceHeader] eec where IsActive=1 and eec.EmployeeId=em.EmployeeId) then 2 else 1 end )ClearanceStatus from EmployeeExitResignation rl join EmployeeMaster em on rl.EmployeeId = em.EmployeeId join JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId join DepartmentMaster dm on em.DepartmentId = dm.DepartmentId where rl.UnitId=" + unitId + " " + where + " order by EmployeeName asc";



            List<EmployeeExitResignationDTO> resignationListDTOs = await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeExitResignationDTO>(sQuery);
            return Ok(resignationListDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<IActionResult> GetResignationListByStatus(int? unitId, int UserId, string type, string Status)
    {
        try
        {
            List<EmployeeExitResignationDTO> outputModel = new List<EmployeeExitResignationDTO>();

            string where = "";
            string sQuery = "";
            string whereClauseOPT = " ";
            if (type == "Manager")
            {
                where = "and em.ManagerId=@UserId";
                if (Status == "Pending")
                {
                    whereClauseOPT = " and (ManagerApproval=0 and AdminApproval=0) ";
                }
                else if (Status == "Completed")
                {
                    whereClauseOPT = " and ( \r\n(ManagerApproval=1) or (rl.CreationDateEmployee is null and rl.AdminApproval=1) \r\n) ";
                }
                else if (Status == "All")
                {

                }
            }
            else if (type == "Admin")
            {
                if (Status == "Pending")
                {
                    whereClauseOPT = " AND AdminApproval!=-1 AND (\r\n    (rl.ClearanceByPass = 0\r\n\t--and (Select count(1) from [dbo].[EmployeeExitClearance] eec where IsActive=1 and eec.EmployeeId=em.EmployeeId)=0\r\n    and (\r\n        SELECT COUNT(1) \r\n        FROM [dbo].[EmployeeExitClearance] eec \r\n        WHERE eec.IsActive = 1 AND eec.EmployeeId = em.EmployeeId\r\n    ) != (\r\n        SELECT COUNT(1) \r\n        FROM [dbo].[EmployeeExitClearanceHeader] eec \r\n        WHERE eec.IsActive = 1 AND eec.EmployeeId = em.EmployeeId\r\n    ))\r\n\tor AdminApproval=0\r\n\tor (ActivateExitInterview=1 and InterviewStatus!=2)\r\n\tor rl.SettlementStatus!=2\r\n) ";
                }
                else if (Status == "Completed")
                {
                    whereClauseOPT = " AND (\r\n    rl.ClearanceByPass = 1\r\n    or ((\r\n        SELECT COUNT(1) \r\n        FROM [dbo].[EmployeeExitClearance] eec \r\n        WHERE eec.IsActive = 1 AND eec.EmployeeId = em.EmployeeId\r\n    ) = (\r\n        SELECT COUNT(1) \r\n        FROM [dbo].[EmployeeExitClearanceHeader] eec \r\n        WHERE eec.IsActive = 1 AND eec.EmployeeId = em.EmployeeId\r\n    ) and (Select count(1) from [dbo].[EmployeeExitClearance] eec where IsActive=1 and eec.EmployeeId=em.EmployeeId)!=0\r\n\t)\r\n)\r\nand AdminApproval=1\r\nand (ActivateExitInterview=0 or InterviewStatus=2)\r\nand rl.SettlementStatus=1 ";
                }
                else if (Status == "All")
                {
                    whereClauseOPT = "";
                }

            }

            sQuery = "select rl.TicketId,rl.ResignationListId ,rl.EmployeeId ,rl.EmployeeCode ,em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName,rl.CreationDateEmployee ,(Case when AdminApproval=1 then rl.ResignationDateAdmin when ManagerApproval=1 then rl.ResignationDateManager else rl.ResignationDate end)ResignationDate ,(Case when AdminApproval=1 then rl.LastWorkingDateAdmin when ManagerApproval=1 then rl.LastWorkingDateManager else rl.LastWorkingDate end)LastWorkingDate  ,rl.NoticePeriod ,rl.ReasonForLeaving ,rl.EmployeeComments,rl.SettlementStatus,rl.InterviewStatus,rl.ManagerApproval,rl.NoticePeriodWaiveOff,rl.AdminApproval,rl.NoticePeriodWaiveOffAdmin,rl.ActivateExitInterview,rl.ClearanceByPass,(Case when (Select count(1) from [dbo].[EmployeeExitClearance] eec where IsActive=1 and eec.EmployeeId=em.EmployeeId)=0 then 0 when (Select count(1) from [dbo].[EmployeeExitClearance] eec where IsActive=1 and eec.EmployeeId=em.EmployeeId) = (Select count(1) from [dbo].[EmployeeExitClearanceHeader] eec where IsActive=1 and eec.EmployeeId=em.EmployeeId) then 2 else 1 end )ClearanceStatus from EmployeeExitResignation rl join EmployeeMaster em on rl.EmployeeId = em.EmployeeId join JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId join DepartmentMaster dm on em.DepartmentId = dm.DepartmentId where rl.UnitId=@UnitId " + where + whereClauseOPT + " order by EmployeeName asc";

            var parameters = new { UnitId = unitId, UserId };

            List<EmployeeExitResignationDTO> resignationListDTOs = await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeExitResignationDTO>(sQuery, parameters);
            return Ok(resignationListDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<IActionResult> GetEmployeeInfoByEmployeeId(int EmployeeId)
    {
        try
        {
            var parameters = new { EmployeeId };
            string query = "Select * from EmployeeMaster where EmployeeId=@EmployeeId";
            var res = await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeMasterDTO>(query, parameters);
            if (res != null && res.Count() > 0)
            {
                return Ok(res.FirstOrDefault());
            }
            return BadRequest(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving {nameof(GetEmployeeInfoByEmployeeId)}");
            throw;
        }
    }

    public async Task<ExitViewModel> SaveResignationDetails(ExitViewModel exitVM)
    {
        if (exitVM.ResignationDetails?.ResignationListId == 0)
        {
            //CallSave

            exitVM.ResignationDetails.LWDPolicy = exitVM.ResignationDetails.LastWorkingDate?.ToString("dd-MMM-yyyy");

            exitVM.ResignationDetails.TicketId = CommonHelper.CreateTicket("Exit", "");
            exitVM.ResignationDetails.ResignationListId = await _unitOfWork.EmployeeExitResignation.AddAsync(_mapper.Map<EmployeeExitResignation>(exitVM.ResignationDetails));
            _unitOfWork.Save();
            exitVM.DisplayMessage = "SUCCESS";

        }
        else
        {
            await _unitOfWork.EmployeeExitResignation.UpdateAsync(_mapper.Map<EmployeeExitResignation>(exitVM.ResignationDetails));
            _unitOfWork.Save();
            exitVM.DisplayMessage = "SUCCESS";
            //Call Update
        }

        //string sQuery = "select rl.ResignationListId ,rl.EmployeeId ,rl.EmployeeCode ,em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName ,rl.ResignationDate ,rl.LastWorkingDate ,rl.NoticePeriod ,rl.ReasonForLeaving ,rl.EmployeeComments from EmployeeExitResignation rl join EmployeeMaster em on rl.EmployeeId = em.EmployeeId join JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId join DepartmentMaster dm on em.DepartmentId = dm.DepartmentId where EmployeeId=" + exitVM.ResignationDetails.EmployeeId + " order by EmployeeName asc";
        //exitVM = await EmployeeExitList(exitVM);

        ExitEmailDTO exitEmailDTO = new ExitEmailDTO();
        exitEmailDTO.ResignationListId = exitVM?.ResignationDetails?.ResignationListId ?? default(int);
        await SendMail_ResignationDetailsReceivedByManager(exitEmailDTO);
        return exitVM;
    }
    public async Task<ActionResult> SaveResignationDetailsByManager(EmployeeExitResignationDTO inputData)
    {
        if (inputData.ResignationListId > 0)
        {
            EmployeeExitResignation? fetchedData = _unitOfWork.EmployeeExitResignation.GetTableDataExec<EmployeeExitResignation>("select * from [dbo].[EmployeeExitResignation] where resignationlistid=" + inputData.ResignationListId).Result.FirstOrDefault();
            if (fetchedData != null)
            {

                fetchedData.ResignationDateManager = inputData.ResignationDateManager;
                fetchedData.LastWorkingDateManager = inputData.LastWorkingDateManager;
                fetchedData.NoticePeriodWaiveOff = inputData.NoticePeriodWaiveOff;
                fetchedData.EligibleToHire = inputData.EligibleToHire;
                if (inputData.Document != null)
                {
                    fetchedData.Document = inputData.Document;
                    fetchedData.DocumentName = inputData.DocumentName;
                    fetchedData.DocumentExtension = inputData.DocumentExtension;
                }
                fetchedData.ManagerRemarks = inputData.ManagerRemarks;
                fetchedData.ManagerApproval = inputData.ManagerApproval;
                fetchedData.ReasonForLeavingManager = inputData.ReasonForLeavingManager;
                fetchedData.ManagerApprovalDate = System.DateTime.Now;
                var saveRes = await _unitOfWork.EmployeeExitResignation.UpdateAsync(fetchedData);
                if (saveRes)
                {
                    _unitOfWork.Save();

                    if (fetchedData.ManagerApproval == -1)
                    {
                        ExitEmailDTO exitEmailDTO = new ExitEmailDTO();
                        exitEmailDTO.ResignationListId = fetchedData.ResignationListId;
                        await SendMail_ResignationRequestRejectedByManager(exitEmailDTO);
                    }
                    else if (fetchedData.ManagerApproval == 1)
                    {
                        ExitEmailDTO exitEmailDTO = new ExitEmailDTO();
                        exitEmailDTO.ResignationListId = fetchedData.ResignationListId;
                        await SendMail_ResignationRequestReceivedByHR_ManagerApproval(exitEmailDTO);
                    }
                    return Ok("Success");
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Unable to find the record");
            }
        }
        else
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Unable to find the record");
        }
    }
    public async Task<ActionResult> SaveResignationDetailsByAdmin(EmployeeExitResignationDTO inputData)
    {
        if (inputData.ResignationListId > 0)
        {
            EmployeeExitResignation? fetchedData = _unitOfWork.EmployeeExitResignation.GetTableDataExec<EmployeeExitResignation>("select * from [dbo].[EmployeeExitResignation] where resignationlistid=" + inputData.ResignationListId).Result.FirstOrDefault();
            if (fetchedData != null)
            {

                fetchedData.ResignationDateAdmin = inputData.ResignationDateAdmin;
                fetchedData.LastWorkingDateAdmin = inputData.LastWorkingDateAdmin;
                fetchedData.NoticePeriodWaiveOffAdmin = inputData.NoticePeriodWaiveOffAdmin;
                fetchedData.EligibleToHireAdmin = inputData.EligibleToHireAdmin;
                fetchedData.ActivateExitInterview = inputData.ActivateExitInterview;
                fetchedData.ClearanceByPass = inputData.ClearanceByPass;
                if (inputData.DocumentAdmin != null)
                {
                    fetchedData.DocumentAdmin = inputData.DocumentAdmin;
                    fetchedData.DocumentNameAdmin = inputData.DocumentNameAdmin;
                    fetchedData.DocumentExtensionAdmin = inputData.DocumentExtensionAdmin;
                }
                fetchedData.LWDPolicy = inputData.LWDPolicy;
                fetchedData.AdminRemarks = inputData.AdminRemarks;
                fetchedData.AdminApproval = inputData.AdminApproval;
                fetchedData.ReasonForLeavingAdmin = inputData.ReasonForLeavingAdmin;
                fetchedData.AdminApprovalDate = System.DateTime.Now;

                if (fetchedData.InterviewStatus == 0 && fetchedData.ActivateExitInterview == true)
                {
                    fetchedData.InterviewStatus = 1;
                }

                var res = await _unitOfWork.EmployeeExitResignation.UpdateAsync(fetchedData);
                _unitOfWork.Save();
                ExitEmailDTO exitEmailDTO = new ExitEmailDTO();
                exitEmailDTO.ResignationListId = fetchedData.ResignationListId;
                if (res)
                {
                    List<AppMessage> appMessageRes = await _unitOfWork.AppMessage.GetFilterAll(x => x.MessageType == "ExitInterview" && x.TargetEmployeeId == inputData.EmployeeId && x.IsActive == true);
                    if (inputData.ActivateExitInterview == true && inputData.AdminApproval == 1)
                    {
                        if (!(appMessageRes != null && appMessageRes.Count() > 0))
                        {
                            string encEmployeeId = CommonHelper.EncryptURLHTML(inputData.EmployeeId.ToString());
                            AppMessage appMessage = new AppMessage();
                            appMessage.MessageSubject = "Exit Interview";
                            appMessage.MessageText = "";
                            appMessage.MessageHTML = "<span>Please click here for exit interview</span>";
                            appMessage.RedirectLink = "/ExitManagement/EmployeeExitInterview";
                            appMessage.MessageType = "ExitInterview";
                            appMessage.ReferenceId = inputData.EmployeeId;
                            appMessage.TargetEmployeeId = inputData.EmployeeId;
                            appMessage.IsViewed = false;
                            appMessage.CreatedOn = DateTime.Now;
                            //appMessage.CreatedBy = inputData.;
                            appMessage.IsActive = true;

                            int insertedAppMessage = await _unitOfWork.AppMessage.AddAsync(appMessage);
                            _unitOfWork.Save();
                        }

                        await SendMail_ExitInterviewEmailToEmployee(exitEmailDTO);

                    }
                    else
                    {
                        if (appMessageRes != null && appMessageRes.Count() > 0)
                        {
                            string encEmployeeId = CommonHelper.EncryptURLHTML(inputData.EmployeeId.ToString());
                            AppMessage appMessage = appMessageRes.FirstOrDefault();
                            appMessage.IsActive = false;
                            await _unitOfWork.AppMessage.UpdateAsync(appMessage);
                            _unitOfWork.Save();
                        }
                    }

                    if (fetchedData.AdminApproval == 1)
                    {
                        await SendMail_ResignationRequestReceivedByEmployee_HRApproval(exitEmailDTO);
                    }
                    else if (fetchedData.AdminApproval == -1)
                    {
                        await SendMail_ResignationRequestRejectedByAdmin(exitEmailDTO);
                    }

                }
                return Ok("Success");
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Unable to find the record");
            }
        }
        else
        {
            inputData.TicketId = CommonHelper.CreateTicket("Exit", "");
            var res = await _unitOfWork.EmployeeExitResignation.AddAsync(_mapper.Map<EmployeeExitResignation>(inputData));
            inputData.ResignationListId = res;
            _unitOfWork.Save();

            ExitEmailDTO exitEmailDTO = new ExitEmailDTO();
            exitEmailDTO.ResignationListId = inputData.ResignationListId;

            if (res > 0)
            {
                List<AppMessage> appMessageRes = await _unitOfWork.AppMessage.GetFilterAll(x => x.MessageType == "ExitInterview" && x.TargetEmployeeId == inputData.EmployeeId && x.IsActive == true);
                if (inputData.ActivateExitInterview == true && inputData.AdminApproval == 1)
                {
                    if (!(appMessageRes != null && appMessageRes.Count() > 0))
                    {
                        string encEmployeeId = CommonHelper.EncryptURLHTML(inputData.EmployeeId.ToString());
                        AppMessage appMessage = new AppMessage();
                        appMessage.MessageSubject = "Exit Interview";
                        appMessage.MessageText = "";
                        appMessage.MessageHTML = "<span>Please click here for exit interview</span>";
                        appMessage.RedirectLink = "/ExitManagement/EmployeeExitInterview";
                        appMessage.MessageType = "ExitInterview";
                        appMessage.ReferenceId = inputData.EmployeeId;
                        appMessage.TargetEmployeeId = inputData.EmployeeId;
                        appMessage.IsViewed = false;
                        appMessage.CreatedOn = DateTime.Now;
                        //appMessage.CreatedBy = inputData.;
                        appMessage.IsActive = true;

                        int insertedAppMessage = await _unitOfWork.AppMessage.AddAsync(appMessage);
                        _unitOfWork.Save();
                    }

                    await SendMail_ExitInterviewEmailToEmployee(exitEmailDTO);

                }
                else
                {
                    if (appMessageRes != null && appMessageRes.Count() > 0)
                    {
                        string encEmployeeId = CommonHelper.EncryptURLHTML(inputData.EmployeeId.ToString());
                        AppMessage appMessage = appMessageRes.FirstOrDefault();
                        appMessage.IsActive = false;
                        await _unitOfWork.AppMessage.UpdateAsync(appMessage);
                        _unitOfWork.Save();
                    }
                }
            }
            return Ok("Success");
        }
    }
    public async Task<ActionResult> SaveEmployeeExitInterviewFormComponent(EmployeeExitInterViewFormMasterDTO inputDTO)
    {
        if (inputDTO.EmployeeExitInterViewFormMasterId > 0)
        {
            Expression<Func<EmployeeExitInterViewFormMaster, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.FormName == inputDTO.FormName && a.IsActive == true && a.EmployeeExitInterViewFormMasterId != inputDTO.EmployeeExitInterViewFormMasterId;
            if (!_unitOfWork.EmployeeExitInterViewFormMaster.Exists(expression))
            {
                _unitOfWork.EmployeeExitInterViewFormMaster.Update(_mapper.Map<EmployeeExitInterViewFormMaster>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "A form with the same exists");
            }
        }
        else
        {
            Expression<Func<EmployeeExitInterViewFormMaster, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.FormName == inputDTO.FormName && a.IsActive == true;
            if (!_unitOfWork.EmployeeExitInterViewFormMaster.Exists(expression))
            {
                _unitOfWork.EmployeeExitInterViewFormMaster.AddAsync(_mapper.Map<EmployeeExitInterViewFormMaster>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "A form with the same exists");
            }
        }
    }


    public async Task<ExitViewModel> EmployeeExitList(ExitViewModel exitVM)
    {
        string sQuery = string.Empty;
        List<EmployeeExitResignationDTO> eerDTOList = new List<EmployeeExitResignationDTO>();
        sQuery = $"SELECT COALESCE(a.NoticePeriod,b.NoticePeriod) NoticePeriod FROM EmployeeMaster a INNER JOIN UnitMaster b ON a.UnitId=b.UnitId AND a.EmployeeId={exitVM.ResignationDetails.EmployeeId}";
        exitVM.ResignationDetails.ResignationDate = DateTime.Now;
        eerDTOList = _mapper.Map<List<EmployeeExitResignationDTO>>(await _unitOfWork.EmployeeExitResignation.GetQueryAll(sQuery));
        exitVM.ResignationDetails.NoticePeriod = Convert.ToInt32((eerDTOList.Select(x => x.NoticePeriod).FirstOrDefault().ToString()));
        exitVM.ResignationDetails.LastWorkingDate = exitVM.ResignationDetails.ResignationDate.Value.AddDays(Convert.ToDouble(exitVM.ResignationDetails.NoticePeriod));
        exitVM.ResignationList = await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeExitResignationDTO?>(sQuery);
        if (exitVM.ResignationDetails.EmployeeId == 0)
            sQuery = $"SELECT TOP 1 em.UnitId,rl.ResignationListId ,rl.EmployeeId ,rl.EmployeeCode ,em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName ,rl.ResignationDate ,rl.LastWorkingDate ,rl.NoticePeriod ,rl.ReasonForLeaving ,rl.EmployeeComments FROM EmployeeExitResignation rl JOIN EmployeeMaster em on rl.EmployeeId = em.EmployeeId JOIN JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId JOIN DepartmentMaster dm on em.DepartmentId = dm.DepartmentId WHERE em.UnitId={exitVM.ResignationDetails.UnitId} AND IsResignationRolledBack=0 ORDER BY EmployeeName ASC";
        else if (exitVM.ResignationDetails.EmployeeId != 0)
            sQuery = $"SELECT TOP 1 em.UnitId,rl.ResignationListId ,rl.EmployeeId ,rl.EmployeeCode ,em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName ,rl.ResignationDate ,rl.LastWorkingDate ,rl.NoticePeriod ,rl.ReasonForLeaving ,rl.EmployeeComments FROM EmployeeExitResignation rl JOIN EmployeeMaster em on rl.EmployeeId = em.EmployeeId JOIN JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId JOIN DepartmentMaster dm on em.DepartmentId = dm.DepartmentId WHERE  em.EmployeeId={exitVM.ResignationDetails.EmployeeId} AND IsResignationRolledBack=0 ORDER BY EmployeeName ASC";
        try
        {
            exitVM.ResignationList = await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeExitResignationDTO?>(sQuery);
        }
        catch (Exception ex) { }
        return exitVM;
    }

    public async Task<ExitViewModel> EmployeeExitInfo(int employeeId)
    {
        string sQuery = string.Empty;
        ExitViewModel exitVM = new ExitViewModel();
        if (exitVM.ResignationDetails.EmployeeId != 0)
            sQuery = $"SELECT TOP 1 em.UnitId,rl.ResignationListId,rl.TicketId ,rl.EmployeeId ,rl.EmployeeCode ,em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName ,rl.ResignationDate ,rl.LastWorkingDate ,rl.NoticePeriod ,rl.ReasonForLeaving ,rl.EmployeeComments,ManagerApproval,AdminApproval,ResignationDateManager,LastWorkingDateManager,ResignationDateAdmin,LastWorkingDateAdmin FROM EmployeeExitResignation rl JOIN EmployeeMaster em on rl.EmployeeId = em.EmployeeId JOIN JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId JOIN DepartmentMaster dm on em.DepartmentId = dm.DepartmentId WHERE  rl.EmployeeId={employeeId}  AND (IsResignationRolledBack=0 and ManagerApproval <> -1  AND AdminApproval <> -1) ORDER BY EmployeeName ASC";
        try
        {
            exitVM.ResignationDetails = _mapper.Map<EmployeeExitResignationDTO>((await _unitOfWork.EmployeeExitResignation.GetQueryAll(sQuery)).FirstOrDefault());
        }
        catch (Exception ex) { }
        if (exitVM.ResignationDetails == null)
        {
            exitVM.ResignationDetails = new EmployeeExitResignationDTO();
        }
        if (exitVM.ResignationDetails.ResignationListId == 0)
        {
            EmployeeExitResignationDTO resigDTO = new EmployeeExitResignationDTO();
            List<EmployeeExitResignationDTO> eerDTOList = new List<EmployeeExitResignationDTO>();
            sQuery = $"SELECT COALESCE(a.NoticePeriod,b.NoticePeriod) NoticePeriod FROM EmployeeMaster a INNER JOIN UnitMaster b ON a.UnitId=b.UnitId AND a.EmployeeId={employeeId}";
            exitVM.ResignationDetails.ResignationDate = DateTime.Now;
            eerDTOList = _mapper.Map<List<EmployeeExitResignationDTO>>(await _unitOfWork.EmployeeExitResignation.GetQueryAll(sQuery));
            exitVM.ResignationDetails.NoticePeriod = Convert.ToInt32((eerDTOList.Select(x => x.NoticePeriod).FirstOrDefault().ToString()));
            exitVM.ResignationDetails.LastWorkingDate = exitVM.ResignationDetails.ResignationDate.Value.AddDays(Convert.ToDouble(exitVM.ResignationDetails.NoticePeriod));
        }

        return exitVM;
    }

    public async Task<string> ValidateExitInfo(ExitViewModel exitVM)
    {
        string sMsg = string.Empty;
        ExitViewModel exitViewModel = new ExitViewModel();
        //Validate all required controls
        return sMsg;
    }


    public async Task<IActionResult> GetResignationDetails(EmployeeExitResignationDTO resignationListDTO)
    {
        try
        {
            List<EmployeeExitResignationDTO> outputModel = new List<EmployeeExitResignationDTO>();

            string sQuery = "select rl.*, em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName from EmployeeExitResignation rl join EmployeeMaster em on rl.EmployeeId = em.EmployeeId join JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId join DepartmentMaster dm on em.DepartmentId = dm.DepartmentId where ResignationListId=" + resignationListDTO.ResignationListId + " order by EmployeeName asc";

            //var resignationDTO = _unitOfWork.PayrollEarningComponent.GetTableData<ResignationListDTO>(sQuery).Result.FirstOrDefault();
            EmployeeExitResignationDTO? resignationDTO = _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeExitResignationDTO?>(sQuery).Result.FirstOrDefault();
            //payrollEarningVM.PayrollEarningComponentList = _mapper.Map<List<PayrollEarningComponentDTO>>(await _unitOfWork.PayrollEarningComponent.GetTableData<PayrollEarningComponentDTO>(sQuery));

            //payrollEarningVM.PayrollEarningComponentList.Skip(offset * limit).Take(limit).ToList();
            //}
            return Ok(resignationDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<IActionResult> GetExitClearanceInfo(ExitViewModel exitVM)
    {
        try
        {
            string sQuery = $"SELECT a.EmployeeId,a.EmployeeCode,a.EmployeeName,b.DepartmentName DepartmentName,b.DepartmentId,c.JobTitle JobTitleName,c.JobTitleId,e.CompanyName,d.UnitName," +
                $"f.EmployeeName ManagerName, f.EmployeeId ManagerId, g.ResignationDate,g.ResignationListId FROM EmployeeMaster a INNER JOIN DepartmentMaster b ON a.DepartmentId = b.DepartmentId AND b.IsActive = 1 " +
                $" INNER JOIN JobTitleMaster c ON c.JobTitleId = a.JobTitleId AND c.IsActive = 1 INNER JOIN UnitMaster d ON d.UnitId = a.UnitId AND d.IsActive = 1 " +
                $" INNER JOIN Client e ON e.ClientId = d.ClientId AND e.IsActive = 1 INNER JOIN EmployeeMaster f ON f.EmployeeId = a.ManagerId " +
                $" INNER JOIN EmployeeExitResignation g ON a.EmployeeId = g.EmployeeId WHERE a.EmployeeId = {exitVM.EmployeeId}  and AdminApproval=1";
            exitVM.EmployeeMasterInfo = (await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeMasterDTO>(sQuery)).FirstOrDefault();
            sQuery = $"SELECT distinct a.EmployeeName OwnerName,d.EmployeeClearanceHeaderId,b.ClearanceMappingId,b.ClearanceBy OwnerId,b.EmployeeId,b.DepartmentId,e.DepartmentName,e.CreatedOn,e.ModifiedOn,c.JobTitleId,c.JobTitle JobTitleName,d.ClearanceStatus,d.FinalRemark " +
                $" FROM EmployeeMaster a INNER JOIN EmployeeExitClearance b ON a.EmployeeId=b.ClearanceBy INNER JOIN JobTitleMaster c ON c.JobTitleId=a.JobTitleId " +
                $" LEFT JOIN EmployeeExitClearanceHeader d ON d.OwnerId=a.EmployeeId  AND b.ClearanceMappingId=d.ClearanceMappingId INNER JOIN DepartmentMaster e ON e.DepartmentId=b.DepartmentId " +
                $" INNER JOIN ExitClearanceAssetMapping f ON f.ClearanceMappingId=b.ClearanceMappingId WHERE b.EmployeeId = {exitVM.EmployeeId} ";
            exitVM.EmployeeExitClearanceHeaderList = await _unitOfWork.EmployeeExitClearanceHeader.GetTableData<EmployeeExitClearanceHeaderDTO?>(sQuery);
            sQuery = $"SELECT a.ResourceId AssetId, a.ResourceName AssetName, c.EmployeeId,c.DepartmentId,f.DepartmentName,COALESCE(g.PrimaryClearancePerson, g.SecondaryClearancePerson) OwnerId, " +
                $" h.EmployeeName OwnerName, b.ClearanceMappingId,e.EmployeeClearanceDetailId,e.AssetClearanceStatus,e.Remark,e.RecoveryStatus,e.RecoveryAmount,e.CreatedOn,e.ModifiedOn  FROM  ResourceMaster a INNER JOIN ExitClearanceAssetMapping b " +
                $" ON a.ResourceId = b.AssetId INNER JOIN EmployeeExitClearance c ON c.ClearanceMappingId = b.ClearanceMappingId  AND c.IsActive=1 LEFT JOIN EmployeeExitClearanceHeader d " +
                $" ON d.ClearanceMappingId = c.ClearanceMappingId AND c.IsActive=1 LEFT JOIN EmployeeExitClearanceDetail e ON e.EmployeeClearanceHeaderID = d.EmployeeClearanceHeaderID AND e.ClearanceMappingId=b.ClearanceMappingId AND e.AssetId=b.AssetId AND e.IsActive = 1 AND d.IsActive = 1 " +
                $" INNER JOIN DepartmentMaster f ON f.DepartmentId = c.DepartmentId INNER JOIN ExitClearanceMapping g ON g.ClearanceMappingId=b.ClearanceMappingId " +
                $" INNER JOIN EmployeeMaster h ON h.EmployeeId = COALESCE(g.PrimaryClearancePerson, g.SecondaryClearancePerson)  AND h.IsActive=1  WHERE c.EmployeeId =  {exitVM.EmployeeId} ";
            exitVM.EmployeeExitClearanceDetailList = await _unitOfWork.EmployeeExitClearanceDetail.GetTableData<EmployeeExitClearanceDetailDTO?>(sQuery);
            return Ok(exitVM);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveExitClearanceInfo(ExitViewModel exitVM)
    {
        List<EmployeeExitClearanceHeaderDTO> headerList = new List<EmployeeExitClearanceHeaderDTO>();
        headerList = exitVM.EmployeeExitClearanceHeaderList;
        List<EmployeeExitClearanceDetailDTO> detailList = exitVM.EmployeeExitClearanceDetailList;
        detailList = exitVM.EmployeeExitClearanceDetailList;
        exitVM.DisplayMessage = await _unitOfWork.EmployeeExitClearanceHeader.SaveExitClearanceInfo(_mapper.Map<List<EmployeeExitClearanceHeader>>(headerList), _mapper.Map<List<EmployeeExitClearanceDetail>>(detailList));
        string sQuery = @"update EmployeeExitResignation set ClearanceStatus=2 WHERE EmployeeId=" + exitVM.EmployeeExitClearanceHeaderList[0].EmployeeId;
        bool isSuccess = await _unitOfWork.EmployeeExitResignation.RunSQLCommand(sQuery);
        if (isSuccess)
        {
        }

        return Ok(exitVM);
    }
    public async Task<IActionResult> GetExitClearanceAuthority(ExitViewModel exitVM)
    {
        try
        {
            string sQuery = "SELECT a.ClearanceMappingId,a.PrimaryClearancePerson,a.SecondaryClearancePerson,a.DepartmentId,a.UnitId,a.CreatedBy,a.ModifiedBy,a.IsActive," +
                " b.EmployeeName PrimaryClearancePersonName, c.EmployeeName SecondaryClearancePersonName,d.DepartmentName " +
                " FROM ExitClearanceMapping a INNER JOIN EmployeeMaster b ON a.PrimaryClearancePerson=b.EmployeeId AND a.IsActive=1" +
                " INNER JOIN EmployeeMaster c ON a.SecondaryClearancePerson=c.EmployeeId AND c.IsActive=1" +
                " INNER JOIN DepartmentMaster d ON a.DepartmentId=d.DepartmentId AND a.UnitId=d.UnitId ";
            exitVM.ExitClearanceMappingList = await _unitOfWork.ExitClearanceMapping.GetTableData<ExitClearanceMappingDTO?>(sQuery);
            sQuery = $"SELECT a.ClearanceMappingId FROM EmployeeExitClearance a WHERE a.EmployeeId={exitVM.EmployeeId} AND a.IsActive=1";
            exitVM.EmployeeExitClearanceList = await _unitOfWork.EmployeeExitClearance.GetTableData<EmployeeExitClearanceDTO?>(sQuery);
            return Ok(exitVM);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveEmployeeExitClearanceAuthorities(ExitViewModel exitVM)
    {
        try
        {
            //Delete Mappings
            string sQuery = @"DELETE FROM EmployeeExitClearance WHERE EmployeeId = " + exitVM.EmployeeId + " AND IsActive=1";
            bool isSuccess = await _unitOfWork.EmployeeExitClearance.RunSQLCommand(sQuery);

            //Delete Alerts
            sQuery = @"DELETE FROM AppMessages WHERE ReferenceId = " + exitVM.EmployeeId + " AND IsActive=1 and MessageType='ExitClearancePending'";
            bool isSuccessAlerts = await _unitOfWork.EmployeeExitClearance.RunSQLCommand(sQuery);

            if (isSuccess)
            {
                sQuery = @"
                        insert EmployeeExitClearance(ClearanceMappingId,EmployeeId,ClearanceBy,UnitId,DepartmentId,ModifiedBy,ModifiedOn,IsActive)
                        values(@ClearanceMappingId, @EmployeeId,@ClearanceBy,@UnitId,@DepartmentId,@ModifiedBy,@ModifiedOn,@IsActive)";

                isSuccess = await _unitOfWork.EmployeeExitClearance.ExecuteListData<EmployeeExitClearance>(_mapper.Map<List<EmployeeExitClearance>>(exitVM.EmployeeExitClearanceList), sQuery);

                exitVM.DisplayMessage = isSuccess ? "Success" : "Fail";
            }

            List<EmployeeMasterDTO> empResignedEmployeeList = await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeMasterDTO>($"Select EmployeeCode,EmployeeName\r\n,(Select DepartmentName from DepartmentMaster dm where dm.DepartmentId=em.DepartmentId) DepartmentName\r\n,(Select Top 1 LastWorkingDate from [dbo].[EmployeeExitResignation] eer where eer.EmployeeId=em.EmployeeId  order by ResignationListId desc)LastWorkingDate\r\nfrom EmployeeMaster em where EmployeeId={exitVM.EmployeeId}");
            EmployeeMasterDTO? em = new EmployeeMasterDTO();
            string clearanceLink = "";
            if (empResignedEmployeeList != null && empResignedEmployeeList.Count() > 0)
            {
                em = empResignedEmployeeList.FirstOrDefault();
                if (em != null)
                {
                    clearanceLink = $"{(em.EmployeeCode == null ? "" : em.EmployeeCode)}/{(em.EmployeeName == null ? "" : em.EmployeeName)}/{(em.DepartmentName == null ? "" : em.DepartmentName)}/LWD {(em.LastWorkingDate == null ? "" : ((DateTime)em.LastWorkingDate).ToString("dd-MMM-yyyy"))}";
                }
            }

            string sQury = @"update EmployeeExitResignation set ClearanceStatus=1 WHERE EmployeeId=" + exitVM.EmployeeId;
            bool isSuc = await _unitOfWork.EmployeeExitResignation.RunSQLCommand(sQury);
            if (isSuc)
            {
                if (exitVM != null && exitVM.EmployeeExitClearanceList != null)
                {
                    List<AppMessage> appMessages = new List<AppMessage>();
                    foreach (var item in exitVM.EmployeeExitClearanceList)
                    {
                        List<AppMessage> appMessageRes = await _unitOfWork.AppMessage.GetFilterAll(x => x.MessageType == "ExitClearancePending" && x.TargetEmployeeId == item.ClearanceBy && x.ReferenceId == item.EmployeeId && x.IsActive == true);
                        if (appMessageRes != null && appMessageRes.Count() > 0)
                        {

                        }
                        else
                        {
                            string encEmployeeId = CommonHelper.EncryptURLHTML(item.EmployeeId.ToString());
                            AppMessage appMessage = new AppMessage();
                            appMessage.MessageSubject = "Clearance Pending";
                            appMessage.MessageText = $"{clearanceLink}";
                            appMessage.MessageHTML = $"{clearanceLink}";
                            appMessage.RedirectLink = $"/ExitManagement/ExitClearance/{encEmployeeId}";
                            appMessage.MessageType = "ExitClearancePending";
                            appMessage.ReferenceId = item.EmployeeId;
                            appMessage.TargetEmployeeId = item.ClearanceBy;
                            appMessage.IsViewed = false;
                            appMessage.CreatedOn = DateTime.Now;
                            appMessage.CreatedBy = (exitVM != null && exitVM.EmployeeMasterInfo != null) ? exitVM.EmployeeMasterInfo.EmployeeId : 0;
                            appMessage.IsActive = true;

                            //appMessages.Add(appMessage);
                            int insertedAppMessage = await _unitOfWork.AppMessage.AddAsync(appMessage);
                            _unitOfWork.Save();
                        }
                    }

                    //Send Email to Clearance Owners
                    ExitEmailDTO exitEmailDTO = new ExitEmailDTO();
                    exitEmailDTO.EmployeeId = exitVM?.EmployeeId;
                    await SendMail_ClearanceRequestEmail(exitEmailDTO);

                }
            }
            //if (isSuccess)
            //{
            //}


            return Ok(exitVM);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<IActionResult> GetExitInterviewFormComponent(int unitId)
    {
        try
        {
            string sQuery = "select * from EmployeeExitInterViewFormMaster where unitId=" + unitId;
            //EmployeeExitInterViewFormMasterDTO dto = _mapper.Map<EmployeeExitInterViewFormMasterDTO>(_unitOfWork.EmployeeExitInterViewFormMaster.GetWithRawSql(sQuery, null).FirstOrDefault());

            EmployeeExitInterViewFormMasterDTO? dto = _mapper.Map<EmployeeExitInterViewFormMasterDTO>(_unitOfWork.EmployeeExitInterViewFormMaster.GetWithRawSql(sQuery).FirstOrDefault());
            //payrollEarningVM.PayrollEarningComponentList = _mapper.Map<List<PayrollEarningComponentDTO>>(await _unitOfWork.PayrollEarningComponent.GetTableData<PayrollEarningComponentDTO>(sQuery));

            //payrollEarningVM.PayrollEarningComponentList.Skip(offset * limit).Take(limit).ToList();
            //}
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<ActionResult> SaveClearanceMapping(ExitClearanceMappingDTO inputDTO)
    {


        if (inputDTO.ClearanceMappingId > 0)
        {
            List<ExitClearanceMapping> ecm = await _unitOfWork.ExitClearanceMapping.GetQueryAll("select * from  [dbo].[ExitClearanceMapping] where unitId=" + inputDTO.UnitId + " and departmentid=" + inputDTO.DepartmentId + " and clearancemappingid!=" + inputDTO.ClearanceMappingId + " and isactive=1");
            if (ecm.Count == 0)
            {
                _unitOfWork.ExitClearanceMapping.UpdateAsync(_mapper.Map<ExitClearanceMapping>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry");
            }
        }
        else
        {
            List<ExitClearanceMapping> ecm = await _unitOfWork.ExitClearanceMapping.GetQueryAll("select * from  [dbo].[ExitClearanceMapping] where unitId=" + inputDTO.UnitId + " and departmentid=" + inputDTO.DepartmentId + " and isactive=1");
            if (ecm.Count == 0)
            {
                await _unitOfWork.ExitClearanceMapping.AddAsync(_mapper.Map<ExitClearanceMapping>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry");
            }
        }
    }

    public async Task<ActionResult> GetClearanceMappingById(ExitClearanceMappingDTO inputDTO)
    {
        try
        {
            List<EmployeeExitResignationDTO> outputModel = new List<EmployeeExitResignationDTO>();

            ExitClearanceMappingDTO exitClearanceMappingDTO = _mapper.Map<ExitClearanceMappingDTO>(await _unitOfWork.ExitClearanceMapping.FindByIdAsync(inputDTO.ClearanceMappingId));
            //var exitClearanceMappingDTO = _unitOfWork.ExitClearanceMapping.FindByIdAsync(inputDTO.ClearanceMappingId);

            //string sQuery = "select ecm.ClearanceMappingId,ecm.PrimaryClearancePerson,ecm.SecondaryClearancePerson,ecm.DepartmentId,em.employeename as PrimaryClearancePersonName,em1.employeename as SecondaryClearancePersonName,dm.DepartmentName as DepartmentName from  [dbo].[ExitClearanceMapping] ecm  join employeemaster em on ecm.PrimaryClearancePerson=em.employeeid join employeemaster em1 on ecm.SecondaryClearancePerson=em1.employeeid join departmentmaster dm on ecm.DepartmentId=dm.departmentId where ecm.isactive=1 and ecm.unitid=" + unitId + "";

            //List<ExitClearanceMappingDTO> dto = await _unitOfWork.EmployeeExitResignation.GetTableData<ExitClearanceMappingDTO>(sQuery);
            return Ok(exitClearanceMappingDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }
    public async Task<ActionResult> DeleteClearanceMappingById(ExitClearanceMappingDTO inputDTO)
    {
        try
        {
            List<EmployeeExitResignationDTO> outputModel = new List<EmployeeExitResignationDTO>();
            ExitClearanceMapping exitClearanceMapping = await _unitOfWork.ExitClearanceMapping.FindByIdAsync(inputDTO.ClearanceMappingId);



            exitClearanceMapping.IsActive = false;
            _unitOfWork.ExitClearanceMapping.UpdateAsync(exitClearanceMapping);
            _unitOfWork.Save();
            return Ok("success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Deleteting Clearance Data {nameof(DeleteClearanceMappingById)}");
            throw;
        }
    }
    public async Task<IActionResult> GetExitClearanceMappingTable(int unitId)
    {
        try
        {
            List<EmployeeExitResignationDTO> outputModel = new List<EmployeeExitResignationDTO>();

            string sQuery = "select ecm.ClearanceMappingId,ecm.PrimaryClearancePerson,ecm.SecondaryClearancePerson,ecm.DepartmentId,em.employeename as PrimaryClearancePersonName,em1.employeename as SecondaryClearancePersonName,dm.DepartmentName as DepartmentName from  [dbo].[ExitClearanceMapping] ecm  join employeemaster em on ecm.PrimaryClearancePerson=em.employeeid join employeemaster em1 on ecm.SecondaryClearancePerson=em1.employeeid join departmentmaster dm on ecm.DepartmentId=dm.departmentId where ecm.isactive=1 and ecm.unitid=" + unitId + "";

            List<ExitClearanceMappingDTO> dto = await _unitOfWork.EmployeeExitResignation.GetTableData<ExitClearanceMappingDTO>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }
    public async Task<IActionResult> MarkEmployeeResigned()
    {
        try
        {
            var res = await _unitOfWork.EmployeeExitResignation.ExecuteQuery("exec MarkEmployeeResigned", null);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }
    public async Task<int> DepartmentIdAssignedToOwner(int EmployeeId, int ClearanceBy)
    {
        try
        {
            string query = $"Select DepartmentId from [EmployeeExitClearance] where EmployeeId={EmployeeId} and ClearanceBy={ClearanceBy} and IsActive=1";

            var res = await _unitOfWork.EmployeeExitResignation.GetTableData<int>(query);
            if (res != null && res.Count > 0)
            {
                return res.FirstOrDefault();
            }
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }



    public async Task<IActionResult> GetExitClearanceAssetMappingTable(ExitClearanceAssetMappingDTO inputDTO)
    {
        try
        {
            List<ExitClearanceAssetMappingDTO> outputModel = new List<ExitClearanceAssetMappingDTO>();

            string sQuery = "select ResourceId AssetId,ResourceName,isnull((select top 1 1 from ExitClearanceAssetMapping ecam where ecam.AssetId=rm.ResourceId and ecam.ClearanceMappingId=" + inputDTO.ClearanceMappingId + " and isactive=1),0)IsChecked from resourcemaster rm where rm.unitid=" + inputDTO.UnitId + " and rm.isactive=1";

            List<ExitClearanceAssetMappingDTO> dto = await _unitOfWork.EmployeeExitResignation.GetTableData<ExitClearanceAssetMappingDTO>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveAssetMapping(ExitViewModel inputDTO)
    {
        try
        {





            if (inputDTO != null)
            {
                if (inputDTO.ExitClearanceAssetMappingList != null)
                {
                    if (inputDTO.ExitClearanceAssetMappingList.Count > 0)
                    {
                        string sQuery = @"DELETE FROM ExitClearanceAssetMapping WHERE ClearanceMappingId=" + inputDTO.ExitClearanceAssetMappingList.FirstOrDefault().ClearanceMappingId;
                        bool isSuccess = await _unitOfWork.EmployeeExitClearance.RunSQLCommand(sQuery);
                        if (isSuccess)
                        {
                            foreach (var entry in inputDTO.ExitClearanceAssetMappingList)
                            {
                                ExitClearanceAssetMapping ecam = new ExitClearanceAssetMapping();
                                ecam.AssetId = entry.AssetId;
                                ecam.ClearanceMappingId = entry.ClearanceMappingId;
                                ecam.IsActive = true;

                                await _unitOfWork.ExitClearanceAssetMapping.AddAsync(ecam);
                                _unitOfWork.Save();
                            }
                            return Ok("Success");
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Unable to process the data");
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "No Data Found");
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No Data Found");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "No Data Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<ActionResult> ResignationDetailsByEmployeeId(int EmployeeId)
    {
        if (EmployeeId > 0)
        {
            var res = await _unitOfWork.EmployeeExitResignation.GetTableDataExec<EmployeeExitResignationDTO>(@"select * from [dbo].[EmployeeExitResignation] where AdminApproval=1 and  EmployeeId=" + EmployeeId);
            if (res != null)
            {
                EmployeeExitResignationDTO? fetchedData = res.FirstOrDefault();
                return Ok(fetchedData);
            }
            else
            {
                return BadRequest("No Data found");
            }

        }
        else
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Unable to find the record");
        }
    }


    public async Task<ActionResult> UpdateEmployeeExitInterviewData(EmployeeExitResignationDTO inputData)
    {
        if (inputData.EmployeeId > 0)
        {
            EmployeeExitResignation? fetchedData = _unitOfWork.EmployeeExitResignation.GetTableDataExec<EmployeeExitResignation>("select * from [dbo].[EmployeeExitResignation] where EmployeeId=" + inputData.EmployeeId).Result.FirstOrDefault();
            if (fetchedData != null)
            {
                fetchedData.ExitInterviewData = inputData.ExitInterviewData;
                fetchedData.InterviewStatus = 2;
                fetchedData.ExitInterviewSubmissionDate = DateTime.Now;
                await _unitOfWork.EmployeeExitResignation.UpdateAsync(fetchedData);
                _unitOfWork.Save();
                return Ok("Success");
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "The Person is not eligible for Exit Interview");
            }
        }
        else
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Unable to find the record");
        }
    }

    public async Task<IActionResult> GetInterviewResponsesList(InterviewResponses inputDTO)
    {
        try
        {
            string sQuery = "";
            List<EmployeeExitResignationDTO> outputModel = new List<EmployeeExitResignationDTO>();
            if (inputDTO.LastWorkingDateFrom != null && inputDTO.LastWorkingDateTo != null)
            {
                sQuery = "select rl.*, em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName,em.DOJ,(Select a.Location from WorkLocationMaster a where a.WorkLocationId=em.WorkLocationId)Location from EmployeeExitResignation rl join EmployeeMaster em on rl.EmployeeId = em.EmployeeId join JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId join DepartmentMaster dm on em.DepartmentId = dm.DepartmentId where rl.unitid=" + inputDTO.UnitId + " and rl.LastworkingDateAdmin >= '" + ((DateTime)inputDTO.LastWorkingDateFrom).ToString("dd-MMM-yyyy") + "' and rl.LastworkingDateAdmin < '" + ((DateTime)inputDTO.LastWorkingDateTo).ToString("dd-MMM-yyyy") + "' order by LastworkingDateAdmin asc";
            }
            else if (inputDTO.LastWorkingDateFrom != null)
            {
                sQuery = "select rl.*, em.EmployeeName ,jtm.JobTitle ,dm.DepartmentNamee,m.DOJ,(Select a.Location from WorkLocationMaster a where a.WorkLocationId=em.WorkLocationId)Location from EmployeeExitResignation rl join EmployeeMaster em on rl.EmployeeId = em.EmployeeId join JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId join DepartmentMaster dm on em.DepartmentId = dm.DepartmentId where rl.unitid=" + inputDTO.UnitId + " and rl.LastworkingDateAdmin >= '" + ((DateTime)inputDTO.LastWorkingDateFrom).ToString("dd-MMM-yyyy") + "' order by LastworkingDateAdmin asc";
            }
            else if (inputDTO.LastWorkingDateTo != null)
            {
                sQuery = "select rl.*, em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName,em.DOJ,(Select a.Location from WorkLocationMaster a where a.WorkLocationId=em.WorkLocationId)Location from EmployeeExitResignation rl join EmployeeMaster em on rl.EmployeeId = em.EmployeeId join JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId join DepartmentMaster dm on em.DepartmentId = dm.DepartmentId where rl.unitid=" + inputDTO.UnitId + " and rl.LastworkingDateAdmin < '" + ((DateTime)inputDTO.LastWorkingDateTo).ToString("dd-MMM-yyyy") + "' order by LastworkingDateAdmin asc";
            }
            else
            {
                sQuery = "select rl.*, em.EmployeeName ,jtm.JobTitle ,dm.DepartmentName,em.DOJ,(Select a.Location from WorkLocationMaster a where a.WorkLocationId=em.WorkLocationId)Location from EmployeeExitResignation rl join EmployeeMaster em on rl.EmployeeId = em.EmployeeId join JobTitleMaster jtm on em.JobTitleId = jtm.JobTitleId join DepartmentMaster dm on em.DepartmentId = dm.DepartmentId where rl.unitid=" + inputDTO.UnitId + " order by LastworkingDateAdmin asc";
            }
            List<EmployeeExitResignationDTO>? resignationDTO = await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeExitResignationDTO>(sQuery);

            return Ok(resignationDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    public async Task<IActionResult> GetActiveEmployeeForResignationList(int UnitId)
    {
        try
        {
            var parameters = new { UnitId };
            string sQuery = "Select EmployeeName,EmployeeCode,EmployeeId from EmployeeMaster where UnitId=@UnitId and isActive=1 and InfoFillingStatus=1 and EmployeeStatus='Active' and DOJ <= GETDATE() and EmployeeId not in (Select EmployeeId from EmployeeExitResignation where AdminApproval != -1)";
            //string sQuery = "Select EmployeeName,EmployeeCode,EmployeeId from EmployeeMaster where UnitId=@UnitId and isActive=1 and InfoFillingStatus=1 and EmployeeStatus='Active' and DOJ <= GETDATE()";
            var res = await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeKeyValues>(sQuery, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetResignationList)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> EmployeeFullnFinalDetails(EmployeeFnFDetailslDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.EmployeeFnFId == 0)
                {
                    Expression<Func<EmployeeFnFDetails, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.UnitId == inputDTO.UnitId;
                    if (!_unitOfWork.EmployeeFnFDetails.Exists(expression))
                    {
                        _unitOfWork.EmployeeFnFDetails.AddAsync(_mapper.Map<EmployeeFnFDetails>(inputDTO));
                        _unitOfWork.Save();
                        string sQuery = @"update EmployeeExitResignation set SettlementStatus=1 WHERE EmployeeId=" + inputDTO.EmployeeId;
                        bool isSuccess = await _unitOfWork.EmployeeExitResignation.RunSQLCommand(sQuery);
                        if (isSuccess)
                        {
                        }
                        return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                    }
                    else
                    {
                        EmployeeFnFDetails FnFSettings = _mapper.Map<EmployeeFnFDetails>(_unitOfWork.EmployeeFnFDetails.GetAll(null, null, null).Result.Where(a => a.UnitId == inputDTO.UnitId && a.EmployeeId == inputDTO.EmployeeId).FirstOrDefault());
                        FnFSettings.NoticePeriod = inputDTO.NoticePeriod;
                        FnFSettings.LeaveBalance = inputDTO.LeaveBalance;
                        FnFSettings.Gratuity = inputDTO.Gratuity;
                        _unitOfWork.EmployeeFnFDetails.Update(_mapper.Map<EmployeeFnFDetails>(FnFSettings));
                        _unitOfWork.Save();
                        string sQuery = @"update EmployeeExitResignation set SettlementStatus=1 WHERE EmployeeId=" + inputDTO.EmployeeId;
                        bool isSuccess = await _unitOfWork.EmployeeExitResignation.RunSQLCommand(sQuery);
                        if (isSuccess)
                        {
                        }
                        return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                    }

                }
                else
                {

                    EmployeeFnFDetails outputDetails = _mapper.Map<EmployeeFnFDetails>(await _unitOfWork.EmployeeFnFDetails.GetByIdAsync(inputDTO.EmployeeFnFId));
                    outputDetails.NoticePeriod = inputDTO.NoticePeriod;
                    outputDetails.LeaveBalance = inputDTO.LeaveBalance;
                    outputDetails.Gratuity = inputDTO.Gratuity;
                    _unitOfWork.EmployeeFnFDetails.Update(_mapper.Map<EmployeeFnFDetails>(outputDetails));
                    _unitOfWork.Save();

                    string sQuery = @"update EmployeeExitResignation set SettlementStatus=1 WHERE EmployeeId=" + outputDetails.EmployeeId;
                    bool isSuccess = await _unitOfWork.EmployeeExitResignation.RunSQLCommand(sQuery);
                    if (isSuccess)
                    {
                    }
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                }

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee FullnFinal Details {nameof(EmployeeFullnFinalDetails)}");
            throw;
        }
    }



    public async Task<EmployeeFnFDetailslDTO> GetEmployeeFullnFinalDetails(int unitId, int employeeId)
    {
        try
        {
            // HttpResponseMessage httpMessage = new HttpResponseMessage();
            //inputDTO.SettingId = 0;
            EmployeeFnFDetailslDTO employeeFnF = _mapper.Map<EmployeeFnFDetailslDTO>(_unitOfWork.EmployeeFnFDetails.GetAll(null, null, null).Result.Where(a => a.UnitId == unitId && a.EmployeeId == employeeId).FirstOrDefault());

            return employeeFnF;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee Full n Final setting {nameof(GetEmployeeFullnFinalDetails)}");
            throw;
        }
    }



    [HttpPost]
    public async Task<List<EmployeeSettlementDetailslDTO>> GetEmployeeSettlementDetails(int employeeId, int unitId)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"@EmpId", employeeId, DbType.Int32);
            parms.Add(@"@UnitId", unitId, DbType.Int32);
            // parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            //parms.Add(@"@Month", inputVariables.Month, DbType.Int32);
            //parms.Add(@"@Year", inputVariables.Year, DbType.Int32);
            try
            {
                var result = _mapper.Map<List<EmployeeSettlementDetailslDTO>>(await _unitOfWork.EmployeeSettlementDetails.GetSPData("usp_FullnFinalSettlement", parms));
                // objResult = (SalarySummery)result;
                List<EmployeeSettlementDetailslDTO>? objResultData = (List<EmployeeSettlementDetailslDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Employee settlement details {nameof(GetEmployeeSettlementDetails)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<List<PaySlipComponentsDTO>> GetPaySlipComponents(EmployeeSettlementDetailslDTO inputVariables)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"@EmpId", inputVariables.EmployeeId, DbType.Int32);
            parms.Add(@"@UnitId", 5, DbType.Int32);
            parms.Add(@"@NoticeShortFall", inputVariables.NoticeShortfall, DbType.Int32);
            parms.Add(@"@ELBalance", inputVariables.ELBalance, DbType.Int32);
            parms.Add(@"@WorkingDays", inputVariables.WorkingDays, DbType.Int32);
            parms.Add(@"@NoticePeriodId", inputVariables.NoticePeriodId, DbType.Int32);
            parms.Add(@"@LeaveBalanceId", inputVariables.LeaveBalanceId, DbType.Int32);
            try
            {
                var result = _mapper.Map<List<PaySlipComponentsDTO>>(await _unitOfWork.PaySlipComponents.GetSPData("usp_GetComponentsforFinallSettlement", parms));
                // objResult = (SalarySummery)result;
                List<PaySlipComponentsDTO>? objResultData = (List<PaySlipComponentsDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetPaySlipComponents)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> SaveEmployeeSettlement(EmployeeSettlementSummeryDTO inputDTO, bool isFixed)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeSettlementSummery, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId;
                if (!_unitOfWork.EmployeeSettlementSummery.Exists(expression))
                {
                    _unitOfWork.EmployeeSettlementSummery.AddAsync(_mapper.Map<EmployeeSettlementSummery>(inputDTO));
                    _unitOfWork.Save();
                    if (isFixed)
                    {
                        string sQuery = @"update EmployeeExitResignation set SettlementStatus=1 WHERE EmployeeId=" + inputDTO.EmployeeId;
                        bool isSuccess = await _unitOfWork.EmployeeExitResignation.RunSQLCommand(sQuery);
                        if (isSuccess)
                        {
                        }
                    }
                    return Ok("Success");
                }
                else
                {
                    EmployeeSettlementSummery? EmployeeSettlementSummery = _unitOfWork.EmployeeSettlementSummery.GetWithRawSql("select * from EmployeeSettlementSummery where EmployeeId=" + inputDTO.EmployeeId).FirstOrDefault();
                    if (EmployeeSettlementSummery != null)
                    {
                        EmployeeSettlementSummery.OtherDeductions = inputDTO.OtherDeductions;
                        EmployeeSettlementSummery.OtherPayments = inputDTO.OtherPayments;
                        EmployeeSettlementSummery.Remarks = inputDTO.Remarks;
                        EmployeeSettlementSummery.ProcessBy = inputDTO.ProcessBy;
                        EmployeeSettlementSummery.ProcessDate = inputDTO.ProcessDate;
                        EmployeeSettlementSummery.IsFixed = inputDTO.IsFixed;
                        EmployeeSettlementSummery.IsMailSent = inputDTO.IsMailSent;
                        _unitOfWork.EmployeeSettlementSummery.Update(_mapper.Map<EmployeeSettlementSummery>(EmployeeSettlementSummery));
                        _unitOfWork.Save();
                        if (isFixed)
                        {
                            string sQuery = @"update EmployeeExitResignation set SettlementStatus=2 WHERE EmployeeId=" + inputDTO.EmployeeId;
                            bool isSuccess = await _unitOfWork.EmployeeExitResignation.RunSQLCommand(sQuery);
                            if (isSuccess)
                            {
                            }
                        }
                        return Ok("Success");
                    }
                    else
                    {
                        return BadRequest("failed");
                    }
                }


            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Employee Settlement {nameof(SaveEmployeeSettlement)}");
            throw;
        }
    }

    async Task<bool> SendMail_ResignationDetailsReceivedByManager(ExitEmailDTO dto)
    {
        try
        {
            string returnMessage = await _unitOfWork.EmployeeExitResignation.SendEmailResignationDetailsReceivedByManager(dto);
            return true;
        }
        catch (Exception ex) { return false; }
    }
    async Task<bool> SendMail_ResignationRequestRejectedByManager(ExitEmailDTO dto)
    {
        try
        {
            string returnMessage = await _unitOfWork.EmployeeExitResignation.SendEmailResignationRequestRejectedByManager(dto);
            return true;
        }
        catch (Exception ex) { return false; }
    }
    async Task<bool> SendMail_ResignationRequestRejectedByAdmin(ExitEmailDTO dto)
    {
        try
        {
            string returnMessage = await _unitOfWork.EmployeeExitResignation.SendEmailResignationRequestRejectedByAdmin(dto);
            return true;
        }
        catch (Exception ex) { return false; }
    }
    async Task<bool> SendMail_ResignationRequestReceivedByHR_ManagerApproval(ExitEmailDTO dto)
    {
        try
        {
            string returnMessage = await _unitOfWork.EmployeeExitResignation.SendEmailResignationRequestReceivedByHR_ManagerApproval(dto);
            return true;
        }
        catch (Exception ex) { return false; }
    }
    async Task<bool> SendMail_ResignationRequestReceivedByEmployee_HRApproval(ExitEmailDTO dto)
    {
        try
        {
            string returnMessage = await _unitOfWork.EmployeeExitResignation.SendEmailResignationRequestReceivedByEmployee_HRApproval(dto);
            return true;
        }
        catch (Exception ex) { return false; }
    }
    async Task<bool> SendMail_ExitInterviewEmailToEmployee(ExitEmailDTO dto)
    {
        try
        {
            string returnMessage = await _unitOfWork.EmployeeExitResignation.SendEmailExitInterviewEmailToEmployee(dto);
            return true;
        }
        catch (Exception ex) { return false; }
    }
    async Task<bool> SendMail_ClearanceRequestEmail(ExitEmailDTO dto)
    {
        try
        {
            string returnMessage = await _unitOfWork.EmployeeExitResignation.SendEmailClearanceRequestEmail(dto);
            return true;
        }
        catch (Exception ex) { return false; }
    }

}
