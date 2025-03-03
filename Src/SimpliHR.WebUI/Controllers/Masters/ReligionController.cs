using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class ReligionController : Controller
    {
        private readonly ReligionMasterController _religionAPIController;
        public ReligionController(ReligionMasterController religionAPIController)
        {
            _religionAPIController = religionAPIController;
        }

        public async Task<IActionResult> Religion()
        {
            ReligionMasterDTO outputData = new ReligionMasterDTO();
            outputData.ReligionMasterList = await GetReligionList();
            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<ReligionMasterDTO>?> GetReligionList()
        {

            IActionResult actionResult = await _religionAPIController.GetReligions(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<ReligionMasterDTO> objResultData = (List<ReligionMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.ReligionId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveReligion(ReligionMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Religion", inputData);
            //}
            inputData.IsActive = true;
            IActionResult actionResult;
            ReligionMasterDTO viewModel = new ReligionMasterDTO();
            if (inputData.ReligionId == 0)
                actionResult = _religionAPIController.SaveReligion(inputData);
            else
                actionResult = _religionAPIController.UpdateReligion(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.ReligionId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.ReligionId = 0;
                inputData.ReligionMasterList = await GetReligionList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("Religion", viewModel);

        }


        [HttpGet]
        [Route("Religion/GetReligionInfo/{ereligionId}")]
        public async Task<IActionResult> GetReligionInfo(string ereligionId)
        {

            int religionId = 0;
            try
            {
                religionId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ereligionId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (religionId != 0)
            {
                ReligionMasterDTO outputData = new ReligionMasterDTO();
                outputData.ReligionId = religionId;

                IActionResult actionResult;

                actionResult = await _religionAPIController.GetReligion(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (ReligionMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("Religion", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.ReligionId = 0;
                    objResultData.ReligionMasterList = await GetReligionList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("Religion", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("Religion", "Religion");
        }

        [HttpGet]
        [Route("Religion/DeleteReligion/{ereligionId}")]
        public async Task<IActionResult> DeleteReligion(string ereligionId)
        {

            int religionId = 0;
            try
            {
                religionId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ereligionId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (religionId != 0)
            {
                ReligionMasterDTO outputData = new ReligionMasterDTO();
                outputData.ReligionId = religionId;

                IActionResult actionResult;

                actionResult = await _religionAPIController.DeleteReligion(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.ReligionId = 0;
                outputData.ReligionMasterList = await GetReligionList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("Religion", outputData);
                //}
            }
            return RedirectToAction("Religion", "Religion");
        }
    }
}
