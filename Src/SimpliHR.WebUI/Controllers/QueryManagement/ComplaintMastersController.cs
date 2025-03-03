using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.QueryManagement;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Complaint;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Page;
using SimpliHR.Infrastructure.ViewModels.QueryManagement;

namespace SimpliHR.WebUI.Controllers.QueryManagement;

public class ComplaintMastersController : Controller
{
    private readonly ComplaintMastersAPIController _complaintMastersAPIController;
    private readonly QueryManagementMastersAPIController _queryManagementMastersAPIController;

    public ComplaintMastersController(ComplaintMastersAPIController complaintMastersAPIController, QueryManagementMastersAPIController queryManagementMastersAPIController)
    {
        _complaintMastersAPIController = complaintMastersAPIController;
        this._queryManagementMastersAPIController = queryManagementMastersAPIController;
    }

    public IActionResult ComplaintPriorityMaster()
    {
        return View();
    }
    public IActionResult ComplaintStatusMaster()
    {
        return View();
    }
    public IActionResult ComplaintCategoryMaster()
    {
        return View();
    }
    public async Task<IActionResult> ComplaintCategoryAddPartialView([FromBody] ComplaintCategoryDTO inputDTO)
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
        ComplaintCategoryDTO? dto = new ComplaintCategoryDTO();
        if (!String.IsNullOrEmpty(inputDTO.encId))
        {
            inputDTO.Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encId));
            var res = await _queryManagementMastersAPIController.GetComplaintCategoryById(inputDTO.Id);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (ComplaintCategoryDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (dto != null)
                    {
                        dto.encId = CommonHelper.EncryptURLHTML(dto.Id.ToString());
                    }
                }
            }
        }
        return PartialView("_complaintCategoryMaster/_add", dto);
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
        ComplaintCategoryViewModel dto = new ComplaintCategoryViewModel();

        var res = await _queryManagementMastersAPIController.GetComplaintCategoryList(empSession.UnitId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.ComplaintCategories = (List<ComplaintCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (dto.ComplaintCategories != null)
                {
                    foreach (var item in dto.ComplaintCategories)
                    {
                        item.encId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                    }
                }
            }
        }
        return PartialView("_complaintCategoryMaster/_list", dto);
    }
}
