using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Page;

namespace SimpliHR.WebUI.Controllers.Masters;

public class AnnouncementTypeMasterController : Controller
{

    private readonly AnnouncementTypeMasterAPIController _announcementTypeMasterAPIController;
    public AnnouncementTypeMasterController(AnnouncementTypeMasterAPIController announcementTypeMasterAPIController)
    {
        _announcementTypeMasterAPIController = announcementTypeMasterAPIController;
    }
    public IActionResult AnnounceTypeMaster()
    {
        return View();
    }

    public async Task<IActionResult> AddPartialView([FromBody] AnnouncementTypeMasterDTO inputDTO)
    {
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (String.IsNullOrEmpty(strEmpSession))
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        if (inputDTO == null)
        {
            Error error = new Error();
            error.Heading = "Invalid Data";
            error.Message = "Please refresh the page and try again";
            error.ButtonMessage = "Go back to previous page";
            error.ButtonURL = "javascript:history.go(-1)";
            return View("../Page/Error", error);
        }
        AnnouncementTypeMasterDTO? dto = new AnnouncementTypeMasterDTO();
        if (!String.IsNullOrEmpty(inputDTO.encAnnouncementTypeId))
        {
            inputDTO.AnnouncementTypeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encAnnouncementTypeId));
            var res = await _announcementTypeMasterAPIController.GetAnnouncementTypeById(inputDTO.AnnouncementTypeId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (AnnouncementTypeMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (dto != null)
                    {
                        dto.encAnnouncementTypeId = CommonHelper.EncryptURLHTML(dto.AnnouncementTypeId.ToString());
                    }
                }
            }
        }
        return PartialView("_announcementTypeMaster/_add", dto);
    }
    public async Task<IActionResult> ListPartialView()
    {
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (String.IsNullOrEmpty(strEmpSession))
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
        if (empSession == null)
        {
            Error error = new Error();
            error.Heading = "Session has expired";
            error.Message = "Please login again to resume your session";
            error.ButtonMessage = "Go To Login Page";
            error.ButtonURL = "/Account/Login";
            return View("../Page/Error", error);
        }
        AnnouncementTypeViewModel dto = new AnnouncementTypeViewModel();

        var res = await _announcementTypeMasterAPIController.GetAnnouncementTypeList(empSession.UnitId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.AnnouncementTypeMasterList = (List<AnnouncementTypeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (dto.AnnouncementTypeMasterList != null)
                {
                    foreach (var item in dto.AnnouncementTypeMasterList)
                    {
                        item.encAnnouncementTypeId = CommonHelper.EncryptURLHTML(item.AnnouncementTypeId.ToString());
                    }
                }
            }
        }
        return PartialView("_announcementTypeMaster/_list", dto);
    }
    public async Task<IActionResult> Save([FromBody] AnnouncementTypeMasterDTO inputDTO)
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (String.IsNullOrEmpty(strEmpSession))
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }

            if (inputDTO == null)
            {
                throw new Exception("Data is not valid");
            }
            if (String.IsNullOrEmpty(inputDTO.encAnnouncementTypeId))
            {
                inputDTO.UnitId = empSession.UnitId ?? default(int);
                inputDTO.IsActive = true;
                inputDTO.CreatedDate = DateTime.Now;
                inputDTO.CreatedBy = empSession.EmployeeId;
            }
            else
            {
                inputDTO.UnitId = empSession.UnitId ?? default(int);
                inputDTO.AnnouncementTypeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encAnnouncementTypeId));
                inputDTO.ModifiedDate = DateTime.Now;
                inputDTO.ModifiedBy = empSession.EmployeeId;
                inputDTO.IsActive = true;
            }
            var res = await _announcementTypeMasterAPIController.Save(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
    public async Task<IActionResult> Delete([FromBody] AnnouncementTypeMasterDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception("Data is invalid");
            }
            if (String.IsNullOrEmpty(inputDTO.encAnnouncementTypeId))
            {
                throw new Exception("Data is invalid");
            }
            inputDTO.AnnouncementTypeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encAnnouncementTypeId));
            var res = await _announcementTypeMasterAPIController.DeleteAnnouncementTypeById(inputDTO.AnnouncementTypeId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
