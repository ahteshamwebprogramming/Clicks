using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTProjectPriorityDTO
{
    
    public int PriorityId { get; set; }
    public string? encPriorityId { get; set; }

    public string? Priority { get; set; }

    public bool IsActive { get; set; }

    public int UnitId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
