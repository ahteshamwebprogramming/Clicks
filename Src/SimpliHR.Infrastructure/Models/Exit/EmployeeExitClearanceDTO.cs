using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Exit;
public partial class EmployeeExitClearanceDTO
{
    public int EmployeeExitClearanceId { get; set; }
    public int? ClearanceMappingId { get; set; }
    public int? EmployeeId { get; set; }
    public int? UnitId { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime? ClearanceDate { get; set; }

    public int? ClearanceBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public bool? IsActive { get; set; }
}
