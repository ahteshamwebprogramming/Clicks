using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("EmployeeSettlementSummery")]
public partial class EmployeeSettlementSummery
{
    [Key]
    public int SettlementId { get; set; }
    public int? EmployeeId { get; set; }

    public int? OtherPayments { get; set; }

    public int? OtherDeductions { get; set; }
    public int? UnitId { get; set; }
    public string? Remarks { get; set; }
    public bool IsFixed { get; set; }
    public bool IsMailSent { get; set; }
    public DateTime ProcessDate { get; set; }
    public int? ProcessBy { get; set; }
    //public virtual ICollection<EmployeePersonalDetail> EmployeePersonalDetails { get; set; } = new List<EmployeePersonalDetail>();
}
