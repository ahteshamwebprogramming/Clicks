using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Master;

public class NewsCategoryTagMasterDTO
{
    public int NewsCategoryTagId { get; set; }
    public string? encNewsCategoryTagId { get; set; }

    public string NewsCategoryTag { get; set; } = null!;

    public int UnitId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public string? Source { get; set; }
}

public class NewsCategoryTagViewModel
{
    public NewsCategoryTagMasterDTO? NewsCategoryTag { get; set; }
    public List<NewsCategoryTagMasterDTO>? NewsCategoryTagList { get; set; }
}
