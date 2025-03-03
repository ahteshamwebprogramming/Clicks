
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Common;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Employee;

public class EmployeeCertificationController : Controller
{
    private readonly EmployeeCertificationDetailController _employeeCertificationAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeMasterController _employeeMasterController;

    public EmployeeCertificationController(EmployeeCertificationDetailController employeeAPIController, EmployeeMasterController employeeMasterController, MastersKeyValueController mastersKeyValueController)
    {
        _employeeCertificationAPIController = employeeAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _employeeMasterController = employeeMasterController;
    }

    [HttpPost]
    public async Task<IActionResult> SaveEditEmployeeCertificationInfo(EmployeeCertificationDetailDTO inputData)
    {
        //inputData.IsAdel = new EmployeeFamilyDetailDTO();
        dynamic actionResult;
        string attachmentMsg;
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();

        if (inputData != null)
        {
            if (!inputData.EmployeeId.ToString().Equals("0"))
            {
                int unitId, clientId;
                unitId = (int)HttpContext.Session.GetInt32("UnitId");
                int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId);
                EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                empTempDoc.ScreenTab = "Certification Details";
                empTempDoc.SessionId = HttpContext.Session.Id;
                empTempDoc.ReferenceId = inputData.CertificationDetailId;
                empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                empTempDoc.EmployeeId = inputData.EmployeeId;
                attachmentMsg = await _employeeMasterController.VerifyRequiredAttachment(empTempDoc, clientId, unitId);
                if (!string.IsNullOrEmpty(attachmentMsg))
                {
                    objResultData.DisplayMessage = attachmentMsg;
                }
                else
                {
                    actionResult = _employeeCertificationAPIController.SaveEditEmployeeCertificationDetail(inputData);
                    //else
                    //{
                    //    inputData.IsActive = inputData.IsActive == false ? inputData.IsActive = true : true;
                    //    actionResult = _employeeFamilyAPIController.UpdateEmployeeFamilyDetail(inputData);
                    //}

                    string sDetailId = "";
                    sDetailId = actionResult.Value.ToString();
                    int detailId = 0;
                    if (inputData.CertificationDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                    {
                        inputData.CertificationDetailId = 0;
                        AddDeleteTableActionDTO? addDetailForApproval = new AddDeleteTableActionDTO();
                        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

                        addDetailForApproval.ActionBy = empSession.EmployeeId;
                        addDetailForApproval.ActionStatus = 0;
                        addDetailForApproval.ActionType = "Add";
                        inputData.TicketId = CommonHelper.CreateTicket("EMPEdit", inputData.TicketId);
                        addDetailForApproval.TicketId = inputData.TicketId;
                        addDetailForApproval.ReferenceTable = "EmployeeCertificationDetail";
                        addDetailForApproval.ReferenceId = detailId;
                        addDetailForApproval.EmployeeId = inputData.EmployeeId;
                        addDetailForApproval.EntrySource = inputData.EntrySource;
                        addDetailForApproval.CreatedOn = DateTime.Now;
                        addDetailForApproval.LoggedInUser = empSession.EmployeeId;
                        addDetailForApproval.IsActive = true;
                        addDetailForApproval.EntrySource = inputData.EntrySource.IsNullOrEmpty() ? "EmployeeEditScreen" : inputData.EntrySource;
                        actionResult = await _employeeMasterController.SaveEmployeeDetailForApproval(addDetailForApproval);
                        objResultData.DisplayMessage = actionResult.Value.ToString();
                        objResultData.TicketId = inputData.TicketId;

                        //objResultData
                        //}
                        //if (inputData.EmployeeFamilyDetailId != 0)
                        //{
                        
                        attachmentMsg = await _employeeMasterController.SaveAttachment(empTempDoc);
                        if (attachmentMsg != "Success")
                        {
                            objResultData.DisplayMessage = attachmentMsg;
                        }
                    }
                }
            }


            objResultData.EmployeeCertificationDetails = await GetEmployeeCertificationList(inputData.EmployeeId);
        }
        return Ok(objResultData);
    }

    [HttpPost]
    public async Task<IActionResult> SaveEmployeeCertificationInfo(EmployeeCertificationDetailDTO inputData)
    {
        inputData.IsActive = true;
        dynamic actionResult;
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
        //List<EmployeeCertificationDetailDTO> objResultData = new List<EmployeeCertificationDetailDTO>();
        //inputData.EmployeeId = 10;
        if (inputData != null)
        {
            if (!inputData.EmployeeId.ToString().Equals("0"))
            {
                if (inputData.CertificationDetailId.ToString().Equals("0"))
                    actionResult = _employeeCertificationAPIController.SaveEmployeeCertificationDetail(inputData);
                else
                    actionResult = _employeeCertificationAPIController.UpdateEmployeeCertificationDetail(inputData);


                string sDetailId = "";
                sDetailId = actionResult.Value.ToString();
                objResultData.DisplayMessage = actionResult.Value.ToString();
                int detailId = 0;
                if (inputData.CertificationDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                {
                    inputData.CertificationDetailId = detailId;
                    objResultData.DisplayMessage = "Success";
                }
                if (inputData.CertificationDetailId != 0)
                {
                    EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                    empTempDoc.ScreenTab = "Certification Details";
                    empTempDoc.SessionId = HttpContext.Session.Id;
                    empTempDoc.ReferenceId = inputData.CertificationDetailId;
                    empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                    empTempDoc.EmployeeId = inputData.EmployeeId;
                    string attachmentMsg = await _employeeCertificationAPIController.SaveAttachment(empTempDoc);
                    if (attachmentMsg != "Success")
                    {
                        objResultData.DisplayMessage = attachmentMsg;
                    }
                }

            }
        }

        objResultData.EmployeeCertificationDetails = await GetEmployeeCertificationList(inputData.EmployeeId);

        return Ok(objResultData);
    }

    public async Task<List<EmployeeCertificationDetailDTO>?> GetEmployeeCertificationList(int? employeeID)
    {

        IActionResult actionResult = await _employeeCertificationAPIController.GetEmployeeCertificationList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, employeeID);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<EmployeeCertificationDetailDTO> objResultData = (List<EmployeeCertificationDetailDTO>)objResult.Value;
        return objResultData;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeCertificationDetail(EmployeeCertificationDetailDTO inputData)
    {
        try
        {
            EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
            if (inputData != null)
            {
                if (!inputData.CertificationDetailId.ToString().Equals("0"))
                {
                    var objRes = await _employeeCertificationAPIController.DeleteEmployeeCertificationDetail(inputData);
                    if (objRes != null)
                    {
                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                        {
                            objResultData.EmployeeCertificationDetails = await GetEmployeeCertificationList(inputData.EmployeeId);
                            objResultData.DisplayMessage = "Success";
                            return Ok(objResultData);
                        }
                        else
                        {
                            objResultData.DisplayMessage = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).Value.ToString();
                            return BadRequest(objRes);
                            throw new Exception(objResultData.DisplayMessage);
                        }
                    }
                }
                else
                {
                }
            }
            else
            {
                throw new Exception("Invalid Data");
            }
            objResultData.DisplayMessage = "Error";
            return BadRequest("Error has occurred while deleting data. Please contact administrator");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

}
