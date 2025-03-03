using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Payroll;

namespace SimpliHR.WebUI.Controllers.Payroll;

public class SalaryTemplateController : Controller
{
    private readonly SalaryTemplateAPIController _salaryTemplateAPIController;  
    private readonly EarningComponentAPIController _payrollController;
    public SalaryTemplateController(SalaryTemplateAPIController salaryTemplateAPIController, EarningComponentAPIController leaveAPIController)
    {
        _salaryTemplateAPIController = salaryTemplateAPIController;     
        _payrollController = leaveAPIController;
    }


    public async Task<IActionResult> SalaryTemplate(string SalaryTemplateId = null, string Type = null)
    {
        SalaryTemplateDTOForSave masterDTO = new SalaryTemplateDTOForSave();
        //IActionResult actionResult = await _salaryComponentMasterAPIController.GetSalaryComponents(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
        //ObjectResult objectResult = (ObjectResult)actionResult;
        //masterDTO.SalaryComponentMasterListDTO = (List<SalaryComponentMasterDTO>)objectResult.Value;
        masterDTO.UnitId= HttpContext.Session.GetInt32("UnitId");
        masterDTO = await _payrollController.GetSalaryComponents(masterDTO, 1000, 0);
       // masterDTO = await _payrollController.GetDeductionComponents(masterDTO, 1000, 0);
        if (SalaryTemplateId == null)
        {

        }
        else
        {
            int salaryTemplateId = Convert.ToInt32(CommonHelper.DecryptURLHTML(SalaryTemplateId));
            string type = Convert.ToString(CommonHelper.DecryptURLHTML(Type));
            if (type == "Edit")
                masterDTO.SalaryTemplateId = salaryTemplateId;
            else
                masterDTO.SalaryTemplateId = 0;
            IActionResult actionResultmapping = await _salaryTemplateAPIController.GetSalaryTemplateComponentMapping(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, salaryTemplateId);
            ObjectResult objectResultmapping = (ObjectResult)actionResultmapping;
            List<SalaryTemplateComponentsMappingDTO> salaryTemplateComponentsMappingDTOs = (List<SalaryTemplateComponentsMappingDTO>)objectResultmapping.Value;
            int[] salaryComponents = new int[salaryTemplateComponentsMappingDTOs.Count];
            for (int i = 0; i < salaryTemplateComponentsMappingDTOs.Count; i++)
            {
                salaryComponents[i] = salaryTemplateComponentsMappingDTOs[i].SalaryComponentId ?? default(int);
            }
            masterDTO.SalaryComponentIds = salaryComponents;


            IActionResult actionResultSalaryTemplate = await _salaryTemplateAPIController.GetSalaryTemplateById(salaryTemplateId);
            ObjectResult objectResultSalaryTemplate = (ObjectResult)actionResultSalaryTemplate;
            SalaryTemplateDTO salaryTemplateDTO = (SalaryTemplateDTO)objectResultSalaryTemplate.Value;

            masterDTO.TemplateName = salaryTemplateDTO.TemplateName;
            masterDTO.Description = salaryTemplateDTO.Description;
            masterDTO.AnnualCTC = salaryTemplateDTO.AnnualCtc;
        }
        return View(masterDTO);
    }

