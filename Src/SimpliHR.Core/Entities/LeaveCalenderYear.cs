using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Core.Entities;



[Table("LeaveCalenderYear")]
public partial class LeaveCalenderYear
    {
    [Key]
    public int LeaveYearId { get; set; }

    public string? CalendarName { get; set; }

    [StringLength(50)]
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public int? UnitId { get; set; }
    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
}

