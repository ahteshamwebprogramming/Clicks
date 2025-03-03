using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("SalaryComponentMaster")]
public partial class SalaryComponentMaster
{
    [Key]
    [Column("SalaryComponentID")]
    public int SalaryComponentId { get; set; }

    [StringLength(50)]
    public string? SalaryComponentTitle { get; set; }

    public int? SalaryComponentDisapyOrder { get; set; }

    [StringLength(500)]
    public string? SalaryComponentDec { get; set; }

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
}
