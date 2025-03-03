using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PerformanceSettingSkillSetMatrixDTO
{
    public int PerformanceSettingSkillSetMatrixId { get; set; }

    public int PerformanceSettingId { get; set; }

    public int BandId { get; set; }

    public double KRAWeightage { get; set; }

    public double SoftSkillsWeightage { get; set; }
}
