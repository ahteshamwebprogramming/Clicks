using SimpliHR.Core.Entities;
using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeFamilyDetail
{
    public int EmployeeFamilyDetailId { get; set; }
    
    public int? EmployeeId { get; set; }
    
    public string? MemberName { get; set; }

    public string? Relationship { get; set; }

    public DateTime? MemberDob { get; set; }


    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
    public virtual EmployeeMaster? Employee { get; set; }


}
