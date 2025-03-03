using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("EmployeeCompOff")]
public partial class EmployeeCompOff
{
    [Key]
    public int CompOffId { get; set; }
    public string? CompOffType { get; set; }

    public DateTime? CompOffDate { get; set; }

    public string? Remarks { get; set; }

    public int? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
    public int? ApprovedBy { get; set; }

    public DateTime? CreatedOn { get; set; }
    public int? UnitId { get; set; }
    public int? EmployeeId { get; set; }
    public string? TicketId { get; set; }
    
    //public virtual ICollection<EmployeePersonalDetail> EmployeePersonalDetails { get; set; } = new List<EmployeePersonalDetail>();
}
