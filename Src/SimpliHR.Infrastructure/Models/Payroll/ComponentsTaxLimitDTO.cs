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

public partial class ComponentsTaxLimitDTO
{
    public int TaxLimitId { get; set; }
    public decimal? GratuityLimit { get; set; }
    public decimal? LeaveEncashmentLimit { get; set; }
    public decimal? PFLimit { get; set; }
    public int? CreatedBy { get; set; }
     public DateTime? CreatedOn { get; set; }
    public int? ModifiedBy { get; set; }   
    public DateTime? ModifiedOn { get; set; }
    public int? UnitId { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}

