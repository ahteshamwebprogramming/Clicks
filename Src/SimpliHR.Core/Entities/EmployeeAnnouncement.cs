using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("EmployeeAnnouncement")]
public class EmployeeAnnouncement
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeAnnouncementId { get; set; }

    public int UnitId { get; set; }

    public int AnnouncementType { get; set; }
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Poll { get; set; }

    public bool? Survey { get; set; }

    public string Departments { get; set; } = null!;

    public string KeywordsRaw { get; set; } = null!;

    public string Keywords { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
    public DateTime PublishDate { get; set; }

    public TimeSpan PublishTime { get; set; }
}
