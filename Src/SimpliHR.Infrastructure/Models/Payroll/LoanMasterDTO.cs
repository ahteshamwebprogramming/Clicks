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

public partial class LoanMasterDTO
{
    public int SanctionId { get; set; }
    public string EncryptedId { get; set; }
    public int? EmployeeId { get; set; }
    public string? Employee { get; set; }
    public string? Month { get; set; }
    public string? StopMonth { get; set; }
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
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<LoanMasterDTO>? GetSanctionLoan { get; set; }
    public List<LoanPaymentDetailDTO>? GetLoanPaymentDetails { get; set; }
    public List<SalaryMonths> SalMonths = new List<SalaryMonths>();   
    public List<int> Years = new List<int>();
    public ICollection<EmployeeMasterDTO> EmployeeMasterList { get; set; } = new List<EmployeeMasterDTO>();

    // [ValidateNever]
    //public EmployeeMasterDTO Employees { get; set; } = null!;

    public string DisplayMessage { get; set; } = "_blank";
}

