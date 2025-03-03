using iTextSharp.text.pdf.codec.wmf;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.ExcelUploads;
using SimpliHR.Endpoints.Exit;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Endpoints.StatutoryComponent;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Infrastructure.Models.StatutoryComponent;
using System.Net;

namespace SimpliHR.WebUI.Controllers;

public class StatutoryComponentController : Controller
{
    private readonly StatutoryComponentAPIController _statutoryComponentAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly ClientController _clientSettingAPIController;


    public StatutoryComponentController(MastersKeyValueController mastersKeyValueController, StatutoryComponentAPIController statutoryComponentAPIController, ClientController clientSettingAPIController)
    {
        _mastersKeyValueController = mastersKeyValueController;
        _statutoryComponentAPIController = statutoryComponentAPIController;
        _clientSettingAPIController = clientSettingAPIController;

    }
    public async Task<IActionResult> StatutoryComponents()
    {

        // int? unitId = HttpContext.Session.GetInt32("UnitId");
        // ComponentsTaxLimitDTO outputData = new ComponentsTaxLimitDTO();
        //outputData.UnitId = unitId;
        //IActionResult actionResult;

        //actionResult = await _statutoryComponentAPIController.GetComponentsTaxLimit(outputData);
        //ObjectResult objResult = (ObjectResult)actionResult;
        //var objResultData = (ComponentsTaxLimitDTO)objResult.Value;
        //if (objResultData.HttpStatusCode != 200)
        //{
        //    objResultData.DisplayMessage = "Records not found";
        //    //return View("City", objResultData);
        //}
        //  return View(outputData);
        return View();
    }

    public async Task<IActionResult> ComponentTaxLimit()
    {

        int? unitId = HttpContext.Session.GetInt32("UnitId");
        ComponentsTaxLimitDTO outputData = new ComponentsTaxLimitDTO();
        outputData.UnitId = unitId;
        IActionResult actionResult;

        actionResult = await _statutoryComponentAPIController.GetComponentsTaxLimit(outputData);
        ObjectResult objResult = (ObjectResult)actionResult;
        var objResultData = (ComponentsTaxLimitDTO)objResult.Value;
        if (objResultData.HttpStatusCode != 200)
        {
            objResultData.DisplayMessage = "No Records Found";
            //return View("City", objResultData);
        }
        return View(objResultData);
        // return View();
    }
    public async Task<IActionResult> ESI()
    {
        return View();
    }
    public async Task<IActionResult> StatutoryComponentsEPFView()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        StatutoryComponent_EPFDTO outputData = new StatutoryComponent_EPFDTO();

