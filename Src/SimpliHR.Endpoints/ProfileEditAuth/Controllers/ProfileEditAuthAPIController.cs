using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SimpliHR.Infrastructure.Models.ProfileEditAuth;
using SimpliHR.Core.Entities;
using System.Drawing;
using Microsoft.AspNetCore.Components.Forms;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Common;
using SimpliHR.Infrastructure.Models.Masters;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using SimpliHR.Infrastructure.Helper;
using System.Diagnostics.Eventing.Reader;
using System.Data;
using Microsoft.AspNetCore.Hosting.Server;

namespace SimpliHR.Endpoints.ProfileEditAuth;

[Route("api/[controller]")]
[ApiController]
public class ProfileEditAuthAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProfileEditAuthAPIController> _logger;
    private readonly IMapper _mapper;
    public ProfileEditAuthAPIController(IUnitOfWork unitOfWork, ILogger<ProfileEditAuthAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        // _simpliDbContext = SimpliDbContext;
    }

    public async Task<IActionResult> GetProfileEditAuthTable(int unitId)
    {
        try
        {
            List<ProfileEditAuthDTO> outputModel = new List<ProfileEditAuthDTO>();

            //string sQuery = "select ecm.ClearanceMappingId,ecm.PrimaryClearancePerson,ecm.SecondaryClearancePerson,ecm.DepartmentId,em.employeename as PrimaryClearancePersonName,em1.employeename as SecondaryClearancePersonName,dm.DepartmentName as DepartmentName from  [dbo].[ExitClearanceMapping] ecm  join employeemaster em on ecm.PrimaryClearancePerson=em.employeeid join employeemaster em1 on ecm.SecondaryClearancePerson=em1.employeeid join departmentmaster dm on ecm.DepartmentId=dm.departmentId where ecm.isactive=1 and ecm.unitid=" + unitId + "";
            string sQuery = "select ProfileFieldId,ProfileFieldName,isnull((select IsEditable from ProfileEditAuth pea where pf.ProfileFieldId = pea.ProfileFieldId and pea.unitID = " + unitId + " and pea.IsActive = 1),0)IsEditable,isnull((select AttachmentRequired from ProfileEditAuth pea where pf.ProfileFieldId = pea.ProfileFieldId and pea.unitID = 5 and pea.IsActive = 1),0)AttachmentRequired from ProfileField pf where pf.IsActive = 1";

            List<ProfileEditAuthDTO> dto = await _unitOfWork.ProfileEditAuth.GetTableData<ProfileEditAuthDTO>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProfileEditAuthTable)}");
            throw;
        }
    }
    public async Task<IActionResult> GetEmployeeEditTicketList(int clientId, int UnitId)
    {
        try
        {
            string sQuery = "";

            sQuery = @"select ee.TicketID TicketId,ee.EmployeeId
                            ,(select EmployeeName from EmployeeMaster em where em.EmployeeId=ee.EmployeeId)EmployeeName
                            ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ee.EmployeeId)Department
                            from 
                            EditEmployeeData ee
                            join 
                            EmployeeMaster em
							on em.EmployeeId=ee.EmployeeId
							join 
							EmployeeValidationMaster evm
							on ee.EmployeeValidationId=evm.EmployeeValidationId
                            where em.UnitId=" + UnitId + " and isnull(IsApproved,0)=0  Group by ee.TicketID,ee.EmployeeId union select TicketId "
                            + " ,ad.EmployeeId "
                            + " ,(select EmployeeName from EmployeeMaster em where em.employeeid=ad.EmployeeId)EmployeeName"
                            + " ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ad.EmployeeId)Department"
                            + " from [dbo].[AddDeleteTableAction] ad where IsActive=1 and ActionStatus=0  and EmployeeId in (select EmployeeId from EmployeeMaster where  UnitId=" + UnitId + ") Group by" + " ad.TicketID,ad.EmployeeId";

            List<EmployeeEditTicketViewModel> dto = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeEditTicketViewModel>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeEditTicketList)}");
            throw;
        }
    }

    public async Task<IActionResult> GetEmployeeEditTicketListApproved(int clientId)
    {
        try
        {
            string sQuery = "";
            sQuery = @"select ee.TicketID TicketId,ee.EmployeeId
                            ,(select EmployeeName from EmployeeMaster em where em.EmployeeId=ee.EmployeeId)EmployeeName
                            ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ee.EmployeeId)Department
                            from 
                            EmployeeValidation ev 
                            join EditEmployeeData ee
                            on ev.EmployeeValidationId=ee.EmployeeValidationId 
                            where ev.ClientId=" + clientId + " and ticketid not in (select TicketId from EditEmployeeData where isnull(isapproved,0)=0  union select TicketId from [AddDeleteTableAction] where ActionStatus=0)  "
                          + " Group by ee.TicketID,ee.EmployeeId "
                          + " union "
                          + "select ev.TicketID TicketId"
                          + " ,ev.EmployeeId"
                          + " ,(select EmployeeName from EmployeeMaster em where em.EmployeeId=ev.EmployeeId)EmployeeName"
                          + " ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ev.EmployeeId)Department"
                          + " from "
                          + " [AddDeleteTableAction] ev "
                          + " where IsActive=1 and EmployeeId in (select EmployeeId from EmployeeMaster where ClientId=" + clientId + ") and TicketId not in (select TicketId from [AddDeleteTableAction] where ActionStatus=0"
                          + "  union select TicketId from EditEmployeeData where isnull(isapproved,0)=0 )"
                          + " Group by ev.TicketID,ev.EmployeeId";


            List<EmployeeEditTicketViewModel> dto = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeEditTicketViewModel>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeEditTicketList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetEmployeeEditTicketListByEmployeeId(int EmployeeId)
    {
        try
        {
            string sQuery = @"select ee.TicketID TicketId,ee.EmployeeId
                            ,(select EmployeeName from EmployeeMaster em where em.EmployeeId=ee.EmployeeId)EmployeeName
                            ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ee.EmployeeId)Department
                            from 
                            EmployeeValidation ev 
                            join EditEmployeeData ee
                            on ev.EmployeeValidationId=ee.EmployeeValidationId 
                            where employeeId=" + EmployeeId + " Group by ee.TicketID,ee.EmployeeId " +
                            " union select TicketId  ,ad.EmployeeId ,(select EmployeeName from EmployeeMaster em where em.employeeid=ad.EmployeeId)EmployeeName ," +
                            " (select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ad.EmployeeId)Department " +
                            " from [dbo].[AddDeleteTableAction] ad where IsActive=1 and ActionStatus=0  and EmployeeId in (select EmployeeId from EmployeeMaster" +
                            " where employeeId = " + EmployeeId + ") Group by ad.TicketID,ad.EmployeeId"; 
                            

            List<EmployeeEditTicketViewModel> dto = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeEditTicketViewModel>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeEditTicketList)}");
            throw;
        }
    }

    
   
    public async Task<IActionResult> ApproveEmployeeDetailsAddDelete(AddDeleteTableActionDTO inputDTO)
    {
        try
        {
            //EditEmployeeData editEmployeeData = await _unitOfWork.EditEmployeeData.GetFilter(x => x.TicketId == TicketId && x.EmployeeValidationId == EmployeeValidationId && x.IsActive == true);
            if (inputDTO != null)
            {

                AddDeleteTableActionDTO addDeleteTableActionDTO = new AddDeleteTableActionDTO();

                string sQry = $"SELECT [ActionId],[ActionBy],[ActionStatus],[ActionType],[TicketId],[ReferenceTable],[ReferenceId],[EmployeeId],[EntrySource]," +
                                $"[CreatedOn],[CreatedBy],[ModifiedOn],[ModifiedBy],[IsActive] FROM AddDeleteTableAction WHERE ActionId={inputDTO.ActionId}";
                addDeleteTableActionDTO = (await _unitOfWork.EmployeeValidation.GetTableData<AddDeleteTableActionDTO>(sQry)).FirstOrDefault();

                var tableIdCol = (await _unitOfWork.EmployeeValidation.GetTableData<EmployeeValidationMasterDTO>($"Select top 1 TableIdColumn from EmployeeValidationMaster where TableName='{addDeleteTableActionDTO.ReferenceTable}'")).FirstOrDefault().TableIdColumn;
                if(addDeleteTableActionDTO.ActionType.ToUpper()=="DELETE")
                    sQry = $"UPDATE {addDeleteTableActionDTO.ReferenceTable} SET IsActive=0 WHERE {tableIdCol} = {addDeleteTableActionDTO.ReferenceId}";
                else
                    sQry = $"UPDATE {addDeleteTableActionDTO.ReferenceTable} SET IsActive=1 WHERE {tableIdCol} = {addDeleteTableActionDTO.ReferenceId}";
                await _unitOfWork.EmployeeValidation.RunSQLCommand(sQry);

                sQry = $"UPDATE AddDeleteTableAction SET ActionStatus={inputDTO.ActionStatus},ModifiedBy=1,ModifiedOn='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE ActionId = {addDeleteTableActionDTO.ActionId}";
                await _unitOfWork.EmployeeValidation.RunSQLCommand(sQry);
 
                return Ok("Success");
                
            }
            return BadRequest("No Record Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ApproveEmployeeEditDetails)}");
            throw;
        }
    }

    public async Task<IActionResult> ApproveEmployeeEditDetails( string TicketId,int EmployeeValidationId,int IsApproved,int? approvedBy = 0, string sRootPath="")
    {
        try
        {
            EditEmployeeData editEmployeeData = await _unitOfWork.EditEmployeeData.GetFilter(x => x.TicketId == TicketId && x.EmployeeValidationId == EmployeeValidationId && x.IsActive == true);
            EmployeeMasterDTO employeeInfo = new EmployeeMasterDTO();
            employeeInfo.EmployeeId = editEmployeeData.EmployeeId.Value;
            employeeInfo = _mapper.Map<EmployeeMasterDTO>(await _unitOfWork.EmployeeMaster.GetByIdAsync(editEmployeeData.EmployeeId.Value));

            if (editEmployeeData != null)
            {
                editEmployeeData.IsApproved = IsApproved;
                await _unitOfWork.EditEmployeeData.UpdateAsync(editEmployeeData);
                _unitOfWork.Save();
                if (IsApproved == 2)
                {
                    var employeeValidationList = await _unitOfWork.EmployeeValidation.GetTableData<EmployeeValidationDTO>("Select * from EmployeeValidationMaster where EmployeeValidationId=" + EmployeeValidationId + "");
                    EmployeeValidationDTO employeeValidation = new EmployeeValidationDTO();
                    if (employeeValidationList != null && employeeValidationList.Count() > 0)
                    {
                        employeeValidation = employeeValidationList.FirstOrDefault();
                    }
                    //_mapper.Map<EmployeeValidationDTO>(await _unitOfWork.EmployeeValidation.FindByIdAsync(EmployeeValidationId));
                    if (employeeValidation != null)
                    {
                        string FieldName = employeeValidation.FieldName;
                        string qq = "",approveStatus=string.Empty;
                        int Id = 0;
                        editEmployeeData.ApprovedBy = approvedBy;
                        
                        int? tableReferenceId = editEmployeeData.TableReferenceId ?? default(int);
                        //if (employeeValidation.ScreenTab == "Personal Information" || employeeValidation.ScreenTab == "Job Information" || employeeValidation.ScreenTab == "Passport Details" || employeeValidation.ScreenTab == "Profile Picture")
                        //{
                            if(employeeValidation.FieldName.ToUpper()=="UPLOADIMAGE")
                            {
                                qq = @"update EmployeeMaster set ProfileImage=@newProfilePic,ProfileImageExtension=@docType,ModifiedBy=@modifiedBy,ModifiedOn=@modifiedOn where EmployeeId=@employeeId";
                               
                                await _unitOfWork.EmployeeMaster.ExecuteRawQuery(qq, new[] {new Microsoft.Data.SqlClient.SqlParameter("@newProfilePic", editEmployeeData.Attachment),
                                                                                    new Microsoft.Data.SqlClient.SqlParameter("@docType", editEmployeeData.DocumentType),
                                                                                     new Microsoft.Data.SqlClient.SqlParameter("@employeeId", editEmployeeData.EmployeeId),
                                new Microsoft.Data.SqlClient.SqlParameter("@modifiedBy", approvedBy),
                                new Microsoft.Data.SqlClient.SqlParameter("@modifiedOn", DateTime.Now)});
                            //Save Employee Photo in physical location
                            string sFilePath = $@"{sRootPath}\EmployeeProfile\{employeeInfo.UnitId}\{employeeInfo.EmployeeCode}.{editEmployeeData.DocumentType}";
                            System.IO.File.WriteAllBytes(sFilePath, editEmployeeData.Attachment);
                            //newProfilePic = editEmployeeData.Attachment;
                        }
                            else
                                approveStatus = await ApproveEditChanges(editEmployeeData, approvedBy);
                        //else
                        //{
                        //qq = @"update EmployeeMaster set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where EmployeeId=" + editEmployeeData.EmployeeId + "";
                        //await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);


                        //if(!(editEmployeeData.Attachment==null || editEmployeeData.Attachment.ToString()== "0x"))
                        //{
                        //    var empDoc = _unitOfWork.EmployeeUploadDocument.FindFirstByExpression(x => x.EmployeeId == editEmployeeData.EmployeeId && x.FieldName == editEmployeeData.ChangeType);
                        //    if (empDoc != null)
                        //    {
                        //        empDoc.IsActive = false;
                        //        //empDoc.EmployeeDocument = editEmployeeData.Attachment;
                        //        empDoc.ModifiedOn = DateTime.Now;
                        //        empDoc.ModifiedBy = editEmployeeData.ApprovedBy;
                        //        _unitOfWork.EmployeeUploadDocument.Update(empDoc);
                        //    }
                        //    // empDoc.CreatedBy ;
                        //    empDoc =new EmployeeUploadDocument();
                        //    empDoc.DocumentType = editEmployeeData.DocumentType;
                        //    empDoc.IsActive = true;
                        //    empDoc.EmployeeDocument = editEmployeeData.Attachment;
                        //    empDoc.EmployeeId = editEmployeeData.EmployeeId;
                        //    empDoc.FieldName = editEmployeeData.ChangeType;
                        //    empDoc.CreatedOn = DateTime.Now;
                        //    empDoc.CreatedBy = editEmployeeData.ApprovedBy;
                        //    _unitOfWork.EmployeeUploadDocument.AddAsync(empDoc);
                        //}
                        //}
                        //  }

                        //else if (employeeValidation.ScreenTab == "Current Address" || employeeValidation.ScreenTab == "Permanent Address")
                        //{
                        //    if (FieldName == "CountryId")
                        //    {
                        //        qq = @"update [EmployeeContactDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where EmployeeContactDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    if (FieldName == "StateId")
                        //    {
                        //        qq = @"update [EmployeeContactDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where EmployeeContactDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    if (FieldName == "CityId")
                        //    {
                        //        qq = @"update [EmployeeContactDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where EmployeeContactDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    else
                        //    {
                        //        qq = @"update [EmployeeContactDetail] set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where EmployeeContactDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //}
                        //else if (employeeValidation.ScreenTab == "Bank Details")
                        //{
                        //    if (FieldName == "BankId")
                        //    {
                        //        qq = @"update [EmployeeBankDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where BankDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    else
                        //    {
                        //        qq = @"update [EmployeeBankDetail] set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where BankDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //}
                        //else if (employeeValidation.ScreenTab == "Family Details")
                        //{
                        //    qq = @"update [EmployeeFamilyDetail] set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where EmployeeFamilyDetailId=" + tableReferenceId + "";
                        //    await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //}
                        //else if (employeeValidation.ScreenTab == "Academic Details")
                        //{
                        //    if (FieldName == "AcademicId")
                        //    {
                        //        qq = @"update [EmployeeAcademicDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where AcademicDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    else
                        //    {
                        //        qq = @"update [EmployeeAcademicDetail] set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where AcademicDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //}
                        //else if (employeeValidation.ScreenTab == "Experiences Backgroud")
                        //{

                        //    if (FieldName == "ExperienceJobTitleId")
                        //    {
                        //        qq = @"update [EmployeeExperienceDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where ExperienceDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    else
                        //    {
                        //        qq = @"update [EmployeeAcademicDetail] set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where ExperienceDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //}
                        //else if (employeeValidation.ScreenTab == "Certification Details")
                        //{
                        //    if (FieldName == "")
                        //    {
                        //        qq = @"update [EmployeeCertificationDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where CertificationDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    else
                        //    {
                        //        qq = @"update [EmployeeCertificationDetail] set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where CertificationDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //}
                        //else if (employeeValidation.ScreenTab == "Reference Details")
                        //{
                        //    if (FieldName == "")
                        //    {
                        //        qq = @"update [EmployeeReferenceDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where EmployeeReferenceId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    else
                        //    {
                        //        qq = @"update [EmployeeReferenceDetail] set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where EmployeeReferenceId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //}
                        //else if (employeeValidation.ScreenTab == "Language")
                        //{
                        //    if (FieldName == "")
                        //    {
                        //        qq = @"update [EmployeeLanguageDetail] set " + FieldName + "=" + editEmployeeData.ChangeValue + " where EmployeeLanguageDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //    else
                        //    {
                        //        qq = @"update [EmployeeLanguageDetail] set " + FieldName + "='" + editEmployeeData.ChangeValue + "' where EmployeeLanguageDetailId=" + tableReferenceId + "";
                        //        await _unitOfWork.EmployeeValidation.RunSQLCommand(qq);
                        //    }
                        //}
                        return Ok(approveStatus);
                    }
                }
                else if (IsApproved == 3 || IsApproved == 1)
                {
                    return Ok("Success");
                }

            }
            return BadRequest("No Record Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ApproveEmployeeEditDetails)}");
            throw;
        }
    }

    public async Task<string> ApproveEditChanges(EditEmployeeData editEmployeeData,int? approvedBy)
    {
        string tableName = "", tableIdCol = "";
        string oldValue = "";
        List<string> fieldValue = new List<string>();
        EmployeeValidationMasterDTO empValidInfo = new EmployeeValidationMasterDTO();
        string sSql = $"SELECT ScreenTab,TableName,TableIdColumn from EmployeeValidationMaster WHERE FieldName=@changeType";
        var objData = new { @changeType = editEmployeeData.ChangeType };
        string sRetMessage = "Fail to approve request";
        empValidInfo = (await _unitOfWork.EmployeeValidationMaster.ExecuteQuery<EmployeeValidationMasterDTO>(sSql, objData)).FirstOrDefault();
        if (empValidInfo != null)
        {
            dynamic objData1;
            dynamic empDoc;

            sSql = $"UPDATE " + empValidInfo.TableName + " SET " + editEmployeeData.ChangeType + " = @changeValue,ModifiedBy=@modifiedBy,ModifiedOn=@modifiedOn WHERE EmployeeId=@employeeId";
           
            if (!(editEmployeeData.TableReferenceId == 0 || editEmployeeData.TableReferenceId == null || tableIdCol.ToLower() == "employeeid"))
            {
                sSql = sSql + $" AND {empValidInfo.TableIdColumn}=@refId";
                objData1 = new { @changeValue = editEmployeeData.ChangeValue, @employeeId = editEmployeeData.EmployeeId, refId = editEmployeeData.TableReferenceId, @modifiedBy = approvedBy, @modifiedOn = DateTime.Now };
                //empDoc = _unitOfWork.EmployeeUploadDocument.FindFirstByExpression(x => x.EmployeeId == editEmployeeData.EmployeeId && x.ReferenceId == editEmployeeData.TableReferenceId && x.FieldName == editEmployeeData.ChangeType);
            }
            else
            {
                objData1 = new { @changeValue = editEmployeeData.ChangeValue, @employeeId = editEmployeeData.EmployeeId, @modifiedBy = approvedBy, @modifiedOn = DateTime.Now };
            }

            bool success = (await _unitOfWork.EmployeeValidationMaster.ExecuteQuery(sSql, objData1));
           
            if(success && editEmployeeData.Attachment.Length > 0)
            {
                empDoc = _unitOfWork.EmployeeUploadDocument.FindFirstByExpression(x => x.EmployeeId == editEmployeeData.EmployeeId && x.FieldName.ToLower() == editEmployeeData.ChangeType.ToLower());

                if (empDoc != null)
                {
                    empDoc.IsActive = false;
                    empDoc.ModifiedBy = approvedBy;
                    empDoc.ModifiedOn = DateTime.Now;
                    _unitOfWork.EmployeeUploadDocument.Update(empDoc);
                    _unitOfWork.Save();
                }
                if(editEmployeeData.Attachment.Length>0)
                {
                    empDoc = new EmployeeUploadDocument();
                    empDoc.EmployeeId = editEmployeeData.EmployeeId;
                    empDoc.ReferenceId = (editEmployeeData.TableReferenceId == null ? 0 : editEmployeeData.TableReferenceId);
                    empDoc.FieldName = editEmployeeData.ChangeType;
                    empDoc.ScreenTab = empValidInfo.ScreenTab;
                    empDoc.DocumentType = editEmployeeData.DocumentType;
                    empDoc.EmployeeDocument = editEmployeeData.Attachment;
                    empDoc.ScreenTab = empValidInfo.ScreenTab;
                    empDoc.CreatedBy = approvedBy;
                    empDoc.CreatedOn = DateTime.Now;
                    empDoc.IsActive = true;
                    _unitOfWork.EmployeeUploadDocument.AddAsync(empDoc);
                    _unitOfWork.Save();
                }
                sRetMessage = "Success";
            }


            sRetMessage = "Success";

        }
        return sRetMessage;
    }


    public async Task<IActionResult> GetEmployeeEditTicketByTicketId(string ticketId)
    {
        try
        {
           
            string sQuery = @"select ee.TicketID TicketId
                                ,ee.EmployeeId
                                ,(select EmployeeName from EmployeeMaster em where em.EmployeeId=ee.EmployeeId)EmployeeName
                                ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId=em.DepartmentId where em.EmployeeId=ee.EmployeeId)Department
                                from  EditEmployeeData ee
                                 where ee.TicketID='" + ticketId + "' Group by ee.TicketID,ee.EmployeeId"

                                 + " union"

                                + " select ev.TicketID TicketId"
                                + " , ev.EmployeeId"
                                + " ,(select EmployeeName from EmployeeMaster em where em.EmployeeId = ev.EmployeeId)EmployeeName"
                                + " ,(select dm.DepartmentName from EmployeeMaster em join DepartmentMaster dm on dm.DepartmentId = em.DepartmentId where em.EmployeeId = ev.EmployeeId)Department"
                                + " from[AddDeleteTableAction] ev where IsActive = 1 and TicketId = '" + ticketId + "' ";

            List<EmployeeEditTicketViewModel> dto = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeEditTicketViewModel>(sQuery);

            if (dto != null)
            {
                return Ok(dto.FirstOrDefault());
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, "No Ticket Record Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeEditTicketList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetEmployeeEditInfoByTicketId(string ticketId, int ClientId, int UnitId)
    {
        try
        {
            string sQuery = $"select " +
                $"EmployeeUpdateId EmployeeUpdateId" +
                                $",EmployeeId" +
                                $",ChangeValue" +
                                $",ed.EmployeeValidationId,OldValue,isnull(IsApproved,0)IsApproved,ApprovedBy,TicketId TicketId,EntrySource,Attachment,ScreenName,ScreenTab,TabSequence,fieldName,DisplayText " +
                                $",Attachment" +
                                $",DocumentType" +
                                $" from EditEmployeeData ed " +
                                $"left join EmployeeValidationMaster evm " +
                                $" on ed.EmployeeValidationId = evm.EmployeeValidationId" +
                                $" where ed.IsActive=1 and ed.TicketId=@ticketId and EntrySource='EmployeeEditScreen' and ScreenName='Employee Master'";

            List<EmployeeEditInfoViewModel> dto = await _unitOfWork.ProfileEditAuth.ExecuteQuery<EmployeeEditInfoViewModel>(sQuery, new { @ticketId=ticketId });
            var profileObj = dto.Where(x => x.FieldName.ToUpper() == "UPLOADIMAGE").Select(p => p).ToList();
            foreach(var profileItem in profileObj)
            {
                profileItem.OldProfile = _unitOfWork.EmployeeMaster.Find(x => x.EmployeeId == profileItem.EmployeeId).FirstOrDefault().ProfileImage;
                string Base64String = CommonHelper.GetBase64String(profileItem.DocumentType);
                profileItem.OldProfileBase64String = Base64String + Convert.ToBase64String(profileItem.OldProfile, 0, profileItem.OldProfile.Length);
            }
            dto.ForEach(x => { x.AttachmentBase64String = x.DocumentType != null ? CommonHelper.GetBase64String(x.DocumentType) + Convert.ToBase64String(x.Attachment, 0, x.Attachment.Length) : ""; });
            
            if (dto != null)
            {
                return Ok(dto);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, "No Ticket Record Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeEditTicketList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetEmployeeAddDeleteInfoByTicketId(string ticketId,int unitId)
    {
        try
        {
            EmployeeAddDeleteInfoViewModel dto = new EmployeeAddDeleteInfoViewModel();

            string sQuery = "";

            sQuery = @"select * from [AddDeleteTableAction] where TicketId='" + ticketId + "'";
            List<AddDeleteTableActionDTO> addDeleteTableActionDTOs = await _unitOfWork.ProfileEditAuth.GetTableData<AddDeleteTableActionDTO>(sQuery);
            List<EmployeeContactDetailDTO> employeeContactDetailList = new List<EmployeeContactDetailDTO>();
            List<EmployeeBankDetailDTO> employeeBankDetailList = new List<EmployeeBankDetailDTO>();
            List<EmployeeFamilyDetailDTO> employeeFamilyDetailList = new List<EmployeeFamilyDetailDTO>();
            List<EmployeeAcademicDTO> employeeAcademicList = new List<EmployeeAcademicDTO>();
            List<EmployeeExperienceDetailDTO> employeeExperienceDetailList = new List<EmployeeExperienceDetailDTO>();
            List<EmployeeCertificationDetailDTO> employeeCertificationDetailList = new List<EmployeeCertificationDetailDTO>();
            List<EmployeeReferenceDetailDTO> EmployeeReferenceDetailList = new List<EmployeeReferenceDetailDTO>();
            List<EmployeeLanguageDetailDTO> EmployeeLanguageDetailList = new List<EmployeeLanguageDetailDTO>();

            foreach (var item in addDeleteTableActionDTOs)
            {
                if (item.ReferenceTable == "EmployeeContactDetail")
                {
                    var employeeContactDetail = _mapper.Map<EmployeeContactDetailDTO>(_unitOfWork.EmployeeContactDetail.FindFirstByExpression(x => x.EmployeeContactDetailId == item.ReferenceId));
                    
                    if (employeeContactDetail != null)
                    {
                        employeeContactDetail.Country.CountryName = _unitOfWork.CountryMaster.FindFirstByExpression(x => x.CountryId == employeeContactDetail.CountryId).CountryName;
                        employeeContactDetailList.Add(employeeContactDetail);
                    }
                }
                if (item.ReferenceTable == "EmployeeBankDetail")
                {
                    var employeeBankDetail = _mapper.Map<EmployeeBankDetailDTO>(_unitOfWork.EmployeeBankDetail.FindFirstByExpression(x => x.BankDetailId == item.ReferenceId));
                    
                    if (employeeBankDetail != null)
                    {
                        employeeBankDetail.Bank.BankName = _unitOfWork.BankMaster.FindFirstByExpression(x => x.BankId == employeeBankDetail.BankId).BankName;
                        employeeBankDetailList.Add(employeeBankDetail);
                    }
                }
                if (item.ReferenceTable == "EmployeeFamilyDetail")
                {
                    var employeeFamilyDetails = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeFamilyDetailDTO>("select * from EmployeeFamilyDetail where EmployeeFamilyDetailId=" + item.ReferenceId);
                    if (employeeFamilyDetails != null && employeeFamilyDetails.Count() > 0)
                    {
                        employeeFamilyDetailList.Add(employeeFamilyDetails.FirstOrDefault());
                    }
                }
                else if ((item.ReferenceTable == null ? "" : item.ReferenceTable.ToUpper()) == "EMPLOYEEACADEMICDETAIL")
                {
                    var employeeAcademicDetails = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeAcademicDTO>("select AcademicDetailId ,InstituteName,EmployeeId ,AcademicId ,(Select AcademicName from AcademicMaster am where am.AcademicId = ead.AcademicId)AcademicName ,PassingYear ,Percentage  from [dbo].[EmployeeAcademicDetail] ead where AcademicDetailId=" + item.ReferenceId);
                    if (employeeAcademicDetails != null && employeeAcademicDetails.Count() > 0)
                    {
                        employeeAcademicList.Add(employeeAcademicDetails.FirstOrDefault());
                    }
                }
                else if ((item.ReferenceTable == null ? "" : item.ReferenceTable.ToUpper()) == "EMPLOYEEEXPERIENCEDETAIL")
                {
                    var employeeExperienceDetails = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeExperienceDetailDTO>("select ExperienceDetailId ,EmployeeId ,CompanyName ,ExperienceJobTitleId ,(Select JobTitle from JobTitleMaster jm where jm.JobTitleId=eed.ExperienceJobTitleId)JobTitle ,JoinDate ,LastWorkingDate from [dbo].[EmployeeExperienceDetail] eed where eed.ExperienceDetailId=" + item.ReferenceId);
                    if (employeeExperienceDetails != null && employeeExperienceDetails.Count() > 0)
                    {
                        employeeExperienceDetailList.Add(employeeExperienceDetails.FirstOrDefault());
                    }
                }
                else if ((item.ReferenceTable == null ? "" : item.ReferenceTable.ToUpper()) == "EMPLOYEECERTIFICATIONDETAIL")
                {
                    var employeeCertificationDetails = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeCertificationDetailDTO>("select * from [dbo].[EmployeeCertificationDetail] where CertificationDetailId=" + item.ReferenceId);
                    if (employeeCertificationDetails != null && employeeCertificationDetails.Count() > 0)
                    {
                        employeeCertificationDetailList.Add(employeeCertificationDetails.FirstOrDefault());
                    }
                }
                else if ((item.ReferenceTable == null ? "" : item.ReferenceTable.ToUpper()) == "EMPLOYEEREFERENCEDETAIL")
                {
                    var employeeReferenceDetail = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeReferenceDetailDTO>("select * from [dbo].[EmployeeReferenceDetail] where EmployeeReferenceId=" + item.ReferenceId);
                    if (employeeReferenceDetail != null && employeeReferenceDetail.Count() > 0)
                    {
                        EmployeeReferenceDetailList.Add(employeeReferenceDetail.FirstOrDefault());
                    }
                }
                else if ((item.ReferenceTable == null ? "" : item.ReferenceTable.ToUpper()) == "EMPLOYEELANGUAGEDETAIL")
                {

                    var employeeLanguageDetail = _mapper.Map<EmployeeLanguageDetailDTO>(_unitOfWork.EmployeeLanguageDetail.FindFirstByExpression(x => x.EmployeeLanguageDetailId == item.ReferenceId));
                    if (employeeLanguageDetail != null)
                    {
                        employeeLanguageDetail.LanguageMaster.Language = _unitOfWork.LanguageUnitMaster.FindFirstByExpression(x => x.LanguageParentId == employeeLanguageDetail.LanguageId && x.UnitId==unitId).Language;
                        EmployeeLanguageDetailList.Add(employeeLanguageDetail);
                        
                    }
                }
            }

            dto.employeeContactDetailList = employeeContactDetailList;
            dto.employeeBankDetailList = employeeBankDetailList;
            dto.addDeleteTableActionList = addDeleteTableActionDTOs;
            dto.employeeFamilyDetailList = employeeFamilyDetailList;
            dto.employeeAcademicDetailList = employeeAcademicList;
            dto.EmployeeExperienceDetailList = employeeExperienceDetailList;
            dto.EmployeeCertificationDetailList = employeeCertificationDetailList;
            dto.EmployeeReferenceDetailList = EmployeeReferenceDetailList;
            dto.EmployeeLanguageDetailList = EmployeeLanguageDetailList;

            if (dto != null)
            {
                return Ok(dto);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, "No Ticket Record Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeEditTicketList)}");
            throw;
        }
    }

    public async Task<IActionResult> GetEmployeeEditAddDeleteInfoByTicketId(string ticketId)
    {
        try
        {
            List<AddDeleteTableActionDTO> addDeleteTableActionDTO = await _unitOfWork.ProfileEditAuth.GetTableData<AddDeleteTableActionDTO>($"select * from [AddDeleteTableAction] where ticketid='{@ticketId=ticketId}'");

            if (addDeleteTableActionDTO != null)
            {
                if (addDeleteTableActionDTO.Count > 0)
                {
                    foreach (var item in addDeleteTableActionDTO)
                    {
                        if (item.ReferenceTable == "EmployeeAcademicDetail")
                        {

                        }
                    }
                }
            }


            string sQuery = @"select 
                                EmployeeUpdateId EmployeeUpdateId
                                ,EmployeeId
                                ,ChangeValue
                                ,ev.EmployeeValidationId,OldValue,isnull(IsApproved,0)IsApproved,ApprovedBy,TicketId TicketId,EntrySource,Attachment,ScreenName,ScreenTab,TabSequence,fieldName,DisplayText
                                ,Attachment
                                ,DocumentType
                                from EditEmployeeData ed
                                join EmployeeValidation ev
                                on ed.EmployeeValidationId=ev.EmployeeValidationId

                                where ed.IsActive=1 and ev.IsActive=1 and ed.TicketId=." + ticketId + "'  and EntrySource='EmployeeEditScreen' and ScreenName='Employee Master'";

            List<EmployeeEditInfoViewModel> dto = await _unitOfWork.ProfileEditAuth.GetTableData<EmployeeEditInfoViewModel>(sQuery);

            if (dto != null)
            {
                return Ok(dto);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, "No Ticket Record Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetEmployeeEditTicketList)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveProfileEditAuth(ProfileEditAuthViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.ProfileEditAuthList != null)
                {
                    if (inputDTO.ProfileEditAuthList.Count > 0)
                    {
                        foreach (var entry in inputDTO.ProfileEditAuthList)
                        {
                            List<Core.Entities.ProfileEditAuth> ecm = await _unitOfWork.ProfileEditAuth.GetQueryAll("select * from ProfileEditAuth where unitid=" + inputDTO.UnitId + " and isactive=1 and ProfileFieldId=" + entry.ProfileFieldId + "");
                            if (ecm.Count == 0)
                            {
                                entry.UnitId = inputDTO.UnitId;
                                entry.IsActive = true;
                                await _unitOfWork.ProfileEditAuth.AddAsync(_mapper.Map<Core.Entities.ProfileEditAuth>(entry));
                                _unitOfWork.Save();
                            }
                            else
                            {
                                Core.Entities.ProfileEditAuth profileEditAuth = ecm.FirstOrDefault();
                                profileEditAuth.IsEditable = entry.IsEditable;
                                profileEditAuth.AttachmentRequired = entry.AttachmentRequired;

                                await _unitOfWork.ProfileEditAuth.UpdateAsync(_mapper.Map<Core.Entities.ProfileEditAuth>(profileEditAuth));
                                _unitOfWork.Save();

                            }
                        }
                        return Ok("Success");
                    }
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "No Data Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveProfileEditAuth)}");
            throw;
        }
    }
}
