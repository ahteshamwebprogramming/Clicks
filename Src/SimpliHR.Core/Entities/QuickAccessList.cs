using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("QuickAccessList")]
public partial class QuickAccessList
{
    [Key]
    public int? QuickAccessId { get; set; }
    public string? QuickAccessLogo { get; set; }
    public string? QuickAccessName { get; set; }
    public bool? IsActive { get; set; }

}
