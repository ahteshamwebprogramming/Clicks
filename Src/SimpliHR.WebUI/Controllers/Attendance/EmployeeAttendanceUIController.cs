using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Login;
using SimpliHR.Endpoints;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Services.DBContext;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.KeyValue;
using System.Collections.Generic;
using SimpliHR.Infrastructure.Helper;
using Microsoft.AspNetCore.Components.Forms;
using SimpliHR.Core.Entities;
using System.Linq;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Newtonsoft.Json;
using SimpliHR.Infrastructure.Models.Employee;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using SimpliHR.Infrastructure.Models.Login;
using SimpliHR.WebUI.BL;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.Drawing;
using OfficeOpenXml.FormulaParsing.FormulaExpressions;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http.Features;
using System.Net.Http;
using System;
using iTextSharp.text.pdf.codec.wmf;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.WebUI.Controllers.Attendance;

public class EmployeeAttendanceUIController : Controller
{
    private readonly EmployeeAttendanceController _employeeAttendanceAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly LoginController _loginAPIController;
    private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
    private readonly IHttpContextAccessor _accessor;
    private readonly ClientController _ClientController;

    private static Random random = new Random();
   
    public EmployeeAttendanceUIController(EmployeeAttendanceController employeeAttendanceAPIController, MastersKeyValueController mastersKeyValueController, LoginController loginController, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,IHttpContextAccessor accessor, ClientController ClientManagementController)
    {
        _employeeAttendanceAPIController = employeeAttendanceAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _loginAPIController = loginController;
        _environment = hostingEnvironment;
        _accessor = accessor;
        _ClientController = ClientManagementController;
    }

    [HttpGet]
    public async Task<IActionResult> LockAttendance(int pageNo = 0)
    {
        LockAttendanceVM lockAttendanceVM = new LockAttendanceVM();

        return View("LockAttendance", lockAttendanceVM);
    }
    [HttpPost]
    public async Task<IActionResult> LockAttendance(LockAttendanceDTO lockAttendance)
    {       
        LockAttendanceVM lockAttendanceVM = new LockAttendanceVM();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        lockAttendanceVM.LoggedInUser = empSession.EmployeeId;
        lockAttendance.UnitId = (int)empSession.UnitId;
        lockAttendance.CreatedOn=DateTime.Now;
        lockAttendance.CreatedBy = empSession.EmployeeId;
        lockAttendanceVM.LockAttendance = await _employeeAttendanceAPIController.LockAttendance(lockAttendance);
        lockAttendanceVM.DisplayMessage = lockAttendanceVM.LockAttendance.DisplayMessage;
        return Ok(lockAttendanceVM);
    }

