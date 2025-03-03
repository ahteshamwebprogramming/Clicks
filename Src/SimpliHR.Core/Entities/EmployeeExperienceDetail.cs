using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeExperienceDetail
{
    public int ExperienceDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? CompanyName { get; set; }

    public int? ExperienceJobTitleId { get; set; }

    public DateTime? JoinDate { get; set; }

    public DateTime? LastWorkingDate { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }
}
