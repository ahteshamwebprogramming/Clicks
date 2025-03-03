using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("PTProject")]
public class PTProject
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProjectID { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int StatusID { get; set; }

    public int? PriorityId { get; set; }

    public int CategoryID { get; set; }
    public int UnitId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    [DefaultValue(true)]
    public bool? IsActive { get; set; }

    public int? IsApproved { get; set; }
    public bool ApprovalNeeded { get; set; }
    public bool StatusChangeRequest { get; set; }
    public int? RequestStatusId { get; set; }
}
