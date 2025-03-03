using SimpliHR.Infrastructure.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class ProjectViewModel
{
    public PTProjectDTO? Project { get; set; }
    public List<PTProjectDTO>? ProjectList { get; set; }
    public List<ProjectWithChild>? ProjectWithChildList { get; set; }
    public ProjectWithChild? ProjectWithChild { get; set; }
    public List<PTProjectCategoryDTO>? ProjectCategoryList { get; set; }
    public List<PTProjectPriorityDTO>? PriorityList { get; set; }
    public List<PTStatusDTO>? StatusList { get; set; }
    public List<PTProjectMemberDTO>? ProjectMembers { get; set; }
    public List<PTProjectMemberWithChild>? ProjectMembersWithChild { get; set; }

    public List<PTMilestonesDTO>? MilestonesList { get; set; }
    public PTMilestonesDTO? Milestone { get; set; }
    public List<PTDeliverablesDTO>? DeliverablesList { get; set; }
    public PTDeliverablesDTO? Deliverable { get; set; }
    public string? SerializedMilestones { get; set; }
    public string? SerializedDeliverables { get; set; }
    public List<PTEmployees>? EmployeeList { get; set; }
    public int? ApproverId { get; set; }
    public string? CollaboratorIds { get; set; }

    public List<PTCommentWithChild>? Comments { get; set; }

    public ProjectPageDetails? PageDetails { get; set; }

    public string? Source { get; set; }
    public string? encProjectId { get; set; }
}
public class ProjectPageDetails
{
    public string? ProjectStatusType { get; set; }
    public string? SearchKeyword { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
    public int? TotalPages { get; set; }
    public int? TotalRecords { get; set; }
    public string? Source { get; set; }
}