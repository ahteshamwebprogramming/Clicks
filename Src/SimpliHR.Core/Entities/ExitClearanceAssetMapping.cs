using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("ExitClearanceAssetMapping")]
public partial class ExitClearanceAssetMapping
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClearanceAssetMapId { get; set; }

    public int? ClearanceMappingId { get; set; }

    public int? AssetId { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public int? UnitId { get; set; }

    public bool? IsActive { get; set; }
}
