using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Exit;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Webui.Modals.Account;

namespace SimpliHR.WebUI.Controllers.Masters;

public class TemplateMasterController : Controller
{
    private readonly TemplateMasterAPIController _templateMasterAPIController;

    public TemplateMasterController(TemplateMasterAPIController templateMasterAPIController)
    {
        _templateMasterAPIController = templateMasterAPIController;
    }
    public IActionResult CreateTemplate(string? encId = null)
    {
        TemplateMasterDynamicDTO template = new TemplateMasterDynamicDTO();
        if (string.IsNullOrEmpty(encId))
        {

        }
        else
        {
            string decID = SimpliHR.Infrastructure.Helper.CommonHelper.DecryptURL(encId);
            if (decID != null)
            {
                int Id = Convert.ToInt32(decID);
                template.TemplateMasterDynamicId = Id;

            }
        }
        return View(template);
    }
    public IActionResult TemplateList()
    {
        return View();
    }
    public async Task<IActionResult> GetTemplateListPartial()
    {
        List<TemplateMasterDynamicDTO> dTOs = new List<TemplateMasterDynamicDTO>();
        var objRes = await _templateMasterAPIController.GetTemplateList();
        if (objRes != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
            {
                var res = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).Value;
                if (res != null)
                {
                    dTOs = (List<TemplateMasterDynamicDTO>)res;
                }
            }
        }
        return PartialView("_templateList/_list", dTOs);
    }
    public async Task<IActionResult> GetTemplateFormById([FromBody] TemplateMasterDynamicDTO inputDTO)
    {
        var res = await _templateMasterAPIController.GetTemplateFormById(inputDTO);
        return res;
    }

    public async Task<IActionResult> SaveTemplateFormComponent([FromBody] TemplateMasterDynamicDTO inputDTO)
    {
        if (inputDTO != null)
        {

            int? UserId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
            inputDTO.CreatedBy = UserId.ToString();
            inputDTO.CreatedDate = DateTime.Now;
            inputDTO.IsActive = true;

            var res = await _templateMasterAPIController.SaveTemplateFormComponent(inputDTO);

            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Some error has occurred. Please refresh the page and try again");
        }
    }
}
