
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

public class EmployeeReferenceController : Controller
{
    private readonly EmployeeReferenceDetailController _employeeReferenceAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeMasterController _employeeMasterController;

    public EmployeeReferenceController(EmployeeReferenceDetailController employeeAPIController, EmployeeMasterController employeeMasterController, MastersKeyValueController mastersKeyValueController)
    {
        _employeeReferenceAPIController = employeeAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _employeeMasterController = employeeMasterController;
    }

    [HttpPost]
    public async Task<IActionResult> SaveEditEmployeeReferenceInfo(EmployeeReferenceDetailDTO inputData)
    {
        //inputData.IsAdel = new EmployeeFamilyDetailDTO();
        dynamic actionResult;
        string attachmentMsg;
        //List<EmployeeFamilyDetailDTO> objResultData = new List<EmployeeFamilyDetailDTO>();
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
        //IActionResult actionResult;
        //inputData.EmployeeId = 10;
        if (inputData != null)
        {
            if (!inputData.ReferenceOf.ToString().Equals("0"))
            {
                int unitId, clientId;
                unitId = (int)HttpContext.Session.GetInt32("UnitId");
                int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId);
                EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                empTempDoc.ScreenTab = "Reference Details";
                empTempDoc.SessionId = HttpContext.Session.Id;
                empTempDoc.ReferenceId = inputData.EmployeeReferenceId;
                empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                empTempDoc.EmployeeId = inputData.ReferenceOf;
                attachmentMsg = await _employeeMasterController.VerifyRequiredAttachment(empTempDoc, clientId, unitId);
                if (!string.IsNullOrEmpty(attachmentMsg))
                {
                    objResultData.DisplayMessage = attachmentMsg;
                }
                else
                {
                        actionResult = _employeeReferenceAPIController.SaveEditEmployeeReferenceDetail(inputData);
                    //else
                    //{
                    //    inputData.IsActive = inputData.IsActive == false ? inputData.IsActive = true : true;
                    //    actionResult = _employeeFamilyAPIController.UpdateEmployeeFamilyDetail(inputData);
                    //}

                    string sDetailId = "";
                    sDetailId = actionResult.Value.ToString();
                    int detailId = 0;
                    if (inputData.EmployeeReferenceId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                    {
                        inputData.EmployeeReferenceId = 0;
                        AddDeleteTableActionDTO? addDetailForApproval = new AddDeleteTableActionDTO();
                        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

                        addDetailForApproval.ActionBy = empSession.EmployeeId;
                        addDetailForApproval.ActionStatus = 0;
                        addDetailForApproval.ActionType = "Add";
                        inputData.TicketId = CommonHelper.CreateTicket("EMPEdit", inputData.TicketId);
                        addDetailForApproval.TicketId = inputData.TicketId;
                        addDetailForApproval.ReferenceTable = "EmployeeReferenceDetail";
                        addDetailForApproval.ReferenceId = detailId;
                        addDetailForApproval.EmployeeId = inputData.ReferenceOf;
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


            objResultData.EmployeeReferenceDetails = await GetEmployeeReferenceList(inputData.ReferenceOf);
        }
        return Ok(objResultData);
    }


    [HttpPost]
    public async Task<IActionResult> SaveEmployeeReferenceInfo(EmployeeReferenceDetailDTO inputData)
    {
        inputData.IsActive = true;
        dynamic actionResult;
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
        //List<EmployeeReferenceDetailDTO> objResultData = new List<EmployeeReferenceDetailDTO>();
        //inputData.EmployeeId = 10;
        if (inputData != null)
        {
            if (!inputData.ReferenceOf.ToString().Equals("0"))
            {
                if (inputData.EmployeeReferenceId.ToString().Equals("0"))
                    actionResult = _employeeReferenceAPIController.SaveEmployeeReferenceDetail(inputData);
                else
                    actionResult = _employeeReferenceAPIController.UpdateEmployeeReferenceDetail(inputData);

                string sDetailId = "";
                sDetailId = actionResult.Value.ToString();
                int detailId = 0;
                if (inputData.EmployeeReferenceId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                {
                    inputData.EmployeeReferenceId = detailId;
                    objResultData.DisplayMessage = "Success";
                }
                if (inputData.EmployeeReferenceId != 0)
                {
                    EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                    empTempDoc.ScreenTab = "Reference Details";
                    empTempDoc.SessionId = HttpContext.Session.Id;
                    empTempDoc.ReferenceId = inputData.EmployeeReferenceId;
                    empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                    empTempDoc.EmployeeId = inputData.ReferenceOf;
                    string attachmentMsg = await _employeeReferenceAPIController.SaveAttachment(empTempDoc);
                    if (attachmentMsg != "Success")
                    {
                        objResultData.DisplayMessage = attachmentMsg;
                    }
                }

            }
            objResultData.EmployeeReferenceDetails = await GetEmployeeReferenceList(inputData.ReferenceOf);
        }

        return Ok(objResultData);
    }

    public async Task<List<EmployeeReferenceDetailDTO>?> GetEmployeeReferenceList(int? employeeID)
    {

        IActionResult actionResult = await _employeeReferenceAPIController.GetEmployeeReferenceList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, employeeID);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<EmployeeReferenceDetailDTO> objResultData = (List<EmployeeReferenceDetailDTO>)objResult.Value;
        return objResultData;

    }


    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeReferenceDetail(EmployeeReferenceDetailDTO inputData)
    {
        try
        {
            EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
            if (inputData != null)
            {
                if (!inputData.EmployeeReferenceId.ToString().Equals("0"))
                {
                    var objRes = await _employeeReferenceAPIController.DeleteEmployeeReferenceDetail(inputData);
                    if (objRes != null)
                    {
                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                        {
                            objResultData.EmployeeReferenceDetails = await GetEmployeeReferenceList(inputData.ReferenceOf);
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
