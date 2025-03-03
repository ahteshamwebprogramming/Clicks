using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("SalaryComponentMaster")]
public partial class SalaryComponentMaster
{
    [Key]
    public int SalaryComponentId { get; set; }

    [StringLength(50)]
    public string? SalaryComponentTitle { get; set; }

    public int? SalaryComponentDisapyOrder { get; set; }

    [StringLength(500)]
    public string? SalaryComponentDec { get; set; }

    public string? SalaryComponentType { get; set; }
    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [Column("modifedBy")]
    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column("modifiedOn", TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    public bool? IsBasic { get; set; }

    public bool? IsFixed { get; set; }
    
    public int? UnitId { get; set; }
}
