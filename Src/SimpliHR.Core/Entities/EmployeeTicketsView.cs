using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeTicketsView
{

    public int? Id { get; set; }
    public string? TicketId { get; set; }
    public string? Type { get; set; }
    public string? EmployeeName { get; set; }

    public string? TicketType { get; set; }

    public DateTime? CreatedOn { get; set; }
    public DateTime ApprovedOn { get; set; }
    public int? StatusId { get; set; }
    public string? Status { get; set; }

}
