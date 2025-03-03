using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeAcademicDTO

{
    public int AcademicDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? TicketId { get; set; }

    public string? EntrySource { get; set; }
    
    public string? InstituteName { get; set; }

    public int? AcademicId { get; set; }

    public string? AcademicName { get; set; }

    public int? PassingYear { get; set; }

    public string? Percentage { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMasterDTO? Employee { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;

    public string FormName { get; set; }
    public int? ClientId { get; set; }
}
