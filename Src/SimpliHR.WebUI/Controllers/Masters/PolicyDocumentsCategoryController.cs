using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Page;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class PolicyDocumentsCategoryController : Controller
    {
        private readonly PolicyDocumentsCategoryMasterController _policyDocumentsCategoryAPIController;

        public PolicyDocumentsCategoryController(PolicyDocumentsCategoryMasterController policyDocumentsCategoryAPIController)
        {
            _policyDocumentsCategoryAPIController = policyDocumentsCategoryAPIController;
        }
        public async Task<IActionResult> PolicyDocumentsCategory()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            PolicyDocumentsCategoryMasterDTO outputData = new PolicyDocumentsCategoryMasterDTO();
            IActionResult actionResult = await _policyDocumentsCategoryAPIController.GetPolicyDocumentsCategories(new Core.Helper.RequestParams { PageNumber = 1, PageSize = 100 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;
            outputData.PolicyDocumentsCategoryMasterList = (List<PolicyDocumentsCategoryMasterDTO>)objResult.Value;



            if (outputData != null)
            {
                foreach (var item in outputData.PolicyDocumentsCategoryMasterList)
                {
                    item.EncryptedId = CommonHelper.EncryptURLHTML(item.PolicyDocumentsCategoryId.ToString());
                }
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        public async Task<List<PolicyDocumentsCategoryMasterDTO>?> GetPolicyDocumentsCategoryList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _policyDocumentsCategoryAPIController.GetPolicyDocumentsCategories(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<PolicyDocumentsCategoryMasterDTO> objResultData = (List<PolicyDocumentsCategoryMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.PolicyDocumentsCategoryId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SavePolicyDocumentsCategory(PolicyDocumentsCategoryMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Academic", inputData);
            //}

            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            int? unitId = empSession.UnitId;
            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            PolicyDocumentsCategoryMasterDTO viewModel = new PolicyDocumentsCategoryMasterDTO();
            if (inputData.PolicyDocumentsCategoryId == 0)
                actionResult = await _policyDocumentsCategoryAPIController.SavePolicyDocumentsCategory(inputData);
            else
                actionResult = _policyDocumentsCategoryAPIController.UpdatePolicyDocumentsCategory(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;


            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.PolicyDocumentsCategoryId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.PolicyDocumentsCategoryId = 0;
                inputData.PolicyDocumentsCategoryMasterList = await GetPolicyDocumentsCategoryList();

            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            viewModel = inputData;
            return View("PolicyDocumentsCategory", viewModel);

        }


        [HttpGet]
        [Route("PolicyDocumentsCategory/GetPolicyDocumentsCategoryInfo/{ePolicyDocumentsCategoryId}")]
        public async Task<IActionResult> GetPolicyDocumentsCategoryInfo(string ePolicyDocumentsCategoryId)
        {
            int PolicyDocumentsCategoryId = 0;
            try
            {
                PolicyDocumentsCategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ePolicyDocumentsCategoryId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (PolicyDocumentsCategoryId != 0)
            {
                PolicyDocumentsCategoryMasterDTO outputData = new PolicyDocumentsCategoryMasterDTO();
                outputData.PolicyDocumentsCategoryId = PolicyDocumentsCategoryId;

                IActionResult actionResult;

                actionResult = await _policyDocumentsCategoryAPIController.GetPolicyDocumentsCategory(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (PolicyDocumentsCategoryMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("PolicyDocumentsCategory", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.PolicyDocumentsCategoryId = 0;
                    objResultData.PolicyDocumentsCategoryMasterList = await GetPolicyDocumentsCategoryList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("PolicyDocumentsCategory", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("PolicyDocumentsCategory", "PolicyDocumentsCategory");
        }

        [HttpGet]
        [Route("PolicyDocumentsCategory/DeletePolicyDocumentsCategory/{ePolicyDocumentsCategoryId}")]
        public async Task<IActionResult> DeletePolicyDocumentsCategory(string ePolicyDocumentsCategoryId)
        {
            int PolicyDocumentsCategoryId = 0;
            try
            {
                PolicyDocumentsCategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ePolicyDocumentsCategoryId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (PolicyDocumentsCategoryId != 0)
            {
                PolicyDocumentsCategoryMasterDTO outputData = new PolicyDocumentsCategoryMasterDTO();
                outputData.PolicyDocumentsCategoryId = PolicyDocumentsCategoryId;

                IActionResult actionResult;

                actionResult = await _policyDocumentsCategoryAPIController.DeletePolicyDocumentsCategory(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.PolicyDocumentsCategoryId = 0;
                outputData.PolicyDocumentsCategoryMasterList = await GetPolicyDocumentsCategoryList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("PolicyDocumentsCategory", outputData);
                //}
            }
            return RedirectToAction("PolicyDocumentsCategory", "PolicyDocumentsCategory");
        }
    }
}
