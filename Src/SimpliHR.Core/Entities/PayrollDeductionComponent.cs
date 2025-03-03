using Dapper.Contrib.Extensions;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PayrollDeductionComponent")]

public partial class PayrollDeductionComponent
{
    [Key] // not [ExplicitKey]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DeductionId { get; set; }

    public int? SalaryComponentId { get; set; }

    public int? UnitId { get; set; }

    public string? AssociateWith { get; set; }

    public string? NameInPaySlip { get; set; }

    public bool? IsProRataBasis { get; set; }

    public string CalculationType { get; set; }

    public decimal? Percentage { get; set; }

    public bool? IsTaxableIncome { get; set; } = false;

    public bool? IsEsiConsidrable { get; set; } = false;

    public bool? IsVisibleInPaySlip { get; set; } = false;
    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
}
