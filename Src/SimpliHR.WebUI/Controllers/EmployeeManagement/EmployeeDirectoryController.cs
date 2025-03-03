using Masters.Controllers;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Leave;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Masters;
namespace SimpliHR.WebUI.Controllers.EmployeeManagement;


public class EmployeeDirectoryController : Controller
{
    private readonly EmployeeDirectoryAPIController _directoryAPIController;


    public EmployeeDirectoryController(EmployeeDirectoryAPIController directoryAPIController)
    {

        _directoryAPIController = directoryAPIController;


    }
    public async Task<IActionResult> EmployeeDirectoryList()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        MainDirectoryDTO DirectoryDetails = new MainDirectoryDTO();
        DirectoryDetails = await _directoryAPIController.GetEmployeeDirectoryDetails(unitId);
        return View(DirectoryDetails);
    }

    [HttpGet]
    public async Task<MainDirectoryDTO> GetActiveDirectoryList()
    {
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        MainDirectoryDTO DirectoryDetails = new MainDirectoryDTO();
        DirectoryDetails = await _directoryAPIController.GetEmployeeDirectoryDetails(unitId);
        return DirectoryDetails;
    }

    [HttpPost]
    public async Task<IActionResult> SaveEmployeeDirectory(EmployeeDirectoryAction userAction)
    {
        userAction.UnitId = HttpContext.Session.GetInt32("UnitId");
        EmployeeDirectoryAction quickAccessVM = new EmployeeDirectoryAction();
        //  userAction.ApprovedBy = employeeId;
        // userAction.TicketId= "Leave/" + DateTime.Now.Month + "/" + GenerateTicket(6);
        if (userAction.EmployeeDirectoryIds != null && userAction.EmployeeDirectoryIds.Length != 0)
        {
            string sRetMsg = await _directoryAPIController.SaveEmployeeDirectory(userAction);
            quickAccessVM.DisplayMessage = sRetMsg;
        }
        // else
        //   manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
        return Ok(quickAccessVM.DisplayMessage);
    }

    [HttpGet]
    public async Task<members> GetEmployeeDirectory()
    {
        members outputData = new members();
        //  outputData.EmployeeBirthDays = (List<EmployeeDashboardDetailsDTO>)await _employeeAPIController.GetEmployeeBirthDayInfo(outputData.EmployeeMaster);
        IActionResult actionResult = await _directoryAPIController.GetEmployeeDirectory(HttpContext.Session.GetInt32("UnitId"));
        ObjectResult objResult = (ObjectResult)actionResult;
        outputData.membersList = (List<members>)objResult.Value;
        foreach (var item in outputData.membersList)
        {
            if (item.ProfileImage != null)
                item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
        }
          return outputData;
    }

    [HttpGet]
    public async Task<int> PositionIsExist(int positionId)
    {
       int? UnitId = HttpContext.Session.GetInt32("UnitId");
        var status = await _directoryAPIController.PositionIsExit(UnitId, positionId);

        return status;


        //  return RedirectToAction("Ticket", "List");

    }

    [HttpGet]
    public async Task<EmployeeDirectoryCardDetailsDTO> GetEmployeeDirectoryDetails(int employeeId)
    {
        EmployeeDirectoryCardDetailsDTO outputData = new EmployeeDirectoryCardDetailsDTO();
        //  outputData.EmployeeBirthDays = (List<EmployeeDashboardDetailsDTO>)await _employeeAPIController.GetEmployeeBirthDayInfo(outputData.EmployeeMaster);
        IActionResult actionResult = await _directoryAPIController.GetEmployeeDirectoryDetails((int)HttpContext.Session.GetInt32("UnitId"), employeeId);
        ObjectResult objResult = (ObjectResult)actionResult;
        outputData.EmployeeCardDetails = (List<EmployeeDirectoryCardDetailsDTO>)objResult.Value;
        foreach (var item in outputData.EmployeeCardDetails)
        {
            if (item.ProfileImage != null)
                item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
        }
        return outputData;
    }

}

