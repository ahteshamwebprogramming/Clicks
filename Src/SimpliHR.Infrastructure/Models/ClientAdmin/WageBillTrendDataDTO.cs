using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ClientAdmin;

public class WageBillTrendDataDTO
{
    public int WageBillTrendDataId { get; set; }

    public int UnitId { get; set; }

    public int? EmployeeCount { get; set; }

    public double? WageBill { get; set; }

    public double? AverageWageBill { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }
}
