using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.ClientManagement;

using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "Footer")]
    public class FooterViewComponent : ViewComponent
    {
        private readonly ClientController _clientSettingAPIController;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FooterViewComponent(ClientController clientSettingAPIController, IHttpContextAccessor httpContextAccessor)
        {
            _clientSettingAPIController = clientSettingAPIController;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int output;
            ClientSettingDTO clientSettingDTO = new ClientSettingDTO();
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out output) == true)
            {
                if (output == -1)
                {
                    clientSettingDTO.ClientId = -1;
                }
                else
                {
                    clientSettingDTO = await _clientSettingAPIController.GetClientSettingDetails(output);
                }
                return View("Footer", clientSettingDTO);
            }
            else
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
                {
                    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Index");
                }

                return View("Footer", clientSettingDTO);
            }
           
        }
    }
}
