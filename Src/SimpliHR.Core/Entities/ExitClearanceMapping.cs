using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("ExitClearanceMapping")]
public partial class ExitClearanceMapping
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClearanceMappingId { get; set; }

    public int? PrimaryClearancePerson { get; set; }

    public int? SecondaryClearancePerson { get; set; }

    public int? DepartmentId { get; set; }

    public int? UnitId { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
}
