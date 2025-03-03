using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.EmployeeSocialActivity;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientAdmin;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Page;
using SimpliHR.Webui.Modals.Account;

namespace SimpliHR.WebUI.Controllers;

public class AdminController : Controller
{
    private readonly EmployeeMasterController _employeeMasterController;
    private readonly DepartmentMasterController _departmentMasterAPIController;
    private readonly WorkLocationMasterController _workLocationMasterAPIController;

    public AdminController(EmployeeMasterController employeeMasterController, DepartmentMasterController departmentMasterAPIController, WorkLocationMasterController workLocationMasterAPIController)
    {
        _employeeMasterController = employeeMasterController;
        _departmentMasterAPIController = departmentMasterAPIController;
        _workLocationMasterAPIController = workLocationMasterAPIController;
    }
    public IActionResult Index_H()
    {
        return View();
    }
    public IActionResult Index_V()
    {
        return View();
    }
    public async Task<IActionResult> Index()
    {
        ClientAdminDashboard dto = new ClientAdminDashboard();
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (!string.IsNullOrEmpty(strEmpSession))
        {
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(strEmpSession));
            if (empSession != null)
            {
                int? unitId = empSession.UnitId;
                dto.Departments = _departmentMasterAPIController.GetDepartmentsByUnitId(unitId ?? default(int));
                Core.Helper.RequestParams requestParams = new Core.Helper.RequestParams();
                requestParams.IsActive = true;
                var res = await _workLocationMasterAPIController.GetWorkLocations(requestParams, unitId ?? default(int));
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.WorkLocations = (List<WorkLocationMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
        }
        return View(dto);
    }
    public IActionResult Index1()
    {
        return View();

    }
    public IActionResult ManagerDashbaord()
    {
        return View();

    }
    public IActionResult Pricing()
    {
        return View();
    }
    public IActionResult Home()
    {
        return View();
    }
    
    public IActionResult ChangeHtV()
    {
        ThemeType.themeO = "V";
        return RedirectToAction("Index");
    }
    public IActionResult ChangeVtH()
    {
        ThemeType.themeO = "H";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> GetActiveEmployee()
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                return Unauthorized("Session has expired");
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                return Unauthorized("Session has expired");
            }

            ClientAdminDashboard clientAdminDashboard = new ClientAdminDashboard();

            var resEmployeeList = await _employeeMasterController.GetEmployeeForClientDashboardStats(empSession.UnitId ?? default(int));
            var resEmployeeExperience = await _employeeMasterController.GetEmployeeExperienceForDasboard(empSession.UnitId ?? default(int));
            var resEmployeeExitList = await _employeeMasterController.GetEmployeeExitListForDasboard(empSession.UnitId ?? default(int));
            var resCurrentDateEmployeeStats = await _employeeMasterController.CurrentDateEmployeeStats(empSession.UnitId ?? default(int));

            if (resEmployeeList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeList).StatusCode == 200)
            {
                clientAdminDashboard.EmployeeList = (List<EmployeeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeList).Value;
            }
            if (resEmployeeExperience != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeExperience).StatusCode == 200)
            {
                clientAdminDashboard.EmployeeExperienceList = (List<EmployeeExperienceDetailDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeExperience).Value;
            }
            if (resEmployeeExitList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeExitList).StatusCode == 200)
            {
                clientAdminDashboard.EmployeeExitList = (List<EmployeeExitResignationDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeExitList).Value;
            }
            if (resCurrentDateEmployeeStats != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCurrentDateEmployeeStats).StatusCode == 200)
            {
                clientAdminDashboard.CurrentDateEmployeeStatsDTO = (CurrentDateEmployeeStatsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCurrentDateEmployeeStats).Value;
            }

            return Ok(clientAdminDashboard);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> GetEmployeesBySalaryBandChartData()
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                return Unauthorized("Session has expired");
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                return Unauthorized("Session has expired");
            }

            ClientAdminDashboard clientAdminDashboard = new ClientAdminDashboard();

            var resEmployeeList = await _employeeMasterController.GetEmployeeForClientDashboardStats(empSession.UnitId ?? default(int));
            //var resBandMasterList = await _employeeMasterController.GetBandMasterForDasboard(empSession.UnitId ?? default(int));

            if (resEmployeeList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeList).StatusCode == 200)
            {
                clientAdminDashboard.EmployeeList = (List<EmployeeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeList).Value;
            }
            //if (resBandMasterList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resBandMasterList).StatusCode == 200)
            //{
            //    clientAdminDashboard.BandMasterList = (List<BandMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resBandMasterList).Value;
            //}

            return Ok(clientAdminDashboard);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> GetEmployeesByQualificationChartData()
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                return Unauthorized("Session has expired");
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                return Unauthorized("Session has expired");
            }

            ClientAdminDashboard clientAdminDashboard = new ClientAdminDashboard();

            var resEmployeeList = await _employeeMasterController.GetEmployeeForClientDashboardStats(empSession.UnitId ?? default(int));
            var resEmployeeQualificationList = await _employeeMasterController.GetEmployeeQualificationForDasboard(empSession.UnitId ?? default(int));
            var resAcademicMasterList = await _employeeMasterController.GetAcademicMasterForDasboard(empSession.UnitId ?? default(int));

            if (resEmployeeList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeList).StatusCode == 200)
            {
                clientAdminDashboard.EmployeeList = (List<EmployeeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeList).Value;
            }
            if (resEmployeeQualificationList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeQualificationList).StatusCode == 200)
            {
                clientAdminDashboard.EmployeeAcademicList = (List<EmployeeAcademicDetailDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeQualificationList).Value;
            }
            if (resAcademicMasterList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resAcademicMasterList).StatusCode == 200)
            {
                clientAdminDashboard.AcademicMasterList = (List<AcademicMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAcademicMasterList).Value;
            }

            return Ok(clientAdminDashboard);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> GetEmployeesByTenureChartData()
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                return Unauthorized("Session has expired");
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                return Unauthorized("Session has expired");
            }

            ClientAdminDashboard clientAdminDashboard = new ClientAdminDashboard();

            var resEmployeeList = await _employeeMasterController.GetEmployeeForClientDashboardStats(empSession.UnitId ?? default(int));

            if (resEmployeeList != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeList).StatusCode == 200)
            {
                clientAdminDashboard.EmployeeList = (List<EmployeeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resEmployeeList).Value;
            }

            return Ok(clientAdminDashboard);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> WageBillTrendChartData()
    {
        try
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                return Unauthorized("Session has expired");
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                return Unauthorized("Session has expired");
            }
            List<WageBillTrendDataDTO>? wageBillTrendDataDTOs = new List<WageBillTrendDataDTO>();
            await _employeeMasterController.UpdateWageBillTrendDataForDashboard(empSession.UnitId ?? default(int));
            var res = await _employeeMasterController.GetWageBillTrendDataForDashboard(empSession.UnitId ?? default(int));

            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                wageBillTrendDataDTOs = (List<WageBillTrendDataDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }

            return Ok(wageBillTrendDataDTOs);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


}
