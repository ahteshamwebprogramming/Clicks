using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PerformanceEmployeeDataViewModel
{
    public PerformanceEmployeeDataDTO? performanceEmployeeDataDTO { get; set; }
    public List<PerformanceEmployeeKRADataDTO>? PerformanceEmployeeKRADatas { get; set; }
    public List<PerformanceEmployeeTrainingDataDTO>? PerformanceEmployeeTrainingDatas { get; set; }
    public int? LoggedInUserId { get; set; }
    public string? ButtonType { get; set; }
}
