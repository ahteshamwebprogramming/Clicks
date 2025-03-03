using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTMilestonesDTO
{
    public int MilestoneId { get; set; }
    public string? encMilestoneId { get; set; }

    public int SNo { get; set; }

    public string MilestoneName { get; set; } = null!;
    public string Deliverables { get; set; } = null!;

    public int IsActive { get; set; }

    public int IsCompleted { get; set; }

    public int ProjectId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? ApprovalNeeded { get; set; }
    public bool? ApprovalSent { get; set; }
    public int? Approved { get; set; }

    public string? Action { get; set; }
}
