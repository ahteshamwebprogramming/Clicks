using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("EmployeeExitClearanceHeader")]
public partial class EmployeeExitClearanceHeader
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeClearanceHeaderId { get; set; }

    public int? ClearanceMappingId { get; set; }
    
    public int? OwnerId { get; set; }

    public int? EmployeeId { get; set; }

    public int? UnitId { get; set; }

    public string? ClearanceStatus { get; set; }

    public string? FinalRemark { get; set; }

    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
}
