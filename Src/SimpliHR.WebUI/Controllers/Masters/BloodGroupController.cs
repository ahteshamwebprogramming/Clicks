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
    public class BloodGroupController : Controller
    {

        private readonly BloodGroupMasterController _BloodGroupAPIController;
        public BloodGroupController(BloodGroupMasterController BloodGroupAPIController)
        {
            _BloodGroupAPIController = BloodGroupAPIController;
        }

        public async Task<IActionResult> BloodGroup()
        {
            BloodGroupMasterDTO outputData = new BloodGroupMasterDTO();
            outputData.BloodGroupMasterList = await GetBloodGroupList();

            //foreach (var item in outputData.BloodGroupMasterList)
            //{
            //    item.EncryptedBloodGroupId = CommonHelper.EncryptURLHTML(item.BloodGroupId.ToString());
            //}

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        public async Task<List<BloodGroupMasterDTO>?> GetBloodGroupList()
        {

            IActionResult actionResult = await _BloodGroupAPIController.GetBloodGroups(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<BloodGroupMasterDTO> objResultData = (List<BloodGroupMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedBloodGroupId = CommonHelper.EncryptURLHTML(item.BloodGroupId.ToString());
            }
            return objResultData;
        }

        [HttpGet]
        [Route("BloodGroup/GetBloodGroupInfo/{eBloodGroupId}")]
        public async Task<IActionResult> GetBloodGroupInfo(string eBloodGroupId)
        {
            int BloodGroupId = 0;
            try
            {
                BloodGroupId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eBloodGroupId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
          
                if (BloodGroupId != 0)
                {
                    BloodGroupMasterDTO outputData = new BloodGroupMasterDTO();
                    outputData.BloodGroupId = BloodGroupId;

                    IActionResult actionResult;

                    actionResult = await _BloodGroupAPIController.GetBloodGroup(outputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    var objResultData = (BloodGroupMasterDTO)objResult.Value;
                    if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return View("BloodGroup", objResultData);
                        //return RedirectToAction("BloodGroup","BloodGroup", objResultData);
                    }
                    else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                    {
                        objResultData.BloodGroupId = 0;
                        objResultData.BloodGroupMasterList = await GetBloodGroupList();
                        objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                        //return View("BloodGroup",objResultData);
                        //return RedirectToAction("BloodGroup", objResultData);
                    }
                }
                return RedirectToAction("BloodGroup", "BloodGroup");
           
        }

        [HttpGet]
        [Route("BloodGroup/DeleteBloodGroup/{eBloodGroupId}")]
        public async Task<IActionResult> DeleteBloodGroup(string eBloodGroupId)
        {

            int BloodGroupId = 0;
            try
            {
                BloodGroupId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eBloodGroupId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (BloodGroupId != 0)
                {
                    BloodGroupMasterDTO outputData = new BloodGroupMasterDTO();
                    outputData.BloodGroupId = BloodGroupId;

                    IActionResult actionResult;

                    actionResult = await _BloodGroupAPIController.DeleteBloodGroup(outputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    var objResultData = objResult.Value;

                    //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                    //{
                    outputData.BloodGroupId = 0;
                    outputData.BloodGroupMasterList = await GetBloodGroupList();
                    outputData.DisplayMessage = "Transaction Successful!";
                    return View("BloodGroup", outputData);
                    //}
                }
                return RedirectToAction("BloodGroup", "BloodGroup");
           
        }

        [HttpPost]
        public async Task<IActionResult> SaveBloodGroup(BloodGroupMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("BloodGroup", inputData);
            //}
            inputData.IsActive = true;
            IActionResult actionResult;
            BloodGroupMasterDTO viewModel = new BloodGroupMasterDTO();
            if (inputData.BloodGroupId == 0)
                actionResult = _BloodGroupAPIController.SaveBloodGroup(inputData);
            else
                actionResult = _BloodGroupAPIController.UpdateBloodGroup(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.BloodGroupId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.BloodGroupId = 0;
                inputData.BloodGroupMasterList = await GetBloodGroupList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            viewModel = inputData;
            return View("BloodGroup", viewModel);

        }
    }
}
