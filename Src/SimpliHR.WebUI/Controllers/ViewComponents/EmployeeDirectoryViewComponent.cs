using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Employee;


namespace SimpliHR.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "EmployeeDirectory")]
    public class EmployeeDirectoryViewComponent : ViewComponent
    {
        private readonly EmployeeDirectoryAPIController _directoryAPIController;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmployeeDirectoryViewComponent(EmployeeDirectoryAPIController directoryAPIController, IHttpContextAccessor httpContextAccessor)
        {
            _directoryAPIController = directoryAPIController;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            EmployeeDirectoryCardDetailsDTO outputData = new EmployeeDirectoryCardDetailsDTO();
            //int output;
            //ClientSettingDTO clientSettingDTO = new ClientSettingDTO();
            //if (int.TryParse(HttpContext.Session.GetString("ClientId"), out output) == true)
            //{
            //    if (output == -1)
            //    {
            //        clientSettingDTO.ClientId = -1;
            //    }
            //    else
            //    {
            //        clientSettingDTO = await _clientSettingAPIController.GetClientSettingDetails(output);
            //    }
            //    return View("EmployeeDirectory", clientSettingDTO);
            //}
            //else
            //{
            //    if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
            //    {
            //        _httpContextAccessor.HttpContext.Response.Redirect("/Account/Index");
            //    }

            return View("EmployeeDirectory", outputData);
          //  }
           
        }


        [HttpGet]
        public async Task<IViewComponentResult> GetEmployeeDirectoryDetails(int employeeId)
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
            return View("EmployeeDirectory", outputData);
        }
    }
}
