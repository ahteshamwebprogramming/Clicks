using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectTracker.Controllers;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.ProjectTracker.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Page;
using SimpliHR.Infrastructure.Models.ProjectTracker;

namespace SimpliHR.WebUI.Controllers.ProjectTracker;

public class ProjectTrackerMastersController : Controller
{
    private readonly ProjectCategoryMasterAPIController _projectCategoryMasterAPIController;
    private readonly PriorityMasterAPIController _priorityMasterAPIController;
    public ProjectTrackerMastersController(ProjectCategoryMasterAPIController projectCategoryMasterAPIController, PriorityMasterAPIController priorityMasterAPIController)
    {
        _projectCategoryMasterAPIController = projectCategoryMasterAPIController;
        _priorityMasterAPIController = priorityMasterAPIController;
    }

    #region Project Category
    public IActionResult ProjectCategoryMaster()
    {
        return View();
    }
    public async Task<IActionResult> AddProjectCategoryPartialView([FromBody] PTProjectCategoryDTO inputDTO)
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
        PTProjectCategoryDTO? dto = new PTProjectCategoryDTO();
        if (!String.IsNullOrEmpty(inputDTO.encCategoryID))
        {
            inputDTO.CategoryID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encCategoryID));
            var res = await _projectCategoryMasterAPIController.GetProjectCategoryById(inputDTO.CategoryID);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (PTProjectCategoryDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (dto != null)
                    {
                        dto.encCategoryID = CommonHelper.EncryptURLHTML(dto.CategoryID.ToString());
                    }
                }
            }
        }
        return PartialView("_projectCategoryMaster/_add", dto);
    }
    public async Task<IActionResult> ListProjectCategoryPartialView()
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
        PTProjectCategoryMasterViewModel dto = new PTProjectCategoryMasterViewModel();

        var res = await _projectCategoryMasterAPIController.GetProjectCategoryList(empSession.UnitId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.projectCategoryList = (List<PTProjectCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (dto.projectCategoryList != null)
                {
                    foreach (var item in dto.projectCategoryList)
                    {
                        item.encCategoryID = CommonHelper.EncryptURLHTML(item.CategoryID.ToString());
                    }
                }
            }
        }
        return PartialView("_projectCategoryMaster/_list", dto);
    }
    public async Task<IActionResult> SaveProjectCategory([FromBody] PTProjectCategoryDTO inputDTO)
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
            if (String.IsNullOrEmpty(inputDTO.encCategoryID))
            {
                inputDTO.UnitId = empSession.UnitId ?? default(int);
                inputDTO.IsActive = true;
                inputDTO.CreatedDate = DateTime.Now;
                inputDTO.CreatedBy = empSession.EmployeeId;
            }
            else
            {
                inputDTO.UnitId = empSession.UnitId ?? default(int);
                inputDTO.CategoryID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encCategoryID));
                inputDTO.ModifiedDate = DateTime.Now;
                inputDTO.ModifiedBy = empSession.EmployeeId;
                inputDTO.IsActive = true;
            }
            var res = await _projectCategoryMasterAPIController.Save(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
    public async Task<IActionResult> DeleteProjectCategory([FromBody] PTProjectCategoryDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception("Data is invalid");
            }
            if (String.IsNullOrEmpty(inputDTO.encCategoryID))
            {
                throw new Exception("Data is invalid");
            }
            inputDTO.CategoryID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encCategoryID));
            var res = await _projectCategoryMasterAPIController.DeleteProjectCategoryById(inputDTO.CategoryID);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region Project Priority
    public IActionResult ProjectPriorityMaster()
    {
        return View();
    }
    public async Task<IActionResult> AddProjectPriorityPartialView([FromBody] PTProjectPriorityDTO inputDTO)
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
        PTProjectPriorityDTO? dto = new PTProjectPriorityDTO();
        if (!String.IsNullOrEmpty(inputDTO.encPriorityId))
        {
            inputDTO.PriorityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encPriorityId));
            var res = await _priorityMasterAPIController.GetProjectPriorityById(inputDTO.PriorityId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (PTProjectPriorityDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (dto != null)
                    {
                        dto.encPriorityId = CommonHelper.EncryptURLHTML(dto.PriorityId.ToString());
                    }
                }
            }
        }
        return PartialView("_projectPriorityMaster/_add", dto);
    }
    public async Task<IActionResult> ListProjectPriorityPartialView()
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
        PTProjectPriorityMasterViewModel dto = new PTProjectPriorityMasterViewModel();

        var res = await _priorityMasterAPIController.GetProjectPriorityList(empSession.UnitId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.projectPriorityList = (List<PTProjectPriorityDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                if (dto.projectPriorityList != null)
                {
                    foreach (var item in dto.projectPriorityList)
                    {
                        item.encPriorityId = CommonHelper.EncryptURLHTML(item.PriorityId.ToString());
                    }
                }
            }
        }
        return PartialView("_projectPriorityMaster/_list", dto);
    }
    public async Task<IActionResult> SaveProjectPriority([FromBody] PTProjectPriorityDTO inputDTO)
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
            if (String.IsNullOrEmpty(inputDTO.encPriorityId))
            {
                inputDTO.UnitId = empSession.UnitId ?? default(int);
                inputDTO.IsActive = true;
                inputDTO.CreatedDate = DateTime.Now;
                inputDTO.CreatedBy = empSession.EmployeeId;
            }
            else
            {
                inputDTO.UnitId = empSession.UnitId ?? default(int);
                inputDTO.PriorityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encPriorityId));
                inputDTO.ModifiedDate = DateTime.Now;
                inputDTO.ModifiedBy = empSession.EmployeeId;
                inputDTO.IsActive = true;
            }
            var res = await _priorityMasterAPIController.Save(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
    public async Task<IActionResult> DeleteProjectPriority([FromBody] PTProjectPriorityDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception("Data is invalid");
            }
            if (String.IsNullOrEmpty(inputDTO.encPriorityId))
            {
                throw new Exception("Data is invalid");
            }
            inputDTO.PriorityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encPriorityId));
            var res = await _priorityMasterAPIController.DeleteProjectPriorityById(inputDTO.PriorityId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion
}
