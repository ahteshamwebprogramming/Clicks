using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("LanguageUnitMaster")]
public partial class LanguageUnitMaster
{
    [Key]
    public int LanguageId { get; set; }

    public string? Language { get; set; }

    public int? LanguageParentId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; } 
    public int? UnitId { get; set; }
}
