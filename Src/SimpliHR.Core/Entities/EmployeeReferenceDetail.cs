using SimpliHR.Infrastructure.Models.Employee;
using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeReferenceDetail
{
    public int EmployeeReferenceId { get; set; }

    public int? ReferenceOf { get; set; }
    public string? PersonName { get; set; }

    public string? PresentCompany { get; set; }

    public string? ReferenceDesignation { get; set; }

    public string? ReferenceContactNo { get; set; }

    public string? ReferenceMobileNo { get; set; }

    public string? HowYouKnow { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }
}
