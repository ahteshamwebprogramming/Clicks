using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class LeaveTypeController : Controller
    {
        private readonly LeaveTypeMasterController _leaveTypeAPIController;
        public LeaveTypeController(LeaveTypeMasterController leaveTypeAPIController)
        {
            _leaveTypeAPIController = leaveTypeAPIController;
        }

        public async Task<IActionResult> LeaveType()
        {

            LeaveTypeMasterDTO outputData = new LeaveTypeMasterDTO();

            if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                outputData.UnitId = 0;
            else
                outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
            outputData.LeaveTypeMasterList = await GetLeaveTypeList(outputData.UnitId);
            if (outputData != null)
            {
                //foreach (var item in outputData.LeaveTypeMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.LeaveTypeId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<LeaveTypeMasterDTO>?> GetLeaveTypeList(int? UnitId)
        {

            IActionResult actionResult = await _leaveTypeAPIController.GetLeaveTypes(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, UnitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<LeaveTypeMasterDTO> objResultData = (List<LeaveTypeMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.LeaveTypeId.ToString());
            }
            return objResultData;
        }

        [HttpPost]
        public async Task<IActionResult> SaveLeaveType(LeaveTypeMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("LeaveType", inputData);
            //}
            inputData.IsActive = true;
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                inputData.UnitId = 0;
            else
                inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult;
            LeaveTypeMasterDTO viewModel = new LeaveTypeMasterDTO();
            if (inputData.LeaveTypeId == 0)
                actionResult = _leaveTypeAPIController.SaveLeaveType(inputData);
            else
                actionResult = _leaveTypeAPIController.UpdateLeaveType(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.LeaveTypeId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.LeaveTypeId = 0;
                inputData.LeaveTypeMasterList = await GetLeaveTypeList(inputData.UnitId);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("LeaveType", viewModel);

        }


        [HttpGet]
        [Route("LeaveType/GetLeaveTypeInfo/{eleaveTypeId}")]
        public async Task<IActionResult> GetLeaveTypeInfo(string eleaveTypeId)
        {
            int leaveTypeId = 0;
            try
            {
                leaveTypeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eleaveTypeId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (leaveTypeId != 0)
            {
                LeaveTypeMasterDTO outputData = new LeaveTypeMasterDTO();
                outputData.LeaveTypeId = leaveTypeId;

                if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                    outputData.UnitId = 0;
                else
                    outputData.UnitId = HttpContext.Session.GetInt32("UnitId");

                IActionResult actionResult;

                actionResult = await _leaveTypeAPIController.GetLeaveType(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (LeaveTypeMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("LeaveType", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.LeaveTypeId = 0;
                    objResultData.LeaveTypeMasterList = await GetLeaveTypeList(outputData.UnitId);
                    objResultData.DisplayMessage = "You cannot edit locked leave. Contact Admin for further details";
                    return View("LeaveType", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("LeaveType", "LeaveType");
        }

        [HttpGet]
        [Route("LeaveType/DeleteLeaveType/{eleaveTypeId}")]
        public async Task<IActionResult> DeleteLeaveType(string eleaveTypeId)
        {
            int leaveTypeId = 0;
            try
            {
                leaveTypeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eleaveTypeId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (leaveTypeId != 0)
            {


                LeaveTypeMasterDTO outputData = new LeaveTypeMasterDTO();

                if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                    outputData.UnitId = 0;
                else
                    outputData.UnitId = HttpContext.Session.GetInt32("UnitId");

                outputData.LeaveTypeId = leaveTypeId;

                IActionResult actionResult;

                actionResult = await _leaveTypeAPIController.DeleteLeaveType(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.LeaveTypeId = 0;
                outputData.LeaveTypeMasterList = await GetLeaveTypeList(outputData.UnitId);
                outputData.DisplayMessage = "Transaction Successful!";
                return View("LeaveType", outputData);
                //}
            }
            return RedirectToAction("LeaveType", "LeaveType");
        }
    }
}
