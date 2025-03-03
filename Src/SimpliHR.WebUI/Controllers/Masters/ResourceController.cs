using Microsoft.AspNetCore.Mvc;

using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using SimpliHR.Infrastructure.Helper;
using Microsoft.AspNetCore.Components.Forms;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class ResourceController : Controller
    {
        private readonly ResourceMasterController _resourceAPIController;
        public ResourceController(ResourceMasterController resourceAPIController)
        {
            _resourceAPIController = resourceAPIController;
        }

        public async Task<IActionResult> Resource()
        {

            ResourceMasterDTO outputData = new ResourceMasterDTO();
            outputData.ResourceMasterList = await GetResourceList();
            if (outputData != null)
            {
                //foreach (var item in outputData.ResourceMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.ResourceId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<ResourceMasterDTO>?> GetResourceList()
        {

            IActionResult actionResult = await _resourceAPIController.GetResources(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<ResourceMasterDTO> objResultData = (List<ResourceMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.ResourceId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveResource(ResourceMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Resource", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            ResourceMasterDTO viewModel = new ResourceMasterDTO();

            if (inputData.ResourceId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
                actionResult = _resourceAPIController.SaveResource(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifedBy = employeeId;
                actionResult = _resourceAPIController.UpdateResource(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.ResourceId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.ResourceId = 0;
                inputData.ResourceMasterList = await GetResourceList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("Resource", viewModel);

        }


        [HttpGet]
        [Route("Resource/GetResourceInfo/{eresourceId}")]
        public async Task<IActionResult> GetResourceInfo(string eresourceId)
        {

            int resourceId = 0;
            try
            {
                resourceId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eresourceId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (resourceId != 0)
            {
                ResourceMasterDTO outputData = new ResourceMasterDTO();
                outputData.ResourceId = resourceId;

                IActionResult actionResult;

                actionResult = await _resourceAPIController.GetResource(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (ResourceMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("Resource", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.ResourceId = 0;
                    objResultData.ResourceMasterList = await GetResourceList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("Resource", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("Resource", "Resource");
        }

        [HttpGet]
        [Route("Resource/DeleteResource/{eresourceId}")]
        public async Task<IActionResult> DeleteResource(string eresourceId)
        {
            int resourceId = 0;
            try
            {
                resourceId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eresourceId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (resourceId != 0)
            {

                ResourceMasterDTO outputData = new ResourceMasterDTO();
                outputData.ResourceId = resourceId;

                IActionResult actionResult;

                actionResult = await _resourceAPIController.DeleteResource(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.ResourceId = 0;
                outputData.ResourceMasterList = await GetResourceList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("Resource", outputData);
                //}
            }
            return RedirectToAction("Resource", "Resource");
        }
    }
}
