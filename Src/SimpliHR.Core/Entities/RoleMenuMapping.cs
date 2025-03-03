using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;


public partial class RoleMenuMapping
{
    public int RoleMenuMappingId { get; set; }

    public int? RoleId { get; set; }

    public int? JobTitleId { get; set; }
    public int? DepartmentId { get; set; }

    public int? MenuId { get; set; }
    public int? ClientId { get; set; }
    public int? UnitId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int? IsActive { get; set; }

    public virtual JobTitleMaster? JobTitle { get; set; }

    public virtual MenuMaster? Menu { get; set; }

    public virtual RoleMaster? Role { get; set; }

    public virtual DepartmentMaster? Department { get; set; }
}
