using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Attendance;

public partial class ManualPunchesDTO
{
    public int ManualPunchId { get; set; }

    public int? EmployeeId { get; set; }

    public string EmployeeName { get; set; }

    public string? ManualPunchReason { get; set; }

    public DateTime? manualPunchDate { get; set; }

    public TimeSpan? ManualPunchInTime { get; set; }

    public TimeSpan? ManualPunchOutTime { get; set; }

    public string? PunchType { get; set; }

    public string ActionType { get; set; }

    public int? ActionBy { get; set; }

    public DateTime? ActionOn { get; set; }

    public string ActionRemark { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public bool? IsActive { get; set; }
}

public partial class ManualPunchesAttendanceViewModel
{
    public int ManualPunchId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }

    public string? ManualPunchReason { get; set; }

    public DateTime? manualPunchDate { get; set; }

    public TimeSpan? ManualPunchInTime { get; set; }

    public TimeSpan? ManualPunchOutTime { get; set; }

    public string? PunchType { get; set; }

    public decimal? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public bool? IsActive { get; set; }
    public string ShiftIDAttended { get; set; }

    public string? TicketId { get; set; }

}
public partial class ManualPunchesAction
{
    public string ManualPunchIds { get; set; }
    public string ActionType { get; set; }
    public string ActionRemarks { get; set; }
    public int? EmailProvider { get; set; }
    public string? DisplayName { get; set; }
    public string? Profile { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
}
public partial class ManualPunchesViewModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ManualPunchesDTO ManualPunchesDTO { get; set; }
    public List<ManualPunchesDTO> ManualPunchesList { get; set; } = new List<ManualPunchesDTO>();
    public List<ManualPunchesAttendanceViewModel> ManualPunchesAttendanceVMList { get; set; } = new List<ManualPunchesAttendanceViewModel>();
    public List<DepartmentKeyValues> DepartmentMasterKeyValue { get; set; }
    public List<ShiftKeyValues> ShiftMasterKeyValue { get; set; }
    public List<EmployeeKeyValues> EmployeeMasterKeyValue { get; set; }
   // public List<GpslocationDetailDTO> GpslocationDetailList { get; set; } = new List<GpslocationDetailDTO>();
    public string DisplayMessage { get; set; } = "_blank";

  
}