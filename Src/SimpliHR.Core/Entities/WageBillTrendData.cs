using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("WageBillTrendData")]
public class WageBillTrendData
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int WageBillTrendDataId { get; set; }

    public int UnitId { get; set; }

    public int? EmployeeCount { get; set; }

    public double? WageBill { get; set; }

    public double? AverageWageBill { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }
}
