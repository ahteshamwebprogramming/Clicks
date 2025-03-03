using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Core.Entities;

[Table("WorkFlowSettings")]
public partial class WorkFlowSettings
    {
    [Key]
    public int WorkFlowSettingsId { get; set; }

    [StringLength(50)]
    public string? Name { get; set; }

    public int? ActionId { get; set; }

    public int? ModuleId { get; set; }

    public int? UnitId { get; set; }

    // public int? LevelId1 { get; set; }

    public int? Authority1 { get; set; }
   // public int? LevelId2 { get; set; }

    public int? Authority2 { get; set; }

   // public int? LevelId3 { get; set; }

    public int? Authority3 { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    [NavigationProperty]
    public virtual ModuleMaster Module { get; set; } = null!;

   

}

