using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("EmployeeExitClearanceHeader")]
public partial class EmployeeExitClearanceDetail
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeClearanceDetailId { get; set; }
    public int? EmployeeClearanceHeaderId { get; set; }   
    public int? AssetId { get; set; }
    public int? ClearanceMappingId { get; set; }
    public string? AssetClearanceStatus { get; set; }
    public string? Remark { get; set; }
    public bool? RecoveryStatus { get; set; }
    public decimal? RecoveryAmount { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool? IsActive { get; set; }
}
