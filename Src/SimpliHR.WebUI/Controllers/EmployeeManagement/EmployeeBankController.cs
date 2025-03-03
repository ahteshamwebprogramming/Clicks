
using iTextSharp.tool.xml.html.table;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Common;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Services.DBContext;
using System.Net;

namespace SimpliHR.WebUI.Controllers.Employee;

public class EmployeeBankController : Controller
{
    private readonly EmployeeBankDetailController _employeeBankAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeMasterController _employeeMasterController;
    public EmployeeBankController(EmployeeBankDetailController employeeBankAPIController, EmployeeMasterController employeeMasterController, MastersKeyValueController mastersKeyValueController)
    {
        _employeeBankAPIController = employeeBankAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _employeeMasterController = employeeMasterController;
    }


    [HttpPost]
    public async Task<EmployeeMasterDTO> SaveEmployeeBankInfo(EmployeeBankDetailDTO inputData)
    {
        //inputData.IsAdel = new EmployeeBankDetailDTO();
        dynamic actionResult = null;
        int bankDetailId = 0;
        string sDetailId = string.Empty;

        //string objResultData = null;
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
        //string message = string.Empty;
        if (inputData != null)
        {
            if (!inputData.EmployeeId.ToString().Equals("0"))
            {
                if (inputData.BankDetailId.ToString().Equals("0"))
                    actionResult = _employeeBankAPIController.SaveEmployeeBankDetail(inputData);
                else
                    actionResult = _employeeBankAPIController.UpdateEmployeeBankDetail(inputData);
            }

            ObjectResult objResult = (ObjectResult)actionResult;
            sDetailId = objResult.Value.ToString();
            if (inputData.BankDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out bankDetailId))
            {
                inputData.BankDetailId = bankDetailId;
                objResultData.BankDetailId = bankDetailId;
                objResultData.DisplayMessage = "Success";
            }
            else
            {
                objResultData.DisplayMessage = sDetailId;
                objResultData.BankDetailId = inputData.BankDetailId;
            }

            if (!inputData.BankDetailId.ToString().Equals("0"))
            {
                EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                empTempDoc.ScreenTab = "Bank Details";
                empTempDoc.SessionId = HttpContext.Session.Id;
                empTempDoc.ReferenceId = inputData.BankDetailId;
                empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                empTempDoc.EmployeeId = inputData.EmployeeId;
                string attachmentMsg = await _employeeMasterController.SaveAttachment(empTempDoc);
                if (attachmentMsg != "Success")
                {
                    objResultData.DisplayMessage = attachmentMsg;
                }
            }

        }
        return objResultData;
    }

    [HttpPost]
    public async Task<EmployeeMasterDTO> SaveEditEmployeeBankInfo(EmployeeBankDetailDTO inputData)
    {

        dynamic actionResult = null;
        int bankDetailId = 0;
        string sDetailId = "";
        int clientId, unitId;
        //string objResultData = null;
        EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
        string attachmentMsg;
        //string message = string.Empty;
        if (inputData != null)
        {
            EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
            empTempDoc.ScreenTab = "Bank Details";
            empTempDoc.SessionId = HttpContext.Session.Id;
            empTempDoc.ReferenceId = inputData.BankDetailId;
            empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            empTempDoc.EmployeeId = inputData.EmployeeId;

            if (!inputData.EmployeeId.ToString().Equals("0"))
            {
                if (inputData.BankDetailId.ToString().Equals("0"))
                {
                    unitId = (int)HttpContext.Session.GetInt32("UnitId");
                    if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
                    {
                        attachmentMsg = await _employeeMasterController.VerifyRequiredAttachment(empTempDoc, clientId, unitId);
                        if (!string.IsNullOrEmpty(attachmentMsg))
                        {
                            objResultData.DisplayMessage = attachmentMsg;
                        }
                        else
                        {
                            AddDeleteTableActionDTO addDelRequest = await _employeeMasterController.FindEditRequest("EmployeeBankDetail");
                            if (addDelRequest != null)
                            {
                                objResultData.DisplayMessage = $"Request already exists with ticket id : {addDelRequest.TicketId}";
                            }
                            else
                            {
                                actionResult = _employeeBankAPIController.SaveEditEmployeeBankDetail(inputData);
                                sDetailId = ((ObjectResult)actionResult).Value.ToString();

                                if (inputData.BankDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out bankDetailId))
                                {
                                    inputData.BankDetailId = bankDetailId;
                                    AddDeleteTableActionDTO? addDetailForApproval = new AddDeleteTableActionDTO();
                                    EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
                                    addDetailForApproval.ActionBy = empSession.EmployeeId;
                                    addDetailForApproval.ActionStatus = 0;
                                    addDetailForApproval.ActionType = "Add";
                                    inputData.TicketId = CommonHelper.CreateTicket("BANK", inputData.TicketId);
                                    addDetailForApproval.TicketId = inputData.TicketId;
                                    addDetailForApproval.ReferenceTable = "EmployeeBankDetail";
                                    addDetailForApproval.ReferenceId = bankDetailId;
                                    addDetailForApproval.EmployeeId = inputData.EmployeeId;
                                    addDetailForApproval.EntrySource = inputData.EntrySource;
                                    addDetailForApproval.CreatedOn = DateTime.Now;
                                    addDetailForApproval.LoggedInUser = empSession.EmployeeId;
                                    addDetailForApproval.IsActive = true;
                                    addDetailForApproval.EntrySource = string.IsNullOrEmpty(inputData.EntrySource) ? "EmployeeEditScreen" : inputData.EntrySource;
                                    actionResult = await _employeeMasterController.SaveEmployeeDetailForApproval(addDetailForApproval);
                                    objResultData.DisplayMessage = actionResult.Value.ToString();
                                    objResultData.TicketId = inputData.TicketId;
                                    //objResultData.BankId = bankDetailId;

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
                                else
                                {
                                    objResultData.DisplayMessage = sDetailId;
                                }
                            }
                        }
                    }
                   
                }

            }
        }
        return objResultData;
    }

    //public async Task<IActionResult> Employee()
    //{
    //    EmployeeBankDetailDTO outputData = new EmployeeBankDetailDTO();
    //    outputData.EmployeeBankDetailList = await GetEmployeeList();

    //    if (outputData != null)
    //    {
    //        return View("Dashboard", outputData);
    //    }
    //    else
    //    {
    //        return View("Dashboard");
    //    }
    //}

    //public async Task<List<EmployeeBankDetailDTO>?> GetEmployeeList()
    //{

    //    IActionResult actionResult = await _employeeBankAPIController.GetEmployees(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
    //    ObjectResult objResult = (ObjectResult)actionResult;

    //    List<EmployeeBankDetailDTO> objResultData = (List<EmployeeBankDetailDTO>)objResult.Value;
    //    return objResultData;
    //}



    //[HttpGet]
    //[Route("Employee/GetEmployeeInfo/{EmployeeId:int}")]
    //public async Task<IActionResult> GetEmployeeInfo(int EmployeeId)
    //{
    //    if (EmployeeId.ToString().Equals(string.Empty))
    //    {
    //        EmployeeBankDetailDTO outputData = new EmployeeBankDetailDTO();
    //        outputData.EmployeeId = EmployeeId;

    //        IActionResult actionResult;

    //        actionResult = await _employeeBankAPIController.GetEmployee(outputData);
    //        ObjectResult objResult = (ObjectResult)actionResult;
    //        var objResultData = (EmployeeBankDetailDTO)objResult.Value;
    //        if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
    //        {
    //            return View("Employee", objResultData);
    //            //return RedirectToAction("Role","Role", objResultData);
    //        }
    //        else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
    //        {
    //            objResultData.EmployeeId.ToString(string.Empty);
    //            objResultData.EmployeeBankDetailList = await GetEmployeeList();
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
    //        EmployeeBankDetailDTO outputData = new EmployeeBankDetailDTO();
    //        outputData.EmployeeId = EmployeeId;

    //        IActionResult actionResult;

    //        actionResult = await _employeeBankAPIController.DeleteEmployee(outputData);
    //        ObjectResult objResult = (ObjectResult)actionResult;
    //        var objResultData = objResult.Value;

    //        //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
    //        //{
    //        outputData.EmployeeId.ToString(string.Empty);
    //        outputData.EmployeeBankDetailList = await GetEmployeeList();
    //        outputData.DisplayMessage = "Employee record deactivated successfully";
    //        return View("Employee", outputData);
    //        //}
    //    }
    //    return RedirectToAction("Employee", "Employee");
    //}
}
