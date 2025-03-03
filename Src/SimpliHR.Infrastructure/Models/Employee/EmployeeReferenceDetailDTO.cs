using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeReferenceDetailDTO
{
    public int EmployeeReferenceId { get; set; }

    public int? ReferenceOf { get; set; }

    public string? EmployeeCode { get; set; }

    public string? TicketId { get; set; }

    public string? EntrySource { get; set; }

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

    public virtual EmployeeMasterDTO? Employee { get; set; }

    public string FormName { get; set; }
    public int? ClientId { get; set; }

}
