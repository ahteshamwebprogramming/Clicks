using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PerformanceEmployeeTrainingDataDTO
{
    public int PerformanceEmployeeTrainingDataId { get; set; }

    public int TrainingNeedsMasterId { get; set; }
    public string? Training { get; set; }

    public int PerformanceEmployeeDataId { get; set; }

    public int? TrainingType { get; set; }

    public int? TrainingUrgency { get; set; }
}
