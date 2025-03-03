using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("PerformanceEmployeeKRAData")]
public class PerformanceEmployeeKRAData
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PerformanceEmployeeKRADataId { get; set; }

    public int PerformanceEmployeeDataId { get; set; }

    public int? SNo { get; set; }

    public string? KRA { get; set; }

    public double? Weightage { get; set; }

    public int? EmployeeRating { get; set; }

    public string? EmployeeRemarks { get; set; }

    public int? ManagerRating { get; set; }

    public string? ManagerRemarks { get; set; }

    public double? WAScore { get; set; }

    public string? Source { get; set; }
}
