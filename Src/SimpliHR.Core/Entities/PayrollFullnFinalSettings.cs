using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Core.Entities;

[Table("PayrollFullnFinalSettings")]
public partial class PayrollFullnFinalSettings
{
    [Key]
    public int SettingId { get; set; }
    public int? LeavePayableId { get; set; }

    public int? LeaveRecoveryId { get; set; }

    public int? NoticePeriodPayableId { get; set; }

    public int? LeaveEncashmentId { get; set; }

    public int? NoticePeriodRecoveryId { get; set; }

    public int? CompoffApplicabilityId { get; set; }

    public int? GratuityApplicabilityId { get; set; }

    public int? OTApplicabilityId { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? UnitId { get; set; }
    //public virtual ICollection<EmployeePersonalDetail> EmployeePersonalDetails { get; set; } = new List<EmployeePersonalDetail>();
}
