using SimpliHR.Infrastructure.Models.Complaint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.ViewModels.QueryManagement;

public class ComplaintCategoryViewModel
{
    public ComplaintCategoryDTO? ComplaintCategory { get; set; }
    public List<ComplaintCategoryDTO>? ComplaintCategories { get; set; }
}
