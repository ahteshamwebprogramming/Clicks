using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Attendance;
public partial class AttendanceLateSettingDTO
{
    public int LateMasterId { get; set; }
    public string EncryptedId { get; set; }
    public int? NoOfLate { get; set; }
    public string? AppliedOn { get; set; }
    public int? UnitId { get; set; }
    public TimeSpan? LateDuration { get; set; }
    public string? ShowPostLimit { get; set; }

    public string? ActionPostLate { get; set; }

    public bool? CanDeductLeave { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public bool? Refill { get; set; }
    public bool? IsActive { get; set; }    
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }   
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public List<AttendanceLateSettingDTO>? AttendanceLateSettingList { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
}




