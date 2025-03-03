
using Microsoft.AspNetCore.Components.Forms;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SimpliHR.Endpoints.Login;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Login;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Webui.Modals.Account;
using System.Net;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.WebUI.BL;
using System.Web;
using Org.BouncyCastle.Asn1.Cms;
using SimpliHR.Core.Entities;
using System.Linq.Expressions;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.X509;
using System.Drawing.Drawing2D;
using SimpliHR.Services.DBContext;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Infrastructure.Models.ClientManagement;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SimpliHR.Endpoints.EditEmployeeData;
using SimpliHR.Endpoints.TicketMaster;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Endpoints.ProfileEditAuth;
using SimpliHR.Infrastructure.Models.Page;
using SimpliHR.Infrastructure.Models.ProfileEditAuth;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using SimpliHR.Infrastructure.Models.Exit;
using Humanizer;
using SimpliHR.Endpoints.Leave;
using SimpliHR.Infrastructure.Models.Leave;
using OfficeOpenXml.FormulaParsing.FormulaExpressions;
using SimpliHR.Endpoints.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using HtmlAgilityPack;
using iTextSharp.tool.xml.html;
using Microsoft.IdentityModel.Tokens;
using iTextSharp.text.pdf.codec.wmf;
using SimpliHR.Endpoints;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.Common;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using SimpliHR.Infrastructure.Models.GoogleCalendar;
using Masters.Controllers;
using Org.BouncyCastle.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
//using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Security.Claims;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Globalization;
using ProjectTracker.Controllers;
using SimpliHR.Infrastructure.Models.ProjectTracker;
//using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SimpliHR.WebUI.Controllers.Employee;

public class EmployeeController : Controller
{
    private readonly EmployeeMasterController _employeeAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly LoginController _loginAPIController;
    private readonly PolicyDocumentController _policyDocumentAPIController;
    private readonly ClientController _clientController;
    private Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnv;
    private ProfileEditAuthAPIController _profileEditAuthAPIController;
    private readonly LeaveAPIController _leaveAPIController;
    private readonly EmployeeNewsAPIController _employeeNewsAPIController;
    private readonly EmployeeAnnouncementAPIController _employeeAnnouncementAPIController;
    private readonly EmployeeAttendanceController _employeeAttendanceAPIController;
    private readonly QuickAccessAPIController _quickaccessAPIController;
    private readonly IConfiguration _config;
    static string[] Scopes = { CalendarService.Scope.Calendar };
    static string ApplicationName = "Google Calendar API .NET Quickstart";
    private readonly ProjectAPIController _projectAPIController;

    
    private readonly SchedularController _SchedularController;
    //private readonly IDNTCaptchaValidatorService _ValidatorService;
    //private readonly DNTCaptchaOptions _CaptchaOptions;
    public EmployeeController(EmployeeMasterController EmployeeAPIController, MastersKeyValueController mastersKeyValueController, LoginController loginController, PolicyDocumentController policyDocumentAPIController, ClientController clientController, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ProfileEditAuthAPIController profileEditAuthAPIController, LeaveAPIController leaveAPIController, EmployeeNewsAPIController employeeNewsAPIController, EmployeeAnnouncementAPIController employeeAnnouncementAPIController, EmployeeAttendanceController employeeAttendanceAPIController, QuickAccessAPIController quickaccessAPIController, IConfiguration config, SchedularController schedularController, ProjectAPIController projectAPIController)
    {
        _employeeAPIController = EmployeeAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _loginAPIController = loginController;
        _policyDocumentAPIController = policyDocumentAPIController;
        _clientController = clientController;
        this.hostingEnv = env;
        _profileEditAuthAPIController = profileEditAuthAPIController;
        _leaveAPIController = leaveAPIController;
        _employeeNewsAPIController = employeeNewsAPIController;
        _employeeAnnouncementAPIController = employeeAnnouncementAPIController;
        _employeeAttendanceAPIController = employeeAttendanceAPIController;
        _quickaccessAPIController = quickaccessAPIController;
        _SchedularController = schedularController;
        _config = config;
        _projectAPIController = projectAPIController;
    }


    public async Task<IActionResult> QuickAccessSettings()
    {
        return View();
    }
    [Route("Employee/Dashboard")]
    public async Task<IActionResult> EmployeeDashboard()
    {

        //var logPath = Path.Combine(this.hostingEnv.WebRootPath, "log");

        //CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "Khan11222");


        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }

        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion

        int? unitId = HttpContext.Session.GetInt32("UnitId");
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();
        int employeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        if (employeeId != 0)
        {
            outputData.TotalLeaveBalance = 0;
            outputData.TotalAvailed = 0;
            // Employee Details
            outputData.EmployeeMaster.EmployeeId = employeeId;
            outputData.EmployeeMaster = await _employeeAPIController.GetEmployeeInfo(outputData.EmployeeMaster);
            //var birthDay = await _employeeAPIController.GetEmployeeBirthDayInfo(outputData.EmployeeMaster);
            //var onBoardingEmployes = await _employeeAPIController.GetOnBoardingEmployeeInfo(outputData.EmployeeMaster);
            outputData.EmployeeMaster.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.EmployeeMaster.ProfileImage, 0, outputData.EmployeeMaster.ProfileImage.Length);

            IActionResult actionResultAnnivesary = await _employeeAPIController.GetAnnivesaryEmployeeInfo(unitId);
            ObjectResult objResultAnnivesary = (ObjectResult)actionResultAnnivesary;
            outputData.EmployeeAnnivesary = (List<EmployeeDashboardDetailsDTO>)objResultAnnivesary.Value;
            foreach (var item in outputData.EmployeeAnnivesary)
            {
                if (item.ProfileImage != null)
                    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            }

            //  outputData.EmployeeBirthDays = (List<EmployeeDashboardDetailsDTO>)await _employeeAPIController.GetEmployeeBirthDayInfo(outputData.EmployeeMaster);
            IActionResult actionResultBirth = await _employeeAPIController.GetEmployeeBirthDayInfo(unitId);
            ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
            outputData.EmployeeBirthDays = (List<EmployeeDashboardDetailsDTO>)objResultBirth.Value;
            foreach (var item in outputData.EmployeeBirthDays)
            {
                if (item.ProfileImage != null)
                    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            }

            IActionResult actionResultOnBoarding = await _employeeAPIController.GetOnBoardingEmployeeInfo(unitId);
            ObjectResult objResultOnBoarding = (ObjectResult)actionResultOnBoarding;
            outputData.EmployeeOnBoardings = (List<EmployeeDashboardDetailsDTO>)objResultOnBoarding.Value;
            foreach (var item in outputData.EmployeeOnBoardings)
            {
                if (item.ProfileImage != null)
                    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            }

            // Leave Details
            IActionResult actionResult = await _leaveAPIController.GetEmployeeLeaveSummary(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, employeeId, 0);
            ObjectResult objResult = (ObjectResult)actionResult;
            outputData.EmployeeLeaveSummary = (List<EmployeeLeaveBalanceDTO>)objResult.Value;
            if (outputData.EmployeeLeaveSummary.Count > 0)
            {
                for (int i = 0; i < outputData.EmployeeLeaveSummary.Count; i++)
                {
                    outputData.TotalLeaveBalance = outputData.TotalLeaveBalance + outputData.EmployeeLeaveSummary[i].LeaveBalance;
                    outputData.TotalAvailed = outputData.TotalAvailed + outputData.EmployeeLeaveSummary[i].TotalAvailed;
                }
            }

            attendanceHistoryVM.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            attendanceHistoryVM.EndDate = attendanceHistoryVM.StartDate.Value.AddMonths(1).AddDays(-1);
            // Attendance Details
            attendanceHistoryVM.EmployeeId = employeeId;
            UnitMaster unitMaster = await _employeeAttendanceAPIController.GetUnitPayrollDates(empSession.UnitId);
            if (unitMaster != null)
            {
                attendanceHistoryVM.StartDate = (unitMaster.PayrollStartDate == 0 || unitMaster.PayrollStartDate == null) ? attendanceHistoryVM.StartDate : new DateTime(DateTime.Today.Year, DateTime.Today.Month, unitMaster.PayrollStartDate.Value);
                attendanceHistoryVM.EndDate = DateTime.Now;

                outputData.AttendanceHistory = await _employeeAttendanceAPIController.GetEmployeeAttendance(attendanceHistoryVM, 1000, 0);

                outputData.CurrentDate = Convert.ToDateTime(attendanceHistoryVM.EndDate).ToString("dd-MM-yyyy");
            }

            outputData.QuickAccessDetail = await _quickaccessAPIController.GetQuickAccessUnitDetails(empSession.UnitId);

            //outputData.GoogleEvents = CalendarEvents();

