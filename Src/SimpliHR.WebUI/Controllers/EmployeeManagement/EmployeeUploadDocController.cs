
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.EditEmployeeData;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Services.DBContext;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace SimpliHR.WebUI.Controllers.Employee;

public class EmployeeUploadDocController : Controller
{
    private readonly EmployeeUploadDocumentController _employeeAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;

    public EmployeeUploadDocController(EmployeeUploadDocumentController employeeAPIController, MastersKeyValueController mastersKeyValueController)
    {
        _employeeAPIController = employeeAPIController;
        _mastersKeyValueController = mastersKeyValueController;
    }


    [HttpPost]
    public async Task<EditEmployeeDataDTO> SaveEditEmployeeUploadInfo(EmployeeDocumentsDTO filesData)
    {
        IActionResult actionResult = null;
        string sErrorMsg = string.Empty;
        EmployeeTempDocUploadDTO employeeTempDocUpload = new EmployeeTempDocUploadDTO();
        EditEmployeeDataDTO editEmpInfo = new EditEmployeeDataDTO();
        if (filesData.UploadedFile != null)
        {
            foreach (var file in filesData.UploadedFile)
            {
                if (file != null)
                {
                    if (file.Length > 0)
                    {
                        int typeId;
                        //bool success = int.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out typeId);
                        string docType = Path.GetExtension(file.FileName).Replace(".", "");

                        //employeeTempDocUpload.DcumentTypeId = Convert.ToInt32(typeId);
                        using (var target = new MemoryStream())
                        {
                            file.CopyTo(target);
                            //employeeTempDocUpload.EmployeeDocument = target.ToArray();
                            employeeTempDocUpload.EmployeeId = filesData.EmployeeId;
                            //employeeTempDocUpload.EmailId = filesData.EmailId;
                            employeeTempDocUpload.SessionId = HttpContext.Session.Id;
                            employeeTempDocUpload.FieldName = filesData.FieldName;
                            employeeTempDocUpload.DocumentType = docType;

                            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
                            employeeTempDocUpload.LoggedInUser = empSession.EmployeeId;
                            employeeTempDocUpload.IsActive = true;

                            // employeeTempDocUpload.UploadDcumentDetailId = filesData.UploadDcumentDetailId;
                            employeeTempDocUpload.FormName = filesData.FormName;
                            employeeTempDocUpload.ClientId = filesData.ClientId;
                            //employeeTempDocUpload.AttachmentFile.CopyTo(target);
                            employeeTempDocUpload.UploadedFile = target.ToArray();
                            //Added for Edit Employee Screen
                            employeeTempDocUpload.TicketId = filesData.TicketId;
                            employeeTempDocUpload.ChangeValue = filesData.ChangeValue;
                            employeeTempDocUpload.TableReferenceId = filesData.TableReferenceId;
                            employeeTempDocUpload.EntrySource = filesData.EntrySource;
                            //employeeTempDocUpload.DocumentType = (docType.IsNullOrEmpty() ? "unknown" : docType.Replace(".", ""));                                                      
                            editEmpInfo = await _employeeAPIController.SaveEmployeeEditInfo(employeeTempDocUpload);

                        }


                    }
                }
            }
        }
        return editEmpInfo;
    }

    [HttpPost]
    public async Task<EmployeeTempDocUploadDTO> SaveEmployeeUploadInfo(EmployeeDocumentsDTO filesData)
    {
        dynamic actionResult = null;
        string sErrorMsg = string.Empty;
        EmployeeTempDocUploadDTO employeeTempDocUpload = new EmployeeTempDocUploadDTO();
        if (filesData.UploadedFile != null)
        {
            foreach (var file in filesData.UploadedFile)
            {
                if (file != null)
                {
                    if (file.Length > 0)
                    {
                        int typeId;
                        //bool success = int.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out typeId);
                        string docType = Path.GetExtension(file.FileName).Replace(".", "");

                        //employeeTempDocUpload.DcumentTypeId = Convert.ToInt32(typeId);
                        using (var target = new MemoryStream())
                        {
                            file.CopyTo(target);
                            //employeeTempDocUpload.EmployeeDocument = target.ToArray();
                            employeeTempDocUpload.EmployeeId = filesData.EmployeeId;
                            //employeeTempDocUpload.EmailId = filesData.EmailId;
                            employeeTempDocUpload.SessionId = HttpContext.Session.Id;
                            employeeTempDocUpload.FieldName = filesData.FieldName;
                            employeeTempDocUpload.DocumentType = docType;

                            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
                            employeeTempDocUpload.LoggedInUser = empSession.EmployeeId;
                            employeeTempDocUpload.IsActive = true;

                            // employeeTempDocUpload.UploadDcumentDetailId = filesData.UploadDcumentDetailId;
                            employeeTempDocUpload.FormName = filesData.FormName;
                            employeeTempDocUpload.ClientId = filesData.ClientId;
                            //employeeTempDocUpload.AttachmentFile.CopyTo(target);
                            employeeTempDocUpload.UploadedFile = target.ToArray();
                            //Added for Edit Employee Screen
                            employeeTempDocUpload.ChangeValue = filesData.ChangeValue;
                            employeeTempDocUpload.TableReferenceId = filesData.TableReferenceId;
                            employeeTempDocUpload.EntrySource = filesData.EntrySource;
                            //employeeTempDocUpload.DocumentType = (docType.IsNullOrEmpty() ? "unknown" : docType.Replace(".", ""));                                                      
                            actionResult = await _employeeAPIController.SaveEmployeeTempDocUpload(employeeTempDocUpload);

                            employeeTempDocUpload.DisplayMessage = Convert.ToString(actionResult.Value);


                        }


                    }
                }
            }
        }
        return employeeTempDocUpload;
    }

