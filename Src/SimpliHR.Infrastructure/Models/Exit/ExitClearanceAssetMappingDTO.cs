using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Exit;
public partial class ExitClearanceAssetMappingDTO
{
    public int? ClearanceAssetMapId { get; set; }

    public int? ClearanceMappingId { get; set; }

    public int? AssetId { get; set; }
    public string? ResourceName { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsChecked { get; set;}
    public int? UnitId { get; set;}
}