    public async Task<IActionResult> SalaryTemplateList()
    {
        int clientId;
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
        {
            List<SalaryTemplateDTO> salaryTemplateDTO = new List<SalaryTemplateDTO>();
            salaryTemplateDTO = await GetSalaryTemplateList(unitId ?? default(int));
            return View(salaryTemplateDTO);
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }
    }
    public async Task<List<SalaryTemplateDTO>?> GetSalaryTemplateList(int unitId)
    {

        IActionResult actionResult = await _salaryTemplateAPIController.GetSalaryTemplates(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
        ObjectResult objResult = (ObjectResult)actionResult;
        List<SalaryTemplateDTO> objResultData = (List<SalaryTemplateDTO>)objResult.Value;
        foreach (var item in objResultData)
        {
            item.EncryptedId = CommonHelper.Encrypt(Convert.ToString(item.SalaryTemplateId));
        }
        return objResultData;
    }


    [HttpPost]
    public ActionResult ManageSalaryTemplate(SalaryTemplateDTOForSave inputDTO)
    {
        try
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (unitId != null)
            {
                inputDTO.UnitId = unitId ?? default(int);

                if (inputDTO.SalaryTemplateId != null)
                {
                    if (inputDTO.SalaryTemplateId > 0)
                    {
                        SalaryTemplateDTO salaryTemplateDTO = new SalaryTemplateDTO();
                        salaryTemplateDTO.SalaryTemplateId = inputDTO.SalaryTemplateId ?? default(int);
                        salaryTemplateDTO.UnitId = inputDTO.UnitId;
                        salaryTemplateDTO.AnnualCtc = inputDTO.AnnualCTC;
                        salaryTemplateDTO.Description = inputDTO.Description;
                        salaryTemplateDTO.ModifiedDate = System.DateTime.Now;
                        salaryTemplateDTO.TemplateName = inputDTO.TemplateName;
                        salaryTemplateDTO.IsActive = true;
                        var resSalaryTemplateUpdate = _salaryTemplateAPIController.UpdateSalaryTemplate(salaryTemplateDTO);
                        if (resSalaryTemplateUpdate != null)
                        {
                            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resSalaryTemplateUpdate).StatusCode == 200)
                            {
                                var res = _salaryTemplateAPIController.DeleteSalaryTemplateComponentsMappingBySalaryTemplateId(inputDTO);
                                if (res != null)
                                {
                                    if (((Microsoft.AspNetCore.Mvc.StatusCodeResult)res).StatusCode == 200)
                                    {
                                        foreach (var item in inputDTO.SalaryComponentIds)
                                        {
                                            SalaryTemplateComponentsMappingDTO salaryTemplateComponentsMappingDTO = new SalaryTemplateComponentsMappingDTO();
                                            salaryTemplateComponentsMappingDTO.SalaryTemplateId = inputDTO.SalaryTemplateId;
                                            salaryTemplateComponentsMappingDTO.SalaryComponentId = item;
                                            salaryTemplateComponentsMappingDTO.UnitId = inputDTO.UnitId;
                                            salaryTemplateComponentsMappingDTO.SalaryComponentType = "E";
                                            salaryTemplateComponentsMappingDTO.IsActive = true;
                                            _salaryTemplateAPIController.SaveSalaryTemplateComponentsMapping(salaryTemplateComponentsMappingDTO);
                                        }
                                        return Ok(StatusCodes.Status200OK);
                                    }
                                    else
                                    {
                                        return (ActionResult)resSalaryTemplateUpdate;
                                    }
                                }
                                else
                                {
                                    throw new Exception("Some error has occurred. Please contact administrator");
                                }
                            }
                            else
                            {
                                return (ActionResult)resSalaryTemplateUpdate;
                            }
                        }
                        else
                        {
                            throw new Exception("Some error has occurred. Please contact administrator");
                        }
                    }
                    else
                    {
                        SalaryTemplateDTO salaryTemplateDTO = new SalaryTemplateDTO();
                        salaryTemplateDTO.TemplateName = inputDTO.TemplateName;
                        salaryTemplateDTO.AnnualCtc = inputDTO.AnnualCTC;
                        salaryTemplateDTO.UnitId = inputDTO.UnitId;
                        salaryTemplateDTO.Description = inputDTO.Description;
                        salaryTemplateDTO.IsActive = true;
                        var resSalaryTemplateSave = _salaryTemplateAPIController.SaveSalaryTemplate(salaryTemplateDTO);
                        if (resSalaryTemplateSave != null)
                        {
                            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resSalaryTemplateSave).StatusCode == 200)
                            {
                                IActionResult actionResult = (IActionResult)resSalaryTemplateSave;
                                ObjectResult objectResult = (ObjectResult)actionResult;
                                int templateId = Convert.ToInt32(objectResult.Value);
                                inputDTO.SalaryTemplateId = templateId;
                                var res = _salaryTemplateAPIController.DeleteSalaryTemplateComponentsMappingBySalaryTemplateId(inputDTO);
                                if (res != null)
                                {
                                    if (((Microsoft.AspNetCore.Mvc.StatusCodeResult)res).StatusCode == 200)
                                    {
                                        foreach (var item in inputDTO.SalaryComponentIds)
                                        {
                                            SalaryTemplateComponentsMappingDTO salaryTemplateComponentsMappingDTO = new SalaryTemplateComponentsMappingDTO();
                                            salaryTemplateComponentsMappingDTO.SalaryTemplateId = inputDTO.SalaryTemplateId;
                                            salaryTemplateComponentsMappingDTO.SalaryComponentId = item;
                                            salaryTemplateComponentsMappingDTO.IsActive = true;
                                            salaryTemplateComponentsMappingDTO.UnitId = inputDTO.UnitId;
                                            salaryTemplateComponentsMappingDTO.SalaryComponentType = "E";
                                            _salaryTemplateAPIController.SaveSalaryTemplateComponentsMapping(salaryTemplateComponentsMappingDTO);
                                        }
                                        return Ok(StatusCodes.Status200OK);
                                    }
                                    else
                                    {
                                        return (ActionResult)resSalaryTemplateSave;
                                    }
                                }
                                else
                                {
                                    throw new Exception("Some error has occurred. Please contact administrator");
                                }
                            }
                            else
                            {
                                return (ActionResult)resSalaryTemplateSave;
                            }
                        }
                        else
                        {
                            throw new Exception("Some error has occurred. Please contact administrator");
                        }
                    }
                }
                else
                {
                    throw new Exception("Some error has occurred. Please contact administrator");
                }
            }
            else
            {
                return RedirectToAction("/Account/Login");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }

    }
    [HttpGet]
    [Route("SalaryTemplate/DeleteSalaryTemplate/{eSalaryTemplateId}")]
    public async Task<IActionResult> DeleteSalaryTemplate(string eSalaryTemplateId)
    {
        int salaryTemplateId = 0;
        try
        {
            salaryTemplateId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eSalaryTemplateId));
        }
        catch (Exception ex)
        {
            return RedirectToAction("Login", "Account");
        }
        if (salaryTemplateId != 0)
        {

            SalaryTemplateDTO outputData = new SalaryTemplateDTO();
            outputData.SalaryTemplateId = salaryTemplateId;
            var res = await _salaryTemplateAPIController.DeleteSalaryTemplate(outputData);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
}
