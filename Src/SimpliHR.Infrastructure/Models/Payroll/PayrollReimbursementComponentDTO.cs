namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class PayrollReimbursementComponentDTO
{
    public int ReimbursementId { get; set; }
    public string? EncryptedId { get; set; }
    public int SalaryComponentId { get; set; }
    public string SalaryComponentTitle { get; set; }
    public int? UnitId { get; set; }
    public string UnitName { get; set; }

    public string? NameInPaySlip { get; set; }

    public bool? IsFlexibleBenifitPlan { get; set; }

    public decimal? AmountValue { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
}
