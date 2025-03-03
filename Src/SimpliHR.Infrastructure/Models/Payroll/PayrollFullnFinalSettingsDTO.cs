using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class PayrollFullnFinalSettingsDTO
{
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
    public HttpResponseMessage? HttpMessage { get; set; }
   public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
}

