using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeCertificationDetail
{
    public int CertificationDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? CertificationName { get; set; }

    public int? YearOfCertification { get; set; }

    public string? Duration { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }
}
