using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Attendance;

public partial class FaceAttendanceDTO
{
    public int FaceAttendanceId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? DutyDate { get; set; }

    public DateTime? ClockInTime { get; set; }

    public DateTime? ClockOutTime { get; set; }

    public string? sClockInTime { get; set; } = "";

    public string? sClockOutTime { get; set; } = "";

    public bool? IsAutoClockedOut { get; set; }

    public bool? IsShiftOn { get; set; }
    public TimeOnly? StayDuration { get; set; }
    public string? DBMessage { get; set; }
}