            #region News Tags
            var resNewsCategoryTags = await _employeeNewsAPIController.NewsCategoryTagsForDashboard(empSession.UnitId ?? default(int));
            if (resNewsCategoryTags != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resNewsCategoryTags).StatusCode == 200)
                {
                    outputData.NewsCategoryTags = (List<NewsCategoryTagMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resNewsCategoryTags).Value;
                }
            }
            #endregion
            #region AnnouncementTypes
            var resAnnouncementType = await _employeeAnnouncementAPIController.AnnouncementTypesForDashboard(empSession.UnitId ?? default(int));
            if (resAnnouncementType != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resAnnouncementType).StatusCode == 200)
                {
                    outputData.AnnouncementTypes = (List<AnnouncementTypeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAnnouncementType).Value;
                }
            }
            #endregion

        }
        return View(outputData);
    }
    [Route("Manager/Dashboard")]
    public async Task<IActionResult> ManagerDashboard()
    {
        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        #endregion

        int? unitId = HttpContext.Session.GetInt32("UnitId");
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();
        //  EmployeeLeaveBalanceDTO input = new 
        // int? unitId = empSession.UnitId;
        int employeeId = empSession.EmployeeId; //Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        if (employeeId != 0)
        {
            outputData.TotalLeaveBalance = 0;
            outputData.TotalAvailed = 0;

            outputData.EmployeeMaster.EmployeeId = employeeId;
            outputData.EmployeeMaster = await _employeeAPIController.GetEmployeeInfo(outputData.EmployeeMaster);
            outputData.EmployeeMaster.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.EmployeeMaster.ProfileImage, 0, outputData.EmployeeMaster.ProfileImage.Length);

            IActionResult actionResult = await _leaveAPIController.GetEmployeeLeaveSummary(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, employeeId, 0);
            ObjectResult objResult = (ObjectResult)actionResult;
            outputData.EmployeeLeaveSummary = (List<EmployeeLeaveBalanceDTO>)objResult.Value;

            if (outputData.EmployeeLeaveSummary.Count > 0)
            {
                for (int i = 0; i < outputData.EmployeeLeaveSummary.Count; i++)
                {
                    outputData.TotalLeaveBalance = outputData.TotalLeaveBalance + outputData.EmployeeLeaveSummary[i].LeaveBalance;
                    outputData.TotalAvailed = outputData.TotalAvailed + outputData.EmployeeLeaveSummary[i].TotalAvailed;
                }
            }

            IActionResult actionResultAnnivesary = await _employeeAPIController.GetAnnivesaryEmployeeInfo(unitId);
            ObjectResult objResultAnnivesary = (ObjectResult)actionResultAnnivesary;
            outputData.EmployeeAnnivesary = (List<EmployeeDashboardDetailsDTO>)objResultAnnivesary.Value;
            foreach (var item in outputData.EmployeeAnnivesary)
            {
                if (item.ProfileImage != null)
                    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            }

            //  outputData.EmployeeBirthDays = (List<EmployeeDashboardDetailsDTO>)await _employeeAPIController.GetEmployeeBirthDayInfo(outputData.EmployeeMaster);
            IActionResult actionResultBirth = await _employeeAPIController.GetEmployeeBirthDayInfo(unitId);
            ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
            outputData.EmployeeBirthDays = (List<EmployeeDashboardDetailsDTO>)objResultBirth.Value;
            foreach (var item in outputData.EmployeeBirthDays)
            {
                if (item.ProfileImage != null)
                    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            }

            IActionResult actionResultOnBoarding = await _employeeAPIController.GetOnBoardingEmployeeInfo(unitId);
            ObjectResult objResultOnBoarding = (ObjectResult)actionResultOnBoarding;
            outputData.EmployeeOnBoardings = (List<EmployeeDashboardDetailsDTO>)objResultOnBoarding.Value;
            foreach (var item in outputData.EmployeeOnBoardings)
            {
                if (item.ProfileImage != null)
                    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            }
            //IActionResult actionResultAnnivesary = await _employeeAPIController.GetAnnivesaryEmployeeInfo(outputData.EmployeeMaster);
            //ObjectResult objResultAnnivesary = (ObjectResult)actionResultAnnivesary;
            //outputData.EmployeeAnnivesary = (List<EmployeeMasterDTO>)objResultAnnivesary.Value;
            //foreach (var item in outputData.EmployeeAnnivesary)
            //{
            //    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            //}

            //IActionResult actionResultBirth = await _employeeAPIController.GetEmployeeBirthDayInfo(outputData.EmployeeMaster);
            //ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
            //outputData.EmployeeBirthDays = (List<EmployeeMasterDTO>)objResultBirth.Value;
            //foreach (var item in outputData.EmployeeBirthDays)
            //{
            //    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            //}

            //IActionResult actionResultOnBoarding = await _employeeAPIController.GetOnBoardingEmployeeInfo(outputData.EmployeeMaster);
            //ObjectResult objResultOnBoarding = (ObjectResult)actionResultOnBoarding;
            //outputData.EmployeeOnBoardings = (List<EmployeeMasterDTO>)objResultOnBoarding.Value;
            //foreach (var item in outputData.EmployeeOnBoardings)
            //{
            //    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            //}
            //outputData.EmployeeLeaveDetail = await _leaveAPIController.GetLeavePendingForApproval(Convert.ToInt32(employeeId));


            //IActionResult actionResultCompOff = await _leaveAPIController.GetEmployeeCompOffInfo(employeeId, empSession.UnitId);
            //ObjectResult objResultCompOff = (ObjectResult)actionResultCompOff;
            //outputData.EmployeeCompOffList = (List<EmployeeCompOffDTO>)objResultCompOff.Value;

            // Attendance Details
            attendanceHistoryVM.EmployeeId = employeeId;
            UnitMaster unitMaster = await _employeeAttendanceAPIController.GetUnitPayrollDates(empSession.UnitId);

            attendanceHistoryVM.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            attendanceHistoryVM.EndDate = attendanceHistoryVM.StartDate.Value.AddMonths(1).AddDays(-1);
            if (unitMaster != null)
            {
                attendanceHistoryVM.StartDate = (unitMaster.PayrollStartDate == 0 || unitMaster.PayrollStartDate == null) ? attendanceHistoryVM.StartDate : new DateTime(DateTime.Today.Year, DateTime.Today.Month, unitMaster.PayrollStartDate.Value);
                attendanceHistoryVM.EndDate = DateTime.Now;

                outputData.AttendanceHistory = await _employeeAttendanceAPIController.GetEmployeeAttendance(attendanceHistoryVM, 1000, 0);

                outputData.CurrentDate = Convert.ToDateTime(attendanceHistoryVM.EndDate).ToString("dd-MM-yyyy");
            }
            outputData.QuickAccessDetail = await _quickaccessAPIController.GetQuickAccessUnitDetails(empSession.UnitId);


            //  outputData.manualPunchVM = await _employeeAttendanceAPIController.GetAttendancePendingForApproval(employeeId);

            #region News Tags
            var resNewsCategoryTags = await _employeeNewsAPIController.NewsCategoryTagsForDashboard(empSession.UnitId ?? default(int));
            if (resNewsCategoryTags != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resNewsCategoryTags).StatusCode == 200)
                {
                    outputData.NewsCategoryTags = (List<NewsCategoryTagMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resNewsCategoryTags).Value;
                }
            }
            #endregion
            #region AnnouncementTypes
            var resAnnouncementType = await _employeeAnnouncementAPIController.AnnouncementTypesForDashboard(empSession.UnitId ?? default(int));
            if (resAnnouncementType != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resAnnouncementType).StatusCode == 200)
                {
                    outputData.AnnouncementTypes = (List<AnnouncementTypeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAnnouncementType).Value;
                }
            }
            #endregion

        }
        return View(outputData);
    }

    public async Task<IActionResult> CelebrationCornorView()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();

        IActionResult actionResultAnnivesary = await _employeeAPIController.GetAnnivesaryEmployeeInfo(unitId);
        ObjectResult objResultAnnivesary = (ObjectResult)actionResultAnnivesary;
        outputData.EmployeeAnnivesary = (List<EmployeeDashboardDetailsDTO>)objResultAnnivesary.Value;
        foreach (var item in outputData.EmployeeAnnivesary)
        {
            if (item.ProfileImage != null)
                item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
        }

        //  outputData.EmployeeBirthDays = (List<EmployeeDashboardDetailsDTO>)await _employeeAPIController.GetEmployeeBirthDayInfo(outputData.EmployeeMaster);
        IActionResult actionResultBirth = await _employeeAPIController.GetEmployeeBirthDayInfo(unitId);
        ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
        outputData.EmployeeBirthDays = (List<EmployeeDashboardDetailsDTO>)objResultBirth.Value;
        foreach (var item in outputData.EmployeeBirthDays)
        {
            if (item.ProfileImage != null)
                item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
        }

        IActionResult actionResultOnBoarding = await _employeeAPIController.GetOnBoardingEmployeeInfo(unitId);
        ObjectResult objResultOnBoarding = (ObjectResult)actionResultOnBoarding;
        outputData.EmployeeOnBoardings = (List<EmployeeDashboardDetailsDTO>)objResultOnBoarding.Value;
        foreach (var item in outputData.EmployeeOnBoardings)
        {
            if (item.ProfileImage != null)
                item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
        }
        return View(outputData);
    }
    public async Task<IActionResult> ApprovalCentres([FromBody] EmployeeCompOffDTO inputDTO)
    {

        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();

        #region Employee News
        outputData.EmployeeLeaveDetail = await _leaveAPIController.GetLeavePendingForApproval(empSession.EmployeeId, empSession.UnitId, 0);
        IActionResult actionResultCompOff = await _leaveAPIController.GetEmployeeCompOffInfo(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, empSession.EmployeeId, empSession.UnitId);
        ObjectResult objResultCompOff = (ObjectResult)actionResultCompOff;
        outputData.EmployeeCompOffList = (List<EmployeeCompOffDTO>)objResultCompOff.Value;
        outputData.manualPunchVM = await _employeeAttendanceAPIController.GetAttendancePendingForApproval(empSession.EmployeeId);
        // return View("ApprovalCentre", outputData);
        #endregion

        return PartialView("ApprovalCentre/_approvalCentre", outputData);
    }
    public async Task<IActionResult> QuickAccess([FromBody] QuickAccessUnitListDTO inputDTO)
    {

        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();

        #region Employee News
        outputData.QuickAccessDetail = await _quickaccessAPIController.GetQuickAccessUnitDetails(empSession.UnitId);
        #endregion

        return PartialView("QuickAccess/_quickAccess", outputData);
    }
    public async Task<IActionResult> NewsForDashboard([FromBody] NewsCategoryTagMasterDTO inputDTO)
    {

        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();

        #region Employee News
        var resEmployeeNews = await _employeeNewsAPIController.EmployeeNewsListForDashboard(empSession.UnitId ?? default(int), inputDTO.NewsCategoryTagId);
        if (resEmployeeNews != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeNews).StatusCode == 200)
            {
                outputData.EmployeeNews = (List<EmployeeNewsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeNews).Value;
                if (outputData.EmployeeNews != null)
                {
                    foreach (var item in outputData.EmployeeNews)
                    {
                        item.encEmployeeNewsId = CommonHelper.EncryptURLHTML(item.EmployeeNewsId.ToString());
                        var doc = new HtmlDocument();
                        doc.LoadHtml(item.Article);
                        string extractedText = doc.DocumentNode.InnerText;
                        HtmlNodeCollection pTags = doc.DocumentNode.SelectNodes("//p");
                        string pText = "";
                        if (pTags != null)
                        {
                            foreach (HtmlNode pTag in pTags)
                            {
                                pText = pTag.InnerText;
                                if (!String.IsNullOrEmpty(pText))
                                {
                                    //if (pText.Length > 200)
                                    //{
                                    //    pText = pText.Substring(0, 200);
                                    //}
                                    break;
                                }
                            }
                        }
                        //var firstPTag = doc.DocumentNode.SelectSingleNode("//p");                            
                        if (!String.IsNullOrEmpty(pText))
                            item.Article = pText;
                        item.Article = extractedText;
                    }
                }
            }
        }

        outputData.Source = inputDTO.Source;
        #endregion

        return PartialView("_dashboards/_news", outputData);
    }

    public async Task<IActionResult> ProjectsForDashboard([FromBody] ProjectPageDetails inputDTO)
    {
        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion

        #region Projects
        ProjectViewModel viewModel = new ProjectViewModel();
        if (!String.IsNullOrEmpty(inputDTO.Source))
        {
            viewModel.Source = CommonHelper.EncryptURLHTML(inputDTO.Source);
        }
        var res = await _projectAPIController.ProjectListProjectStatusWiseAndPageWise(empSession.UnitId ?? default(int), empSession.EmployeeId, inputDTO, "Data");

        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                viewModel.ProjectWithChildList = (List<ProjectWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (viewModel.ProjectWithChildList != null)
                {

                    foreach (var item in viewModel.ProjectWithChildList)
                    {
                        if (item != null)
                        {
                            item.encProjectID = CommonHelper.EncryptURLHTML(item.ProjectID.ToString());
                        }
                    }
                    int[] ProjectIDs = viewModel.ProjectWithChildList.Select(x => x.ProjectID).ToArray();
                    if (ProjectIDs.Length > 0)
                    {
                        var teamMembersRes = await _projectAPIController.GetTeamMembersByProjectIdArray(ProjectIDs);
                        if (teamMembersRes != null)
                        {
                            if (((Microsoft.AspNetCore.Mvc.ObjectResult)teamMembersRes).StatusCode == 200)
                            {
                                viewModel.ProjectMembersWithChild = (List<PTProjectMemberWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)teamMembersRes).Value;

                                if (viewModel.ProjectMembersWithChild != null && viewModel.ProjectMembersWithChild.Any())
                                {
                                    foreach (var item in viewModel.ProjectMembersWithChild)
                                    {
                                        if (item.ProfileImage != null)
                                            item.UserProfileImagePath = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        return PartialView("_dashboards/_projectTracker", viewModel);
    }

    public async Task<IActionResult> AnnouncementsForDashboard([FromBody] AnnouncementTypeMasterDTO inputDTO)
    {

        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        #endregion
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();

        #region Employee Announcement
        var resEmployeeAnnouncement = await _employeeAnnouncementAPIController.EmployeeAnnouncementListForDashboard(empSession.UnitId ?? default(int), empSession.DepartmentId ?? default(int), inputDTO.AnnouncementTypeId);
        if (resEmployeeAnnouncement != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeAnnouncement).StatusCode == 200)
            {
                outputData.EmployeeAnnouncements = (List<EmployeeAnnouncementDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeAnnouncement).Value;
                if (outputData.EmployeeAnnouncements != null)
                {
                    foreach (var item in outputData.EmployeeAnnouncements)
                    {
                        item.encEmployeeAnnouncementId = CommonHelper.EncryptURLHTML(item.EmployeeAnnouncementId.ToString());
                        var doc = new HtmlDocument();
                        doc.LoadHtml(item.Description);
                        string extractedText = doc.DocumentNode.InnerText;
                        HtmlNodeCollection pTags = doc.DocumentNode.SelectNodes("//p");
                        string pText = "";
                        if (pTags != null)
                        {
                            foreach (HtmlNode pTag in pTags)
                            {
                                pText = pTag.InnerText;
                                if (!String.IsNullOrEmpty(pText))
                                {
                                    //if (pText.Length > 200)
                                    //{
                                    //    pText = pText.Substring(0, 200);
                                    //}
                                    break;
                                }
                            }
                        }
                        //var firstPTag = doc.DocumentNode.SelectSingleNode("//p");                            
                        if (!String.IsNullOrEmpty(pText))
                            item.Description = pText;
                        item.Description = extractedText;
                    }
                }
            }
        }

        outputData.Source = inputDTO.Source;

        #endregion

        return PartialView("_dashboards/_announcements", outputData);
    }

    public async Task<ActionResult> EmployeePolicies()
    {
        int clientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        int employeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        int? unitId = HttpContext.Session.GetInt32("UnitId");

        if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && int.TryParse(HttpContext.Session.GetString("EmployeeId"), out employeeId) && unitId != null)
        {
            List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTOs = await _employeeAPIController.GetEmployeePolicies(clientId, employeeId, unitId ?? default(int));
            return View(employeePolicyAcceptanceDTOs);
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }
    }



    public async Task<JsonResult> AcceptPolicies(int[] PolicyDocumentIds)
    {
        int clientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        int employeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        //string employeeCode = HttpContext.Session.GetString("EmployeeCode");

        EmployeeMasterDTO employeeMasterDTO = JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee"));

        var res = _employeeAPIController.DeletePolicyAcceptance(employeeId);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.StatusCodeResult)res).StatusCode == 200)
            {
                List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTO = new List<EmployeePolicyAcceptanceDTO>();
                foreach (var item in PolicyDocumentIds)
                {
                    employeePolicyAcceptanceDTO.Add(new EmployeePolicyAcceptanceDTO
                    {
                        PolicyDocumentsMasterId = item,
                        EmployeeId = employeeId,
                        EmployeeCode = employeeMasterDTO.EmployeeCode == null ? "" : employeeMasterDTO.EmployeeCode,
                        Accepted = true,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    });
                }
                _employeeAPIController.SavePolicyAcceptanceByRange(employeePolicyAcceptanceDTO);
            }
        }
        List<EmployeePolicyAcceptanceDTO> employeePolicyAcceptanceDTOs = await _employeeAPIController.GetEmployeePolicies(clientId, employeeId, unitId ?? default(int));
        if (employeePolicyAcceptanceDTOs.Count(x => x.AcceptanceRequired == true && x.Accepted == false) > 0)
        {
            return Json(new { message = "Incomplete" });
        }
        else
        {
            return Json(new { message = "Complete" });
        }
    }

    public async Task<FileResult> DownloadPolicies(int documentId)
    {
        IActionResult actionResult = await _policyDocumentAPIController.GetPolicyDocumentByID(documentId);
        ObjectResult objResult = (ObjectResult)actionResult;
        PolicyDocumentsMasterDTO objResultData = (PolicyDocumentsMasterDTO)objResult.Value;

        string path = objResultData.PolicyDocumentPath;
        string extension = Path.GetExtension(path);
        string contentType = CommonHelper.getContentTypeByExtesnion(extension);



        return File(path, contentType, objResultData.PolicyDocument);
        //return View(employeePolicyAcceptanceDTOs);
    }

    //[Authorize(Roles = "Clientadmin")]
    public async Task<IActionResult> EmployeeDetail(string eEmployeeId = "")
    {
        CheckSession();

        int clientId;
        if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
        {
            return RedirectToAction("Login", "Account");
        }
        EmployeeMasterVM outputData = new EmployeeMasterVM();
        //string eId = CommonHelper.Encrypt("1");

        eEmployeeId = HttpUtility.UrlDecode(eEmployeeId);

        string eId = CommonHelper.Encrypt(eEmployeeId);

        int employeeId = 0;
        if (eEmployeeId != "")
        {
            employeeId = Convert.ToInt32(CommonHelper.Decrypt(eEmployeeId));
        }
        outputData.Opt = "Add";

        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (employeeId != 0)
        {

           
            outputData.EmployeeId = employeeId;
            outputData.EmployeeMaster.EmployeeId = employeeId;
            //outputData.EmployeeMaster = await _employeeAPIController.GetEmployee(outputData.EmployeeMaster);
            outputData.EmployeeMaster = await _employeeAPIController.GetEmployeeInfo(outputData.EmployeeMaster);

            

            outputData = await _employeeAPIController.GetEmployeeTabFillingStatus(outputData);
            if (outputData.EmployeeMaster.InfoFillingStatus == 1)
                outputData.Opt = "Edit";
            //outputData.EmployeeValidationList = await _employeeAPIController.GetEmployee(outputData.EmployeeMaster);

            if (outputData.EmployeeMaster.EmployeeCode == null || outputData.EmployeeMaster.EmployeeCode == "")
            {
                outputData.EmployeeMaster.EmployeeCode = outputData.EmployeeId.ToString();

            }
            outputData.EnycEmployeeId = eEmployeeId;
            if (outputData.EmployeeMaster.ProfileImage != null)
                outputData.EmployeeMaster.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.EmployeeMaster.ProfileImage, 0, outputData.EmployeeMaster.ProfileImage.Length);
        }


        outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        outputData.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, outputData.UnitId, outputData.ClientId);
        outputData.EmployeeMastersKeyValues.EmployeeValidations = await _mastersKeyValueController.EmployeeValidationKeyValue("EmployeeMaster", "", clientId, outputData.UnitId);


        EmployeeKeyValues clientAdminKeyValue = new EmployeeKeyValues();
        
        clientAdminKeyValue.EmployeeId = empSession.EmployeeId;
        clientAdminKeyValue.EmployeeName = empSession.EmployeeName;
        outputData.LogInEmployeeId = empSession.EmployeeId;
        if (outputData != null && outputData.EmployeeMastersKeyValues != null && outputData.EmployeeMastersKeyValues.EmployeeKeyValues != null)
        {
            if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Where(x => x.EmployeeId == empSession.EmployeeId).Count() == 0)
            {
                outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Add(clientAdminKeyValue);
            }
        }

        outputData.UnitMaster = await _clientController.GetClientUnitById(outputData.UnitId ?? default(int));

        if (outputData.UnitMaster != null)
        {
            if (outputData.EmployeeMaster != null && outputData.EmployeeMaster.ConfirmationPeriod == null)
                outputData.EmployeeMaster.ConfirmationPeriod = outputData.UnitMaster.ConfirmationPeriod;

            if (outputData.EmployeeMaster != null && outputData.EmployeeMaster.NoticePeriod == null)
                outputData.EmployeeMaster.NoticePeriod = outputData.UnitMaster.NoticePeriod;
        }

        return View("Employee", outputData);

    }

    public async Task<IActionResult> EditEmployee(string eEmployeeId = "")
    {
        CheckSession();

        int clientId;
        if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
        {
            return RedirectToAction("Login", "Account");
        }
        EmployeeMasterVM outputData = new EmployeeMasterVM();
        //string eId = CommonHelper.Encrypt("1");
        outputData.TicketId = CommonHelper.CreateTicket("EMPEdit", outputData.TicketId);
        eEmployeeId = HttpUtility.UrlDecode(eEmployeeId);

        string eId = CommonHelper.Encrypt(eEmployeeId);

        int employeeId = 0;
        if (eEmployeeId != "")
            employeeId = Convert.ToInt32(CommonHelper.Decrypt(eEmployeeId));

        if (employeeId != 0)
        {
            outputData.EmployeeId = employeeId;
            outputData.EmployeeMaster.EmployeeId = employeeId;
            //outputData.EmployeeMaster = await _employeeAPIController.GetEmployee(outputData.EmployeeMaster);
            outputData.EmployeeMaster = await _employeeAPIController.GetEmployeeInfo(outputData.EmployeeMaster);
            //outputData.EmployeeValidationList = await _employeeAPIController.GetEmployee(outputData.EmployeeMaster);

            if (outputData.EmployeeMaster.EmployeeCode == null || outputData.EmployeeMaster.EmployeeCode == "")
            {
                outputData.EmployeeMaster.EmployeeCode = outputData.EmployeeId.ToString();
            }
            outputData.EmployeeMaster.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.EmployeeMaster.ProfileImage, 0, outputData.EmployeeMaster.ProfileImage.Length);
        }

        outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        outputData.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, outputData.UnitId, outputData.ClientId);
        outputData.EmployeeMastersKeyValues.EmployeeValidations = await _mastersKeyValueController.EmployeeValidationKeyValue("EmployeeMaster", "", clientId, outputData.UnitId);


        EmployeeKeyValues clientAdminKeyValue = new EmployeeKeyValues();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

        clientAdminKeyValue.EmployeeId = empSession.EmployeeId;
        clientAdminKeyValue.EmployeeName = empSession.EmployeeName;
        if (outputData != null)
        {
            if (outputData.EmployeeMastersKeyValues != null)
            {
                if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues != null)
                {
                    if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Where(x => x.EmployeeId == empSession.EmployeeId).Count() == 0)
                    {
                        outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Add(clientAdminKeyValue);
                    }
                }
            }
        }

        outputData.UnitMaster = await _clientController.GetClientUnitById(outputData.UnitId ?? default(int));

        if (outputData.UnitMaster != null)
        {
            if (outputData.ConfirmationPeriod == null)
            {
                outputData.ConfirmationPeriod = outputData.UnitMaster.ConfirmationPeriod;
            }
            if (outputData.NoticePeriod == null)
            {
                outputData.NoticePeriod = outputData.UnitMaster.NoticePeriod;
            }
        }
        outputData.EnycEmployeeId = eEmployeeId;
        return View("EditEmployee", outputData);
    }

    public async Task<IActionResult> EmployeeEdit(string eEmployeeId = "")
    {
        CheckSession();

        int clientId;
        if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
        {
            return RedirectToAction("Login", "Account");
        }
        EmployeeMasterVM outputData = new EmployeeMasterVM();
        //string eId = CommonHelper.Encrypt("1");

        eEmployeeId = HttpUtility.UrlDecode(eEmployeeId);

        string eId = CommonHelper.Encrypt(eEmployeeId);

        int employeeId = 0;
        if (eEmployeeId != "")
            employeeId = Convert.ToInt32(CommonHelper.Decrypt(eEmployeeId));

        if (employeeId != 0)
        {

            outputData.EmployeeId = employeeId;
            outputData.EmployeeMaster.EmployeeId = employeeId;
            outputData.EmployeeMaster = await _employeeAPIController.GetEmployee(outputData.EmployeeMaster);
            //outputData.EmployeeValidationList = await _employeeAPIController.GetEmployee(outputData.EmployeeMaster);

            if (outputData.EmployeeMaster.EmployeeCode == null || outputData.EmployeeMaster.EmployeeCode == "")
            {
                outputData.EmployeeMaster.EmployeeCode = outputData.EmployeeId.ToString();
            }
            outputData.EmployeeMaster.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.EmployeeMaster.ProfileImage, 0, outputData.EmployeeMaster.ProfileImage.Length);
        }

        outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        outputData.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, outputData.UnitId, outputData.ClientId);
        outputData.EmployeeMastersKeyValues.EmployeeValidations = await _mastersKeyValueController.EmployeeValidationKeyValue("EmployeeMaster", "", clientId);


        EmployeeKeyValues clientAdminKeyValue = new EmployeeKeyValues();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        clientAdminKeyValue.EmployeeId = empSession.EmployeeId;
        clientAdminKeyValue.EmployeeName = empSession.EmployeeName;
        if (outputData != null)
        {
            if (outputData.EmployeeMastersKeyValues != null)
            {
                if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues != null)
                {
                    if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Where(x => x.EmployeeId == empSession.EmployeeId).Count() == 0)
                    {
                        outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Add(clientAdminKeyValue);
                    }
                }
            }
        }

        outputData.UnitMaster = await _clientController.GetClientUnitById(outputData.UnitId ?? default(int));

        if (outputData.UnitMaster != null)
        {
            if (outputData.ConfirmationPeriod == null)
            {
                outputData.ConfirmationPeriod = outputData.UnitMaster.ConfirmationPeriod;
            }
            if (outputData.NoticePeriod == null)
            {
                outputData.NoticePeriod = outputData.UnitMaster.NoticePeriod;
            }
        }
        return View("EmployeeEdit", outputData);
    }

    public async Task<IActionResult> EEmployeeDetail(string eEmployeeId = "")
    {
        CheckSessionEJoining(eEmployeeId);

        EmployeeMasterVM outputData = new EmployeeMasterVM();


        //        EmployeeMasterDTO outputData = new EmployeeMasterDTO();
        //string eId = CommonHelper.Encrypt("1");

        eEmployeeId = HttpUtility.UrlDecode(eEmployeeId);
        string eId = CommonHelper.Encrypt(eEmployeeId);

        int employeeId = 0;
        if (eEmployeeId != "")
        {
            employeeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eEmployeeId));
        }
        outputData.Opt = "Add";

        if (employeeId != 0)
        {
            outputData.EmployeeId = employeeId;
            outputData.EmployeeMaster.EmployeeId = employeeId;
            outputData.EmployeeMaster = await _employeeAPIController.GetEmployeeInfo(outputData.EmployeeMaster);
            outputData = await _employeeAPIController.GetEmployeeTabFillingStatus(outputData);
            if (outputData.EmployeeMaster.EmployeeCode == null || outputData.EmployeeMaster.EmployeeCode == "")
            {
                outputData.EmployeeMaster.EmployeeCode = outputData.EmployeeId.ToString();

            }
            outputData.EnycEmployeeId = eEmployeeId;

            if (outputData.EmployeeMaster.ProfileImage != null)
                outputData.EmployeeMaster.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.EmployeeMaster.ProfileImage, 0, outputData.EmployeeMaster.ProfileImage.Length);
        }

        outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        outputData.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
        outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, outputData.UnitId, outputData.ClientId);
        outputData.EmployeeMastersKeyValues.EmployeeValidations = await _mastersKeyValueController.EmployeeValidationKeyValue("EmployeeMaster", "", outputData.ClientId, outputData.UnitId);

        EmployeeKeyValues clientAdminKeyValue = new EmployeeKeyValues();
        //EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        //clientAdminKeyValue.EmployeeId = empSession.EmployeeId;
        //clientAdminKeyValue.EmployeeName = empSession.EmployeeName;
        //if (outputData != null)
        //{
        //    if (outputData.EmployeeMastersKeyValues != null)
        //    {
        //        if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues != null)
        //        {
        //            if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Where(x => x.EmployeeId == empSession.EmployeeId).Count() == 0)
        //            {
        //                outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Add(clientAdminKeyValue);
        //            }
        //        }
        //    }
        //}[

        return View("EEmployee", outputData);
    }

    [Authorize(Roles = "Clientadmin")]
    public async Task<IActionResult> E_Joinee()
    {
        int clientId;
        if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
        {
            return RedirectToAction("Login", "Account");
        }
        EmployeeEJoineeDTO outputData = new EmployeeEJoineeDTO();
        //outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue();
        outputData.UnitId = Convert.ToInt32(HttpContext.Session.GetInt32("UnitId"));
        outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.E_JoineeEmployeeMastersKeyValue(true, outputData.UnitId);

        EmployeeKeyValues clientAdminKeyValue = new EmployeeKeyValues();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        clientAdminKeyValue.EmployeeId = empSession.EmployeeId;
        clientAdminKeyValue.EmployeeName = empSession.EmployeeName;
        if (outputData != null)
        {
            if (outputData.EmployeeMastersKeyValues != null)
            {
                if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues != null)
                {
                    if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Where(x => x.EmployeeId == empSession.EmployeeId).Count() == 0)
                    {
                        outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Add(clientAdminKeyValue);
                    }
                }
            }
        }

        return View("E_Joinee", outputData);
    }
    [HttpPost]
    public async Task<IActionResult> E_Joinee(EmployeeEJoineeDTO inputData)
    {
        ClientSettingDTO outputData = new ClientSettingDTO();
        int clientId;
        if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
        {
            return RedirectToAction("Login", "Account");
        }
        string? strUnit = HttpContext.Session.GetString("unit");
        if (String.IsNullOrEmpty(strUnit))
        {
            Infrastructure.Models.Page.Error error = new Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(strUnit);
        if (unit == null)
        {
            Infrastructure.Models.Page.Error error = new Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }

        outputData = await _clientController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));
        inputData.EmailProvider = outputData.EmailProvider;

        EmployeeMastersKeyValues emkv = new EmployeeMastersKeyValues();
        int unitId = Convert.ToInt32(HttpContext.Session.GetInt32("UnitId"));
        inputData.UnitId = unit.UnitID;
        emkv = await _mastersKeyValueController.E_JoineeEmployeeMastersKeyValue(true, unitId);

        EmployeeKeyValues clientAdminKeyValue = new EmployeeKeyValues();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        clientAdminKeyValue.EmployeeId = empSession.EmployeeId;
        clientAdminKeyValue.EmployeeName = empSession.EmployeeName;
        if (emkv != null)
        {
            if (emkv.EmployeeKeyValues != null)
            {
                if (emkv.EmployeeKeyValues.Where(x => x.EmployeeId == empSession.EmployeeId).Count() == 0)
                {
                    emkv.EmployeeKeyValues.Add(clientAdminKeyValue);
                }
            }
        }

        if (!ModelState.IsValid)
        {
            inputData.EmployeeMastersKeyValues = emkv;
            return View("E_Joinee", inputData);
        }
        BL.Employee blemp = new BL.Employee();
        inputData.IsActive = true;
        IActionResult actionResult;
        IActionResult EEEactionResult;
        IActionResult MNEactionResult;
        EmployeeEJoineeDTO viewModel = new EmployeeEJoineeDTO();

        EEEactionResult = await _employeeAPIController.CheckEEmployeeExistByEmailId(inputData);
        ObjectResult EEEobjResult = (ObjectResult)EEEactionResult;
        var objEEEResultData = (HttpResponseMessage)EEEobjResult.Value;
        if (objEEEResultData.IsSuccessStatusCode == false)
        {
            viewModel.DisplayMessage = "Duplicate Email Found";
            viewModel.EmployeeMastersKeyValues = emkv;

            inputData.HttpMessage = objEEEResultData;
            inputData.DisplayMessage = viewModel.DisplayMessage;
            inputData.EmployeeMastersKeyValues = emkv;

            return View("E_Joinee", inputData);
        }
        MNEactionResult = await _employeeAPIController.CheckEEmployeeExistByMobileNo(inputData);
        ObjectResult MNEobjResult = (ObjectResult)MNEactionResult;
        var objMNEResultData = (HttpResponseMessage)MNEobjResult.Value;
        if (objMNEResultData.IsSuccessStatusCode == false)
        {
            inputData.DisplayMessage = viewModel.DisplayMessage = "Duplicate Contact Number Found";
            inputData.EmployeeMastersKeyValues = viewModel.EmployeeMastersKeyValues = emkv;

            inputData.HttpMessage = objMNEResultData;

            return View("E_Joinee", inputData);
        }
        IActionResult EEEcodeactionResult = await _employeeAPIController.CheckEEmployeeExistByEmployeeCode(inputData);
        ObjectResult EEEcodeobjResult = (ObjectResult)EEEcodeactionResult;
        var objEEEcodeResultData = (HttpResponseMessage)EEEcodeobjResult.Value;
        if (objEEEcodeResultData.IsSuccessStatusCode == false)
        {
            inputData.DisplayMessage = viewModel.DisplayMessage = "Duplicate Employee Code Found";
            inputData.EmployeeMastersKeyValues = viewModel.EmployeeMastersKeyValues = emkv;

            inputData.HttpMessage = objEEEcodeResultData;
            return View("E_Joinee", inputData);
        }

        LoginDetailDTO loginDetailDTO = new LoginDetailDTO();
        loginDetailDTO.UserName = inputData.EmailId;
        loginDetailDTO.MobileNo = inputData.ContactNo;
        loginDetailDTO.Password = blemp.RandomString();

        loginDetailDTO.EncryptedPassword = CommonHelper.Encrypt(loginDetailDTO.Password);
        loginDetailDTO.LoginType = 3;
        loginDetailDTO.ClientId = clientId;
        IActionResult loginactionResult = await _loginAPIController.GetLoginDetail(loginDetailDTO);
        ObjectResult objLoginResult = (ObjectResult)loginactionResult;

        LoginDetailDTO objloginResultData = (LoginDetailDTO)objLoginResult.Value;
        HttpResponseMessage getlogindata = objloginResultData.HttpMessage;
        if (getlogindata.IsSuccessStatusCode == true)
        {
            inputData.DisplayMessage = viewModel.DisplayMessage = "Duplicate Email Id or Mobile Number Found";
            inputData.EmployeeMastersKeyValues = viewModel.EmployeeMastersKeyValues = emkv;

            inputData.HttpMessage = getlogindata;
            return View("E_Joinee", inputData);
        }

        inputData.JoinType = 0; //0 is for E-Joining    
        inputData.ClientId = loginDetailDTO.ClientId;
        inputData.UnitId = Convert.ToInt32(HttpContext.Session.GetInt32("UnitId"));
        actionResult = await _employeeAPIController.SaveEmployeeEJoinee(inputData);
        ObjectResult objResult = (ObjectResult)actionResult;
        var objResultData = (HttpResponseMessage)objResult.Value;
        if (objResultData != null)
        {
            inputData.HttpMessage = objResultData;
            if (objResultData.IsSuccessStatusCode == true)
            {
                IActionResult actionResult_L;
                int generatedEmployeeId;
                if (int.TryParse(objResultData.Content.ReadAsStringAsync().Result, out generatedEmployeeId) == false)
                {
                    EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
                    employeeMasterDTO.EmployeeId = generatedEmployeeId;
                    employeeMasterDTO.EmployeeCode = generatedEmployeeId.ToString();
                    //_employeeAPIController.UpdateEmployee(employeeMasterDTO, "EmployeeCode");

                    inputData.DisplayMessage = viewModel.DisplayMessage = "Error occurred while creating E-Joining Link";
                    inputData.EmployeeMastersKeyValues = viewModel.EmployeeMastersKeyValues = emkv;

                    inputData.HttpMessage = objResultData;
                    return View("E_Joinee", inputData);
                }
                inputData.EmployeeId = generatedEmployeeId;

                loginDetailDTO.EmployeeId = generatedEmployeeId;
                loginDetailDTO.JoiningMailSent = true;
                string password = loginDetailDTO.Password;
                loginDetailDTO.Password = loginDetailDTO.EncryptedPassword;
                actionResult_L = await _loginAPIController.SaveLoginDetail(loginDetailDTO);
                loginDetailDTO.Password = password;
                ObjectResult objResult_L = (ObjectResult)actionResult_L;
                var objResultData_L = (HttpResponseMessage)objResult_L.Value;
                if (objResultData_L != null)
                {
                    if (objResultData_L.IsSuccessStatusCode == true)
                    {
                        inputData.JobTitle = emkv.JobTitleKeyValues.Where(x => x.JobTitleId == inputData.JobTitleId).Select(x => x.JobTitle).FirstOrDefault();
                        inputData.ClientKeyValue = _mastersKeyValueController.ClientKeyValue(r => r.ClientId == Convert.ToInt32(HttpContext.Session.GetString("ClientId"))).Result.FirstOrDefault();
                        if (blemp.SendJoiningLink(inputData, loginDetailDTO, unit))
                        {
                            inputData.HttpMessage = objResultData_L;
                            inputData.DisplayMessage = viewModel.DisplayMessage = "Email Sent Successfully";
                            return RedirectToAction("Employees");
                        }
                        else
                        {
                            inputData.DisplayMessage = viewModel.DisplayMessage = "Unable to send Email";
                        }
                    }
                    else
                    {
                        string result_L = objResultData_L.Content.ReadAsStringAsync().Result;
                        inputData.DisplayMessage = viewModel.DisplayMessage = result_L;
                    }
                }
                else
                {
                    inputData.DisplayMessage = viewModel.DisplayMessage = "Some error has occurred. Please try again";
                }
            }
            else
            {
                string result = objResultData.Content.ReadAsStringAsync().Result;
                inputData.DisplayMessage = viewModel.DisplayMessage = result;
            }
        }
        else
        {
            inputData.DisplayMessage = viewModel.DisplayMessage = "Some error has occurred.";
        }



        inputData.EmployeeMastersKeyValues = viewModel.EmployeeMastersKeyValues = emkv;

        return View("E_Joinee", inputData);

    }

    public ActionResult CheckSession()
    {
        if (Convert.ToInt32(HttpContext.Session.GetString("ClientId")) == 0)
        {
            //return RedirectToAction("~/Account/Login");
            return RedirectToAction("Login", "Account");
        }
        else
            return Ok();
    }
    public ActionResult CheckSessionEJoining(string eEmployeeId)
    {
        if (Convert.ToInt32(HttpContext.Session.GetString("ClientId")) == 0)
        {
            return Redirect("/simplihr.newjoinee/" + eEmployeeId);
        }
        else
            return Ok();
    }

    public bool IsSessionAlive(HttpContext curHTTPcontext)
    {
        if (Convert.ToInt32(curHTTPcontext.Session.GetString("ClientId")) == 0)
        {
            //return RedirectToAction("~/Account/Login");

            return false;
        }
        else
            return true;

    }

    [HttpPost]
    //[Authorize(Roles = "Clientadmin,User")]
    public async Task<IActionResult> SaveEmployeePersonalInfo(EmployeeMasterDTO inputData)
    {
        inputData.IsActive = true;
        IActionResult actionResult1;
        dynamic objResultData = null;
        EmployeeMasterDTO employeeData = new EmployeeMasterDTO();
        ObjectResult objResult;
        //inputData.EmployeeId = 10;
        bool sendMail = false;
        if (inputData != null)
        {
            inputData.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
            inputData.UnitId = Convert.ToInt32(HttpContext.Session.GetInt32("UnitId"));
            UnitMasterDTO? unitMasterDTO = new UnitMasterDTO();
            var resUnit = await _clientController.GetClientUnitNameById(inputData.UnitId ?? default(int));
            if (resUnit != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resUnit).StatusCode == 200)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)resUnit).Value != null)
                    {
                        unitMasterDTO = (UnitMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resUnit).Value;
                    }
                }
            }

            //if (inputData.ClientId == -1) inputData.ClientId = 32;
            LoginDetailDTO loginDetail = new LoginDetailDTO();
            EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
            empTempDoc.ScreenTab = "Personal Information";
            empTempDoc.SessionId = HttpContext.Session.Id;
            empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            if (inputData.EmployeeId.ToString().Equals("0"))
            {
                inputData.JoinType = 1; //1 is for employee who joined via Add Employee not via E-Joining
                inputData.ConfirmationPeriod = unitMasterDTO.ConfirmationPeriod;
                inputData.NoticePeriod = unitMasterDTO.NoticePeriod;
                inputData.CreatedOn = System.DateTime.Now;
                inputData.CreatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                var actionResult = _employeeAPIController.SaveEmployee(inputData);

                if (((Microsoft.AspNetCore.Mvc.ObjectResult)actionResult).StatusCode == 200)
                {
                    var employeeMasterDTO = (EmployeeMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)actionResult).Value;
                    if (employeeMasterDTO != null)
                    {
                        employeeMasterDTO.EmployeeId = employeeMasterDTO.EmployeeId;
                        employeeMasterDTO.EnycEmployeeId = CommonHelper.EncryptURLHTML(employeeMasterDTO.EmployeeId.ToString());
                        employeeMasterDTO.EmployeeCode = employeeMasterDTO.EmployeeCode == null ? employeeMasterDTO.EmployeeId.ToString() : employeeMasterDTO.EmployeeCode;
                        empTempDoc.EmployeeId = employeeMasterDTO.EmployeeId;

                        loginDetail = _loginAPIController.CreateLoginForEmployee(employeeMasterDTO);
                        empTempDoc.ReferenceId = 0;
                        string attachmentMsg = await _employeeAPIController.SaveAttachment(empTempDoc);
                        if (attachmentMsg != "Success")
                        {
                            return BadRequest("Unable to save attachment");
                        }
                        employeeMasterDTO.DisplayMessage = "Success";
                        return Ok(employeeMasterDTO);
                    }
                    else
                    {
                        return BadRequest("Unable to Save Employee. Please try again");
                    }
                }
                return actionResult;
            }
            else
            {
                string sProperties = "FirstName,LastName,MiddleName,GenderId,Dob,ContactNo,EmailId,FatherName,SpouseName,MaritalStatusId,ReligionId,BloodGroupId,Pannumber,AadharNumber,EmergencyContactPerson,EmergencyContactNo,EmergencyContactRelation,InfoFillingStatus";
                var actionResult = _employeeAPIController.UpdateEmployee(inputData, sProperties);
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)actionResult).StatusCode == 200)
                {
                    inputData.EnycEmployeeId = CommonHelper.EncryptURLHTML(inputData.EmployeeId.ToString());
                    empTempDoc.EmployeeId = inputData.EmployeeId;
                    empTempDoc.ReferenceId = 0;
                    string attachmentMsg = await _employeeAPIController.SaveAttachment(empTempDoc);
                    loginDetail = _loginAPIController.GetLoginByEmployeeId(inputData.EmployeeId);

                    if (loginDetail == null)
                    {
                        EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
                        employeeMasterDTO = await _employeeAPIController.GetEmployeeById(inputData.EmployeeId);
                        loginDetail = _loginAPIController.CreateLoginForEmployee(employeeMasterDTO);
                    }

                    empTempDoc.ReferenceId = 0;

                    if (attachmentMsg != "Success")
                    {
                        return BadRequest("Unable to save attachment");
                    }
                    inputData.DisplayMessage = "Success";
                    return Ok(inputData);
                }
                return actionResult;
            }
        }
        return null;
    }

    [HttpGet]
    [Route("Employee/ViewFile/{fieldName}&{eEmployeeId}")]
    //[Route("/EmployeeAttendance/ManualPunchesProcessing/{Ids}&{ua}")]
    public async Task<ActionResult> ViewFile(string fieldName, string eEmployeeId = "")
    {
        EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
        empTempDoc.FieldName = fieldName;
        string sRefId = string.Empty;
        int refId = 0;
        string refStr = "-=refid__";
        empTempDoc.SessionId = HttpContext.Session.Id;
        if (!string.IsNullOrEmpty(eEmployeeId))
        {
            if (eEmployeeId.Contains(refStr))
            {
                var refValue = eEmployeeId.Substring(eEmployeeId.IndexOf(refStr), (eEmployeeId.Length - eEmployeeId.IndexOf(refStr)));
                sRefId = refValue.Replace(refStr, "");
                empTempDoc.EmployeeId = Convert.ToInt32(CommonHelper.Decrypt(eEmployeeId.Replace(refValue, "")));
                int.TryParse(sRefId, out refId);
                empTempDoc.ReferenceId = refId;

            }
            else
                empTempDoc.EmployeeId = Convert.ToInt32(CommonHelper.Decrypt(eEmployeeId));
        }




        EmployeeUploadDocumentDTO employeeUploadDocument = new EmployeeUploadDocumentDTO();
        employeeUploadDocument = await _employeeAPIController.GetUploadedInfo(empTempDoc);
        if (employeeUploadDocument != null)
        {
            string Base64String = string.Empty;
            string imgExtStr = ",png,jpeg,jpg,bmp,gif,tiff,";
            if (imgExtStr.ToLower().Contains("," + employeeUploadDocument.DocumentType.ToLower() + ","))
                Base64String = "data:image/png;base64,";
            else if (employeeUploadDocument.DocumentType.ToLower() == "pdf")
                Base64String = "data:application/pdf;base64,";
            else if (employeeUploadDocument.DocumentType.ToLower() == "csv")
                Base64String = "data:application/octet-stream;charset=utf-8;base64,";
            else if (employeeUploadDocument.DocumentType.ToLower() == "txt")
                Base64String = "data:application/octet-stream,";
            employeeUploadDocument.Base64Document = Base64String + Convert.ToBase64String(employeeUploadDocument.EmployeeDocument, 0, employeeUploadDocument.EmployeeDocument.Length);
        }
        else
        {
            employeeUploadDocument = new EmployeeUploadDocumentDTO();
            employeeUploadDocument.DisplayMessage = "Required attachment not found";
        }

        return View(employeeUploadDocument);
    }


    [HttpPost]
    //[Authorize(Roles = "Clientadmin,User")]
    public async Task<Object> FinalDataSubmit(EmployeeMasterDTO inputData)
    {
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();

        //if (!_ValidatorService.HasRequestValidCaptchaEntry())
        //{
        //    this.ModelState.AddModelError(_CaptchaOptions.CaptchaComponent.CaptchaInputName, "Please add the security code as number");
        //    // return Ok("InvalidCaptcha");
        //}
        //else
        //{

        ClientSettingDTO outputData = new ClientSettingDTO();
        inputData.IsActive = true;
        dynamic actionResult = null;
        //dynamic objResultData = null;

        LoginDetailDTO loginDetail = new LoginDetailDTO();
        ObjectResult objResult;
        //inputData.EmployeeId = 10;
        bool sendMail = false;
        inputData.JobTitleKeyValue = _mastersKeyValueController.JobTitleKeyValue(r => r.JobTitleId == inputData.JobTitleId).Result.FirstOrDefault();
        inputData.ClientKeyValue = _mastersKeyValueController.ClientKeyValue(r => r.ClientId == Convert.ToInt32(HttpContext.Session.GetString("ClientId"))).Result.FirstOrDefault();
        if (inputData != null)
        {
            inputData.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
            inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
            if (!inputData.EmployeeId.ToString().Equals("0"))
            {
                string? sSeeion = HttpContext.Session.Id;
                _employeeAPIController.DeleteSessionAttachment(sSeeion);
                if (inputData.InfoFillingStatus == 0)
                {

                    inputData.InfoFillingStatus = 1;
                    string sProperties = "InfoFillingStatus";
                    actionResult = _employeeAPIController.UpdateEmployee(inputData, sProperties);
                    objResultData.DisplayMessage = actionResult.Value;
                }
                else
                {
                    objResultData.DisplayMessage = _employeeAPIController.ValidateEmployeeInfo(inputData);

                    if (string.IsNullOrEmpty(objResultData.DisplayMessage))
                        objResultData.DisplayMessage = "Success";
                    //objResultData.DisplayMessage = actionResult.Value;
                }
                if (inputData.InfoFillingStatus != null && inputData.InfoFillingStatus == 1 && (inputData.EmployeeStatus != null && inputData.EmployeeStatus.ToUpper() == "ACTIVE"))
                    _SchedularController.AddEmployeeLeaveBalance(Convert.ToString(inputData.UnitId), Convert.ToString(inputData.EmployeeId));

                if (actionResult != null)
                {
                    UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
                    inputData.UnitMaster = unit;
                    outputData = await _clientController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));
                    inputData.EmailProvider = outputData.EmailProvider;
                    string actionValue = Convert.ToString(actionResult.Value);
                    if (actionValue.ToUpper() == "SUCCESS")
                    {
                        loginDetail = await _loginAPIController.UpldateLoginForFinalSubmit(inputData);
                        if (loginDetail.DisplayMessage != "_blank")
                            objResultData.DisplayMessage = objResultData.DisplayMessage != "_blank" ? loginDetail.DisplayMessage : objResultData.DisplayMessage;
                    }

                }

            }
        }

        // }
        return Ok(objResultData);
    }

    //[Authorize(Roles = "Clientadmin")]
    public async Task<Object> ResendInductionEmail([FromBody] EmployeeMasterDTO inputData)
    {
        try
        {
            ClientSettingDTO outputData = new ClientSettingDTO();
            UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));

            outputData = await _clientController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));

            inputData = await _employeeAPIController.GetEmployeeById(inputData.EmployeeId);



            if (inputData != null)
            {

                inputData.JobTitleKeyValue = _mastersKeyValueController.JobTitleKeyValue(r => r.JobTitleId == inputData.JobTitleId).Result.FirstOrDefault();
                inputData.ClientKeyValue = _mastersKeyValueController.ClientKeyValue(r => r.ClientId == Convert.ToInt32(HttpContext.Session.GetString("ClientId"))).Result.FirstOrDefault();


            }


            LoginDetailDTO loginDetail = _loginAPIController.GetLoginByEmployeeId(inputData.EmployeeId);

            if (loginDetail != null)
            {
                loginDetail.Password = CommonHelper.Decrypt(loginDetail.Password);
                inputData.EmailProvider = outputData.EmailProvider;

                bool mailSent = MailHelper.SendLoginDetailMail(inputData, loginDetail, unit);
                if (mailSent)
                {
                    var actionResult = await _employeeAPIController.UpdateEmployeeMailStamp(inputData.EmployeeId);
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<Object> ResendEJoiningEmail([FromBody] EmployeeEJoineeDTO inputData)
    {
        try
        {
            ClientSettingDTO clientData = new ClientSettingDTO();
            EmployeeMasterDTO outputData = new EmployeeMasterDTO();
            UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));

            clientData = await _clientController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));

            inputData = await _employeeAPIController.GetEEmployeeById(inputData.EmployeeId);
            inputData.EmailProvider = clientData.EmailProvider;

            if (inputData != null)
            {
                //inputData.JobTitleKeyValue = _mastersKeyValueController.JobTitleKeyValue(r => r.JobTitleId == inputData.JobTitleId).Result.FirstOrDefault();
                inputData.ClientKeyValue = _mastersKeyValueController.ClientKeyValue(r => r.ClientId == Convert.ToInt32(HttpContext.Session.GetString("ClientId"))).Result.FirstOrDefault();
            }
            LoginDetailDTO loginDetail = _loginAPIController.GetLoginByEmployeeId(inputData.EmployeeId);
            loginDetail.Password = CommonHelper.Decrypt(loginDetail.Password);

            BL.Employee blemp = new BL.Employee();
            bool mailSent = blemp.SendJoiningLink(inputData, loginDetail, unit);

            if (mailSent)
            {
                outputData.EmployeeId = inputData.EmployeeId;            
                outputData.LastTimeStamp = DateTime.Now;
                string sProperties = "LastTimeStamp";
                var actionResult = await _employeeAPIController.UpdateEmployeeMailStamp(inputData.EmployeeId);
                
            }


            //bool mailSent = MailHelper.SendLoginDetailMail(inputData, loginDetail, unit);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }


    //public LoginDetailDTO CreateLoginForEmployee(EmployeeMasterDTO employeeDTO)
    //{
    //    LoginDetailDTO loginDetail = new LoginDetailDTO();
    //    try 
    //    {

    //        loginDetail.LoginType = 2;0

    //        loginDetail.UserName = employeeDTO.EmailId;
    //        loginDetail.EmployeeId = employeeDTO.EmployeeId;
    //        loginDetail.Password = CommonHelper.RandomString();
    //        loginDetail.ClientId = 32; //Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
    //        loginDetail.IsActive = true;           
    //        _loginAPIController.SaveLoginDetail(loginDetail);
    //        return loginDetail;
    //    }
    //    catch (Exception ex)
    //    {
    //        return loginDetail;
    //    }

    //}
    //[Authorize(Roles = "Clientadmin")]
    public async Task<Object> SaveEmployeeJobInfo(EmployeeMasterDTO inputData)
    {
        string? strempSession = HttpContext.Session.GetString("employee");
        if (String.IsNullOrEmpty(strempSession))
        {
            Infrastructure.Models.Page.Error error = new Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strempSession);
        if (empSession == null)
        {
            Infrastructure.Models.Page.Error error = new Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }


        inputData.IsActive = true;
        dynamic objResultData = null;
        IActionResult actionResult;
        EmployeeMasterDTO employeeData = new EmployeeMasterDTO();
        //inputData.EmployeeId = 10;
        if (inputData != null)
        {
            EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
            empTempDoc.ScreenTab = "Job Information";
            empTempDoc.SessionId = HttpContext.Session.Id;
            empTempDoc.LoggedInUser = empSession.EmployeeId;
            empTempDoc.EmployeeId = inputData.EmployeeId;
            empTempDoc.ReferenceId = 0;
            inputData.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
            if (inputData.EmployeeId.ToString().Equals("0"))
            {
                actionResult = _employeeAPIController.SaveEmployee(inputData);
                string attachmentMsg = await _employeeAPIController.SaveAttachment(empTempDoc);
            }
            else
            {
                if (inputData.InfoFillingStatus == 1)
                {
                    LoginDetailDTO loginDetail = _loginAPIController.GetLoginByEmployeeId(inputData.EmployeeId);  //.GetLoginByEmployeeId(inputData.EmployeeId).Result.FirstOrDefault();
                    loginDetail.LoginType = 2;
                    _loginAPIController.UpdateLogin(loginDetail, "LoginType");
                }
                string sPropertyToUpdate = "EmployeeCode,JobTitleId,DepartmentId,ManagerId,HODId,Doj,WorkLocationId,EmploymentType,IdentityId,EmployeeStatus,OfficialEmail,RoleId,BandId,CTC,EPFNumber,ESINumber,UANNumber,ConfirmationPeriod,DOC,NoticePeriod";

                if (!(inputData.EmployeeCode != null && _employeeAPIController.ValidateEmployeeCode(inputData.EmployeeCode, empSession.UnitId ?? default(int), inputData.EmployeeId) == true))
                {
                    return BadRequest("Duplicate Employee Code Found");
                }
                actionResult = _employeeAPIController.UpdateEmployee(inputData, sPropertyToUpdate);

                string attachmentMsg = await _employeeAPIController.SaveAttachment(empTempDoc);
            }

            ObjectResult objResult = (ObjectResult)actionResult;

            objResultData = objResult.Value;
        }
        return objResultData;
    }

    //[Authorize(Roles = "Clientadmin,User")]
    public async Task<IActionResult> SaveEmployeePassportInfo(EmployeeMasterDTO inputData)
    {
        try
        {
            if (!(inputData.EmployeeId > 0))
            {
                throw new Exception("Please fill in Personal Details first if you are adding a new Employee.");
            }
            string sPropertyToUpdate = "PassportNumber,PassportIssueDate,PassportValidTillDate,PassportIssueCountryId,PassportIssueStateId,PassportIssueCityId";
            var res = _employeeAPIController.UpdateEmployee(inputData, sPropertyToUpdate);
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
            empTempDoc.ScreenTab = "Passport Details";
            empTempDoc.SessionId = HttpContext.Session.Id;
            empTempDoc.LoggedInUser = empSession.EmployeeId;
            empTempDoc.EmployeeId = inputData.EmployeeId;
            empTempDoc.ReferenceId = 0;
            string attachmentMsg = await _employeeAPIController.SaveAttachment(empTempDoc);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }


    public async Task<IActionResult> EmployeeEditList(string tenc, string tsc)
    {
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session Time Out";
            error.Message = "You have been logged out of your session. Please login again";
            error.ButtonMessage = "Go to login page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        if (String.IsNullOrEmpty(tenc) || String.IsNullOrEmpty(tsc))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Invalid";
            error.Message = "The ticket that you are trying to visit is invalid";
            error.ButtonMessage = "Go back to ticket list";
            error.ButtonURL = "/Employee/EmployeeEditTicketList";
            return View("../Page/Error", error);
        }
        EmployeeEditApprovalViewModel dto = new EmployeeEditApprovalViewModel();
        string TicketId = CommonHelper.DecryptURLHTML(tenc);
        string SourceScreen = CommonHelper.DecryptURLHTML(tsc);
        var res = await _profileEditAuthAPIController.GetEmployeeEditTicketByTicketId(TicketId);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.EmployeeEditTicketDetails = (EmployeeEditTicketViewModel?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (dto.EmployeeEditTicketDetails != null)
                {
                    dto.EmployeeEditTicketDetails.SourceScreen = CommonHelper.DecryptURLHTML(tsc);
                    dto.EmployeeEditTicketDetails.encSourceScreen = tsc;
                }
            }
        }

        var keyvalues = await _mastersKeyValueController.EmployeeMastersKeyValueExcludingEmployeeKey(true, empSession.UnitId ?? default(int));

        int clientId;
        if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
            keyvalues.EmployeeValidations = await _mastersKeyValueController.EmployeeValidationKeyValue("EmployeeMaster", "", clientId);

        var resEmployeeEditInfoByTicketId = await _profileEditAuthAPIController.GetEmployeeEditInfoByTicketId(TicketId, empSession.ClientId ?? default(int), empSession.UnitId ?? default(int));

        if (resEmployeeEditInfoByTicketId != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeEditInfoByTicketId).StatusCode == 200)
            {
                dto.employeeEditInfos = (List<EmployeeEditInfoViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeEditInfoByTicketId).Value;
                if (dto.employeeEditInfos != null)
                {
                    foreach (var item in dto.employeeEditInfos)
                    {
                        if (keyvalues != null)
                        {
                            if (item.FieldName.ToUpper() == "GenderId".ToUpper())
                            {
                                item.FieldType = "Id";
                                item.NewValueText = item.ChangeValue == "1" ? "Male" : item.ChangeValue == "2" ? "Female" : "";
                                item.OldValueText = item.OldValue == "1" ? "Male" : item.OldValue == "2" ? "Female" : "";
                            }
                            else if (item.FieldName.ToUpper() == "MaritalStatusId".ToUpper())
                            {
                                if (keyvalues.MaritalStatusKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.MaritalStatusKeyValues.Where(x => x.MaritalStatusId.ToString() == item.ChangeValue).Select(x => x.MaritalStatusName).FirstOrDefault();
                                    item.OldValueText = keyvalues.MaritalStatusKeyValues.Where(x => x.MaritalStatusId.ToString() == item.OldValue).Select(x => x.MaritalStatusName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "ReligionId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "PersonalInformation".ToUpper())
                            {
                                if (keyvalues.ReligionKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.ReligionKeyValues.Where(x => x.ReligionId.ToString() == item.ChangeValue).Select(x => x.ReligionName).FirstOrDefault();
                                    item.OldValueText = keyvalues.ReligionKeyValues.Where(x => x.ReligionId.ToString() == item.OldValue).Select(x => x.ReligionName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "BloodGroupId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "PersonalInformation".ToUpper())
                            {
                                if (keyvalues.BloodGroupKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.BloodGroupKeyValues.Where(x => x.BloodGroupId.ToString() == item.ChangeValue).Select(x => x.BloodGroupName).FirstOrDefault();
                                    item.OldValueText = keyvalues.BloodGroupKeyValues.Where(x => x.BloodGroupId.ToString() == item.OldValue).Select(x => x.BloodGroupName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "CountryId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "CurrentAddress".ToUpper())
                            {
                                if (keyvalues.CountryKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "CountryId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "CurrentAddress".ToUpper())
                            {
                                if (keyvalues.CountryKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "StateId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "CurrentAddress".ToUpper())
                            {
                                //if (keyvalues.CountryKeyValues != null)
                                //{
                                //    item.FieldType = "Id";
                                //    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                //    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                //}
                            }
                            else if (item.FieldName.ToUpper() == "CityId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "CurrentAddress".ToUpper())
                            {
                                //if (keyvalues.CountryKeyValues != null)
                                //{
                                //    item.FieldType = "Id";
                                //    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                //    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                //}
                            }

                            else if (item.FieldName.ToUpper() == "PermanentCountryId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "PermanentAddress".ToUpper())
                            {
                                if (keyvalues.CountryKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "PermanentStateId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "PermanentAddress".ToUpper())
                            {
                                //if (keyvalues.CountryKeyValues != null)
                                //{
                                //    item.FieldType = "Id";
                                //    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                //    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                //}
                            }
                            else if (item.FieldName.ToUpper() == "PermanentCityId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "PermanentAddress".ToUpper())
                            {
                                //if (keyvalues.CountryKeyValues != null)
                                //{
                                //    item.FieldType = "Id";
                                //    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                //    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                //}
                            }
                            else if (item.FieldName.ToUpper() == "BankId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "BankDetails".ToUpper())
                            {
                                if (keyvalues.BankKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.BankKeyValues.Where(x => x.BankId.ToString() == item.ChangeValue).Select(x => x.BankName).FirstOrDefault();
                                    item.OldValueText = keyvalues.BankKeyValues.Where(x => x.BankId.ToString() == item.OldValue).Select(x => x.BankName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "AcademicId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "AcademicDetails".ToUpper())
                            {
                                if (keyvalues.AcademicKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.AcademicKeyValues.Where(x => x.AcademicId.ToString() == item.ChangeValue).Select(x => x.AcademicName).FirstOrDefault();
                                    item.OldValueText = keyvalues.AcademicKeyValues.Where(x => x.AcademicId.ToString() == item.OldValue).Select(x => x.AcademicName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "ExperienceJobTitleId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "ExperiencesBackgroud".ToUpper())
                            {
                                if (keyvalues.JobTitleKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.JobTitleKeyValues.Where(x => x.JobTitleId.ToString() == item.ChangeValue).Select(x => x.JobTitle).FirstOrDefault();
                                    item.OldValueText = keyvalues.JobTitleKeyValues.Where(x => x.JobTitleId.ToString() == item.OldValue).Select(x => x.JobTitle).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "PassportIssueCountryId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "PassportDetails".ToUpper())
                            {
                                if (keyvalues.CountryKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "PassportIssueCityId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "PassportDetails".ToUpper())
                            {
                                if (keyvalues.CountryKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "PassportIssueCityId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "PassportDetails".ToUpper())
                            {
                                if (keyvalues.CountryKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.ChangeValue).Select(x => x.CountryName).FirstOrDefault();
                                    item.OldValueText = keyvalues.CountryKeyValues.Where(x => x.CountryId.ToString() == item.OldValue).Select(x => x.CountryName).FirstOrDefault();
                                }
                            }
                            else if (item.FieldName.ToUpper() == "LanguageId".ToUpper() && item.ScreenTab.ToUpper().Replace(" ", "") == "Language".ToUpper())
                            {
                                if (keyvalues.CountryKeyValues != null)
                                {
                                    item.FieldType = "Id";
                                    item.NewValueText = keyvalues.LanguageKeyValue.Where(x => x.LanguageId.ToString() == item.ChangeValue).Select(x => x.Language).FirstOrDefault();
                                    item.OldValueText = keyvalues.LanguageKeyValue.Where(x => x.LanguageId.ToString() == item.OldValue).Select(x => x.Language).FirstOrDefault();
                                }
                            }
                        }


                        if (item.Attachment != null && item.DocumentType != null)
                        {

                            string Base64String = CommonHelper.GetBase64String(item.DocumentType);
                            item.AttachmentBase64String = Base64String + Convert.ToBase64String(item.Attachment, 0, item.Attachment.Length);
                        }
                    }
                }
            }
        }

        var resEmployeeAddDeleteInfo = await _profileEditAuthAPIController.GetEmployeeAddDeleteInfoByTicketId(TicketId, empSession.UnitId ?? default(int));
        if (resEmployeeEditInfoByTicketId != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeEditInfoByTicketId).StatusCode == 200)
        {
            dto.employeeAddDeleteInfoViewModel = (EmployeeAddDeleteInfoViewModel?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeAddDeleteInfo).Value;
        }
        return View(dto);
    }
    public async Task<IActionResult> EmployeeEditTicketList()
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            //return BadRequest("Session had expired. Please logout and login again");

            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeEditTicketListViewModel dto = new EmployeeEditTicketListViewModel();
        //List<EmployeeEditTicketViewModel> dto = new List<EmployeeEditTicketViewModel>();
        var res = await _profileEditAuthAPIController.GetEmployeeEditTicketList(empSession.ClientId ?? default(int), empSession.UnitId ?? default(int));
        var resApproved = await _profileEditAuthAPIController.GetEmployeeEditTicketListApproved(empSession.ClientId ?? default(int));

        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.EmployeeEditTicketListActionNeeded = (List<EmployeeEditTicketViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (dto.EmployeeEditTicketListActionNeeded != null)
                {
                    foreach (var item in dto.EmployeeEditTicketListActionNeeded)
                    {
                        item.encTicketId = CommonHelper.EncryptURLHTML(item.TicketId.ToString());
                        item.SourceScreen = "ClientAdminNeedAction";
                        item.encSourceScreen = CommonHelper.EncryptURLHTML(item.SourceScreen);
                    }
                }
            }
        }
        if (resApproved != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resApproved).StatusCode == 200)
            {
                dto.EmployeeEditTicketListApproved = (List<EmployeeEditTicketViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resApproved).Value;
                if (dto.EmployeeEditTicketListApproved != null)
                {
                    foreach (var item in dto.EmployeeEditTicketListApproved)
                    {
                        item.encTicketId = CommonHelper.EncryptURLHTML(item.TicketId.ToString());
                        item.SourceScreen = "ClientAdminCompleted";
                        item.encSourceScreen = CommonHelper.EncryptURLHTML(item.SourceScreen);
                    }
                }
            }
        }

        return View(dto);
    }

    //[Authorize(Roles = "Clientadmin")]
    [HttpPost]
    public async Task<IActionResult> ApproveEmployeeDetailsAddDelete(AddDeleteTableActionDTO inputDTO)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            //return BadRequest("Session had expired. Please logout and login again");

            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        var res = await _profileEditAuthAPIController.ApproveEmployeeDetailsAddDelete(inputDTO);
        return res;
    }

    //[Authorize(Roles = "Clientadmin")]
    public async Task<IActionResult> ApproveEmployeeEditDetails(EditEmployeeDataDTO inputDTO)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            //return BadRequest("Session had expired. Please logout and login again");

            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        byte[] newProfilePic;
        //string empCode, string sRootPath,int EmployeeValidationId,int IsApproved,int? approvedBy = 0)
        var res = await _profileEditAuthAPIController.ApproveEmployeeEditDetails(inputDTO.TicketId ?? default(string), inputDTO.EmployeeValidationId ?? default(int), 2, empSession.EmployeeId, hostingEnv.WebRootPath);
        return res;
    }
    [Authorize(Roles = "Clientadmin")]
    public async Task<IActionResult> RejectEmployeeEditDetails(EditEmployeeDataDTO inputDTO)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        //EmployeeMasterDTO employeeInfo = new EmployeeMasterDTO();
        //employeeInfo.EmployeeId = inputDTO.EmployeeId.Value;
        //employeeInfo = await _employeeAPIController.GetEmployee(employeeInfo);
        var res = await _profileEditAuthAPIController.ApproveEmployeeEditDetails(inputDTO.TicketId ?? default(string), inputDTO.EmployeeValidationId ?? default(int), 2, empSession.EmployeeId, hostingEnv.WebRootPath);
        //var res = await _profileEditAuthAPIController.ApproveEmployeeEditDetails(inputDTO.TicketId ?? default(string), inputDTO.EmployeeValidationId ?? default(int), 3);
        return res;
    }
    public async Task<IActionResult> CancelEmployeeEditDetails(EditEmployeeDataDTO inputDTO)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            //return BadRequest("Session had expired. Please logout and login again");

            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        //var res = await _profileEditAuthAPIController.ApproveEmployeeEditDetails(inputDTO.TicketId ?? default(string), inputDTO.EmployeeValidationId ?? default(int), 1);
        //return res;
        //EmployeeMasterDTO employeeInfo = new EmployeeMasterDTO();
        //employeeInfo.EmployeeId = inputDTO.EmployeeId.Value;
        //employeeInfo = await _employeeAPIController.GetEmployee(employeeInfo);
        var res = await _profileEditAuthAPIController.ApproveEmployeeEditDetails(inputDTO.TicketId ?? default(string), inputDTO.EmployeeValidationId ?? default(int), 2, empSession.EmployeeId, hostingEnv.WebRootPath);
        return res;
    }




    public async Task<IActionResult> SaveEmployeeEditInfo()
    {

        return Ok("Save completed");
    }

    //Before Optimization Employee function
    //public async Task<IActionResult> Employee()
    //{
    //    CheckSession();

    //    EmployeeMasterVM outputData = new EmployeeMasterVM();

    //    outputData.EmployeeMasterList = await GetEmployeeList();
    //    //outputData.EmployeeMasterList = await GetEmployeeInfo();
    //    foreach (var item in outputData.EmployeeMasterList)
    //    {
    //        item.EnycEmployeeId = CommonHelper.Encrypt(Convert.ToString(item.EmployeeId));
    //    }

    //    outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, HttpContext.Session.GetInt32("UnitId"));

    //    //int clientId;
    //    //if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
    //    //    outputData.EmployeeMastersKeyValues.EmployeeValidations = await _mastersKeyValueController.EmployeeValidationKeyValue("EmployeeMaster","",clientId);
    //    EmployeeKeyValues clientAdminKeyValue = new EmployeeKeyValues();
    //    EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
    //    clientAdminKeyValue.EmployeeId = empSession.EmployeeId;
    //    clientAdminKeyValue.EmployeeName = empSession.EmployeeName;
    //    if (outputData != null)
    //    {
    //        if (outputData.EmployeeMastersKeyValues != null)
    //        {
    //            if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues != null)
    //            {
    //                if (outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Where(x => x.EmployeeId == empSession.EmployeeId).Count() == 0)
    //                {
    //                    outputData.EmployeeMastersKeyValues.EmployeeKeyValues.Add(clientAdminKeyValue);
    //                }
    //            }
    //        }
    //    }

    //    return View("Dashboard", outputData);

    //}

    public async Task<IActionResult> Employees()
    {
        CheckSession();
        EmployeeMasterVM outputData = new EmployeeMasterVM();
        outputData.EmployeeMasterList = await GetEmployeeListing();
        return View("Dashboard", outputData);
    }

    public async Task<IActionResult> ReportingEmployees()
    {
        CheckSession();

        EmployeeMasterDTO outputData = new EmployeeMasterDTO();

        outputData.EmployeeMasterList = await GetEmployeeList();
        foreach (var item in outputData.EmployeeMasterList)
        {
            item.EnycEmployeeId = CommonHelper.Encrypt(Convert.ToString(item.EmployeeId));
        }

        outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, HttpContext.Session.GetInt32("UnitId"));
        int clientId;
        if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
            outputData.EmployeeMastersKeyValues.EmployeeValidations = await _mastersKeyValueController.EmployeeValidationKeyValue("EmployeeMaster", "", clientId);
        return View("ReportingEmployees", outputData);

    }
    [HttpPost]
    //[Authorize(Roles = "Clientadmin,User")]
    public object UploadEmployeeProfileImage(EmployeeProfileImageDTO formData)
    {

        if (formData != null)
        {
            IActionResult actionResult;
            EmployeeMasterDTO viewModel = new EmployeeMasterDTO();
            var profileImage = formData.ProfileImageFile;
            byte[] decodedByteArray;
            if (formData.EmployeeId > 0)
            {
                if (profileImage != null)
                {
                    if (profileImage.Length > 0)
                    {
                        var fileName = Path.GetFileName(profileImage.FileName);
                        var fileExtension = Path.GetExtension(fileName);
                        viewModel.ProfileImageExtension = fileExtension;
                        formData.ProfileImageExtension = fileExtension;
                        var base64Data = formData.CroppedImageBase64.Split(',')[1];
                        if (CommonHelper.IsBase64String(base64Data))
                        {
                            //Save cropped image to Location
                            decodedByteArray = Convert.FromBase64CharArray(base64Data.ToCharArray(), 0, base64Data.Length);
                            string FilePath = Path.Combine(hostingEnv.WebRootPath, "EmployeeProfile", formData.UnitId.ToString());
                            if (!Directory.Exists(FilePath))
                                Directory.CreateDirectory(FilePath);
                            var filePath = Path.Combine(FilePath, formData.EmployeeId.ToString() + fileExtension);
                            System.IO.File.WriteAllBytes(filePath, decodedByteArray);

                            //Set the cropped image to save it in the DB
                            viewModel.ProfileImage = decodedByteArray;

                            // viewModel.ProfileImage.CopyTo(target);
                        }
                        //profileImage.CopyTo(target);
                        //viewModel.ProfileImage = target.ToArray();
                        viewModel.EmployeeId = formData.EmployeeId;
                        viewModel.FormName = formData.FormName;
                        viewModel.ClientId = formData.ClientId;
                        //using (var target = new MemoryStream())
                        //{


                        //}


                        //using (FileStream fs = System.IO.File.Create(filePath))
                        //{
                        //    profileImage.CopyTo(fs);
                        //}

                        if (formData.EmployeeId.ToString().Equals("0"))
                            actionResult = _employeeAPIController.SaveEmployeeProfileImage(formData);
                        else
                            actionResult = _employeeAPIController.UpdateEmployee(viewModel, "ProfileImage,ProfileImageExtension");

                        ObjectResult objResult = (ObjectResult)actionResult;
                        var objResultData = objResult.Value;
                    }
                }

                //string Base64String = "data:image/png;base64," + Convert.ToBase64String(formData.ProfileImage, 0, formData.ProfileImage.Length);       
                //string bs64 = Convert.ToBase64String(formData.ProfileImage);


            }
        }
        return Ok();
    }


    [HttpPost]
    //[Authorize(Roles = "Clientadmin")]
    public async Task<EditEmployeeDataDTO> UploadEditEmployeeProfileImage(EditEmployeeDataDTO formData)
    {
        if (formData != null)
        {
            IActionResult actionResult;
            //EmployeeMasterDTO viewModel = new EmployeeMasterDTO();
            //EditEmployeeDataDTO viewModel = new EditEmployeeDataDTO();
            var profileImage = formData.AttachmentFile;
            //IFormFile ProfileImageFile
            if (formData.EmployeeId > 0)
            {
                if (profileImage != null)
                {
                    if (profileImage.Length > 0)
                    {
                        var fileName = Path.GetFileName(profileImage.FileName);
                        var fileExtension = Path.GetExtension(fileName);
                        formData.DocumentType = fileExtension.Replace(".", "");
                        //formData.ProfileImageExtension = fileExtension;
                        using (var target = new MemoryStream())
                        {
                            profileImage.CopyTo(target);
                            formData.Attachment = target.ToArray();
                            formData.TicketId = CommonHelper.CreateTicket("EMPEdit", formData.TicketId);
                            formData.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));

                            formData = await _employeeAPIController.SaveEditEmployeeProfileImage(formData);

                            //viewModel.Attachment = target.ToArray();
                            //viewModel.EmployeeId = formData.EmployeeId;
                            //viewModel.FormName = formData.FormName;
                            //viewModel.ClientId = formData.ClientId;
                            //viewModel.ChangeType = formData.ChangeType;
                            //viewModel.EntrySource = formData.EntrySource;
                        }
                        //string FilePath = Path.Combine(hostingEnv.WebRootPath, "EmployeeProfile", formData.UnitId.ToString());
                        //if (!Directory.Exists(FilePath))
                        //    Directory.CreateDirectory(FilePath);
                        //var filePath = Path.Combine(FilePath, formData.EmployeeId.ToString() + fileExtension);
                        //using (FileStream fs = System.IO.File.Create(filePath))
                        //{
                        //    profileImage.CopyTo(fs);
                        //}
                    }
                }

                //string Base64String = "data:image/png;base64," + Convert.ToBase64String(formData.ProfileImage, 0, formData.ProfileImage.Length);       
                //string bs64 = Convert.ToBase64String(formData.ProfileImage);

                //if (formData.EmployeeId.ToString().Equals("0"))
                //    actionResult = _employeeAPIController.SaveEditEmployeeProfileImage(viewModel);
                //else
                //    actionResult = _employeeAPIController.UpdateEmployee(viewModel, "ProfileImage,ProfileImageExtension");

                //ObjectResult objResult = (ObjectResult)actionResult;
                //var objResultData = objResult.Value;
            }
        }
        return formData;
    }


    public async Task<List<EmployeeMasterDTO>?> GetEmployeeList()
    {
        int _loginId;

        int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
        bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        IActionResult actionResult = await _employeeAPIController.GetEmployeesForClient(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"), isclient, loginId);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<EmployeeMasterDTO> objResultData = (List<EmployeeMasterDTO>)objResult.Value;
        return objResultData;
    }

    public async Task<IList<EmployeeMasterDTO>> GetEmployeesInfo()
    {
        int _loginId;

        int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
        bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        IActionResult actionResult = await _employeeAPIController.GetEmployeesInfo(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"), isclient, loginId);
        ObjectResult objResult = (ObjectResult)actionResult;

        IList<EmployeeMasterDTO> objResultData = (IList<EmployeeMasterDTO>)objResult.Value;
        return objResultData;
    }

    public async Task<IList<EmployeeMasterDTO>> GetEmployeeListing()
    {
        int _loginId;

        int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
        bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        IActionResult actionResult = await _employeeAPIController.GetEmployeeListing(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"), isclient, loginId);
        ObjectResult objResult = (ObjectResult)actionResult;

        IList<EmployeeMasterDTO> objResultData = (IList<EmployeeMasterDTO>)objResult.Value;
        return objResultData;
    }




    [HttpGet]
    [Route("Employee/GetEmployeeInfo/{EmployeeId:int}")]
    [Authorize(Roles = "Clientadmin")]
    public async Task<IActionResult> GetEmployee(int EmployeeId)
    {
        EmployeeMasterDTO outputData = new EmployeeMasterDTO();
        if (!EmployeeId.ToString().Equals(0))
        {
            outputData.EmployeeId = EmployeeId;
            outputData = await _employeeAPIController.GetEmployee(outputData);
        }
        return View("Employee", outputData);
    }



    [HttpGet]
    [Route("Employee/DeleteEmployee/{EmployeeId:int}")]
    [Authorize(Roles = "Clientadmin")]
    public async Task<IActionResult> DeleteEmployee(int EmployeeId)
    {
        if (EmployeeId.ToString().Equals(string.Empty))
        {
            EmployeeMasterDTO outputData = new EmployeeMasterDTO();
            outputData.EmployeeId = EmployeeId;

            IActionResult actionResult;

            actionResult = await _employeeAPIController.DeleteEmployee(outputData);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;

            //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
            //{
            outputData.EmployeeId.ToString(string.Empty);
            outputData.EmployeeMasterList = await GetEmployeeList();
            outputData.DisplayMessage = "Employee record deactivated successfully";
            return View("Employee", outputData);
            //}
        }
        return RedirectToAction("Employee", "Employee");
    }


    public IActionResult GoogleCalndarEvents()
    {

        EmployeeDashboardVM outputData = new EmployeeDashboardVM();


        outputData.GoogleEvents = CalendarEventsTest();
        //  var CalenderEvnt1 = new GoogleCalendarReqDTO();
        try
        {

            //var status= CalendarEventsTest();
            // // ViewBag.EventList = GoogleEvents;
            //   CalenderEvnt1.Message = "Test";
            return View(outputData);
        }
        catch (SystemException ex)
        {
            // CalenderEvnt1.Message = ex.Message;
            return View();
        }
    }

    public List<GoogleCalendarReqDTO> CalendarEvents()
    {

        List<GoogleCalendarReqDTO> objlist = new List<GoogleCalendarReqDTO>();
        try
        {
            UserCredential credential;

            string path = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");
            // Load client secrets.
            using (var stream =
                   new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                /* The file token.json stores the user's access and refresh tokens, and is created
                 automatically when the authorization flow completes for the first time. */
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            foreach (var eventItem in events.Items)
            {
                var CalenderEvnt = new GoogleCalendarReqDTO();
                CalenderEvnt.Summary = eventItem.Summary;
                CalenderEvnt.Organizer = eventItem.Organizer.Email;
                CalenderEvnt.Description = eventItem.Description;
                //CalenderEvnt.StartTime = eventItem.Start.DateTime.Value.ToString("dd MMM hh:mm tt");
                CalenderEvnt.StartTime = eventItem.Start.DateTime.Value.ToString("hh:mm tt");
                CalenderEvnt.EndTime = eventItem.End.DateTime.Value.ToString("hh:mm tt");
                // var link = eventItem.HangoutLink;
                CalenderEvnt.Link = eventItem.HtmlLink;
                CalenderEvnt.Message = "Test";
                objlist.Add(CalenderEvnt);


            }

            return objlist;


        }
        catch (FileNotFoundException ex)
        {
            var CalenderEvnt = new GoogleCalendarReqDTO();
            //CalenderEvnt.Message = ex.Message;
            //GoogleEvents.Add(CalenderEvnt);
            return objlist;
            //Console.WriteLine(ex.Message);
        }
    }

    public List<GoogleCalendarReqDTO> CalendarEventsTest()
    {
        string sLogPath1 = _config.GetValue<string>("LogFilePathName");
        List<GoogleCalendarReqDTO> objlist = new List<GoogleCalendarReqDTO>();
        try
        {
            UserCredential credential;

            CommonHelper.WriteToFile(sLogPath1, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "first");
            // string path = Path.Combine(this.hostingEnv.WebRootPath, "Credentials.json");
            // Load client secrets.
            //  using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //{
            /* The file token.json stores the user's access and refresh tokens, and is created
             automatically when the authorization flow completes for the first time. */
            string credPath = "token.json";


            CommonHelper.WriteToFile(sLogPath1, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "first 11");

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
             new ClientSecrets
             {
                 ClientId = "93165296384-0tkje9l6n81ihmm4aos5o4p8uli4qfrl.apps.googleusercontent.com",
                 ClientSecret = "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"

             },
              Scopes,
              "user",
              CancellationToken.None, new FileDataStore(credPath, true)).Result;

            // }

            CommonHelper.WriteToFile(sLogPath1, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "first 222");
            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            // var bookshelves = await service..List(("primary").ExecuteAsync();
            CommonHelper.WriteToFile(sLogPath1, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "first 3333");
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            //Console.WriteLine("Upcoming events:");
            //if (events.Items == null || events.Items.Count == 0)
            //{
            //    Console.WriteLine("No upcoming events found.");
            //    return;
            //}
            foreach (var eventItem in events.Items)
            {
                var CalenderEvnt = new GoogleCalendarReqDTO();
                CalenderEvnt.Summary = eventItem.Summary;
                CalenderEvnt.Organizer = eventItem.Organizer.Email;
                CalenderEvnt.Description = eventItem.Description;
                //CalenderEvnt.StartTime = eventItem.Start.DateTime.Value.ToString("dd MMM hh:mm tt");
                CalenderEvnt.StartTime = eventItem.Start.DateTime.Value.ToString("hh:mm tt");
                CalenderEvnt.EndTime = eventItem.End.DateTime.Value.ToString("hh:mm tt");
                // var link = eventItem.HangoutLink;
                CalenderEvnt.Link = eventItem.HtmlLink;
                CalenderEvnt.Message = "Test";
                objlist.Add(CalenderEvnt);


            }

            return objlist;


        }
        catch (Exception ex)
        {

            var DisplayMessage = $"Source: {ex.Source}({nameof(CalendarEventsTest)})\n{ex.Message}";
            string sLogPath = _config.GetValue<string>("LogFilePathName");
            CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", DisplayMessage);
            var CalenderEvnt = new GoogleCalendarReqDTO();
            //CalenderEvnt.Message = ex.Message;
            //GoogleEvents.Add(CalenderEvnt);
            return objlist;
            //Console.WriteLine(ex.Message);
        }
    }

    public List<GoogleCalendarReqDTO> CalendarEventsTest1()
    {
        List<GoogleCalendarReqDTO> objlist = new List<GoogleCalendarReqDTO>();
        try
        {
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "486688183945-1fsdat6a30j15125d6m9b4g174hcldha.apps.googleusercontent.com",
                    ClientSecret = "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",
                },
                new[] { CalendarService.Scope.Calendar },
                "user",
                CancellationToken.None).Result;

            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Calender API v3",
            });

            var queryStart = DateTime.Now;
            var queryEnd = queryStart.AddYears(1);

            var query = service.Events.List("primary");
            // query.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime; - not supported :(
            query.TimeMin = queryStart;
            query.TimeMax = queryEnd;

            var events = query.Execute().Items;

            var eventList = events.Select(e => new Tuple<DateTime, string>(DateTime.Parse(e.Start.Date), e.Summary)).ToList();
            eventList.Sort((e1, e2) => e1.Item1.CompareTo(e2.Item1));

            // Console.WriteLine("Query from {0} to {1} returned {2} results", queryStart, queryEnd, eventList.Count);

            foreach (var item in eventList)
            {
                // Console.WriteLine("{0}\t{1}", item.Item1, item.Item2);
            }
            return objlist;
        }
        catch (Exception e)
        {
            return null;
            // Console.WriteLine("Exception encountered: {0}", e.Message);
        }

    }

    public async Task<IActionResult> RefreshCalendarEvents([FromBody] GoogleCalendarReqDTO inputDTO)
    {

        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();


        outputData.GoogleEvents = CalendarEventsTest();

        return PartialView("/Views/Shared/_partialViews/_todayMeetings.cshtml", outputData);
        // return PartialView("TodayMeetings/_todayMeetings", outputData);
    }

    public async Task<IActionResult> TodayMeetingEvents([FromBody] GoogleCalendarReqDTO inputDTO)
    {

        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();


        // outputData.GoogleEvents = CalendarEvents();// CalendarEventsTest();


        return PartialView("/Views/Shared/_partialViews/_todayMeetings.cshtml", outputData);
    }

    public async Task<IActionResult> TodaysMeetingEvents([FromBody] GoogleCalendarReqDTO inputDTO)
    {

        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();


        //   outputData.GoogleEvents = CalendarEvents();


        return PartialView("/Views/Shared/_partialViews/_todayMeetings.cshtml", outputData);
    }

    public async Task<IActionResult> FaceAttendanceEvent([FromBody] GoogleCalendarReqDTO inputDTO)
    {

        #region Employee Session
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (string.IsNullOrEmpty(strEmpSession))
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            SimpliHR.Infrastructure.Models.Page.Error error = new SimpliHR.Infrastructure.Models.Page.Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Index";
            return View("../Page/Error", error);
        }
        #endregion
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();


        //   outputData.GoogleEvents = CalendarEvents();


        return PartialView("FaceAttendance/_faceAttendance", outputData);
    }

    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] MyMeetingsDTO input)
    {

        try
        {
            EmployeeDashboardVM outPut = new EmployeeDashboardVM();
            int _loginId = 0;
            input.EmployeeId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
            input.UnitId = HttpContext.Session.GetInt32("UnitId");
            input.EncryptedPassword = CommonHelper.Encrypt(input.UserPassword);
            var results = _employeeAPIController.SaveEmployeeUserIdDetails(input);
            //  outPut.DisplayMessage = "Sucess";
            return PartialView("/Views/Shared/_partialViews/_todayMeetings.cshtml", outPut);
        }
        catch (Exception ex)
        {


            return View();
        }

    }

    public async Task<IActionResult> CalendarEventsDetails()
    {
        List<GoogleCalendarReqDTO> objlist = new List<GoogleCalendarReqDTO>();
        string url = "https://outlook.office365.com/ews/exchange.asmx";
        // ServicePointManager.ServerCertificateValidationCallback = this.CertificateValidationCallBack;
        ExchangeService _exchangeService = new ExchangeService();
        EmployeeDashboardVM outPut = new EmployeeDashboardVM();
        MyMeetingsDTO inPut = new MyMeetingsDTO();
        int _loginId = 0;
        inPut.EmployeeId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
        inPut.UnitId = HttpContext.Session.GetInt32("UnitId");
        try
        {
            IActionResult actionResultBirth = await _employeeAPIController.GetMyMeetnigDetails(inPut);
            ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
            var myAccountDetails = (List<MyMeetingsDTO>)objResultBirth.Value;
            if (myAccountDetails.Count > 0)
            {
                for (int i = 0; i < myAccountDetails.Count; i++)
                {
                    if (myAccountDetails[i].UserType.Trim() == "O")
                    {
                        //    _exchangeService.TraceListener = ITraceListenerInstance;
                        // _exchangeService.TraceFlags = TraceFlags.EwsRequest | TraceFlags.EwsResponse;
                        //   _exchangeService.TraceEnabled = true;

                        _exchangeService.Url = new Uri(url);
                        _exchangeService.UseDefaultCredentials = true;
                        string userPwd = CommonHelper.Decrypt(myAccountDetails[i].UserPassword);
                        _exchangeService.Credentials = new WebCredentials(myAccountDetails[i].UserId, userPwd);
                        DateTime startDate = DateTime.Now;
                        DateTime endDate = startDate.AddDays(30);
                        const int num_Apt = 5;
                        try
                        {
                            CalendarFolder calendar = CalendarFolder.Bind(_exchangeService, WellKnownFolderName.Calendar, new PropertySet());

                            CalendarView cView = new CalendarView(startDate, endDate, num_Apt);

                            cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End);
                            FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);
                            foreach (Appointment a in appointments)
                            {
                                var CalenderEvnt = new GoogleCalendarReqDTO();
                                CalenderEvnt.Summary = a.Subject;
                                CalenderEvnt.StartTime = a.Start.ToString("hh:mm tt");
                                CalenderEvnt.EndTime = a.End.ToString("hh:mm tt");
                                CalenderEvnt.Link = a.JoinOnlineMeetingUrl;
                                objlist.Add(CalenderEvnt);

                                //  string sub = a.Subject;
                                // DateTime sDate = a.Start;
                                // DateTime eDate = a.End;


                            }
                            outPut.GoogleEvents = objlist;
                        }
                        catch
                        {

                            return PartialView("/Views/Shared/_partialViews/_todayMeetings.cshtml", outPut);

                        }
                    }
                }
            }

            // outPut.GoogleEvents = CalendarEvents();
            return PartialView("/Views/Shared/_partialViews/_todayMeetings.cshtml", outPut);
        }
        catch (SystemException ex)
        {
            throw;
        }
    }


    public ActionResult RefreshTokens()
    {
        // var logPath = Path.Combine(this.hostingEnv.WebRootPath, "log");

        // CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "Khan11222");

        string sLogPath = _config.GetValue<string>("LogFilePathName");
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();
        string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");
        string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "Tokens.json");
        try
        {
            JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(Tpath));
            // RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();
            objRequest.AddQueryParameter("client_id", credital["client_id"].ToString());
            objRequest.AddQueryParameter("client_secret", credital["client_secret"].ToString());
            objRequest.AddQueryParameter("grant_type", "refresh_token");
            objRequest.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());
            //  CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "Khan11");
            var client = new RestClient(new System.Uri("https://oauth2.googleapis.com/token"));

            //var response = client.Post(objRequest);

            // RestClient client;
            // _restClient = new RestClient(new System.Uri("https://oauth2.googleapis.com/token"));
            // string baseUrl = @"https://oauth2.googleapis.com/token";
            //  HttpClient http = new HttpClient();
            // http.BaseAddress = new Uri(baseUrl);
            // client = new RestClient(http, new RestClientOptions { BaseUrl = new Uri(baseUrl) });
            var response = client.Post(objRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject newTokens = JObject.Parse(response.Content);
                newTokens["refresh_token"] = tokens["refresh_token"].ToString();
                System.IO.File.WriteAllText(Tpath, newTokens.ToString());
                // CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "mohd OK");
                outputData.GoogleEvents = GetAllEvents();
            }


        }
        catch (SystemException ex)
        {
            var DisplayMessage = $"Source: {ex.Source}({nameof(RefreshTokens)})\n{ex.Message}";
            CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", DisplayMessage);
            // outputData = null;
        }
        // return Redirect("/Employee/Dashboard");
        return PartialView("/Views/Shared/_partialViews/_todayMeetings.cshtml", outputData);
        // return View("ExceptionMessage");
    }

    public ActionResult RevokeTokens()
    {
        EmployeeDashboardVM outputData = new EmployeeDashboardVM();
        string sLogPath = _config.GetValue<string>("LogFilePathName");
        try
        {

            // string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");
            string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "Tokens.json");

            //JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(Tpath));
            RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();
            objRequest.AddQueryParameter("token", tokens["access_token"].ToString());


            var client = new RestClient(new System.Uri("https://oauth2.googleapis.com/revoke"));
            // var baseUrl = client.Options.BaseUrl;
            var response = client.Post(objRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                // RedirectToAction("CalendarAPIEvents", "CalendarAPI");
            }
        }
        catch (SystemException ex)
        {
            var DisplayMessage = $"Source: {ex.Source}({nameof(RevokeTokens)})\n{ex.Message}";
            CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", DisplayMessage);
            // outputData = null;
        }

        return PartialView("/Views/Shared/_partialViews/_todayMeetings.cshtml", outputData);
    }

    public List<GoogleCalendarReqDTO> GetAllEvents()
    {
        string sLogPath = _config.GetValue<string>("LogFilePathName");
        List<GoogleCalendarReqDTO> objlist = new List<GoogleCalendarReqDTO>();
        // string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");
        string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "Tokens.json");
        try
        {
            //  CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "First");
            //JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(Tpath));
            RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();
            objRequest.AddQueryParameter("key", "AIzaSyCDAJbpvP4Vld4NTN0K5E7DpZtZQztoXrE");
            //objRequest.AddQueryParameter("timeMin", DateTime.Now);
            //objRequest.AddQueryParameter("timeMax", DateTime.Now.AddDays(1));
            objRequest.AddHeader("Authorization", "Bearer " + tokens["access_token"]);
            objRequest.AddHeader("Accept", "Application/json");

            //  CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "Second");

            var client = new RestClient(new System.Uri("https://www.googleapis.com/calendar/v3/calendars/primary/events"));
            // var baseUrl = client.Options.BaseUrl;
            var response = client.Get(objRequest);
            // CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", response.Content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "OK");
                JObject calendarEvents = JObject.Parse(response.Content);
                var Events = calendarEvents["items"].ToObject<IEnumerable<Event>>();
                var allEvents = calendarEvents["items"].ToObject<IEnumerable<Event>>().TakeLast(5).Where(x => DateTime.ParseExact(x.Start.DateTimeRaw.Substring(0, 10), "MM/dd/yyyy", CultureInfo.CurrentUICulture.DateTimeFormat) >= DateTime.Now.Date);
                //  int? totalEvents= allEvents.
                if (allEvents != null)
                {
                    // CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "OK1");
                    // .ToString("hh:mm tt")
                    foreach (var item in allEvents)
                    {
                        // CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "OK2");
                        var CalenderEvnt = new GoogleCalendarReqDTO();
                        CalenderEvnt.Summary = item.Summary;
                        CalenderEvnt.StartTime = item.Start.DateTimeRaw.Substring(11, 5);
                        CalenderEvnt.EndTime = item.End.DateTimeRaw.Substring(11, 5);
                        CalenderEvnt.Link = item.HtmlLink;

                        objlist.Add(CalenderEvnt);
                    }
                    return objlist;

                }
                //  RedirectToAction("CalendarAPIEvents", "CalendarAPI");
            }
        }
        catch (SystemException ex)
        {
            var DisplayMessage = $"Source: {ex.Source}({nameof(GetAllEvents)})\n{ex.Message}";
            CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", DisplayMessage);
            // outputData = null;
        }
        return objlist;


        // return View("ExceptionMessage");
    }

    public IActionResult GetCaptcha()
    {
        return View();
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public IActionResult FinalSubmit()
    {
        //if(ModelState.IsValid)
        //{
        //if(!_ValidatorService.HasRequestValidCaptchaEntry())
        //{
        //    this.ModelState.AddModelError(_CaptchaOptions.CaptchaComponent.CaptchaInputName, "Please add the security code as number");
        //}
        //  }
        return View();
    }


}
