using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeLeaveDetails
{
    public int LeaveDetailsId { get; set; }

    public int? EmployeeId { get; set; }

    public int? LeaveTypeId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? NoOfLeave { get; set; }

    public string? Remarks { get; set; }
    public bool? IsBillRequired { get; set; }
    public string? TicketId { get; set; }
    public string? BillName { get; set; }

    public int? LeaveStatus { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
    public string? ApprovedBy2 { get; set; }
    public DateTime? ApprovedOn2 { get; set; }
    public int? UnitId { get; set; }
    public int? Lavel1 { get; set; }
    public int? Lavel2 { get; set; }
    public int? Lavel3 { get; set; }
    public virtual EmployeeMaster? Employee { get; set; }

}
