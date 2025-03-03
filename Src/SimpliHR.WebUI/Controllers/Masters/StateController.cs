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
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using DocumentFormat.OpenXml.Wordprocessing;
using SimpliHR.WebUI.BL;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class StateController : Controller
    {

        private readonly StateMasterController _stateAPIController;
        private readonly CountryMasterController _countryMasterController;
        public StateController(StateMasterController stateAPIController, CountryMasterController countryMasterController)
        {
            _stateAPIController = stateAPIController;
            _countryMasterController = countryMasterController;
        }

        public async Task<IActionResult> State()
        {
            

            StateMasterDTO outputData = new StateMasterDTO();
            outputData.StateMasterList = await GetStateList();
            // _stateAPIController.GetStatesCountry(outputData.StateMasterList);
            outputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
            //foreach (var item in outputData.StateMasterList)
            //{
            //    item.EncryptedStateId = CommonHelper.EncryptURLHTML(item.StateId.ToString());
            //}
            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }


        }

        public async Task<IActionResult> UnitState()
        {
            UnitStateMasterDTO outputData = new UnitStateMasterDTO();
            outputData.UnitStateMasterList = await GetUnitStateList();
            // _stateAPIController.GetStatesCountry(outputData.StateMasterList);
            outputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
            //foreach (var item in outputData.StateMasterList)
            //{
            //    item.EncryptedStateId = CommonHelper.EncryptURLHTML(item.StateId.ToString());
            //}
            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }


        public async Task<IActionResult> AssignUnitState()
        {

            UnitStateListVM outputData = new UnitStateListVM();
            outputData.StateMasterList = await GetStateList();
            outputData.UnitStateList = await GetUnitStateList();
            int clientId = 0;
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
            {
                outputData.Units = await _stateAPIController.GetClientUnits(clientId);
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
        public async Task<List<StateMasterDTO>?> GetStateList()
        {

            IActionResult actionResult = await _stateAPIController.GetStates(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<StateMasterDTO> objResultData = (List<StateMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedStateId = CommonHelper.EncryptURLHTML(item.StateId.ToString());
            }

            return objResultData;
        }

        public async Task<List<UnitStateMasterDTO>?> GetUnitStateList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _stateAPIController.GetUnitStates(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<UnitStateMasterDTO> objResultData = (List<UnitStateMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedStateId = CommonHelper.EncryptURLHTML(item.StateId.ToString());
            }

            return objResultData;
        }

        [HttpGet]
        [Route("State/GetStateInfo/{eStateId}")]
        public async Task<IActionResult> GetStateInfo(string eStateId)
        {
            int StateId = 0;
            try
            {
                StateId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eStateId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (StateId != 0)
            {
                StateMasterDTO outputData = new StateMasterDTO();
                outputData.StateId = StateId;

                IActionResult actionResult;

                actionResult = await _stateAPIController.GetState(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (StateMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    objResultData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
                    return View("State", objResultData);
                    //return RedirectToAction("State","State", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.StateId = 0;
                    objResultData.StateMasterList = await GetStateList();
                    outputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
                    //_stateAPIController.GetStatesCountry(outputData.StateMasterList);
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    //return View("State",objResultData);
                    //return RedirectToAction("State", objResultData);
                }
            }
            return RedirectToAction("State", "State");
        }


        [HttpGet]
        [Route("State/GetUnitStateInfo/{eStateId}")]
        public async Task<IActionResult> GetUnitStateInfo(string eStateId)
        {
            int StateId = 0;
            try
            {
                StateId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eStateId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (StateId != 0)
            {
                UnitStateMasterDTO outputData = new UnitStateMasterDTO();
                outputData.StateId = StateId;

                IActionResult actionResult;

                actionResult = await _stateAPIController.GetUnitState(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (UnitStateMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    objResultData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
                    return View("UnitState", objResultData);
                    //return RedirectToAction("State","State", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.StateId = 0;
                    objResultData.UnitStateMasterList = await GetUnitStateList();
                    outputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
                    //_stateAPIController.GetStatesCountry(outputData.StateMasterList);
                    objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                    //return View("State",objResultData);
                    //return RedirectToAction("State", objResultData);
                }
            }
            return RedirectToAction("UnitState", "State");
        }

        [HttpGet]
        [Route("State/DeleteState/{eStateId}")]
        public async Task<IActionResult> DeleteState(string eStateId)
        {
            int StateId = 0;
            try
            {
                StateId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eStateId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (StateId != 0)
            {
                StateMasterDTO outputData = new StateMasterDTO();
                outputData.StateId = StateId;

                IActionResult actionResult;

                actionResult = await _stateAPIController.DeleteState(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.StateId = 0;
                outputData.StateMasterList = await GetStateList();
                outputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
                // _stateAPIController.GetStatesCountry(outputData.StateMasterList);
                outputData.DisplayMessage = "Transaction Successful!";
                return View("State", outputData);
                //}
            }
            return RedirectToAction("State", "State");
        }

        [HttpGet]
        [Route("State/DeleteUnitState/{eStateId}")]
        public async Task<IActionResult> DeleteUnitState(string eStateId)
        {
            int StateId = 0;
            try
            {
                StateId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eStateId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (StateId != 0)
            {
                UnitStateMasterDTO outputData = new UnitStateMasterDTO();
                outputData.StateId = StateId;

                IActionResult actionResult;

                actionResult = await _stateAPIController.DeleteUnitState(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.StateId = 0;
                outputData.UnitStateMasterList = await GetUnitStateList();
                outputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
                // _stateAPIController.GetStatesCountry(outputData.StateMasterList);
                outputData.DisplayMessage = "Transaction Successful!";
                return View("UnitState", outputData);
                //}
            }
            return RedirectToAction("UnitState", "State");
        }

        [HttpPost]
        public async Task<IActionResult> SaveState(StateMasterDTO inputData)
        {

            inputData.IsActive = true;
            IActionResult actionResult;
            StateMasterDTO viewModel = new StateMasterDTO();
            if (inputData.StateId == 0)
                actionResult = _stateAPIController.SaveState(inputData);
            else
                actionResult = _stateAPIController.UpdateState(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.StateId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.StateId = 0;
                inputData.StateMasterList = await GetStateList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            inputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
            viewModel = inputData;

            return View("State", viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> SaveUnitState(UnitStateMasterDTO inputData)
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    inputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
            //    return View("State", inputData);
            //}
            inputData.UnitId = (int)unitId;
            inputData.IsActive = true;
            inputData.CreatedOn = DateTime.Now;
            inputData.CreatedBy = Convert.ToInt32(employeeId);
            IActionResult actionResult;

            UnitStateMasterDTO viewModel = new UnitStateMasterDTO();
            if (inputData.StateId == 0)
            {
                inputData.StateParentId = 0;
                actionResult = _stateAPIController.SaveUnitState(inputData);
            }
            else
                actionResult = _stateAPIController.UpdateUnitState(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.StateId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.StateId = 0;
                inputData.UnitStateMasterList = await GetUnitStateList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            inputData.CountryList = _countryMasterController.GetCountryKeyValue().ToList();
            viewModel = inputData;
            return View("UnitState", viewModel);

        }


        [HttpPost]
        public async Task<UnitStateListVM> SaveUnitStateFromMaster(UnitStateListVM unitStateVM)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            IActionResult actionResult;
            actionResult = _stateAPIController.SaveUnitStateFromMaster(unitStateVM, employeeId);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;
            unitStateVM.DisplayMessage = "Success";
            return unitStateVM;
        }
    }
}
