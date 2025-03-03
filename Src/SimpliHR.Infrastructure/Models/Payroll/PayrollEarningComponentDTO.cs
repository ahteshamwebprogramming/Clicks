using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class PayrollEarningComponentDTO
{
    public int EarningId { get; set; }

    public string? EncryptedId { get; set; }
    public int SalaryComponentId { get; set; }

    public string SalaryComponentTitle { get; set; }

    public int? UnitId { get; set; }

    public string UnitName { get; set; }

    public string EarningName { get; set; } = "";

    public string NameInPaySlip { get; set; }

    public string CalculationType { get; set; }

    public string? EPFConsidrationType { get; set; }

    public decimal? Percentage { get; set; }

    public bool? IsTaxableIncome { get; set; } = false;

    public bool? IsSalaryPart { get; set; } = false;

    public bool? IsProRataCalculation { get; set; } = false;

    public bool? IsEpfConsidration { get; set; } = false;

    public string EpfConsidrationType { get; set; }

    public bool? IsEsiConsidrable { get; set; } = false;

    public bool? IsVisibleInPaySlip { get; set; }=false;

    public string CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; } = false;

    public bool? IsBasic { get; set; } = false;
}


public partial class PayrollComponentViewModel
{
    public List<SalaryComponentKeyValues> SalaryComponentKeyValues { get; set; }
    public PayrollEarningComponentDTO PayrollEarningComponent { get; set; } = new PayrollEarningComponentDTO();
    public List<PayrollEarningComponentDTO> PayrollEarningComponentList { get; set; }

    public PayrollDeductionComponentDTO PayrollDeductionComponent { get; set; } = new PayrollDeductionComponentDTO();
    public List<PayrollDeductionComponentDTO> PayrollDeductionComponentList { get; set; }

    public PayrollReimbursementComponentDTO PayrollReimbursementComponent { get; set; } = new PayrollReimbursementComponentDTO();
    public List<PayrollReimbursementComponentDTO> PayrollReimbursementComponentList { get; set; }

    public string DisplayMessage { get; set; } = "_Blank";

}
