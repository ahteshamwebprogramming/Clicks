using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class JobTitleController : Controller
    {
        private readonly JobTitleMasterController _JobTitleAPIController;
        public JobTitleController(JobTitleMasterController jobTitleApiController)
        {
            _JobTitleAPIController = jobTitleApiController;
        }
        public async Task<IActionResult> JobTitle()
        {
            JobTitleMasterDTO outputData = new JobTitleMasterDTO();
            outputData.JobTitleMasterList = await GetJobTitleList();


            if (outputData != null)
            {
                //foreach (var item in outputData.JobTitleMasterList)
                //{
                //    item.EncryptedJobTitleId = CommonHelper.EncryptURLHTML(item.JobTitleId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }

        }

        public async Task<List<JobTitleMasterDTO>?> GetJobTitleList()
        {

            IActionResult actionResult = await _JobTitleAPIController.GetJobTitles(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<JobTitleMasterDTO> objResultData = (List<JobTitleMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedJobTitleId = CommonHelper.EncryptURLHTML(item.JobTitleId.ToString());
            }
            return objResultData;
        }


        [HttpGet]
        [Route("JobTitle/GetJobTitleInfo/{eJobTitleId}")]
        public async Task<IActionResult> GetJobTitleInfo(string eJobTitleId)
        {

            int JobTitleId = 0;
            try
            {
                JobTitleId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eJobTitleId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (JobTitleId != 0)
            {
                JobTitleMasterDTO outputData = new JobTitleMasterDTO();
                outputData.JobTitleId = JobTitleId;

                IActionResult actionResult;

                actionResult = await _JobTitleAPIController.GetJobTitle(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (JobTitleMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("JobTitle", objResultData);
                    //return RedirectToAction("JobTitle","JobTitle", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.JobTitleId = 0;
                    objResultData.JobTitleMasterList = await GetJobTitleList();
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    //return View("JobTitle",objResultData);
                    //return RedirectToAction("JobTitle", objResultData);
                }
            }
            return RedirectToAction("JobTitle", "JobTitle");

        }


        [HttpGet]
        [Route("JobTitle/DeleteJobTitle/{eJobTitleId}")]
        public async Task<IActionResult> DeleteJobTitle(string eJobTitleId)
        {
            int JobTitleId = 0;
            try
            {
                JobTitleId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eJobTitleId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (JobTitleId != 0)
            {
                JobTitleMasterDTO outputData = new JobTitleMasterDTO();
                outputData.JobTitleId = JobTitleId;

                IActionResult actionResult;

                actionResult = await _JobTitleAPIController.DeleteJobTitle(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.JobTitleId = 0;
                outputData.JobTitleMasterList = await GetJobTitleList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("JobTitle", outputData);
                //}
            }
            return RedirectToAction("JobTitle", "JobTitle");

        }


        public async Task<IActionResult> SaveJobTitle(JobTitleMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("JobTitle", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            JobTitleMasterDTO viewModel = new JobTitleMasterDTO();
            if (inputData.JobTitleId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
                actionResult = _JobTitleAPIController.SaveJobTitle(inputData);
            }
            else
            {

                inputData.ModifedBy = employeeId;
                actionResult = _JobTitleAPIController.UpdateJobTitle(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.JobTitleId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.JobTitleId = 0;
                inputData.JobTitleMasterList = await GetJobTitleList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            viewModel = inputData;
            return View("JobTitle", viewModel);

        }

    }
}
