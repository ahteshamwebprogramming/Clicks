using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;


public partial class LeaveAttribute
{
    public int LeaveAttributeId { get; set; }

    public int? PolicyId { get; set; }
    public string? PolicyName { get; set; }
    public int? CalenderYearId { get; set; }
    public int? UnitId { get; set; }

    public int? LeaveTypeId { get; set; }

    public int? MaxLimitPerYear { get; set; }

    public decimal? MaxLimitPerMonth { get; set; }

    public string? AccuralType { get; set; }

    public int? MaxAccuralLimit { get; set; }

    public int? MaxAvailLimit { get; set; }

    public string? CreditLimit { get; set; }

    public int? MinAvailLimit { get; set; }
    public int? NoOfMonth { get; set; }

    public int? WeekOffType { get; set; }

    public int? NoOfTimesAvailLimit { get; set; }

    public int? AvailFromType { get; set; }

    public bool? IsRoundOff { get; set; }

    public bool? IsCarryForward { get; set; }

    public int? CarryForwardLimit { get; set; }

    public bool? IsNegativeBalance { get; set; }

    public int? NegativeBalanceLimit { get; set; }

    public bool? IsBillRequired { get; set; }

    public decimal? MinLeaveNoForBill { get; set; }
    public bool? IsHalfDay { get; set; }

    public int? InterveningHolidays { get; set; }

    public bool? IsApplyAdmin { get; set; }

    public bool? Ispaternity { get; set; }

    public bool? Ismternity { get; set; }

    public bool? IsClubbing { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
