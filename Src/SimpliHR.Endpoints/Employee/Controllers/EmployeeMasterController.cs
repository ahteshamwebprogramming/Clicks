using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using Microsoft.Data.SqlClient;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Services.DBContext;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Dapper;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Infrastructure.Models.Common;
using SimpliHR.Infrastructure.Models.Exit;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Linq;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using SimpliHR.Infrastructure.Models.Login;
using SimpliHR.Infrastructure.Models.ClientAdmin;

namespace SimpliHR.Endpoints.Masters;

[EnableCors()]
[Route("api/[controller]/[action]")]
[ApiController]
public class EmployeeMasterController : ControllerBase
{
    private readonly SimpliDbContext _simpliDbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeMasterController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    public EmployeeMasterController(IUnitOfWork unitOfWork, ILogger<EmployeeMasterController> logger, IMapper mapper, SimpliDbContext simpliDbContext, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = simpliDbContext;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<EmployeeMasterDTO> GetEmployee(EmployeeMasterDTO inputDTO)
    {
        try
        {

            EmployeeMasterDTO outputDTO = _mapper.Map<EmployeeMasterDTO>(await _unitOfWork.EmployeeMaster.GetByIdWithChildrenAsync(inputDTO.EmployeeId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return outputDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployee)}");
            throw;
        }
    }


    //[HttpPost]
    //public async Task<EmployeeMasterDTO> GetEmployeeLoginInfo(EmployeeMasterDTO inputDTO)
    //{
    //    try
    //    {
    //        EmployeeMasterDTO outputDTO = new EmployeeMasterDTO();
    //        outputDTO = _mapper.Map<EmployeeMasterDTO>((await _unitOfWork.EmployeeMaster.GetLoginEmployeesInfo(p => p.EmployeeId == inputDTO.EmployeeId && p.IsActive==true)).FirstOrDefault());           
    //        HttpResponseMessage httpMessage = new HttpResponseMessage();
    //        if (outputDTO == null)
    //        {
    //            httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
    //            outputDTO = CommonHelper.GetClassObject(outputDTO);
    //        }
    //        else
    //            httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

    //        outputDTO.HttpMessage = httpMessage;
    //        return outputDTO;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployeeInfo)}");
    //        throw;
    //    }
    //}

    [HttpPost]
    public async Task<EmployeeMasterDTO> GetEmployeeLoginInfo(EmployeeMasterDTO inputDTO)
    {
        EmployeeMasterDTO outputData = new EmployeeMasterDTO();
        string sQuery = "SELECT [EmployeeId],[EmployeeCode],[ClientId],[FirstName],[MiddleName],[LastName],[EmployeeName],[DOB]," +
            "[GenderID],[FatherName],[ContactNo],[EmailId],[Age],[SpouseName],[ReligionId],[MaritalStatusId],[AadharNumber]," +
            "[PANNumber],[BloodGroupId],[WorkLocationId],[DOJ],[DepartmentId],[JobTitleId],[ManagerId],[OfficialEmail]," +
            "[IdentityID],[EmployeeStatus],[RoleId],[BandId],[ProfileImage],[BasicSalary],[AnnualBasicSalary],[AnnualCTC]," +
            "[MonthlyCTC],[OtherCompensation],[SalaryInHand],[CreatedOn],[CreatedBy],[ModifiedOn],[ModifiedBy],[InfoFillingStatus]," +
            "[JoinType],[isActive],[HODId],[UnitId],[CTC],[EPFNumber]" +
            ",[ConfirmationPeriod],[NoticePeriod],[DOC],[IsConfirmed],[PassportNumber]," +
            "[PassportIssueDate],[PassportValidTillDate],[PassportIssueCountryId],[PassportIssueStateId],[PassportIssueCityId],[EmergencyContactPerson]," +
            "[EmergencyContactNo],[EmergencyContactRelation],[ESINumber],[UANNumber],[EmploymentType],[ProfileImageExtension] FROM [dbo].[EmployeeMaster] WHERE EmployeeId=@employeeId AND IsActive=@isActive";
        outputData = (await _unitOfWork.EditEmployeeData.GetTableDataExec<EmployeeMasterDTO>(sQuery, new { @employeeId = inputDTO.EmployeeId, @isActive = inputDTO.IsActive })).FirstOrDefault();
        //outputData = (await _unitOfWork.EditEmployeeData.GetTableData<EmployeeMasterDTO>(sQuery)).FirstOrDefault();
        //.Select(p => p));
        return outputData;
    }

    

    [HttpPost]
    public async Task<EmployeeMasterDTO> GetEmployeeInfo(EmployeeMasterDTO inputDTO)
    {
        try
        {
                EmployeeMasterDTO outputDTO = new EmployeeMasterDTO();
            outputDTO = _mapper.Map<EmployeeMasterDTO>((await _unitOfWork.EmployeeMaster.GetEmployeesInfo(p => p.EmployeeId == inputDTO.EmployeeId)).FirstOrDefault());
            string sQry = $"SELECT ResignationListId,EmployeeId,UnitId,EmployeeCode,NoticePeriod,ResignationDate,LastWorkingDate,ReasonForLeaving,EmployeeComments," +
                         $" CreationDateEmployee,ResignationInitiatedBy,IsAllFormalityCompleted,IsResignationRolledBack,ResignationDateManager,LastWorkingDateManager,NoticePeriodWaiveOff,EligibleToHire,Document,ManagerRemarks,DocumentName,DocumentExtension,ResignationDateAdmin," +
                         $" LastWorkingDateAdmin,NoticePeriodWaiveOffAdmin,EligibleToHireAdmin,ActivateExitInterview,ClearanceByPass,DocumentAdmin,DocumentNameAdmin,DocumentExtensionAdmin,LWDPolicy,AdminRemarks,ReasonForLeavingManager,ReasonForLeavingAdmin,ExitInterviewData,ClearanceStatus,InterviewStatus,SettlementStatus " +
                         $" FROM EmployeeExitResignation WHERE EmployeeId={inputDTO.EmployeeId}";

            if (outputDTO != null)
                outputDTO.EmployeeExitResignationDetails = (await _unitOfWork.EmployeeExitResignation.GetTableData<EmployeeExitResignationDTO>(sQry));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return outputDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployeeInfo)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<EmployeeMasterVM> GetEmployeeTabFillingStatus(EmployeeMasterVM inputDTO)
    {
        try
        {
            //EmployeeMasterVM outputDTO = new EmployeeMasterVM();0
            string sQry = $"GetEmployeeTabFillingStatus";
            inputDTO.EmployeeTabFillingStatusData = (await _unitOfWork.EditEmployeeData.GetSPData<EmployeeTabFillingStatus>(sQry, new { @unitId = inputDTO.EmployeeMaster.UnitId, @employeeId = inputDTO.EmployeeMaster.EmployeeId })).FirstOrDefault();
            return inputDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving GetEmployeeTabFillingStatus {nameof(GetEmployeeInfo)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> GetEmployeeBirthDayInfo(int? UnitId)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@UnitId", UnitId, DbType.Int32);
            //IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            IList<EmployeeDashboardDetailsDTO> outputModel = new List<EmployeeDashboardDetailsDTO>();
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(await _unitOfWork.EmployeeMaster.GetEmployeesInfo(p =>p.UnitId== inputDTO.UnitId && p.Dob.Value.Day == DateTime.Now.Day && p.Dob.Value.Month == DateTime.Now.Month)).ToList();
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.FindAllByExpression(p => p.UnitId == inputDTO.UnitId && p.Dob.Value.Day == DateTime.Now.Day && p.Dob.Value.Month == DateTime.Now.Month));
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.CallStoredProc("usp_GetEmployeeBirthDay").ToList());
            outputModel = _mapper.Map<IList<EmployeeDashboardDetailsDTO>>(await _unitOfWork.EmployeeDashboardDetails.GetSPData<EmployeeDashboardDetailsDTO>("usp_GetEmployeeBirthDay", parms));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployeeBirthDayInfo)}");
            throw;

        }
    }


    [HttpPost]
    public async Task<IActionResult> GetOnBoardingEmployeeInfo(int? UnitId)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@UnitId", UnitId, DbType.Int32);
            //IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            IList<EmployeeDashboardDetailsDTO> outputModel = new List<EmployeeDashboardDetailsDTO>();
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(await _unitOfWork.EmployeeMaster.GetEmployeesInfo(p =>p.UnitId== inputDTO.UnitId && p.Dob.Value.Day == DateTime.Now.Day && p.Dob.Value.Month == DateTime.Now.Month)).ToList();
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.FindAllByExpression(p => p.UnitId == inputDTO.UnitId && p.Dob.Value.Day == DateTime.Now.Day && p.Dob.Value.Month == DateTime.Now.Month));
            // outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.CallStoredProc("usp_GetEmployeeBirthDay").ToList());
            outputModel = _mapper.Map<IList<EmployeeDashboardDetailsDTO>>(await _unitOfWork.EmployeeDashboardDetails.GetSPData<EmployeeDashboardDetailsDTO>("usp_GetEmployeeOnBoard", parms));
            return Ok(outputModel);
            //IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            ////outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(await _unitOfWork.EmployeeMaster.GetEmployeesInfo(p => p.UnitId == inputDTO.UnitId && p.Doj.Value.Date == DateTime.Now.Date)).ToList();
            //outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.FindAllByExpression(p => p.UnitId == inputDTO.UnitId && p.Doj.Value.Date == DateTime.Now.Date));

            //return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployeeInfo)}");
            throw;
        }
    }

    public async Task<IActionResult> GetAnnivesaryEmployeeInfo(int? UnitId)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@UnitId", UnitId, DbType.Int32);
            //IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            IList<EmployeeDashboardDetailsDTO> outputModel = new List<EmployeeDashboardDetailsDTO>();
            outputModel = _mapper.Map<IList<EmployeeDashboardDetailsDTO>>(await _unitOfWork.EmployeeDashboardDetails.GetSPData<EmployeeDashboardDetailsDTO>("usp_GetEmployeeAnnivesary", parms));
            return Ok(outputModel);
            //IList<EmployeeMasterDTO> ViewModel = new List<EmployeeMasterDTO>();
            //string sQuery = "select * from EmployeeMaster WHERE (YEAR(getdate()) - YEAR(DOJ)) % 1 = 0 and YEAR(getdate()) > YEAR(DOJ) and FORMAT(DOJ, 'MMdd') = FORMAT( GETDATE(), 'MMdd') and unitId=" + inputDTO.UnitId;
            ////EmployeeExitInterViewFormMasterDTO dto = _mapper.Map<EmployeeExitInterViewFormMasterDTO>(_unitOfWork.EmployeeExitInterViewFormMaster.GetWithRawSql(sQuery, null).FirstOrDefault());

            //ViewModel = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.CallStoredProc("usp_GetEmployeeAnnivesary",null).ToList());
            ////payrollEarningVM.PayrollEarningComponentList = _mapper.Map<List<PayrollEarningComponentDTO>>(await _unitOfWork.PayrollEarningComponent.GetTableData<PayrollEarningComponentDTO>(sQuery));

            ////payrollEarningVM.PayrollEarningComponentList.Skip(offset * limit).Take(limit).ToList();
            ////}
            //return Ok(ViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetAnnivesaryEmployeeInfo)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> CheckEEmployeeExistByEmailId(EmployeeEJoineeDTO inputDTO)
    {
        try
        {
            EmployeeMasterDTO outputDTO = new EmployeeMasterDTO();
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            Expression<Func<EmployeeMaster, bool>> expression = a => a.EmailId.Trim() == inputDTO.EmailId && a.IsActive == true;
            if (!_unitOfWork.EmployeeMaster.Exists(expression))
            {
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "No Record Found"));
            }
            else
            {
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployee)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> CheckEEmployeeExistByMobileNo(EmployeeEJoineeDTO inputDTO)
    {
        try
        {
            EmployeeMasterDTO outputDTO = new EmployeeMasterDTO();
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            Expression<Func<EmployeeMaster, bool>> expression = a => a.ContactNo == inputDTO.ContactNo && a.IsActive == true;
            if (! _unitOfWork.EmployeeMaster.Exists(expression))
            {
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "No Record Found"));
            }
            else
            {
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetEmployee)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> CheckEEmployeeExistByEmployeeCode(EmployeeEJoineeDTO inputDTO)
    {
        try
        {
            EmployeeMasterDTO outputDTO = new EmployeeMasterDTO();
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            Expression<Func<EmployeeMaster, bool>> expression = a => a.EmployeeCode.Trim() == inputDTO.EmployeeCode && a.UnitId == inputDTO.UnitId && a.IsActive == true;
            if (!_unitOfWork.EmployeeMaster.Exists(expression))
            {
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "No Record Found"));
            }
            else
            {
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(CheckEEmployeeExistByEmployeeCode)}");
            throw;
        }
    }

    [HttpPost(Name = "GetEmployees")]
    public async Task<IActionResult> GetEmployees(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(await _unitOfWork.EmployeeMaster.GetPagedList(requestParams)).Where(p => p.IsActive == true).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetEmployees)}");
            throw;
        }
    }


    [HttpPost(Name = "GetEmployeesForClient")]
    public async Task<IActionResult> GetEmployeesForClient(Core.Helper.RequestParams requestParams, int? unitId, bool isClient, int? logInUser)
    {
        try
        {
            Expression<Func<EmployeeMaster, bool>> expression;
            IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();

            if (isClient)
            {
                expression = (p => p.UnitId == unitId && p.IsActive == true);
                //    outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(await _unitOfWork.EmployeeMaster.GetAll(requestParams, expression, null)).ToList();
            }
            // DateOnly unitid filter of client

            else
            {
                expression = (p => p.UnitId == unitId && (logInUser == null ? true : p.ManagerId == logInUser) && p.ManagerId > 0 && p.IsActive == true);
                //  outputModel = _mapper.Map<IList<EmployeeMasterDTO>>(await _unitOfWork.EmployeeMaster.GetAll(requestParams, expression, null)).Where(x => x.EmployeeCode != null).ToList();
            }
            outputModel = await _unitOfWork.EmployeeMaster.GetEmployeesInfo(expression);
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetEmployees)}");
            throw;
        }
    }


    [HttpPost(Name = "GetEmployeesInfo")]
    public async Task<IActionResult> GetEmployeesInfo(Core.Helper.RequestParams requestParams, int? unitId, bool isClient, int? logInUser)
    {
        try
        {
            IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            outputModel = await _unitOfWork.EmployeeMaster.GetEmployeesInfo(p => p.UnitId == unitId && p.IsActive == true);
            if (!isClient)
            {
                outputModel = outputModel.Where(x => x.EmployeeCode != null).ToList();
            }
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetEmployees)}");
            throw;
        }
    }


    [HttpPost(Name = "GetEmployeeListing")]
    public async Task<IActionResult> GetEmployeeListing(Core.Helper.RequestParams requestParams, int? unitId, bool isClient, int? logInUser)
    {
        try
        {
            IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            outputModel = await _unitOfWork.EmployeeMaster.GetEmployeeListing(p => p.UnitId == unitId && p.IsActive == true);
            if (!isClient)
            {
                outputModel = outputModel.Where(x => x.EmployeeCode != null).ToList();
            }
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetEmployees)}");
            throw;
        }
    }

    public async Task<List<EmployeeValidationDTO>> GetEmployeeValidation(string sTabName, int clientId, int unitId)
    {
        List<EmployeeValidationDTO> empValidationList = new List<EmployeeValidationDTO>();
        var parms = new DynamicParameters();
        parms.Add(@"@unitId", unitId, DbType.Int32);
        parms.Add(@"@ClientId", clientId, DbType.Int32);
        parms.Add(@"@ScreenName", "", DbType.String);
        parms.Add(@"@ScreenTab", sTabName, DbType.String);
        empValidationList = _mapper.Map<List<EmployeeValidationDTO>>(await _unitOfWork.EmployeeValidation.GetSPData("GetEmployeeValidation", parms));
        return empValidationList;
    }

    public bool ValidateEmployeeCode(string EmployeeCode, int UnitId, int? EmployeeId)
    {
        try
        {
            if (EmployeeId != null && EmployeeId > 0)
            {
                var res = _unitOfWork.EmployeeMaster.FindFirstByExpression(x => x.EmployeeCode == EmployeeCode && x.UnitId == UnitId && x.IsActive == true && x.EmployeeId != EmployeeId);
                if (res != null)
                { return false; }
                else { return true; }
            }
            else
            {
                var res = _unitOfWork.EmployeeMaster.FindFirstByExpression(x => x.EmployeeCode == EmployeeCode && x.UnitId == UnitId && x.IsActive == true);
                if (res != null) { return false; }
                else { return true; }
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    [HttpPost]
    public string ValidateEmployeeInfo(EmployeeMasterDTO inputDTO)
    {
        string validMessage = "";
        StringBuilder messageBuilder = new StringBuilder();
        // inputDTO.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        Expression<Func<EmployeeMaster, bool>> expression = (a => (a.IsActive == true) && (a.ClientId == inputDTO.ClientId) && (a.EmployeeId != ((inputDTO.EmployeeId == 0 || inputDTO.EmployeeId == null) ? a.EmployeeId : inputDTO.EmployeeId)));
        IList<EmployeeMasterDTO> tableData = new List<EmployeeMasterDTO>();
        tableData = _mapper.Map<IList<EmployeeMasterDTO>>(_unitOfWork.EmployeeMaster.FindAllByExpression(expression));

        if (inputDTO.Pannumber != null && tableData.Where((a => (a.Pannumber != null && a.Pannumber.Trim() == inputDTO.Pannumber.Trim()))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate PAN Number({inputDTO.Pannumber.Trim()}) found</br>");
        }
        if (inputDTO.EmailId != null && tableData.Where((a => (a.EmailId != null && a.EmailId.Trim() == inputDTO.EmailId.Trim()))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate Email Address({inputDTO.EmailId.Trim()}) found</br>");
        }
        if (inputDTO.AadharNumber != null && tableData.Where((a => (a.AadharNumber != null && a.AadharNumber.Trim() == inputDTO.AadharNumber.Trim()))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate Aadhar Number({inputDTO.AadharNumber.Trim()}) found</br>");
        }
        if (inputDTO.ContactNo != null && tableData.Where((a => (a.ContactNo != null && a.ContactNo.Trim() == inputDTO.ContactNo.Trim()))).Count() != 0)
        {
            messageBuilder.Append($"Duplicate Contact Number({inputDTO.ContactNo.Trim()}) found</br>");
        }

        if (!messageBuilder.ToString().Trim().IsNullOrEmpty())
            validMessage = messageBuilder.ToString();
        return validMessage;
    }


    [HttpPost]
    public async Task<string> SaveAttachment(EmployeeTempDocUploadDTO empTempDoc)
    {
        string attachmentMsg = "Fail to save attachment";

        var screenTab = new Microsoft.Data.SqlClient.SqlParameter("@ScreenTab", empTempDoc.ScreenTab);
        var id = new Microsoft.Data.SqlClient.SqlParameter("@Id", empTempDoc.EmployeeId);
        var sessionId = new Microsoft.Data.SqlClient.SqlParameter("@SessionId", empTempDoc.SessionId);
        var loggedInUser = new Microsoft.Data.SqlClient.SqlParameter("@loggedInUser", empTempDoc.LoggedInUser);
        var refrenceId = new Microsoft.Data.SqlClient.SqlParameter("@RefrenceId", empTempDoc.ReferenceId);
        //var objData = new { @ScreenTab = empTempDoc.ScreenTab, @Id = empTempDoc.EmployeeId, @SessionId = empTempDoc.SessionId, @LoggedInUser = empTempDoc.LoggedInUser, @refrenceId=empTempDoc.ReferenceId };
        _unitOfWork.EmployeeMaster.ExecuteRawQuery("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser,@RefrenceId", new[] { screenTab, id, sessionId, loggedInUser, refrenceId });

        attachmentMsg = "Success";
        return attachmentMsg;
    }

    public async Task<string> VerifyRequiredAttachment(EmployeeTempDocUploadDTO empTempDoc, int? clientId, int? unitId)
    {
        string sMsg = string.Empty;
        List<EmployeeValidationDTO> empValidation = await GetEmployeeValidation((empTempDoc.ScreenTab == "All" ? "" : empTempDoc.ScreenTab), clientId.Value, unitId.Value);
        List<EmployeeTempDocUploadDTO> empTempDocs = await GetEmployeeTempDocUpload((empTempDoc.ScreenTab == "All" ? "" : empTempDoc.ScreenTab), empTempDoc.SessionId);
        empValidation.Where(y=>(y.AddAttachment.HasValue && y.EditAttachment.Value)).ToList().ForEach(x =>
        {
            if (!empTempDocs.Exists(r => r.FieldName == x.FieldName))
            {
                sMsg += string.IsNullOrEmpty(sMsg) ? $"Attachment is mandatory for {CommonHelper.NewLineEntry()}{x.DisplayText}" : $"{CommonHelper.NewLineEntry()}{x.DisplayText}";
            }
        });
        return sMsg;
    }

    public async Task<List<EmployeeTempDocUploadDTO>> GetEmployeeTempDocUpload(string sTabName, string sessionId)
    {
        List<EmployeeTempDocUploadDTO> attachmentList = new List<EmployeeTempDocUploadDTO>();
        //string sQuery = "Select [SessionId],[FieldName],[ReferenceId] ,[ScreenTab],[DcumentTypeId],[DocumentType],[UploadedFile],[EmployeeId],[ActionStatus],[ActionBy],[IsActive] FROM EmployeeTempDocUpload WHERE SessionId=@sessionId " +
        string sQuery = $"Select [SessionId],a.[FieldName],[ReferenceId],b.[ScreenTab],[DcumentTypeId],[DocumentType],[UploadedFile],[EmployeeId]" +
            $" ,[ActionStatus],[ActionBy],a.[IsActive] FROM EmployeeTempDocUpload a INNER JOIN EmployeeValidationMaster b ON a.FieldName = b.FieldName " +
            $" WHERE a.[IsActive]=1 AND SessionId = @sessionId  ";
        //AND Replace(b.ScreenTab,' ','') = Case WHEN b.ScreenTab IS NULL THEN null ELSE @screenTab END";
        //(sTabName!="" ? " AND Replace(ScreenTab,' ','') = Case WHEN ScreenTab IS NULL THEN ScreenTab ELSE @screenTab END" : "");
        //if (sTabName == "CurrentAddress")
        //    sTabName = "'CurrentAddress','PermanentAddress'";
        attachmentList = await _unitOfWork.EmployeeTempDocUpload.ExecuteQuery<EmployeeTempDocUploadDTO>(sQuery, new { @sessionId = sessionId, @screenTab = sTabName });
        return attachmentList;
    }


    public async Task<string> DeleteSessionAttachment(string sessionId)
    {
        string sQry = "Delete from EmployeeTempDocUpload where sessionid=@sessionId";
        _unitOfWork.EmployeeTempDocUpload.ExecuteQuery(sQry, new { @sessionId = sessionId });
        string sMsg = "Success";
        return sMsg;
    }

    public async Task<EmployeeUploadDocumentDTO> GetUploadedInfo(EmployeeTempDocUploadDTO empTempDoc)
    {
        string sSql = "SELECT [EmployeeId],[FieldName],[ScreenTab],[DcumentTypeId],[DocumentType],[UploadedFile],[CreatedOn],[CreatedBy],[IsActive] FROM EmployeeTempDocUpload  WHERE SessionId=@sessionId AND FieldName=@fieldName";
        var pram = new { @sessionId = empTempDoc.SessionId, @fieldName = empTempDoc.FieldName };
        //await _unitOfWork.EmployeeTempDocUpload.ExecuteQuery(sSql, pram);
        EmployeeUploadDocumentDTO empUploadedDoc = new EmployeeUploadDocumentDTO();
        EmployeeTempDocUploadDTO empTempDocOutput = new EmployeeTempDocUploadDTO();
        empTempDocOutput = _mapper.Map<EmployeeTempDocUploadDTO>((await _unitOfWork.EmployeeTempDocUpload.ExecuteQuery<EmployeeTempDocUpload>(sSql, pram)).FirstOrDefault());//_mapper.Map<EmployeeTempDocUploadDTO>(_unitOfWork.EmployeeTempDocUpload.GetFilter(x => x.SessionId == empTempDoc.SessionId && x.FieldName == empTempDoc.FieldName).Result);

        if (empTempDocOutput == null)
            if (empTempDoc.ReferenceId == null)
                empUploadedDoc = _mapper.Map<EmployeeUploadDocumentDTO>(_unitOfWork.EmployeeUploadDocument.FindFirstByExpression(x => x.FieldName == empTempDoc.FieldName && x.EmployeeId == empTempDoc.EmployeeId));
            else
                empUploadedDoc = _mapper.Map<EmployeeUploadDocumentDTO>(_unitOfWork.EmployeeUploadDocument.FindFirstByExpression(x => x.FieldName == empTempDoc.FieldName && x.EmployeeId == empTempDoc.EmployeeId && x.ReferenceId == empTempDoc.ReferenceId));
        else
        {
            empUploadedDoc.EmployeeDocument = empTempDocOutput.UploadedFile;
            empUploadedDoc.DocumentType = empTempDocOutput.DocumentType;
            empUploadedDoc.FieldName = empTempDocOutput.FieldName;
        }
        return empUploadedDoc;
    }

    [HttpPost]
    public IActionResult SaveEmployee(EmployeeMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                bool isValidationMessageEmpty = ValidateEmployeeInfo(inputDTO) == string.Empty ? true : false;
                if (isValidationMessageEmpty)
                {
                    var em = _mapper.Map<EmployeeMasterDTO>(_unitOfWork.EmployeeMaster.Insert(_mapper.Map<EmployeeMaster>(inputDTO)));
                    int empID = em.EmployeeId;
                    string encempid = CommonHelper.EncryptURLHTML(em.EmployeeId.ToString());
                    em.EnycEmployeeId = encempid;
                    return Ok(em);
                }
                else
                    return BadRequest(isValidationMessageEmpty);
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SaveEmployee)}");
            throw;
        }
    }



    [HttpPost]
    public IActionResult SaveEmployeeProfileImage(EmployeeProfileImageDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //var outputDTO = _mapper.Map<EmployeeMaster>(inputDTO);
                int empID = _unitOfWork.EmployeeMaster.Insert(_mapper.Map<EmployeeMaster>(inputDTO)).EmployeeId;
                _unitOfWork.Save();
                // int empID = _unitOfWork.EmployeeMaster.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeId);
                //return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, empID.ToString()));
                return Ok(empID.ToString());

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SaveEmployee)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<EditEmployeeDataDTO> SaveEditEmployeeProfileImage(EditEmployeeDataDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //var outputDTO = _mapper.Map<EmployeeMaster>(inputDTO);
                inputDTO.EmployeeValidationId = _unitOfWork.EmployeeValidationMaster.GetTableData<EmployeeValidationMaster>($"SELECT EmployeeValidationId FROM  EmployeeValidationMaster WHERE FieldName = '{inputDTO.ChangeType}'").Result.FirstOrDefault().EmployeeValidationId;
                var parm = new { @employeeValidationId = inputDTO.EmployeeValidationId, @employeeId = inputDTO.EmployeeId };

                bool isExists = await _unitOfWork.EditEmployeeData.IsExists("Select EmployeeUpdateId FROM editemployeedata WHERE EmployeeValidationId=@employeeValidationId AND IsNull(IsApproved,0)=0 AND EmployeeId=@employeeId", parm);
                if (!isExists)
                {
                    inputDTO.CreatedBy = inputDTO.LoggedInUser;
                    inputDTO.CreatedOn = DateTime.Now;
                    inputDTO.IsActive = true;
                    _unitOfWork.EditEmployeeData.AddAsync(_mapper.Map<EditEmployeeData>(inputDTO));
                    _unitOfWork.Save();
                    inputDTO.DisplayMessage = "Success";
                }
                else
                    inputDTO.DisplayMessage = "Profile change request already raised. Cancel existing one first to raise it again";
            }
            else
                inputDTO.DisplayMessage = "Invalid Model";

            return inputDTO;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SaveEmployee)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveEmployeeEJoinee(EmployeeEJoineeDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeMaster, bool>> expression = a => a.EmployeeName.Trim().Replace(" ", "") == inputDTO.EmployeeName;
                Expression<Func<EmployeeMaster, bool>> expression1 = a => a.EmailId.Trim() == inputDTO.EmailId;
                if (!_unitOfWork.EmployeeMaster.Exists(expression1))
                {
                    //var outputDTO = _mapper.Map<EmployeeMaster>(inputDTO);
                    //_unitOfWork.EmployeeMaster.AddAsync(_mapper.Map<EmployeeMaster>(inputDTO));

                    var em = _unitOfWork.EmployeeMaster.Insert(_mapper.Map<EmployeeMaster>(inputDTO));
                    //_unitOfWork.Save();
                    //int empID = _unitOfWork.EmployeeMaster.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeId);
                    int empID = em.EmployeeId;

                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, empID.ToString()));
                }
                else
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SaveEmployee)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateEmployee(EmployeeMasterDTO inputDTO, string properties)
    {
        try
        {
            if (ModelState.IsValid)
            {
                string isValidationMessageEmpty = string.Empty;

                if (inputDTO.FormName.ToUpper() == "EMPLOYEEPERSONALINFOFORM" || inputDTO.FormName.ToUpper() == "FINALSUBMIT")
                    isValidationMessageEmpty = ValidateEmployeeInfo(inputDTO);

                if (isValidationMessageEmpty.Equals(""))
                {
                    bool success = _unitOfWork.EmployeeMaster.UpdateDbEntry(_mapper.Map<EmployeeMaster>(inputDTO), properties);
                    _unitOfWork.Save();

                    // var results = _SchedularController.SchedularEvent(Convert.ToString(units), employeeid);
                    return Ok("Success");
                }
                else
                    return BadRequest(isValidationMessageEmpty);

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee updates {nameof(UpdateEmployee)}");
            throw;
        }
    }

    //[HttpPost]
    //public async Task<IActionResult> AddEmployeeLeaveBalance(EmployeeMasterDTO inputDTO)
    //{
    //    try
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            //new { @UnitIds = inputDTO.UnitId, @EmployeeIds = inputDTO.EmployeeId, @LeaveTypeIds = "" }
    //            var parms = new DynamicParameters();
    //            parms.Add(@"@UnitIds", inputDTO.UnitId, DbType.String);
    //            parms.Add(@"@EmployeeIds", inputDTO.EmployeeId, DbType.String);
    //            parms.Add(@"@LeaveTypeIds","", DbType.String);

    //            await _unitOfWork.EmployeeLeaveBalance.GetStoredProcedure("AddEmployeeLeaveBalance", parms);
    //            return Ok("Success");
    //        }
    //        else
    //            return BadRequest("Invalid Model");
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in Employee updates {nameof(UpdateEmployee)}");
    //        throw;
    //    }
    //}



    [HttpPost]
    public async Task<IActionResult> DeleteEmployee(EmployeeMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeMaster outputMaster = _mapper.Map<EmployeeMaster>(await _unitOfWork.EmployeeMaster.GetByIdAsync(inputDTO.EmployeeId));
                outputMaster.IsActive = false;
                _unitOfWork.EmployeeMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee {nameof(DeleteEmployee)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<EmployeeKeyValues> GetEmployeeKeyValue()
    {
        return (_unitOfWork.EmployeeMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new EmployeeKeyValues()
                           {
                               EmployeeId = p.EmployeeId,
                               EmployeeName = p.EmployeeName
                           })).ToList();
    }

    [HttpPost]
    public async Task<List<EmployeePolicyAcceptanceDTO>> GetEmployeePolicies(int clientId, int employeeId, int UnitId)
    {
        Expression<Func<PolicyDocumentsMaster, bool>> expression = (p => p.ClientId == clientId && p.IsActive == true && p.UnitId == UnitId);
        Expression<Func<EmployeePolicyAcceptance, bool>> expressionE = (p => p.EmployeeId == employeeId && p.IsActive == true);

        IList<EmployeePolicyAcceptance> employeePolicyAcceptances = new List<EmployeePolicyAcceptance>();
        employeePolicyAcceptances = _unitOfWork.EmployeePolicyAcceptance.FindAllByExpression(expressionE);

        IList<PolicyDocumentsMaster> policyDocumentsMaster = new List<PolicyDocumentsMaster>();
        policyDocumentsMaster = await _unitOfWork.PolicyDocumentsMaster.GetAll(expression);


        List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTO = new List<EmployeePolicyAcceptanceDTO>();

        foreach (var policyDocument in policyDocumentsMaster)
        {
            employeePolicyAcceptanceDTO.Add(new EmployeePolicyAcceptanceDTO
            {
                PolicyDocumentsMasterId = policyDocument.PolicyDocumentsMasterId,
                PolicyDocumentsCategory = policyDocument.PolicyDocumentsCategory.PolicyDocumentsCategory,
                PolicyDocumentsSubCategory = policyDocument.PolicyDocumentsSubCategory.PolicyDocumentsSubCategory,
                PolicyDocument = policyDocument.PolicyDocument,
                PolicyDocumentPath = policyDocument.PolicyDocumentPath,
                AcceptanceRequired = policyDocument.AcceptanceRequired,
                Accepted = employeePolicyAcceptances.Count(x => x.PolicyDocumentsMasterId == policyDocument.PolicyDocumentsMasterId && x.Accepted == true) > 0 ? true : false,
            });
        }
        return employeePolicyAcceptanceDTO;
    }

    [HttpPost]
    public List<EmployeePolicyAcceptanceDTO> GetPolicyDocumentsMasterById(int clientId, int employeeId, int unitId)
    {
        Expression<Func<PolicyDocumentsMaster, bool>> expression = (p => p.ClientId == clientId && p.IsActive == true && p.UnitId == unitId);
        Expression<Func<EmployeePolicyAcceptance, bool>> expressionE = (p => p.EmployeeId == employeeId && p.IsActive == true);

        List<PolicyDocumentsMaster> policyDocumentsMaster = _unitOfWork.PolicyDocumentsMaster.GetAll(expression, null).Result.ToList();

        List<EmployeePolicyAcceptance> employeePolicyAcceptances = _unitOfWork.EmployeePolicyAcceptance.GetAll(expressionE, null).Result.ToList();

        List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTO = new List<EmployeePolicyAcceptanceDTO>();

        foreach (var policyDocument in policyDocumentsMaster)
        {
            employeePolicyAcceptanceDTO.Add(new EmployeePolicyAcceptanceDTO
            {
                PolicyDocumentsMasterId = policyDocument.PolicyDocumentsMasterId,
                PolicyDocumentsCategory = policyDocument.PolicyDocumentsCategory.PolicyDocumentsCategory,
                PolicyDocumentsSubCategory = policyDocument.PolicyDocumentsSubCategory.PolicyDocumentsSubCategory,
                PolicyDocument = policyDocument.PolicyDocument,
                PolicyDocumentPath = policyDocument.PolicyDocumentPath,
                AcceptanceRequired = policyDocument.AcceptanceRequired,
                Accepted = employeePolicyAcceptances.Count(x => x.PolicyDocumentsMasterId == policyDocument.PolicyDocumentsMasterId && x.Accepted == true) > 0 ? true : false,
            });
        }
        return employeePolicyAcceptanceDTO;
    }

    [HttpPost]
    public IActionResult DeletePolicyAcceptance(int employeeId)
    {
        try
        {
            List<EmployeePolicyAcceptance> employeePolicyAcceptances = _unitOfWork.EmployeePolicyAcceptance.FindAllByExpression(x => x.EmployeeId == employeeId).ToList();

            foreach (var e in employeePolicyAcceptances)
            {
                e.IsActive = false;
                e.ModifiedDate = DateTime.Now;
            }
            _unitOfWork.EmployeePolicyAcceptance.UpdateRange(employeePolicyAcceptances);
            _unitOfWork.Save();
            //foreach (var roleMenu in roleMenus)
            //{
            //    roleMenu.IsActive = 0;
            //    _unitOfWork.RoleMenuMapping.Update(roleMenu);
            //    _unitOfWork.Save();
            //}
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee {nameof(DeletePolicyAcceptance)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SavePolicyAcceptanceByRange(List<EmployeePolicyAcceptanceDTO> inputDTOList)
    {
        try
        {
            foreach (var inputDTO in inputDTOList)
            {
                if (ModelState.IsValid)
                {
                    Expression<Func<EmployeePolicyAcceptance, bool>> expression = (a => ((a.PolicyDocumentsMasterId == inputDTO.PolicyDocumentsMasterId && a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true)));
                    if (!_unitOfWork.EmployeePolicyAcceptance.Exists(expression))
                    {
                        _unitOfWork.EmployeePolicyAcceptance.AddAsync(_mapper.Map<EmployeePolicyAcceptance>(inputDTO));
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    throw new Exception("Not authorised");
                }
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SavePolicyAcceptanceByRange)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<EmployeeMasterDTO> GetEmployeeById(int EmployeeId)
    {
        try
        {
            EmployeeMasterDTO dto = _mapper.Map<EmployeeMasterDTO>(await _unitOfWork.EmployeeMaster.GetByIdAsync(EmployeeId));
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SavePolicyAcceptanceByRange)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<EmployeeEJoineeDTO> GetEEmployeeById(int EmployeeId)
    {
        try
        {
            EmployeeEJoineeDTO dto = _mapper.Map<EmployeeEJoineeDTO>(await _unitOfWork.EmployeeMaster.GetByIdAsync(EmployeeId));
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SavePolicyAcceptanceByRange)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<EmployeeKeyValues> GetClientAdminKeyValueByClientId(int ClientId)
    {
        try
        {
            int? EmployeeId = _unitOfWork.LoginDetail.FindFirstByExpression(x => x.IsActive == true && x.ClientId == ClientId && x.LoginType == 1).EmployeeId;

            var res = _unitOfWork.EmployeeMaster.FindFirstByExpression(x => x.EmployeeId == EmployeeId);

            EmployeeKeyValues employeeKeyValues = new EmployeeKeyValues();
            employeeKeyValues.EmployeeId = res.EmployeeId;
            employeeKeyValues.EmployeeName = res.EmployeeName;

            return employeeKeyValues;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(GetClientAdminKeyValueByClientId)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeForClientDashboardStats(int unitId)
    {
        try
        {
            //List<EmployeeMasterDTO> employeeMasterDTO = _mapper.Map<List<EmployeeMasterDTO>>(await _unitOfWork.EmployeeMaster.GetAll(x => x.IsActive == true && x.InfoFillingStatus == 1 && x.EmployeeStatus == "Active" && x.UnitId == unitId));
            //return Ok(employeeMasterDTO);

            List<EmployeeMasterDTO> res = await _unitOfWork.EmployeeAnnouncement.GetTableData<EmployeeMasterDTO>($"Select * from EmployeeMaster where isActive=1 and InfoFillingStatus=1 and EmployeeStatus='Active' and UnitId={unitId}");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetEmployeeTickets)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetBandMasterForDasboard(int unitId)
    {
        try
        {
            List<BandMasterDTO> res = await _unitOfWork.EmployeeAnnouncement.GetTableData<BandMasterDTO>($"Select * from bandMaster where UnitId={unitId}");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetEmployeeTickets)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetEmployeeQualificationForDasboard(int unitId)
    {
        try
        {
            List<EmployeeAcademicDetailDTO> res = await _unitOfWork.EmployeeAnnouncement.GetTableData<EmployeeAcademicDetailDTO>($"Select AD.AcademicDetailId,AD.EmployeeId,Convert(nvarchar,AD.Percentage)Percentage,AD.InstituteName,AD.PassingYear,AD.AcademicId from EmployeeAcademicDetail AD join EmployeeMaster EM on AD.EmployeeId=EM.EmployeeId where EM.isActive=1 and AD.isActive=1 and EM.InfoFillingStatus=1 and EM.EmployeeStatus='Active' and EM.UnitId={unitId}");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetEmployeeTickets)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetEmployeeExperienceForDasboard(int unitId)
    {
        try
        {
            List<EmployeeExperienceDetailDTO> res = await _unitOfWork.EmployeeAnnouncement.GetTableData<EmployeeExperienceDetailDTO>($"Select AD.*\r\nfrom [EmployeeExperienceDetail] AD \r\njoin EmployeeMaster EM on AD.EmployeeId=EM.EmployeeId where EM.isActive=1 and AD.isActive=1 and EM.InfoFillingStatus=1 and EM.EmployeeStatus='Active' and EM.UnitId={unitId}");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetEmployeeTickets)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetEmployeeExitListForDasboard(int unitId)
    {
        try
        {
            List<EmployeeExitResignationDTO> res = await _unitOfWork.EmployeeAnnouncement.GetTableData<EmployeeExitResignationDTO>($"Select * from [dbo].[EmployeeExitResignation] where AdminApproval=1 and unitId={unitId}");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetEmployeeTickets)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> CurrentDateEmployeeStats(int unitId)
    {
        try
        {
            //List<CurrentDateEmployeeStatsDTO> res = await _unitOfWork.EmployeeAnnouncement.GetTableData<CurrentDateEmployeeStatsDTO>($"Select \r\n\r\nisnull((SELECT SUM(NoOfLeave) FROM EmployeeLeaveDetails WHERE StartDate <= getdate() AND EndDate >= getdate() and UnitId={unitId} and leavestatus=0),0)AS TotalLeave\r\n\r\n\r\n,isnull((Select count(1) from attendancehistory where convert(date,dutydate)=convert(date,getdate()) and unitid={unitId} and InTime != '00:00:00.0000000' AND OutTime = '00:00:00.0000000' group by EmployeeId),0) Present\r\n,isnull((Select count(1) from attendancehistory where convert(date,dutydate)=convert(date,getdate()) and unitid={unitId} and InTime != '00:00:00.0000000' AND OutTime = '00:00:00.0000000' group by EmployeeId),0) LateComing\r\n,isnull((Select count(1) from attendancehistory where convert(date,dutydate)=convert(date,getdate()) and unitid={unitId} and InTime != '00:00:00.0000000' AND OutTime = '00:00:00.0000000' group by EmployeeId),0) [Absent]");
            List<CurrentDateEmployeeStatsDTO> res = await _unitOfWork.EmployeeAnnouncement.GetTableData<CurrentDateEmployeeStatsDTO>($"Select \r\nisnull((Select count(1) from EmployeeMaster where isActive=1 and InfoFillingStatus=1 and EmployeeStatus='Active' and UnitId=1),0)TotalEmployee\r\n,isnull(2,0)AS TotalLeave\r\n,isnull((Select count(1) from EmployeeMaster where isActive=1 and InfoFillingStatus=1 and EmployeeStatus='Active' and UnitId={unitId})-4,0) Present\r\n,isnull(1,0)AS LateComing\r\n,isnull((Select count(1) from EmployeeMaster where isActive=1 and InfoFillingStatus=1 and EmployeeStatus='Active' and UnitId={unitId})-((Select count(1) from EmployeeMaster where isActive=1 and InfoFillingStatus=1 and EmployeeStatus='Active' and UnitId={unitId})- 4 + 2),0)AS [Absent]");
            if (res != null && res.Count > 0)
            {
                return Ok(res.FirstOrDefault());
            }
            else
            {
                return BadRequest("No Data Found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetEmployeeTickets)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetAcademicMasterForDasboard(int unitId)
    {
        try
        {
            List<AcademicMasterDTO> res = await _unitOfWork.EmployeeAnnouncement.GetTableData<AcademicMasterDTO>($"Select * from AcademicMaster where UnitId={unitId} and IsActive=1");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetEmployeeTickets)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateWageBillTrendDataForDashboard(int unitId)
    {
        try
        {
            string query = $"SELECT \r\n    SUM(CONVERT(int, CTC) / 12) AS WageBill,\r\n    AVG(CONVERT(int, CTC) / 12) AS AverageWageBill,\r\n    COUNT(EmployeeId) AS EmployeeCount\r\nFROM \r\n    EmployeeMaster \r\nWHERE \r\n    isActive = 1 \r\n    AND InfoFillingStatus = 1 \r\n    AND EmployeeStatus = 'Active' \r\n    AND UnitId = {unitId}\r\n    AND EmployeeId NOT IN (\r\n        SELECT \r\n            EmployeeId \r\n        FROM \r\n            [dbo].[EmployeeExitResignation] \r\n        WHERE \r\n            unitid = {unitId} \r\n            AND AdminApproval = 1 \r\n            AND LastWorkingDateAdmin < GETDATE()\r\n    );";
            var res = await _unitOfWork.WageBillTrendData.GetTableData<WageBillTrendData>(query);
            if (res != null && res.Count > 0)
            {
                WageBillTrendData? wageBillTrendDataRes = res.FirstOrDefault();
                if (wageBillTrendDataRes != null)
                {
                    WageBillTrendData? wageBillTrendData;
                    //bool wbdtExistsForCurrentMonth = await _unitOfWork.WageBillTrendData.Exists(x => x.UnitId == unitId && x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year);
                    var wbdtExistsForCurrentMonth = await _unitOfWork.WageBillTrendData.GetTableData<WageBillTrendData>($"Select * from WageBillTrendData wbtd where UnitId={unitId} and wbtd.[Month]={DateTime.Now.Month} and wbtd.[Year]={DateTime.Now.Year}");
                    if (wbdtExistsForCurrentMonth != null && wbdtExistsForCurrentMonth.Count > 0)
                    {
                        wageBillTrendData = wbdtExistsForCurrentMonth.FirstOrDefault();
                        if (wageBillTrendData != null)
                        {
                            wageBillTrendData.AverageWageBill = wageBillTrendDataRes.AverageWageBill;
                            wageBillTrendData.EmployeeCount = wageBillTrendDataRes.EmployeeCount;
                            wageBillTrendData.WageBill = wageBillTrendDataRes.WageBill;
                            await _unitOfWork.WageBillTrendData.UpdateAsync(wageBillTrendData);
                            return Ok();
                        }
                    }
                    wageBillTrendData = new WageBillTrendData();
                    wageBillTrendData.UnitId = unitId;
                    wageBillTrendData.EmployeeCount = wageBillTrendDataRes.EmployeeCount;
                    wageBillTrendData.WageBill = wageBillTrendDataRes.WageBill;
                    wageBillTrendData.AverageWageBill = wageBillTrendDataRes.AverageWageBill;
                    wageBillTrendData.Month = DateTime.Now.Month;
                    wageBillTrendData.Year = DateTime.Now.Year;
                    await _unitOfWork.WageBillTrendData.AddAsync(wageBillTrendData);
                    return Ok();
                }
            }
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(UpdateWageBillTrendDataForDashboard)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetWageBillTrendDataForDashboard(int unitId)
    {
        try
        {
            string query = $";WITH Last12Months AS (\r\n    SELECT DATEADD(MONTH, -12, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS MonthYear\r\n    UNION ALL\r\n    SELECT DATEADD(MONTH, 1, MonthYear)\r\n    FROM Last12Months\r\n    WHERE DATEADD(MONTH, 1, MonthYear) < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)\r\n)\r\nSELECT \r\n    MONTH(L12M.MonthYear) AS Month,\r\n    YEAR(L12M.MonthYear) AS Year,\r\n    ISNULL(SUM(WageBill), 0) AS WageBill,\r\n\tISNULL(SUM(AverageWageBill), 0) AS AverageWageBill,\r\n\tISNULL(SUM(EmployeeCount), 0) AS EmployeeCount\r\nFROM \r\n    Last12Months L12M\r\nLEFT JOIN \r\n    WageBillTrendData WBT\r\n    ON MONTH(L12M.MonthYear) = WBT.Month AND YEAR(L12M.MonthYear) = WBT.Year AND WBT.UnitId = 1\r\nGROUP BY \r\n    MONTH(L12M.MonthYear),\r\n    YEAR(L12M.MonthYear)\r\nORDER BY \r\n    YEAR(L12M.MonthYear), MONTH(L12M.MonthYear);";
            var res = await _unitOfWork.WageBillTrendData.GetTableData<WageBillTrendDataDTO>(query);
            if (res != null && res.Count > 0)
            {
                return Ok(res);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetWageBillTrendDataForDashboard)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<List<EmployeeTicketViewModelDTO>> GetEmployeeTickets(int unitId, int moduleId)
    {
        try
        {

            var parms = new DynamicParameters();
            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add("@ModuleId", moduleId, DbType.Int32);
            // parms.Add(@"@UnitId", inputVariables.UnitId, DbType.Int32);
            //parms.Add(@"@Month", inputVariables.Month, DbType.Int32);
            //parms.Add(@"@Year", inputVariables.Year, DbType.Int32);
            try
            {
                var result = _mapper.Map<List<EmployeeTicketViewModelDTO>>(await _unitOfWork.EmployeeTicketViewModel.GetSPData("usp_getEmployeeTickets", parms));
                // objResult = (SalarySummery)result;
                List<EmployeeTicketViewModelDTO>? objResultData = (List<EmployeeTicketViewModelDTO>)result;
                return objResultData;
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the Salary {nameof(GetEmployeeTickets)}");
            throw;
        }
    }

    public async Task<AddDeleteTableActionDTO> FindEditRequest(string refTab)
    {
        try
        {
            string sSql = "SELECT TOP 1 [ActionStatus],[ActionType],[TicketId],[ReferenceTable],[ReferenceId],[EmployeeId],[EntrySource],[CreatedOn]," +
                " [CreatedBy],[IsActive] FROM AddDeleteTableAction WHERE IsNull(ActionStatus,0)=0 AND ActionType='ADD' AND ReferenceTable=@refTab";
            AddDeleteTableActionDTO addDeleteActionDTO = (await _unitOfWork.EditEmployeeData.ExecuteQuery<AddDeleteTableActionDTO>(sSql, new { @refTab=refTab })).FirstOrDefault();
            return addDeleteActionDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeDetailForApproval)}");
            throw;
        }

    }

    public async Task<IActionResult> SaveEmployeeDetailForApproval(AddDeleteTableActionDTO? deleteEmployeeData)
    {
        try
        {
            string sSql = "INSERT INTO [dbo].[AddDeleteTableAction] ([ActionStatus],[ActionType],[TicketId]," +
                "[ReferenceTable],[ReferenceId],[EmployeeId],[EntrySource],[CreatedOn],[CreatedBy],[IsActive])" +
                " VALUES(" + deleteEmployeeData.ActionStatus + ",'" + deleteEmployeeData.ActionType
                + "','" + deleteEmployeeData.TicketId + "','" + deleteEmployeeData.ReferenceTable + "'," + deleteEmployeeData.ReferenceId + "," + deleteEmployeeData.EmployeeId
                + ",'" + deleteEmployeeData.EntrySource + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + deleteEmployeeData.LoggedInUser + ",1)";
            await _unitOfWork.EditEmployeeData.RunSQLCommand(sSql);

            return Ok("Success");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeDetailForApproval)}");
            throw;
        }

    }

    [HttpPost]
    public async Task<IActionResult> GetMyMeetnigDetails(MyMeetingsDTO inputDTO)
    {
        try
        {


            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeId", inputDTO.EmployeeId, DbType.Int32);         
            parms.Add(@"@UnitId", inputDTO.UnitId, DbType.Int32);

            //IList<EmployeeMasterDTO> outputModel = new List<EmployeeMasterDTO>();
            IList<MyMeetingsDTO> outputModel = new List<MyMeetingsDTO>();          
            outputModel = _mapper.Map<IList<MyMeetingsDTO>>(await _unitOfWork.MyMeetings.GetSPData<MyMeetingsDTO>("usp_GetAccountDetails", parms));
            return Ok(outputModel);



        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in employee user id {nameof(SaveEmployeeUserIdDetails)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<string> SaveEmployeeUserIdDetails(MyMeetingsDTO inputDTO)
    {

        try
        {


            var parms = new DynamicParameters();
            parms.Add(@"@EmployeeId", inputDTO.EmployeeId, DbType.Int32);
            parms.Add(@"@UserId", inputDTO.UserId, DbType.String);
            parms.Add(@"@UnitId", inputDTO.UnitId, DbType.Int32);
            parms.Add(@"@UserPassword", inputDTO.EncryptedPassword, DbType.String);
            parms.Add(@"@UserType", inputDTO.UserType, DbType.String);



            var result = await _unitOfWork.MyMeetings.GetSPData("usp_SaveEmployeeSignInDetails", parms);

            string returnMessage = "Sucess";
            //var result = await _unitOfWork.EmployeeLeaveDetails.GetStoredProcedure("usp_ApplyEmployeeLeave", parms);
            if (result.Count == 0)
                returnMessage = "Saved!";
            else
                returnMessage = "Failed!";

            return returnMessage;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in employee user id {nameof(SaveEmployeeUserIdDetails)}");
            throw;
        }

    }


    [HttpPost]
    public async Task<bool> UpdateEmployeeMailStamp(int empId)
    {
        try
        {
            var employeeId = new Microsoft.Data.SqlClient.SqlParameter("EmployeeId", empId);         
            _unitOfWork.EmployeeMaster.ExecuteRawQuery("EXEC usp_UpdateEmployeeStamp @EmployeeId", new[] { employeeId });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee mail updates {nameof(UpdateEmployeeMailStamp)}");
            throw;
        }
    }


    //public IActionResult SchedularEvent(string? unitIds, string employeeId)
    //{
    //    try
    //    {


    //        using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
    //        {
    //            if (con.State == ConnectionState.Closed)
    //                con.Open();

    //            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("UpdateLeaveBalance", con))
    //            {
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                //cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
    //                cmd.Parameters.Add("@UnitIds", SqlDbType.NVarChar).Value = unitIds;
    //                cmd.Parameters.Add("@EmployeeIds", SqlDbType.NVarChar).Value = employeeId;
    //                cmd.Parameters.Add("@LeaveTypeIds", SqlDbType.NVarChar).Value = "";
    //                cmd.ExecuteNonQuery();



    //            }

    //        }

    //        return Ok("Success");
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Banks {nameof(SchedularEvent)}");
    //        throw;
    //    }
    //}

    //[HttpPost]
    //public async Task<string> SaveAttachment(EmployeeTempDocUploadDTO empTempDoc)
    //{
    //    string attachmentMsg = "Fail to save attachment";
    //    var screenTab = new Microsoft.Data.SqlClient.SqlParameter("@ScreenTab", empTempDoc.ScreenTab);
    //    var id = new Microsoft.Data.SqlClient.SqlParameter("@Id", empTempDoc.EmployeeId);
    //    var sessionId = new Microsoft.Data.SqlClient.SqlParameter("@SessionId", empTempDoc.SessionId);
    //    var loggedInUser = new Microsoft.Data.SqlClient.SqlParameter("@loggedInUser", empTempDoc.LoggedInUser);
    //    var refId = new Microsoft.Data.SqlClient.SqlParameter("@RefrenceId", empTempDoc.ReferenceId);
    //    //var objData = new { @ScreenTab = empTempDoc.ScreenTab, @Id = empTempDoc.EmployeeId, @SessionId = empTempDoc.SessionId, @LoggedInUser = empTempDoc.LoggedInUser, @RefrenceId=refId };
    //    try
    //    {
    //        _unitOfWork.EmployeeFamilyDetail.ExecuteRawQuery("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser,@RefrenceId", new[] { screenTab, id, sessionId, loggedInUser, refId });
    //        attachmentMsg = "Success";
    //    }
    //    catch (Exception ex) { }


    //    return attachmentMsg;
    //}

}
