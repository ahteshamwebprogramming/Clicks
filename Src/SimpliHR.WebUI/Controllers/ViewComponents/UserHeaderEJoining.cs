using DocumentFormat.OpenXml.InkML;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;


namespace SimpliHR.WebUI.Controllers.ViewComponents;
[ViewComponent(Name = "UserHeaderEJoining")]
public class UserHeaderEJoiningComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserHeaderEJoiningComponent(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
       
        EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
        string employee = HttpContext.Session.GetString("employee");
        if (!String.IsNullOrEmpty(employee))
        {
            employeeMasterDTO = JsonConvert.DeserializeObject<EmployeeMasterDTO>(employee.Trim());
            employeeMasterDTO.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(employeeMasterDTO.ProfileImage, 0, employeeMasterDTO.ProfileImage.Length);
            return View("UserHeaderEJoining", employeeMasterDTO);
        }
        else
        {
            //if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
            //{
            //    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Login");
            //}

            return View("UserHeaderEJoining", employeeMasterDTO);
        }        
    }
}
