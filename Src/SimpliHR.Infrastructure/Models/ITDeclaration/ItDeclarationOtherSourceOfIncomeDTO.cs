using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ITDeclaration;

public class ItDeclarationOtherSourceOfIncomeDTO
{
    public int ItDeclarationOtherSourceOfIncomeId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public int? UnitId { get; set; }

    public double? IncomeFromOtherSource { get; set; }

    public double? InterestEarnedFromSavingsDeposit { get; set; }

    public double? InterestEarnedFromFixedDeposit { get; set; }

    public double? InterestEarnedFromNationalSavingsCertificates { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }
    public string? ProofFileName { get; set; }

    public string? ProofFileExtension { get; set; }

    public byte[]? ProofFile { get; set; }
    public string? FY { get; set; }
    public string? Regime { get; set; }
}
