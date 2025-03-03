using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("BankUnitMaster")]
public partial class BankUnitMaster
{
    [Key]
    public int BankId { get; set; }

    [StringLength(50)]
    public string? BankName { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
    public int? UnitId { get; set; }
}
