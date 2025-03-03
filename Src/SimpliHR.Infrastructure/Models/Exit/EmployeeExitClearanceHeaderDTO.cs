using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Exit;
public partial class EmployeeExitClearanceHeaderDTO
{
    public int EmployeeClearanceHeaderId { get; set; }
    public int? ClearanceMappingId { get; set; } 
    public int? OwnerId { get; set; }
    public string OwnerName { get; set; }
    public int? EmployeeId { get; set; }
    public int? UnitId { get; set; }
    public int? DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int? JobTitleId { get; set; }
    public string JobTitleName { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool? ClearanceStatus { get; set; }
    public string FinalRemark { get; set; }
    public bool? IsActive { get; set; }
}
