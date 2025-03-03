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
using DocumentFormat.OpenXml.Spreadsheet;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class ShiftController : Controller
    {

        private readonly ShiftMasterController _ShiftAPIController;
        public ShiftController(ShiftMasterController ShiftAPIController)
        {
            _ShiftAPIController = ShiftAPIController;
        }

        public async Task<IActionResult> Shift()
        {
            ShiftMasterDTO outputData = new ShiftMasterDTO();
            outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
            outputData.ShiftMasterList = await GetShiftList(outputData.UnitId);


            if (outputData != null)
            {
                //foreach (var item in outputData.ShiftMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.ShiftId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }
        public async Task<List<ShiftMasterDTO>?> GetShiftList(int? UnitId)
        {

            IActionResult actionResult = await _ShiftAPIController.GetShifts(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, UnitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<ShiftMasterDTO> objResultData = (List<ShiftMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.ShiftId.ToString());
            }
            return objResultData;
        }

        [HttpGet]
        [Route("Shift/GetShiftInfo/{eShiftId}")]
        public async Task<IActionResult> GetShiftInfo(string eShiftId)
        {
            int ShiftId = 0;
            try
            {
                ShiftId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eShiftId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ShiftId != 0)
            {
                ShiftMasterDTO outputData = new ShiftMasterDTO();
                outputData.ShiftId = ShiftId;
                outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
                IActionResult actionResult;

                actionResult = await _ShiftAPIController.GetShift(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (ShiftMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("Shift", objResultData);
                    //return RedirectToAction("Shift","Shift", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.ShiftId = 0;
                    objResultData.ShiftMasterList = await GetShiftList(outputData.UnitId);
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    //return View("Shift",objResultData);
                    //return RedirectToAction("Shift", objResultData);
                }
            }
            return RedirectToAction("Shift", "Shift");
        }

        [HttpGet]
        [Route("Shift/DeleteShift/{eShiftId}")]
        public async Task<IActionResult> DeleteShift(string eShiftId)
        {
            int ShiftId = 0;
            try
            {
                ShiftId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eShiftId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ShiftId != 0)
            {
                ShiftMasterDTO outputData = new ShiftMasterDTO();
                outputData.ShiftId = ShiftId;
                outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
                IActionResult actionResult;

                actionResult = await _ShiftAPIController.DeleteShift(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.ShiftId = 0;
                outputData.ShiftMasterList = await GetShiftList(outputData.UnitId);
                outputData.DisplayMessage = "Shift deactivated successfully";
                return View("Shift", outputData);
                //}
            }
            return RedirectToAction("Shift", "Shift");
        }

        [HttpPost]
        public async Task<IActionResult> SaveShift(ShiftMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Shift", inputData);
            //}
            inputData.IsActive = true;
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                inputData.UnitId = 0;
            else
                inputData.UnitId = HttpContext.Session.GetInt32("UnitId");

            IActionResult actionResult;
            ShiftMasterDTO viewModel = new ShiftMasterDTO();
            if (inputData.ShiftId == 0)
                actionResult = _ShiftAPIController.SaveShift(inputData);
            else
                actionResult = _ShiftAPIController.UpdateShift(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.ShiftId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.ShiftId = 0;
                inputData.ShiftMasterList = await GetShiftList(inputData.UnitId);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("Shift", viewModel);

        }
    }
}
