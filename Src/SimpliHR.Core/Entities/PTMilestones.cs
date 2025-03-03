using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PTMilestones")]
public class PTMilestones
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MilestoneId { get; set; }

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
}

