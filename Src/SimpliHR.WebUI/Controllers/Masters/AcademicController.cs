using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using System.Web;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class AcademicController : Controller
    {
        private readonly AcademicMasterController _academicAPIController;
        public AcademicController(AcademicMasterController academicAPIController)
        {
            _academicAPIController = academicAPIController;
        }

        public async Task<IActionResult> Academic()
        {
            AcademicMasterDTO outputData = new AcademicMasterDTO();
            IActionResult actionResult = await _academicAPIController.GetAcademics(new Core.Helper.RequestParams { PageNumber = 1, PageSize = 100 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;
            outputData.AcademicMasterList = (List<AcademicMasterDTO>)objResult.Value;

            if (outputData != null)
            {
                foreach (var item in outputData.AcademicMasterList)
                {
                    item.EncryptedAcademicId = CommonHelper.EncryptURLHTML(item.AcademicId.ToString());
                }
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<AcademicMasterDTO>?> GetAcademicList()
        {

            IActionResult actionResult = await _academicAPIController.GetAcademics(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<AcademicMasterDTO> objResultData = (List<AcademicMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedAcademicId = CommonHelper.EncryptURLHTML(item.AcademicId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveAcademic(AcademicMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Academic", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            AcademicMasterDTO viewModel = new AcademicMasterDTO();
            if (inputData.AcademicId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
                actionResult = _academicAPIController.SaveAcademic(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifedBy = employeeId;
                actionResult = _academicAPIController.UpdateAcademic(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;


            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.AcademicId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.AcademicId = 0;
                inputData.AcademicMasterList = await GetAcademicList();

            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            viewModel = inputData;
            return View("Academic", viewModel);

        }


        [HttpGet]
        [Route("Academic/GetAcademicInfo/{eacademicId}")]
        public async Task<IActionResult> GetAcademicInfo(string eacademicId)
        {
            int academicId = 0;
            try
            {
                academicId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eacademicId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (academicId != 0)
            {
                AcademicMasterDTO outputData = new AcademicMasterDTO();
                outputData.AcademicId = academicId;

                IActionResult actionResult;

                actionResult = await _academicAPIController.GetAcademic(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (AcademicMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("Academic", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.AcademicId = 0;
                    objResultData.AcademicMasterList = await GetAcademicList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    return View("Academic", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("Academic", "Academic");
        }



        [HttpGet]
        [Route("Academic/DeleteAcademic/{eacademicId}")]
        public async Task<IActionResult> DeleteAcademic(string eacademicId)
        {

            int academicId = 0;
            try
            {
                academicId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eacademicId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (academicId != 0)
            {
                AcademicMasterDTO outputData = new AcademicMasterDTO();
                outputData.AcademicId = academicId;

                IActionResult actionResult;

                actionResult = await _academicAPIController.DeleteAcademic(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.AcademicId = 0;
                outputData.AcademicMasterList = await GetAcademicList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("Academic", outputData);
                //}
            }
            return RedirectToAction("Academic", "Academic");

        }


        public static bool DetectInvalidChars(string s)
        {
            const string specialChars = "\r\n\t .,;:-_!\"'?()[]{}&%$§=*+~#@|<>äöüÄÖÜß/\\^€";
            return s.Any(ch => !(
                specialChars.Contains(ch) ||
                (ch >= '0' && ch <= '9') ||
                (ch >= 'a' && ch <= 'z') ||
                (ch >= 'A' && ch <= 'Z')));
        }
    }
}
