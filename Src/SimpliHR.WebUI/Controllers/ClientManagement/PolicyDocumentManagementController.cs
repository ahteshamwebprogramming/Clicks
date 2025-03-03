using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Helper;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.Login;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValues;
using System.IO;
using System.Linq.Expressions;
using H = Microsoft.AspNetCore.Hosting;

namespace SimpliHR.WebUI.Controllers.ClientManagement
{
    public class PolicyDocumentManagementController : Controller
    {
        private readonly PolicyDocumentController _policyDocumentAPIController;
        private readonly H.IHostingEnvironment hostingEnvironment;
        public PolicyDocumentManagementController(PolicyDocumentController policyDocumentAPIController, H.IHostingEnvironment environment)
        {
            _policyDocumentAPIController = policyDocumentAPIController;
            hostingEnvironment = environment;
        }
        public async Task<IActionResult> PolicyDocumentManagement()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            PolicyDocumentsMasterDTO policy = new PolicyDocumentsMasterDTO();
            policy.PolicyDocumentCategoryKeyValues = await _policyDocumentAPIController.PolicyDocumentCategoryKeyValues(true, unitId);
            return View(policy);
        }
        public async Task<IActionResult> PolicyDocumentManagementSaved()
        {
            ModelState.AddModelError("", "Transaction Successful!");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            PolicyDocumentsMasterDTO policy = new PolicyDocumentsMasterDTO();
            policy.PolicyDocumentCategoryKeyValues = await _policyDocumentAPIController.PolicyDocumentCategoryKeyValues(true, unitId);
            return View("PolicyDocumentManagement", policy);
        }

        [HttpGet]
        public async Task<List<PolicyDocumentSubCategoryKeyValues>>? GetSubCategories(int categoryId)
        {
            return await _policyDocumentAPIController.PolicyDocumentSubCategoryKeyValues(true, categoryId);
        }
        [HttpPost]
        public async Task<ActionResult> SavePolicyDocuments(PolicyDocumentsMasterDTO inputData)
        {
            try
            {
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                int clientId;
                if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
                {
                    inputData.PolicyDocumentCategoryKeyValues = await _policyDocumentAPIController.PolicyDocumentCategoryKeyValues(true, unitId);
                    if (inputData.PolicyDocumentFile != null)
                    {
                        var file = inputData.PolicyDocumentFile;
                        string path = "";
                        string repPath = Path.Combine("PolicyDocuments", clientId.ToString(), unitId.ToString());
                        inputData.IsActive = true;
                        inputData.PolicyDocument = file.FileName;
                        inputData.ClientId = clientId;
                        inputData.UnitId = unitId ?? default(int);

                        if (_policyDocumentAPIController.PolicyDocumentExists(inputData))
                        {
                            throw new Exception("Duplicate Entry");
                        }

                        if (file.Length > 0)
                        {
                            path = Path.GetFullPath(Path.Combine(hostingEnvironment.WebRootPath, repPath));
                            var p = Path.GetPathRoot(path);

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string filename = inputData.PolicyDocumentsCategoryId + "_" + inputData.PolicyDocumentsSubCategoryId + "_" + file.FileName;
                            using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                            inputData.PolicyDocumentPath = Path.Combine(repPath, filename);
                            IActionResult actionResult = _policyDocumentAPIController.SavePolicyDocumentDetails(inputData);
                            ObjectResult objResult = (ObjectResult)actionResult;
                            if (objResult.StatusCode == 200)
                            {
                                ModelState.AddModelError("", "Transaction Successful!");
                                inputData.AcceptanceRequired = false;
                                inputData.PolicyDocumentsCategoryId = 0;
                                inputData.PolicyDocumentsSubCategoryId = 0;
                                inputData.Description = "";

                                PolicyDocumentsMasterDTO dto = new PolicyDocumentsMasterDTO();
                                dto.PolicyDocumentCategoryKeyValues = await _policyDocumentAPIController.PolicyDocumentCategoryKeyValues(true, unitId);
                                //return View("PolicyDocumentManagement", dto);
                                return RedirectToAction("PolicyDocumentManagementSaved", "PolicyDocumentManagement");
                            }
                            else
                            {
                                throw new Exception(objResult.Value.ToString());
                            }
                        }
                        else
                        {
                            throw new Exception("Please add files");
                        }
                    }
                    else
                    {
                        throw new Exception("Please add files");
                    }
                }
                {
                    throw new Exception("Seems like your session has expired. Please re login and try again");
                }
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Validation Failed", ex.Message);
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                inputData.PolicyDocumentCategoryKeyValues = await _policyDocumentAPIController.PolicyDocumentCategoryKeyValues(true, unitId);
                return View("PolicyDocumentManagement", inputData);
            }
        }


        public async Task<ActionResult> ListOfPolicies()
        {
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {
                Core.Helper.RequestParams parm = new Core.Helper.RequestParams();
                parm.PageSize = 10;
                parm.PageNumber = 1;
                IActionResult actionResult = await _policyDocumentAPIController.GetPolicyDocuments(parm, clientId, unitId);
                ObjectResult objResult = (ObjectResult)actionResult;
                List<PolicyDocumentsMasterDTO> objResultData = (List<PolicyDocumentsMasterDTO>)objResult.Value;
                foreach (var item in objResultData)
                {
                    item.EncryptedId = CommonHelper.EncryptURLHTML(item.PolicyDocumentsMasterId.ToString());
                }
                return View(objResultData);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }



        }
        [HttpGet]
        [Route("PolicyDocumentManagement/DeletePolicyDocument/{eId}")]
        public async Task<IActionResult> DeletePolicyDocument(string eId)
        {
            int policyDocumentId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (policyDocumentId != 0)
            {
                PolicyDocumentsMasterDTO outputData = new PolicyDocumentsMasterDTO();
                outputData.PolicyDocumentsMasterId = policyDocumentId;

                IActionResult actionResult;

                actionResult = await _policyDocumentAPIController.DeletePolicyDocument(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{



                //IActionResult actionResult1 = await _policyDocumentAPIController.GetPolicyDocuments(parm, clientId);
                //ObjectResult objResult1 = (ObjectResult)actionResult1;
                //List<PolicyDocumentsMasterDTO> objResultData1 = (List<PolicyDocumentsMasterDTO>)objResult1.Value;
                //foreach (var item in objResultData1)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.PolicyDocumentsMasterId.ToString());
                //}
                ////outputData.DisplayMessage = "Academic record deactivated successfully";
                //return View("ListOfPolicies", objResultData1);
                ////}
            }
            return RedirectToAction("ListOfPolicies", "PolicyDocumentManagement");
        }

    }
}
