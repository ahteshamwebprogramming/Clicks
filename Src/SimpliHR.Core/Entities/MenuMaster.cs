using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SimpliHR.Core.Entities;

public partial class MenuMaster
{
    public int MenuId { get; set; }

    public string? MenuName { get; set; }
    public string? Role { get; set; }

    public string? PageLink { get; set; }

    public int? ParentMenuId { get; set; }

    public int? ModuleId { get; set; }

    public string? Icon { get; set; }

    public int? Sn { get; set; }
    public int? IsHeading { get; set; }

    public int? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ModuleMaster? Module { get; set; }

    public virtual ICollection<RoleMenuMapping> RoleMenuMappings { get; set; } = new List<RoleMenuMapping>();
}
