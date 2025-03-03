using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("QuickAccessUnitList")]
public partial class QuickAccessUnitList
{
    [Key]
    public int? QuickAccessId { get; set; }
    public string? QuickAccessLink { get; set; }
    public int? ParentQuickAccessId { get; set; }
    public int? UnitId { get; set; }
    public int? PositionId { get; set; }
    public bool? IsActive { get; set; }
    public string? QuickAccessLogo { get; set; }
    public string? QuickAccessName { get; set; }
}
