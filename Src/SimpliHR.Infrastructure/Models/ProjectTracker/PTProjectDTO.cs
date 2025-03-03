using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTProjectDTO
{
    public int ProjectID { get; set; }
    public string? encProjectID { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int StatusID { get; set; }

    public int? PriorityId { get; set; }

    public int CategoryID { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }
    public int UnitId { get; set; }
    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public int? IsApproved { get; set; }

    public bool ApprovalNeeded { get; set; }
    public bool StatusChangeRequest { get; set; }
    public int? RequestStatusId { get; set; }
}
