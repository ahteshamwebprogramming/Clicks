using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTProjectCategoryDTO
{
    public int CategoryID { get; set; }
    public string? encCategoryID { get; set; }

    public string CategoryName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int UnitId { get; set; }
    public bool? IsActive { get; set; }
}
