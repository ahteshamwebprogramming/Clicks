using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Core.Entities;



[Table("LeaveCompOff")]
public partial class LeaveCompOff
{
    [Key]
    public int CompOffId { get; set; }

    public string? LeavePolicy { get; set; }

    public int? CalendarYear { get; set; }

    public TimeSpan? MinHalfDay { get; set; }

    public TimeSpan? MinFullDay { get; set; }

    public int? ApplicableDay { get; set; }

    public int? AvailDay { get; set; }

    public string? ApplicableFor { get; set; }
    public int? UnitId { get; set; }
    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
}

