using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.WebUI.Controllers.QueryManagement;

public class QueryManagementController : Controller
{
    //private readonly rolesandpermissions.RolesController _rolesAPIController;
    //private readonly ClientController _clientAPIController;
    public QueryManagementController()
    {

    }

    public async Task<IActionResult> AddComplaint()
    {
        return View();
    }
    public async Task<IActionResult> ComplaintList()
    {
        return View();
    }

}
