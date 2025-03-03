using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("ResourceMaster")]
public partial class ResourceMaster
{
    [Key]
    public int ResourceId { get; set; }

    [StringLength(100)]
    public string? ResourceName { get; set; }

    [StringLength(500)]
    public string? ResourceDesc { get; set; }

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
