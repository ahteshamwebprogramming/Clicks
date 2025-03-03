
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
using SimpliHR.WebUI.BL;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Employee;

public class EmployeeFamilyController : Controller
{
    private readonly EmployeeFamilyDetailController _employeeFamilyAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeMasterController _employeeMasterController;

    public EmployeeFamilyController(EmployeeFamilyDetailController employeeFamilyAPIController, EmployeeMasterController employeeMasterController, MastersKeyValueController mastersKeyValueController)
    {
        _employeeFamilyAPIController = employeeFamilyAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _employeeMasterController = employeeMasterController;
    }


    [HttpPost]
    public async Task<IActionResult> SaveEmployeeFamilyInfo(EmployeeFamilyDetailDTO inputData)
    {
        //inputData.IsAdel = new EmployeeFamilyDetailDTO();
        dynamic actionResult;

        //List<EmployeeFamilyDetailDTO> objResultData = new List<EmployeeFamilyDetailDTO>();
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
        //inputData.EmployeeId = 10;
        if (inputData != null)
        {
            if (!inputData.EmployeeId.ToString().Equals("0"))
            {
                if (inputData.EmployeeFamilyDetailId.ToString().Equals("0"))
                    actionResult = _employeeFamilyAPIController.SaveEmployeeFamilyDetail(inputData);
                else
                {
                    inputData.IsActive = inputData.IsActive == false ? inputData.IsActive = true : true;
                    actionResult = _employeeFamilyAPIController.UpdateEmployeeFamilyDetail(inputData);
                }

                string sDetailId = "";
                sDetailId = actionResult.Value.ToString();
                objResultData.DisplayMessage = sDetailId;
                int detailId = 0;
                if (inputData.EmployeeFamilyDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                {
                    inputData.EmployeeFamilyDetailId = detailId;
                    objResultData.DisplayMessage = "Success";
                }
                if (inputData.EmployeeFamilyDetailId != 0)
                {
                    EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                    empTempDoc.ScreenTab = "Family Details";
                    empTempDoc.SessionId = HttpContext.Session.Id;
                    empTempDoc.ReferenceId = inputData.EmployeeFamilyDetailId;
                    empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                    empTempDoc.EmployeeId = inputData.EmployeeId;
                    string attachmentMsg = await _employeeMasterController.SaveAttachment(empTempDoc);
                    if (attachmentMsg != "Success")
                    {
                        objResultData.DisplayMessage = attachmentMsg;
                    }
                }
            }


            objResultData.EmployeeFamilyDetails = await GetEmployeeFamilyList(inputData.EmployeeId);
        }
        return Ok(objResultData);
    }

    [HttpPost]
    public async Task<IActionResult> SaveEditEmployeeFamilyInfo(EmployeeFamilyDetailDTO inputData)
    {
        //inputData.IsAdel = new EmployeeFamilyDetailDTO();
        dynamic actionResult;

        //List<EmployeeFamilyDetailDTO> objResultData = new List<EmployeeFamilyDetailDTO>();
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
        //IActionResult actionResult;
        //inputData.EmployeeId = 10;
        if (inputData != null)
        {
            if (!inputData.EmployeeId.ToString().Equals("0"))
            {
                EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                empTempDoc.ScreenTab = "Family Details";
                empTempDoc.SessionId = HttpContext.Session.Id;
                empTempDoc.ReferenceId = inputData.EmployeeFamilyDetailId;
                empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                empTempDoc.EmployeeId = inputData.EmployeeId;
                string attachmentMsg = string.Empty;
                int unitId, clientId;
                unitId = (int)HttpContext.Session.GetInt32("UnitId");
                int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId);
                attachmentMsg = await _employeeMasterController.VerifyRequiredAttachment(empTempDoc, clientId, unitId);
                if (!string.IsNullOrEmpty(attachmentMsg))
                {
                    objResultData.DisplayMessage = attachmentMsg;
                }
                else
                {
                    //AddDeleteTableActionDTO addDelRequest = await _employeeMasterController.FindEditRequest("EmployeeFamilyDetail");
                    //if (addDelRequest != null)
                    //{
                    //    objResultData.DisplayMessage = $"Request already exists with ticket id : {addDelRequest.TicketId}";
                    //}
                    //else
                    //{
                        actionResult = _employeeFamilyAPIController.SaveEditEmployeeFamilyDetail(inputData);

                        string sDetailId = "";
                        sDetailId = actionResult.Value.ToString();
                        int detailId = 0;
                        if (inputData.EmployeeFamilyDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                        {
                            //inputData.EmployeeFamilyDetailId = detailId;
                            AddDeleteTableActionDTO? addDetailForApproval = new AddDeleteTableActionDTO();
                            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

                            addDetailForApproval.ActionBy = empSession.EmployeeId;
                            addDetailForApproval.ActionStatus = 0;
                            addDetailForApproval.ActionType = "Add";
                            inputData.TicketId = CommonHelper.CreateTicket("EMPEdit", inputData.TicketId);
                            addDetailForApproval.TicketId = inputData.TicketId;
                            addDetailForApproval.ReferenceTable = "EmployeeFamilyDetail";
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
                        //}
                    }
                }
            }

            objResultData.EmployeeFamilyDetails = await GetEmployeeFamilyList(inputData.EmployeeId);
        }
        return Ok(objResultData);
    }

    public async Task<List<EmployeeFamilyDetailDTO>?> GetEmployeeFamilyList(int? employeeID)
    {

        IActionResult actionResult = await _employeeFamilyAPIController.GetEmployeeFamilyList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, employeeID);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<EmployeeFamilyDetailDTO> objResultData = (List<EmployeeFamilyDetailDTO>)objResult.Value;
        return objResultData;
    }
    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeFamilyDetail(EmployeeFamilyDetailDTO inputData)
    {
        try
        {
            EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
            if (inputData != null)
            {
                if (!inputData.EmployeeFamilyDetailId.ToString().Equals("0"))
                {
                    var objRes = await _employeeFamilyAPIController.DeleteEmployeeFamilyDetail(inputData);
                    if (objRes != null)
                    {
                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                        {
                            objResultData.EmployeeFamilyDetails = await GetEmployeeFamilyList(inputData.EmployeeId);
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


    //[HttpGet]
    //[Route("Employee/GetEmployeeInfo/{EmployeeId:int}")]
    //public async Task<IActionResult> GetEmployeeInfo(int EmployeeId)
    //{
    //    if (EmployeeId.ToString().Equals(string.Empty))
    //    {
    //        EmployeeFamilyDetailDTO outputData = new EmployeeFamilyDetailDTO();
    //        outputData.EmployeeId = EmployeeId;

    //        IActionResult actionResult;

    //        actionResult = await _employeeFamilyAPIController.GetEmployee(outputData);
    //        ObjectResult objResult = (ObjectResult)actionResult;
    //        var objResultData = (EmployeeFamilyDetailDTO)objResult.Value;
    //        if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
    //        {
    //            return View("Employee", objResultData);
    //            //return RedirectToAction("Role","Role", objResultData);
    //        }
    //        else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
    //        {
    //            objResultData.EmployeeId.ToString(string.Empty);
    //            objResultData.EmployeeFamilyDetailList = await GetEmployeeList();
    //            objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
    //            return View("Employee", objResultData);
    //            //return RedirectToAction("Role", objResultData);
    //        }
    //    }
    //    return RedirectToAction("Employee", "Employee");
    //}

    //[HttpGet]
    //[Route("Employee/DeleteEmployee/{EmployeeId:int}")]
    //public async Task<IActionResult> DeleteEmployee(int EmployeeId)
    //{
    //    if (EmployeeId.ToString().Equals(string.Empty))
    //    {
    //        EmployeeFamilyDetailDTO outputData = new EmployeeFamilyDetailDTO();
    //        outputData.EmployeeId = EmployeeId;

    //        IActionResult actionResult;

    //        actionResult = await _employeeFamilyAPIController.DeleteEmployee(outputData);
    //        ObjectResult objResult = (ObjectResult)actionResult;
    //        var objResultData = objResult.Value;

    //        //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
    //        //{
    //        outputData.EmployeeId.ToString(string.Empty);
    //        outputData.EmployeeFamilyDetailList = await GetEmployeeList();
    //        outputData.DisplayMessage = "Employee record deactivated successfully";
    //        return View("Employee", outputData);
    //        //}
    //    }
    //    return RedirectToAction("Employee", "Employee");
    //}
}
