using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.WebUI.Controllers.Employee;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class BankController : Controller
    {
        private readonly BankMasterController _bankAPIController;
        public BankController(BankMasterController bankAPIController)
        {
            _bankAPIController = bankAPIController;
        }

        public async Task<IActionResult> Bank()
        {
            BankUnitMasterDTO outputData = new BankUnitMasterDTO();
            outputData.BankMasterList = await GetBankList();
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

        public async Task<IActionResult> BankMaster()
        {
            BankMasterDTO outputData = new BankMasterDTO();
            outputData.BankMasterList = await GetBankMasterList();
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
        public async Task<IActionResult> AssignUnitBanks()
        {

            UnitBankListVM outputData = new UnitBankListVM();
            outputData.BankMasterList = await GetBankMasterList();
            outputData.UnitBankList = await GetBankList();
            int clientId = 0;
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
            {
                outputData.Units = await _bankAPIController.GetClientUnits(clientId);
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
        public async Task<List<BankUnitMasterDTO>?> GetBankList()
        {

            IActionResult actionResult = await _bankAPIController.GetUnitBanks(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<BankUnitMasterDTO> objResultData = (List<BankUnitMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.BankId.ToString());
            }
            return objResultData;
        }

        public async Task<List<BankMasterDTO>?> GetBankMasterList()
        {

            IActionResult actionResult = await _bankAPIController.GetBankMaster(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<BankMasterDTO> objResultData = (List<BankMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.BankId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveBank(BankMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Bank", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            //int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = 0;
            inputData.IsActive = true;
            IActionResult actionResult;
            BankMasterDTO viewModel = new BankMasterDTO();

            if (inputData.BankId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
                actionResult = _bankAPIController.SaveBankMaster(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifedBy = employeeId;
                actionResult = _bankAPIController.UpdateBankMaster(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.BankId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.BankId = 0;

                //return View("Bank", viewModel);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            inputData.BankMasterList = await GetBankMasterList();
            viewModel = inputData;
            return View("BankMaster", viewModel);

        }


        [HttpPost]
        public async Task<IActionResult> SaveUnitBank(BankUnitMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Bank", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            BankUnitMasterDTO viewModel = new BankUnitMasterDTO();

            if (inputData.BankId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = Convert.ToInt32(employeeId);
                actionResult = _bankAPIController.SaveUnitBank(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifedBy = Convert.ToInt32(employeeId);
                actionResult = _bankAPIController.UpdateBank(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.BankId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.BankId = 0;

                //return View("Bank", viewModel);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            inputData.BankMasterList = await GetBankList();
            viewModel = inputData;
            return View("Bank", viewModel);

        }


        [HttpGet]
        [Route("Bank/GetBankInfo/{ebankId}")]
        public async Task<IActionResult> GetBankInfo(string ebankId)
        {

            int bankId = 0;
            try
            {
                bankId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ebankId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (bankId != 0)
            {
                BankUnitMasterDTO outputData = new BankUnitMasterDTO();
                outputData.BankId = bankId;

                IActionResult actionResult;

                actionResult = await _bankAPIController.GetUnitBank(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (BankUnitMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    objResultData.BankMasterList = await GetBankList();
                    return View("Bank", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.BankId = 0;
                    objResultData.BankMasterList = await GetBankList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    objResultData.BankMasterList = await GetBankList();
                    return View("Bank", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("Bank", "Bank");

        }

        [HttpGet]
        [Route("Bank/GetBankMasterInfo/{ebankId}")]
        public async Task<IActionResult> GetBankMasterInfo(string ebankId)
        {

            int bankId = 0;
            try
            {
                bankId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ebankId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (bankId != 0)
            {
                BankMasterDTO outputData = new BankMasterDTO();
                outputData.BankId = bankId;

                IActionResult actionResult;

                actionResult = await _bankAPIController.GetBank(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (BankMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    objResultData.BankMasterList = await GetBankMasterList();

                    return View("BankMaster", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.BankMasterList = await GetBankMasterList();
                    objResultData.BankId = 0;
                    objResultData.BankMasterList = await GetBankMasterList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("BankMaster", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("BankMaster", "Bank");

        }

        [HttpGet]
        [Route("Bank/DeleteBank/{ebankId}")]
        public async Task<IActionResult> DeleteBank(string ebankId)
        {

            int bankId = 0;
            try
            {
                bankId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ebankId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (bankId != 0)
            {
                BankUnitMasterDTO outputData = new BankUnitMasterDTO();
                outputData.BankId = bankId;

                IActionResult actionResult;

                actionResult = await _bankAPIController.DeleteBank(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.BankId = 0;
                outputData.BankMasterList = await GetBankList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("Bank", outputData);
                //}
            }
            return RedirectToAction("Bank", "Bank");

        }

        [HttpGet]
        [Route("Bank/DeleteBankMaster/{ebankId}")]
        public async Task<IActionResult> DeleteBankMaster(string ebankId)
        {

            int bankId = 0;
            try
            {
                bankId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ebankId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (bankId != 0)
            {
                BankMasterDTO outputData = new BankMasterDTO();
                outputData.BankId = bankId;

                IActionResult actionResult;

                actionResult = await _bankAPIController.DeleteBankMaster(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.BankId = 0;
                outputData.BankMasterList = await GetBankMasterList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("BankMaster", outputData);
                //}
            }
            return RedirectToAction("BankMaster", "Bank");

        }


        [HttpPost]
        public async Task<UnitBankListVM> SaveUnitBankFromMaster(UnitBankListVM unitBankVM)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            IActionResult actionResult;
            actionResult = _bankAPIController.SaveUnitBankFromMaster(unitBankVM, employeeId);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;
            unitBankVM.DisplayMessage = "Success";
            return unitBankVM;
        }
    }
}
