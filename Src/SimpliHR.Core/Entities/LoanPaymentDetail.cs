using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Core.Entities;

[Table("LoanPaymentDetail")]
public partial class LoanPaymentDetail
{
    [Key]
    public int RepaymentId { get; set; }

    public int? SanctionId { get; set; }
    public string? Remarks { get; set; }
    public decimal? OpeningBalance { get; set; }
    public decimal? Deduction { get; set; }
    public decimal? ClosingBalance { get; set; }
    public int? EmployeeId { get; set; }

    public DateTime? RepaymentDate { get; set; }


    public int? RepaymentMonth { get; set; }

    public int? RepaymentYear { get; set; }
    public int? PendingInstallment { get; set; }

    public int? Status { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }

  
}
