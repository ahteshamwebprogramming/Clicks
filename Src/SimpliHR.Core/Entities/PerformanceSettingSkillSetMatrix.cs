using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;
[Dapper.Contrib.Extensions.Table("PerformanceSettingSkillSetMatrix")]
public class PerformanceSettingSkillSetMatrix
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PerformanceSettingSkillSetMatrixId { get; set; }

    public int PerformanceSettingId { get; set; }

    public int BandId { get; set; }

    public double KRAWeightage { get; set; }

    public double SoftSkillsWeightage { get; set; }
}
