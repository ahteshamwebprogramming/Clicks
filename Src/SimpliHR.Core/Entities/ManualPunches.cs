using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class ManualPunches
{
    public int ManualPunchId { get; set; }

    public int? EmployeeId { get; set; }

    public string? ManualPunchReason { get; set; }

    public DateTime? ManualPunchDate { get; set; }

    public TimeSpan? ManualPunchInTime { get; set; }

    public TimeSpan? ManualPunchOutTime { get; set; }

    public string? PunchType { get; set; }

    public decimal? ActionBy { get; set; }

    public DateTime? ActionOn { get; set; }

    public string ActionType { get; set; }

    public string ActionRemark { get; set; }
    
    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsApprovalMailSent { get; set; }

    public bool? IsActionMailSent { get; set; }
    public string? HostName { get; set; }
    public string? IPAddress { get; set; }
    public string? GPSLocation { get; set; }
    public string? longitude { get; set; }
    public string? latitude { get; set; }
    public string? TicketId { get; set; }
}


