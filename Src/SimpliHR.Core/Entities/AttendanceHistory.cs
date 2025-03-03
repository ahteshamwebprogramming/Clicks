using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;


public partial class AttendanceHistory
{
    [ExplicitKey]
    public int EmployeeId { get; set; }

    [ExplicitKey]
    public DateTime DutyDate { get; set; }
    public string EmployeeName { get; set; }
    public int? UnitId { get; set; }

    public int? WorkLocationId { get; set; }

    public int? ShiftMonth { get; set; }

    public int? ShiftYear { get; set; }

    public string? ShiftIDScheduled { get; set; }

    public string? ShiftIDAttended { get; set; }

    public DateTime? ShiftStartTime { get; set; }

    public DateTime? ShiftEndTime { get; set; }

    public TimeSpan? InTime { get; set; }

    public TimeSpan? OutTime { get; set; }

    public decimal? AttendanceInSeconds { get; set; }

    public decimal? EarlyArrival { get; set; }

    public decimal? EarlyDeparture { get; set; }

    public decimal? LateArrival { get; set; }

    public decimal? LateDeparture { get; set; }

    public string? AttendanceType { get; set; }

    public TimeSpan? Present { get; set; }

    public TimeSpan? Absent { get; set; }

    public TimeSpan? Total { get; set; }

    public string? Status { get; set; }

    public bool? IsManual { get; set; }
    public string ManualPunchReason { get; set; }
    public double? AttendanceInHours { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public decimal? ModifiedBy { get; set; }

    public string? ModifiedOn { get; set; }

    public string? EntrySource { get; set; }

    public int? EntrySourceId { get; set; }

    public string? TicketId { get; set; }
    public bool? IsHoliday { get; set; }
    public string? HostName { get; set; }
    public string? IPAddress { get; set; }
    public string? GPSLocation { get; set; }
    public string? longitude { get; set; }
    public string? latitude { get; set; }
}


//public partial class AttendanceHistory
//{
//    [ExplicitKey]
//    public int EmployeeId { get; set; }

//    [ExplicitKey]
//    public DateOnly DutyDate { get; set; }

//    public int? ClientId { get; set; }

//    public int? WorkLocationId { get; set; }

//    public int? ShiftMonth { get; set; }

//    public int? ShiftYear { get; set; }

//    public string? ShiftIdScheduled { get; set; }

//    public string? ShiftIdAttended { get; set; }

//    public DateTime? ShiftStartTime { get; set; }

//    public DateTime? ShiftEndTime { get; set; }

//    public TimeOnly? InTime { get; set; }

//    public TimeOnly? OutTime { get; set; }

//    public decimal? AttendanceInSeconds { get; set; }

//    public decimal? EarlyArrival { get; set; }

//    public decimal? EarlyDeparture { get; set; }

//    public decimal? LateArrival { get; set; }

//    public decimal? LateDeparture { get; set; }

//    public string? AttendanceType { get; set; }

//    public TimeSpan? Present { get; set; }

//    public TimeSpan? Absent { get; set; }

//    public TimeSpan? Total { get; set; }

//    public string? Status { get; set; }

//    public bool? IsManual { get; set; } = false;

//    public string ManualPunchReason { get; set; }


//    public float? AttendanceInHours { get; set; }

//    public string? CreatedBy { get; set; }

//    public DateTime? CreatedOn { get; set; }

//    public decimal? ModifiedBy { get; set; }

//    public string? ModifiedOn { get; set; }

//    public string? EntrySource { get; set; }

//    public int? EntrySourceId { get; set; }

//    public bool? IsHoliday { get; set; } = false;
//}
