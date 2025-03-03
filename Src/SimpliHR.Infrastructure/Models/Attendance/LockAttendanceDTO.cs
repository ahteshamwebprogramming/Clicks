using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Infrastructure.Models.Attendance;
public partial class LockAttendanceDTO
{
    public int UnitId { get; set; }
    public int LockMonth { get; set; }
    public int LockYear { get; set; }
    public bool AttendanceByPass { get; set; }
    public bool RunLeaveScheduler { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string DisplayMessage { get; set; } = "_blank";
}


public partial class LockAttendanceVM
{
    public List<LockAttendanceDTO> LockAttendancelist { get; set; } = new List<LockAttendanceDTO>();
    public LockAttendanceDTO LockAttendance { get; set; } = new LockAttendanceDTO();
    public List<int> YearList { get; set; } = CommonHelper.GetYears(DateTime.Now.Year-1, 2);
    public int LoggedInUser { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
}

