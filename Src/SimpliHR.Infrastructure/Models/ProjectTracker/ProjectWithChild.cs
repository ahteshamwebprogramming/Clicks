using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class ProjectWithChild
{
    public int ProjectID { get; set; }
    public string? encProjectID { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }
    public string? ProjectCategoryName { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int StatusID { get; set; }
    public string? Status { get; set; }

    public int? PriorityId { get; set; }
    public string? Priority { get; set; }

    public int CategoryID { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }
    public int UnitId { get; set; }
    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public int? IsApproved { get; set; }

    public bool ApprovalNeeded { get; set; }
    public bool? ProjectApprovalNeeded { get; set; }
    public bool? MilestoneApprovalNeeded { get; set; }
    public int? IsApprover { get; set; }
    public int? IsCollaborator { get; set; }
    public int? IsInitiator { get; set; }
    public string? Milestone { get; set; }
    public int? MilestoneId { get; set; }
    public string? Initiator { get; set; }
    public string? InitiatorProfileImagePath { get; set; }
    public string? InitiatorProfileImageExtension { get; set; }
    public int? InitiatorUnitId { get; set; }
    public byte[]? ProfileImage { get; set; }
    public string? ProjectStatusType { get; set; }
    public int? TotalMilestones { get; set; }
    public int? CompletedMilestones { get; set; }

    public bool StatusChangeRequest { get; set; }
    public int? RequestStatusId { get; set; }

}
