using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("PerformanceSettingMechanism")]
public class PerformanceSettingMechanism
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PerformanceSettingMechanismId { get; set; }

    public int PerformanceSettingId { get; set; }

    public int Point { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }

    public double ScoreFrom { get; set; }

    public double ScoreTo { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }
}
