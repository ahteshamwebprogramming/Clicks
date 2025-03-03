using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Exit;
public partial class EmployeeExitClearanceDetailDTO
{
   public int? EmployeeClearanceDetailId { get; set; }

    public int? EmployeeClearanceHeaderId { get; set; }

    public int? ClearanceAssetMapId { get; set; }

    public int? DepartmentId { get; set; }

    public string AssetClearanceStatus { get; set; }

    public int? ClearanceMappingId { get; set; }

    public int? AssetId { get; set; }
    public string AssetName { get; set; }
    public int? OwnerId { get; set; }
    public int? EmployeeId { get; set; }

    public string? Remark { get; set; }

    public bool? RecoveryStatus { get; set; }
    public decimal? RecoveryAmount { get; set; }

   // public DateTime? RecoveryDate { get; set; }

    public string? CreatedOn { get; set; }

    public string? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
}
