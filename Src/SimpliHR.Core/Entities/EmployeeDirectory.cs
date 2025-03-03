using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("EmployeeDirectory")]
public partial class EmployeeDirectory
{
    [Key]
    public int? EmployeeDirectoryId { get; set; }
    public int? ParentDirectoryId { get; set; }
    public int? UnitId { get; set; }
    public int? PositionId { get; set; }
    public string? SearchColumns { get; set; }
    public bool? IsActive { get; set; }

}
