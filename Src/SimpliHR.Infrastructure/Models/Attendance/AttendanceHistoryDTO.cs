using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Infrastructure.Models.Attendance;
public partial class AttendanceHistoryDTO
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime DutyDate { get; set; }

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

    public bool? IsHoliday { get; set; }
    public bool IsAttendanceLocked { get; set; }
    public string? TicketId { get; set; }
    public string? HostName { get; set; }
    public string? IPAddress { get; set; }
    public string? GPSLocation { get; set; }
    public string? Longitude { get; set; }
    public string? Latitude { get; set; }
}

public partial class AttendanceDTO
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime DutyDate { get; set; }

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

    public string? TotalStayHours { get; set; }

    public string? Status { get; set; }

    public bool? IsManual { get; set; }
    public double? AttendanceInHours { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public decimal? ModifiedBy { get; set; }

    public string? ModifiedOn { get; set; }

    public string? EntrySource { get; set; }

    public int? EntrySourceId { get; set; }

    public bool? IsHoliday { get; set; }
    public bool IsAttendanceLocked { get; set; }
    public string? TicketId { get; set; }
    public string? HostName { get; set; }
    public string? IPAddress { get; set; }
    public string? GPSLocation { get; set; }
    public string? Longitude { get; set; }
    public string? Latitude { get; set; }

    public int ManualPunchId { get; set; }

    public string? ManualPunchReason { get; set; }

    public DateTime? manualPunchDate { get; set; }

    public TimeSpan? ManualPunchInTime { get; set; }

    public TimeSpan? ManualPunchOutTime { get; set; }

    public string PunchType { get; set; }

    public string ActionType { get; set; }

    public int? ActionBy { get; set; }

    public string ActionByName { get; set; }

    public DateTime? ActionOn { get; set; }

    public string ActionRemark { get; set; }

  

    //public string HostName { get; set; }
    //public string HostName { get; set; }
    //public string GPSLocation { get; set; }

    //public string Longitude { get; set; }
    //public string Latitude { get; set; }


}


public partial class AttendanceHistoryViewModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public double? EarlyDelayTime { get; set; }
    public string? sEarlyDelayTime { get; set; } = "";
    public TimeOnly? FaceAttendanceStayHours { get; set; }

//    [NotMapped]
//    public TimeSpan? TimeSpanFaceAttendanceStayHours
//    {
//        get { return TimeSpan.Parse(FaceAttendanceStayHours); }
//        set { FaceAttendanceStayHours = value.ToString(); }
        
//}
    public string? sFaceAttendanceStayHours { get; set; } = "";
    public DateTime? LastFaceLogin { get; set; }
    public DateTime? InTime { get; set; }
    public string? sInTime { get; set; }
    public DateTime? OutTime { get; set; }
    public string? sOutTime { get; set; }
    public string Status { get; set; }
    public int EmployeeId { get; set; }
    public string eEmployeeId { get; set; }
    public int UnitId { get; set; }
    public string DepartmentIds { get; set; } = "";
    public string EmployeeIds { get; set; } = "";
    public float DutyDays { get; set; } = 0;
    public float PresentDays { get; set; } = 0;
    public float AbsentDays { get; set; } = 0;
    public float HalfDays { get; set; } = 0;
    public float Leaves { get; set; } = 0;
    public float OutsideDuty { get; set; } = 0;
    public float WeeklyOff { get; set; } = 0;
    public string DaySelected { get; set; } = "";
    public float Holidays { get; set; } = 0;
    public float Approved { get; set; } = 0;
    public float Rejected { get; set; } = 0;
    public float CompOff { get; set; } = 0;
    public ulong EPOCHInOutTime { get; set; } = 0;
    public string ManualPunchReason { get; set; } = "";

    public string ProfilePic { get; set; } = "";

    public string? HostName { get; set; } = "";
    public string? IPAddress { get; set; } = "";
    public string? GPSLocation { get; set; } = "";
    public string? longitude { get; set; } = "";
    public string? latitude { get; set; } = "";
    public string? TicketId { get; set; } = "";
    public string? AttendanceType { get; set; } = "";
    public string? ShiftCode { get; set; } = "";
    public int? ShiftId { get; set; }
    public TimeOnly MaximumTime { get; set; }
    public TimeOnly MinimumTime { get; set; }
    public bool IsAutoClockedOut { get; set; }

    public int EmailProvider { get; set; }
    public string? DisplayName { get; set; }
    public string? Profile { get; set; }
    public List<GpslocationDetailDTO> GpslocationDetailList { get; set; } = new List<GpslocationDetailDTO>();
    public SortedDictionary<string,string> WeekDays { get; set; }=new SortedDictionary<string, string>();
    public AttendanceHistoryDTO AttendanceHistory { get; set; } = new AttendanceHistoryDTO();

    public List<AttendanceHistoryDTO> AttendanceHistoryList { get; set; } = new List<AttendanceHistoryDTO>();
    public List<AttendanceDTO> AttendanceList { get; set; } = new List<AttendanceDTO>();
    public List<DepartmentKeyValues> DepartmentMasterKeyValue { get; set; }
    public List<ShiftKeyValues> ShiftMasterKeyValue { get; set; }
    public List<EmployeeKeyValues> EmployeeMasterKeyValue { get; set; }
    public List<WorkLocationKeyValues> WorkLocationKeyValue { get; set; }
    public List<FaceAttendanceDTO> FaceRecognitionAttendanceList { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
}

