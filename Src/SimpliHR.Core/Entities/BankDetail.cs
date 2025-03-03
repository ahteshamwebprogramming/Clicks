using System;
using System.Collections.Generic;

namespace SimpliHR.Core.Entities;

public partial class BankDetail1
{
    public int BankDetailId { get; set; }

    public int? EmployeeId { get; set; }

    public string? BankId { get; set; }

    public string? AccountNo { get; set; }

    public string? Ifsccode { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual EmployeeMaster? Employee { get; set; }
    public int? UnitId { get; set; }
}
