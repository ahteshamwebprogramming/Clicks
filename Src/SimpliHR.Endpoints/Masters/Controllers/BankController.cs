using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Controllers
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
            BankMasterDTO outputData = new BankMasterDTO();
            outputData.BankMasterList = await GetBankList();
            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<BankMasterDTO>?> GetBankList()
        {

            IActionResult actionResult = await _bankAPIController.GetBanks(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<BankMasterDTO> objResultData = (List<BankMasterDTO>)objResult.Value;
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveBank(BankMasterDTO inputData)
        {
            inputData.IsActive = true;
            IActionResult actiionResult;
            BankMasterDTO viewModel = new BankMasterDTO();
            ;
            if (inputData.BankId == 0)
                actiionResult = _bankAPIController.SaveBank(inputData);
            else
                actiionResult = _bankAPIController.UpdateBank(inputData);

            ObjectResult objResult = (ObjectResult)actiionResult;

            var objResultData = objResult.Value;

            if (objResultData != null)
            {
                if (inputData.BankId == 0)
                    viewModel.DisplayMessage = "Bank successfully created";
                else
                    viewModel.DisplayMessage = "Bank updates completed successfully";
                inputData.BankId = 0;
                viewModel.BankMasterList = await GetBankList();
                return View("Bank", viewModel);
            }
            else
            {
                return View();
            }

        }


        [HttpGet]
        [Route("Bank/GetBankInfo/{bankId:int}")]
        public async Task<IActionResult> GetBankInfo(int bankId)
        {
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
                    return View("Bank", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.BankId = 0;
                    objResultData.BankMasterList = await GetBankList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("Bank", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("Bank", "Bank");
        }

        [HttpGet]
        [Route("Bank/DeleteBank/{bankId:int}")]
        public async Task<IActionResult> DeleteBank(int bankId)
        {
            if (bankId != 0)
            {
                BankMasterDTO outputData = new BankMasterDTO();
                outputData.BankId = bankId;

                IActionResult actionResult;

                actionResult = await _bankAPIController.DeleteBank(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.BankId = 0;
                outputData.BankMasterList = await GetBankList();
                outputData.DisplayMessage = "Bank record deactivated successfully";
                return View("Bank", outputData);
                //}
            }
            return RedirectToAction("Bank", "Bank");
        }
    }
}
