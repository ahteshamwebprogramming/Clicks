using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Exit;
public partial class EmployeeSettlementDetailslDTO
{
    public int EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? EmployeeCode { get; set; }

    public string? DOJ { get; set; }

    public string? DOR { get; set; }

    public string? LWD { get; set; }

    public string? Designation { get; set; }

    public string? Department { get; set; }

    public string? UnitName { get; set; }

    public string? Address { get; set; }

    public string? StateName { get; set; }

    public string? Location { get; set; }

    public float? NoticePeriod { get; set; }
    public float? EmpNoticePeriod { get; set; }
    public string? TotalServices { get; set; }
    public float? NoticeShortfall { get; set; }
    public float? ELBalance { get; set; }
    public int? NoticePeriodId { get; set; }
    public int? LeaveBalanceId { get; set; }
    public int? GratuityId { get; set; }
    public float? WorkingDays { get; set; }
    public string? Remarks { get; set; }


    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
}
