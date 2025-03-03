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
using AutoMapper;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Services.DBContext;
using SimpliHR.Core.Helper;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.WebUI.BL;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class CityController : Controller
    {

        private readonly CityMasterController _cityAPIController;
        private readonly MastersKeyValueController _mastersKeyValueController;
        public CityController(CityMasterController CityAPIController, MastersKeyValueController mastersKeyValueController)
        {
            _cityAPIController = CityAPIController;
            _mastersKeyValueController = mastersKeyValueController;

        }

        public async Task<IActionResult> City()
        {
            CityMasterDTO outputData = new CityMasterDTO();
            outputData.CityMasterList = await GetCityList();
            outputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> UnitCity()
        {
            UnitCityMasterDTO outputData = new UnitCityMasterDTO();
          
            outputData.UnitCityMasterList = await GetUnitCityList();
            outputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> AssignUnitCity()
        {

            UnitCityListVM outputData = new UnitCityListVM();
            outputData.CityMasterList = await GetCityList();
            outputData.UnitCityList = await GetUnitCityList();
            int clientId = 0;
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
            {
                outputData.Units = await _cityAPIController.GetClientUnits(clientId);
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
        public async Task<List<CityMasterDTO>?> GetCityList()
        {

            IActionResult actionResult = await _cityAPIController.GetCities(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;
            List<CityMasterDTO> objResultData = (List<CityMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedCityId = CommonHelper.EncryptURLHTML(item.CityId.ToString());
            }
            return objResultData;
        }

        public async Task<List<UnitCityMasterDTO>?> GetUnitCityList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _cityAPIController.GetUnitCities(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;
            List<UnitCityMasterDTO> objResultData = (List<UnitCityMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedCityId = CommonHelper.EncryptURLHTML(item.CityId.ToString());
            }
            return objResultData;
        }

        [HttpGet]
        [Route("City/GetCityInfo/{eCityId}")]
        public async Task<IActionResult> GetCityInfo(string eCityId)
        {

            int CityId = 0;
            try
            {
                CityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCityId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (CityId != 0)
            {
                CityMasterDTO outputData = new CityMasterDTO();
                outputData.CityId = CityId;

                IActionResult actionResult;

                actionResult = await _cityAPIController.GetCity(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (CityMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    objResultData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
                    return View("City", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.CityId = 0;
                    objResultData.CityMasterList = await GetCityList();

                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                }
            }
            return RedirectToAction("City", "City");

        }

        [HttpGet]
        [Route("City/GetUnitCityInfo/{eCityId}")]
        public async Task<IActionResult> GetUnitCityInfo(string eCityId)
        {

            int CityId = 0;
            try
            {
                CityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCityId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (CityId != 0)
            {
                UnitCityMasterDTO outputData = new UnitCityMasterDTO();
                outputData.CityId = CityId;

                IActionResult actionResult;

                actionResult = await _cityAPIController.GetUnitCity(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (UnitCityMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    objResultData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
                    return View("City", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.CityId = 0;
                    objResultData.UnitCityMasterList = await GetUnitCityList();

                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                }
            }
            return RedirectToAction("UnitCity", "City");

        }

        [HttpGet]
        public async Task<List<StateKeyValues>>? GetCounryStates(int countryId)
        {
            return await _mastersKeyValueController.StateKeyValue(true, countryId);
        }

        [HttpGet]
        public async Task<List<StateKeyValues>>? GetCounryUnitStates(int countryId)
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            return await _mastersKeyValueController.UnitStateKeyValue(true, countryId, unitId);
        }

        [HttpGet]
        [Route("City/DeleteCity/{eCityId}")]
        public async Task<IActionResult> DeleteCity(string eCityId)
        {

            int CityId = 0;
            try
            {
                CityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCityId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (CityId != 0)
            {
                CityMasterDTO outputData = new CityMasterDTO();
                outputData.CityId = CityId;

                IActionResult actionResult;

                actionResult = await _cityAPIController.DeleteCity(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.CityId = 0;
                outputData.CityMasterList = await GetCityList();
                outputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
                //_cityAPIController.GetCityCountry(outputData.CityMasterList);
                //_cityAPIController.GetCityState(outputData.CityMasterList);
                outputData.DisplayMessage = "Transaction Successful!";
                return View("City", outputData);
                //}
            }
            return RedirectToAction("City", "City");

        }

        [HttpGet]
        [Route("City/DeleteUnitCity/{eCityId}")]
        public async Task<IActionResult> DeleteUnitCity(string eCityId)
        {

            int CityId = 0;
            try
            {
                CityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCityId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (CityId != 0)
            {
                UnitCityMasterDTO outputData = new UnitCityMasterDTO();
                outputData.CityId = CityId;

                IActionResult actionResult;

                actionResult = await _cityAPIController.DeleteUnitCity(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.CityId = 0;
                outputData.UnitCityMasterList = await GetUnitCityList();
                outputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
                //_cityAPIController.GetCityCountry(outputData.CityMasterList);
                //_cityAPIController.GetCityState(outputData.CityMasterList);
                outputData.DisplayMessage = "Transaction Successful!";
                return View("UnitCity", outputData);
                //}
            }
            return RedirectToAction("UnitCity", "City");

        }

        [HttpPost]
        public async Task<IActionResult> SaveCity(CityMasterDTO inputData)
        {

            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    inputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);                
            //    return View("City", inputData);
            //}
            inputData.IsActive = true;
            IActionResult actionResult;
            CityMasterDTO viewModel = new CityMasterDTO();
            if (inputData.CityId == 0)
                actionResult = _cityAPIController.SaveCity(inputData);
            else
                actionResult = _cityAPIController.UpdateCity(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;
            inputData.HttpStatusCode = objResult.StatusCode;

            var objResultData = objResult.Value;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.CityId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.CityId = 0;
                inputData.CityMasterList = await GetCityList();
                inputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            inputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
            viewModel = inputData;
            return View("City", viewModel);

        }


        [HttpPost]
        public async Task<IActionResult> SaveUnitCity(UnitCityMasterDTO inputData)
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    inputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);                
            //    return View("City", inputData);
            //}
            inputData.UnitId = (int)unitId;
            inputData.IsActive = true;
            inputData.CreatedOn = DateTime.Now;
            inputData.CreatedBy = Convert.ToInt32(employeeId);
            IActionResult actionResult;
            UnitCityMasterDTO viewModel = new UnitCityMasterDTO();
            if (inputData.CityId == 0)
                actionResult = _cityAPIController.SaveUnitCity(inputData);
            else
                actionResult = _cityAPIController.UpdateUnitCity(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;
            inputData.HttpStatusCode = objResult.StatusCode;

            var objResultData = objResult.Value;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.CityId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.CityId = 0;
                inputData.UnitCityMasterList = await GetUnitCityList();
                inputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            inputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
            viewModel = inputData;
            return View("UnitCity", viewModel);

        }

        [HttpPost]
        public async Task<UnitCityListVM> SaveUnitCityFromMaster(UnitCityListVM unitCityVM)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            IActionResult actionResult;
            actionResult = _cityAPIController.SaveUnitCityFromMaster(unitCityVM, employeeId);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;
            unitCityVM.DisplayMessage = "Success";
            return unitCityVM;
        }
    }
}
