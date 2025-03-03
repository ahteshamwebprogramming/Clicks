using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public class ItDeclarationPreviousEmployement
{
    public int ItDeclarationPreviousEmployementId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public int? UnitId { get; set; }

    public double? IncomeAfterExemptions { get; set; }

    public double? IncomeTax { get; set; }

    public double? ProfessionalTax { get; set; }

    public double? EmployeeProvidentFund { get; set; }

    public double? LeaveEncashmentExemptions { get; set; }

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
