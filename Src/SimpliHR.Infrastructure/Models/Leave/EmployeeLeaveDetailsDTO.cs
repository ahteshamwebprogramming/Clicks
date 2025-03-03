using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Infrastructure.Models.Leave;

public partial class EmployeeLeaveDetailsDTO
    {
    public int LeaveDetailsId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? EmployeeCode { get; set; }

    public string? Location { get; set; }

    public int? LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }

    public string? LeaveTypeCode { get; set; }
    public int? UnitId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? NoOfLeave { get; set; }

    public string? Remarks { get; set; }
    public string? DisplayName { get; set; }
    public string? Reason { get; set; }
    public bool? IsBillRequired { get; set; }
    public string? TicketId { get; set; }
    public string? BillName { get; set; }
    public string? Profile { get; set; }   

    public int? LeaveStatus { get; set; }

    public int? EmailProvider { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public string? ApprovedBy2 { get; set; }
    public DateTime? ApprovedOn2 { get; set; }
    public string? DOJ { get; set; }
    public string? DOC { get; set; }
    public int? Lavel1 { get; set; }
    public int? Lavel2 { get; set; }
    public int? Lavel3 { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public HttpResponseMessage? HttpMessage { get; set; }
        //public List<RoleMasterDTO>? RoleMasterList { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;

    public LeaveAttributeMasterKeyValues? LeaveAttributeKeyValues { get; set; }
    public List<EmployeeLeaveDetailsDTO>? EmployeeLeaveList { get; set; } = null;
    public List<EmployeeLeaveBalanceDTO>? EmployeeLeaveSummary { get; set; } = null;
    public List<EmployeeLeaveHistoryDTO>? EmployeeLeaveHistoryList { get; set; } = null;
    public List<EmployeeLeaveDetailsDTO>? EmployeeRevarsalList { get; set; } = null;
    public List<EmployeeLeaveDetailsDTO>? EmployeeAppliedList { get; set; } = null;
}
public partial class LeaveAction
{
    public string? TicketId { get; set; }
    public string ActionType { get; set; }

    public string? LeaveIds { get; set; }
    public string? ActionRemarks { get; set; }
    public string? Profile { get; set; }
    public int? EmployeeId { get; set; }
    public string? ApprovedBy { get; set; }
    public int? EmailProvider { get; set; }
    public string? DisplayName { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
}

public partial class CompOffAction
{
    public string? TicketId { get; set; }
    public string? ActionType { get; set; }
    public string? CompOffIds { get; set; }
    public string? ActionRemarks { get; set; }    
    public int? EmployeeId { get; set; }
    public string? ApprovedBy { get; set; }
    public int? EmailProvider { get; set; }
    public string? DisplayName { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
}