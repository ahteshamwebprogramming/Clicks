using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("AcademicMaster")]
public partial class AcademicMaster
{
    [Key]
    public int AcademicId { get; set; }

    [StringLength(10)]
    public string? AcademicCode { get; set; }

    [StringLength(100)]
    public string? AcademicName { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
    public int? UnitId { get; set; }
}
