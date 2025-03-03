using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using SimpliHR.WebUI.Modals.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.WebUI.Masters;
using System.Data;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Components.Forms;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class DistrictController : Controller
    {

        private readonly DistrictMasterController _districtAPIController;
        private readonly StateMasterController _stateAPIController;
        public DistrictController(DistrictMasterController DistrictAPIController, StateMasterController stateAPIController)
        {
            _districtAPIController = DistrictAPIController;
            _stateAPIController = stateAPIController;   
        }

        public async Task<IActionResult> District()
        {
            DistrictMasterDTO outputData = new DistrictMasterDTO();
            outputData.DistrictMasterList = await GetDistrictList();
            outputData.StateList = _stateAPIController.GetStateKeyValue().ToList();

            if (outputData != null)
            {
                //foreach (var item in outputData.DistrictMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.SalaryComponentId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        public async Task<List<DistrictMasterDTO>?> GetDistrictList()
        {

            IActionResult actionResult = await _districtAPIController.GetDistricts(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<DistrictMasterDTO> objResultData = (List<DistrictMasterDTO>)objResult.Value;
            //foreach (var item in objResultData)
            //{
            //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.DistrictId.ToString());
            //}
            return objResultData;
        }

        [HttpGet]
        [Route("District/GetDistrictInfo/{DistrictId:int}")]
        public async Task<IActionResult> GetDistrictInfo(int DistrictId)
        {
            if (DistrictId != 0)
            {
                DistrictMasterDTO outputData = new DistrictMasterDTO();
                outputData.DistrictId = DistrictId;

                IActionResult actionResult;

                actionResult = await _districtAPIController.GetDistrict(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (DistrictMasterDTO)objResult.Value;
                objResultData.StateList = _stateAPIController.GetStateKeyValue().ToList();
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("District", objResultData);
                    //return RedirectToAction("District","District", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.DistrictId = 0;
                    objResultData.DistrictMasterList = await GetDistrictList();
                    _districtAPIController.GetDistrictStates(outputData.DistrictMasterList, 1);
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    //return View("District",objResultData);
                    //return RedirectToAction("District", objResultData);
                }
            }
            return RedirectToAction("District", "District");
        }

        [HttpGet]
        [Route("District/DeleteDistrict/{DistrictId:int}")]
        public async Task<IActionResult> DeleteDistrict(int DistrictId)
        {
            if (DistrictId != 0)
            {
                DistrictMasterDTO outputData = new DistrictMasterDTO();
                outputData.DistrictId = DistrictId;

                IActionResult actionResult;

                actionResult = await _districtAPIController.DeleteDistrict(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.DistrictId = 0;
                outputData.DistrictMasterList = await GetDistrictList();
                _districtAPIController.GetDistrictStates(outputData.DistrictMasterList,1);
                outputData.DisplayMessage = "District deactivated successfully";
                return View("District", outputData);
                //}
            }
            return RedirectToAction("District", "District");
        }

        [HttpPost]
        public async Task<IActionResult> SaveDistrict(DistrictMasterDTO inputData)
        {
            inputData.IsActive = true;
            IActionResult actionResult;
            DistrictMasterDTO viewModel = new DistrictMasterDTO();
            inputData.CountryId = 1;
            if (inputData.DistrictId == 0)
                actionResult = _districtAPIController.SaveDistrict(inputData);
            else
                actionResult = _districtAPIController.UpdateDistrict(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;

            if (objResultData != null)
            {
                if (inputData.DistrictId == 0)
                    viewModel.DisplayMessage = "District successfully created";
                else
                    viewModel.DisplayMessage = "District updates completed successfully";
                inputData.DistrictId = 0;
                viewModel.DistrictMasterList = await GetDistrictList();
                //viewModel.StateList = _stateAPIController.GetStateKeyValue().ToList();
                _districtAPIController.GetDistrictStates(viewModel.DistrictMasterList,1);
                return View("District", viewModel);
            }
            else
            {
                return View();
            }

        }
    }
}
