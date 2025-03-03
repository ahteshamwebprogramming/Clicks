using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTDeliverablesDTO
{
    public int DeliverableId { get; set; }

    public int SNo { get; set; }

    public string DeliverableName { get; set; } = null!;

    public int IsActive { get; set; }

    public int IsCompleted { get; set; }

    public int ProjectId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
