using Dapper.Contrib.Extensions;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PayrollEarningComponent")]

public partial class PayrollEarningComponent
{
    [Key] // not [ExplicitKey]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EarningId { get; set; }

    public int? SalaryComponentId { get; set; }

    public int? UnitId { get; set; }

    public string? EarningName { get; set; }

    public string? NameInPaySlip { get; set; }

    public string? CalculationType { get; set; }

    public string? EPFConsidrationType { get; set; }
    
    public decimal? Percentage { get; set; }

    public bool? IsTaxableIncome { get; set; }

    public bool? IsProRataCalculation { get; set; }

    public bool? IsEpfConsidration { get; set; }

    public bool? IsEsiconsidrable { get; set; }

    public bool? IsVisibleInPaySlip { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
}
