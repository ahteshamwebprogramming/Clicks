using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Exit;
public partial class EmployeeFnFDetailslDTO
{
    public int EmployeeFnFId { get; set; }
    public int? EmployeeId { get; set; }

    public int? NoticePeriod { get; set; }

    public int? LeaveBalance { get; set; }

    public int? Gratuity { get; set; }

    public int? IsProcess { get; set; }

    public int? IsMailSent { get; set; }
    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }
    public int? UnitId { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
}
