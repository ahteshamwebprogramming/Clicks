using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PerformanceEmployeeKRADataDTO
{
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
