using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeLeaveBalanceReport
{
    public int ID { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

    public string? Designation { get; set; }

    public string? Department { get; set; }

    public string? Location { get; set; }
    public string? DOJ { get; set; }
    public string? TicketId { get; set; }
    public string? BillName { get; set; }

    public int? LeaveStatus { get; set; }

    public string? CreatedBy { get; set; }

    public string? LWD { get; set; }

    public string? LeaveType { get; set; }

    public decimal? OpeningBalance { get; set; }
    public decimal? Accrued { get; set; }
    public decimal? Availed { get; set; }
    public decimal? ClosingBalance { get; set; }
    public int? LeaveTypeId { get; set; }
   

}
