using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.Login;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Login;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Common;
using rolesandpermissions = SimpliHR.Endpoints.RolesAndPermissions;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Endpoints;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.ProfileEditAuth;
using SimpliHR.Endpoints.ProfileEditAuth;
using SimpliHR.Endpoints.TicketMaster;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Endpoints.EditEmployeeData;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using DocumentFormat.OpenXml.Math;
using System.Net;
using Microsoft.AspNetCore.Session;
using static Dapper.SqlMapper;
using DocumentFormat.OpenXml.InkML;
using System.Threading;
using Microsoft.AspNetCore.Http;
//using static Org.BouncyCastle.Math.EC.ECCurve;
using SimpliHR.WebUI.BL;
using SimpliHR.Webui.Modals.Account;
using DocumentFormat.OpenXml.Spreadsheet;
using SimpliHR.Core.Entities;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace SimpliHR.WebUI.Controllers;


public class AccountController : Controller
{
    private readonly LoginController _loginApiController;
    private readonly ClientController _clientSettingAPIController;
    private readonly EmployeeMasterController _employeeAPIController;
    //public AccountController(LoginController loginApiController, ClientController clientSettingAPIController, EmployeeMasterController employeeAPIController)
    //private readonly EmployeeMasterController _employeeAPIController;
    private readonly rolesandpermissions.RolesController _rolesAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeAttendanceController _employeeAttendanceAPIController;
    private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
    private readonly ProfileEditAuthAPIController _profileEditAuthAPIController;
    private readonly TicketMasterAPIController _ticketMasterAPIController;
    private readonly EditEmployeeDataAPIController _editEmployeeDataAPIController;
    private readonly RoleMasterController _roleAPIController;
    private readonly IConfiguration _config;
    private double _sessionTimeInSeconds = (60 * 20);
    public AccountController(LoginController loginApiController, ClientController clientSettingAPIController, EmployeeMasterController employeeAPIController, rolesandpermissions.RolesController rolesAPIController, MastersKeyValueController mastersKeyValueController, EmployeeAttendanceController employeeAttendanceController, ProfileEditAuthAPIController profileEditAuthAPIController, TicketMasterAPIController ticketMasterAPIController, EditEmployeeDataAPIController editEmployeeDataAPIController, IConfiguration config, RoleMasterController roleAPIController)
    {
        _rolesAPIController = rolesAPIController;
        _loginApiController = loginApiController;
        _clientSettingAPIController = clientSettingAPIController;
        _employeeAPIController = employeeAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _employeeAttendanceAPIController = employeeAttendanceController;
        _profileEditAuthAPIController = profileEditAuthAPIController;
        _ticketMasterAPIController = ticketMasterAPIController;
        _editEmployeeDataAPIController = editEmployeeDataAPIController;
        _config = config;
        _roleAPIController = roleAPIController;

        _sessionTimeInSeconds = _config.GetValue<double>("SessionTimeInSecond");
    }

    public IActionResult Login()
    {

        HttpContext.Session.Clear();
        return View(new Login());
    }
    public IActionResult ForgetPassword()
    {
        HttpContext.Session.Clear();
        return View();
    }
    public IActionResult ForgotPassword()
    {
        HttpContext.Session.Clear();
        return View();
    }

    public IActionResult ClientDeActivate()
    {
        HttpContext.Session.Clear();
        return View();
    }

    public IActionResult Dashboard()
    {
        HttpContext.Session.Clear();
        return View();
    }

    public IActionResult Index()
    {
        //Login loginObj = new Login();
        HttpContext.Session.Clear();
        return View(new Login());
    }

    [HttpPost]
    public string CompareImageFromDB(byte[] imageBytes)
    {


        return "Success";
    }
    public IActionResult LoginAttendance()
    {
        HttpContext.Session.Clear();
        return View();
    }



    [Route("simplihr.newjoinee/{employeeId}")]
    public IActionResult LoginEJoinee(string employeeId)
    {
        HttpContext.Session.Clear();
        LoginDetailDTO loginDetailDTO = new LoginDetailDTO();
        loginDetailDTO.EncryptedEmpId = employeeId;
        return View(loginDetailDTO);
    }
    [HttpPost]
    public async Task<IActionResult> LoginEJoinee(LoginDetailDTO login)
    {


        login.LoginType = 3;
        login.EncryptedPassword = CommonHelper.Encrypt(login.Password);
        IActionResult actionResult = _loginApiController.GetNewJoineeLoginDetail(login);
        ObjectResult objResult = (ObjectResult)actionResult;
        LoginDetailDTO objResultData = (LoginDetailDTO)objResult.Value;
        HttpResponseMessage getdata = objResultData.HttpMessage;
        if (getdata.IsSuccessStatusCode)
        {
            int clientId;
            HttpContext.Session.Clear();
            if (int.TryParse(objResultData.ClientId.ToString(), out clientId))
            {
                HttpContext.Session.SetString("ClientId", clientId.ToString());
                EmployeeMasterDTO empdto = await _employeeAPIController.GetEmployeeById(objResultData.EmployeeId ?? default(int));
                HttpContext.Session.SetString("employee", JsonConvert.SerializeObject(empdto));
                HttpContext.Session.SetInt32("UnitId", empdto.UnitId ?? default(int));
                UnitMasterDTO unitStatus = await _clientSettingAPIController.GetClientUnitStatus(empdto.UnitId);
                HttpContext.Session.SetString("unit", JsonConvert.SerializeObject(unitStatus));
                HttpContext.Session.SetString("isClient", "false");
                HttpContext.Session.SetString("RoleType", "U");
                //ClientSettingDTO clientSettingDTO = await _clientSettingAPIController.GetClientSettingDetails(clientId);
                //HttpContext.Session.SetString("MenuStyle", clientSettingDTO.MenuStyle);
                //string result = getdata.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("EEmployeeDetail", "Employee", new { eEmployeeId = login.EncryptedEmpId });
            //https://localhost:7151/Employee/EmployeeDetail?employeeId=10
        }
        else
        {
            ModelState.AddModelError("LoginNotFound", "Login Failed. Please login from website in case you are not a new joinee.");
            return Redirect("/simplihr.newjoinee/" + login.EncryptedEmpId);
        }
    }

    public async Task<IActionResult> ProfileView()
    {
        string semployeeId = HttpContext.Session.GetString("EmployeeId").ToString();
        string sClientId = HttpContext.Session.GetString("ClientId").ToString();
        int employeeId;

        if (int.TryParse(HttpContext.Session.GetString("EmployeeId"), out employeeId))
        {
            EmployeeMasterDTO outputData = new EmployeeMasterDTO();
            outputData.EmployeeId = employeeId;
            //outputData = await _employeeAPIController.GetEmployeeLoginInfo(outputData);
            outputData = await _employeeAPIController.GetEmployeeInfo(outputData);
            outputData.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.ProfileImage, 0, outputData.ProfileImage.Length);
            outputData.Gender = outputData.Gender == null ? null : ((Gender)outputData.GenderId).ToString();

            outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, outputData.UnitId);

            if (sClientId != null)
            {
                if (sClientId.Trim() != "")
                {
                    int clientId = Convert.ToInt32(sClientId);
                    EmployeeKeyValues clientAdminKeyValue = await _employeeAPIController.GetClientAdminKeyValueByClientId(clientId);
                    if (outputData != null)
                    {
                        if (outputData.EmployeeMastersKeyValues != null)
                        {
                            if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues != null)
                            {
                                outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Add(clientAdminKeyValue);
                            }
                        }
                    }

                }
            }

