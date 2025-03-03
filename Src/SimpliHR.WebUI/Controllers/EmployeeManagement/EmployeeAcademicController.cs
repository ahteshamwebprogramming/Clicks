
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
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Services.DBContext;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Employee;

public class EmployeeAcademicController : Controller
{
    private readonly EmployeeAcademicDetailController _employeeAcademicAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeMasterController _employeeMasterController;

    public EmployeeAcademicController(EmployeeAcademicDetailController employeeAcademicAPIController, EmployeeMasterController employeeController, MastersKeyValueController mastersKeyValueController)
    {
        _employeeAcademicAPIController = employeeAcademicAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _employeeMasterController = employeeController;
    }

    [HttpPost]
    public async Task<IActionResult> SaveEditEmployeeAcademicInfo(EmployeeAcademicDTO inputData)
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
            if (!inputData.EmployeeId.ToString().Equals("0"))
            {

                int unitId, clientId;
                unitId = (int)HttpContext.Session.GetInt32("UnitId");
                int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId);
                EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                empTempDoc.ScreenTab = "Academic Details";
                empTempDoc.SessionId = HttpContext.Session.Id;
                empTempDoc.ReferenceId = inputData.AcademicDetailId;
                empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                empTempDoc.EmployeeId = inputData.EmployeeId;

                attachmentMsg = await _employeeMasterController.VerifyRequiredAttachment(empTempDoc, clientId, unitId);
                if (!string.IsNullOrEmpty(attachmentMsg))
                {
                    objResultData.DisplayMessage = attachmentMsg;
                }
                else
                {
                    actionResult = _employeeAcademicAPIController.SaveEditEmployeeAcademicDetail(inputData);


                    string sDetailId = "";
                    sDetailId = actionResult.Value.ToString();
                    int detailId = 0;
                    if (inputData.AcademicDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                    {
                        inputData.AcademicDetailId = detailId;
                        AddDeleteTableActionDTO? addDetailForApproval = new AddDeleteTableActionDTO();
                        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

                        addDetailForApproval.ActionBy = empSession.EmployeeId;
                        addDetailForApproval.ActionStatus = 0;
                        addDetailForApproval.ActionType = "Add";
                        inputData.TicketId = CommonHelper.CreateTicket("EMPEdit", inputData.TicketId); // creates a 8 digit random no.
                        addDetailForApproval.TicketId = inputData.TicketId;
                        addDetailForApproval.ReferenceTable = "EmployeeAcademicDetail";
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


            objResultData.EmployeeAcademicDetails = await GetEmployeeAcademicList(inputData.EmployeeId);
        }
        return Ok(objResultData);
    }


    [HttpPost]
    public async Task<IActionResult> SaveEmployeeAcademicInfo(EmployeeAcademicDTO inputData)
    {
        inputData.IsActive = true;
        dynamic actionResult = null;
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
        //inputData.EmployeeId = 10;
        if (inputData != null)
        {
            if (!inputData.EmployeeId.ToString().Equals("0"))
            {
                if (inputData.AcademicDetailId.ToString().Equals("0"))
                    actionResult = _employeeAcademicAPIController.SaveEmployeeAcademicDetail(inputData);
                else
                    actionResult = _employeeAcademicAPIController.UpdateEmployeeAcademicDetail(inputData);
                string sDetailId = "";
                sDetailId = actionResult.Value.ToString();
                objResultData.DisplayMessage = actionResult.Value.ToString();
                int detailId = 0;
                if (inputData.AcademicDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                {
                    inputData.AcademicDetailId = detailId;
                    objResultData.DisplayMessage = "Success";
                }
                if (inputData.AcademicDetailId != 0)
                {
                    EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                    empTempDoc.ScreenTab = "Academic Details";
                    empTempDoc.SessionId = HttpContext.Session.Id;
                    empTempDoc.ReferenceId = inputData.AcademicDetailId;
                    empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                    empTempDoc.EmployeeId = inputData.EmployeeId;
                    string attachmentMsg = await _employeeMasterController.SaveAttachment(empTempDoc);
                    if (attachmentMsg != "Success")
                    {
                        objResultData.DisplayMessage = attachmentMsg;
                    }
                }
            }

            objResultData.EmployeeAcademicDetails = await GetEmployeeAcademicList(inputData.EmployeeId);
        }
        return Ok(objResultData);
    }

    public async Task<List<EmployeeAcademicDTO>?> GetEmployeeAcademicList(int? employeeID)
    {

        IActionResult actionResult = await _employeeAcademicAPIController.GetEmployeeAcademicList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, employeeID);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<EmployeeAcademicDTO> objResultData = (List<EmployeeAcademicDTO>)objResult.Value;
        List<AcademicKeyValues> academicList = _mastersKeyValueController.AcademicKeyValue().Result;
        foreach (var employeeAcademics in objResultData)
            employeeAcademics.AcademicName = academicList.Where(r => r.AcademicId == employeeAcademics.AcademicId).Select(x => x.AcademicName).FirstOrDefault();
        return objResultData;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeAcademicDetail(EmployeeAcademicDTO inputData)
    {
        try
        {
            EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
            if (inputData != null)
            {
                if (!inputData.AcademicDetailId.ToString().Equals("0"))
                {
                    var objRes = await _employeeAcademicAPIController.DeleteEmployeeAcademicDetail(inputData);
                    if (objRes != null)
                    {
                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                        {
                            objResultData.EmployeeAcademicDetails = await GetEmployeeAcademicList(inputData.EmployeeId);
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
