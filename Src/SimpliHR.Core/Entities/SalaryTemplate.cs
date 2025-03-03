using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public class SalaryTemplate
{
    public int SalaryTemplateId { get; set; }

    public int? UnitId { get; set; }

    public string? TemplateName { get; set; }

    public string? Description { get; set; }

    public double? AnnualCtc { get; set; }

    public double? CostToCompanyMonthly { get; set; }

    public double? CostToCompanyAnnully { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public bool? IsActive { get; set; }
}
