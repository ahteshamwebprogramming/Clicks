using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeExperienceDetailDTO
{
    public int ExperienceDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? TicketId { get; set; }

    public string? EntrySource { get; set; }

    public string? CompanyName { get; set; }

    public int? ExperienceJobTitleId { get; set; }

    public string? JobTitle { get; set; }
    public DateTime? JoinDate { get; set; }

    public DateTime? LastWorkingDate { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public EmployeeMasterDTO? Employee { get; set; }

    public string FormName { get; set; }
    public int? ClientId { get; set; }
}
