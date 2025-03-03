using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("MainDirectory")]
public partial class MainDirectory
{
    [Key]
    public int? DirectoryId { get; set; }
    public string? DirectoryColumns { get; set; }
    public bool? IsActive { get; set; }
    public int? PositionId { get; set; }

}
