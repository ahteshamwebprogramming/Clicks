
using iTextSharp.text.pdf.codec.wmf;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

public class EmployeeContactController : Controller
{
    private readonly EmployeeContactDetailController _employeeContactAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeMasterController _employeeMasterController;
    public EmployeeContactController(EmployeeContactDetailController EmployeeContactAPIController, EmployeeMasterController employeeMasterController, MastersKeyValueController mastersKeyValueController)
    {
        _employeeContactAPIController = EmployeeContactAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _employeeMasterController = employeeMasterController;
    }
    public async Task<IActionResult> EmployeeContactDetail()
    {
        EmployeeContactDetailDTO outputData = new EmployeeContactDetailDTO();
       // outputData.EmployeeMastersKeyValue = await _mastersKeyValueController.EmployeeContactMastersKeyValue();
        return View("EmployeeContact", outputData);
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<object> SaveEmployeeContactInfo(string employeeContactDataString)
    {

        IEnumerable<EmployeeContactDetailDTO> inputData = JsonConvert.DeserializeObject<List<EmployeeContactDetailDTO>>(employeeContactDataString);
        try
        {
            string sDetailId;
            int iDetailId;
            string sTicket = string.Empty;
            IActionResult actionResult;
            EmployeeContactDetailDTO viewModel = new EmployeeContactDetailDTO();
            string attachmentMsg=string.Empty;
            if (inputData != null)
            {
                foreach (var tableData in inputData)
                {
                    if (!tableData.EmployeeId.ToString().Equals("0"))
                    {
                        EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                        
                        empTempDoc.SessionId = HttpContext.Session.Id;
                        empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                        empTempDoc.EmployeeId = tableData.EmployeeId;
                        if (tableData.EmployeeContactDetailId.ToString().Equals("0"))
                        {
                            //tableData.IsActive = true;
                            actionResult = _employeeContactAPIController.SaveEmployeeContacts(tableData);
                            sDetailId = ((ObjectResult)actionResult).Value.ToString();
                            if (int.TryParse(sDetailId, out iDetailId))
                            {
                                tableData.EmployeeContactDetailId = iDetailId;
                                tableData.DisplayMessage = "Success";
                            }
                            else
                            {
                                tableData.DisplayMessage = sDetailId;
                            }
                            empTempDoc.ReferenceId = tableData.EmployeeContactDetailId;

                            if (tableData.ContactType.ToUpper() == "CURRENT")
                                empTempDoc.ScreenTab = "Current Address";
                            else if (tableData.ContactType == "PERMANENT")
                                empTempDoc.ScreenTab = "Permanent Address";
                            attachmentMsg = await _employeeMasterController.SaveAttachment(empTempDoc);

                            //empTempDoc.ScreenTab = "Permanent Address";
                            //attachmentMsg = await _employeeContactAPIController.SaveAttachment(empTempDoc);

                        }
                        else
                        {
                            if (tableData.ContactType.ToUpper() == "CURRENT")
                                empTempDoc.ScreenTab = "Current Address";
                            else if (tableData.ContactType.ToUpper() == "PERMANENT")
                                empTempDoc.ScreenTab = "Permanent Address";
                            empTempDoc.ReferenceId = tableData.EmployeeContactDetailId;
                             attachmentMsg = await _employeeMasterController.SaveAttachment(empTempDoc);
                            tableData.DisplayMessage = _employeeContactAPIController.UpdateEmployeeContacts(tableData);
                        }
                    }
                }
            }
            
            return inputData;
        }
        catch (Exception ex)
        {
            return inputData;
        }
    }

    public async Task<object> SaveEditEmployeeContactInfo(string employeeContactDataString)
    {
        IEnumerable<EmployeeContactDetailDTO> inputData = JsonConvert.DeserializeObject<List<EmployeeContactDetailDTO>>(employeeContactDataString);
        try
        {
            dynamic actionResult = null;
            string sDetailId;
            int iDetailId;
            string sTicket = string.Empty;
            string attachmentMsg = string.Empty;
            int unitId, clientId;
            EmployeeContactDetailDTO viewModel = new EmployeeContactDetailDTO();
            
            if (inputData != null)
            {
                foreach (var tableData in inputData)
                {
                    if (!tableData.EmployeeId.ToString().Equals("0"))
                    {
                        //EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();

                        //empTempDoc.SessionId = HttpContext.Session.Id;
                        //empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                        //empTempDoc.EmployeeId = tableData.EmployeeId;
                        if (tableData.EmployeeContactDetailId.ToString().Equals("0"))
                        {
                            
                            EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                            
                            //tableData.IsActive = true;
                            if (tableData.ContactType.ToUpper() == "CURRENT")
                                empTempDoc.ScreenTab = "Current Address";
                            else if (tableData.ContactType.ToUpper() == "PERMANENT")
                                empTempDoc.ScreenTab = "Permanent Address";
                            empTempDoc.SessionId = HttpContext.Session.Id;
                            empTempDoc.ReferenceId = tableData.EmployeeContactDetailId;
                            empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                            empTempDoc.EmployeeId = tableData.EmployeeId;
                            unitId = (int)HttpContext.Session.GetInt32("UnitId");
                            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
                            {
                                attachmentMsg = await _employeeMasterController.VerifyRequiredAttachment(empTempDoc, clientId, unitId);
                                if(!string.IsNullOrEmpty(attachmentMsg))
                                {
                                    tableData.DisplayMessage = empTempDoc.ScreenTab == "Permanent Address"? attachmentMsg.Replace("Attachment is mandatory for", ""): attachmentMsg;
                                    
                                }
                            }
                            
                            if(tableData.DisplayMessage=="_blank")
                            {
                                AddDeleteTableActionDTO addDelRequest = await _employeeMasterController.FindEditRequest("EmployeeContactDetail");
                                if (addDelRequest != null)
                                {
                                    tableData.DisplayMessage = $"Request already exists with ticket id : {addDelRequest.TicketId}";
                                    return tableData;
                                }
                                actionResult = _employeeContactAPIController.SaveEditEmployeeContacts(tableData);
                                //empTempDoc.ScreenTab = "Current Address";
                                //string attachmentMsg = await _employeeContactAPIController.SaveAttachment(empTempDoc);
                                //empTempDoc.ScreenTab = "Permanent Address";
                                //attachmentMsg = await _employeeContactAPIController.SaveAttachment(empTempDoc);
                                sDetailId = ((ObjectResult)actionResult).Value.ToString();

                                if (int.TryParse(sDetailId, out iDetailId))
                                {
                                    tableData.EmployeeContactDetailId = iDetailId;
                                    AddDeleteTableActionDTO? addDetailForApproval = new AddDeleteTableActionDTO();
                                    EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

                                    addDetailForApproval.ActionBy = empSession.EmployeeId;
                                    addDetailForApproval.ActionStatus = 0;
                                    addDetailForApproval.ActionType = "Add";

                                    if (string.IsNullOrEmpty(sTicket) && string.IsNullOrEmpty(tableData.TicketId))
                                    {
                                        tableData.TicketId = CommonHelper.CreateTicket("Contact", tableData.TicketId);
                                    }
                                    sTicket = tableData.TicketId;
                                    addDetailForApproval.TicketId = sTicket;
                                    addDetailForApproval.ReferenceTable = "EmployeeContactDetail";
                                    addDetailForApproval.ReferenceId = iDetailId;
                                    addDetailForApproval.EmployeeId = tableData.EmployeeId;
                                    addDetailForApproval.EntrySource = tableData.EntrySource;
                                    addDetailForApproval.CreatedOn = DateTime.Now;
                                    addDetailForApproval.LoggedInUser = empSession.EmployeeId;
                                    addDetailForApproval.IsActive = true;
                                    addDetailForApproval.EntrySource = string.IsNullOrEmpty(tableData.EntrySource) ? "EmployeeEditScreen" : tableData.EntrySource;
                                    actionResult = await _employeeMasterController.SaveEmployeeDetailForApproval(addDetailForApproval);
                                    tableData.DisplayMessage = actionResult.Value.ToString();
                                    tableData.TicketId = tableData.TicketId;



                                    attachmentMsg = await _employeeMasterController.SaveAttachment(empTempDoc);
                                    if (attachmentMsg != "Success")
                                    {
                                        tableData.DisplayMessage = attachmentMsg;
                                    }
                                }
                            }
                            

                        }
                       


                    }
                }
            }

            return inputData;
        }
        catch (Exception ex)
        {
            return inputData;
        }
    }


    public async Task<IActionResult> EmployeeContact()
    {
        EmployeeContactDetailDTO outputData = new EmployeeContactDetailDTO();
        //outputData.EmployeeContactMasterList = await GetEmployeeContactList();
       
        if (outputData != null)
        {
            return View("Dashboard", outputData);
        }
        else
        {
            return View("Dashboard");
        }
    }

    public async Task<List<EmployeeContactDetailDTO>?> GetEmployeeContactList(int? employeeId)
    {

        IActionResult actionResult = await _employeeContactAPIController.GetEmployeeContacts(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, employeeId);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<EmployeeContactDetailDTO> objResultData = (List<EmployeeContactDetailDTO>)objResult.Value;
        return objResultData;
    }
  


    //[HttpGet]
    //[Route("EmployeeContact/GetEmployeeContactInfo/{EmployeeContactDetailId:int}")]
    //public async Task<IActionResult> GetEmployeeContactInfo(int EmployeeContactDetailId)
    //{
    //    if (EmployeeContactDetailId.ToString().Equals(string.Empty))
    //    {
    //        EmployeeContactDetailDTO outputData = new EmployeeContactDetailDTO();
    //        outputData.EmployeeContactDetailId = EmployeeContactDetailId;

    //        IActionResult actionResult;

    //       // actionResult = await _employeeContactAPIController.GetEmployeeContacts(outputData);
    //        ObjectResult objResult = (ObjectResult)actionResult;
    //        var objResultData = (EmployeeContactDetailDTO)objResult.Value;
    //        //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
    //        //{
    //        //    return View("EmployeeContact", objResultData);
    //        //    //return RedirectToAction("Role","Role", objResultData);
    //        //}
    //        //else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
    //        //{
    //        //    objResultData.EmployeeContactDetailId.ToString(string.Empty);
    //        //   // objResultData.EmployeeContactMasterList = await GetEmployeeContactList();
    //        //   // objResultData.DisplayMessage = "You cannot edit locked resource. Contact Admin for further details";
    //        //    return View("EmployeeContact", objResultData);
    //        //    //return RedirectToAction("Role", objResultData);
    //        //}
    //    }
    //    return RedirectToAction("EmployeeContact", "EmployeeContact");
    //}

//    [HttpGet]
//    [Route("EmployeeContact/DeleteEmployeeContact/{EmployeeContactDetailId:int}")]
//    public async Task<IActionResult> DeleteEmployeeContact(int EmployeeContactDetailId)
//    {
//        if (EmployeeContactDetailId.ToString().Equals(string.Empty))
//        {
//            EmployeeContactDetailDTO outputData = new EmployeeContactDetailDTO();
//            outputData.EmployeeContactDetailId = EmployeeContactDetailId;

//            IActionResult actionResult;

//            actionResult = await _employeeContactAPIController.DeleteEmployeeContacts(outputData);
//            ObjectResult objResult = (ObjectResult)actionResult;
//            var objResultData = objResult.Value;

//            //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
//            //{
//            outputData.EmployeeContactDetailId.ToString(string.Empty);
//          //  outputData.EmployeeContactMasterList = await GetEmployeeContactList();
//          //  outputData.DisplayMessage = "EmployeeContact record deactivated successfully";
//            return View("EmployeeContact", outputData);
//            //}
//        }
//        return RedirectToAction("EmployeeContact", "EmployeeContact");
//    }
}
