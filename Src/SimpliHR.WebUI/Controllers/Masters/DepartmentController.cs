using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class DepartmentController : Controller
    {
        private readonly DepartmentMasterController _departmentAPIController;
        public DepartmentController(DepartmentMasterController departmentAPIController)
        {
            _departmentAPIController = departmentAPIController;
        }

        public async Task<IActionResult> Department()
        {
            DepartmentMasterDTO outputData = new DepartmentMasterDTO();
            outputData.DepartmentMasterList = await GetDepartmentList();

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<DepartmentMasterDTO>?> GetDepartmentList()
        {

            IActionResult actionResult = await _departmentAPIController.GetDepartments(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<DepartmentMasterDTO> objResultData = (List<DepartmentMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedDepartmentId = CommonHelper.EncryptURLHTML(item.DepartmentId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveDepartment(DepartmentMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Department", inputData);
            //}
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.IsActive = true;
            inputData.UnitId = unitId;
            IActionResult actionResult;
            DepartmentMasterDTO viewModel = new DepartmentMasterDTO();
            ;
            if (inputData.DepartmentId == 0)
                actionResult = _departmentAPIController.SaveDepartment(inputData);
            else
                actionResult = _departmentAPIController.UpdateDepartment(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.DepartmentId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.DepartmentId = 0;
                inputData.DepartmentMasterList = await GetDepartmentList();

            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("Department", viewModel);
        }


        [HttpGet]
        [Route("Department/GetDepartmentInfo/{edepartmentId}")]

        public async Task<IActionResult> GetDepartmentInfo(string edepartmentId)
        {

            int departmentId = 0;
            try
            {
                departmentId = Convert.ToInt32(CommonHelper.DecryptURLHTML(edepartmentId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (departmentId != 0)
            {
                DepartmentMasterDTO outputData = new DepartmentMasterDTO();
                outputData.DepartmentId = departmentId;

                IActionResult actionResult;

                actionResult = await _departmentAPIController.GetDepartment(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (DepartmentMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("Department", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.DepartmentId = 0;
                    objResultData.DepartmentMasterList = await GetDepartmentList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("Department", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("Department", "Department");

        }

        [HttpGet]
        [Route("Department/DeleteDepartment/{edepartmentId}")]
        public async Task<IActionResult> DeleteDepartment(string edepartmentId)
        {
            int departmentId = 0;
            try
            {
                departmentId = Convert.ToInt32(CommonHelper.DecryptURLHTML(edepartmentId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (departmentId != 0)
            {
                DepartmentMasterDTO outputData = new DepartmentMasterDTO();
                outputData.DepartmentId = departmentId;

                IActionResult actionResult;

                actionResult = await _departmentAPIController.DeleteDepartment(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.DepartmentId = 0;
                outputData.DepartmentMasterList = await GetDepartmentList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("Department", outputData);
                //}
            }
            return RedirectToAction("Department", "Department");

        }
    }
}
