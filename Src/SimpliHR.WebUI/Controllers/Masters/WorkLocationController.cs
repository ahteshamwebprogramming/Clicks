using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using System.Net;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class WorkLocationController : Controller
    {
        private readonly WorkLocationMasterController _workLocationAPIController;
        private readonly MastersKeyValueController _masterKeyValueAPIController;

        public WorkLocationController(WorkLocationMasterController workLocationAPIController, MastersKeyValueController masterKeyValueAPIController)
        {
            _workLocationAPIController = workLocationAPIController;
            _masterKeyValueAPIController = masterKeyValueAPIController;
        }



        public async Task<IActionResult> WorkLocation()
        {
            WorkLocationMasterDTO outputData = new WorkLocationMasterDTO();
            outputData.WorkLocationMasterList = await GetWorkLocationList();
            outputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<List<StateKeyValues>>? GetCounryStates(int countryId)
        {
            return await _masterKeyValueAPIController.StateKeyValue(true, countryId);
        }

        [HttpGet]
        public async Task<List<CityKeyValues>>? GetStateCities(int stateId)
        {
            return await _masterKeyValueAPIController.CityKeyValue(true, stateId);
        }

        public async Task<List<WorkLocationMasterDTO>> GetWorkLocationList()
        {
            int unitId = (int)HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _workLocationAPIController.GetWorkLocations(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1, IsActive = true }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<WorkLocationMasterDTO> objResultData = (List<WorkLocationMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.WorkLocationId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveWorkLocation(WorkLocationMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    inputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            //    return View("WorkLocation", inputData);
            //}
            inputData.IsActive = true;
            IActionResult actionResult;
            WorkLocationMasterDTO viewModel = new WorkLocationMasterDTO();
            int unitId = (int)HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            if (inputData.WorkLocationId == 0)
                actionResult = _workLocationAPIController.SaveWorkLocation(inputData);
            else
                actionResult = _workLocationAPIController.UpdateWorkLocation(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.WorkLocationId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";

                inputData.WorkLocationId = 0;
                inputData.WorkLocationMasterList = await GetWorkLocationList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            inputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            viewModel = inputData;
            return View("WorkLocation", viewModel);

        }


        [HttpGet]
        [Route("WorkLocation/GetWorkLocationInfo/{eworkLocationId}")]
        public async Task<IActionResult> GetWorkLocationInfo(string eworkLocationId)
        {
            int workLocationId = 0;
            try
            {
                workLocationId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eworkLocationId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (workLocationId != 0)
            {
                WorkLocationMasterDTO outputData = new WorkLocationMasterDTO();
                outputData.WorkLocationId = workLocationId;

                IActionResult actionResult;

                actionResult = await _workLocationAPIController.GetWorkLocation(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (WorkLocationMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    //objResultData.WorkLocationMasterList = await GetWorkLocationList();
                    objResultData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
                    return View("WorkLocation", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.WorkLocationId = 0;
                    objResultData.WorkLocationMasterList = await GetWorkLocationList();
                    objResultData.DisplayMessage = "You cannot edit locked work location. Contact Admin for further details";
                    return View("WorkLocation", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("WorkLocation", "WorkLocation");
        }

        [HttpGet]
        [Route("WorkLocation/DeleteWorkLocation/{eworkLocationId}")]
        public async Task<IActionResult> DeleteWorkLocation(string eworkLocationId)
        {
            int workLocationId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eworkLocationId));
            if (workLocationId != 0)
            {

                WorkLocationMasterDTO outputData = new WorkLocationMasterDTO();
                outputData.WorkLocationId = workLocationId;

                IActionResult actionResult;

                actionResult = await _workLocationAPIController.DeleteWorkLocation(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{

                outputData.WorkLocationMasterList = await GetWorkLocationList();
                outputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
                outputData.WorkLocationId = 0;
                outputData.DisplayMessage = "Transaction Successful!";
                return View("WorkLocation", outputData);
                //}
            }
            return RedirectToAction("WorkLocation", "WorkLocation");
        }
    }
}
