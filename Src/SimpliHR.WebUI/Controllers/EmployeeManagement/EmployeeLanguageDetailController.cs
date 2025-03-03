using SimpliHR.Endpoints.Masters;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpliHR.Infrastructure.Models.Common;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Helper;

namespace SimpliHR.WebUI.Controllers.EmployeeManagement;

public class EmployeeLanguageDetailController : Controller
{
    private readonly EmployeeLanguageDetailAPIController _employeeLanguageDetailAPIController;
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly EmployeeMasterController _employeeMasterController;
    public EmployeeLanguageDetailController(EmployeeLanguageDetailAPIController employeeLanguageDetailAPIController, EmployeeMasterController employeeMasterController, MastersKeyValueController mastersKeyValueController)
    {
        _employeeLanguageDetailAPIController = employeeLanguageDetailAPIController;
        _mastersKeyValueController = mastersKeyValueController;
        _employeeMasterController = employeeMasterController;
    }

    [HttpPost]
    public async Task<IActionResult> SaveEditEmployeeLanguageInfo(EmployeeLanguageDetailDTO inputData)
    {
        //inputData.IsAdel = new EmployeeFamilyDetailDTO();
        dynamic actionResult;
        string attachmentMsg = string.Empty;
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
                empTempDoc.ScreenTab = "Language";
                empTempDoc.SessionId = HttpContext.Session.Id;
                empTempDoc.ReferenceId = inputData.EmployeeLanguageDetailId;
                empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                empTempDoc.EmployeeId = inputData.EmployeeId;
                attachmentMsg = await _employeeMasterController.VerifyRequiredAttachment(empTempDoc, clientId, unitId);
                if (!string.IsNullOrEmpty(attachmentMsg))
                {
                    objResultData.DisplayMessage = attachmentMsg;
                }
                else
                {
                    actionResult = _employeeLanguageDetailAPIController.SaveEditEmployeeLanguageDetail(inputData);
                    //else
                    //{
                    //    inputData.IsActive = inputData.IsActive == false ? inputData.IsActive = true : true;
                    //    actionResult = _employeeFamilyAPIController.UpdateEmployeeFamilyDetail(inputData);
                    //}

                    string sDetailId = "";
                    sDetailId = actionResult.Value.ToString();
                    int detailId = 0;
                    if (inputData.EmployeeLanguageDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                    {
                        inputData.EmployeeLanguageDetailId = detailId;
                        AddDeleteTableActionDTO? addDetailForApproval = new AddDeleteTableActionDTO();
                        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

                        addDetailForApproval.ActionBy = empSession.EmployeeId;
                        addDetailForApproval.ActionStatus = 0;
                        addDetailForApproval.ActionType = "Add";
                        inputData.TicketId = CommonHelper.CreateTicket("EMPEdit", inputData.TicketId);
                        addDetailForApproval.TicketId = inputData.TicketId;
                        addDetailForApproval.ReferenceTable = "EmployeeLanguageDetail";
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


            objResultData.EmployeeLanguageDetails = await GetEmployeeLanguageList(inputData.EmployeeId);
        }
        return Ok(objResultData);
    }

    [HttpPost]
    public async Task<IActionResult> SaveEmployeeLanguageInfo(EmployeeLanguageDetailDTO inputData)
    {
        try
        {

            EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
            inputData.IsActive = true;
            dynamic objRes;
            if (inputData != null)
            {
                if (!inputData.EmployeeId.ToString().Equals("0"))
                {
                    if (inputData.EmployeeLanguageDetailId.ToString().Equals("0"))
                    {
                        inputData.CreatedOn = DateTime.Now;
                        objRes = _employeeLanguageDetailAPIController.SaveEditEmployeeLanguageDetail(inputData);
                    }
                    else
                    {
                        objRes = _employeeLanguageDetailAPIController.UpdateEmployeeLanguageDetail(inputData);
                    }
                    if (objRes != null)
                    {
                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                        {
                            string sDetailId = "";
                            sDetailId = objRes.Value.ToString();
                            int detailId = 0;
                            if (inputData.EmployeeLanguageDetailId.ToString().Equals("0") && int.TryParse(sDetailId, out detailId))
                            {
                                inputData.EmployeeLanguageDetailId = detailId;
                                objResultData.DisplayMessage = "Success";
                            }
                            if (inputData.EmployeeLanguageDetailId != 0)
                            {
                                EmployeeTempDocUploadDTO empTempDoc = new EmployeeTempDocUploadDTO();
                                empTempDoc.ScreenTab = "Language";
                                empTempDoc.SessionId = HttpContext.Session.Id;
                                empTempDoc.ReferenceId = inputData.EmployeeLanguageDetailId;
                                empTempDoc.LoggedInUser = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
                                empTempDoc.EmployeeId = inputData.EmployeeId;
                                empTempDoc.ReferenceId = inputData.EmployeeLanguageDetailId;
                                string attachmentMsg = await _employeeMasterController.SaveAttachment(empTempDoc);
                                if (attachmentMsg != "Success")
                                {
                                    objResultData.DisplayMessage = attachmentMsg;
                                }
                            }
                            List<LanguageKeyValues> languageKeyValue = new List<LanguageKeyValues>();
                            languageKeyValue = await _mastersKeyValueController.LanguageKeyValue();
                            objResultData.EmployeeLanguageDetails = await GetEmployeeLanguageList(inputData.EmployeeId);
                            objResultData.EmployeeLanguageDetails.ToList().ForEach(p => { p.Language = languageKeyValue.Where(x => x.LanguageId == p.LanguageId).Select(r => r.Language).FirstOrDefault(); });
                            return Ok(objResultData);
                        }
                        else
                        {
                            objResultData.DisplayMessage = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).Value.ToString();
                            return Ok(objResultData);
                        }
                    }
                }
                else
                {
                    throw new Exception("In case you are adding new employee please fill Personal Details first");
                }
            }
            else
            {
                throw new Exception("Invalid Data");
            }
            objResultData.DisplayMessage = "Error";
            return Ok(objResultData);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<List<EmployeeLanguageDetailDTO>?> GetEmployeeLanguageList(int? employeeID)
    {

        IActionResult actionResult = await _employeeLanguageDetailAPIController.GetEmployeeLanguageList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, employeeID);
        ObjectResult objResult = (ObjectResult)actionResult;
        List<EmployeeLanguageDetailDTO> objResultData = (List<EmployeeLanguageDetailDTO>)objResult.Value;
        //List<JobTitleKeyValues> keyValueList = _mastersKeyValueController.JobTitleKeyValue().Result;
        //foreach (var row in objResultData)
        //    row.JobTitle = keyValueList.Where(r => r.JobTitleId == row.ExperienceJobTitleId).Select(x => x.JobTitle).FirstOrDefault();
        return objResultData;

    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeLanguageDetail(EmployeeLanguageDetailDTO inputData)
    {
        try
        {
            EmployeeMasterDTO objResultData = new EmployeeMasterDTO();
            if (inputData != null)
            {
                if (!inputData.EmployeeLanguageDetailId.ToString().Equals("0"))
                {
                    var objRes = await _employeeLanguageDetailAPIController.DeleteEmployeeLanguageDetail(inputData);
                    if (objRes != null)
                    {
                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                        {
                            objResultData.EmployeeLanguageDetails = await GetEmployeeLanguageList(inputData.EmployeeId);
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
