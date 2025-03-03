using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.EmployeeSocialActivity;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Page;

namespace SimpliHR.WebUI.Controllers.Masters;

public class NewsTypeMasterController : Controller
{
    private readonly NewTypeMasterAPIController _newsTypeMasterAPIController;
    public NewsTypeMasterController(NewTypeMasterAPIController newsTypeMasterAPIController)
    {
        _newsTypeMasterAPIController = newsTypeMasterAPIController;
    }
    public IActionResult NewsTypeMaster()
    {
        return View();
    }
    public async Task<IActionResult> AddPartialView([FromBody] NewsCategoryTagMasterDTO inputDTO)
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
        NewsCategoryTagMasterDTO? dto = new NewsCategoryTagMasterDTO();
        if (!String.IsNullOrEmpty(inputDTO.encNewsCategoryTagId))
        {
            inputDTO.NewsCategoryTagId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encNewsCategoryTagId));
            var res = await _newsTypeMasterAPIController.GetNewsTypeById(inputDTO.NewsCategoryTagId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (NewsCategoryTagMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (dto != null)
                    {
                        dto.encNewsCategoryTagId = CommonHelper.EncryptURLHTML(dto.NewsCategoryTagId.ToString());
                    }
                }
            }
        }
        return PartialView("_newsTypeMaster/_add", dto);
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
        NewsCategoryTagViewModel dto = new NewsCategoryTagViewModel();

        var res = await _newsTypeMasterAPIController.GetNewsTypeList(empSession.UnitId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.NewsCategoryTagList = (List<NewsCategoryTagMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (dto.NewsCategoryTagList != null)
                {
                    foreach (var item in dto.NewsCategoryTagList)
                    {
                        item.encNewsCategoryTagId = CommonHelper.EncryptURLHTML(item.NewsCategoryTagId.ToString());
                    }
                }
            }
        }
        return PartialView("_newsTypeMaster/_list", dto);
    }
    public async Task<IActionResult> Save([FromBody] NewsCategoryTagMasterDTO inputDTO)
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
            if (String.IsNullOrEmpty(inputDTO.encNewsCategoryTagId))
            {
                inputDTO.UnitId = empSession.UnitId ?? default(int);
                inputDTO.IsActive = true;
                inputDTO.CreatedDate = DateTime.Now;
                inputDTO.CreatedBy = empSession.EmployeeId;
            }
            else
            {
                inputDTO.UnitId = empSession.UnitId ?? default(int);
                inputDTO.NewsCategoryTagId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encNewsCategoryTagId));
                inputDTO.ModifiedDate = DateTime.Now;
                inputDTO.ModifiedBy = empSession.EmployeeId;
                inputDTO.IsActive = true;
            }
            var res = await _newsTypeMasterAPIController.Save(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
    public async Task<IActionResult> Delete([FromBody] NewsCategoryTagMasterDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception("Data is invalid");
            }
            if (String.IsNullOrEmpty(inputDTO.encNewsCategoryTagId))
            {
                throw new Exception("Data is invalid");
            }
            inputDTO.NewsCategoryTagId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encNewsCategoryTagId));
            var res = await _newsTypeMasterAPIController.DeleteNewsTypeById(inputDTO.NewsCategoryTagId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
