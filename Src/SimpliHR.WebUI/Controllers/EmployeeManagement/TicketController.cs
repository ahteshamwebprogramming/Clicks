using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpliHR.Endpoints.Leave;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Leave;

namespace SimpliHR.WebUI.Controllers.Employee;

public class TicketController : Controller
{
    private readonly EmployeeTicketController _employeeTicketController;
    public TicketController(EmployeeTicketController employeeTicketController)
    {
        _employeeTicketController = employeeTicketController;


    }
    public async Task<IActionResult> List()
    {
        EmployeeTicketsViewDTO viewModel = new EmployeeTicketsViewDTO();
        EmployeeTicketsInputs inputData = new EmployeeTicketsInputs();

        var roletype = HttpContext.Session.GetString("RoleType");
        bool isClient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        if (roletype.Trim() == "U")
        {
            inputData.MgrId = 0;
            inputData.EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        }
        else
        {
            inputData.MgrId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
            inputData.EmployeeId = 0;
        }
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        inputData.TicketId = "";
        inputData.StartDate = "";
        inputData.EndDate = "";
        inputData.Status = 0;
        inputData.ModuleId = 0;
        if (isClient)
            inputData.IsAdmin = 1;
        else
            inputData.IsAdmin = 0;

        IActionResult actionResultBirth = await _employeeTicketController.GetEmployeeTickets(inputData);
        ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
        viewModel.EmployeeToMeTickets = (List<EmployeeTicketsViewDTO>)objResultBirth.Value;

        return View(viewModel);
    }
    // [HttpGet]
    [HttpPost]
    public async Task<IActionResult> GetMyTicketsList(EmployeeTicketsInputs inputData)
    {
        bool isClient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        inputData.EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        inputData.MgrId = 0;
        inputData.TicketId = "";
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        if (string.IsNullOrEmpty(inputData.StartDate))
        {
            inputData.StartDate = "";
            inputData.EndDate = "";
        }
        if (isClient)
            inputData.IsAdmin = 1;
        else
            inputData.IsAdmin = 0;

        EmployeeTicketsViewDTO viewModel = new EmployeeTicketsViewDTO();
        IActionResult actionResultBirth = await _employeeTicketController.GetEmployeeTickets(inputData);
        ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
        viewModel.EmployeeTickets = (List<EmployeeTicketsViewDTO>)objResultBirth.Value;
        return View("List", viewModel);

    }

    [HttpGet]
    public async Task<EmployeeTicketsViewDTO> GetMyTickets(int? moduleId, int? status)
    {
        EmployeeTicketsInputs inputData = new EmployeeTicketsInputs();
        bool isClient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        inputData.EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        inputData.MgrId = 0;
        inputData.TicketId = "";
        inputData.ModuleId = moduleId;
        inputData.Status = status;
        if (string.IsNullOrEmpty(inputData.StartDate))
        {
            inputData.StartDate = "";
            inputData.EndDate = "";
        }
        if (isClient)
            inputData.IsAdmin = 1;
        else
            inputData.IsAdmin = 0;

        EmployeeTicketsViewDTO viewModel = new EmployeeTicketsViewDTO();
        IActionResult actionResult = await _employeeTicketController.GetEmployeeTickets(inputData);
        ObjectResult objResult = (ObjectResult)actionResult;
        viewModel.EmployeeTickets = (List<EmployeeTicketsViewDTO>)objResult.Value;
        return viewModel;

    }

    [HttpPost]
    public async Task<IActionResult> GetToMeList(EmployeeTicketsInputs inputData)
    {
        bool isClient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        inputData.EmployeeId = 0;
        inputData.Status = 0;
        inputData.MgrId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        if (string.IsNullOrEmpty(inputData.TicketId))
            inputData.TicketId = "";

        if (string.IsNullOrEmpty(inputData.StartDate))
        {
            inputData.StartDate = "";
            inputData.EndDate = "";
        }
        if (isClient)
            inputData.IsAdmin = 1;
        else
            inputData.IsAdmin = 0;

        EmployeeTicketsViewDTO viewModel = new EmployeeTicketsViewDTO();
        IActionResult actionResultBirth = await _employeeTicketController.GetEmployeeTickets(inputData);
        ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
        viewModel.EmployeeToMeTickets = (List<EmployeeTicketsViewDTO>)objResultBirth.Value;
        return View("List", viewModel);

    }

    [HttpPost]
    public async Task<IActionResult> GetByMeList(EmployeeTicketsInputs inputData)
    {
        bool isClient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        inputData.EmployeeId = 0;
        // inputData.Status = 0;
        inputData.MgrId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        if (string.IsNullOrEmpty(inputData.TicketId))
            inputData.TicketId = "";

        if (string.IsNullOrEmpty(inputData.StartDate))
        {
            inputData.StartDate = "";
            inputData.EndDate = "";
        }
        if (isClient)
            inputData.IsAdmin = 1;
        else
            inputData.IsAdmin = 0;

        EmployeeTicketsViewDTO viewModel = new EmployeeTicketsViewDTO();
        IActionResult actionResultBirth = await _employeeTicketController.GetEmployeeTickets(inputData);
        ObjectResult objResultBirth = (ObjectResult)actionResultBirth;
        viewModel.EmployeeByMeTickets = (List<EmployeeTicketsViewDTO>)objResultBirth.Value;
        return View("List", viewModel);

    }

    [HttpGet]
    public async Task<EmployeeTicketsViewDTO> ActionedByMe(int? moduleId, int? status)
    {
        EmployeeTicketsInputs inputData = new EmployeeTicketsInputs();
        bool isClient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
        inputData.EmployeeId = 0;
        inputData.ModuleId = moduleId;
        inputData.Status = status;
        inputData.MgrId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        if (string.IsNullOrEmpty(inputData.TicketId))
            inputData.TicketId = "";

        if (string.IsNullOrEmpty(inputData.StartDate))
        {
            inputData.StartDate = "";
            inputData.EndDate = "";
        }
        if (isClient)
            inputData.IsAdmin = 1;
        else
            inputData.IsAdmin = 0;

        EmployeeTicketsViewDTO viewModel = new EmployeeTicketsViewDTO();
        IActionResult actionResult = await _employeeTicketController.GetEmployeeTickets(inputData);
        ObjectResult objResult = (ObjectResult)actionResult;
        viewModel.EmployeeByMeTickets = (List<EmployeeTicketsViewDTO>)objResult.Value;
        return viewModel;

    }



}
