using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Employee;

public class EmployeeTicketsViewDTO
{
    public int? Id { get; set; }
    public string? TicketId { get; set; }
    public string? Type { get; set; }
    public string? EmployeeName { get; set; }
    public string? TicketType { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ApprovedOn { get; set; }
    public int? StatusId { get; set; }
    public string? Status { get; set; }

    public List<EmployeeTicketsViewDTO>? EmployeeTickets { get; set; } = null;
    public List<EmployeeTicketsViewDTO>? EmployeeToMeTickets { get; set; } = null;
    public List<EmployeeTicketsViewDTO>? EmployeeByMeTickets { get; set; } = null;
}

public class EmployeeTicketsInputs
{
    public int? ModuleId { get; set; }

    public int? Status { get; set; }

    public string? TicketId { get; set; }

    public string? StartDate { get; set; }
    public string? EndDate { get; set; }

    public int? MgrId { get; set; }
    public int? UnitId { get; set; }
    public int? EmployeeId { get; set; }
    public int? IsAdmin { get; set; }
}
