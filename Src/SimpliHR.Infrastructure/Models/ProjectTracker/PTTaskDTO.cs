using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTTaskDTO
{
    public int TaskID { get; set; }

    public string TaskDescription { get; set; } = null!;

    public int AssignedTo { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public int StatusID { get; set; }

    public string? Priority { get; set; }

    public int ProjectID { get; set; }

    public string? Comments { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
}
