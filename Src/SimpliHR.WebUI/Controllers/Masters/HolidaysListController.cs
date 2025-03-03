using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.KeyValue;
using iTextSharp.xmp.impl;
using SimpliHR.Core.Entities;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class HolidaysListController : Controller
    {
        private readonly HolidaysListMasterController _holidaysListAPIController;
        private readonly MastersKeyValueController _mastersKeyValueController;
        public HolidaysListController(HolidaysListMasterController holidaysListAPIController, MastersKeyValueController mastersKeyValueController)
        {
            _holidaysListAPIController = holidaysListAPIController;
            _mastersKeyValueController = mastersKeyValueController;
        }

        public async Task<IActionResult> HolidaysList()
        {
            HolidaysListMasterDTO outputData = new HolidaysListMasterDTO();
            outputData.HolidaysListMasterList = await GetHolidaysListList();
            if (outputData != null)
            {
                //foreach (var item in outputData.HolidaysListMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.HolidayId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<HolidaysListMasterDTO>?> GetHolidaysListList()
        {

            IActionResult actionResult = await _holidaysListAPIController.GetHolidaysLists(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<HolidaysListMasterDTO> objResultData = (List<HolidaysListMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.HolidayId.ToString());
            }
            return objResultData;
        }
        [HttpPost]
        public async Task<IActionResult> SaveHolidaysList(HolidaysListMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("HolidaysList", inputData);
            //}
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            HolidaysListMasterDTO viewModel = new HolidaysListMasterDTO();
            ;
            if (inputData.HolidayId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
                actionResult = _holidaysListAPIController.SaveHolidaysList(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifedBy = employeeId;
                actionResult = _holidaysListAPIController.UpdateHolidaysList(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.HolidayId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.HolidayId = 0;
                inputData.HolidaysListMasterList = await GetHolidaysListList();
            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            viewModel = inputData;
            return View("HolidaysList", viewModel);

        }


        [HttpGet]
        [Route("HolidaysList/GetHolidaysListInfo/{eholidaysListId}")]
        public async Task<IActionResult> GetHolidaysListInfo(string eholidaysListId)
        {
            int holidaysListId = 0;
            try
            {
                holidaysListId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eholidaysListId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (holidaysListId != 0)
            {
                HolidaysListMasterDTO outputData = new HolidaysListMasterDTO();
                outputData.HolidayId = holidaysListId;

                IActionResult actionResult;

                actionResult = await _holidaysListAPIController.GetHolidaysList(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (HolidaysListMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("HolidaysList", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.HolidayId = 0;
                    objResultData.HolidaysListMasterList = await GetHolidaysListList();
                    objResultData.DisplayMessage = "You cannot edit locked holiday. Contact Admin for further details";
                    return View("HolidaysList", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("HolidaysList", "HolidaysList");

        }

        [HttpGet]
        [Route("HolidaysList/DeleteHolidaysList/{eholidaysListId}")]
        public async Task<IActionResult> DeleteHolidaysList(string eholidaysListId)
        {
            int holidaysListId = 0;
            try
            {
                holidaysListId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eholidaysListId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (holidaysListId != 0)
            {

                HolidaysListMasterDTO outputData = new HolidaysListMasterDTO();
                outputData.HolidayId = holidaysListId;

                IActionResult actionResult;

                actionResult = await _holidaysListAPIController.DeleteHolidaysList(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.HolidayId = 0;
                outputData.HolidaysListMasterList = await GetHolidaysListList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("HolidaysList", outputData);
                //}
            }
            return RedirectToAction("HolidaysList", "HolidaysList");

        }


        #region "Unit wise Holiday List"

        public async Task<IActionResult> UnitHolidayList()
        {

            UnitHolidayListVM outputData = new UnitHolidayListVM();
            outputData.HolidayTypes = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "HolidaysList" && x.PageName == "AssignUnitHolidays" && x.ControlName == "HolidayType");
            outputData.UnitHolidayList = await GetUnitHolidaysList(","+HttpContext.Session.GetInt32("UnitId").ToString()+",");
            if (outputData != null)
            {
                //foreach (var item in outputData.HolidaysListMasterList)
                //{
                //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.HolidayId.ToString());
                //}
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<UnitHolidayListDTO>?> GetUnitHolidaysList(string sUnit)
        {

            IActionResult actionResult = await _holidaysListAPIController.GetUnitHolidaysList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, sUnit);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<UnitHolidayListDTO> objResultData = (List<UnitHolidayListDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.UnitHolidayId.ToString());
                item.HolidayDayName = @item.HolidayDay!=null? Enum.GetName(typeof(DayOfWeek), @item.HolidayDay) : "" ;
            }
            return objResultData;
        }

        [HttpPost]
        public async Task<IActionResult> SaveUnitHolidayList(UnitHolidayListDTO inputData)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;
            IActionResult actionResult;
            UnitHolidayListVM viewModel = new UnitHolidayListVM();
            inputData.HolidayDay = inputData.HolidayDay == null ? (int)inputData.HolidayDate.Value.DayOfWeek : inputData.HolidayDay;
            viewModel.UnitHolidays = inputData;
            if (inputData.UnitHolidayId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
                actionResult = _holidaysListAPIController.SaveUnitHolidayList(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifedBy = employeeId;
                actionResult = _holidaysListAPIController.UpdateUnitHolidayList(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.HolidayId == 0)
                    viewModel.DisplayMessage = "Transaction Successful!";
                else
                    viewModel.DisplayMessage = "Transaction Successful!";
                viewModel.UnitHolidays.UnitHolidayId = 0;
                viewModel.UnitHolidayList = await GetUnitHolidaysList("," + HttpContext.Session.GetInt32("UnitId").ToString() + ",");
            }
            else
                viewModel.DisplayMessage = objResultData.ToString();

            //viewModel = inputData;
            return View("UnitHolidayList", viewModel);

        }

        [HttpGet]
        [Route("HolidaysList/GetUnitHolidaysListInfo/{eholidaysListId}")]
        public async Task<IActionResult> GetUnitHolidaysListInfo(string eholidaysListId)
        {

            int holidaysListId = 0;
            try
            {
                holidaysListId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eholidaysListId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            if (holidaysListId != 0)
            {
                UnitHolidayListDTO outputData = new UnitHolidayListDTO();
                outputData.UnitHolidayId = holidaysListId;

                IActionResult actionResult;
                UnitHolidayListVM unitHolidayListVM = new UnitHolidayListVM();
                actionResult = await _holidaysListAPIController.GetUnitHolidayData(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (UnitHolidayListDTO)objResult.Value;
                unitHolidayListVM.UnitHolidays = (UnitHolidayListDTO)objResult.Value;
                unitHolidayListVM.HolidayTypes = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "HolidaysList" && x.PageName == "AssignUnitHolidays" && x.ControlName == "HolidayType");
                unitHolidayListVM.UnitHolidays.HolidayDayName = unitHolidayListVM.UnitHolidays.HolidayDate.Value.DayOfWeek.ToString();
                unitHolidayListVM.UnitHolidays.HolidayDay =(int)unitHolidayListVM.UnitHolidays.HolidayDate.Value.DayOfWeek;
                if (objResult.StatusCode == 200)
                {
                    return View("UnitHolidayList", unitHolidayListVM);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.HolidayId = 0;
                    //objResultData.HolidaysListMasterList = await GetHolidaysListList();
                    //objResultData.DisplayMessage = "You cannot edit locked holiday. Contact Admin for further details";
                    //return View("HolidaysList", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("UnitHolidayList", "UnitHolidaysList");

        }

        [HttpGet]
        [Route("HolidaysList/DeleteHolidaysList/{eholidaysListId}")]
        public async Task<IActionResult> DeleteUnitHolidaysList(string eholidaysListId)
        {
            int holidaysListId = 0;
            try
            {
                holidaysListId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eholidaysListId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (holidaysListId != 0)
            {

                HolidaysListMasterDTO outputData = new HolidaysListMasterDTO();
                outputData.HolidayId = holidaysListId;

                IActionResult actionResult;

                actionResult = await _holidaysListAPIController.DeleteHolidaysList(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.HolidayId = 0;
                outputData.HolidaysListMasterList = await GetHolidaysListList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("HolidaysList", outputData);
                //}
            }
            return RedirectToAction("UnitHolidayList", "HolidaysList");

        }



        #endregion "Unit wise Holiday List"

        public async Task<IActionResult> AssignUnitHolidays()
        {

            UnitHolidayListVM outputData = new UnitHolidayListVM();
            outputData.HolidayMasterList = await GetHolidaysListList();
            
            outputData.HolidayTypes = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "HolidaysList" && x.PageName == "AssignUnitHolidays" && x.ControlName == "HolidayType");
            int clientId = 0;
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
            {
                outputData.Units = await _holidaysListAPIController.GetClientUnits(clientId);
                string sUnitId= "," + string.Join(",", outputData.Units.Select(t => { return t.UnitID; })) + ",";
                outputData.UnitHolidayList = await GetUnitHolidaysList(sUnitId);
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

        [HttpPost]
        public async Task<UnitHolidayListVM> SaveUnitHolidaysFromMaster(UnitHolidayListVM unitHolidaysVM)
        {
            unitHolidaysVM.DisplayMessage = "Transaction Successful!";
            unitHolidaysVM = await _holidaysListAPIController.AssignUnitHolidays(unitHolidaysVM);

            return unitHolidaysVM;
        }

    }
}
