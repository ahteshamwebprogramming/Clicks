using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public class SalaryTemplateComponentsMapping
{
    public int SalaryTemplateComponentsMappingId { get; set; }

    public int? SalaryTemplateId { get; set; }

    public int? SalaryComponentId { get; set; }
 
    public int? UnitId { get; set; }
    public string? SalaryComponentType { get; set; }
    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }
}
