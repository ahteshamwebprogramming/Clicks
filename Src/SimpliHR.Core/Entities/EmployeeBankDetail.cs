using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class EmployeeBankDetail
{
    public int BankDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? BankId { get; set; }

    public string? AccountNo { get; set; }

    public string? IFSCCode { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }

    //[NavigationProperty]
    //public virtual BankMaster Bank { get; set; } = null!;

    //[NavigationProperty]
    //public virtual BankMaster Bank { get; set; } = null!;
}
