using DocumentFormat.OpenXml.InkML;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;


namespace SimpliHR.WebUI.Controllers.ViewComponents;
[ViewComponent(Name = "UserHeader")]
public class UserHeaderComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserHeaderComponent(IHttpContextAccessor httpContextAccessor)
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
            return View("UserHeader", employeeMasterDTO);
        }
        else
        {
            //if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.Request.Path.Value != "/Account/Login")
            //{
            //    _httpContextAccessor.HttpContext.Response.Redirect("/Account/Login");
            //}

            return View("UserHeader", employeeMasterDTO);
        }
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
        //}
        //return View("UserHeader");
    }
}
