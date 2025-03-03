using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.Leave;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.WebUI.BL;
using SimpliHR.WebUI.Controllers.Attendance;

namespace SimpliHR.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "ApprovalCentre")]
    public class ApprovalCentreViewComponent : ViewComponent
    {
        private readonly ClientController _clientSettingAPIController;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LeaveAPIController _leaveAPIController;
        private readonly EmployeeAttendanceController _employeeAttendanceAPIController;
        public ApprovalCentreViewComponent(ClientController clientSettingAPIController, IHttpContextAccessor httpContextAccessor, EmployeeAttendanceController employeeAttendanceAPIController, LeaveAPIController leaveAPIController)
        {
            _clientSettingAPIController = clientSettingAPIController;
            _httpContextAccessor = httpContextAccessor;
            _leaveAPIController = leaveAPIController;
            _employeeAttendanceAPIController = employeeAttendanceAPIController;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int output;
            ClientSettingDTO clientSettingDTO = new ClientSettingDTO();
            List<MenuMasterDTO> menus = new List<MenuMasterDTO>();
            SideBarMenusDTO sideBar = new SideBarMenusDTO();

            AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
            EmployeeDashboardVM outputData = new EmployeeDashboardVM();
            int employeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out output) == true)
            {
                outputData.EmployeeLeaveDetail = await _leaveAPIController.GetLeavePendingForApproval(Convert.ToInt32(employeeId), unitId,0);
                IActionResult actionResultCompOff = await _leaveAPIController.GetEmployeeCompOffInfo(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 },employeeId, unitId);
                ObjectResult objResultCompOff = (ObjectResult)actionResultCompOff;
                outputData.EmployeeCompOffList = (List<EmployeeCompOffDTO>)objResultCompOff.Value;
                outputData.manualPunchVM = await _employeeAttendanceAPIController.GetAttendancePendingForApproval(employeeId);
                return View("ApprovalCentre", outputData);
            }
            else
            {
                //if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
                //{
                //    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Index");
                //}

                //sideBar.ClientSettings = clientSettingDTO;
                //sideBar.Menus = menus;
                return View("ApprovalCentre", outputData);
            }



        }
    }
}
