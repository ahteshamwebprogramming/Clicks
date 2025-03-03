using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System.Linq.Expressions;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using SimpliHR.Services.DBContext;
using System.Text.RegularExpressions;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Core.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Endpoints;
using Microsoft.EntityFrameworkCore.Migrations;
using Dapper;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SimpliHR.Infrastructure.Models.Masters;
using System.Data;

[Route("api/[controller]/[action]")]
[ApiController]
public class EmployeeUploadDocumentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeUploadDocumentController> _logger;
    private readonly IMapper _mapper;

    public EmployeeUploadDocumentController(IUnitOfWork unitOfWork, ILogger<EmployeeUploadDocumentController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<string> GetTypeOldValue(string changeType, int? employeeId, int? refId)
    {
        string tableName = "", tableIdCol = "";
        string oldValue = "";
        List<string> fieldValue = new List<string>();
        EmployeeValidationMasterDTO empValidInfo = new EmployeeValidationMasterDTO();
        string sSql = $"SELECT TableName,TableIdColumn from EmployeeValidationMaster WHERE FieldName=@changeType";
        var objData = new { @changeType = changeType };

        empValidInfo = (await _unitOfWork.EmployeeValidationMaster.ExecuteQuery<EmployeeValidationMasterDTO>(sSql, objData)).FirstOrDefault();
        if (empValidInfo != null)
        {
            sSql = $"SELECT " + @changeType + " FROM " + empValidInfo.TableName + " WHERE EmployeeId=@employeeId";
            dynamic objData1;
            if (!(refId == 0 || refId == null || tableIdCol.ToLower() == "employeeid"))
            {
                sSql = sSql + $" AND {empValidInfo.TableIdColumn}=@refId";
                objData1 = new { @changeType = changeType, @employeeId = employeeId, refId = @refId };
            }
            else
                objData1 = new { @employeeId = employeeId };
            fieldValue = (await _unitOfWork.EmployeeValidationMaster.ExecuteQuery<string>(sSql, objData1));
            oldValue = fieldValue.FirstOrDefault();
        }
        return oldValue;
    }
    
    public async Task<EditEmployeeDataDTO> SaveEmployeeEditInfo(EmployeeTempDocUploadDTO inputDTO)
    {
        IActionResult returnMessage;
        EditEmployeeDataDTO? editEmployeeData = new EditEmployeeDataDTO();
        if (inputDTO != null)
        {
            //Generate TicketId
            if (inputDTO.TicketId == null || inputDTO.TicketId == "")
            {
               
                inputDTO.TicketId = CommonHelper.CreateTicket("EMPEdit", inputDTO.TicketId); // creates a 8 digit random no.
            }
            
            editEmployeeData.EmployeeId = inputDTO.EmployeeId;
            editEmployeeData.TicketId = inputDTO.TicketId;
            editEmployeeData.DocumentType = inputDTO.DocumentType;
            editEmployeeData.ChangeType = inputDTO.FieldName;
            editEmployeeData.ChangeValue = inputDTO.ChangeValue;
            editEmployeeData.Attachment = inputDTO.UploadedFile;
            editEmployeeData.EntrySource = inputDTO.EntrySource;
            editEmployeeData.IsActive = true;
            editEmployeeData.IsApproved = 3;


            EditEmployeeData editData = new EditEmployeeData();
            editData = await _unitOfWork.EditEmployeeData.GetFilter(x => x.ChangeType.Trim() == editEmployeeData.ChangeType.Trim() && x.EntrySource == editEmployeeData.EntrySource && x.EmployeeId == editEmployeeData.EmployeeId && (x.IsApproved == null || x.IsApproved == 0) && x.IsActive == true);
            int validationId = (await _unitOfWork.EmployeeValidationMaster.GetFilter(x => x.FieldName.ToLower().Trim() == editEmployeeData.ChangeType.ToLower().Trim())).EmployeeValidationId;

            if (editData == null)
            {
                editEmployeeData.OldValue = await GetTypeOldValue(editEmployeeData.ChangeType, editEmployeeData.EmployeeId, editEmployeeData.TableReferenceId);
                editEmployeeData.CreatedBy = inputDTO.LoggedInUser;
                editEmployeeData.CreatedOn = DateTime.Now;
                editEmployeeData.IsApproved = 0;
                editEmployeeData.EmployeeValidationId = validationId;
                await _unitOfWork.EditEmployeeData.AddAsync(_mapper.Map<EditEmployeeData>(editEmployeeData));
                _unitOfWork.Save();
                editEmployeeData.DisplayMessage = "Success";
            }
            else
            {
                if(editData.TicketId!= editEmployeeData.TicketId)
                {
                    editEmployeeData.DisplayMessage = $"Edit Request is already pending with Ticket ID : {editData.TicketId}";
                }
                else 
                {
                    editEmployeeData.OldValue = await GetTypeOldValue(editEmployeeData.ChangeType, editEmployeeData.EmployeeId, editEmployeeData.TableReferenceId);
                    editData.Attachment = editEmployeeData.Attachment;
                    editData.DocumentType = editEmployeeData.DocumentType;
                    editData.ChangeValue = editEmployeeData.ChangeValue;
                    editData.ModifiedBy = inputDTO.LoggedInUser;
                    editData.IsApproved = null;
                    editData.ModifiedOn = DateTime.Now;
                    editData.EmployeeValidationId = validationId;
                    editData.ModifiedBy = editEmployeeData.EmployeeId;
                    await _unitOfWork.EditEmployeeData.UpdateAsync(_mapper.Map<EditEmployeeData>(editData));
                    _unitOfWork.Save();
                    editEmployeeData.DisplayMessage = "Success";
                }

            }
            //_unitOfWork.Save();

           // editEmployeeData.DisplayMessage = "Success";
            
        }
        else
            editEmployeeData.DisplayMessage = "Data is not foud to save";
        
        
        return editEmployeeData;
    }

    public async Task<List<EmployeeTempDocUploadDTO>> GetEmployeeTempDocUpload(string sTabName,string sessionId)
    {
        List<EmployeeTempDocUploadDTO> attachmentList = new List<EmployeeTempDocUploadDTO>();
        //string sQuery = "Select [SessionId],[FieldName],[ReferenceId] ,[ScreenTab],[DcumentTypeId],[DocumentType],[UploadedFile],[EmployeeId],[ActionStatus],[ActionBy],[IsActive] FROM EmployeeTempDocUpload WHERE SessionId=@sessionId " +
        string sQuery = $"Select [SessionId],a.[FieldName],[ReferenceId],b.[ScreenTab],[DcumentTypeId],[DocumentType],[UploadedFile],[EmployeeId]" +
            $" ,[ActionStatus],[ActionBy],a.[IsActive] FROM EmployeeTempDocUpload a INNER JOIN EmployeeValidationMaster b ON a.FieldName = b.FieldName " +
            $" WHERE a.[IsActive]=1 AND SessionId = @sessionId  ";
            //AND Replace(b.ScreenTab,' ','') = Case WHEN b.ScreenTab IS NULL THEN null ELSE @screenTab END";
         //(sTabName!="" ? " AND Replace(ScreenTab,' ','') = Case WHEN ScreenTab IS NULL THEN ScreenTab ELSE @screenTab END" : "");
        //if (sTabName == "CurrentAddress")
        //    sTabName = "'CurrentAddress','PermanentAddress'";
        attachmentList = await _unitOfWork.EmployeeTempDocUpload.ExecuteQuery<EmployeeTempDocUploadDTO>(sQuery, new { @sessionId = sessionId, @screenTab=sTabName });
        return attachmentList;
    }

    public List<EmployeeUploadDocumentDTO> GetEmployeeUploadedDoc(int empId, int refrenceId,int prefrenceId)
    {
        List<EmployeeUploadDocumentDTO> empUploadedDocList = new List<EmployeeUploadDocumentDTO>();
        Expression<Func<EmployeeUploadDocument, bool>> expression = null;
         if (empId!=0 && refrenceId != 0 && prefrenceId!=0)
            expression = x => x.EmployeeId == empId && (x.ReferenceId == refrenceId || x.ReferenceId==prefrenceId) && x.IsActive == true;
        else if (empId != 0 && refrenceId != 0)
            expression = x => x.EmployeeId == empId && (x.ReferenceId == refrenceId ) && x.IsActive == true;
        else if (empId != 0 &&  prefrenceId != 0)
            expression = x => x.EmployeeId == empId && (x.ReferenceId == prefrenceId) && x.IsActive == true;
        else if(empId!=0)
            expression= x => x.EmployeeId == empId && x.IsActive == true;
        
        if(expression != null)
            empUploadedDocList = _mapper.Map<List<EmployeeUploadDocumentDTO>>(_unitOfWork.EmployeeUploadDocument.FindAllByExpression(expression));
        return empUploadedDocList;
    }
    public async Task<List<EmployeeValidationDTO>> GetEmployeeValidation(string sTabName, int clientId,int unitId)
    {
        List<EmployeeValidationDTO> empValidationList = new List<EmployeeValidationDTO>();
        var parms = new DynamicParameters();
        parms.Add(@"@unitId", unitId, DbType.Int32);
        parms.Add(@"@ClientId", clientId, DbType.Int32);
        parms.Add(@"@ScreenName", "", DbType.String);
        parms.Add(@"@ScreenTab", sTabName, DbType.String);
        empValidationList = _mapper.Map<List<EmployeeValidationDTO>>(await _unitOfWork.EmployeeValidation.GetSPData("GetEmployeeValidation", parms));
        return  empValidationList;
    }
    

    [HttpPost]
    public async Task<IActionResult> SaveEmployeeTempDocUpload(EmployeeTempDocUploadDTO inputDTO)
    {
        try
        {
            IActionResult returnMessage = Ok(string.Empty);
            if (ModelState.IsValid)
            {

                //if (inputDTO.EntrySource == "EmployeeEditScreen")
                //{
                //    returnMessage = await SaveEmployeeEditInfo(inputDTO);
                //}
                //else
                //{
                string sSql = "DELETE  FROM EmployeeTempDocUpload  WHERE SessionId!=@sessionId AND CreatedBy=@loggedInUser";
                var pram = new { @sessionId = inputDTO.SessionId, @loggedInUser = inputDTO.LoggedInUser };
                await _unitOfWork.EmployeeTempDocUpload.ExecuteQuery(sSql, pram);

                sSql = "Select top 1 ScreenTab  FROM EmployeeValidationMaster WHERE FieldName=@fieldName";
                var data = new { @fieldName = inputDTO.FieldName };
                string screenTab = (await _unitOfWork.EmployeeValidation.ExecuteQuery<EmployeeValidationMaster>(sSql, data)).Select(x => x.ScreenTab).FirstOrDefault().ToString();
                
                sSql = "SELECT [SessionId],[FieldName],[UploadedFile],[EmployeeId],[CreatedOn],[CreatedBy],[ModifiedOn],[IsActive] FROM EmployeeTempDocUpload a WHERE a.SessionId=@sessionId AND UPPER(LTRIM(RTRIM(a.FieldName)))=@fieldName AND a.IsActive=1 AND a.EmployeeId=(CASE WHEN @employeeId=0 THEN a.EmployeeId ELSE @employeeId END)";
                var objData = new { @sessionId = inputDTO.SessionId, @employeeId = inputDTO.EmployeeId.Value, @fieldName = inputDTO.FieldName.Trim().ToUpper() };
                bool isExists = await _unitOfWork.EmployeeTempDocUpload.IsExists(sSql, objData);

                if (!isExists)
                {
                    inputDTO.CreatedOn = DateTime.Now;
                    inputDTO.CreatedBy = inputDTO.LoggedInUser;
                    inputDTO.ScreenTab = screenTab;
                    _unitOfWork.EmployeeTempDocUpload.AddAsync(_mapper.Map<EmployeeTempDocUpload>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                {
                    EmployeeTempDocUploadDTO tempEmployeeDoc = new EmployeeTempDocUploadDTO();
                    tempEmployeeDoc = _mapper.Map<EmployeeTempDocUploadDTO>((await _unitOfWork.EmployeeTempDocUpload.GetDynamicQuery(sSql, objData)).FirstOrDefault());
                    tempEmployeeDoc.ModifiedOn = DateTime.Now;
                    tempEmployeeDoc.ModifiedBy = inputDTO.LoggedInUser;

                    if (tempEmployeeDoc != null)
                    {
                        tempEmployeeDoc.UploadedFile = inputDTO.UploadedFile;
                        tempEmployeeDoc.DocumentType = inputDTO.DocumentType;
                        string sProperties = "UploadedFile";
                        if (await _unitOfWork.EmployeeTempDocUpload.UpdateAsync(_mapper.Map<EmployeeTempDocUpload>(tempEmployeeDoc)))
                            _unitOfWork.Save();
                        else
                            return Ok("Fail to update file");
                        return Ok("Success");
                    }
                }
            }

            //}
            else
                return Ok("Invalid Model");

            return Ok(returnMessage);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Document Information {nameof(SaveEmployeeTempDocUpload)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeDocs(EmployeeUploadDocumentDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeUploadDocument, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.DcumentTypeId == inputDTO.DcumentTypeId && a.IsActive == true;
                if (!_unitOfWork.EmployeeUploadDocument.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<EmployeeAcademicDetail>(inputDTO);
                    _unitOfWork.EmployeeUploadDocument.AddAsync(_mapper.Map<EmployeeUploadDocument>(inputDTO));
                    _unitOfWork.Save();
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                }
                else
                {
                    EmployeeUploadDocumentDTO tempEmployeeDoc = new EmployeeUploadDocumentDTO();
                    tempEmployeeDoc = _mapper.Map<EmployeeUploadDocumentDTO>(_unitOfWork.EmployeeUploadDocument.FindFirstByExpression(expression));
                    if (tempEmployeeDoc != null)
                    {
                        tempEmployeeDoc.EmployeeDocument = inputDTO.EmployeeDocument;
                        string sProperties = "EmployeeDocument";
                        _unitOfWork.EmployeeUploadDocument.UpdateDbEntry(_mapper.Map<EmployeeUploadDocument>(tempEmployeeDoc), sProperties);
                        _unitOfWork.Save();
                    }
                }
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Document Information {nameof(SaveEmployeeDocs)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult UpdateEmployeeDocs(EmployeeUploadDocumentDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.EmployeeUploadDocument.Update(_mapper.Map<EmployeeUploadDocument>(inputDTO));
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in updating Employee document Details {nameof(UpdateEmployeeDocs)}");
            throw;
        }

    }

    [HttpPost]
    public async Task<IActionResult> ListEmployeeDocs(EmployeeUploadDocumentDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                IList<EmployeeUploadDocument> outputModel = new List<EmployeeUploadDocument>();
                Expression<Func<EmployeeUploadDocument, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.DcumentTypeId == inputDTO.DcumentTypeId && a.IsActive == true;

                outputModel = _mapper.Map<IList<EmployeeUploadDocument>>(await _unitOfWork.EmployeeUploadDocument.GetPagedListWithExpression(null, expression, null)).ToList();
                _unitOfWork.Save();
                return Ok(outputModel);
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in updating Employee document Details {nameof(UpdateEmployeeDocs)}");
            throw;
        }

    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeAcademicDetail(EmployeeUploadDocumentDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeUploadDocument outputMaster = _mapper.Map<EmployeeUploadDocument>(await _unitOfWork.EmployeeAcademicDetail.GetByIdAsync(inputDTO.UploadDcumentDetailId));
                outputMaster.IsActive = false;
                _unitOfWork.EmployeeUploadDocument.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee Academic Detail {nameof(DeleteEmployeeAcademicDetail)}");
            throw;
        }
    }

}
