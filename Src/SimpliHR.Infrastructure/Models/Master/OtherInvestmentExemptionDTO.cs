using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Master;

public class OtherInvestmentExemptionDTO
{
    public int OtherInvestmentExemptionId { get; set; }

    public string? OtherInvestmentExemption1 { get; set; }

    public string? Optgroup { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }
}
