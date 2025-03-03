using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Complaint;

public class ComplaintCategoryDTO
{
    public int Id { get; set; }
    public string? encId { get; set; }

    public string? Name { get; set; } = null!;
    public string? Description { get; set; } = null!;


    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsActive { get; set; }
    public int UnitId { get; set; }
}
