using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Table("AttendanceSetting")]
public partial class AttendanceSetting
{
    [Key]
    public int AttendanceSettingId { get; set; }

    public int? ShiftId { get; set; }
    public string? ShiftCode { get; set; }
    public int? LegendType { get; set; }
    public int? UnitId { get; set; }
    public int? LocationId { get; set; }
    public TimeSpan? MinimumTime { get; set; }
    public TimeSpan? MaximumTime { get; set; }
    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [NavigationProperty]
    public virtual ShiftMaster Shift { get; set; } = null!;


}

