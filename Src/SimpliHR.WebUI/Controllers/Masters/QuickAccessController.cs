using Masters.Controllers;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Infrastructure.Models.Masters;



namespace SimpliHR.WebUI.Controllers.Masters
{
    public class QuickAccessController : Controller
    {
        private readonly QuickAccessAPIController _quickaccessAPIController;
   

        public QuickAccessController(QuickAccessAPIController quickaccessAPIController)
        {

            _quickaccessAPIController = quickaccessAPIController;
          

        }
        public async Task<IActionResult> QuickAccessSetting()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            QuickAccessUnitListDTO QuickAccessDetail = new QuickAccessUnitListDTO();
            QuickAccessDetail = await _quickaccessAPIController.GetQuickAccessDetails(unitId);
            return View(QuickAccessDetail);        
           
        }

        [HttpPost]
        public async Task<IActionResult> SaveQuickAccessUnitLink(QuickAccessAction userAction)
        {
            userAction.UnitId = HttpContext.Session.GetInt32("UnitId");
            QuickAccessAction quickAccessVM = new QuickAccessAction();
          //  userAction.ApprovedBy = employeeId;
            // userAction.TicketId= "Leave/" + DateTime.Now.Month + "/" + GenerateTicket(6);
            if (userAction.QuickAccessIds != null && userAction.QuickAccessIds.Length != 0)
            {
                string sRetMsg = await _quickaccessAPIController.SaveQuickAccessUnitLink(userAction);
                quickAccessVM.DisplayMessage = sRetMsg;
            }
           // else
             //   manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
            return Ok(quickAccessVM.DisplayMessage);
        }

    }
}
