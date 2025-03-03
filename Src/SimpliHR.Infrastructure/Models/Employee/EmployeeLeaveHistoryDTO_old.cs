using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeLeaveHistoryDTO_old
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

    public HttpResponseMessage? HttpMessage { get; set; }
    public List<RoleMasterDTO>? RoleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}
