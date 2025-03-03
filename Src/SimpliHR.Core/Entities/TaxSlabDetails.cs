using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("TaxSlabDetails")]
public partial class TaxSlabDetails
{
    [Key]
    public int SlabID { get; set; }
    public decimal? AmtFrom { get; set; }
    public decimal? AmtTo { get; set; }
    public bool? IsActive { get; set; }
    public int? TaxPercentage { get; set; }
    public string? Regime { get; set; }
    public int? CessTax { get; set; }
    public int? UnitId { get; set; }
    public int? AgeGroupId { get; set; }
    public string? FY { get; set; }
}
