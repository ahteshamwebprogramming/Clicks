using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Master;

public class AnnouncementTypeMasterDTO
{
    public int AnnouncementTypeId { get; set; }
    public string? encAnnouncementTypeId { get; set; }

    public string AnnouncementType { get; set; } = null!;

    public int UnitId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
    public string? Source { get; set; }
}
public class AnnouncementTypeViewModel
{
    public AnnouncementTypeMasterDTO? AnnouncementTypeMaster { get; set; }
    public List<AnnouncementTypeMasterDTO>? AnnouncementTypeMasterList { get; set; }
}