    [HttpPost]
    public async Task<EmployeeValidationVM> ValidateEmployeeAttachments(string sTabName, string sOpt, string sEditFieldNames, int? employeeId = 0, int refrenceId = 0,int prefrenceId=0, string formId = "", string sPage = "")
    {
        var sMsg = "";
        int? unitId, clientId;
        string sTab = "";
        EmployeeValidationVM empVM = new EmployeeValidationVM();
        List<EmployeeTempDocUploadDTO> attachmentTempDocList = new List<EmployeeTempDocUploadDTO>();
        List<EmployeeUploadDocumentDTO> empUploadedDocList = new List<EmployeeUploadDocumentDTO>();
        List<EmployeeValidationDTO> empValidation = new List<EmployeeValidationDTO>();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        clientId = empSession.ClientId;
        string sTableForm = ",employeeBankform,employeefamilyform,employeeacdemicform,employeeexperienceform,employeecertificationform,employeereferenceForm,employeecontactdetailForm,employeelanguagesForm,".ToUpper();
        string sMasterForm = ",employeepersonalinfoForm,employeejobForm,employeepassportdetailsForm,".ToUpper();
        string sFinalTab = ",PersonalInformation,PermanentAddress,CurrentAddress,PassportDetails,BankDetails,";

        if (sPage == "Employee")
            sFinalTab += "";
        formId = formId.ToUpper();
        int empId = 0;

        string checkAttachmentIn = string.Empty;
        if (employeeId == null)
            empId = 0;
        else
            empId = employeeId.Value;
        //if (employeeId != null && employeeId > 0)
        //{
        //    empId = employeeId ?? default(int);
        //}
        //else
        //{
        //    empId = empSession.EmployeeId;
        //}

        unitId = HttpContext.Session.GetInt32("UnitId");
        if (string.IsNullOrEmpty(sTabName))
            sTab = "All";
        else if (sTabName == "CurrentAddress")
            sTab = "CurrentAddress,PermanentAddress";
        else
            sTab = sTabName;

        string[] arrTab = sTab.Split(',');
        foreach (var tab in arrTab)
        {
            if (tab == "PermanentAddress")
                refrenceId = prefrenceId;

            empValidation = await _employeeAPIController.GetEmployeeValidation((tab == "All" ? "" : tab), clientId.Value, unitId.Value);
           
                if (empValidation != null && empValidation.Count > 0)
            {
                attachmentTempDocList = await _employeeAPIController.GetEmployeeTempDocUpload((tab == "All" ? "" : tab), HttpContext.Session.Id);
                dynamic validationData = null;

                if (sOpt == "Edit")
                {
                    if (!string.IsNullOrEmpty(sEditFieldNames))
                        validationData = empValidation.Where(x => x.EditAttachment == true && ("," + sEditFieldNames + ",").Contains(x.FieldName)).Select(p => p);
                }
                else if (sOpt == "Add")
                {
                    //List <EmployeeValidationDTO> addAttachment= new List<EmployeeValidationDTO>();
                    empUploadedDocList = _employeeAPIController.GetEmployeeUploadedDoc(empId, refrenceId, prefrenceId);

                    if (formId == "FINALSUBMIT")
                        validationData = empValidation.Where(x => x.AddAttachment == true && sFinalTab.ToUpper().Contains(x.ScreenTab.Replace(" ","").ToUpper())).Select(p => p);
                    else
                        validationData = empValidation.Where(x => x.AddAttachment == true).Select(p => p);
                }
                if (validationData != null)
                {
                    foreach (var validation in validationData)
                    {
                        if (sOpt == "Add")
                        {
                            if(formId=="FINALSUBMIT")
                                checkAttachmentIn = "FINALTABLE";
                            else if ((sMasterForm.Contains("," + formId + ",") && empId == 0) || (sTableForm.Contains("," + formId + ",") && empId != 0 && refrenceId == 0))
                                checkAttachmentIn = "TEMPTABLE";
                            else if ((sMasterForm.Contains("," + formId + ",") && empId != 0) || (sTableForm.Contains("," + formId + ",") && empId != 0 && refrenceId != 0))
                                checkAttachmentIn = "BOTH";
                        }
                        else if (sOpt == "Edit")
                        {
                            checkAttachmentIn = "TEMPTABLE";
                            //sMsg = sMsg.IndexOf(validation.ScreenTab) < 0 ? sMsg.Length > 0 ? sMsg + validation.ScreenTab + "<br>" : (validation.ScreenTab + "<br>") : sMsg;
                            //sMsg = sMsg + validation.DisplayText + "<br>";
                        }
                        if(!string.IsNullOrEmpty(checkAttachmentIn))
                            sMsg =  CheckAttachment(validation, attachmentTempDocList, empUploadedDocList, empId, refrenceId, checkAttachmentIn, sMsg);
                        checkAttachmentIn = string.Empty;
                    }
                }
            }
        }

        empVM.DisplayMessage = sMsg == "" ? "Success" : sMsg;
        return empVM;
    }
    public string CheckAttachment(dynamic validation, List<EmployeeTempDocUploadDTO> attachmentTempDocList, List<EmployeeUploadDocumentDTO> empUploadedDocList, int empId, int refrenceId, string sCheckAttachmentIn,string sMsg)
    {
        //string sMsg = string.Empty;
        if (sCheckAttachmentIn == "BOTH")
        {
            if (!attachmentTempDocList.Exists(r => r.FieldName.Trim().ToLower() == validation.FieldName.Trim().ToLower() && r.IsActive == true) && (!empUploadedDocList.Select(r => r.FieldName.Trim().ToLower()).ToList().Contains(validation.FieldName.Trim().ToLower())))
            {
                sMsg = sMsg.IndexOf(validation.ScreenTab) < 0 ? sMsg.Length > 0 ? sMsg + validation.ScreenTab + "<br>" : (validation.ScreenTab + "<br>") : sMsg;
                sMsg = sMsg + validation.DisplayText + "<br>";
            }
        }
        else if(sCheckAttachmentIn == "TEMPTABLE")
        {
            if (!attachmentTempDocList.Exists(r => r.FieldName.Trim().ToLower() == validation.FieldName.Trim().ToLower() && r.IsActive == true))
            {
                sMsg = sMsg.IndexOf(validation.ScreenTab) < 0 ? sMsg.Length > 0 ? sMsg + validation.ScreenTab + "<br>" : (validation.ScreenTab + "<br>") : sMsg;
                sMsg = sMsg + validation.DisplayText + "<br>";
            }
        }
        
        else if(sCheckAttachmentIn == "FINALTABLE")
        {
            if (!empUploadedDocList.Select(r => r.FieldName.Trim().ToLower()).ToList().Contains(validation.FieldName.Trim().ToLower()))
            {
                sMsg = sMsg.IndexOf(validation.ScreenTab) < 0 ? sMsg.Length > 0 ? sMsg + validation.ScreenTab + "<br>" : (validation.ScreenTab + "<br>") : sMsg;
                sMsg = sMsg + validation.DisplayText + "<br>";
            }
        }
        return sMsg;
    }

