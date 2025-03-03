using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.WebUI.Modals.Masters;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class CountryController : Controller
    {
        private readonly CountryMasterController _countryAPIController;
        public CountryController(CountryMasterController countryAPIController)
        {
            _countryAPIController = countryAPIController;
        }


        public async Task<IActionResult> Country()
        {

            CountryMasterDTO outputData = new CountryMasterDTO();
            outputData.CountryMasterList = await GetCountryList();

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<CountryMasterDTO>?> GetCountryList()
        {

            IActionResult actionResult = await _countryAPIController.GetCountries(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<CountryMasterDTO> objResultData = (List<CountryMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedCountryId = CommonHelper.Encrypt(Convert.ToString(item.CountryId));
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveCountry(CountryMasterDTO inputData)
        {

            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("Country", inputData);
            //}
            CountryMasterDTO viewModel = new CountryMasterDTO();
            try
            {
                inputData.IsActive = true;
                IActionResult actionResult;
                
                if (inputData.CountryId == 0)
                    //throw new Exception("Testing the error message");
                    actionResult = _countryAPIController.SaveCountry(inputData);
                else
                    actionResult = _countryAPIController.UpdateCountry(inputData);

                ObjectResult objResult = (ObjectResult)actionResult;

                var objResultData = objResult.Value;
                inputData.HttpStatusCode = objResult.StatusCode;

                if (inputData.HttpStatusCode == 200)
                {
                    if (inputData.CountryId == 0)
                        inputData.DisplayMessage = "Transaction Successful!";
                    else
                        inputData.DisplayMessage = "Transaction Successful!";
                    inputData.CountryId = 0;
                    inputData.CountryMasterList = await GetCountryList();
                    
                }
                else
                    inputData.DisplayMessage = objResultData.ToString();
                viewModel = inputData;
                return View("Country", viewModel);
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", "Invalid Username or Password");
                return View("Country", viewModel);
            }
        }


        [HttpGet]
        [Route("Country/GetCountryInfo/{ecountryId}")]
        public async Task<IActionResult> GetCountryInfo(string ecountryId)
        {
            int countryId = 0;
            try
            {
                countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (countryId != 0)
                {
                    CountryMasterDTO outputData = new CountryMasterDTO();
                    outputData.CountryId = countryId;

                    IActionResult actionResult;

                    actionResult = await _countryAPIController.GetCountry(outputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    var objResultData = (CountryMasterDTO)objResult.Value;
                    if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return View("Country", objResultData);
                        //return RedirectToAction("Role","Role", objResultData);
                    }
                    else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                    {
                        objResultData.CountryId = 0;
                        objResultData.CountryMasterList = await GetCountryList();
                        objResultData.DisplayMessage = "You cannot edit locked country. Contact Admin for further details";
                        return View("Country", objResultData);
                        //return RedirectToAction("Role", objResultData);
                    }
                }
                return RedirectToAction("Country", "Country");
           
        }

        [HttpGet]
        [Route("Country/DeleteCountry/{ecountryId}")]
        public async Task<IActionResult> DeleteCountry(string ecountryId)
        {
            int countryId = 0;
            try
            {
                countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
         
                if (countryId != 0)
                {

                    CountryMasterDTO outputData = new CountryMasterDTO();
                    outputData.CountryId = countryId;

                    IActionResult actionResult;

                    actionResult = await _countryAPIController.DeleteCountry(outputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    var objResultData = objResult.Value;

                    //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                    //{
                    outputData.CountryId = 0;
                    outputData.CountryMasterList = await GetCountryList();
                    outputData.DisplayMessage = "Transaction Successful!";
                    return View("Country", outputData);
                    //}
                }
                return RedirectToAction("Country", "Country");
            }
            
    }
}
