using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Employee;
using SimpliHR.Endpoints.EmployeeSocialActivity;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;

namespace SimpliHR.WebUI.Controllers.EmployeeManagement;

public class NotificationController : Controller
{
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly NotificationAPIController _notificationAPIController;
    private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnv;
    public NotificationController(MastersKeyValueController mastersKeyValueController, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnv, NotificationAPIController notificationAPIController)
    {
        _mastersKeyValueController = mastersKeyValueController;
        this._hostingEnv = hostingEnv;
        _notificationAPIController = notificationAPIController;
    }

    public async Task<IActionResult> GetNotificationsPartialView()
    {
        List<AppMessageDTO>? appMessageDTOs = new List<AppMessageDTO>();
        string? strEmpSession = HttpContext.Session.GetString("employee");
        if (!string.IsNullOrEmpty(strEmpSession))
        {
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession != null)
            {
                var res = await _notificationAPIController.GetNotifications(empSession.EmployeeId);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    appMessageDTOs = (List<AppMessageDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (appMessageDTOs != null)
                    {
                        foreach (var item in appMessageDTOs)
                        {
                            item.encMessageId = CommonHelper.EncryptURLHTML(item.MessageId.ToString());
                        }
                    }
                }
            }
        }
        return PartialView("_partialViews/_notification", appMessageDTOs);
    }


    public async void ClearNotification([FromBody] AppMessageDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.encMessageId != null)
                {
                    inputDTO.MessageId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encMessageId));
                    var x = await _notificationAPIController.ClearNotification(inputDTO.MessageId);
                }
            }

        }
        catch (Exception ex)
        {
        }

    }


}
