using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Login;

namespace SimpliHR.WebUI.Controllers.ViewComponents
{
    [ViewComponent(Name = "SelectedUnit")]
    public class SelectedUnitViewComponent : ViewComponent
    {
        private readonly ClientController _clientSettingAPIController;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SelectedUnitViewComponent(ClientController clientSettingAPIController, IHttpContextAccessor httpContextAccessor)
        {
            _clientSettingAPIController = clientSettingAPIController;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? UnitId = HttpContext.Session.GetInt32("UnitId");
            UnitMasterDTO unitMasterDTO = new UnitMasterDTO();
            if (UnitId != null)
            {
                string isClient = HttpContext.Session.GetString("isClient");
             
                if (isClient.ToLower() == "true")
                {
                    if (UnitId != null)
                    {
                        IActionResult actionResult = await _clientSettingAPIController.GetClientUnitNameById(UnitId ?? default(int));
                        ObjectResult objResult = (ObjectResult)actionResult;
                        unitMasterDTO = (UnitMasterDTO)objResult.Value;

                        //unitMasterDTO.UnitName = unitName;
                        //unitMasterDTO.CityName = 
                        return View("SelectedUnit", unitMasterDTO);
                    }
                    else
                    {
                        //if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
                        //{
                        //    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Login");
                        //}
                        return View("SelectedUnit", unitMasterDTO);
                    }

                }
                else
                {
                    //if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
                    //{
                    //    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Login");
                    //}
                    return View("SelectedUnit", unitMasterDTO);
                }
            }
            else
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
                {
                    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Index");
                }

                return View("SelectedUnit", unitMasterDTO);
            }
        }
    }
}
