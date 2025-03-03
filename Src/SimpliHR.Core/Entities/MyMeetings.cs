
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("MyMeetings")]
public partial class MyMeetings
{
    [Key]
    public int? MeetingID { get; set; }

    public int? EmployeeId { get; set; }

    public int? UnitId { get; set; }

    public string? UserId { get; set; }

    public string? UserPassword { get; set; }  

    public string? UserType { get; set; }

    public bool? IsActive { get; set; }

 //   public virtual EmployeeMaster? Employee { get; set; }
}
