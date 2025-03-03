using iTextSharp.text.pdf.codec.wmf;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Attendance;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Attendance;
public class AttendanceController : Controller
{
    private readonly MastersKeyValueController _masterKeyValueAPIController;
    private readonly AttendanceSettingController _AttendanceSettingController;
    private readonly AttendanceLateSettingController _AttendanceLateSettingController;
    public AttendanceController(MastersKeyValueController masterKeyValueAPIController, AttendanceSettingController AttendanceSettingController, AttendanceLateSettingController AttendanceLateSettingController)
    {
        _AttendanceLateSettingController = AttendanceLateSettingController;
        _AttendanceSettingController = AttendanceSettingController;
        _masterKeyValueAPIController = masterKeyValueAPIController;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Roster()
    {
        return View();
    }

    public async Task<IActionResult> AttendanceLateSetting()
    {

        AttendanceLateSettingDTO outputData = new AttendanceLateSettingDTO();
        outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        outputData.AttendanceLateSettingList = await GetAttendanceLateSettingList(outputData.UnitId);        


        if (outputData != null)
        {

            return View(outputData);
        }
        else
        {
            return View();
        }
    }
    public async Task<IActionResult> AttendanceSetting()
    {
        AttendanceSettingDTO outputData = new AttendanceSettingDTO();
        outputData.UnitId =HttpContext.Session.GetInt32("UnitId");
        outputData.AttendanceSettingList = await GetAttendanceSettingList(outputData.UnitId);
        
        outputData.LocationKeyValues = await _masterKeyValueAPIController.WorkLocationKeyValue((p => p.IsActive == true && p.UnitId == outputData.UnitId), (m => m.OrderBy(x => x.Location)));
        
        Expression<Func<ShiftMaster, bool>>? expression = p => (p.UnitId == outputData.UnitId && p.IsActive == true);
        outputData.ShiftMasterList = await _masterKeyValueAPIController.ShiftKeyValue(expression);

        if (outputData != null)
        {
           
            return View(outputData);
        }
        else
        {
            return View();
        }
    }

    public async Task<List<AttendanceLateSettingDTO>?> GetAttendanceLateSettingList(int? UnitId)
    {

        IActionResult actionResult = await _AttendanceLateSettingController.GetAttendanceLateSettingList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, UnitId);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<AttendanceLateSettingDTO> objResultData = (List<AttendanceLateSettingDTO>)objResult.Value;

        foreach (var item in objResultData)
        {
            item.EncryptedId = CommonHelper.EncryptURLHTML(item.LateMasterId.ToString());
        }
        return objResultData;
    }

    public async Task<List<AttendanceSettingDTO>?> GetAttendanceSettingList(int? UnitId)
    {
      
        IActionResult actionResult = await _AttendanceSettingController.GetAttendanceSettingList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, UnitId);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<AttendanceSettingDTO> objResultData = (List<AttendanceSettingDTO>)objResult.Value;

