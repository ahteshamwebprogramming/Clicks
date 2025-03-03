using Dapper.Contrib.Extensions;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PayrollReimbursementComponent")]

public partial class PayrollReimbursementComponent
{
    [Key] // not [ExplicitKey]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReimbursementId { get; set; }

    public int? SalaryComponentId { get; set; }

    public int? UnitId { get; set; }

    public string? NameInPaySlip { get; set; }

    public bool? IsFlexibleBenifitPlan { get; set; }

    public decimal? AmountValue { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
}
