using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("PerformanceEmployeeTrainingData")]
public class PerformanceEmployeeTrainingData
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PerformanceEmployeeTrainingDataId { get; set; }

    public int TrainingNeedsMasterId { get; set; }

    public int PerformanceEmployeeDataId { get; set; }

    public int? TrainingType { get; set; }

    public int? TrainingUrgency { get; set; }
}