        if (unitId != null)
        {
            IActionResult actionResult = await _statutoryComponentAPIController.GetStatutoryComponent(unitId ?? default(int));
            ObjectResult objResult = (ObjectResult)actionResult;
            outputData = (StatutoryComponent_EPFDTO)objResult.Value;
            if (outputData != null)
                outputData.StatutoryComponentsIdEnc = CommonHelper.Encrypt(outputData.StatutoryComponentsId.ToString());
            return PartialView("_StatutoryComponent/_EPFView", outputData);
        }
        else
        {
            return PartialView("_StatutoryComponent/_EPFView", outputData);
        }
    }
    public async Task<IActionResult> StatutoryComponentsESIView()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        StatutoryComponentsEsiDTO outputData = new StatutoryComponentsEsiDTO();

        if (unitId != null)
        {
            IActionResult actionResult = await _statutoryComponentAPIController.GetStatutoryComponentESI(unitId ?? default(int));
            ObjectResult objResult = (ObjectResult)actionResult;
            outputData = (StatutoryComponentsEsiDTO)objResult.Value;
            if (outputData != null)
                outputData.StatutoryComponentsEsiIdEnc = CommonHelper.Encrypt(outputData.StatutoryComponentsEsiid.ToString());
            return PartialView("_StatutoryComponent/_ESIView", outputData);
        }
        else
        {
            return PartialView("_StatutoryComponent/_ESIView", outputData);
        }
    }
    public async Task<IActionResult> StatutoryComponentsLabourWelfareFundView()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        StatutoryComponentsLabourWelfareFundDTO outputData = new StatutoryComponentsLabourWelfareFundDTO();
        if (unitId != null)
        {
            IActionResult actionResult = await _statutoryComponentAPIController.GetStatutoryComponentLabourWelfareFund(unitId ?? default(int));
            ObjectResult objResult = (ObjectResult)actionResult;
            outputData = (StatutoryComponentsLabourWelfareFundDTO)objResult.Value;
            if (outputData != null)
            {
                IActionResult actionResultUnit = await _clientSettingAPIController.GetClientUnitNameById(unitId ?? default(int));
                ObjectResult objResultUnit = (ObjectResult)actionResultUnit;
                outputData.SelectedUnit = (UnitMasterDTO)objResultUnit.Value;
            }
            return PartialView("_StatutoryComponent/_LabourWelfareFundView", outputData);
        }
        else
        {
            return PartialView("_StatutoryComponent/_LabourWelfareFundView", outputData);
        }
    }
    public async Task<IActionResult> StatutoryComponentsEPFEdit()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");

        StatutoryComponent_EPFDTO outputData = new StatutoryComponent_EPFDTO();
        if (unitId != null)
        {
            IActionResult actionResult = await _statutoryComponentAPIController.GetStatutoryComponent(unitId ?? default(int));
            ObjectResult objResult = (ObjectResult)actionResult;
            if (objResult.Value != null)
            {
                outputData = (StatutoryComponent_EPFDTO)objResult.Value;

            }

            outputData.EmployeeKeyValues = await _mastersKeyValueController.EmployeeKeyValue(p => p.IsActive == true && p.InfoFillingStatus == 1 && p.UnitId == unitId);
            outputData.DepartmentKeyValues = await _mastersKeyValueController.DepartmentKeyValue(x => x.IsActive == true && x.UnitId == unitId);
            outputData.UnitId = (int)unitId;
            return PartialView("_StatutoryComponent/_EPFEdit", outputData);
        }
        else
        {
            return PartialView("_StatutoryComponent/_EPFEdit", outputData);
        }
    }
    public async Task<IActionResult> StatutoryComponentsESIEdit()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");

        StatutoryComponentsEsiDTO outputData = new StatutoryComponentsEsiDTO();
        if (unitId != null)
        {
            IActionResult actionResult = await _statutoryComponentAPIController.GetStatutoryComponentESI(unitId ?? default(int));
            ObjectResult objResult = (ObjectResult)actionResult;
            if (objResult.Value != null)
            {
                outputData = (StatutoryComponentsEsiDTO)objResult.Value;

            }
            return PartialView("_StatutoryComponent/_ESIEdit", outputData);
        }
        else
        {
            return PartialView("_StatutoryComponent/_ESIEdit", outputData);
        }
    }
    public async Task<IActionResult> StatutoryComponentsLabourWelfareFundEdit()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        StatutoryComponentsLabourWelfareFundDTO outputData = new StatutoryComponentsLabourWelfareFundDTO();
        if (unitId != null)
        {
            IActionResult actionResult = await _statutoryComponentAPIController.GetStatutoryComponentLabourWelfareFund(unitId ?? default(int));
            ObjectResult objResult = (ObjectResult)actionResult;
            if ((StatutoryComponentsLabourWelfareFundDTO)objResult.Value != null)
            {
                outputData = (StatutoryComponentsLabourWelfareFundDTO)objResult.Value;
            }

            if (outputData != null)
            {
                IActionResult actionResultUnit = await _clientSettingAPIController.GetClientUnitNameById(unitId ?? default(int));
                ObjectResult objResultUnit = (ObjectResult)actionResultUnit;
                outputData.SelectedUnit = (UnitMasterDTO)objResultUnit.Value;
            }
            return PartialView("_StatutoryComponent/_LabourWelfareFundEdit", outputData);
        }
        else
        {
            return PartialView("_StatutoryComponent/_LabourWelfareFundEdit", outputData);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveEPFData([FromBody] StatutoryComponent_EPFDTO statutoryComponent_EPFDTO)
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        if (unitId != null)
        {
            statutoryComponent_EPFDTO.UnitId = unitId ?? default(int);
            IActionResult actionResult = await _statutoryComponentAPIController.SaveEmployeeEPFMapping(statutoryComponent_EPFDTO);
            return actionResult;

        }
        else
        {
            return Unauthorized();
        }


    }
    [HttpPost]
    public async Task<IActionResult> SaveESIData([FromBody] StatutoryComponentsEsiDTO inputData)
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        if (unitId != null)
        {
            inputData.UnitId = unitId ?? default(int);
            IActionResult actionResult = await _statutoryComponentAPIController.SaveStatutoryComponentsESI(inputData);
            return actionResult;

        }
        else
        {
            return Unauthorized();
        }


    }

    [HttpGet]
    [Route("StatutoryComponent/GetEPFEmployees/{id}")]
    public async Task<StatutoryComponent_EPFDTO> GetEPFEmployees(string id)
    {
        StatutoryComponent_EPFDTO scEPFData = new StatutoryComponent_EPFDTO();
        string _employeeStatuoryId = CommonHelper.Decrypt(id);
        int employeeStatuoryId;
        if (CommonHelper.IsNumeric(_employeeStatuoryId))
        {
            int.TryParse(_employeeStatuoryId, out employeeStatuoryId);
            scEPFData.EpfemployeeMappingList = await _statutoryComponentAPIController.GetEPFEmployees(employeeStatuoryId);
            scEPFData.EmployeeKeyValues = await _mastersKeyValueController.EmployeeKeyValue();
            foreach (var item in scEPFData.EpfemployeeMappingList)
                item.EmployeeName = scEPFData.EmployeeKeyValues.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.EmployeeName).FirstOrDefault();
            scEPFData.DisplayMessage = "Success";
        }
        else
            scEPFData.DisplayMessage = "Error searching details for given Id. Invalid Inputs to edit";

        return scEPFData;
    }


    [HttpPost]
    public async Task<IActionResult> SaveLabourWelfareFundData([FromBody] StatutoryComponentsLabourWelfareFundDTO statutoryComponentsLabourWelfareFundDTO)
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        if (unitId != null)
        {
            //statutoryComponentsLabourWelfareFundDTO.UnitId = unitId ?? default(int);
            statutoryComponentsLabourWelfareFundDTO.UnitId = (int)unitId;
            IActionResult actionResult = await _statutoryComponentAPIController.SaveStatutoryComponentLabourWelfareFund(statutoryComponentsLabourWelfareFundDTO);
            //ObjectResult objResult = (ObjectResult)actionResult;
            //if (objResult != null)
            //{
            //    if (objResult.StatusCode == 200)
            //    {
            //        return Ok("Saved");
            //    }
            //}
            return actionResult;

        }
        else
        {
            return Unauthorized();
        }
    }
    public async Task<IActionResult> EnableEPFData([FromBody] StatutoryComponent_EPFDTO statutoryComponent_EPFDTO)
    {
        if (statutoryComponent_EPFDTO.StatutoryComponentsId != 0)
        {
            var res = await _statutoryComponentAPIController.EnableEPFData(statutoryComponent_EPFDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> DisableEPFData([FromBody] StatutoryComponent_EPFDTO statutoryComponent_EPFDTO)
    {
        if (statutoryComponent_EPFDTO.StatutoryComponentsId != 0)
        {
            var res = await _statutoryComponentAPIController.DisableEPFData(statutoryComponent_EPFDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> EnableESIData([FromBody] StatutoryComponentsEsiDTO inputData)
    {
        if (inputData.StatutoryComponentsEsiid != 0)
        {
            var res = await _statutoryComponentAPIController.EnableESIData(inputData);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> DisableESIData([FromBody] StatutoryComponentsEsiDTO inputData)
    {
        if (inputData.StatutoryComponentsEsiid != 0)
        {
            var res = await _statutoryComponentAPIController.DisableESIData(inputData);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> EnableLabourWelfareFundData([FromBody] StatutoryComponentsLabourWelfareFundDTO statutoryComponentsLabourWelfareFundDTO)
    {
        if (statutoryComponentsLabourWelfareFundDTO.StatutoryComponentsLabourWelfareFundId != 0)
        {
            var res = await _statutoryComponentAPIController.EnableLabourWelfareFundData(statutoryComponentsLabourWelfareFundDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> DisableLabourWelfareFundData([FromBody] StatutoryComponentsLabourWelfareFundDTO statutoryComponentsLabourWelfareFundDTO)
    {
        if (statutoryComponentsLabourWelfareFundDTO.StatutoryComponentsLabourWelfareFundId != 0)
        {
            var res = await _statutoryComponentAPIController.DisableLabourWelfareFundData(statutoryComponentsLabourWelfareFundDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }


    [HttpPost]
    public async Task<IActionResult> SaveProfessionalTax(ProfessionalTaxDTO inputData)
    {
        string? employeeId = HttpContext.Session.GetString("EmployeeId");
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        ProfessionalTaxDTO viewModel = new ProfessionalTaxDTO();
        if (unitId != null)
        {
            inputData.UnitId = unitId ?? default(int);
            inputData.IsActive = true;
            if (inputData.ProfTaxId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy = employeeId;
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifiedBy = employeeId;
            }
            IActionResult actionResult = await _statutoryComponentAPIController.SaveProfessionalTax(inputData);
            //  return actionResult;

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.ProfTaxId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.ProfTaxId = 0;

            }
            else
                inputData.DisplayMessage = objResultData.ToString();

            //  outputData.ProfessionalTaxList = await GetProfessionalTaxList();
            inputData.ProfessionalTaxList = await GetProfessionalTaxList();
            inputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
            viewModel = inputData;
            return View("ProfTax", viewModel);


        }
        else
        {
            return Unauthorized();
        }


    }

    public async Task<List<ProfessionalTaxDTO>?> GetProfessionalTaxList()
    {

        IActionResult actionResult = await _statutoryComponentAPIController.GetProfessionalTaxes(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
        ObjectResult objResult = (ObjectResult)actionResult;

        List<ProfessionalTaxDTO> objResultData = (List<ProfessionalTaxDTO>)objResult.Value;
        foreach (var item in objResultData)
        {
            item.EncryptedId = CommonHelper.EncryptURLHTML(item.ProfTaxId.ToString());
            item.Gender = item.Gender.Trim();
        }
        return objResultData;
    }

    [HttpGet]
    [Route("StatutoryComponent/GetProfessinalTax/{eproftaxId}")]
    public async Task<IActionResult> GetProfessinalTax(string eproftaxId)
    {
        int proftaxId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eproftaxId));
        if (proftaxId != 0)
        {
            ProfessionalTaxDTO outputData = new ProfessionalTaxDTO();
            outputData.ProfTaxId = proftaxId;

            IActionResult actionResult;

            actionResult = await _statutoryComponentAPIController.GetProfessionalTax(outputData);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = (ProfessionalTaxDTO)objResult.Value;
            if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
            {
                objResultData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
                objResultData.ProfessionalTaxList = await GetProfessionalTaxList();
                return View("ProfTax", objResultData);
                //return RedirectToAction("Role","Role", objResultData);
            }
            else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
            {
                objResultData.ProfTaxId = 0;
                objResultData.ProfessionalTaxList = await GetProfessionalTaxList();
                objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
                return View("ProfTax", objResultData);
                //return RedirectToAction("Role", objResultData);
            }
        }
        return RedirectToAction("ProfTax", "StatutoryComponent");
    }

    [HttpGet]
    [Route("StatutoryComponent/DeleteProfTax/{eproftaxId}")]
    public async Task<IActionResult> DeleteProfTax(string eproftaxId)
    {
        int profTaxId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eproftaxId));
        if (profTaxId != 0)
        {

            ProfessionalTaxDTO outputData = new ProfessionalTaxDTO();
            outputData.ProfTaxId = profTaxId;

            IActionResult actionResult;

            actionResult = await _statutoryComponentAPIController.DeleteProfessionalTax(outputData);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;

            //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
            //{
            outputData.ProfTaxId = 0;
            outputData.ProfessionalTaxList = await GetProfessionalTaxList();
            outputData.DisplayMessage = "Transaction Successful!";
            return View("ProfTax", outputData);
            //}
        }
        return RedirectToAction("ProfTax", "StatutoryComponent");
    }

    public async Task<IActionResult> ProfTax()
    {
        ProfessionalTaxDTO outputData = new ProfessionalTaxDTO();
        outputData.ProfessionalTaxList = await GetProfessionalTaxList();
        outputData.CountryList = await _mastersKeyValueController.CountryKeyValue(true);
        return View(outputData);
    }

    [HttpPost]
    public async Task<IActionResult> SaveComponentsTaxLimit(ComponentsTaxLimitDTO inputDTO)
    {
        // ComponentsTaxLimitDTO inputDTO = new ComponentsTaxLimitDTO();
        //inputDTO.PFLimit = PFLimit;
        //inputDTO.GratuityLimit = GratuityLimit;
        //inputDTO.LeaveEncashmentLimit = LeaveEncashmentLimit;
        inputDTO.CreatedOn = DateTime.Now;
        inputDTO.ModifiedOn = DateTime.Now;
        inputDTO.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        inputDTO.ModifiedBy = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        inputDTO.UnitId = HttpContext.Session.GetInt32("UnitId");

        IActionResult actionResult = await _statutoryComponentAPIController.SaveComponentsTaxLimit(inputDTO);
        //  return actionResult;

        ObjectResult objResult = (ObjectResult)actionResult;

        var objResultData = objResult.Value;
        inputDTO.HttpStatusCode = objResult.StatusCode;

        if (inputDTO.HttpStatusCode == 200)
        {
            if (inputDTO.TaxLimitId == 0)
                inputDTO.DisplayMessage = "Transaction Successful!";
            else
                inputDTO.DisplayMessage = "Transaction Successful!";
            inputDTO.TaxLimitId = 0;
            // inputDTO.ProfessionalTaxList = await GetProfessionalTaxList();
        }
        else
            inputDTO.DisplayMessage = objResultData.ToString();

        //    IActionResult actionResult;

        //actionResult = await _statutoryComponentAPIController.GetComponentsTaxLimit(outputData);
        //ObjectResult objResult = (ObjectResult)actionResult;
        //var objResultData = (ComponentsTaxLimitDTO)objResult.Value;

        //viewModel = inputData;
        return View("ComponentTaxLimit", inputDTO);

        //var res = await _statutoryComponentAPIController.SaveComponentsTaxLimit(inputDTO);
        //return res;
    }


}
