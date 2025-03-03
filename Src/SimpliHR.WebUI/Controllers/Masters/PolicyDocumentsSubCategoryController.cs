using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class PolicyDocumentsSubCategoryController : Controller
    {
        private readonly PolicyDocumentsSubCategoryMasterController _policyDocumentsSubCategoryAPIController;
        private readonly PolicyDocumentsCategoryMasterController _policyDocumentsCategoryAPIController;

        public PolicyDocumentsSubCategoryController(PolicyDocumentsSubCategoryMasterController policyDocumentsSubCategoryAPIController, PolicyDocumentsCategoryMasterController policyDocumentsCategoryAPIController)
        {
            _policyDocumentsSubCategoryAPIController = policyDocumentsSubCategoryAPIController;
            _policyDocumentsCategoryAPIController = policyDocumentsCategoryAPIController;
        }
        public async Task<IActionResult> PolicyDocumentsSubCategory()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            PolicyDocumentsSubCategoryMasterDTO outputData = new PolicyDocumentsSubCategoryMasterDTO();
            outputData.PolicyDocumentsSubCategoryMasterList = await GetPolicyDocumentsSubCategoryList();
            outputData.PolicyDocumentCategoryList = _policyDocumentsCategoryAPIController.GetPolicyDocumentsCategoryKeyValue(unitId).ToList();
            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        public async Task<List<PolicyDocumentsSubCategoryMasterDTO>?> GetPolicyDocumentsSubCategoryList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            // inputData.UnitId = unitId;
            IActionResult actionResult = await _policyDocumentsSubCategoryAPIController.GetPolicyDocumentsSubCategories(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<PolicyDocumentsSubCategoryMasterDTO> objResultData = (List<PolicyDocumentsSubCategoryMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.PolicyDocumentsSubCategoryId.ToString());
            }

            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SavePolicyDocumentsSubCategory(PolicyDocumentsSubCategoryMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Academic", inputData);
            //}
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            PolicyDocumentsSubCategoryMasterDTO viewModel = new PolicyDocumentsSubCategoryMasterDTO();
            if (inputData.PolicyDocumentsSubCategoryId == 0)
                actionResult = _policyDocumentsSubCategoryAPIController.SavePolicyDocumentsSubCategory(inputData);
            else
                actionResult = _policyDocumentsSubCategoryAPIController.UpdatePolicyDocumentsSubCategory(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.PolicyDocumentsSubCategoryId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.PolicyDocumentsSubCategoryId = 0;
                inputData.PolicyDocumentsSubCategoryMasterList = await GetPolicyDocumentsSubCategoryList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            inputData.PolicyDocumentCategoryList = _policyDocumentsCategoryAPIController.GetPolicyDocumentsCategoryKeyValue(unitId).ToList();
            viewModel = inputData;
            return View("PolicyDocumentsSubCategory", viewModel);

        }


        [HttpGet]
        [Route("PolicyDocumentsSubCategory/GetPolicyDocumentsSubCategoryInfo/{ePolicyDocumentsSubCategoryId}")]
        public async Task<IActionResult> GetPolicyDocumentsSubCategoryInfo(string ePolicyDocumentsSubCategoryId)
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int PolicyDocumentsSubCategoryId = 0;
            try
            {
                PolicyDocumentsSubCategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ePolicyDocumentsSubCategoryId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (PolicyDocumentsSubCategoryId != 0)
            {
                PolicyDocumentsSubCategoryMasterDTO outputData = new PolicyDocumentsSubCategoryMasterDTO();
                outputData.PolicyDocumentsSubCategoryId = PolicyDocumentsSubCategoryId;

                IActionResult actionResult;

                actionResult = await _policyDocumentsSubCategoryAPIController.GetPolicyDocumentsSubCategory(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (PolicyDocumentsSubCategoryMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    objResultData.PolicyDocumentCategoryList = _policyDocumentsCategoryAPIController.GetPolicyDocumentsCategoryKeyValue(unitId).ToList();
                    return View("PolicyDocumentsSubCategory", objResultData);
                    //return RedirectToAction("State","State", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.PolicyDocumentsSubCategoryId = 0;
                    objResultData.PolicyDocumentsSubCategoryMasterList = await GetPolicyDocumentsSubCategoryList();
                    outputData.PolicyDocumentCategoryList = _policyDocumentsCategoryAPIController.GetPolicyDocumentsCategoryKeyValue(unitId).ToList();
                    //_stateAPIController.GetStatesCountry(outputData.StateMasterList);
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    //return View("State",objResultData);
                    //return RedirectToAction("State", objResultData);
                }
            }
            return RedirectToAction("PolicyDocumentsSubCategory", "PolicyDocumentsSubCategory");
        }

        [HttpGet]
        [Route("PolicyDocumentsSubCategory/DeletePolicyDocumentsSubCategory/{ePolicyDocumentsSubCategoryId}")]
        public async Task<IActionResult> DeletePolicyDocumentsSubCategory(string ePolicyDocumentsSubCategoryId)
        {
            int PolicyDocumentsSubCategoryId = 0;
            try
            {
                PolicyDocumentsSubCategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ePolicyDocumentsSubCategoryId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (PolicyDocumentsSubCategoryId != 0)
            {
                PolicyDocumentsSubCategoryMasterDTO outputData = new PolicyDocumentsSubCategoryMasterDTO();
                outputData.PolicyDocumentsSubCategoryId = PolicyDocumentsSubCategoryId;

                IActionResult actionResult;

                actionResult = await _policyDocumentsSubCategoryAPIController.DeletePolicyDocumentsSubCategory(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.PolicyDocumentsSubCategoryId = 0;
                outputData.PolicyDocumentsSubCategoryMasterList = await GetPolicyDocumentsSubCategoryList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("PolicyDocumentsSubCategory", outputData);
                //}
            }
            return RedirectToAction("PolicyDocumentsSubCategory", "PolicyDocumentsSubCategory");
        }
    }
}
