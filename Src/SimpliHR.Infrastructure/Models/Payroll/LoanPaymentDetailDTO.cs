using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class LoanPaymentDetailDTO
{
    public int RepaymentId { get; set; }
    public string EncryptedId { get; set; }

    public string Remarks { get; set; }
    public int? SanctionId { get; set; }

    public decimal? OpeningBalance { get; set; }
    public decimal? Deduction { get; set; }
    public decimal? ClosingBalance { get; set; }
    public int? EmployeeId { get; set; }

    public DateTime? RepaymentDate { get; set; }


    public int? RepaymentMonth { get; set; }

    public string? Month { get; set; }
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

    //public List<LoanPaymentDetailDTO>? GetLoanPaymentDetails { get; set; }
    //public List<SalaryMonths> SalMonths = new List<SalaryMonths>();   
    //public List<int> Years = new List<int>();
    public string DisplayMessage { get; set; } = "_blank";
}


public class RepaymentLoanInputs
{
   
    public int? RepaymetId { get; set; }
    public string? Remarks { get; set; }
    public decimal? Deduction { get; set; }

    [StringLength(50)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
}
