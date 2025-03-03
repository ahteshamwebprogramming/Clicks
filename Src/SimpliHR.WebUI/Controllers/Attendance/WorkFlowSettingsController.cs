using iTextSharp.text.pdf.codec.wmf;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Attendance;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Page;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Attendance
{
    public class WorkFlowSettingsController : Controller
    {

        private readonly MastersKeyValueController _masterKeyValueAPIController;
        private readonly WorkFlowSettingsAPIController _WorkFlowSettingsAPIController;
        public WorkFlowSettingsController(MastersKeyValueController masterKeyValueAPIController, WorkFlowSettingsAPIController WorkFlowSettingsAPIController)
        {
            _WorkFlowSettingsAPIController = WorkFlowSettingsAPIController;
            _masterKeyValueAPIController = masterKeyValueAPIController;
        }
        public async Task<IActionResult> WorkflowSettings()
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);

            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            WorkFlowSettingsDTO outputData = new WorkFlowSettingsDTO();
            outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
            outputData.WorkFlowSettingsList = await GetWorkFlowSettingList(outputData.UnitId);

            outputData.ModuleMasterList = await _masterKeyValueAPIController.ClientSelectedModules(empSession.UnitId ?? default(int));


            if (outputData != null)
            {

                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<WorkFlowSettingsDTO>?> GetWorkFlowSettingList(int? UnitId)
        {

            IActionResult actionResult = await _WorkFlowSettingsAPIController.GetWorkFlowSettingList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, UnitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<WorkFlowSettingsDTO> objResultData = (List<WorkFlowSettingsDTO>)objResult.Value;

            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.WorkFlowSettingsId.ToString());
            }
            return objResultData;
        }

        [HttpPost]
        public async Task<IActionResult> SaveWorkFlowSetting(WorkFlowSettingsDTO inputData)
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);

            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }

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

            IActionResult actionResult;
            WorkFlowSettingsDTO viewModel = new WorkFlowSettingsDTO();

            actionResult = _WorkFlowSettingsAPIController.WorkFlowSetting(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.WorkFlowSettingsId == 0)
                    inputData.DisplayMessage = "ADDSUCCESS";
                else
                    inputData.DisplayMessage = "EDITSUCCESS";
                ViewBag.div = "List";
                inputData.WorkFlowSettingsId = 0;
                inputData.WorkFlowSettingsList = await GetWorkFlowSettingList(inputData.UnitId);
            }
            else
            {
                ViewBag.Div = "Add";
                inputData.DisplayMessage = objResultData.ToString();
            }

            inputData.ModuleMasterList = await _masterKeyValueAPIController.ClientSelectedModules(empSession.UnitId ?? default(int));

            viewModel = inputData;
            return View("WorkflowSettings", viewModel);

        }


        [HttpGet]
        [Route("WorkFlowSettings/GetWorkflowSettingId/{eWorkFlowSettingsId}")]
        public async Task<IActionResult> GetWorkflowSettingId(string eWorkFlowSettingsId)
        {
            int WorkFlowSettingsId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eWorkFlowSettingsId));
            if (WorkFlowSettingsId != 0)
            {
                WorkFlowSettingsDTO outputData = new WorkFlowSettingsDTO();
                outputData.WorkFlowSettingsId = WorkFlowSettingsId;
                IActionResult actionResult;

                actionResult = await _WorkFlowSettingsAPIController.GetWorkFlowSettingByID(WorkFlowSettingsId);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (WorkFlowSettingsDTO)objResult.Value;
                objResultData.ModuleMasterList = await _masterKeyValueAPIController.ClientSelectedModules((int)HttpContext.Session.GetInt32("UnitId"));
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    //objResultData.WorkLocationMasterList = await GetWorkLocationList();
                    return View("WorkflowSettings", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.WorkFlowSettingsId = 0;
                    //objResultData.ModuleMasterList = await _masterKeyValueAPIController.ModuleKeyValue(p => p.IsActive == true);
                    //  objResultData.DisplayMessage = "You cannot edit locked work location. Contact Admin for further details";
                    return View("WorkflowSettings", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return View("WorkflowSettings", "WorkflowSettings");
        }


        [HttpGet]
        [Route("WorkFlowSettings/DeleteWorkflowSetting/{eWorkFlowSettingsId}")]
        public async Task<IActionResult> DeleteWorkflowSetting(string eWorkFlowSettingsId)
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);

            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            int WorkFlowSettingsId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eWorkFlowSettingsId));
            if (WorkFlowSettingsId != 0)
            {
                WorkFlowSettingsDTO outputData = new WorkFlowSettingsDTO();
                outputData.WorkFlowSettingsId = WorkFlowSettingsId;
                outputData.UnitId = HttpContext.Session.GetInt32("UnitId");

                IActionResult actionResult;

                actionResult = await _WorkFlowSettingsAPIController.DeleteWorkflowSetting(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.WorkFlowSettingsId = 0;
                outputData.WorkFlowSettingsList = await GetWorkFlowSettingList(outputData.UnitId);
                outputData.ModuleMasterList = await _masterKeyValueAPIController.ClientSelectedModules(empSession.UnitId ?? default(int));
                outputData.DisplayMessage = "DELETESUCCESS";
                return View("WorkflowSettings", outputData);
                //}
            }
            return RedirectToAction("WorkflowSettings", "WorkflowSettings");
        }
    }
}
