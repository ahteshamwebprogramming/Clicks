using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("AppMessages")]
public class AppMessage
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageId { get; set; }

    public string? MessageSubject { get; set; }

    public string? MessageText { get; set; }

    public string? MessageHTML { get; set; }

    public string? MessageType { get; set; }
    public string? RedirectLink { get; set; }

    public int? ManagerId { get; set; }
    public int? ReferenceId { get; set; }

    public int? HODId { get; set; }

    public int? TargetEmployeeId { get; set; }

    public DateTime? PublishStartDate { get; set; }

    public DateTime? PublishEndDate { get; set; }

    public bool? IsViewed { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public bool? IsActive { get; set; }
}
