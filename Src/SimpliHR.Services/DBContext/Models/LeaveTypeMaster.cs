using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("LeaveTypeMaster")]
public partial class LeaveTypeMaster
{
    [Key]
    [Column("LeaveTypeID")]
    public int LeaveTypeId { get; set; }

    [StringLength(5)]
    public string? LeaveTypeCode { get; set; }

    [StringLength(50)]
    public string? LeaveType { get; set; }

    public int? MaxLimit { get; set; }

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
