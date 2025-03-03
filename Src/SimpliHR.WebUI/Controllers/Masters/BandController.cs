using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using SimpliHR.Infrastructure.Helper;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class BandController : Controller
    {
        private readonly BandMasterController _bandAPIController;
        public BandController(BandMasterController bandAPIController)
        {
            _bandAPIController = bandAPIController;
        }

        public async Task<IActionResult> Band()
        {
            BandMasterDTO outputData = new BandMasterDTO();
            outputData.BandMasterList = await GetBandList();
            if (outputData != null)
            {
                //foreach (var item in outputData.BandMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.BandId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<BandMasterDTO>?> GetBandList()
        {

            IActionResult actionResult = await _bandAPIController.GetBands(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<BandMasterDTO> objResultData = (List<BandMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.BandId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveBand(BandMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Band", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            BandMasterDTO viewModel = new BandMasterDTO();
            ;
            if (inputData.BandId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
                actionResult = _bandAPIController.SaveBand(inputData);
            }                
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifedBy = employeeId;
                actionResult = _bandAPIController.UpdateBand(inputData);
            }
               

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;


            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.BandId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.BandId = 0;
                //viewModel.BandMasterList = await GetBandList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            inputData.BandMasterList = await GetBandList();
            viewModel = inputData;
            return View("Band", viewModel);
        }


        [HttpGet]
        [Route("Band/GetBandInfo/{ebandId}")]
        public async Task<IActionResult> GetBandInfo(string ebandId)
        {

            int bandId = 0;
            try
            {
                bandId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ebandId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (bandId != 0)
                {
                    BandMasterDTO outputData = new BandMasterDTO();
                    outputData.BandId = bandId;

                    IActionResult actionResult;

                    actionResult = await _bandAPIController.GetBand(outputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    var objResultData = (BandMasterDTO)objResult.Value;
                    if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return View("Band", objResultData);
                        //return RedirectToAction("Role","Role", objResultData);
                    }
                    else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                    {
                        objResultData.BandId = 0;
                        objResultData.BandMasterList = await GetBandList();
                        objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                        return View("Band", objResultData);
                        //return RedirectToAction("Role", objResultData);
                    }
                }
                return RedirectToAction("Band", "Band");
           
        }

        [HttpGet]
        [Route("Band/DeleteBand/{ebandId}")]
        public async Task<IActionResult> DeleteBand(string ebandId)
        {
            int bandId = 0;
            try
            {
                bandId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ebandId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (bandId != 0)
                {
                    BandMasterDTO outputData = new BandMasterDTO();
                    outputData.BandId = bandId;

                    IActionResult actionResult;

                    actionResult = await _bandAPIController.DeleteBand(outputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    var objResultData = objResult.Value;

                    //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                    //{
                    outputData.BandId = 0;
                    outputData.BandMasterList = await GetBandList();
                    outputData.DisplayMessage = "Transaction Successful!";
                    return View("Band", outputData);
                    //}
                }
                return RedirectToAction("Band", "Band");
           
        }
    }
}