        foreach (var item in objResultData)
        {
            item.EncryptedId = CommonHelper.EncryptURLHTML(item.AttendanceSettingId.ToString());
        }
        return objResultData;
    }

    [HttpPost]
    public async Task<IActionResult> SaveAttendanceSetting(AttendanceSettingDTO inputData)
    {
        //if (!ModelState.IsValid)
        //{
        //    ViewBag.Div = "Add";
        //    return View("Shift", inputData);
        //}
       
        inputData.IsActive = true;
        if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
            inputData.UnitId = 0;
            else
            inputData.UnitId = HttpContext.Session.GetInt32("UnitId");

        //inputData.ShiftCode = "G";

        IActionResult actionResult;
        AttendanceSettingDTO viewModel = new AttendanceSettingDTO();
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        Expression<Func<ShiftMaster, bool>>? expression = p => (p.UnitId == inputData.UnitId && p.IsActive == true);
        inputData.ShiftMasterList = await _masterKeyValueAPIController.ShiftKeyValue(expression);
        inputData.ShiftCode = inputData.ShiftMasterList.Where(r=>r.ShiftId==inputData.ShiftId).Select(p=>p.ShiftCode).FirstOrDefault().ToString();
        actionResult = _AttendanceSettingController.AttendanceSetting(inputData);

        ObjectResult objResult = (ObjectResult)actionResult;

        var objResultData = objResult.Value;
        inputData.HttpStatusCode = objResult.StatusCode;

        if (inputData.HttpStatusCode == 200)
        {
            if (inputData.AttendanceSettingId == 0)
                inputData.DisplayMessage = "ADDSUCCESS";
            else
                inputData.DisplayMessage = "EDITSUCCESS";
            inputData.AttendanceSettingId = 0;
            ViewBag.Div = "List";
            inputData.AttendanceSettingList = await GetAttendanceSettingList(inputData.UnitId);
            
        }
        else
        {
            ViewBag.div = "Add";
            inputData.DisplayMessage = objResultData.ToString();
        }
        inputData.LocationKeyValues = await _masterKeyValueAPIController.WorkLocationKeyValue((p => p.IsActive == true && p.UnitId == inputData.UnitId), (m => m.OrderBy(x => x.Location)));
        viewModel = inputData;
        return View("AttendanceSetting", viewModel);

    }


    [HttpPost]
    public async Task<IActionResult> SaveAttendanceLateSetting(AttendanceLateSettingDTO inputData)
    {
       
        inputData.IsActive = true;
        if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
            inputData.UnitId = 0;
        else
            inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
     

        IActionResult actionResult;
        AttendanceLateSettingDTO viewModel = new AttendanceLateSettingDTO();
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        actionResult = _AttendanceLateSettingController.SaveAttendanceLateSetting(inputData);

        ObjectResult objResult = (ObjectResult)actionResult;

        var objResultData = objResult.Value;
        inputData.HttpStatusCode = objResult.StatusCode;

        if (inputData.HttpStatusCode == 200)
        {
            if (inputData.LateMasterId == 0)
                inputData.DisplayMessage = "ADDSUCCESS";
            else
                inputData.DisplayMessage = "EDITSUCCESS";
            //inputData.DisplayMessage = "Attendance Late Setting updates completed successfully";
            inputData.LateMasterId = 0;
            inputData.AttendanceLateSettingList = await GetAttendanceLateSettingList(inputData.UnitId);
        }
        else
        {
            ViewBag.div = "Add";
            inputData.DisplayMessage = objResultData.ToString();
        }
        viewModel = inputData;
        return View("AttendanceLateSetting", viewModel);

    }


    [HttpGet]
    [Route("Attendance/GetAttendanceSettingId/{eAttendanceSettingId}")]
    public async Task<IActionResult> GetAttendanceSettingId(string eAttendanceSettingId)
    {
        int AttendanceSettingId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eAttendanceSettingId));
        if (AttendanceSettingId != 0)
        {
            AttendanceSettingDTO outputData = new AttendanceSettingDTO();
            outputData.AttendanceSettingId = AttendanceSettingId;
            IActionResult actionResult;

            actionResult = await _AttendanceSettingController.GetAttendanceSettingByID(AttendanceSettingId);

            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = (AttendanceSettingDTO)objResult.Value;
            //objResultData.ShiftMasterList = await _masterKeyValueAPIController.ShiftKeyValue(true);
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            Expression<Func<ShiftMaster, bool>>? expression= p => (p.UnitId == unitId && p.IsActive == true);
            objResultData.ShiftMasterList = await _masterKeyValueAPIController.ShiftKeyValue(expression);
            if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
            {
                //objResultData.WorkLocationMasterList = await GetWorkLocationList();
                
                objResultData.LocationKeyValues = await _masterKeyValueAPIController.WorkLocationKeyValue((p => p.IsActive == true && p.UnitId == objResultData.UnitId), (m => m.OrderBy(x => x.Location)));
                return View("AttendanceSetting", objResultData);
                //return RedirectToAction("Role","Role", objResultData);
            }
            else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
            {
                objResultData.AttendanceSettingId = 0;
                objResultData.LocationKeyValues = await _masterKeyValueAPIController.WorkLocationKeyValue((p => p.IsActive == true && p.UnitId == objResultData.UnitId), (m => m.OrderBy(x => x.Location)));
                //  objResultData.DisplayMessage = "You cannot edit locked work location. Contact Admin for further details";
                return View("AttendanceSetting", objResultData);
                //return RedirectToAction("Role", objResultData);
            }
        }
        return RedirectToAction("AttendanceSetting", "Attendance");
    }




    [HttpGet]
    [Route("Attendance/GetAttendanceLateSettingId/{eLateMasterId}")]
    public async Task<IActionResult> GetAttendanceLateSettingId(string eLateMasterId)
    {
        int LateMasterId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eLateMasterId));
        if (LateMasterId != 0)
        {
            AttendanceLateSettingDTO outputData = new AttendanceLateSettingDTO();
            outputData.LateMasterId = LateMasterId;
            IActionResult actionResult;

            actionResult = await _AttendanceLateSettingController.GetAttendanceLateSetting(outputData);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = (AttendanceLateSettingDTO)objResult.Value;
            if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
            {
                //objResultData.WorkLocationMasterList = await GetWorkLocationList();
             //   objResultData.ShiftMasterList = await _masterKeyValueAPIController.ShiftKeyValue(true);
                return View("AttendanceLateSetting", objResultData);
                //return RedirectToAction("Role","Role", objResultData);
            }
            else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
            {
                objResultData.LateMasterId = 0;
               // objResultData.ShiftMasterList = await _masterKeyValueAPIController.ShiftKeyValue(true);
                //  objResultData.DisplayMessage = "You cannot edit locked work location. Contact Admin for further details";
                return View("AttendanceLateSetting", objResultData);
                //return RedirectToAction("Role", objResultData);
            }
        }
        return RedirectToAction("AttendanceLateSetting", "Attendance");
    }


    [HttpGet]
    [Route("Attendance/DeleteAttendanceSetting/{eAttendanceSettingId}")]
    public async Task<IActionResult> DeleteAttendanceSetting(string eAttendanceSettingId)
    {
       int AttendanceSettingId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eAttendanceSettingId));
        if (AttendanceSettingId != 0)
        {
            AttendanceSettingDTO outputData = new AttendanceSettingDTO();
            outputData.AttendanceSettingId = AttendanceSettingId;
            outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
            string unitIds = outputData.UnitId.ToString();
            IActionResult actionResult;

            actionResult = await _AttendanceSettingController.DeleteAttendanceSetting(outputData);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;

            //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
            //{
            outputData.AttendanceSettingId = 0;
            Expression<Func<ShiftMaster, bool>>? expression = p => ((unitIds != "" ? unitIds.Contains("," + p.UnitId.ToString() + ",") : p.UnitId == p.UnitId) && (p.IsActive == true));
            outputData.ShiftMasterList = await _masterKeyValueAPIController.ShiftKeyValue(expression);
            outputData.LocationKeyValues = await _masterKeyValueAPIController.WorkLocationKeyValue((p => p.IsActive == true && p.UnitId == outputData.UnitId), (m => m.OrderBy(x => x.Location)));
            outputData.AttendanceSettingList = await GetAttendanceSettingList(outputData.UnitId);
            outputData.DisplayMessage = "DELETESUCCESS";
            return View("AttendanceSetting", outputData);
            //}
        }
        return RedirectToAction("AttendanceSetting", "Attendance");
    }


    [HttpGet]
    [Route("Attendance/DeleteAttendanceLateSetting/{eAttendanceLateSettingId}")]
    public async Task<IActionResult> DeleteAttendanceLateSetting(string eAttendanceLateSettingId)
    {
        int LateMasterId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eAttendanceLateSettingId));
        if (LateMasterId != 0)
        {
            AttendanceLateSettingDTO outputData = new AttendanceLateSettingDTO();
            outputData.LateMasterId = LateMasterId;
            outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult;

            actionResult = await _AttendanceLateSettingController.DeleteAttendanceLateSetting(outputData);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;

            //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
            //{
            outputData.LateMasterId = 0;
            outputData.AttendanceLateSettingList = await GetAttendanceLateSettingList(outputData.UnitId);
            outputData.DisplayMessage = "Transaction Successful!";
            return View("AttendanceLateSetting", outputData);
            //}
        }
        return RedirectToAction("AttendanceLateSetting", "Attendance");
    }
}
