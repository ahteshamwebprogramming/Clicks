using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using System.Data;
using System.Linq;
using System.Net;

namespace SimpliHR.WebUI.Controllers.NewFolder
{
    public class EmployeeValidationController : Controller
    {
        private readonly EmployeeValidationAPIController _employeeValidationAPIController;
        private readonly MastersKeyValueController _mastersKeyValueController;

        public EmployeeValidationController(EmployeeValidationAPIController employeeValidationAPIAPIController, MastersKeyValueController mastersKeyValueController)
        {
            _employeeValidationAPIController = employeeValidationAPIAPIController;
            _mastersKeyValueController = mastersKeyValueController;
        }

        [HttpGet]
        [Route("EmployeeValidation/EmployeeValidation/{screenName}")]
        public async Task<IActionResult> EmployeeValidation(string screenName)
        {
            EmployeeValidationVM employeeValidationVM = new EmployeeValidationVM();
            EmployeeValidationVM outputData = new EmployeeValidationVM();
           List<EmployeeValidationDTO> tempValidation= new List<EmployeeValidationDTO>();
            //outputData.ScreenTabList = (await _mastersKeyValueController.EmployeeValidationKeyValue(x => x.IsActive == true && x.ScreenName.Replace(" ", "") == (screenName.IsNullOrEmpty() ? x.ScreenName.Replace(" ", "") : screenName.Replace(" ", "")))).OrderBy(r=>r.TabSequence).ToList();
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int? clientId = empSession.ClientId;
            outputData.ClientId = clientId;
            outputData.UnitId = empSession.UnitId;
            tempValidation = await _employeeValidationAPIController.GetEmployeeValidation(screenName, "", 0, 0);
            outputData.ScreenTabList = outputData.ScreenTabList = await _mastersKeyValueController.EmployeeValidationKeyValue(tempValidation);
            outputData.YesNoOptionList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "AnyYesNo" && x.PageName == "AnyYesNo" && x.ControlName == "AnyYesNo");
            outputData.ScreenName = screenName;
            return View(outputData);
        }

