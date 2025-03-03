using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("EmployeeLeaveHistory")]
public partial class EmployeeLeaveHistory
{
    [Key]
    [Column("LeaveID")]
    public int LeaveId { get; set; }

    [Column("EmployeeID")]
    public int? EmployeeId { get; set; }

    [Column("LeaveTypeID")]
    public int? LeaveTypeId { get; set; }

    public double? OpeningBalance { get; set; }

    public double? Used { get; set; }

    public double? ClosingBalance { get; set; }

    [StringLength(500)]
    public string? LeaveReason { get; set; }

    public int? LeaveStatus { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
}
