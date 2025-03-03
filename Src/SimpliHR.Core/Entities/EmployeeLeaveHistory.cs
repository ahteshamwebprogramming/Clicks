using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeLeaveHistory
{
    public int LeaveId { get; set; }

    public int? EmployeeId { get; set; }

    public int? LeaveTypeId { get; set; }

    public double? OpeningBalance { get; set; }

    public double? Used { get; set; }

    public double? ClosingBalance { get; set; }

    public string? LeaveReason { get; set; }

    public int? LeaveStatus { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public virtual EmployeeMaster? Employee { get; set; }

}