    public async Task<IActionResult> ViewAttendance(int pageNo = 0)
    {
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        string startDate, endDate;
        IFormatProvider objFormat;
        //DateTime? starDate,endDate
       
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (attendanceHistoryVM.EmployeeId == 0)
        {
            attendanceHistoryVM.EmployeeId = empSession.EmployeeId;
            attendanceHistoryVM.eEmployeeId = CommonHelper.Encrypt(empSession.EmployeeId.ToString());
        }
        UnitMaster unitMaster = await _employeeAttendanceAPIController.GetUnitPayrollDates(empSession.UnitId);
        
        attendanceHistoryVM.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        attendanceHistoryVM.EndDate = attendanceHistoryVM.StartDate.Value.AddMonths(1).AddDays(-1);
        int lastDayOfMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
        
        if (unitMaster!=null)
        {
            attendanceHistoryVM.StartDate = (unitMaster.PayrollStartDate == 0 || unitMaster.PayrollStartDate == null) ? attendanceHistoryVM.StartDate : new DateTime(DateTime.Today.Year, DateTime.Today.Month, unitMaster.PayrollStartDate.Value);
            int dayDiff = unitMaster.PayrollEndDate.Value - unitMaster.PayrollStartDate.Value;
            //int iMonth = (DateTime.Today.AddMonths(1)).Month;
            if (dayDiff <= 0)
            {
                if (attendanceHistoryVM.StartDate >= DateTime.Now.Date)
                {
                    attendanceHistoryVM.StartDate = (unitMaster.PayrollStartDate == 0 || unitMaster.PayrollStartDate == null) ? attendanceHistoryVM.StartDate : new DateTime(DateTime.Today.Year, (DateTime.Today.AddMonths(-1)).Month, unitMaster.PayrollStartDate.Value);
                    attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, DateTime.Today.Month, unitMaster.PayrollStartDate.Value);
                }
                else
                    attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, (DateTime.Today.AddMonths(1)).Month, unitMaster.PayrollEndDate.Value);
            }
            else if (unitMaster.PayrollEndDate.Value > lastDayOfMonth)
            {
                    attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, (DateTime.Today.Month), lastDayOfMonth);
            }
            else
                attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, DateTime.Today.Month, unitMaster.PayrollEndDate.Value);
        }
        attendanceHistoryVM = await GetEmployeeAttendanceList(attendanceHistoryVM);
        return View("ViewAttendance", attendanceHistoryVM);
    }
    
    [HttpPost]
    public async Task<IActionResult> ViewAttendance(AttendanceHistoryViewModel attendanceVM)
    {
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        string startDate, endDate;
        IFormatProvider objFormat;
        //DateTime? starDate,endDate
        attendanceHistoryVM.StartDate = attendanceVM.StartDate;
        attendanceHistoryVM.EndDate = attendanceVM.EndDate;
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (attendanceHistoryVM.EmployeeId == 0)
        {
            attendanceHistoryVM.EmployeeId = attendanceVM.EmployeeId;
            attendanceHistoryVM.eEmployeeId = CommonHelper.Encrypt(attendanceHistoryVM.EmployeeId.ToString());
        }
        attendanceHistoryVM = await GetEmployeeAttendanceList(attendanceHistoryVM);
        return View("ViewAttendance", attendanceHistoryVM);
    }

    public async Task<IActionResult> AttendanceTickets(int pageNo = 0)
    {
        //AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        //string startDate, endDate;
        //IFormatProvider objFormat;
        ////DateTime? starDate,endDate
        //attendanceHistoryVM.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        //attendanceHistoryVM.EndDate = attendanceHistoryVM.StartDate.Value.AddMonths(1).AddDays(-1);
        //EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        //if (attendanceHistoryVM.EmployeeId == 0)
        //{
        //    attendanceHistoryVM.EmployeeId = empSession.EmployeeId;
        //    attendanceHistoryVM.eEmployeeId = CommonHelper.Encrypt(empSession.EmployeeId.ToString());
        //}
        //attendanceHistoryVM = await GetEmployeeAttendanceList(attendanceHistoryVM);
        //return View("ViewAttendance", attendanceHistoryVM);
        return View();
    }

    [HttpGet]
    [Route("/EmployeeAttendanceUI/MarkAttendance/{eEmployeeId}")]
    public async Task<IActionResult> MarkAttendance(string eEmployeeId)
    {
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

        int employeeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eEmployeeId));
        if (employeeId != 0)
        {
            attendanceHistoryVM.EmployeeId = employeeId;
            //  attendanceHistoryVM.eEmployeeId = CommonHelper.EncryptURLHTML(employeeId.ToString());
        }
        else
            attendanceHistoryVM.DisplayMessage = "Employee details not found. Parameters are missing";

        //attendanceHistoryVM.EmployeeMasterKeyValue = (await _mastersKeyValueController.EmployeeKeyValue()).Where(x => x.ManagerId == employeeId || x.EmployeeId == employeeId).ToList();
        //attendanceHistoryVM.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue();

        if (attendanceHistoryVM.StartDate == null)
            attendanceHistoryVM.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        if (attendanceHistoryVM.EndDate == null)
            attendanceHistoryVM.EndDate = attendanceHistoryVM.StartDate.Value.AddMonths(1).AddDays(-1);
        else if (attendanceHistoryVM.StartDate != null)
            attendanceHistoryVM.EndDate = attendanceHistoryVM.StartDate;
        UnitMaster unitMaster = await _employeeAttendanceAPIController.GetUnitPayrollDates(empSession.UnitId);

        attendanceHistoryVM.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        attendanceHistoryVM.EndDate = attendanceHistoryVM.StartDate.Value.AddMonths(1).AddDays(-1);
        if (unitMaster != null)
        {
            attendanceHistoryVM.StartDate = (unitMaster.PayrollStartDate == 0 || unitMaster.PayrollStartDate == null) ? attendanceHistoryVM.StartDate : new DateTime(DateTime.Today.Year, DateTime.Today.Month, unitMaster.PayrollStartDate.Value);
            int dayDiff = unitMaster.PayrollEndDate.Value - unitMaster.PayrollStartDate.Value;
            int lastDayOfMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            if (dayDiff <= 0)
                if (attendanceHistoryVM.StartDate >= DateTime.Now.Date)
                {
                    attendanceHistoryVM.StartDate = (unitMaster.PayrollStartDate == 0 || unitMaster.PayrollStartDate == null) ? attendanceHistoryVM.StartDate : new DateTime(DateTime.Today.Year, (DateTime.Today.AddMonths(-1)).Month, unitMaster.PayrollStartDate.Value);
                    attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, DateTime.Today.Month, unitMaster.PayrollStartDate.Value);
                }
                else
                    attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, (DateTime.Today.AddMonths(1)).Month, unitMaster.PayrollEndDate.Value);
            //attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, (DateTime.Today.AddMonths(1)).Month, unitMaster.PayrollEndDate.Value);
            else if (unitMaster.PayrollEndDate.Value > lastDayOfMonth)
                attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, (DateTime.Today.Month), lastDayOfMonth);
            else
                attendanceHistoryVM.EndDate = (unitMaster.PayrollEndDate == 0 || unitMaster.PayrollEndDate == null) ? attendanceHistoryVM.EndDate : new DateTime(DateTime.Today.Year, DateTime.Today.Month, unitMaster.PayrollEndDate.Value);
        }
        attendanceHistoryVM = await GetEmployeeAttendanceList(attendanceHistoryVM);
        return View("MarkAttendance", attendanceHistoryVM);

    }

    public async Task<IActionResult> CallSPUsingDapper()
    {
        var data = _employeeAttendanceAPIController.CallSPUsingDapper();
        return Ok();
    }
    public async Task<IActionResult> RegularizeAttendance()
    {
        ManualPunchesViewModel manualPunchVM = new ManualPunchesViewModel();
        if (HttpContext.Session.GetString("employee") != null)
        {
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int employeeId = empSession.EmployeeId;

            //int employeeId = 142; //empSession.EmployeeId;
            manualPunchVM = await _employeeAttendanceAPIController.GetAttendancePendingForApproval(employeeId);
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            Expression<Func<ShiftMaster, bool>>? expression = p => ((p.UnitId == unitId && p.IsActive == true));
            manualPunchVM.ShiftMasterKeyValue = await _mastersKeyValueController.ShiftKeyValue(expression);
            // manualPunchVM.ShiftMasterKeyValue = await _mastersKeyValueController.ShiftKeyValue(true);

        }
        return View(manualPunchVM);
    }

    [HttpPost]
    public async Task<IActionResult> RegularizeAttendance(ManualPunchesAction userAction)
    {
        ManualPunchesViewModel manualPunchVM = new ManualPunchesViewModel();

        UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
        userAction.DisplayName = unit.EmailDisplayName;

        ClientSettingDTO outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));
        userAction.EmailProvider = outputData.EmailProvider;

        string folderpath = "ClientLogo/" + outputData.ClientId.ToString() + "/" + outputData.ClientLogo;
        userAction.Profile = folderpath;


        if (userAction.ManualPunchIds != null && userAction.ManualPunchIds.Length != 0)
        {
            string sRetMsg = await _employeeAttendanceAPIController.ManualPunchesProcessing(userAction);
            manualPunchVM.DisplayMessage = sRetMsg;
        }
        else
            manualPunchVM.DisplayMessage = "Select punches " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
        return Ok(manualPunchVM.DisplayMessage);
    }


    [HttpPost]
    public async Task<IActionResult> GetEmployeeAttendance(AttendanceHistoryViewModel attendanceHistoryVM)
    {
        attendanceHistoryVM = await GetEmployeeAttendanceList(attendanceHistoryVM);
        attendanceHistoryVM.TicketId = CommonHelper.CreateTicket("Att", attendanceHistoryVM.TicketId);
        return View("MarkAttendance", attendanceHistoryVM);
    }

    //[HttpPost]
    //public async Task<IActionResult> GetEmployeeAttendanceView(AttendanceHistoryViewModel attendanceHistoryVM)
    //{
    //    attendanceHistoryVM = await GetEmployeeAttendanceList(attendanceHistoryVM);
    //    return View("ViewAttendance", attendanceHistoryVM);
    //}

    public async Task<AttendanceHistoryViewModel> GetEmployeeAttendanceList(AttendanceHistoryViewModel attendanceHistoryVM)
    {
        int employeeId = 0;
        IList<AttendanceDTO> outputList = new List<AttendanceDTO>();

        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        employeeId = empSession.EmployeeId;
        attendanceHistoryVM.eEmployeeId = CommonHelper.EncryptURLHTML(employeeId.ToString());

        int? unitId = HttpContext.Session.GetInt32("UnitId");
        bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        attendanceHistoryVM = await _employeeAttendanceAPIController.GetEmployeeAttendance(attendanceHistoryVM, 1000, 0);
       
        if(isclient)
            attendanceHistoryVM.EmployeeMasterKeyValue = (await _mastersKeyValueController.EmployeeKeyValue(x => x.IsActive == true && x.EmployeeStatus == "Active" && x.InfoFillingStatus == 1 && x.UnitId == unitId)).ToList();
        else
            attendanceHistoryVM.EmployeeMasterKeyValue = (await _mastersKeyValueController.EmployeeKeyValue(x => x.IsActive==true && x.EmployeeStatus=="Active" && x.InfoFillingStatus == 1 && x.UnitId== unitId && (x.HODId == empSession.EmployeeId || x.ManagerId == empSession.EmployeeId || x.EmployeeId == empSession.EmployeeId))).ToList();

        attendanceHistoryVM.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue(p => (p.UnitId == unitId && p.IsActive == true));
        attendanceHistoryVM.WorkLocationKeyValue = await _mastersKeyValueController.WorkLocationKeyValue(p => (p.UnitId == unitId && p.IsActive == true));
        Expression<Func<ShiftMaster, bool>>? expression = p => (p.UnitId == unitId && p.IsActive == true);
        attendanceHistoryVM.ShiftMasterKeyValue = await _mastersKeyValueController.ShiftKeyValue(expression);
       
        //attendanceHistoryVM.ShiftMasterKeyValue = await _mastersKeyValueController.ShiftKeyValue(true);
        return attendanceHistoryVM;
    }


    [HttpPost]
    // [JsonFilter(InputParam = "manualPunches", JsonDataType = typeof(List<ManualPunchesDTO>))]
    public async Task<IActionResult> ApplyManualPunches(AttendanceHistoryViewModel inputData)
    {
        UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
        inputData.DisplayName = unit.EmailDisplayName;

        ClientSettingDTO outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));
        inputData.EmailProvider = (int)outputData.EmailProvider;

        string folderpath = "ClientLogo/" + outputData.ClientId.ToString() + "/" + outputData.ClientLogo;
        inputData.Profile = folderpath;

        //inputData.TicketId = "Att/" + DateTime.Now.Month + "/" + GenerateTicket(6);
        inputData.TicketId = CommonHelper.CreateTicket("Att", inputData.TicketId);
        var addlist = Dns.GetHostEntry(Dns.GetHostName());
        string GetHostName = addlist.HostName.ToString();
        string GetIPV6 = addlist.AddressList[0].ToString();
        inputData.HostName = GetHostName;
        inputData.IPAddress = GetIPV6;
        inputData.AttendanceList.ForEach(x => x.HostName = GetHostName);
        inputData.AttendanceList.ForEach(x => x.IPAddress = GetIPV6);
        inputData.AttendanceList.ForEach(x => x.TicketId = inputData.TicketId);
        string unitId = HttpContext.Session.GetInt32("UnitId").ToString();
        string sEmployeeId = HttpContext.Session.GetString("EmployeeId").ToString();
        inputData.ProfilePic = System.IO.Path.Combine(_environment.WebRootPath, $"EmployeeProfile\\{unitId}\\{sEmployeeId}.jpg");
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId").Value;
        inputData = await _employeeAttendanceAPIController.SendManualPunchesforApproval(inputData);
        return Ok(inputData);
    }
    // For GPS and IP tracking 

    [HttpPost]
    public string SaveGPSLocation(AttendanceHistoryViewModel inputData)
    {
        try
        {
            AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
            EmployeeMasterDTO empSession;
          //  string employeeCode;
           // string unitId = HttpContext.Session.GetInt32("UnitId").ToString();
            if (HttpContext.Session.GetString("employee") != null)
            {
                empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
                //employeeCode = empSession.EmployeeId.ToString();
                var startTime = DateTime.Now;
                var addlist = Dns.GetHostEntry(Dns.GetHostName());
                string GetHostName = addlist.HostName.ToString();
                string GetIPV6 = addlist.AddressList[0].ToString();
                inputData.HostName = GetHostName;
                inputData.IPAddress = GetIPV6;
                inputData.EmployeeId= empSession.EmployeeId;

                // var startTime = DateTime.Now;
              //  int employeeId = empSession.EmployeeId;
                attendanceHistoryVM = _employeeAttendanceAPIController.UpdateGPSInAttendance(inputData, startTime);
            }
            else
            {
                attendanceHistoryVM.DisplayMessage = "Session Expired. Login again";
                return attendanceHistoryVM.DisplayMessage;
            }
        }
        catch(Exception ex)
        {

        }
      
       // string GetIPV4 = addlist.AddressList[1].ToString();

     


        return "";
    }




    [HttpGet]
    [Route("/EmployeeAttendanceUI/ProcessAttendance/{Ids}&{ua}")]
    public async Task<string> ProcessAttendance(string Ids, string ua)
    {
        string mpIds = CommonHelper.Decrypt(Ids);
        string sRetMsg = await _employeeAttendanceAPIController.ManualPunchesProcessing(mpIds, ua);
        return "Success";
    }


    [HttpGet]
    public async Task<IActionResult> ManageShift()
    {
        //string unitId = HttpContext.Session.GetInt32("UnitId").ToString();
        int? unitid = HttpContext.Session.GetInt32("UnitId");
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        attendanceHistoryVM.EmployeeMasterKeyValue = await _mastersKeyValueController.CompleteFilledInfoEmployeeKeyValue(p => (p.UnitId == unitid && p.IsActive == true)); ;
        attendanceHistoryVM.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue(p => (p.UnitId == unitid && p.IsActive == true));
        attendanceHistoryVM.UnitId = (int)unitid;
        attendanceHistoryVM.WeekDays = await _employeeAttendanceAPIController.GetWeekDays(HttpContext.Session.GetInt32("UnitId").ToString());
        //attendanceHistoryVM.ShiftMasterKeyValue = (await _mastersKeyValueController.ShiftKeyValue(true, unitId));
        return View("ManageShift", attendanceHistoryVM);

    }

    [HttpPost]
    public async Task<IActionResult> ManageShift(AttendanceHistoryViewModel inputData)
    {
        string sMsg = await _employeeAttendanceAPIController.SaveEmployeeShiftDetails(inputData);
        string unitIds = HttpContext.Session.GetInt32("UnitId").ToString();
        int? unitid = HttpContext.Session.GetInt32("UnitId");

        inputData.EmployeeMasterKeyValue = await _mastersKeyValueController.CompleteFilledInfoEmployeeKeyValue(p => (p.UnitId == unitid && p.IsActive == true));
        inputData.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue(p => (p.UnitId == unitid && p.IsActive == true));
        inputData.WeekDays = await _employeeAttendanceAPIController.GetWeekDays(unitIds);
        Expression<Func<ShiftMaster, bool>>? expression = p => (p.UnitId == unitid && p.IsActive == true);
        inputData.ShiftMasterKeyValue = await _mastersKeyValueController.ShiftKeyValue(expression);
        inputData.DisplayMessage = sMsg;
        return Ok(inputData);
    }
    public async Task<IActionResult> GetShiftList(AttendanceHistoryViewModel attendanceHistoryVM)
    {
        string unitIds = HttpContext.Session.GetInt32("UnitId").ToString();
        int? unitid = HttpContext.Session.GetInt32("UnitId");
        attendanceHistoryVM = await _employeeAttendanceAPIController.GetShiftList(attendanceHistoryVM, 1000, 0);
        attendanceHistoryVM.EmployeeMasterKeyValue = await _mastersKeyValueController.CompleteFilledInfoEmployeeKeyValue(p => (p.UnitId == unitid && p.IsActive == true));
        attendanceHistoryVM.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue(p => (p.UnitId == unitid && p.IsActive == true));
        attendanceHistoryVM.WeekDays = await _employeeAttendanceAPIController.GetWeekDays(unitIds);
        Expression<Func<ShiftMaster, bool>>? expression = p => (p.UnitId == unitid && p.IsActive == true);
        attendanceHistoryVM.ShiftMasterKeyValue = await _mastersKeyValueController.ShiftKeyValue(expression);

        // attendanceHistoryVM.ShiftMasterKeyValue = (await _mastersKeyValueController.ShiftKeyValue(true, unitId));
        return View("ManageShift", attendanceHistoryVM);
    }

    [HttpGet]
    [Route("/EmployeeAttendanceUI/EmployeeKeyValue/{unitIds}&{departmentIds}&{isActive}")]
    public async Task<List<EmployeeKeyValues>>? EmployeeKeyValue(string unitIds, string departmentIds, bool isActive = true)
    {
        if (unitIds != "")
            unitIds = "," + unitIds + ",";
        else
            unitIds = "0";


        if (unitIds != "")
            if (unitIds != "0")
            {
                departmentIds = (departmentIds == "0" ? "0" : "," + departmentIds + ",");
                //Expression<Func<EmployeeMaster, bool>>? expression = (p => ((isActive != null ? p.IsActive == isActive : true) && (departmentIds != "" && departmentIds.Contains("," + p.DepartmentId.ToString() + ",")) && (unitIds != "" && unitIds.Contains("," + p.UnitId.ToString() + ","))));
                Expression<Func<EmployeeMaster, bool>>? expression = (p => ((isActive != null ? p.IsActive == isActive : true) && (departmentIds == "0" ? true : departmentIds.Contains("," + p.DepartmentId.ToString() + ",")) && (unitIds == "0" ? true : unitIds.Contains("," + p.UnitId.ToString() + ","))));
                return await _mastersKeyValueController.EmployeeKeyValue(expression);
            }
            else
            {
                Expression<Func<EmployeeMaster, bool>>? expression = (p => ((isActive != null ? p.IsActive == isActive : true) && (unitIds != "" && unitIds.Contains("," + p.UnitId.ToString() + ","))));
                return await _mastersKeyValueController.EmployeeKeyValue(expression);
            }
        return null;

    }

    [HttpPost]
    public async Task<AttendanceHistoryViewModel> MarkFaceAttendance()
    {
        
        
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        EmployeeMasterDTO empSession;
        string employeeCode;
        string unitId = HttpContext.Session.GetInt32("UnitId").ToString(); ;
        bool faceMatched = false;
        string apiResponse=string.Empty;
        var files = HttpContext.Request.Form.Files;
        try
        {
            if (HttpContext.Session.GetString("employee") != null)
            {
                empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
                employeeCode = empSession.EmployeeId.ToString();
            }
            else
            {
                attendanceHistoryVM.DisplayMessage = "Session Expired. Login again";
                return attendanceHistoryVM;
            }
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Getting Filename  
                        var fileName = file.FileName;
                        // Unique filename "Guid"  
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                        // Getting Extension  
                        var fileExtension = System.IO.Path.GetExtension(fileName);
                        // Concating filename + fileExtension (unique filename)  
                        var newFileName = string.Concat(myUniqueFileName, fileExtension);
                        //  Generating Path to store photo
                        string folderPath = System.IO.Path.Combine(_environment.WebRootPath, $@"EmployeeProfile\AttendancePhotos\{unitId}");
                        bool exists = System.IO.Directory.Exists(folderPath);

                        if (!exists)
                            System.IO.Directory.CreateDirectory(folderPath);
                        else
                        {
                            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(folderPath);
                            foreach (System.IO.FileInfo dirfile in directory.GetFiles()) dirfile.Delete();
                        }

                        var filepath = folderPath + $@"\{newFileName}";

                        if (!string.IsNullOrEmpty(filepath))
                        {
                            // Storing Image in Folder  
                            CommonHelper.StoreInFolder(file, filepath);
                        }
                        var imageBytes = System.IO.File.ReadAllBytes(filepath);

                        if (imageBytes != null)
                        {
                            FaceRecognition faceRecognize = new FaceRecognition();

                            apiResponse = await faceRecognize.RecognizeEmployee(filepath, employeeCode, unitId, System.IO.Path.Combine(_environment.WebRootPath, $@"EmployeeProfile\{unitId}"), empSession.ProfileImageExtension);
                            if (apiResponse==string.Empty)
                            {
                                //var startTime = DateTime.Now;
                                var startTime = CommonHelper.GetZoneTime("India Standard Time");
                                int employeeId = empSession.EmployeeId;
                                string TicketId = CommonHelper.CreateTicket("ATT", "");  //"Att/" + DateTime.Now.Month + "/" + GenerateTicket(6);
                                attendanceHistoryVM = _employeeAttendanceAPIController.MarkFaceAttendance(employeeId, startTime, HttpContext.Request.Form["InOutTime"].ToString(), TicketId);
                                if (attendanceHistoryVM.FaceRecognitionAttendanceList.Count > 0 && attendanceHistoryVM.FaceRecognitionAttendanceList.Where(x => x.IsShiftOn == true).Select(p => p).ToList().Count > 0)
                                {
                                    attendanceHistoryVM.EPOCHInOutTime = CommonHelper.GetEpochTime(null);
                                    HttpContext.Session.SetString(HttpContext.Request.Form["InOutTime"].ToString(), startTime.ToString());
                                }
                            }
                            else
                            {
                                attendanceHistoryVM.DisplayMessage = apiResponse;
                            }
                        }
                        else
                        {
                            attendanceHistoryVM.DisplayMessage = "Transaction Failed!";
                        }
                        return attendanceHistoryVM;

                    }
                }
                return attendanceHistoryVM;
                //return Json("");
            }
            return attendanceHistoryVM;
        }
        catch (Exception ex)
        {
            attendanceHistoryVM.DisplayMessage = "Transaction Failed!";
            return attendanceHistoryVM;
        }
    }


    [HttpGet]
    [Route("/EmployeeAttendanceUI/GetAttendaceTime/{InOutTime}")]
    public async Task<AttendanceHistoryViewModel> GetAttendaceTime(string InOutTime)
    {
        int employeeId = 0;
        EmployeeMasterDTO empSession;
        ulong attendanceTime = 0;
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        DateTime? startDateTime;
        if (HttpContext.Session.GetString("employee") != null)
        {
            empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            employeeId = empSession.EmployeeId;
        }
        DateTime todayDate;
        todayDate = CommonHelper.GetZoneTime("India Standard Time");
        attendanceHistoryVM = await _employeeAttendanceAPIController.GetAttendanceInfo(employeeId, todayDate.Date);
        try
        {
            if (attendanceHistoryVM.FaceRecognitionAttendanceList != null)
            {
                startDateTime = attendanceHistoryVM.FaceRecognitionAttendanceList.Where(x => x.IsShiftOn == true).Select(p => p.ClockInTime).Max();
                if (startDateTime != null)
                {
                    double timeDiffInSec = DateTime.Now.Subtract(startDateTime.Value).TotalSeconds;
                    if (timeDiffInSec > attendanceHistoryVM.MaximumTime.ToTimeSpan().TotalSeconds && attendanceHistoryVM.MaximumTime.ToTimeSpan().TotalSeconds>0)
                    {
                        attendanceHistoryVM = _employeeAttendanceAPIController.MarkFaceAttendance(employeeId, DateTime.Now, "OutTime", "", true);
                        attendanceHistoryVM = await _employeeAttendanceAPIController.GetAttendanceInfo(employeeId, todayDate.Date);
                    }
                }
            }
            //}

            attendanceHistoryVM.EPOCHInOutTime = attendanceTime;

            //if (attendanceTime == 0)
            //{

            //}
        }
        catch (Exception ex) { }

        return attendanceHistoryVM;
    }

    public static string GenerateTicket(int length)
    {
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

}
