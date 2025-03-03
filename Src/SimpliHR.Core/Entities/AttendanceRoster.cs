using Dapper.Contrib.Extensions;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("AttendanceRoster")]
public partial class AttendanceRoster
{
   [Key] // not [ExplicitKey]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RosterId { get; set; }
    public int UnitId { get; set; }

    public string? RosterName { get; set; }

    public int? WorkLocationType { get; set; }

    public int? WorkLocationId { get; set; } = 0;

    public int? DepartmentId { get; set; }

    public int? EmployeesSelection { get; set; }
    //public string? RosterEmployeeIDs { get; set; }

    public int? RosterPeriod { get; set; }

    public string ShiftCode { get; set; }

    public int? ShiftMonth { get; set; } = 0;

    public int? ShiftYear { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
   
}
