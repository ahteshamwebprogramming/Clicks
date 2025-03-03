using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Infrastructure.Models.Leave;

public partial class EmployeeLeaveBalanceDTO
{
    public int LeaveBalanceId { get; set; }

    public int? EmployeeId { get; set; }
    public int? LeaveTypeId { get; set; }

    public string LeaveTypeCode { get; set; }

    public string LeaveType { get; set; }

    public decimal? OpeningBalance { get; set; }

    public decimal? LeaveBalance { get; set; }

    public decimal? TotalApplied { get; set; }

    public decimal? TotalAvailed { get; set; }

    public int? CalenderYearId { get; set; }

    //[ValidateNever]
    //public LeaveTypeMasterDTO LeaveTypes { get; set; } = null!;
    public bool? IsActive { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public HttpResponseMessage? HttpMessage { get; set; }
        //public List<RoleMasterDTO>? RoleMasterList { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;

  
    public List<EmployeeLeaveBalanceDTO>? EmployeeLeaveBalanceList { get; set; } = null;
}

