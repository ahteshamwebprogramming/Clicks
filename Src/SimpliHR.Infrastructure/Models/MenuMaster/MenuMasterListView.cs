using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.MenuMaster;

public class MenuMasterListView
{
    public MenuMasterDTO MenuMaster { get; set; }
    public List<MenuMasterDTO> MenuMasterList { get; set; }
    public List<MenuMasterDTO> MenuMasterListAll { get; set; }
    public List<ModuleMasterDTO> ModuleMasterList { get; set; }
}
