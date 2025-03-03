using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("ShiftMaster")]
public partial class ShiftMaster
{
    [Key]
    public int ShiftId { get; set; }

    [StringLength(5)]
   // [Unicode(false)]
    public string? ShiftCode { get; set; }

    [StringLength(100)]
    public string? ShiftName { get; set; }

    public TimeSpan? InTime { get; set; }

    public TimeSpan? OutTime { get; set; }

    public int? UnitId { get; set; }
    //public int? BufferOfInTime { get; set; }
    //public int? BufferOfOutTime { get; set; }
    //public int? NoOfLateAllowed { get; set; }
    public int? IncludeBefore { get; set; }
    public int? IncludeAfter { get; set; }
    //public int? PolicyId { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsNightShift { get; set; }
    public bool? isBufferTimeAllowed { get; set; }
    public bool? IsFlexi { get; set; }
    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
}
