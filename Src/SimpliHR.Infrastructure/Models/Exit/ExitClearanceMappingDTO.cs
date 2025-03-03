using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Exit;
public partial class ExitClearanceMappingDTO
{
    public int ClearanceMappingId { get; set; }

    public int? PrimaryClearancePerson { get; set; }

    public int? SecondaryClearancePerson { get; set; }

    public int? DepartmentId { get; set; }

    public int? UnitId { get; set; }

    public string PrimaryClearancePersonName { get; set; }

    public string SecondaryClearancePersonName { get; set; }

    public string DepartmentName { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
}