        [HttpPost]
        //[Route("EmployeeValidation/EmployeeValidation/")]
        public async Task<IActionResult> EmployeeValidation(EmployeeValidation empValidation)
        {
            string screenName = empValidation.ScreenName;
            string screenTab= empValidation.ScreenTab;
            EmployeeValidationVM outputData = new EmployeeValidationVM();
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int? clientId = empSession.ClientId;
            outputData.ClientId = clientId;
            outputData.UnitId = empSession.UnitId;
            outputData.ScreenName = screenName;
            outputData.EmployeeValidationList = await _employeeValidationAPIController.GetEmployeeValidation(screenName, screenTab,clientId, outputData.UnitId);
            outputData.ScreenTabList = await _mastersKeyValueController.EmployeeValidationKeyValue(screenName);
            //await _mastersKeyValueController.EmployeeValidationKeyValue(x => x.IsActive == true && x.ScreenName.Replace(" ", "") == (screenName.IsNullOrEmpty() ? x.ScreenName.Replace(" ", "") : screenName.Replace(" ", "")));
            outputData.YesNoOptionList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "AnyYesNo" && x.PageName == "AnyYesNo" && x.ControlName == "AnyYesNo");
            outputData.EmployeeValidation.ScreenName = empValidation.ScreenName;
            outputData.ScreenName = empValidation.ScreenName;
            outputData.PageAction = "Edit";
            return View(outputData);
        }


        [HttpPost]
        public async Task<IActionResult> SaveEmployeeValidation(EmployeeValidationVM employeeValidationVM)
        {
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int? clientId = empSession.ClientId;
            employeeValidationVM.LogInUser = empSession.EmployeeId;
            //employeeValidationVM.EmployeeValidation.ClientId = clientId;
            employeeValidationVM = await _employeeValidationAPIController.SaveEmployeeValidation(employeeValidationVM);
            employeeValidationVM.EmployeeValidationList = await _employeeValidationAPIController.GetEmployeeValidation(employeeValidationVM.ScreenName, employeeValidationVM.ScreenTab, clientId, employeeValidationVM.UnitId);
            employeeValidationVM.ScreenTabList = await _mastersKeyValueController.EmployeeValidationKeyValue(employeeValidationVM.EmployeeValidationList);
            employeeValidationVM.YesNoOptionList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "AnyYesNo" && x.PageName == "AnyYesNo" && x.ControlName == "AnyYesNo");
            return Ok(employeeValidationVM);
        }
        //public async Task<List<EmployeeValidationDTO>?> GetAcademicList()
        //{

        //    IActionResult actionResult = await _employeeValidationAPIAPIController.GetAcademics(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
        //    ObjectResult objResult = (ObjectResult)actionResult;

        //    List<EmployeeValidationDTO> objResultData = (List<EmployeeValidationDTO>)objResult.Value;
        //    foreach (var item in objResultData)
        //    {
        //        item.EncryptedAcademicId = CommonHelper.EncryptURLHTML(item.AcademicId.ToString());
        //    }
        //    return objResultData;
        //}
        //[HttpPost]
        //public async Task<IActionResult> SaveAcademic(EmployeeValidationDTO inputData)
        //{
        //    //if (!ModelState.IsValid)
        //    //{
        //    //    ViewBag.Div = "Add";
        //    //    return View("Academic", inputData);
        //    //}
        //    string? employeeId = HttpContext.Session.GetString("EmployeeId");
        //    int? unitId = HttpContext.Session.GetInt32("UnitId");
        //    inputData.UnitId = unitId;
        //    inputData.IsActive = true;
        //    IActionResult actionResult;
        //    EmployeeValidationDTO viewModel = new EmployeeValidationDTO();
        //    if (inputData.AcademicId == 0)
        //    {
        //        inputData.CreatedOn = DateTime.Now;
        //        inputData.CreatedBy = employeeId;
        //        actionResult = _employeeValidationAPIAPIController.SaveAcademic(inputData);
        //    }              
        //    else
        //    {
        //        inputData.ModifiedOn = DateTime.Now;
        //        inputData.ModifedBy = employeeId;
        //        actionResult = _employeeValidationAPIAPIController.UpdateAcademic(inputData);
        //    }


        //    ObjectResult objResult = (ObjectResult)actionResult;

        //    var objResultData = objResult.Value;
        //    inputData.HttpStatusCode = objResult.StatusCode;


        //    if (inputData.HttpStatusCode == 200)
        //    {
        //        if (inputData.AcademicId == 0)
        //            inputData.DisplayMessage = "Academic successfully created";
        //        else
        //            inputData.DisplayMessage = "Academic updates completed successfully";
        //        inputData.AcademicId = 0;
        //        inputData.EmployeeValidationList = await GetAcademicList();

        //    }
        //    else
        //        inputData.DisplayMessage = objResultData.ToString();

        //    viewModel = inputData;
        //    return View("Academic", viewModel);

        //}


        //[HttpGet]
        //[Route("Academic/GetAcademicInfo/{eacademicId}")]
        //public async Task<IActionResult> GetAcademicInfo(string eacademicId)
        //{
        //    int academicId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eacademicId));
        //    if (academicId != 0)
        //    {
        //        EmployeeValidationDTO outputData = new EmployeeValidationDTO();
        //        outputData.AcademicId = academicId;

        //        IActionResult actionResult;

        //        actionResult = await _employeeValidationAPIAPIController.GetAcademic(outputData);
        //        ObjectResult objResult = (ObjectResult)actionResult;
        //        var objResultData = (EmployeeValidationDTO)objResult.Value;
        //        if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
        //        {
        //            return View("Academic", objResultData);
        //            //return RedirectToAction("Role","Role", objResultData);
        //        }
        //        else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
        //        {
        //            objResultData.AcademicId = 0;
        //            objResultData.EmployeeValidationList = await GetAcademicList();
        //            objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
        //            return View("Academic", objResultData);
        //            //return RedirectToAction("Role", objResultData);
        //        }
        //    }
        //    return RedirectToAction("Academic", "Academic");
        //}

        //[HttpGet]
        //[Route("Academic/DeleteAcademic/{eacademicId}")]
        //public async Task<IActionResult> DeleteAcademic(string eacademicId)
        //{
        //    int academicId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eacademicId));
        //    if (academicId != 0)
        //    {
        //        EmployeeValidationDTO outputData = new EmployeeValidationDTO();
        //        outputData.AcademicId = academicId;

        //        IActionResult actionResult;

        //        actionResult = await _employeeValidationAPIAPIController.DeleteAcademic(outputData);
        //        ObjectResult objResult = (ObjectResult)actionResult;
        //        var objResultData = objResult.Value;

        //        //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
        //        //{
        //        outputData.AcademicId = 0;
        //        outputData.EmployeeValidationList = await GetAcademicList();
        //        outputData.DisplayMessage = "Academic record deactivated successfully";
        //        return View("Academic", outputData);
        //        //}
        //    }
        //    return RedirectToAction("Academic", "Academic");
        //}
    }
}
