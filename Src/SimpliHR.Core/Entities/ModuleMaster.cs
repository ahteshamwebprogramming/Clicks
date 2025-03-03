using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("ModuleMaster")]
public partial class ModuleMaster
{
    [Key]
    public int ModuleId { get; set; }

    [StringLength(100)]
    public string? ModuleShortName { get; set; }

    [StringLength(200)]
    public string? ModuleName { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
    public virtual ICollection<MenuMaster> MenuMasters { get; set; } = new List<MenuMaster>();


}
