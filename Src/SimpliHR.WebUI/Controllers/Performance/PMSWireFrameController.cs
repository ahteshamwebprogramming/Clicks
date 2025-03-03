using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Performance;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Page;
using SimpliHR.Infrastructure.Models.Performace;

namespace SimpliHR.WebUI.Controllers.Performance;

public class PMSWireFrameController : Controller
{
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly PerformanceSettingAPIController _performanceSettingAPIController;
    private readonly EmployeeMasterController _employeeMasterAPIController;
    public PMSWireFrameController(MastersKeyValueController mastersKeyValueController, PerformanceSettingAPIController performanceSettingAPIController, EmployeeMasterController employeeMasterController)
    {
        _mastersKeyValueController = mastersKeyValueController;
        _performanceSettingAPIController = performanceSettingAPIController;
        _employeeMasterAPIController = employeeMasterController;
    }

    public async Task<IActionResult> PMSWireFrame(string enc, string snc, string pnc)
    {
        int unitId = 0;
        string EmployeeCode = "";
        int EmployeeId = 0;
        int LoggedInEmployeeId = 0;
        int BandId = 0;
        PerformanceSettingViewModel dto = new PerformanceSettingViewModel();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool isPerformanceEmployeeData = false;
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            //return BadRequest("Session had expired. Please logout and login again");

            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        dto.ViewType = CommonHelper.DecryptURLHTML(snc);
        dto.EmployeeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(enc));
        dto.PerformanceSettingId = Convert.ToInt32(CommonHelper.DecryptURLHTML(pnc));
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (dto.EmployeeId == empSession.EmployeeId)
        {
            unitId = empSession.UnitId ?? default(int);
            EmployeeCode = empSession.EmployeeCode;
            EmployeeId = empSession.EmployeeId;
            BandId = empSession.BandId ?? default(int);
        }
        else
        {
            EmployeeMasterDTO? empDetails = await _employeeMasterAPIController.GetEmployeeById(dto.EmployeeId);
            unitId = empDetails.UnitId ?? default(int);
            EmployeeCode = empDetails.EmployeeCode;
            EmployeeId = empDetails.EmployeeId;
            BandId = empDetails.BandId ?? default(int);
        }
        if (String.IsNullOrEmpty(EmployeeCode))
        {
            Error error = new Error();
            error.Heading = "Employee Code Not found";
            error.Message = "No Employee Code is found for this employee. Please contact Admin";
            error.ButtonMessage = "Go to previous page";
            error.ButtonURL = "javascript:history.go(-1)";
            return View("../Page/Error", error);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        var performanceSettingRes = await _performanceSettingAPIController.GetPMSById(dto.PerformanceSettingId ?? default(int));
        if (performanceSettingRes != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)performanceSettingRes).StatusCode == 200)
            {
                dto.PerformanceSetting = (PerformanceSettingDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)performanceSettingRes).Value;
            }
            else
            {
                Error error = new Error();
                error.Heading = "PMS Not Found";
                error.Message = "No Performance Framework Found";
                error.ButtonMessage = "Go to previous page";
                error.ButtonURL = "javascript:history.go(-1)";
                return View("../Page/Error", error);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var resPerformanceEmployeeData = await _performanceSettingAPIController.PerformanceEmployeeData(dto.EmployeeId, dto.PerformanceSettingId ?? default(int));
        if (resPerformanceEmployeeData != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resPerformanceEmployeeData).StatusCode == 200)
            {
                dto.PerformanceEmployeeData = (PerformanceEmployeeDataDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resPerformanceEmployeeData).Value;
                isPerformanceEmployeeData = true;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var resAverageMethod = await _performanceSettingAPIController.PerformanceSettingAverageMethod(dto.PerformanceSettingId ?? default(int));
        if (resAverageMethod != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageMethod).StatusCode == 200)
            {
                dto.PerformanceSettingMechanismList = (List<PerformanceSettingMechanismDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageMethod).Value;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var resKRAAndBS = await _performanceSettingAPIController.PerformanceEmployeeKRADataListByEmployee(unitId, EmployeeCode, dto.PerformanceSettingId ?? default(int), isPerformanceEmployeeData ? dto.PerformanceEmployeeData.PerformanceEmployeeDataId : 0);
        if (resKRAAndBS != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resKRAAndBS).StatusCode == 200)
            {
                List<PerformanceEmployeeKRADataViewModel> performanceKRAMasterDBDTOs = (List<PerformanceEmployeeKRADataViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resKRAAndBS).Value;

                dto.PerformanceEmployeeKRAList = performanceKRAMasterDBDTOs.Where(x => x.Source == "KRA").ToList();
                dto.PerformanceEmployeeBehavioralList = performanceKRAMasterDBDTOs.Where(x => x.Source == "Behavioral").ToList();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var resSkillSetMatrix = await _performanceSettingAPIController.PerformanceSettingSkillSetMatrixBandWise(dto.PerformanceSetting.PerformanceSettingId, BandId);
        if (resSkillSetMatrix != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resSkillSetMatrix).StatusCode == 200)
            {
                dto.PerformanceSettingSkillSetMatrix = (PerformanceSettingSkillSetMatrixDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resSkillSetMatrix).Value;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var resTrainingNeed = await _performanceSettingAPIController.PerformanceEmployeeTrainingNeedListByEmployee(unitId, EmployeeCode, dto.PerformanceSetting.PerformanceSettingId, dto.PerformanceEmployeeData == null ? 0 : dto.PerformanceEmployeeData.PerformanceEmployeeDataId == null ? 0 : dto.PerformanceEmployeeData.PerformanceEmployeeDataId);
        if (resTrainingNeed != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resTrainingNeed).StatusCode == 200)
            {
                dto.PerformanceEmployeeTrainingDataList = (List<PerformanceEmployeeTrainingDataDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resTrainingNeed).Value;
            }
        }
        var resPerformanceTrainingNeedKeyValues = await _performanceSettingAPIController.PerformanceTrainingNeedKeyValues();
        if (resPerformanceTrainingNeedKeyValues != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resPerformanceTrainingNeedKeyValues).StatusCode == 200)
            {
                List<PageKeyValues> pageKeyValues = (List<PageKeyValues>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resPerformanceTrainingNeedKeyValues).Value;
                dto.PageKeyValueTrainingTypeList = pageKeyValues.Where(x => x.ControlName == "TrainingType").ToList();
                dto.PageKeyValueTrainingUrgencyList = pageKeyValues.Where(x => x.ControlName == "TrainingUrgency").ToList();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        dto.EmployeeCode = EmployeeCode;
        dto.EmployeeId = EmployeeId;
        //dto.ViewType = ViewType;

        if (dto.ViewType == "Employee")
        {
            if (dto.PerformanceEmployeeData != null)
            {
                if (dto.PerformanceEmployeeData.FilledByEmployee == true && (dto.PerformanceEmployeeData.FilledByManager == true || dto.PerformanceEmployeeData.FilledByHOD == true))
                {
                    if (dto.PerformanceEmployeeData.Published == false)
                    {
                        Error error = new Error();
                        error.Heading = "Fill PMS Notification";
                        error.Message = "Your performace review is under process right now. Please come back later";
                        error.ButtonMessage = "Go back to the previous page";
                        error.ButtonURL = "/Performance/PerformanceReview";
                        //return View("../Page/Error", error);

                        //return RedirectToAction("/Page/Error", error);
                    }
                }
            }
        }
        return View(dto);
    }

    //public async Task<IActionResult> KRAPartialView([FromBody] MISViewList inputDTO)
    //{
    //    try
    //    {
    //        var resKRAAndBS = await _performanceSettingAPIController.PerformanceEmployeeKRADataListByEmployee(unitId, EmployeeCode, dto.PerformanceSettingId ?? default(int), isPerformanceEmployeeData ? dto.PerformanceEmployeeData.PerformanceEmployeeDataId : 0);
    //        if (resKRAAndBS != null)
    //        {
    //            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resKRAAndBS).StatusCode == 200)
    //            {
    //                List<PerformanceEmployeeKRADataViewModel> performanceKRAMasterDBDTOs = (List<PerformanceEmployeeKRADataViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resKRAAndBS).Value;

    //                dto.PerformanceEmployeeKRAList = performanceKRAMasterDBDTOs.Where(x => x.Source == "KRA").ToList();
    //                dto.PerformanceEmployeeBehavioralList = performanceKRAMasterDBDTOs.Where(x => x.Source == "Behavioral").ToList();
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //    }

    //}
    //public async Task<IActionResult> BehaviouralPartialView([FromBody] MISViewList inputDTO)
    //{
    //    try
    //    {
    //        var resKRAAndBS = await _performanceSettingAPIController.PerformanceEmployeeBehaviouralDataListByEmployee(unitId, EmployeeCode, dto.PerformanceSettingId ?? default(int), isPerformanceEmployeeData ? dto.PerformanceEmployeeData.PerformanceEmployeeDataId : 0);
    //        if (resKRAAndBS != null)
    //        {
    //            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resKRAAndBS).StatusCode == 200)
    //            {
    //                List<PerformanceEmployeeKRADataViewModel> performanceKRAMasterDBDTOs = (List<PerformanceEmployeeKRADataViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resKRAAndBS).Value;

    //                dto.PerformanceEmployeeKRAList = performanceKRAMasterDBDTOs.Where(x => x.Source == "KRA").ToList();
    //                dto.PerformanceEmployeeBehavioralList = performanceKRAMasterDBDTOs.Where(x => x.Source == "Behavioral").ToList();
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //    }

    //}
}
