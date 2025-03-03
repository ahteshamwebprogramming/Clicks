using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;


public partial class EmployeeLeaveBalance
{
    public int LeaveBalanceId { get; set; }

    public int? EmployeeId { get; set; }   
    public int? LeaveTypeId { get; set; }   

    public string LeaveTypeCode { get; set; }

    public decimal? OpeningBalance { get; set; }

    public decimal? LeaveBalance { get; set; }

    public decimal? TotalApplied { get; set; }

    public decimal? TotalAvailed { get; set; }

    public int? CalenderYearId { get; set; }   

    public bool? IsActive { get; set; }

    //[NavigationProperty]
    //public virtual LeaveTypeMaster LeaveTypes { get; set; } = null!;


}
