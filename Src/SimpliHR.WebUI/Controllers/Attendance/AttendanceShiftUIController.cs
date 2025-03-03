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
using System.Net;
using System.Linq.Expressions;
using Newtonsoft.Json;
using SimpliHR.Infrastructure.Models.Employee;
using iTextSharp.text.pdf.codec.wmf;

namespace SimpliHR.WebUI.Controllers.Attendance;

public class AttendanceShiftUIController : Controller
{
    private readonly EmployeeAttendanceController _employeeAttendanceAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly LoginController _loginAPIController;
    private IWebHostEnvironment Environment;


    public AttendanceShiftUIController(EmployeeAttendanceController employeeAttendanceAPIController, MastersKeyValueController mastersKeyValueController, LoginController loginController)
    {
        _employeeAttendanceAPIController = employeeAttendanceAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _loginAPIController = loginController;
    }

    public async Task<IActionResult> ManageShift(int pageNo = 0)
    {
        AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        //attendanceHistoryVM.EmployeeMasterKeyValue = await _mastersKeyValueController.EmployeeKeyValue();
        //attendanceHistoryVM.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue();
        return View("ManageShift", attendanceHistoryVM);

    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeAttendance(AttendanceHistoryViewModel attendanceHistoryVM)
    {
        attendanceHistoryVM = await GetEmployeeAttendanceList(attendanceHistoryVM);
        
        return View("ViewAttendance", attendanceHistoryVM);
    }

    public async Task<AttendanceHistoryViewModel> GetEmployeeAttendanceList(AttendanceHistoryViewModel attendanceHistoryVM)
    {
        IList<AttendanceHistoryDTO> outputList = new List<AttendanceHistoryDTO>();
        attendanceHistoryVM = await _employeeAttendanceAPIController.GetEmployeeAttendance(attendanceHistoryVM, 1000, 0);
        attendanceHistoryVM.AttendanceHistoryList = (List<AttendanceHistoryDTO>)outputList;
        //attendanceHistoryVM.EmployeeMasterKeyValue = await _mastersKeyValueController.EmployeeKeyValue();
        //attendanceHistoryVM.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue();
        //attendanceHistoryVM.WorkLocationKeyValue  = await _mastersKeyValueController.WorkLocationKeyValue();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        attendanceHistoryVM.EmployeeMasterKeyValue = (await _mastersKeyValueController.EmployeeKeyValue(x => x.IsActive == true && (x.ManagerId == attendanceHistoryVM.EmployeeId || x.EmployeeId == attendanceHistoryVM.EmployeeId))).ToList();
        attendanceHistoryVM.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue(p => (p.UnitId == unitId && p.IsActive == true));
        attendanceHistoryVM.WorkLocationKeyValue = await _mastersKeyValueController.WorkLocationKeyValue(p => (p.UnitId == unitId && p.IsActive == true));
        Expression<Func<ShiftMaster, bool>>? expression = p => (p.UnitId == unitId && p.IsActive == true);
        attendanceHistoryVM.ShiftMasterKeyValue = await _mastersKeyValueController.ShiftKeyValue(expression);
        return attendanceHistoryVM;
    }

    [HttpPost]
    // [JsonFilter(InputParam = "manualPunches", JsonDataType = typeof(List<ManualPunchesDTO>))]
    public async Task<IActionResult> ApplyManualPunches(AttendanceHistoryViewModel inputData)
    {
        //AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
        //string unitId = HttpContext.Session.GetInt32("UnitId").ToString();
        //string sEmployeeId = HttpContext.Session.GetString("EmployeeId").ToString();
        //inputData.ProfilePic = Path.Combine(_environment.WebRootPath, $"EmployeePofile\\{unitId}\\{sEmployeeId}.jpg");
        var addlist = Dns.GetHostEntry(Dns.GetHostName());
        string GetHostName = addlist.HostName.ToString();
        string GetIPV6 = addlist.AddressList[0].ToString();
        inputData.HostName = GetHostName;
        inputData.IPAddress = GetIPV6;
        inputData = await _employeeAttendanceAPIController.SendManualPunchesforApproval(inputData);
        //attendanceHistoryVM = await GetEmployeeAttendanceList(inputData);
        //attendanceHistoryVM.DisplayMessage = inputData.DisplayMessage;
        //return View("ViewAttendance", inputData);
        return Ok(inputData);
    }

}
