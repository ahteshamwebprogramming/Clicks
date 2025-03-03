using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public class ItDeclarationType
{
    public int ItDeclarationTypeId { get; set; }

    public int EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public int? UnitId { get; set; }

    public string? RegimeType { get; set; }

    public string? Fy { get; set; }

    public int? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
