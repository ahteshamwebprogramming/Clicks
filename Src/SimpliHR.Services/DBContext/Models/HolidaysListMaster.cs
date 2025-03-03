using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("HolidaysListMaster")]
public partial class HolidaysListMaster
{
    [Key]
    [Column("HolidayID")]
    public int HolidayId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? HolidayDate { get; set; }

    public int? HolidayMonth { get; set; }

    public int? HolidayDay { get; set; }

    public int? HolidayYear { get; set; }

    [StringLength(100)]
    public string? HolidayName { get; set; }

    [StringLength(50)]
    public string? HolidayType { get; set; }

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
