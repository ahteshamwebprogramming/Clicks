using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("LeaveTypeMaster")]
public partial class LeaveTypeMaster
{
    [Key]
    public int LeaveTypeId { get; set; }

    [StringLength(5)]
    public string? LeaveTypeCode { get; set; }

    [StringLength(50)]
    public string? LeaveType { get; set; }

    public int? MaxLimit { get; set; }
    public int? UnitId { get; set; }

    public double? MaxLeaveRange { get; set; }

    public double? MinLeaveRange { get; set; }

    [StringLength(5)]
    public string? ApplicableFor { get; set; }

    public bool? IsPaidLeave { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsCompOff { get; set; }
   

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
}
