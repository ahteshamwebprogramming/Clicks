using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class SideBarMenusDTO
{
    public List<MenuMasterDTO> Menus { get; set; }
    public ClientSettingDTO ClientSettings { get; set; }
}
