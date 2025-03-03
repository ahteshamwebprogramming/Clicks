using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public class ItDeclarationHouseRentDetail
{
    public int ItDeclarationHouseRentDetailsId { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public int? UnitId { get; set; }

    public string? MonthFrom { get; set; }
    public string? YearFrom { get; set; }
    public string? MonthTo { get; set; }
    public string? YearTo { get; set; }

    public double? Amount { get; set; }

    public string? HouseAddress { get; set; }

    public string? StayingIn { get; set; }

    public string? LandlordName { get; set; }

    public string? LandlordPancard { get; set; }

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
