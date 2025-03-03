using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

[Table("LoanMaster")]
public partial class LoanMaster
{
    [Key]
    public int SanctionId { get; set; }

    public int? EmployeeId { get; set; }
     public decimal? SanctionAmount { get; set; }
    public decimal? MonthlyInstallment { get; set; }
    public int? UnitId { get; set; }
    public int? RepaymentTenure { get; set; }
    public int? RepaymentStartMonth { get; set; }
    public int? RepaymentStartYear { get; set; }
    public int? RepaymentStopMonth { get; set; }
    public int? RepaymentStopYear { get; set; }
    public decimal? ClosingBalance { get; set; } 
    public int? InstallmentMode { get; set; }
    public int? Status { get; set; }
    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    //[NavigationProperty]
    //public virtual EmployeeMaster Employees { get; set; } = null!;

}
