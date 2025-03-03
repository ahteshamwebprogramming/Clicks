using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValue;

namespace SimpliHR.Infrastructure.Models.Attendance;
public partial class AttendanceSettingDTO
{
    public string EncryptedId { get; set; }
    public int AttendanceSettingId { get; set; }

    public int? ShiftId { get; set; }
    public string? ShiftCode { get; set; }
    public int? UnitId { get; set; }
    public int? LegendType { get; set; }
    public int? LocationId { get; set; }
    public string LocationIDs { get; set; }
    public TimeSpan? MinimumTime { get; set; }
    public TimeSpan? MaximumTime { get; set; }
    public bool? IsActive { get; set; }

    public string? ShiftName { get; set; }

    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
    public List<AttendanceSettingDTO>? AttendanceSettingList { get; set; }
    public List<ShiftKeyValues>? ShiftMasterList { get; set; }

    public List<WorkLocationKeyValues>? LocationKeyValues { get; set; }

    public ShiftMasterDTO Shift { get; set; } = null!;
    public int? HttpStatusCode { get; set; } = 200;
}

