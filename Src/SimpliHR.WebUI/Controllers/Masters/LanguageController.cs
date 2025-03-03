using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.WebUI.Controllers.Employee;
using System.Net;
using static SimpliHR.Infrastructure.Models.Masters.LanguageUnitMasterDTO;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class LanguageController : Controller
    {
        private readonly LanguageAPIController _languageAPIController;
        public LanguageController(LanguageAPIController languageAPIController)
        {
            _languageAPIController = languageAPIController;
        }

        public async Task<IActionResult> LanguageMaster()
        {
            LanguageMasterDTO outputData = new LanguageMasterDTO();
            outputData.LanguageMasterList = await GetLanguageMasterList();
            if (outputData != null)
            {
                //foreach (var item in outputData.BankMasterList)
                //{AssignUnitHolidays
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.BankId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> LanguageUnitMaster()
        {
            LanguageUnitMasterDTO outputData = new LanguageUnitMasterDTO();
            outputData.LanguageUnitMasterList = await GetLanguageUnitList();
            if (outputData != null)
            {
                //foreach (var item in outputData.BankMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.BankId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        public async Task<IActionResult> AssignUnitLanguage()
        {

            UnitLanguageListVM outputData = new UnitLanguageListVM();
            outputData.LanguageMasterList = await GetLanguageMasterList();
            outputData.UnitLanguageList = await GetLanguageUnitList();
            int clientId = 0;
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
            {
                outputData.Units = await _languageAPIController.GetClientUnits(clientId);
            }

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        public async Task<List<LanguageUnitMasterDTO>?> GetLanguageUnitList()
        {

            IActionResult actionResult = await _languageAPIController.GetUnitLanguageList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<LanguageUnitMasterDTO> objResultData = (List<LanguageUnitMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.LanguageId.ToString());
            }
            return objResultData;
        }

        public async Task<List<LanguageMasterDTO>?> GetLanguageMasterList()
        {

            IActionResult actionResult = await _languageAPIController.GetLanguageMasterList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<LanguageMasterDTO> objResultData = (List<LanguageMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.LanguageId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveLangaugeMaster(LanguageMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Bank", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            //int? unitId = HttpContext.Session.GetInt32("UnitId");
           // inputData.UnitId = 0;
            inputData.IsActive = true;
            IActionResult actionResult;
            LanguageMasterDTO viewModel = new LanguageMasterDTO();

            if (inputData.LanguageId == 0)
            {
              
                actionResult = _languageAPIController.SaveLanguageMaster(inputData);
            }
            else
            {
               
                actionResult = _languageAPIController.UpdateLanguageMaster(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.LanguageId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.LanguageId = 0;

                //return View("Bank", viewModel);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            inputData.LanguageMasterList = await GetLanguageMasterList();
            viewModel = inputData;
            return View("LanguageMaster", viewModel);

        }


        [HttpPost]
        public async Task<IActionResult> SaveUnitLanguage(LanguageUnitMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Bank", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.LanguageParentId = 0;
            inputData.IsActive = true;
            IActionResult actionResult;
            LanguageUnitMasterDTO viewModel = new LanguageUnitMasterDTO();

            if (inputData.LanguageId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = Convert.ToInt32(employeeId);
                actionResult = _languageAPIController.SaveUnitLanguage(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifiedBy = Convert.ToInt32(employeeId);
                actionResult = _languageAPIController.UpdateUnitLanguage(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.LanguageId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.LanguageId = 0;

                //return View("Bank", viewModel);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            inputData.LanguageUnitMasterList = await GetLanguageUnitList();
            viewModel = inputData;
            return View("LanguageUnitMaster", viewModel);

        }


        [HttpGet]
        [Route("Language/GetUnitLanguageInfo/{eLangId}")]
        public async Task<IActionResult> GetUnitLanguageInfo(string eLangId)
        {

            int languageId = 0;
            try
            {
                languageId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eLangId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (languageId != 0)
            {
                LanguageUnitMasterDTO outputData = new LanguageUnitMasterDTO();
                outputData.LanguageId = languageId;

                IActionResult actionResult;

                actionResult = await _languageAPIController.GetUnitLanguageMaster(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (LanguageUnitMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("LanguageUnitMaster", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.LanguageId = 0;
                    objResultData.LanguageUnitMasterList = await GetLanguageUnitList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("LanguageUnitMaster", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("LanguageUnitMaster", "Language");

        }

        [HttpGet]
        [Route("Language/GetLanguageMasterInfo/{eLangId}")]
        public async Task<IActionResult> GetLanguageMasterInfo(string eLangId)
        {

            int languageId = 0;
            try
            {
                languageId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eLangId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (languageId != 0)
            {
                LanguageMasterDTO outputData = new LanguageMasterDTO();
                outputData.LanguageId = languageId;

                IActionResult actionResult;

                actionResult = await _languageAPIController.GetLanguageMaster(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (LanguageMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("LanguageMaster", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.LanguageId = 0;
                    objResultData.LanguageMasterList = await GetLanguageMasterList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("LanguageMaster", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("LanguageMaster", "Language");

        }

        [HttpGet]
        [Route("Language/DeleteUnitLanguage/{eLangId}")]
        public async Task<IActionResult> DeleteUnitLanguage(string eLangId)
        {

            int LangId = 0;
            try
            {
                LangId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eLangId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (LangId != 0)
            {
                LanguageUnitMasterDTO outputData = new LanguageUnitMasterDTO();
                outputData.LanguageId = LangId;

                IActionResult actionResult;

                actionResult = await _languageAPIController.DeleteUnitLanguage(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.LanguageId = 0;
                outputData.LanguageUnitMasterList = await GetLanguageUnitList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("LanguageUnitMaster", outputData);
                //}
            }
            return RedirectToAction("LanguageUnitMaster", "Language");

        }

        [HttpGet]
        [Route("Language/DeleteLanguageMaster/{eLangId}")]
        public async Task<IActionResult> DeleteLanguageMaster(string eLangId)
        {

            int LangId = 0;
            try
            {
                LangId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eLangId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (LangId != 0)
            {
                LanguageMasterDTO outputData = new LanguageMasterDTO();
                outputData.LanguageId = LangId;

                IActionResult actionResult;

                actionResult = await _languageAPIController.DeleteLanguageMaster(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.LanguageId = 0;
                outputData.LanguageMasterList = await GetLanguageMasterList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("LanguageMaster", outputData);
                //}
            }
            return RedirectToAction("LanguageMaster", "Language");

        }


        [HttpPost]
        public async Task<UnitLanguageListVM> SaveUnitLanguageFromMaster(UnitLanguageListVM unitLangVM)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            IActionResult actionResult;
            actionResult = _languageAPIController.SaveUnitLanguageFromMaster(unitLangVM, employeeId);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;
            unitLangVM.DisplayMessage = "Success";
            return unitLangVM;
        }
    }
}
