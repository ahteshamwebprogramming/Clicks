using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTProjectCategoryMasterViewModel
{
    public PTProjectCategoryDTO? projectCategoryDTO { get; set; }
    public List<PTProjectCategoryDTO>? projectCategoryList { get; set; }
}
