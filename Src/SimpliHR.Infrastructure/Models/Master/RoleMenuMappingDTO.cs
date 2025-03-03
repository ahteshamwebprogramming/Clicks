using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Masters;

public partial class RoleMenuMappingDTO
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

    public virtual JobTitleMasterDTO? JobTitle { get; set; }

    public virtual MenuMasterDTO? Menu { get; set; }

    public virtual RoleMasterDTO? Role { get; set; }
}

public partial class ListRoleMenuMappingDTO
{
    public List<MenuMasterDTO> Menus { get; set; }

    public List<JobTitleMasterDTO> jobTitles { get; set; }
    public List<RoleMasterDTO> Roles { get; set; }

    public List<DepartmentMasterDTO> Departments { get; set; }
}
public partial class ListRoleMenuMappingDTOForSave
{
    public int[] Menus { get; set; }

    public int JobTitleId { get; set; }
    public int RoleId { get; set; }
    public int DepartmentId { get; set; }
    public int ClientId { get; set; }
    public int UnitId { get; set; }
}

