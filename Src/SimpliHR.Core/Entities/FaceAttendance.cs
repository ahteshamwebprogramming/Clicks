using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class FaceAttendance
{
    public int FaceAttendanceId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? DutyDate { get; set; }

    public DateTime? ClockInTime { get; set; }

    public DateTime? ClockOutTime { get; set; }

    public bool? IsAutoClockedOut { get; set; }

    public bool? IsShiftOn { get; set; }

    public string? DBMessage { get; set; }

    public TimeOnly? StayDuration { get; set; }
}