            if (outputData.EmployeeMastersKeyValues != null)
            {
                if (outputData.EmployeeBankDetails != null)
                {
                    if (outputData.EmployeeBankDetails.Count > 0)
                    {
                        int? bankId = outputData.EmployeeBankDetails.FirstOrDefault().BankId;
                        var bankdetails = outputData.EmployeeMastersKeyValues.BankKeyValues.Where(x => x.BankId == bankId).FirstOrDefault();
                        BankMasterDTO bank = new BankMasterDTO();
                        if (bankdetails != null)
                        {
                            bank.BankName = bankdetails.BankName == null ? "" : bankdetails.BankName;
                            bank.BankId = bankdetails.BankId == null ? 0 : bankdetails.BankId;
                            outputData.EmployeeBankDetails.FirstOrDefault().Bank = bank;
                        }

                    }
                }
                AttendanceHistoryDTO ahdto = new AttendanceHistoryDTO();
                ahdto.EmployeeId = outputData.EmployeeId;
                ahdto.DutyDate = System.DateTime.Now;
                ahdto = _employeeAttendanceAPIController.GetEmployeeShiftDetails(ahdto);
                if (ahdto != null)
                {
                    outputData.CurrentShiftDetails = outputData.EmployeeMastersKeyValues.ShiftKeyValues.Where(x => x.ShiftCode == ahdto.ShiftIDAttended && x.UnitId == ahdto.UnitId).FirstOrDefault().ShiftName;
                }
            }
            ProfileEditAuthViewModel profileEditAuthViewModel = new ProfileEditAuthViewModel();
            profileEditAuthViewModel.EmployeeDetails = outputData;

            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int UnitId = empSession.UnitId ?? default(int);
            var res = await _profileEditAuthAPIController.GetProfileEditAuthTable(UnitId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    profileEditAuthViewModel.ProfileEditAuthList = (List<ProfileEditAuthDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }

            #region Ticket For Employee

            var resEmployeeEditTicketList = await _profileEditAuthAPIController.GetEmployeeEditTicketListByEmployeeId(employeeId);
            if (resEmployeeEditTicketList != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeEditTicketList).StatusCode == 200)
                {
                    profileEditAuthViewModel.EmployeeEditTicketListHistory = (List<EmployeeEditTicketViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeEditTicketList).Value;
                    if (profileEditAuthViewModel.EmployeeEditTicketListHistory != null)
                    {
                        foreach (var item in profileEditAuthViewModel.EmployeeEditTicketListHistory)
                        {
                            item.encTicketId = CommonHelper.EncryptURLHTML(item.TicketId.ToString());
                            item.SourceScreen = "EmployeeProfile";
                            item.encSourceScreen = CommonHelper.EncryptURLHTML(item.SourceScreen);
                        }
                    }
                }
            }
            #endregion
            return PartialView("_profile/_profileView", profileEditAuthViewModel);
        }
        else
        {
            return RedirectToAction("/Account/Login");
        }
    }



    public async Task<IActionResult> ProfileEdit()
    {
        string semployeeId = HttpContext.Session.GetString("EmployeeId").ToString();
        int employeeId;

        if (int.TryParse(HttpContext.Session.GetString("EmployeeId"), out employeeId))
        {
            EmployeeMasterDTO outputData = new EmployeeMasterDTO();
            outputData.EmployeeId = employeeId;
            outputData = await _employeeAPIController.GetEmployee(outputData);
            outputData.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.ProfileImage, 0, outputData.ProfileImage.Length);
            outputData.Gender = outputData.Gender == null ? null : ((Gender)outputData.GenderId).ToString();

            outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, outputData.UnitId);

            if (outputData.EmployeeMastersKeyValues != null)
            {
                if (outputData.EmployeeBankDetails != null)
                {
                    if (outputData.EmployeeBankDetails.Count > 0)
                    {
                        int? bankId = outputData.EmployeeBankDetails.FirstOrDefault().BankId;
                        var bankdetails = outputData.EmployeeMastersKeyValues.BankKeyValues.Where(x => x.BankId == bankId).FirstOrDefault();
                        BankMasterDTO bank = new BankMasterDTO();
                        bank.BankName = bankdetails.BankName;
                        bank.BankId = bankdetails.BankId;
                        outputData.EmployeeBankDetails.FirstOrDefault().Bank = bank;
                    }
                }
                AttendanceHistoryDTO ahdto = new AttendanceHistoryDTO();
                ahdto.EmployeeId = outputData.EmployeeId;
                ahdto.DutyDate = System.DateTime.Now;
                ahdto = _employeeAttendanceAPIController.GetEmployeeShiftDetails(ahdto);
                if (ahdto != null)
                {
                    outputData.CurrentShiftDetails = outputData.EmployeeMastersKeyValues.ShiftKeyValues.Where(x => x.ShiftCode == ahdto.ShiftIDAttended && x.UnitId == ahdto.UnitId).FirstOrDefault().ShiftName;
                }

            }

            ProfileEditAuthViewModel profileEditAuthViewModel = new ProfileEditAuthViewModel();
            profileEditAuthViewModel.EmployeeDetails = outputData;

            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int UnitId = empSession.UnitId ?? default(int);
            var res = await _profileEditAuthAPIController.GetProfileEditAuthTable(UnitId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    profileEditAuthViewModel.ProfileEditAuthList = (List<ProfileEditAuthDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
            return PartialView("_profile/_profileEdit", profileEditAuthViewModel);
        }
        else
        {
            return RedirectToAction("/Account/Login");
        }


    }


    public async Task<IActionResult> Profile()
    {
        EmployeeMasterDTO employeeMasterDTO = JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee"));

        if (employeeMasterDTO == null)
        {
            return RedirectToAction("Login");
        }

        employeeMasterDTO.EnycEmployeeId = CommonHelper.EncryptURLHTML(employeeMasterDTO.EmployeeId.ToString());



        return View(employeeMasterDTO);
    }

    [HttpPost]
    public async Task<EditEmployeeDataDTO> SaveEmployeeChangesForApproval(EditEmployeeDataDTO editEmployeeDataDTO)
    {
        try
        {

            var item = editEmployeeDataDTO;
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int empID = empSession.EmployeeId;
            int empCode = Convert.ToInt32(empSession.EmployeeCode);
            int? unitId = empSession.UnitId;
            int? ClientId = empSession.ClientId;

            editEmployeeDataDTO.TicketId = CommonHelper.CreateTicket("EMPEdit", editEmployeeDataDTO.TicketId); // creates a 8 digit random no.


            string changeType = item.ChangeType;

            item.EmployeeId = empID;
            item.IsApproved = 3;
            //item.TicketeId = ticketMasterDTO.TicketId;
            item.IsActive = true;
            //item.EntrySource = "EmployeeEditScreen";
            item.LoggedInUser = empCode;

            item.ClientId = ClientId;

            if (item.AttachmentFile != null)
            {
                using (var target = new MemoryStream())
                {
                    item.AttachmentFile.CopyTo(target);
                    item.Attachment = target.ToArray();
                    item.DocumentType = Path.GetExtension(item.AttachmentFile.FileName).Replace(".", "");
                }
            }

            var resprofileChangeObj = await _editEmployeeDataAPIController.SaveEmployeeChangesForApproval(item);
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resprofileChangeObj).StatusCode == 200)
            {
                editEmployeeDataDTO.DisplayMessage = "Success";

            }
            else
            {
                editEmployeeDataDTO.DisplayMessage = "Error occured while savind the data";
            }
            return editEmployeeDataDTO;
        }
        catch (Exception ex)
        {
            // return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            editEmployeeDataDTO.DisplayMessage = "Internal server error";
            return editEmployeeDataDTO;
        }
    }


    [HttpPost]
    public async Task<EditEmployeeDataVM> SaveChangesForApproval(EditEmployeeDataVM editEmployeeDataVM)
    {
        try
        {
            //CheckSession();
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int empID = empSession.EmployeeId;
            string empCode = empSession.EmployeeCode;
            int? unitId = empSession.UnitId;
            int? ClientId = empSession.ClientId;
            editEmployeeDataVM.TicketId = CommonHelper.CreateTicket("EMPEdit", editEmployeeDataVM.TicketId); // creates a 8 digit random no.
            List<EmployeeTempDocUploadDTO> empTempDocUpload = new List<EmployeeTempDocUploadDTO>();
            string changeTypes = string.Join(",", editEmployeeDataVM.EditEmployeeDataList.Select(t => { return t.ChangeType; }));
            var actionResult = await _editEmployeeDataAPIController.GetAllSessionAttachment(HttpContext.Session.Id, changeTypes);
            ObjectResult objResult = (ObjectResult)actionResult;
            List<EmployeeTempDocUploadDTO> empTempDocs = (List<EmployeeTempDocUploadDTO>)objResult.Value;
            var tab = editEmployeeDataVM.ScreenTab;
            List<EmployeeValidationDTO> empValidation = await _employeeAPIController.GetEmployeeValidation((tab == "All" ? "" : tab), ClientId.Value, unitId.Value);
            editEmployeeDataVM = await VerifyRequiredAttachment(empTempDocs, empValidation, editEmployeeDataVM);

            //Return if attachments are missing
            if (!string.IsNullOrEmpty(editEmployeeDataVM.DisplayMessage))
                return editEmployeeDataVM;
            //empTempDocUpload = (List<EmployeeTempDocUploadDTO>)objResult.Value;
            editEmployeeDataVM.EditEmployeeDataList.ForEach(x =>
            {
                x.TicketId = editEmployeeDataVM.TicketId;
                x.EmployeeId = empID;
                x.ClientId = ClientId;
                x.LoggedInUser = empID;
                x.IsActive = true;
                x.IsApproved = 3;
                x.DocumentType = empTempDocUpload.Where(r => r.FieldName.Trim().ToLower() == x.ChangeType.Trim().ToLower()).Select(p => p.DocumentType).FirstOrDefault();
                x.Attachment = empTempDocUpload.Where(r => r.FieldName.Trim().ToLower() == x.ChangeType.Trim().ToLower()).Select(p => p.UploadedFile).FirstOrDefault();
            });


            editEmployeeDataVM = await _editEmployeeDataAPIController.SaveEmployeeChangesForApproval(editEmployeeDataVM);

            return editEmployeeDataVM;
        }
        catch (Exception ex)
        {
            // return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            editEmployeeDataVM.DisplayMessage = "Internal server error " + ex.Message;
            return editEmployeeDataVM;
        }

    }

    public async Task<EditEmployeeDataVM> VerifyRequiredAttachment(List<EmployeeTempDocUploadDTO> empTempDocs, List<EmployeeValidationDTO> empValidation, EditEmployeeDataVM editEmployeeDataVM)
    {
        string sMsg = string.Empty;
        editEmployeeDataVM.EditEmployeeDataList.ForEach(x =>
        {
            if (empValidation.Where(y => (y.AddAttachment.HasValue && y.EditAttachment.Value)).ToList().Exists(r => r.FieldName == x.ChangeType) && (!empTempDocs.Exists(r => r.FieldName == x.ChangeType)))
            {
                sMsg += string.IsNullOrEmpty(sMsg) ? $"Attachment is mandatory for {CommonHelper.NewLineEntry()}{empValidation.Where(r => r.FieldName == x.ChangeType).Select(p => p.DisplayText).FirstOrDefault()}" : $"{CommonHelper.NewLineEntry()}{empValidation.Where(r => r.FieldName == x.ChangeType).Select(p => p.DisplayText).FirstOrDefault()}";
            }
        });

        editEmployeeDataVM.DisplayMessage = sMsg;
        return editEmployeeDataVM;

    }


    [HttpPost]
    public async Task<AddDeleteTableActionDTO> DeleteEmployeeDetail(AddDeleteTableActionDTO deleteEmployeeDataDTO)
    {
        try
        {
            //return Ok("Success");

            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int empID = empSession.EmployeeId;
            string empCode = empSession.EmployeeCode;
            int? unitId = empSession.UnitId;
            int? ClientId = empSession.ClientId;

            deleteEmployeeDataDTO.TicketId = CommonHelper.CreateTicket("EMPEdit", deleteEmployeeDataDTO.TicketId); // creates a 8 digit random no.

            var item = deleteEmployeeDataDTO;
            string changeType = item.ActionType;

            item.EmployeeId = empID;
            item.ActionBy = empID;
            item.ActionStatus = 0;
            //item.TicketeId = ticketMasterDTO.TicketId;
            item.IsActive = true;
            //item.EntrySource = "EmployeeEditScreen";
            item.LoggedInUser = empID;
            item.ReferenceTable = deleteEmployeeDataDTO.FormName;
            var resprofileChangeObj = await _editEmployeeDataAPIController.SaveDeleteEmployeeDetailForApproval(item);
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resprofileChangeObj).StatusCode == 200)
            {
                deleteEmployeeDataDTO.DisplayMessage = "Success";
            }
            else
            {
                deleteEmployeeDataDTO.DisplayMessage = "Error occured while savind the data";
            }
            return deleteEmployeeDataDTO;
        }
        catch (Exception ex)
        {
            // return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            deleteEmployeeDataDTO.DisplayMessage = "Internal server error";
            return deleteEmployeeDataDTO;
        }

    }


    [HttpPost]
    public async Task<ActionResult> SaveProfileDetailsByEmployee(string inputDTO, List<IFormFile> files)
    {
        try
        {

            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int empID = empSession.EmployeeId;
            string empCode = empSession.EmployeeCode;
            int? unitId = empSession.UnitId;
            List<EditEmployeeDataDTO>? editEmployeeDataDTOs = JsonConvert.DeserializeObject<List<EditEmployeeDataDTO>>(inputDTO);

            TicketMasterDTO ticketMasterDTO = new TicketMasterDTO();
            ticketMasterDTO.ModuleId = 13;
            ticketMasterDTO.TicketSource = "ProfileEditEmployee";
            ticketMasterDTO.CreatedBy = empID;
            ticketMasterDTO.CreatedOn = DateTime.Now;
            ticketMasterDTO.Status = 0;
            ticketMasterDTO.IsActive = true;
            ticketMasterDTO.UnitId = unitId;

            var resTicketObj = await _ticketMasterAPIController.CreateTicket(ticketMasterDTO, "EMP-PROFILEUPDATE-");

            if (resTicketObj != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resTicketObj).StatusCode == 200)
                {
                    var resTicket = ((Microsoft.AspNetCore.Mvc.ObjectResult)resTicketObj).Value;
                    if (resTicket != null)
                    {
                        ticketMasterDTO = (TicketMasterDTO)resTicket;


                        foreach (var item in editEmployeeDataDTOs)
                        {
                            string changeType = item.ChangeType;
                            item.AttachmentFile = files.Where(x => x.FileName == changeType).FirstOrDefault();
                            item.EmployeeId = empID;
                            item.IsApproved = 3;
                            item.TicketId = ticketMasterDTO.TicketId.ToString();
                            item.IsActive = true;
                            item.EntrySource = "Employee";
                            item.CreatedBy = empID;
                            item.CreatedOn = DateTime.Now;

                            if (item.AttachmentFile != null)
                            {
                                using (var target = new MemoryStream())
                                {
                                    item.AttachmentFile.CopyTo(target);
                                    item.Attachment = target.ToArray();
                                }
                            }
                        }

                        var resprofileChangeObj = await _editEmployeeDataAPIController.SaveProfileChangedRequest(editEmployeeDataDTOs);
                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)resprofileChangeObj).StatusCode == 200)
                        {
                            return Ok("Success");
                        }
                    }
                }
            }
            throw new Exception("Some error has occurred");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginDetailDTO login)
    {
        // MailHelper.SentEmailByZeptoSMTP();
        Login loginObj = new Login();

        var roleType = "U";

        try
        {

            //var x = 0;
            //var y = 45 / x;
            string userRole = "Administrator";
            login.EncryptedPassword = CommonHelper.Encrypt(login.Password);
            IActionResult actionResult = _loginApiController.GetLoginDetails(login);
            ObjectResult objResult = (ObjectResult)actionResult;

            LoginDetailDTO objResultData = (LoginDetailDTO)objResult.Value;
            HttpResponseMessage getdata = objResultData.HttpMessage;
            if (getdata.IsSuccessStatusCode)
            {
                string result = getdata.Content.ReadAsStringAsync().Result;
                int clientId;
                bool isClient = false;
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("Timeout", DateTime.UtcNow.AddSeconds(_sessionTimeInSeconds).ToString());
                if (int.TryParse(objResultData.ClientId.ToString(), out clientId))
                {
                    HttpContext.Session.SetString("ClientId", clientId.ToString());
                    HttpContext.Session.SetString("EmployeeId", objResultData.EmployeeId.ToString());
                    ClientSettingDTO clientSettingDTO = await _clientSettingAPIController.GetClientSettingDetails(clientId);

                    //IActionResult actionResultClient = await _clientSettingAPIController.GetClientByID(clientId);
                    //ObjectResult objResultClient = (ObjectResult)actionResultClient;
                    //ClientDTO client = (ClientDTO)objResultClient.Value;

                    HttpContext.Session.SetString("MenuStyle", clientSettingDTO.MenuStyle);
                    HttpContext.Session.SetString("Modules", clientSettingDTO.ModuleIds);

                    EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
                    employeeMasterDTO.EmployeeId = objResultData.EmployeeId ?? default(int);
                    
                    var employeeMasterDTORes = await _employeeAPIController.GetEmployeeLoginInfo(employeeMasterDTO);
                    employeeMasterDTO = CommonHelper.GetEmployeeForSession(employeeMasterDTORes);
                    if(employeeMasterDTO.EmployeeStatus!="Active")
                    {
                        ModelState.AddModelError("LoginNotFound", "Your account is not active, contact your admin.");
                        return View("Index", loginObj);
                    }
                    if (employeeMasterDTO.RoleId > 0)
                        HttpContext.Session.SetInt32("RoleId", (int)employeeMasterDTO.RoleId);
                    else
                        HttpContext.Session.SetInt32("RoleId", 0);

                    RoleMasterDTO outputData = new RoleMasterDTO();
                    outputData.RoleId = (int)HttpContext.Session.GetInt32("RoleId");

                    IActionResult actionResults;

                    actionResults = await _roleAPIController.GetRole(outputData);
                    ObjectResult objResults = (ObjectResult)actionResults;
                    var objResultDatas = (RoleMasterDTO)objResults.Value;
                    roleType = objResultDatas.RoleType;


                    if (!string.IsNullOrEmpty(roleType))
                    {
                        HttpContext.Session.SetString("RoleType", roleType.Trim());
                    }
                    else
                    {
                        roleType = "U";
                        HttpContext.Session.SetString("RoleType", roleType);
                    }
                    //if (!string.IsNullOrEmpty(roleType))
                    //    HttpContext.Session.SetString("RoleType", roleType);
                    //else
                    //    HttpContext.Session.SetString("RoleType", "A");

                    if (objResultData.LoginType == 1)
                    {
                        isClient = true;
                        userRole = "Clientadmin";
                    }
                    else
                    {
                        isClient = false;
                        userRole = "User";
                    }

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,login.UserName),
                new Claim("FullName", login.UserName),
                new Claim("EmployeeId", employeeMasterDTO.EmployeeId.ToString()),
                new Claim(ClaimTypes.Role,userRole),
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


                    RoleMenuMappingDTO roleMenuMappingDTO = new RoleMenuMappingDTO();
                    roleMenuMappingDTO.JobTitleId = employeeMasterDTO.JobTitleId;
                    roleMenuMappingDTO.DepartmentId = employeeMasterDTO.DepartmentId;
                    roleMenuMappingDTO.RoleId = employeeMasterDTO.RoleId;
                    IActionResult actionResultMapping = await _rolesAPIController.GetRoleMenuMappings(roleMenuMappingDTO, isClient, clientId, employeeMasterDTO.UnitId);
                    ObjectResult objResultMapping = (ObjectResult)actionResultMapping;
                    List<RoleMenuMappingDTO> Mapping = (List<RoleMenuMappingDTO>)objResultMapping.Value;

                    IActionResult actionResultMenu = await _rolesAPIController.GetMenus();
                    ObjectResult objResultMenu = (ObjectResult)actionResultMenu;
                    List<MenuMasterDTO> menuMasterDTOs1 = (List<MenuMasterDTO>)objResultMenu.Value;

                    var MenuIds = Mapping.Select(x => x.MenuId).ToList();
                    var ModuleIds = Array.ConvertAll(clientSettingDTO.ModuleIds.Split(",").ToArray(), int.Parse);
                    List<MenuMasterDTO> menuMasterDTOs = new List<MenuMasterDTO>();
                    if (isClient)
                        menuMasterDTOs = menuMasterDTOs1.Where(x => ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
                    else
                        menuMasterDTOs = menuMasterDTOs1.Where(x => MenuIds.Contains(x.MenuId) && ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
                    HttpContext.Session.SetString("isClient", isClient.ToString());
                    //test
                    HttpContext.Session.SetInt32("UnitId", employeeMasterDTO.UnitId ?? default(int));
                    //test end

                    UnitMasterDTO unitStatus = await _clientSettingAPIController.GetClientUnitStatus(employeeMasterDTO.UnitId);
                    HttpContext.Session.SetString("unit", JsonConvert.SerializeObject(unitStatus));
                    if (unitStatus.IsBlock == 1)
                    {
                        return RedirectToAction("ClientDeActivate", "Account");
                    }

                    HttpContext.Session.SetString("employee", JsonConvert.SerializeObject(employeeMasterDTO));
                    HttpContext.Session.SetString("MenuMapping", JsonConvert.SerializeObject(menuMasterDTOs));


                    if (objResultData.IsPasswordSetByUser != true)
                    {
                        ChangePasswordDTO changePasswordDTO = new ChangePasswordDTO();
                        changePasswordDTO.FirstTimeSettingPassword = true;
                        return View("ChangePassword", changePasswordDTO);
                    }
                    if (isClient)
                    {
                        var authProperties = new AuthenticationProperties
                        {
                            // return RedirectToAction("ClientUnitsHome", "ClientManagement");
                            //AllowRefresh = true,
                            //ExpiresUtc = DateTime.UtcNow.AddSeconds(30),
                            //IsPersistent = true,
                            RedirectUri = "/Admin/Index",

                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    }
                    else
                    {

                        int employeeId = Convert.ToInt32(objResultData.EmployeeId.ToString());
                        List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTOs = await _employeeAPIController.GetEmployeePolicies(clientId, employeeId, employeeMasterDTO.UnitId ?? default(int));

                        if (employeePolicyAcceptanceDTOs.Count(x => x.AcceptanceRequired == true && x.Accepted == false) > 0)
                        {
                            return RedirectToAction("EmployeePolicies", "Employee");

                        }
                        else
                        {

                            if (roleType.Trim().ToUpper() == "U")
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/employee/dashboard",

                                };
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }
                            else if (roleType.Trim().ToUpper() == "M")
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/Manager/Dashboard",

                                };
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }
                            else
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/Admin/Index",

                                };

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }


                            //  return RedirectToAction("Index", "Admin");
                        }
                    }



                }
                else
                {
                    userRole = "Admin";
                    roleType = "S";
                    HttpContext.Session.SetString("RoleType", roleType);

                    if (login.UserName.Trim().ToUpper() == "ADMIN")
                    {
                        HttpContext.Session.SetString("ClientId", "-1".ToString());
                        HttpContext.Session.SetString("isClient", "false");
                        HttpContext.Session.SetInt32("UnitId", -1);
                        login.EmployeeId = -1;
                        HttpContext.Session.SetString("EmployeeId", login.EmployeeId.ToString());

                        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,login.UserName),
                new Claim("FullName", login.UserName),
                   new Claim("EmployeeId", "-1"),
                new Claim(ClaimTypes.Role,userRole),
            };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            RedirectUri = "/Admin/Index",
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


                        //  return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return View();
                    }
                }
                if (isClient)
                {
                    //return RedirectToAction("UnitDashboard", "ClientManagement");
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    if (roleType.Trim().ToUpper() == "U")
                    {
                        return RedirectToAction("dashboard", "employee");
                    }
                    else if (roleType.Trim().ToUpper() == "M")
                    {
                        return RedirectToAction("dashboard", "manager");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                }

                // return View("/Admin/Index_V");
            }
            else
            {

                if (getdata.ReasonPhrase == "Not Found")
                {
                    ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
                    return View("Index", loginObj);
                }
                else
                {
                    ModelState.AddModelError("LoginNotFound", "Login Failed. " + getdata.ReasonPhrase);
                    return View("Index", loginObj);
                }
            }
        }
        catch (Exception ex)
        {

            loginObj.DisplayMessage = $"Source: {ex.Source}({nameof(Login)})\n{ex.Message}";
             string sLogPath = _config.GetValue<string>("LogFilePathName");
           // CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", loginObj.DisplayMessage);
            return View("Login", loginObj);
        }
        #region Hide
        //try
        //{

        //    //var x = 0;
        //    //var y = 45 / x;
        //    string userRole = "Administrator";
        //    login.EncryptedPassword = CommonHelper.Encrypt(login.Password);
        //    IActionResult actionResult = _loginApiController.GetLoginDetail(login);
        //    ObjectResult objResult = (ObjectResult)actionResult;

        //    LoginDetailDTO objResultData = (LoginDetailDTO)objResult.Value;
        //    HttpResponseMessage getdata = objResultData.HttpMessage;
        //    if (getdata.IsSuccessStatusCode)
        //    {
        //        string result = getdata.Content.ReadAsStringAsync().Result;
        //        int clientId;
        //        bool isClient = false;
        //        HttpContext.Session.Clear();
        //        HttpContext.Session.SetString("Timeout", DateTime.UtcNow.AddSeconds(_sessionTimeInSeconds).ToString());
        //        if (int.TryParse(objResultData.ClientId.ToString(), out clientId))
        //        {
        //            HttpContext.Session.SetString("ClientId", clientId.ToString());
        //            HttpContext.Session.SetString("EmployeeId", objResultData.EmployeeId.ToString());
        //            ClientSettingDTO clientSettingDTO = await _clientSettingAPIController.GetClientSettingDetails(clientId);

        //            //IActionResult actionResultClient = await _clientSettingAPIController.GetClientByID(clientId);
        //            //ObjectResult objResultClient = (ObjectResult)actionResultClient;
        //            //ClientDTO client = (ClientDTO)objResultClient.Value;

        //            HttpContext.Session.SetString("MenuStyle", clientSettingDTO.MenuStyle);
        //            HttpContext.Session.SetString("Modules", clientSettingDTO.ModuleIds);

        //            EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
        //            employeeMasterDTO.EmployeeId = objResultData.EmployeeId ?? default(int);
        //            //employeeMasterDTO = await _employeeAPIController.GetEmployee(employeeMasterDTO);
        //            //var employeeMasterDTORes = await _employeeAPIController.GetEmployee(employeeMasterDTO);
        //            var employeeMasterDTORes = await _employeeAPIController.GetEmployeeInfo(employeeMasterDTO);
        //            employeeMasterDTO = CommonHelper.GetEmployeeForSession(employeeMasterDTORes);

        //            if (objResultData.LoginType == 1)
        //            {
        //                isClient = true;
        //                userRole = "Clientadmin";
        //            }
        //            else
        //            {
        //                isClient = false;
        //                userRole = "User";
        //            }

        //            var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name,login.UserName),
        //        new Claim("FullName", login.UserName),
        //        new Claim(ClaimTypes.Role,userRole),
        //    };

        //            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //            //var authProperties = new AuthenticationProperties
        //            //{
        //            //    AllowRefresh = true,
        //            //    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
        //            //    IsPersistent = true
        //            //    //,
        //            //    //RedirectUri = "https://localhost:44318/Account/Logout"
        //            //};
        //            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);

        //            RoleMenuMappingDTO roleMenuMappingDTO = new RoleMenuMappingDTO();
        //            roleMenuMappingDTO.JobTitleId = employeeMasterDTO.JobTitleId;
        //            roleMenuMappingDTO.DepartmentId = employeeMasterDTO.DepartmentId;
        //            roleMenuMappingDTO.RoleId = employeeMasterDTO.RoleId;
        //            IActionResult actionResultMapping = await _rolesAPIController.GetRoleMenuMappings(roleMenuMappingDTO, isClient, clientId, employeeMasterDTO.UnitId);
        //            ObjectResult objResultMapping = (ObjectResult)actionResultMapping;
        //            List<RoleMenuMappingDTO> Mapping = (List<RoleMenuMappingDTO>)objResultMapping.Value;

        //            IActionResult actionResultMenu = await _rolesAPIController.GetMenus();
        //            ObjectResult objResultMenu = (ObjectResult)actionResultMenu;
        //            List<MenuMasterDTO> menuMasterDTOs1 = (List<MenuMasterDTO>)objResultMenu.Value;

        //            var MenuIds = Mapping.Select(x => x.MenuId).ToList();
        //            var ModuleIds = Array.ConvertAll(clientSettingDTO.ModuleIds.Split(",").ToArray(), int.Parse);
        //            List<MenuMasterDTO> menuMasterDTOs = new List<MenuMasterDTO>();
        //            if (isClient)
        //                menuMasterDTOs = menuMasterDTOs1.Where(x => ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
        //            else
        //                menuMasterDTOs = menuMasterDTOs1.Where(x => MenuIds.Contains(x.MenuId) && ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
        //            HttpContext.Session.SetString("isClient", isClient.ToString());
        //            //test
        //            HttpContext.Session.SetInt32("UnitId", employeeMasterDTO.UnitId ?? default(int));
        //            //test end

        //            UnitMasterDTO unitStatus = await _clientSettingAPIController.GetClientUnitStatus(employeeMasterDTO.UnitId);
        //            HttpContext.Session.SetString("unit", JsonConvert.SerializeObject(unitStatus));
        //            if (unitStatus.IsBlock == 1)
        //            {
        //                return RedirectToAction("ClientDeActivate", "Account");
        //            }

        //            HttpContext.Session.SetString("employee", JsonConvert.SerializeObject(employeeMasterDTO));
        //            HttpContext.Session.SetString("MenuMapping", JsonConvert.SerializeObject(menuMasterDTOs));


        //            if (objResultData.IsPasswordSetByUser != true)
        //            {
        //                ChangePasswordDTO changePasswordDTO = new ChangePasswordDTO();
        //                changePasswordDTO.FirstTimeSettingPassword = true;
        //                return View("ChangePassword", changePasswordDTO);
        //            }
        //            if (isClient)
        //            {
        //                var authProperties = new AuthenticationProperties
        //                {
        //                    // return RedirectToAction("ClientUnitsHome", "ClientManagement");
        //                    //AllowRefresh = true,
        //                    //ExpiresUtc = DateTime.UtcNow.AddSeconds(30),
        //                    //IsPersistent = true,
        //                    RedirectUri = "/ClientManagement/ClientUnitsHome",

        //                };

        //                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        //            }
        //            else
        //            {

        //                int employeeId = Convert.ToInt32(objResultData.EmployeeId.ToString());
        //                List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTOs = await _employeeAPIController.GetEmployeePolicies(clientId, employeeId, employeeMasterDTO.UnitId ?? default(int));

        //                if (employeePolicyAcceptanceDTOs.Count(x => x.AcceptanceRequired == true && x.Accepted == false) > 0)
        //                {
        //                    return RedirectToAction("EmployeePolicies", "Employee");

        //                }
        //                else
        //                {

        //                    var authProperties = new AuthenticationProperties
        //                    {
        //                        // return RedirectToAction("ClientUnitsHome", "ClientManagement");
        //                        //AllowRefresh = true,
        //                        //ExpiresUtc = DateTime.UtcNow.AddSeconds(30),
        //                        //IsPersistent = true,
        //                        RedirectUri = "/Admin/Index",

        //                    };

        //                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        //                    //  return RedirectToAction("Index", "Admin");
        //                }
        //            }



        //        }
        //        else
        //        {
        //            userRole = "Admin";
        //            if (login.UserName.Trim().ToUpper() == "ADMIN")
        //            {
        //                HttpContext.Session.SetString("ClientId", "-1".ToString());
        //                HttpContext.Session.SetString("isClient", "false");
        //                HttpContext.Session.SetInt32("UnitId", -1);
        //                login.EmployeeId = -1;
        //                HttpContext.Session.SetString("EmployeeId", login.EmployeeId.ToString());

        //                var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name,login.UserName),
        //        new Claim("FullName", login.UserName),
        //        new Claim(ClaimTypes.Role,userRole),
        //    };

        //                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //                var authProperties = new AuthenticationProperties
        //                {
        //                    // return RedirectToAction("ClientUnitsHome", "ClientManagement");
        //                    //AllowRefresh = true,
        //                    //ExpiresUtc = DateTime.UtcNow.AddSeconds(30),
        //                    //IsPersistent = true,
        //                    RedirectUri = "/Admin/Index",

        //                };

        //                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


        //                //  return RedirectToAction("Index", "Admin");
        //            }
        //            else
        //            {
        //                return View();
        //            }
        //        }
        //        if (isClient)
        //        {
        //            return RedirectToAction("ClientUnitsHome", "ClientManagement");
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Admin");
        //        }




        //    }
        //    else
        //    {

        //        if (getdata.ReasonPhrase == "Not Found")
        //        {
        //            ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
        //            return View(loginObj);
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("LoginNotFound", "Login Failed. " + getdata.ReasonPhrase);
        //            return View(loginObj);
        //        }


        //    }
        //}
        //catch (Exception ex)
        //{

        //    loginObj.DisplayMessage = $"Source: {ex.Source}({nameof(Login)})\n{ex.Message}";
        //    string sLogPath = _config.GetValue<string>("LogFilePathName");
        //    // CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", loginObj.DisplayMessage);
        //    return View(loginObj);
        //}

        #endregion

        //string baseURL = "https://localhost:7097/api/Login/";

        //using (HttpClient client = new HttpClient())
        //{
        //    client.BaseAddress = new Uri(baseURL);
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
        //    HttpResponseMessage getdata = await client.PostAsJsonAsync<Modals.Account.Login>("GetEmployee", login);
        //    if (getdata.IsSuccessStatusCode)
        //    {
        //        string result = getdata.Content.ReadAsStringAsync().Result;
        //        return RedirectToAction("Index", "Admin");
        //        // return View("/Admin/Index_V");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

    }

    [HttpPost]
    //public async Task<IActionResult> LoginEmployee([FromBody] LoginDetailDTO login)
    public async Task<IActionResult> LoginEmployee(LoginDetailDTO login)
    {
        // return Ok();
        Login loginObj = new Login();

        var roleType = "U";

        try
        {


            string userRole = "Administrator";
            login.EncryptedPassword = CommonHelper.Encrypt(login.Password);
            IActionResult actionResult = _loginApiController.GetLoginDetails(login);
            ObjectResult objResult = (ObjectResult)actionResult;

            LoginDetailDTO objResultData = (LoginDetailDTO)objResult.Value;
            HttpResponseMessage getdata = objResultData.HttpMessage;
            if (getdata.IsSuccessStatusCode)
            {
                string result = getdata.Content.ReadAsStringAsync().Result;
                int clientId;
                bool isClient = false;
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("Timeout", DateTime.UtcNow.AddSeconds(_sessionTimeInSeconds).ToString());
                if (int.TryParse(objResultData.ClientId.ToString(), out clientId))
                {
                    HttpContext.Session.SetString("ClientId", clientId.ToString());
                    HttpContext.Session.SetString("EmployeeId", objResultData.EmployeeId.ToString());
                    ClientSettingDTO clientSettingDTO = await _clientSettingAPIController.GetClientSettingDetails(clientId);

                    //IActionResult actionResultClient = await _clientSettingAPIController.GetClientByID(clientId);
                    //ObjectResult objResultClient = (ObjectResult)actionResultClient;
                    //ClientDTO client = (ClientDTO)objResultClient.Value;

                    HttpContext.Session.SetString("MenuStyle", clientSettingDTO.MenuStyle);
                    HttpContext.Session.SetString("Modules", clientSettingDTO.ModuleIds);

                    EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
                    employeeMasterDTO.EmployeeId = objResultData.EmployeeId ?? default(int);
                    //employeeMasterDTO = await _employeeAPIController.GetEmployee(employeeMasterDTO);
                    //var employeeMasterDTORes = await _employeeAPIController.GetEmployee(employeeMasterDTO);
                    var employeeMasterDTORes = await _employeeAPIController.GetEmployeeInfo(employeeMasterDTO);
                    employeeMasterDTO = CommonHelper.GetEmployeeForSession(employeeMasterDTORes);
                    if (employeeMasterDTO.RoleId > 0)
                        HttpContext.Session.SetInt32("RoleId", (int)employeeMasterDTO.RoleId);
                    else
                        HttpContext.Session.SetInt32("RoleId", 0);

                    RoleMasterDTO outputData = new RoleMasterDTO();
                    outputData.RoleId = (int)HttpContext.Session.GetInt32("RoleId");

                    IActionResult actionResults;

                    actionResults = await _roleAPIController.GetRole(outputData);
                    ObjectResult objResults = (ObjectResult)actionResults;
                    var objResultDatas = (RoleMasterDTO)objResults.Value;
                    roleType = objResultDatas.RoleType;


                    if (!string.IsNullOrEmpty(roleType))
                    {
                        HttpContext.Session.SetString("RoleType", roleType.Trim());
                    }
                    else
                    {
                        roleType = "U";
                        HttpContext.Session.SetString("RoleType", roleType);
                    }
                    //if (!string.IsNullOrEmpty(roleType))
                    //    HttpContext.Session.SetString("RoleType", roleType);
                    //else
                    //    HttpContext.Session.SetString("RoleType", "A");

                    if (objResultData.LoginType == 1)
                    {
                        isClient = true;
                        userRole = "Clientadmin";
                    }
                    else
                    {
                        isClient = false;
                        userRole = "User";
                    }

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,login.UserName),
                new Claim("FullName", login.UserName),
                new Claim(ClaimTypes.Role,userRole),
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


                    RoleMenuMappingDTO roleMenuMappingDTO = new RoleMenuMappingDTO();
                    roleMenuMappingDTO.JobTitleId = employeeMasterDTO.JobTitleId;
                    roleMenuMappingDTO.DepartmentId = employeeMasterDTO.DepartmentId;
                    roleMenuMappingDTO.RoleId = employeeMasterDTO.RoleId;
                    IActionResult actionResultMapping = await _rolesAPIController.GetRoleMenuMappings(roleMenuMappingDTO, isClient, clientId, employeeMasterDTO.UnitId);
                    ObjectResult objResultMapping = (ObjectResult)actionResultMapping;
                    List<RoleMenuMappingDTO> Mapping = (List<RoleMenuMappingDTO>)objResultMapping.Value;

                    IActionResult actionResultMenu = await _rolesAPIController.GetMenus();
                    ObjectResult objResultMenu = (ObjectResult)actionResultMenu;
                    List<MenuMasterDTO> menuMasterDTOs1 = (List<MenuMasterDTO>)objResultMenu.Value;

                    var MenuIds = Mapping.Select(x => x.MenuId).ToList();
                    var ModuleIds = Array.ConvertAll(clientSettingDTO.ModuleIds.Split(",").ToArray(), int.Parse);
                    List<MenuMasterDTO> menuMasterDTOs = new List<MenuMasterDTO>();
                    if (isClient)
                        menuMasterDTOs = menuMasterDTOs1.Where(x => ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
                    else
                        menuMasterDTOs = menuMasterDTOs1.Where(x => MenuIds.Contains(x.MenuId) && ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
                    HttpContext.Session.SetString("isClient", isClient.ToString());
                    //test
                    HttpContext.Session.SetInt32("UnitId", employeeMasterDTO.UnitId ?? default(int));
                    //test end

                    UnitMasterDTO unitStatus = await _clientSettingAPIController.GetClientUnitStatus(employeeMasterDTO.UnitId);
                    HttpContext.Session.SetString("unit", JsonConvert.SerializeObject(unitStatus));
                    if (unitStatus.IsBlock == 1)
                    {
                        return RedirectToAction("ClientDeActivate", "Account");
                    }

                    HttpContext.Session.SetString("employee", JsonConvert.SerializeObject(employeeMasterDTO));
                    HttpContext.Session.SetString("MenuMapping", JsonConvert.SerializeObject(menuMasterDTOs));


                    if (objResultData.IsPasswordSetByUser != true)
                    {
                        ChangePasswordDTO changePasswordDTO = new ChangePasswordDTO();
                        changePasswordDTO.FirstTimeSettingPassword = true;
                        return View("ChangePassword", changePasswordDTO);
                    }
                    if (isClient)
                    {
                        var authProperties = new AuthenticationProperties
                        {
                            // return RedirectToAction("ClientUnitsHome", "ClientManagement");
                            //AllowRefresh = true,
                            //ExpiresUtc = DateTime.UtcNow.AddSeconds(30),
                            //IsPersistent = true,
                            RedirectUri = "/ClientManagement/UnitDashboard",

                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    }
                    else
                    {

                        int employeeId = Convert.ToInt32(objResultData.EmployeeId.ToString());
                        List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTOs = await _employeeAPIController.GetEmployeePolicies(clientId, employeeId, employeeMasterDTO.UnitId ?? default(int));

                        if (employeePolicyAcceptanceDTOs.Count(x => x.AcceptanceRequired == true && x.Accepted == false) > 0)
                        {
                            return RedirectToAction("EmployeePolicies", "Employee");

                        }
                        else
                        {

                            if (roleType.Trim().ToUpper() == "U")
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/employee/dashboard",

                                };
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }
                            else if (roleType.Trim().ToUpper() == "M")
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/Manager/Dashboard",

                                };
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }
                            else
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/Admin/Index",

                                };

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }


                            //  return RedirectToAction("Index", "Admin");
                        }
                    }



                }
                else
                {
                    userRole = "Admin";
                    roleType = "S";
                    HttpContext.Session.SetString("RoleType", roleType);

                    if (login.UserName.Trim().ToUpper() == "ADMIN")
                    {
                        HttpContext.Session.SetString("ClientId", "-1".ToString());
                        HttpContext.Session.SetString("isClient", "false");
                        HttpContext.Session.SetInt32("UnitId", -1);
                        login.EmployeeId = -1;
                        HttpContext.Session.SetString("EmployeeId", login.EmployeeId.ToString());

                        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,login.UserName),
                new Claim("FullName", login.UserName),
                new Claim(ClaimTypes.Role,userRole),
            };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            RedirectUri = "/Admin/Index",
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


                        //  return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return View();
                    }
                }
                if (isClient)
                {
                    return RedirectToAction("UnitDashboard", "ClientManagement");
                }
                else
                {
                    if (roleType.Trim().ToUpper() == "U")
                    {
                        return RedirectToAction("dashboard", "employee");
                    }
                    else if (roleType.Trim().ToUpper() == "M")
                    {
                        return RedirectToAction("dashboard", "manager");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                }



                // return View("/Admin/Index_V");
            }
            else
            {

                if (getdata.ReasonPhrase == "Not Found")
                {
                    ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
                    return View("Index", loginObj);
                }
                else
                {
                    ModelState.AddModelError("LoginNotFound", "Login Failed. " + getdata.ReasonPhrase);
                    return View("Index", loginObj);
                }


            }
        }
        catch (Exception ex)
        {

            loginObj.DisplayMessage = $"Source: {ex.Source}({nameof(Login)})\n{ex.Message}";
            string sLogPath = _config.GetValue<string>("LogFilePathName");
            // CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", loginObj.DisplayMessage);
            return View("Login", loginObj);
        }


    }



    //[HttpGet]
    public async Task<IActionResult> ErrorMessage()
    {
        return View();
    }
    public async Task<IActionResult> ExceptionMessage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginUser(LoginDetailDTO login)
    {
        var roleType = "U";
        //int i = 0;
        //var r = 2 / i;
        Login loginObj = new Login();
        try
        {

            //var x = 0;
            //var y = 45 / x;
            string userRole = "Administrator";
            login.EncryptedPassword = CommonHelper.Encrypt(login.Password);
            IActionResult actionResult = _loginApiController.GetLoginDetails(login);
            ObjectResult objResult = (ObjectResult)actionResult;

            LoginDetailDTO objResultData = (LoginDetailDTO)objResult.Value;
            HttpResponseMessage getdata = objResultData.HttpMessage;
            if (getdata.IsSuccessStatusCode)
            {
                string result = getdata.Content.ReadAsStringAsync().Result;
                int clientId;
                bool isClient = false;
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("Timeout", DateTime.UtcNow.AddSeconds(_sessionTimeInSeconds).ToString());
                if (int.TryParse(objResultData.ClientId.ToString(), out clientId))
                {
                    HttpContext.Session.SetString("ClientId", clientId.ToString());
                    HttpContext.Session.SetString("EmployeeId", objResultData.EmployeeId.ToString());
                    ClientSettingDTO clientSettingDTO = await _clientSettingAPIController.GetClientSettingDetails(clientId);

                    //IActionResult actionResultClient = await _clientSettingAPIController.GetClientByID(clientId);
                    //ObjectResult objResultClient = (ObjectResult)actionResultClient;
                    //ClientDTO client = (ClientDTO)objResultClient.Value;

                    HttpContext.Session.SetString("MenuStyle", clientSettingDTO.MenuStyle);
                    HttpContext.Session.SetString("Modules", clientSettingDTO.ModuleIds);

                    EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
                    employeeMasterDTO.EmployeeId = objResultData.EmployeeId ?? default(int);
                    //employeeMasterDTO = await _employeeAPIController.GetEmployee(employeeMasterDTO);
                    //var employeeMasterDTORes = await _employeeAPIController.GetEmployee(employeeMasterDTO);
                    var employeeMasterDTORes = await _employeeAPIController.GetEmployeeInfo(employeeMasterDTO);
                    employeeMasterDTO = CommonHelper.GetEmployeeForSession(employeeMasterDTORes);
                    if (employeeMasterDTO.RoleId > 0)
                        HttpContext.Session.SetInt32("RoleId", (int)employeeMasterDTO.RoleId);
                    else
                        HttpContext.Session.SetInt32("RoleId", 0);

                    RoleMasterDTO outputData = new RoleMasterDTO();
                    outputData.RoleId = (int)HttpContext.Session.GetInt32("RoleId");

                    IActionResult actionResults;

                    actionResults = await _roleAPIController.GetRole(outputData);
                    ObjectResult objResults = (ObjectResult)actionResults;
                    var objResultDatas = (RoleMasterDTO)objResults.Value;
                    roleType = objResultDatas.RoleType;

                    if (!string.IsNullOrEmpty(roleType))
                    {
                        HttpContext.Session.SetString("RoleType", roleType.Trim());
                    }
                    else
                    {
                        roleType = "U";
                        HttpContext.Session.SetString("RoleType", roleType);
                    }


                    if (objResultData.LoginType == 1)
                    {
                        isClient = true;
                        userRole = "Clientadmin";
                    }
                    else
                    {
                        isClient = false;
                        userRole = "User";
                    }

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,login.UserName),
                new Claim("FullName", login.UserName),
                new Claim(ClaimTypes.Role,userRole),
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


                    RoleMenuMappingDTO roleMenuMappingDTO = new RoleMenuMappingDTO();
                    roleMenuMappingDTO.JobTitleId = employeeMasterDTO.JobTitleId;
                    roleMenuMappingDTO.DepartmentId = employeeMasterDTO.DepartmentId;
                    roleMenuMappingDTO.RoleId = employeeMasterDTO.RoleId;
                    IActionResult actionResultMapping = await _rolesAPIController.GetRoleMenuMappings(roleMenuMappingDTO, isClient, clientId, employeeMasterDTO.UnitId);
                    ObjectResult objResultMapping = (ObjectResult)actionResultMapping;
                    List<RoleMenuMappingDTO> Mapping = (List<RoleMenuMappingDTO>)objResultMapping.Value;

                    IActionResult actionResultMenu = await _rolesAPIController.GetMenus();
                    ObjectResult objResultMenu = (ObjectResult)actionResultMenu;
                    List<MenuMasterDTO> menuMasterDTOs1 = (List<MenuMasterDTO>)objResultMenu.Value;

                    var MenuIds = Mapping.Select(x => x.MenuId).ToList();
                    var ModuleIds = Array.ConvertAll(clientSettingDTO.ModuleIds.Split(",").ToArray(), int.Parse);
                    List<MenuMasterDTO> menuMasterDTOs = new List<MenuMasterDTO>();
                    if (isClient)
                        menuMasterDTOs = menuMasterDTOs1.Where(x => ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
                    else
                        menuMasterDTOs = menuMasterDTOs1.Where(x => MenuIds.Contains(x.MenuId) && ModuleIds.Contains(x.ModuleId ?? default(int))).ToList();
                    HttpContext.Session.SetString("isClient", isClient.ToString());
                    //test
                    HttpContext.Session.SetInt32("UnitId", employeeMasterDTO.UnitId ?? default(int));
                    //test end

                    UnitMasterDTO unitStatus = await _clientSettingAPIController.GetClientUnitStatus(employeeMasterDTO.UnitId);
                    HttpContext.Session.SetString("unit", JsonConvert.SerializeObject(unitStatus));
                    if (unitStatus.IsBlock == 1)
                    {
                        return RedirectToAction("ClientDeActivate", "Account");
                    }

                    HttpContext.Session.SetString("employee", JsonConvert.SerializeObject(employeeMasterDTO));
                    HttpContext.Session.SetString("MenuMapping", JsonConvert.SerializeObject(menuMasterDTOs));


                    if (objResultData.IsPasswordSetByUser != true)
                    {
                        ChangePasswordDTO changePasswordDTO = new ChangePasswordDTO();
                        changePasswordDTO.FirstTimeSettingPassword = true;
                        return View("ChangePassword", changePasswordDTO);
                    }
                    if (isClient)
                    {
                        var authProperties = new AuthenticationProperties
                        {
                            // return RedirectToAction("ClientUnitsHome", "ClientManagement");
                            //AllowRefresh = true,
                            //ExpiresUtc = DateTime.UtcNow.AddSeconds(30),
                            //IsPersistent = true,
                            RedirectUri = "/ClientManagement/UnitDashboard",

                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    }
                    else
                    {

                        int employeeId = Convert.ToInt32(objResultData.EmployeeId.ToString());
                        List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTOs = await _employeeAPIController.GetEmployeePolicies(clientId, employeeId, employeeMasterDTO.UnitId ?? default(int));

                        if (employeePolicyAcceptanceDTOs.Count(x => x.AcceptanceRequired == true && x.Accepted == false) > 0)
                        {
                            return RedirectToAction("EmployeePolicies", "Employee");

                        }
                        else
                        {

                            if (roleType.Trim().ToUpper() == "U")
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/employee/dashboard",

                                };
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }
                            else if (roleType.Trim().ToUpper() == "M")
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/Manager/Dashboard",

                                };
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }
                            else
                            {
                                var authProperties = new AuthenticationProperties
                                {
                                    RedirectUri = "/Admin/Index",

                                };

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }


                            //  return RedirectToAction("Index", "Admin");
                        }
                    }



                }
                else
                {
                    userRole = "Admin";
                    roleType = "S";
                    HttpContext.Session.SetString("RoleType", roleType);
                    if (login.UserName.Trim().ToUpper() == "ADMIN")
                    {
                        HttpContext.Session.SetString("ClientId", "-1".ToString());
                        HttpContext.Session.SetString("isClient", "false");
                        HttpContext.Session.SetInt32("UnitId", -1);
                        login.EmployeeId = -1;
                        HttpContext.Session.SetString("RoleType", roleType);
                        HttpContext.Session.SetString("EmployeeId", login.EmployeeId.ToString());

                        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,login.UserName),
                new Claim("FullName", login.UserName),
                new Claim(ClaimTypes.Role,userRole),
            };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            RedirectUri = "/Admin/Index",
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


                        //  return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return View();
                    }
                }
                if (isClient)
                {
                    roleType = "A";
                    return RedirectToAction("UnitDashboard", "ClientManagement");
                }
                else
                {
                    if (roleType.Trim().ToUpper() == "U")
                    {
                        return RedirectToAction("dashboard", "employee");
                    }
                    else if (roleType.Trim().ToUpper() == "M")
                    {
                        return RedirectToAction("dashboard", "manager");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                }



                // return View("/Admin/Index_V");
            }
            else
            {

                if (getdata.ReasonPhrase == "Not Found")
                {
                    ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
                    return View("Index", loginObj);
                }
                else
                {
                    ModelState.AddModelError("LoginNotFound", "Login Failed. " + getdata.ReasonPhrase);
                    return View("Index", loginObj);
                }


            }
        }
        catch (Exception ex)
        {

            loginObj.DisplayMessage = $"Source: {ex.Source}({nameof(Login)})\n{ex.Message}";
            string sLogPath = _config.GetValue<string>("LogFilePathName");
            // CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", loginObj.DisplayMessage);
            return View("Index", loginObj);
        }

        //string baseURL = "https://localhost:7097/api/Login/";

        //using (HttpClient client = new HttpClient())
        //{
        //    client.BaseAddress = new Uri(baseURL);
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
        //    HttpResponseMessage getdata = await client.PostAsJsonAsync<Modals.Account.Login>("GetEmployee", login);
        //    if (getdata.IsSuccessStatusCode)
        //    {
        //        string result = getdata.Content.ReadAsStringAsync().Result;
        //        return RedirectToAction("Index", "Admin");
        //        // return View("/Admin/Index_V");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

    }

    public ActionResult ChangePassword()
    {
        string SEmployeeId = HttpContext.Session.GetString("EmployeeId");

        if (string.IsNullOrEmpty(SEmployeeId))
        {
            return RedirectToAction("Login", "Account");
        }
        //int EmployeeId = Convert.ToInt32(SEmployeeId);
        return View();
    }

    public ActionResult Change_Password()
    {
        string SEmployeeId = HttpContext.Session.GetString("EmployeeId");

        if (string.IsNullOrEmpty(SEmployeeId))
        {
            return RedirectToAction("Login", "Account");
        }
        //int EmployeeId = Convert.ToInt32(SEmployeeId);
        return View();
    }

    public ActionResult AdminEncryptionPage()
    {
        AdminEncryption inp = new AdminEncryption();
        return View(inp);
    }
    public ActionResult EncryptInput(AdminEncryption input)
    {
        input.EncryptOutput = CommonHelper.Encrypt(input.EncryptInput);
        return View("AdminEncryptionPage", input);
    }
    public ActionResult DecryptInput(AdminEncryption input)
    {
        input.DecryptOutput = CommonHelper.Decrypt(input.DecryptInput);
        return View("AdminEncryptionPage", input);
    }


    [Route("simplihr.resetpassword/{employeeId}")]
    public IActionResult ResetPasswordByEmployee(string employeeId)
    {
        ForgetPasswordDTO loginDetailDTO = new ForgetPasswordDTO();
        loginDetailDTO.EncryptedEmpId = employeeId;
        loginDetailDTO.EmployeeId = Convert.ToInt32(CommonHelper.Decrypt(employeeId));
        return View(loginDetailDTO);
    }

    [HttpPost]
    public async Task<ActionResult> ResetPasswordByEmployee(ForgetPasswordDTO inputData)
    {
        if (!ModelState.IsValid)
        {
            return View(inputData);
        }

        LoginDetailDTO loginDetailDTO = _loginApiController.GetLoginByEmployeeId(inputData.EmployeeId ?? default(int));

        loginDetailDTO.Password = CommonHelper.Encrypt(inputData.NewPassword);



        bool res = await _loginApiController.UpdateLogin(loginDetailDTO, "Password");
        if (res == true)
        {
            return Redirect("/Account/Login");
        }
        else
        {
            inputData.DisplayMessage = "Unable to set the password at the moment. Try again later";
            return View(inputData);
        }

        return View(inputData);
    }

    [HttpPost]
    public async Task<ActionResult> ForgetPassword(ForgetPasswordDTO inputData)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(inputData);
            }
            else
            {
                var res = _loginApiController.CheckEmailExists(inputData);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        var employeeObj = ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                        if (employeeObj != null)
                        {
                            EmployeeMasterDTO employee = (EmployeeMasterDTO)employeeObj;
                            if (employee != null)
                            {
                                BL.Employee blemp = new BL.Employee();
                                UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
                                ClientSettingDTO outputData = await _clientSettingAPIController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));
                                employee.EmailProvider = outputData.EmailProvider;
                                blemp.SendResetEmailLink(employee, unit);
                            }
                            else
                            {
                                throw new Exception("Unable to find the email");
                            }
                        }
                        else
                        {
                            throw new Exception("Unable to find the email");
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to find the email");
                    }
                }
                else
                {
                    throw new Exception("Unable to find the email");
                }
            }
            ModelState.AddModelError("LoginNotFound", "The password reset link has been sent to your mail.");
            return View(inputData);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("LoginNotFound", ex.Message);
            return View();
        }
    }


    [HttpPost]
    public async Task<ActionResult> ForgotPassword(ForgetPasswordDTO inputData)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(inputData);
            }
            else
            {
                var res = _loginApiController.CheckEmailExists(inputData);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        var employeeObj = ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                        if (employeeObj != null)
                        {
                            EmployeeMasterDTO employee = (EmployeeMasterDTO)employeeObj;
                            if (employee != null)
                            {
                                UnitMasterDTO? unit = new UnitMasterDTO();
                                BL.Employee blemp = new BL.Employee();
                                // UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
                                ClientSettingDTO outputData = await _clientSettingAPIController.GetClientSettingDetails(Convert.ToInt32(employee.ClientId));
                                employee.EmailProvider = outputData.EmailProvider;
                                // blemp.SendChangePwdEmailLink(objResultData, unit, outputData.EmailProvider);
                                blemp.SendResetEmailLink(employee, unit);
                            }
                            else
                            {
                                throw new Exception("Unable to find the email");
                            }
                        }
                        else
                        {
                            throw new Exception("Unable to find the email");
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to find the email");
                    }
                }
                else
                {
                    throw new Exception("Unable to find the email");
                }
            }
            ModelState.AddModelError("LoginNotFound", "The password reset link has been sent to your mail.");
            return View(inputData);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("LoginNotFound", ex.Message);
            return View();
        }
    }
    [HttpPost]
    public async Task<ActionResult> ChangePassword(ChangePasswordDTO inputData)
    {
        if (!ModelState.IsValid)
        {
            return View(inputData);
        }
        BL.Employee blemp = new BL.Employee();
        string SEmployeeId = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrEmpty(SEmployeeId))
        {
            return Redirect("/Account/Index");
        }
        inputData.EmployeeId = Convert.ToInt32(SEmployeeId);

        inputData.OldPasswordEncrypted = CommonHelper.Encrypt(inputData.OldPassword);
        inputData.NewPasswordEncrypted = CommonHelper.Encrypt(inputData.NewPassword);


        IActionResult actionResult = _loginApiController.CheckLoginPasswordExists(inputData);
        ObjectResult objResult = (ObjectResult)actionResult;
        LoginDetailDTO objResultData = (LoginDetailDTO)objResult.Value;
        HttpResponseMessage getdata = objResultData.HttpMessage;
        if (getdata.IsSuccessStatusCode)
        {
            //LoginDetailDTO loginDetailDTO = new LoginDetailDTO();
            //objResultData.LoginId = objResultData.LoginId;
            objResultData.Password = inputData.NewPasswordEncrypted;
            objResultData.IsPasswordSetByUser = true;
            bool res = await _loginApiController.UpdateLogin(objResultData, "Password,IsPasswordSetByUser");
            if (res == true)
            {
                objResultData.Password = CommonHelper.Decrypt(inputData.NewPasswordEncrypted);
                UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
                ClientSettingDTO outputData = await _clientSettingAPIController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));
                blemp.SendChangePwdEmailLink(objResultData, unit, outputData.EmailProvider);
                return Redirect("/Account/Index");
            }
            else
            {
                inputData.DisplayMessage = "Unable to set the password at the moment. Try again later";
                return View(inputData);
            }
        }
        else
        {
            inputData.DisplayMessage = "Old password do not match";
            return View(inputData);
        }

        return View(inputData);
    }

    [HttpPost]
    public async Task<ActionResult> Change_Password(ChangePasswordDTO inputData)
    {
        if (!ModelState.IsValid)
        {
            return View(inputData);
        }
        BL.Employee blemp = new BL.Employee();
        string SEmployeeId = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrEmpty(SEmployeeId))
        {
            return Redirect("/Account/Index");
        }
        inputData.EmployeeId = Convert.ToInt32(SEmployeeId);

        inputData.OldPasswordEncrypted = CommonHelper.Encrypt(inputData.OldPassword);
        inputData.NewPasswordEncrypted = CommonHelper.Encrypt(inputData.NewPassword);


        IActionResult actionResult = _loginApiController.CheckLoginPasswordExists(inputData);
        ObjectResult objResult = (ObjectResult)actionResult;
        LoginDetailDTO objResultData = (LoginDetailDTO)objResult.Value;
        HttpResponseMessage getdata = objResultData.HttpMessage;
        if (getdata.IsSuccessStatusCode)
        {
            //LoginDetailDTO loginDetailDTO = new LoginDetailDTO();
            //objResultData.LoginId = objResultData.LoginId;
            objResultData.Password = inputData.NewPasswordEncrypted;
            objResultData.IsPasswordSetByUser = true;
            bool res = await _loginApiController.UpdateLogin(objResultData, "Password,IsPasswordSetByUser");
            if (res == true)
            {
                objResultData.Password = CommonHelper.Decrypt(inputData.NewPasswordEncrypted);
                UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
                blemp.SendChangePwdEmailLink(objResultData, unit);
                return Redirect("/Account/Index");
            }
            else
            {
                inputData.DisplayMessage = "Unable to set the password at the moment. Try again later";
                return View(inputData);
            }
        }
        else
        {
            inputData.DisplayMessage = "Old password do not match";
            return View(inputData);
        }

        return View(inputData);
    }



    [HttpGet]
    public async Task<IActionResult> Ping()
    {
        DateTime timeout = new DateTime();
        double serverSessionTime = 0;
        DateTime sessionTime = Convert.ToDateTime(HttpContext.Session.GetString("Timeout"));
        var timeDiff = sessionTime.Subtract(DateTime.UtcNow);
        if (timeDiff.Seconds > 0)
        {
            HttpContext.Session.SetString("Timeout", DateTime.UtcNow.AddSeconds(_sessionTimeInSeconds).ToString());
            sessionTime = Convert.ToDateTime(HttpContext.Session.GetString("Timeout"));
            timeDiff = sessionTime.Subtract(DateTime.UtcNow);
            serverSessionTime = Math.Round(timeDiff.TotalSeconds, 0);
        }
        return Ok(serverSessionTime);
    }

    //private string GenerateToken(LoginDetailDTO user,string role)
    //{
    //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
    //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    //    _ = int.TryParse(_config["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);
    //    var claims = new[]
    //    {
    //            new Claim(ClaimTypes.NameIdentifier,user.UserName),
    //            new Claim(ClaimTypes.Role,role)
    //        };
    //    var token = new JwtSecurityToken(_config["Jwt:Issuer"],
    //        _config["Jwt:Audience"],
    //        claims,
    //        expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
    //        signingCredentials: credentials);


    //    return new JwtSecurityTokenHandler().WriteToken(token);

    //}



}