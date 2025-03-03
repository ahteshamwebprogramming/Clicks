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
using Newtonsoft.Json;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.WebUI.BL;

namespace SimpliHR.WebUI.Controllers.Attendance;

public class AttendanceRosterUIController : Controller
{
    private readonly AttendanceRosterController _rosterAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly LoginController _loginAPIController;

    public AttendanceRosterUIController(AttendanceRosterController rosterAPIController, MastersKeyValueController mastersKeyValueController, LoginController loginController)
    {
        _rosterAPIController = rosterAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _loginAPIController = loginController;
    }

    public async Task<IActionResult> Roster(int pageNo = 0)
    {
        int employeeId = 0;
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        AttendanceViewModel attendanceRosterVM = new AttendanceViewModel();
        attendanceRosterVM.AttendanceRoster.UnitId= HttpContext.Session.GetInt32("UnitId");
        //attendanceRosterVM.AttendanceMastersKeyValues = await _mastersKeyValueController.AttendanceMastersKeyValues();
        string sUnitId = HttpContext.Session.GetInt32("UnitId").ToString();
        if (empSession != null)
        {
            employeeId = empSession.EmployeeId;
        }
        bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        attendanceRosterVM.AttendanceMastersKeyValues = await _mastersKeyValueController.AttendanceMastersKeyValues(true, sUnitId, isclient, employeeId);
       
        var attendanceRosterList = await AttendanceRosters(1000, pageNo, "IsActive=1 and UnitId=" + sUnitId + "", "RosterName Asc");
        foreach (var item in attendanceRosterList)
        {
            item.EncryptedId = CommonHelper.EncryptURLHTML(item.RosterId.ToString());
        }
        attendanceRosterVM.AttendanceRosterList = attendanceRosterList;
        return View(attendanceRosterVM);
    }

    [HttpGet]
    [Route("AttendanceRosterUI/DeleteRosterDetails/{eRosterId}")]
    public async Task<IActionResult> DeleteRosterDetails(string eRosterId)
    {
        int rosterId;
        AttendanceViewModel avModel = new AttendanceViewModel();
        if (!string.IsNullOrEmpty(eRosterId))
        {
            rosterId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eRosterId));
            avModel.DisplayMessage = await _rosterAPIController.DeleteRosterDetails(rosterId);
        }
        return Ok(avModel);
    }

    [HttpGet]
    [Route("AttendanceRosterUI/GetRoster/{eRosterId}")]
    public async Task<IActionResult> GetRoster(string eRosterId)
    {
        int employeeId = 0;
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        int rosterId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eRosterId));
        if (rosterId != 0)
        {
            AttendanceViewModel attendanceRosterVM = new AttendanceViewModel();
            string sUnitId = HttpContext.Session.GetInt32("UnitId").ToString();
            if (empSession != null)
            {
                employeeId = empSession.EmployeeId;
            }
            bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
            attendanceRosterVM.AttendanceMastersKeyValues = await _mastersKeyValueController.AttendanceMastersKeyValues(true, sUnitId, isclient, employeeId);
            attendanceRosterVM.ViewScreen = "Edit";
            var attendanceRoster = await _rosterAPIController.GetRoster(rosterId);
            attendanceRosterVM.AttendanceRoster = attendanceRoster;
            return View("Roster", attendanceRosterVM);
        }
        return RedirectToAction("AttendanceRosterUI", "Roster");
    }

    public async Task<IList<AttendanceRosterDTO>> AttendanceRosters(int limit, int offset, string sWhere = "", string sOrderBy = "")
    {
        try
        {
            IList<AttendanceRosterDTO> outputModel = new List<AttendanceRosterDTO>();
            outputModel = await _rosterAPIController.AttendanceRosters(limit, offset, sWhere, sOrderBy);
            return outputModel;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveAttendanceRoster(AttendanceRosterDTO inputData)
    {
        try
        {
            int addedID = 0;
            int employeeId = 0;
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            IActionResult actionResult;
            dynamic objResultData = null;
            ObjectResult objResult;
            AttendanceViewModel attendanceRosterVM = new AttendanceViewModel();
            attendanceRosterVM.AttendanceRoster = inputData;
            string sUnitId = HttpContext.Session.GetInt32("UnitId").ToString();
            if (empSession != null)
            {
                employeeId = empSession.EmployeeId;
            }
            bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
            attendanceRosterVM.AttendanceMastersKeyValues = await _mastersKeyValueController.AttendanceMastersKeyValues(true, sUnitId, isclient, employeeId);
            IList<AttendanceRosterDTO> outputModel = new List<AttendanceRosterDTO>();
            //Perform the server side validation and 
            inputData.UnitId = attendanceRosterVM.AttendanceRoster.UnitId = HttpContext.Session.GetInt32("UnitId");
            actionResult = await _rosterAPIController.SaveAttendanceRoster(attendanceRosterVM);
            objResult = (ObjectResult)actionResult;
            objResultData = objResult.Value;
            attendanceRosterVM.ViewScreen = "Add";
            if (CommonHelper.IsNumeric(objResultData))
            {
                inputData.RosterId = Convert.ToInt32(objResultData);
            }
            attendanceRosterVM.DisplayMessage = objResultData;

         //   return RedirectToAction("Roster", "AttendanceRosterUI");
           return View("Roster", attendanceRosterVM);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
