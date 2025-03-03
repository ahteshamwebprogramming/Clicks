using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "Module")]
    public class ModuleViewComponent : ViewComponent
    {
        private readonly ClientController _clientSettingAPIController;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ModuleViewComponent(ClientController clientSettingAPIController, IHttpContextAccessor httpContextAccessor)
        {
            _clientSettingAPIController = clientSettingAPIController;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int output;
            ClientSettingDTO clientSettingDTO = new ClientSettingDTO();
            List<MenuMasterDTO> menus = new List<MenuMasterDTO>();
            SideBarMenusDTO sideBar = new SideBarMenusDTO();
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
                if (HttpContext.Session.GetString("MenuMapping") != null)
                    menus = JsonConvert.DeserializeObject<List<MenuMasterDTO>>(HttpContext.Session.GetString("MenuMapping"));

                sideBar.ClientSettings = clientSettingDTO;
                sideBar.Menus = menus;
                return View("Module", sideBar);
            }
            else
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
                {
                    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Index");
                }

                sideBar.ClientSettings = clientSettingDTO;
                sideBar.Menus = menus;
                return View("Module", sideBar);
            }


          
        }
    }
}
