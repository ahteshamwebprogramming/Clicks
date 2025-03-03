using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Table("AttendanceLateMaster")]
public partial class AttendanceLateSetting
{
    [Key]
    public int LateMasterId { get; set; }

    public int? NoOfLate { get; set; }
    public string? AppliedOn { get; set; }
    public int? UnitId { get; set; }
    public TimeSpan? LateDuration { get; set; }
    public string? ShowPostLimit { get; set; }

    public string? ActionPostLate { get; set; }

    public bool? CanDeductLeave { get; set; }

    public bool? Refill { get; set; }
    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }




}

