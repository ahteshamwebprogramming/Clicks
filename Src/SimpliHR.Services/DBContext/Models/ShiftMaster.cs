using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("ShiftMaster")]
public partial class ShiftMaster
{
    [Key]
    [Column("ShiftID")]
    public int ShiftId { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string? ShiftCode { get; set; }

    [StringLength(100)]
    public string? ShiftName { get; set; }

    public TimeSpan? InTime { get; set; }

    public TimeSpan? OutTime { get; set; }

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
