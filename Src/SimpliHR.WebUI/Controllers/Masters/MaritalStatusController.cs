using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using SimpliHR.Infrastructure.Helper;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class MaritalStatusController : Controller
    {
        private readonly MaritalStatusMasterController _maritalStatusAPIController;
        public MaritalStatusController(MaritalStatusMasterController maritalStatusAPIController)
        {
            _maritalStatusAPIController = maritalStatusAPIController;
        }

        public async Task<IActionResult> MaritalStatus()
        {
            MaritalStatusMasterDTO outputData = new MaritalStatusMasterDTO();
            outputData.MaritalStatusMasterList = await GetMaritalStatusList();
            if (outputData != null)
            {
                //foreach (var item in outputData.MaritalStatusMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.MaritalStatusId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<MaritalStatusMasterDTO>?> GetMaritalStatusList()
        {

            IActionResult actionResult = await _maritalStatusAPIController.GetMaritalStates(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<MaritalStatusMasterDTO> objResultData = (List<MaritalStatusMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.MaritalStatusId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveMaritalStatus(MaritalStatusMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("MaritalStatus", inputData);
            //}
            inputData.IsActive = true;
            IActionResult actionResult;
            MaritalStatusMasterDTO viewModel = new MaritalStatusMasterDTO();
            ;
            if (inputData.MaritalStatusId == 0)
                actionResult = _maritalStatusAPIController.SaveMaritalStatus(inputData);
            else
                actionResult = _maritalStatusAPIController.UpdateMaritalStatus(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.MaritalStatusId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.MaritalStatusId = 0;
                inputData.MaritalStatusMasterList = await GetMaritalStatusList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("MaritalStatus", viewModel);

        }


        [HttpGet]
        [Route("MaritalStatus/GetMaritalStatusInfo/{emaritalStatusId}")]
        public async Task<IActionResult> GetMaritalStatusInfo(string emaritalStatusId)
        {
            int maritalStatusId = 0;
            try
            {
                maritalStatusId = Convert.ToInt32(CommonHelper.DecryptURLHTML(emaritalStatusId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (maritalStatusId != 0)
            {
                MaritalStatusMasterDTO outputData = new MaritalStatusMasterDTO();
                outputData.MaritalStatusId = maritalStatusId;

                IActionResult actionResult;

                actionResult = await _maritalStatusAPIController.GetMaritalStatus(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (MaritalStatusMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("MaritalStatus", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.MaritalStatusId = 0;
                    objResultData.MaritalStatusMasterList = await GetMaritalStatusList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("MaritalStatus", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("MaritalStatus", "MaritalStatus");
        }

        [HttpGet]
        [Route("MaritalStatus/DeleteMaritalStatus/{emaritalStatusId}")]
        public async Task<IActionResult> DeleteMaritalStatus(string emaritalStatusId)
        {
            int maritalStatusId = 0;
            try
            {
                maritalStatusId = Convert.ToInt32(CommonHelper.DecryptURLHTML(emaritalStatusId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (maritalStatusId != 0)
            {
                MaritalStatusMasterDTO outputData = new MaritalStatusMasterDTO();
                outputData.MaritalStatusId = maritalStatusId;

                IActionResult actionResult;

                actionResult = await _maritalStatusAPIController.DeleteMaritalStatus(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.MaritalStatusId = 0;
                outputData.MaritalStatusMasterList = await GetMaritalStatusList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("MaritalStatus", outputData);
                //}
            }
            return RedirectToAction("MaritalStatus", "MaritalStatus");
        }
    }
}
