using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Exit;
public partial class EmployeeSettlementSummeryDTO
{
    public int SettlementId { get; set; }
    public int? EmployeeId { get; set; }

    public int? OtherPayments { get; set; }

    public int? OtherDeductions { get; set; }
    public int? UnitId { get; set; }

    public string? Remarks { get; set; }

    public bool IsFixed { get; set; }
    public bool IsMailSent { get; set; }
    public DateTime ProcessDate { get; set; }
    public int? ProcessBy { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
}
