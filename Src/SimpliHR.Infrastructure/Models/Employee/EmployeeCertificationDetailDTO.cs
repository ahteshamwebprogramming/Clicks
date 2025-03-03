using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeCertificationDetailDTO
{
    public int CertificationDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }
    public string? TicketId { get; set; }

    public string? EntrySource { get; set; }

    public string? CertificationName { get; set; }

    public int? YearOfCertification { get; set; }

    public string? Duration { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public EmployeeMasterDTO? Employee { get; set; }

    public string FormName { get; set; }
    public int? ClientId { get; set; }
}
