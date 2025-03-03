using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeFamilyDetailDTO
{
    public int EmployeeFamilyDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public DateTime? MemberDob { get; set; }
    public string? MemberName { get; set; }
    public string? Relationship { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }

    public string FormName { get; set; }
    public int? ClientId { get; set; }

    public string? TicketId { get; set; }
    public string? EntrySource { get; set; }


}
