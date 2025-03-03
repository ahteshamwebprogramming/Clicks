using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crmf;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.ProjectTracker.Masters;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.ProjectTracker;

namespace ProjectTracker.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectAPIController> _logger;
    private readonly IMapper _mapper;

    public ProjectAPIController(IUnitOfWork unitOfWork, ILogger<ProjectAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> ProjectList(int UnitId, int LoggedInId)
    {
        try
        {
            string query = $"Select \r\nP.*\r\n,(Select ppp.Priority from PTProjectPriority ppp where ppp.PriorityId=p.PriorityId)Priority,(Select top 1 pm.MilestoneName from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=0 order by pm.SNo asc)Milestone,(Select top 1 pm.MilestoneId from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=0 order by pm.SNo asc)MilestoneId,(Select pc.CategoryName from PTProjectCategory pc where pc.CategoryID=p.CategoryID)ProjectCategoryName,(Select ps.StatusName from PTStatus ps where ps.StatusID=p.StatusID)Status,((Case when \r\n((Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Approver')) > 0 then 1\r\nelse 0 end\r\n))ProjectApprovalNeeded,((Case when (Select Count(1) from PTMilestones where IsActive=1 and ApprovalSent=1 and ProjectId=P.ProjectID ) > 0 then 1\r\nelse 0 end\r\n))MilestoneApprovalNeeded,(Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Approver' and PM.UserID={LoggedInId})IsApprover\r\n,(Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Collaborator' and PM.UserID={LoggedInId})IsCollaborator, Case when (P.CreatedBy={LoggedInId}) then 1 else 0 end IsInitiator\r\nfrom PTProject P where p.IsActive=1 and p.UnitId={UnitId} and (P.CreatedBy={LoggedInId} or {LoggedInId} in (Select UserID from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1))";
            List<ProjectWithChild> dto = await _unitOfWork.PTProject.GetTableData<ProjectWithChild>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProjectList)}");
            throw;
        }
    }
    public async Task<IActionResult> ProjectListProjectStatusWise(int UnitId, int LoggedInId, string ProjectStatusType)
    {
        try
        {
            string where = $"";
            string query = $"Select \r\nP.*\r\n,(Select ppp.Priority from PTProjectPriority ppp where ppp.PriorityId=p.PriorityId)Priority,(Select top 1 pm.MilestoneName from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=0 order by pm.SNo asc)Milestone,(Select top 1 pm.MilestoneId from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=0 order by pm.SNo asc)MilestoneId,(Select count(1) from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 )TotalMilestones\r\n,(Select count(1) from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=1)CompletedMilestones,(Select pc.CategoryName from PTProjectCategory pc where pc.CategoryID=p.CategoryID)ProjectCategoryName,(Select ps.StatusName from PTStatus ps where ps.StatusID=p.StatusID)Status,P.ApprovalNeeded ProjectApprovalNeeded,((Case when (Select Count(1) from PTMilestones where IsActive=1 and ApprovalSent=1 and ProjectId=P.ProjectID ) > 0 then 1\r\nelse 0 end\r\n))MilestoneApprovalNeeded,(Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Approver' and PM.UserID={LoggedInId})IsApprover\r\n,(Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Collaborator' and PM.UserID={LoggedInId})IsCollaborator, Case when (P.CreatedBy={LoggedInId}) then 1 else 0 end IsInitiator\r\nfrom PTProject P where p.IsActive=1 and p.UnitId={UnitId} and (P.CreatedBy={LoggedInId} or {LoggedInId} in (Select UserID from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1))";

            if (ProjectStatusType == "ApprovalPending")
            {
                where += $" and \r\n((p.ApprovalNeeded=1 and IsApproved=0)\r\nor\r\n(Select count(1) from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.ApprovalSent=1)>0) or\r\n(p.StatusChangeRequest=1)";
            }
            else if (ProjectStatusType == "All")
            {
                where += $"";
            }
            else if (ProjectStatusType == "Incomplete")
            {
                where += $"and P.StatusID not in (6,7)";
            }
            else if (ProjectStatusType == "Completed")
            {
                where += $"and P.StatusID in (6)";
            }

            string finalQuery = query + where;

            List<ProjectWithChild> dto = await _unitOfWork.PTProject.GetTableData<ProjectWithChild>(query + where);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProjectList)}");
            throw;
        }
    }

    public async Task<IActionResult> ProjectListProjectStatusWiseAndPageWise(int UnitId, int LoggedInId, ProjectPageDetails inputDTO, string Action)
    {
        try
        {
            string pageDetails = "";
            string where = $"";
            string statusFilter = $"";
            string query = "";
            string selectData = "";
            string orderBy = "";
            if (inputDTO != null)
            {
                if (String.IsNullOrEmpty(inputDTO.SearchKeyword))
                {
                    inputDTO.SearchKeyword = "";
                }
                string searchFilter = "";

                searchFilter += $" and (ProjectName like '%{inputDTO.SearchKeyword}%')";
                if (Action == "Data")
                {
                    selectData = $" \r\nP.*\r\n,(Select ppp.Priority from PTProjectPriority ppp where ppp.PriorityId=p.PriorityId)Priority,(Select top 1 pm.MilestoneName from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=0 order by pm.SNo asc)Milestone,(Select top 1 pm.MilestoneId from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=0 order by pm.SNo asc)MilestoneId,(Select count(1) from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 )TotalMilestones\r\n,(Select count(1) from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=1)CompletedMilestones,(Select pc.CategoryName from PTProjectCategory pc where pc.CategoryID=p.CategoryID)ProjectCategoryName,(Select ps.StatusName from PTStatus ps where ps.StatusID=p.StatusID)Status,P.ApprovalNeeded ProjectApprovalNeeded,((Case when (Select Count(1) from PTMilestones where IsActive=1 and ApprovalSent=1 and ProjectId=P.ProjectID ) > 0 then 1\r\nelse 0 end\r\n))MilestoneApprovalNeeded,(Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Approver' and PM.UserID={LoggedInId})IsApprover\r\n,(Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Collaborator' and PM.UserID={LoggedInId})IsCollaborator, Case when (P.CreatedBy={LoggedInId}) then 1 else 0 end IsInitiator\r\n ";
                    orderBy += $" order by P.ProjectID desc ";
                    if (inputDTO != null && inputDTO.PageNumber != null && inputDTO.PageSize != null)
                    {
                        pageDetails = $"OFFSET {(inputDTO.PageNumber - 1) * inputDTO.PageSize} ROWS FETCH NEXT {inputDTO.PageSize} ROWS ONLY";
                    }
                }
                else
                {
                    selectData = $"count(1)";
                }

                query = $"Select {selectData} from PTProject P where p.IsActive=1 and p.UnitId={UnitId} and (P.CreatedBy={LoggedInId} or {LoggedInId} in (Select UserID from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1))";

                if (inputDTO.ProjectStatusType == "ApprovalPending")
                {
                    statusFilter += $" and \r\n((p.ApprovalNeeded=1 and IsApproved=0)\r\nor\r\n(Select count(1) from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.ApprovalSent=1)>0 or\r\n p.StatusChangeRequest=1)";
                }
                else if (inputDTO.ProjectStatusType == "All")
                {
                    statusFilter += $"";
                }
                else if (inputDTO.ProjectStatusType == "Incomplete")
                {
                    statusFilter += $"and P.StatusID not in (6,7)";
                }
                else if (inputDTO.ProjectStatusType == "Completed")
                {
                    statusFilter += $"and P.StatusID in (6)";
                }

                string finalQuery = query + statusFilter + searchFilter + orderBy + pageDetails;



                if (Action == "Data")
                {
                    List<ProjectWithChild> dto = await _unitOfWork.PTProject.GetTableData<ProjectWithChild>(finalQuery);
                    return Ok(dto);
                }
                else
                {
                    List<int> dto = await _unitOfWork.PTProject.GetTableData<int>(finalQuery);
                    return Ok(dto);
                }


            }
            else
            {
                return BadRequest("");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProjectList)}");
            throw;
        }
    }
    public async Task<IActionResult> ProjectWithChildById(int ProjectId, int LoggedInId)
    {
        try
        {
            string query = $"Select \r\nP.*\r\n,(Select ppp.Priority from PTProjectPriority ppp where ppp.PriorityId=p.PriorityId)Priority,(Select top 1 pm.MilestoneName from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=0 order by pm.SNo asc)Milestone,(Select top 1 pm.MilestoneId from PTMilestones pm where pm.ProjectId=p.ProjectID and pm.IsActive=1 and pm.IsCompleted=0 order by pm.SNo asc)MilestoneId,(Select pc.CategoryName from PTProjectCategory pc where pc.CategoryID=p.CategoryID)ProjectCategoryName,(Select ps.StatusName from PTStatus ps where ps.StatusID=p.StatusID)Status,((Case when \r\n((Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Approver')) > 0 then 1\r\nelse 0 end\r\n))ProjectApprovalNeeded\r\n,((Case when (Select Count(1) from PTMilestones where IsActive=1 and ApprovalSent=1 and ProjectId=P.ProjectID ) > 0 then 1\r\nelse 0 end\r\n))MilestoneApprovalNeeded,(Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Approver' and PM.UserID={LoggedInId})IsApprover\r\n,(Select Count(1) from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1 and PM.RoleType='Collaborator' and PM.UserID={LoggedInId})IsCollaborator, Case when (P.CreatedBy={LoggedInId}) then 1 else 0 end IsInitiator\r\n,(Select em.EmployeeName from EmployeeMaster em where  em.EmployeeId = P.CreatedBy)Initiator,(Select em.ProfileImageExtension from EmployeeMaster em where  em.EmployeeId = P.CreatedBy)InitiatorProfileImageExtension\r\n,(Select em.UnitId from EmployeeMaster em where  em.EmployeeId = P.CreatedBy)InitiatorUnitId,(Select em.ProfileImage from EmployeeMaster em where  em.EmployeeId = P.CreatedBy)ProfileImage from PTProject P where P.ProjectID={ProjectId} and (P.CreatedBy={LoggedInId} or {LoggedInId} in (Select UserID from PTProjectMember PM where PM.ProjectID=P.ProjectID and PM.IsActive=1))";
            List<ProjectWithChild> dto = await _unitOfWork.PTProject.GetTableData<ProjectWithChild>(query);
            if (dto != null && dto.Count > 0)
            {
                return Ok(dto.FirstOrDefault());
            }
            return BadRequest("No Record Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProjectList)}");
            throw;
        }
    }

    public async Task<IActionResult> ProjectById(int ProjectId)
    {
        try
        {
            PTProjectDTO dto = _mapper.Map<PTProjectDTO>(await _unitOfWork.PTProject.FindByIdAsync(ProjectId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProjectById)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveProject(PTProjectDTO inputDTO)
    {
        try
        {
            //int insertedId = await _unitOfWork.PerformanceSetting.AddAsync(_mapper.Map<PerformanceSetting>(inputDTO.PerformanceSetting));
            if (inputDTO != null)
            {
                if (inputDTO.ProjectID > 0)
                {
                    PTProject ea = await _unitOfWork.PTProject.FindByIdAsync(inputDTO.ProjectID);
                    inputDTO.CreatedDate = ea.CreatedDate;
                    inputDTO.CreatedBy = ea.CreatedBy;
                    inputDTO.UnitId = ea.UnitId;
                    inputDTO.IsActive = ea.IsActive;
                    inputDTO.IsApproved = ea.IsApproved;
                    await _unitOfWork.PTProject.UpdateAsync(_mapper.Map<PTProject>(inputDTO));
                    _unitOfWork.Save();
                    PTProjectDTO ead = _mapper.Map<PTProjectDTO>(ea);
                    return Ok(ead);
                }
                else
                {
                    int insertedId = await _unitOfWork.PTProject.AddAsync(_mapper.Map<PTProject>(inputDTO));
                    inputDTO.ProjectID = insertedId;
                    if (insertedId == 0)
                    {
                        return BadRequest("Error while saving the data");
                    }
                    _unitOfWork.Save();
                    return Ok(inputDTO);
                }

            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveProject)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveMilestones(ProjectViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                bool approvalNeeded = await _unitOfWork.PTProjectMember.Exists(x => x.ProjectID == inputDTO.Project.ProjectID && x.RoleType == "Approver" && x.IsActive == true);

                List<PTMilestonesDTO>? milestonesList = inputDTO.MilestonesList;
                if (milestonesList != null && milestonesList.Any())
                {
                    foreach (var item in milestonesList)
                    {
                        item.ApprovalNeeded = approvalNeeded;
                        if (item != null && inputDTO.Project != null)
                        {
                            if (item.MilestoneId > 0)
                            {
                                await _unitOfWork.PTMilestones.UpdateAsync(_mapper.Map<PTMilestones>(item));
                            }
                            else
                            {
                                item.ProjectId = inputDTO.Project.ProjectID;
                                item.MilestoneId = await _unitOfWork.PTMilestones.AddAsync(_mapper.Map<PTMilestones>(item));
                            }
                        }
                    }
                }

            }
            return Ok("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveMilestones)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteMilestones(int MilestoneId)
    {
        try
        {
            PTMilestones res = await _unitOfWork.PTMilestones.FindByIdAsync(MilestoneId);
            if (res != null)
            {
                res.IsActive = 0;
                await _unitOfWork.PTMilestones.UpdateAsync(res);
                _unitOfWork.Save();
                return Ok("Save");
            }
            else
            {
                return BadRequest("File not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteMilestones)}");
            throw;
        }
    }
    public async Task<IActionResult> MarkMilestoneCompleted(int MilestoneId)
    {
        try
        {
            PTMilestones res = await _unitOfWork.PTMilestones.FindByIdAsync(MilestoneId);
            if (res != null)
            {
                if (res.ApprovalNeeded == true)
                {
                    res.ApprovalSent = true;
                    res.Approved = 0;
                    await _unitOfWork.PTMilestones.UpdateAsync(res);
                    _unitOfWork.Save();
                    return Ok("Approval Sent");
                }
                else
                {
                    res.IsCompleted = 1;
                    await _unitOfWork.PTMilestones.UpdateAsync(res);
                    _unitOfWork.Save();
                    return Ok("Completed");
                }
            }
            else
            {
                return BadRequest("File not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteMilestones)}");
            throw;
        }
    }


    public async Task<IActionResult> ActionOnMilestone(int MilestoneId, string action)
    {
        try
        {
            PTMilestones res = await _unitOfWork.PTMilestones.FindByIdAsync(MilestoneId);
            if (res != null)
            {
                if (action == "MilestoneCompleted")
                {
                    if (res.ApprovalNeeded == true)
                    {
                        res.ApprovalSent = true;
                        res.Approved = 0;
                        await _unitOfWork.PTMilestones.UpdateAsync(res);
                        _unitOfWork.Save();
                        return Ok("Approval Sent");
                    }
                    else
                    {
                        res.IsCompleted = 1;
                        await _unitOfWork.PTMilestones.UpdateAsync(res);
                        _unitOfWork.Save();
                        return Ok("Completed");
                    }
                }
                else if (action == "Approve")
                {
                    res.Approved = 1;
                    res.ApprovalSent = false;
                    res.IsCompleted = 1;
                    await _unitOfWork.PTMilestones.UpdateAsync(res);
                    _unitOfWork.Save();
                    return Ok("Approve Successfully");
                }
                else if (action == "Revise")
                {
                    res.Approved = 0;
                    res.ApprovalSent = false;
                    res.IsCompleted = 0;
                    await _unitOfWork.PTMilestones.UpdateAsync(res);
                    _unitOfWork.Save();
                    return Ok("Need Revision");
                }
            }
            else
            {
                return BadRequest("File not found");
            }
            return BadRequest("No Action found on this transaction");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteMilestones)}");
            throw;
        }
    }


    public async Task<IActionResult> ActionOnProject(PTProjectDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception();
            }

            PTProject res = await _unitOfWork.PTProject.FindByIdAsync(inputDTO.ProjectID);

            if (inputDTO.Description == "Approve")
            {
                res.IsApproved = 1;
            }
            else if (inputDTO.Description == "Reject")
            {
                res.IsApproved = -1;
            }
            await _unitOfWork.PTProject.UpdateAsync(res);
            return Ok("Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteMilestones)}");
            throw;
        }
    }
    public async Task<IActionResult> ActionOnStatus(PTProjectDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception();
            }

            PTProject res = await _unitOfWork.PTProject.FindByIdAsync(inputDTO.ProjectID);

            if (inputDTO.Description == "Approve")
            {
                res.StatusChangeRequest = false;
            }
            else if (inputDTO.Description == "Revise")
            {
                res.StatusChangeRequest = false;
                res.StatusID = res.RequestStatusId ?? default(int);
            }
            await _unitOfWork.PTProject.UpdateAsync(res);
            return Ok("Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteMilestones)}");
            throw;
        }
    }
    public async Task<IActionResult> ProjectStatusChange(PTProjectDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception();
            }

            PTProject res = await _unitOfWork.PTProject.FindByIdAsync(inputDTO.ProjectID);

            if (res.ApprovalNeeded == true)
            {
                res.StatusChangeRequest = true;
                res.RequestStatusId = res.StatusID;
                res.StatusID = inputDTO.StatusID;
                await _unitOfWork.PTProject.UpdateAsync(res);
                return Ok("Requested");
            }
            else
            {
                res.StatusID = inputDTO.StatusID;
                await _unitOfWork.PTProject.UpdateAsync(res);
                return Ok("Success");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteMilestones)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveDeliverables(ProjectViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                List<PTDeliverablesDTO>? deliverablesList = inputDTO.DeliverablesList;
                if (deliverablesList != null && deliverablesList.Any())
                {
                    foreach (var item in deliverablesList)
                    {
                        if (item != null && inputDTO.Project != null)
                        {
                            if (item.DeliverableId > 0)
                            {
                                await _unitOfWork.PTDeliverables.UpdateAsync(_mapper.Map<PTDeliverables>(item));
                            }
                            else
                            {
                                item.ProjectId = inputDTO.Project.ProjectID;
                                item.DeliverableId = await _unitOfWork.PTDeliverables.AddAsync(_mapper.Map<PTDeliverables>(item));
                            }
                        }
                    }
                }

            }
            return Ok("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveMilestones)}");
            throw;
        }
    }
    public async Task<IActionResult> GetMilestonesByProjectId(int ProjectId)
    {
        try
        {
            string query = $"Select * from PTMilestones where isActive=1 and ProjectId={ProjectId}";
            List<PTMilestonesDTO> dto = _mapper.Map<List<PTMilestonesDTO>>(await _unitOfWork.PTMilestones.GetTableData<PTMilestonesDTO>(query));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMilestonesByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetTeamMembersByProjectId(int ProjectId)
    {
        try
        {
            string query = $"Select ppm.*\r\n,em.EmployeeName\r\n,em.UnitId\r\n,em.ProfileImageExtension\r\n,em.ProfileImage\r\nfrom PTProjectMember ppm\r\njoin EmployeeMaster em on ppm.UserID=em.EmployeeId\r\nwhere ppm.isActive=1 and ppm.ProjectId={ProjectId}";
            List<PTProjectMemberWithChild> dto = _mapper.Map<List<PTProjectMemberWithChild>>(await _unitOfWork.PTProjectMember.GetTableData<PTProjectMemberWithChild>(query));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetTeamMembersByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetTeamMembersByProjectIdArray(int[] ProjectIds)
    {
        try
        {
            string strProjectIds = String.Join(",", ProjectIds);
            string query = $"Select ppm.*\r\n,em.EmployeeName\r\n,em.UnitId\r\n,em.ProfileImageExtension\r\n,em.ProfileImage\r\nfrom PTProjectMember ppm\r\njoin EmployeeMaster em on ppm.UserID=em.EmployeeId\r\nwhere ppm.isActive=1 and ppm.ProjectId in ({strProjectIds})";
            List<PTProjectMemberWithChild> dto = _mapper.Map<List<PTProjectMemberWithChild>>(await _unitOfWork.PTProjectMember.GetTableData<PTProjectMemberWithChild>(query));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetTeamMembersByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetCommentsByProjectId(int ProjectId, int LoggedInUserId)
    {
        try
        {
            string query = $"Select \r\nem.ProfileImage,(Case when UserID=@LoggedInUserId then 'chat-message-right' else '' end)Orien\r\n,ptc.*,(CASE  WHEN EXISTS ( SELECT 1  FROM PTAttachment pta  WHERE pta.CommentID = ptc.CommentID  AND pta.IsActive = 1 ) THEN 1  ELSE 0  END) AS AttachmentFound \r\nfrom PTComment ptc join EmployeeMaster em on ptc.UserID=em.EmployeeId\r\nwhere ptc.IsActive=1 and ptc.IsVisible=1 and ptc.TaskID=@ProjectId";
            var parameters = new { LoggedInUserId, ProjectId };
            //List<PTCommentWithChild> dto = _mapper.Map<List<PTCommentWithChild>>(await _unitOfWork.PTProjectMember.GetTableData<PTCommentWithChild>(query));
            List<PTCommentWithChild> dto = _mapper.Map<List<PTCommentWithChild>>(await _unitOfWork.PTProjectMember.GetTableData<PTCommentWithChild>(query, parameters));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetTeamMembersByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAttachmentsByCommentId(int CommentID)
    {
        try
        {
            string query = $"Select * from PTAttachment where CommentID=@CommentID";
            var parameters = new { CommentID };
            //List<PTCommentWithChild> dto = _mapper.Map<List<PTCommentWithChild>>(await _unitOfWork.PTProjectMember.GetTableData<PTCommentWithChild>(query));
            List<PTAttachmentDTO> dto = _mapper.Map<List<PTAttachmentDTO>>(await _unitOfWork.PTAttachment.GetTableData<PTAttachmentDTO>(query, parameters));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetTeamMembersByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetDeliverablesByProjectId(int ProjectId)
    {
        try
        {
            string query = $"Select * from PTDeliverables where isActive=1 and ProjectId={ProjectId}";
            List<PTDeliverablesDTO> dto = _mapper.Map<List<PTDeliverablesDTO>>(await _unitOfWork.PTDeliverables.GetTableData<PTDeliverablesDTO>(query));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMilestonesByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetProjectMembersByProjectId(int ProjectId)
    {
        try
        {
            string query = $"Select * from PTProjectMember where IsActive=1 and ProjectID={ProjectId}";
            List<PTProjectMemberDTO> dto = _mapper.Map<List<PTProjectMemberDTO>>(await _unitOfWork.PTProjectMember.GetTableData<PTProjectMemberDTO>(query));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMilestonesByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetEmployeeListByUnitId(int UnitId)
    {
        try
        {
            string query = $"Select em.EmployeeId,em.EmployeeName,em.EmployeeCode\r\n,(Select dm.DepartmentName from DepartmentMaster dm where dm.DepartmentId=em.DepartmentId)Department \r\nfrom EmployeeMaster em where em.UnitId={UnitId} and em.isActive=1 and em.InfoFillingStatus=1 and em.EmployeeStatus='Active' and DOJ <= GETDATE() and EmployeeId not in (Select EmployeeId from LoginDetails where LoginType=1)";
            List<PTEmployees> dto = _mapper.Map<List<PTEmployees>>(await _unitOfWork.PTDeliverables.GetTableData<PTEmployees>(query));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMilestonesByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveProjectMembers(ProjectViewModel projectViewModel, int LoggedInId)
    {
        try
        {
            string query = $"";
            if (projectViewModel != null && projectViewModel.Project != null)
            {
                if (projectViewModel.Project.ProjectID > 0)
                {
                    if (projectViewModel.ApproverId != null && projectViewModel.ApproverId > 0)
                    {
                        query = $"Select * from PTProjectMember where ProjectID={projectViewModel.Project.ProjectID} and RoleType='Approver' and IsActive=1";
                        var projectMemberApprover = await _unitOfWork.PTProjectMember.GetTableData<PTProjectMember>(query);
                        if (projectMemberApprover != null && projectMemberApprover.Count > 0)
                        {
                            PTProjectMember? projectMemberDTO = new PTProjectMember();
                            projectMemberDTO = projectMemberApprover.FirstOrDefault();
                            if (projectMemberDTO != null)
                            {
                                projectMemberDTO.ModifiedDate = DateTime.Now;
                                projectMemberDTO.ModifiedBy = LoggedInId;
                                projectMemberDTO.UserID = projectViewModel.ApproverId ?? default(int);
                                await _unitOfWork.PTProjectMember.UpdateAsync(projectMemberDTO);
                            }
                        }
                        else
                        {
                            PTProjectMember? projectMemberDTO = new PTProjectMember();
                            projectMemberDTO.CreatedDate = DateTime.Now;
                            projectMemberDTO.CreatedBy = LoggedInId;
                            projectMemberDTO.ProjectID = projectViewModel.Project.ProjectID;
                            projectMemberDTO.UserID = projectViewModel.ApproverId ?? default(int);
                            projectMemberDTO.RoleType = "Approver";
                            projectMemberDTO.IsActive = true;
                            await _unitOfWork.PTProjectMember.AddAsync(projectMemberDTO);
                            _unitOfWork.Save();
                        }
                    }
                    if (projectViewModel.CollaboratorIds != null && projectViewModel.CollaboratorIds != "")
                    {
                        query = $"Update PTProjectMember set IsActive=0 where ProjectID={projectViewModel.Project.ProjectID} and RoleType='Collaborator'";
                        await _unitOfWork.PTProjectMember.RunSQLCommand(query);
                        string[] collaboratorStrings = projectViewModel.CollaboratorIds.Split(',');
                        int[] collaboratorInts = Array.ConvertAll(collaboratorStrings, int.Parse);
                        foreach (var item in collaboratorInts)
                        {
                            PTProjectMember? projectMemberDTO = new PTProjectMember();
                            projectMemberDTO.CreatedDate = DateTime.Now;
                            projectMemberDTO.CreatedBy = LoggedInId;
                            projectMemberDTO.ProjectID = projectViewModel.Project.ProjectID;
                            projectMemberDTO.UserID = item;
                            projectMemberDTO.RoleType = "Collaborator";
                            projectMemberDTO.IsActive = true;
                            await _unitOfWork.PTProjectMember.AddAsync(projectMemberDTO);
                            _unitOfWork.Save();
                        }
                    }
                }

            }
            return Ok("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMilestonesByProjectId)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveComments(PTCommentDTO inputDTO)
    {
        try
        {
            //int insertedId = await _unitOfWork.PerformanceSetting.AddAsync(_mapper.Map<PerformanceSetting>(inputDTO.PerformanceSetting));
            if (inputDTO != null)
            {
                int insertedId = await _unitOfWork.PTComment.AddAsync(_mapper.Map<PTComment>(inputDTO));
                inputDTO.CommentID = insertedId;
                _unitOfWork.Save();
                return Ok(inputDTO);
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveComments)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveCommentAttachments(PTAttachmentDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                int insertedId = await _unitOfWork.PTAttachment.AddAsync(_mapper.Map<PTAttachment>(inputDTO));
                inputDTO.AttachmentID = insertedId;
                _unitOfWork.Save();
                return Ok(inputDTO);
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveCommentAttachments)}");
            throw;
        }
    }
    public async Task<IActionResult> HardDeleteAttachments(int CommentId, List<int> FileUploadIds)
    {
        try
        {
            await _unitOfWork.PTAttachment.DeleteAsync(CommentId);
            foreach (int FileUploadId in FileUploadIds)
            {
                await _unitOfWork.PTAttachment.DeleteAsync(FileUploadId);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(HardDeleteAttachments)}");
            throw;
        }
    }
}
