using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValues;

public class AnnouncementTypeKeyValues
{
    public int AnnouncementTypeId { get; set; }
    public string? encAnnouncementTypeId { get; set; }
    public string AnnouncementType { get; set; } = null!;
}
