using SimpliHR.Infrastructure.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ITDeclaration;

public class ItDeclaration80DexemptionDTO
{
    public int ItDeclaration80DexemptionsId { get; set; }

    public int? ItDeclaration80D { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public int? UnitId { get; set; }

    public int? ExemptionType { get; set; }

    public double? Value { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }
    public List<Exemptions80DDTO> exemptions80DDTOs { get; set; }
    public string? ProofFileName { get; set; }

    public string? ProofFileExtension { get; set; }

    public byte[]? ProofFile { get; set; }
    public string? FY { get; set; }
    public string? Regime { get; set; }
}
