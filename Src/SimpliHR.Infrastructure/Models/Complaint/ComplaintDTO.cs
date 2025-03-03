using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Complaint;

public class ComplaintDTO
{
    public Int64 Id { get; set; }
    public string ComplaintNumber { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public int CategoryId { get; set; }
    public int AssignToId { get; set; }
    public int PriorityId { get; set; }

    public DateTime? DueDate { get; set; }
    public int StatusId { get; set; }
    public string? Remarks { get; set; } = null!;

    public int ComplainantId { get; set; }
    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsActive { get; set; }

    public int UnitId { get; set; }
}
