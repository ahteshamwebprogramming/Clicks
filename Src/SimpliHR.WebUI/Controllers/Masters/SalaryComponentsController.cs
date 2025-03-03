using iTextSharp.text.pdf.codec.wmf;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Masters
{
    public class SalaryComponentsController : Controller
    {
        private readonly SalaryComponentMasterController _salaryComponentAPIController;
        public SalaryComponentsController(SalaryComponentMasterController salaryComponentAPIController)
        {
            _salaryComponentAPIController = salaryComponentAPIController;
        }

        public async Task<IActionResult> SalaryComponent()
        {

            SalaryComponentMasterDTO outputData = new SalaryComponentMasterDTO();
            outputData.SalaryComponentMasterList = await GetSalaryComponentList();

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> TaxSlabs()
        {

            TaxSlabDetailsDTO outputData = new TaxSlabDetailsDTO();
            outputData.TaxSlabDetailsList = await GetTaxSlabList(1);

            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<SalaryComponentMasterDTO>?> GetSalaryComponentList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _salaryComponentAPIController.GetSalaryComponents(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<SalaryComponentMasterDTO> objResultData = (List<SalaryComponentMasterDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.SalaryComponentId.ToString());
            }
            return objResultData;
        }

        public async Task<IActionResult> GetTaxList(int ageGroupId)
        {

            TaxSlabDetailsDTO outputData = new TaxSlabDetailsDTO();
            outputData.TaxSlabDetailsList = await GetTaxSlabList(ageGroupId);
            outputData.AgeGroupId = ageGroupId;
            return View("TaxSlabs", outputData);
        }

        public async Task<List<TaxSlabDetailsDTO>?> GetTaxSlabList(int ageGroupId)
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _salaryComponentAPIController.GetTaxSlabs(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId, ageGroupId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<TaxSlabDetailsDTO> objResultData = (List<TaxSlabDetailsDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.SlabID.ToString());
            }
            return objResultData;
        }

        [HttpPost]
        public async Task<IActionResult> SaveTaxSlab(TaxSlabDetailsDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            // return View("SalaryComponent", inputData);
            //}
            inputData.IsActive = true;
            // inputData.CreatedBy = HttpContext.Session.GetString("EmployeeId");
            inputData.UnitId = HttpContext.Session.GetInt32("UnitId");

            IActionResult actionResult;
            TaxSlabDetailsDTO viewModel = new TaxSlabDetailsDTO();
            ;
            if (inputData.SlabID == 0)
            {
                // inputData.CreatedOn = DateTime.UtcNow;
                // inputData.CreatedBy = HttpContext.Session.GetString("EmployeeId");
                actionResult = _salaryComponentAPIController.SaveTaxSlab(inputData);
            }
            else
            {
                // inputData.ModifiedOn = DateTime.UtcNow;
                // inputData.ModifedBy = HttpContext.Session.GetString("EmployeeId");
                actionResult = _salaryComponentAPIController.UpdateTaxSlab(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            //inputData.HttpStatusCode = objResult.StatusCode;

            //if (inputData.HttpStatusCode == 200)
            //{
            //    if (inputData.SalaryComponentId == 0)
            //        inputData.DisplayMessage = "Salary Components successfully created";
            //    else
            //        inputData.DisplayMessage = "Salary Components updates completed successfully";
            //    inputData.SalaryComponentId = 0;
            //}
            //else
            //    inputData.DisplayMessage = objResultData.ToString();

            inputData.TaxSlabDetailsList = await GetTaxSlabList((int)inputData.AgeGroupId);
            viewModel = inputData;
            return View("TaxSlabs", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> SaveSalaryComponent(SalaryComponentMasterDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            // return View("SalaryComponent", inputData);
            //}
            inputData.IsActive = true;
            inputData.CreatedBy = HttpContext.Session.GetString("EmployeeId");
            inputData.UnitId = HttpContext.Session.GetInt32("UnitId");

            IActionResult actionResult;
            SalaryComponentMasterDTO viewModel = new SalaryComponentMasterDTO();
            ;
            if (inputData.SalaryComponentId == 0)
            {
                inputData.CreatedOn = DateTime.UtcNow;
                inputData.CreatedBy = HttpContext.Session.GetString("EmployeeId");
                actionResult = _salaryComponentAPIController.SaveSalaryComponent(inputData);
            }
            else
            {
                inputData.ModifiedOn = DateTime.UtcNow;
                inputData.ModifedBy = HttpContext.Session.GetString("EmployeeId");
                actionResult = _salaryComponentAPIController.UpdateSalaryComponent(inputData);
            }


            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.SalaryComponentId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.SalaryComponentId = 0;
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            inputData.SalaryComponentMasterList = await GetSalaryComponentList();
            viewModel = inputData;
            return View("SalaryComponent", viewModel);
        }


        [HttpGet]
        [Route("SalaryComponents/GetSalaryComponentInfo/{esalaryComponentId}")]
        public async Task<IActionResult> GetSalaryComponentInfo(string esalaryComponentId)
        {
            int salaryComponentId = 0;
            try
            {
                salaryComponentId = Convert.ToInt32(CommonHelper.DecryptURLHTML(esalaryComponentId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (salaryComponentId != 0)
            {
                SalaryComponentMasterDTO outputData = new SalaryComponentMasterDTO();
                outputData.SalaryComponentId = salaryComponentId;

                IActionResult actionResult;

                actionResult = await _salaryComponentAPIController.GetSalaryComponent(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (SalaryComponentMasterDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("SalaryComponent", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.SalaryComponentId = 0;
                    objResultData.SalaryComponentMasterList = await GetSalaryComponentList();
                    objResultData.DisplayMessage = "You cannot edit locked salary Components. Contact Admin for further details";
                    return View("SalaryComponent", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("SalaryComponent", "SalaryComponents");
        }

        [HttpGet]
        [Route("SalaryComponents/DeleteSalaryComponent/{esalaryComponentId}")]
        public async Task<IActionResult> DeleteSalaryComponent(string esalaryComponentId)
        {
            int salaryComponentId = 0;
            try
            {
                salaryComponentId = Convert.ToInt32(CommonHelper.DecryptURLHTML(esalaryComponentId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (salaryComponentId != 0)
            {

                SalaryComponentMasterDTO outputData = new SalaryComponentMasterDTO();
                outputData.SalaryComponentId = salaryComponentId;

                IActionResult actionResult;

                actionResult = await _salaryComponentAPIController.DeleteSalaryComponent(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.SalaryComponentId = 0;
                outputData.SalaryComponentMasterList = await GetSalaryComponentList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("SalaryComponent", outputData);
                //}
            }
            return RedirectToAction("SalaryComponent", "SalaryComponents");
        }

        [HttpGet]
        [Route("SalaryComponents/GetTaxSlabInfo/{etaxslabId}")]
        public async Task<IActionResult> GetTaxSlabInfo(string etaxslabId)
        {
            int slabID = Convert.ToInt32(CommonHelper.DecryptURLHTML(etaxslabId));
            if (slabID != 0)
            {
                TaxSlabDetailsDTO outputData = new TaxSlabDetailsDTO();
                outputData.SlabID = slabID;

                IActionResult actionResult;

                actionResult = await _salaryComponentAPIController.GetTaxSlab(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (TaxSlabDetailsDTO)objResult.Value;
                // objResultData.TaxSlabDetailsList = await GetTaxSlabList();
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("TaxSlabs", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                //else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                //{
                //    objResultData.SalaryComponentId = 0;
                //    objResultData.SalaryComponentMasterList = await GetSalaryComponentList();
                //    objResultData.DisplayMessage = "You cannot edit locked salary Components. Contact Admin for further details";
                //    return View("SalaryComponent", objResultData);
                //    //return RedirectToAction("Role", objResultData);
                //}
            }
            return RedirectToAction("TaxSlabs", "SalaryComponents");
        }


        [HttpGet]
        [Route("SalaryComponents/DeleteTaxSlab/{etaxslabId}")]
        public async Task<IActionResult> DeleteTaxSlab(string etaxslabId)
        {
            int slabID = Convert.ToInt32(CommonHelper.DecryptURLHTML(etaxslabId));
            if (slabID != 0)
            {

                TaxSlabDetailsDTO outputData = new TaxSlabDetailsDTO();
                outputData.SlabID = slabID;

                IActionResult actionResult;

                actionResult = await _salaryComponentAPIController.DeleteTaxSlab(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.SlabID = 0;
                outputData.TaxSlabDetailsList = await GetTaxSlabList(1);
                outputData.DisplayMessage = "Transaction Successful!";
                return View("TaxSlabs", outputData);
                //}
            }
            return RedirectToAction("TaxSlabs", "SalaryComponents");
        }
    }
}
