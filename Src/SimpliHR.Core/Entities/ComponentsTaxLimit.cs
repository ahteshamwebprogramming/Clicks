using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("ComponentsTaxLimit")]
public partial class ComponentsTaxLimit
{
    [Key]
    public int TaxLimitId { get; set; }
     
    public decimal? GratuityLimit { get; set; }
    public decimal? LeaveEncashmentLimit { get; set; }
    public decimal? PFLimit { get; set; }
    public int? CreatedBy { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }
     public int? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
    public int? UnitId { get; set; }
}
