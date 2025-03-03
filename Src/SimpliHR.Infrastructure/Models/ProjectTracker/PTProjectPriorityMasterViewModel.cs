using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTProjectPriorityMasterViewModel
{
    public PTProjectPriorityDTO? projectPriorityDTO { get; set; }
    public List<PTProjectPriorityDTO>? projectPriorityList { get; set; }
}
