using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTStatusDTO
{
    public int StatusID { get; set; }

    public string StatusName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public string? StatusType { get; set; }
    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    
    public bool? IsActive { get; set; }
    public int UnitId { get; set; }
}
