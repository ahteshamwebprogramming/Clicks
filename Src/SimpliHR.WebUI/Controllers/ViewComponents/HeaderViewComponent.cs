using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "Header")]
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ClientController _clientSettingAPIController;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HeaderViewComponent(ClientController clientSettingAPIController, IHttpContextAccessor httpContextAccessor)
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
                    clientSettingDTO.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(clientSettingDTO.ProfileImage, 0, clientSettingDTO.ProfileImage.Length);
                 
                }
                clientSettingDTO.RoleType = HttpContext.Session.GetString("RoleType").Trim();
                return View("Header", clientSettingDTO);
            }
            else
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
                {
                    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Index");
                }

                return View("Header", clientSettingDTO);
            }

        }
    }
}
