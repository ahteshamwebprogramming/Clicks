using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Exit;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.ProfileEditAuth;
using SimpliHR.Endpoints.TicketMaster;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.ProfileEditAuth;

namespace SimpliHR.WebUI.Controllers.Masters;

public class ProfileEditAuthController : Controller
{
    private readonly ProfileEditAuthAPIController _profileEditAuthAPIController;
    private readonly TicketMasterAPIController _ticketMasterAPIController;
    public ProfileEditAuthController(ProfileEditAuthAPIController profileEditAuthAPIController, TicketMasterAPIController ticketMasterAPIController)
    {
        _profileEditAuthAPIController = profileEditAuthAPIController;
        _ticketMasterAPIController = ticketMasterAPIController;
    }
    public IActionResult AuthPage()
    {
        return View();
    }
    public async Task<IActionResult> AuthPageTable()
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

        int UnitId = empSession.UnitId ?? default(int);
        var res = await _profileEditAuthAPIController.GetProfileEditAuthTable(UnitId);
        List<ProfileEditAuthDTO> dto = new List<ProfileEditAuthDTO>();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto = (List<ProfileEditAuthDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_profileEditAuth/_authPageTable", dto);
    }
    public async Task<IActionResult> ProfileChangeRequest()
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

        int UnitId = empSession.UnitId ?? default(int);
        var res = await _ticketMasterAPIController.GetTickets(UnitId);
        List<TicketMasterDTO> dto = new List<TicketMasterDTO>();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto = (List<TicketMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }

        return View(dto);
    }
    public async Task<IActionResult> SaveProfileEditAuth([FromBody] ProfileEditAuthViewModel inputDTO)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

        inputDTO.UnitId = empSession.UnitId ?? default(int);

        var res = await _profileEditAuthAPIController.SaveProfileEditAuth(inputDTO);

        //EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

        //inputDTO.UnitId = empSession.UnitId;

        //var res = await _exitAPIController.SaveAssetMapping(inputDTO);


        return res;

    }
}
