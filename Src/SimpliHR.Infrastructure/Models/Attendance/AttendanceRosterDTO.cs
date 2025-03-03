using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SimpliHR.Infrastructure.Models.Attendance;

public partial class AttendanceRosterDTO
{
    public int RosterId { get; set; }

    public int? UnitId { get; set; }

    public string? EncryptedId { get; set; }

    public string? RosterName { get; set; }

    public int? WorkLocationType { get; set; }

    public int? WorkLocationId { get; set; } = 0;

    public int? DepartmentId { get; set; }

    public int? EmployeesSelection { get; set; }
    //public string? RosterEmployeeIDs { get; set; }
    public int? RosterPeriod { get; set; }

    public int? ShiftId { get; set; }
    public string? ShiftCode { get; set; }
    public int? ShiftMonth { get; set; } = 0;

    public int? ShiftYear { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; } = true;

   public string RosterEmployeeIDs { get; set; }

}
public class AttendanceViewModel
{
    public int? UnitId { get; set; }
    public AttendanceRosterDTO AttendanceRoster { get; set; } = new AttendanceRosterDTO();
    public AttendanceMastersKeyValues? AttendanceMastersKeyValues { get; set; }
    public IList<AttendanceRosterDTO> AttendanceRosterList { get; set; } = new Collection<AttendanceRosterDTO>();
    public string ViewScreen { get; set; } = "List";
    public string DisplayMessage { get; set; } = "_blank";
}
