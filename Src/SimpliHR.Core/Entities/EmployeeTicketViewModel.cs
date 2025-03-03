using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;


public class EmployeeTicketViewModel
{
    public int? TicketId { get; set; }
    public string? encTicketId { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? Department { get; set; }
    
}