    [HttpPost]
    public async Task<EmployeeUploadDocumentDTO> AddEmployeeDocInTempTable(EmployeeDocumentsDTO filesData)
    {
        dynamic actionResult = null;
        EmployeeUploadDocumentDTO employeeUploadDocument = new EmployeeUploadDocumentDTO();
        if (filesData.UploadedFile != null)
        {
            foreach (var file in filesData.UploadedFile)
            {
                if (file != null)
                {
                    if (file.Length > 0)
                    {
                        int typeId;
                        //bool success = int.TryParse(Path.GetFileNameWithoutExtension(file.FileName), out typeId);
                        string docType = Path.GetExtension(file.FileName);
                        // if (success)
                        //{
                        // EmployeeUploadDocument.DcumentTypeId = Convert.ToInt32(typeId);
                        using (var target = new MemoryStream())
                        {
                            file.CopyTo(target);
                            employeeUploadDocument.EmployeeDocument = target.ToArray();
                            employeeUploadDocument.EmployeeId = filesData.EmployeeId;
                            employeeUploadDocument.UploadDcumentDetailId = filesData.UploadDcumentDetailId;
                            employeeUploadDocument.FormName = filesData.FormName;
                            employeeUploadDocument.ClientId = filesData.ClientId;
                            employeeUploadDocument.DocumentType = (docType.IsNullOrEmpty() ? "unknown" : docType.Replace(".", ""));
                            if (filesData.EmployeeId != 0)
                            {
                                if (employeeUploadDocument.UploadDcumentDetailId.ToString().Equals("0"))
                                {
                                    actionResult = _employeeAPIController.SaveEmployeeDocs(employeeUploadDocument);
                                    employeeUploadDocument.DisplayMessage = Convert.ToString(actionResult.Value);
                                }
                            }
                        }
                        //}
                    }
                }
            }
        }
        return employeeUploadDocument;
    }

}
