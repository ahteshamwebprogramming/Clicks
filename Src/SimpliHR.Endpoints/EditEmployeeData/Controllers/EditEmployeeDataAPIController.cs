using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Master;
using System.Linq.Expressions;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Common;
using System.Collections.Generic;
using SimpliHR.Infrastructure.Helper;

namespace SimpliHR.Endpoints.EditEmployeeData;

[Route("api/[controller]")]
[ApiController]
public class EditEmployeeDataAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EditEmployeeDataAPIController> _logger;
    private readonly IMapper _mapper;
    public EditEmployeeDataAPIController(IUnitOfWork unitOfWork, ILogger<EditEmployeeDataAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }



    public async Task<IActionResult> SaveDeleteEmployeeDetailForApproval(AddDeleteTableActionDTO? deleteEmployeeData)
    {
        try
        {
            string sSql = "INSERT INTO [dbo].[AddDeleteTableAction]" +
                "([ActionBy],[ActionStatus],[ActionType],[TicketId],[Ref" +
                "erenceTable],[ReferenceId],[EmployeeId],[EntrySource],[CreatedOn]" +
                ",[CreatedBy],[IsActive])" +
                "VALUES(" + deleteEmployeeData.ActionBy + "," + deleteEmployeeData.ActionStatus + ",'" + deleteEmployeeData.ActionType 
                + "','" + deleteEmployeeData.TicketId + "','" + deleteEmployeeData.ReferenceTable + "'," + deleteEmployeeData.ReferenceId + "," + deleteEmployeeData.EmployeeId
                + ",'" + deleteEmployeeData.EntrySource + "','" +  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + deleteEmployeeData.LoggedInUser + ",1" + ")";
            await _unitOfWork.EditEmployeeData.RunSQLCommand(sSql);
           
            return Ok("Success");
           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveDeleteEmployeeDetailForApproval)}");
            throw;
        }

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
                objData1 = new { @changeType = changeType, @employeeId = employeeId, @refId = refId };
            }

            else
                objData1 = new { @employeeId = employeeId };
            fieldValue = (await _unitOfWork.EmployeeValidationMaster.ExecuteQuery<string>(sSql, objData1));
            oldValue = fieldValue.FirstOrDefault();
        }
        return oldValue;
    }

    public async Task<EditEmployeeDataVM> SaveEmployeeChangesForApproval(EditEmployeeDataVM editEmployeeData)
    {
        //editEmployeeData.DisplayMessage = "Success";
        editEmployeeData.DisplayMessage = "Success";
        foreach (var empEditData in editEmployeeData.EditEmployeeDataList)
        {
            empEditData.OldValue = await GetTypeOldValue(empEditData.ChangeType, empEditData.EmployeeId, empEditData.TableReferenceId);
            var actionResult = await SaveEmployeeChangesForApproval(empEditData);
            ObjectResult objResult = (ObjectResult)actionResult;
            string sResult = (string)objResult.Value;
            if (sResult != "Success")
            {
                editEmployeeData.ServerMessage = editEmployeeData.ServerMessage != "_blank" ? $"{CommonHelper.NewLineEntry()} {empEditData.ChangeType}:{sResult}" : sResult;
                editEmployeeData.DisplayMessage = "Error";
            }
        }
        
        return editEmployeeData;
    }

    public async Task<IActionResult> GetAllSessionAttachment(string sessionId,string fieldNames)
    {
        fieldNames = "'" + fieldNames.Replace(",", "','") + "'";
        // string sSql = $"SELECT SessionId,FieldName,ReferenceId,ScreenTab,DcumentTypeId,DocumentType,UploadedFile,EmployeeId,ActionStatus,ActionBy,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,IsActive FROM EmployeeTempDocUpload " +
        // $" WHERE FieldName IN(@fieldNames) AND SessionId=@sessionId";
        // List<EmployeeTempDocUploadDTO> empTempDocList = new List<EmployeeTempDocUploadDTO>();
        //empTempDocList = await _unitOfWork.EmployeeTempDocUpload.ExecuteQuery<EmployeeTempDocUploadDTO>(sSql, new { @fieldNames = fieldNames, @sessionId = sessionId });
        string sSql = $"SELECT SessionId,FieldName,ReferenceId,ScreenTab,DcumentTypeId,DocumentType,UploadedFile,EmployeeId,ActionStatus,ActionBy,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,IsActive FROM EmployeeTempDocUpload " +
       $" WHERE FieldName IN({fieldNames}) AND SessionId='{sessionId}'";
        List<EmployeeTempDocUploadDTO> empTempDocList = new List<EmployeeTempDocUploadDTO>();
        empTempDocList = await _unitOfWork.EmployeeTempDocUpload.GetTableData<EmployeeTempDocUploadDTO>(sSql);
        return Ok(empTempDocList);
    }

   

    public async Task<IActionResult> SaveEmployeeChangesForApproval(EditEmployeeDataDTO? editEmployeeData)
    {
        try
        {
            string retMsg = string.Empty;
            if (editEmployeeData != null)
            {
                Core.Entities.EditEmployeeData editData = new Core.Entities.EditEmployeeData();
                editData = await _unitOfWork.EditEmployeeData.GetFilter(x => x.ChangeType.Trim() == editEmployeeData.ChangeType.Trim() && x.EntrySource == editEmployeeData.EntrySource && x.EmployeeId == editEmployeeData.EmployeeId && (x.IsApproved == null || x.IsApproved == 0) && x.IsActive == true);
                int validationId = (await _unitOfWork.EmployeeValidationMaster.GetFilter(x => x.FieldName.ToLower().Trim() == editEmployeeData.ChangeType.ToLower().Trim())).EmployeeValidationId;

                if (editData == null)
                {
                    //Random rnd = new Random();
                    //editEmployeeData.TicketeId = rnd.Next(10000, 999999999); // creates a 8 digit random no.
                    editEmployeeData.CreatedBy = editEmployeeData.LoggedInUser;
                    editEmployeeData.CreatedOn = DateTime.Now;
                    editEmployeeData.IsApproved = 0;
                    editEmployeeData.EmployeeValidationId = validationId;
                    await _unitOfWork.EditEmployeeData.AddAsync(_mapper.Map<Core.Entities.EditEmployeeData>(editEmployeeData));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                {
                    if (editData.TicketId != editEmployeeData.TicketId)
                    {
                        editEmployeeData.DisplayMessage = $"Edit Request is already pending for {editData.ChangeType} with Ticket ID : {editData.TicketId}";
                        return Ok(editEmployeeData.DisplayMessage);
                    }
                    else
                    {
                        editData.Attachment = editEmployeeData.Attachment;
                        editData.DocumentType = editEmployeeData.DocumentType;
                        editData.ChangeValue = editEmployeeData.ChangeValue;
                        editEmployeeData.ModifiedBy = editEmployeeData.LoggedInUser;
                        editEmployeeData.IsApproved = null;
                        editEmployeeData.ModifiedOn = DateTime.Now;
                        editData.EmployeeValidationId = validationId;
                        //editData.ModifiedBy = editEmployeeData.EmployeeId;
                        await _unitOfWork.EditEmployeeData.UpdateAsync(_mapper.Map<Core.Entities.EditEmployeeData>(editData));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                }
                
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "No Data Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeChangesForApproval)}");
            throw;
        }

    }

    public async Task<IActionResult> SaveProfileChangedRequest(List<EditEmployeeDataDTO>? inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {

                foreach (var item in inputDTO)
                {
                    await _unitOfWork.EditEmployeeData.AddAsync(_mapper.Map<Core.Entities.EditEmployeeData>(item));
                    _unitOfWork.Save();
                }


                return Ok("Success");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "No Data Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveProfileChangedRequest)}");
            throw;
        }
    }
}
