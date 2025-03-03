using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

[Dapper.Contrib.Extensions.Table("PerformanceKRAMasterDB")]
public class PerformanceKRAMasterDB
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PerformanceKRAMasterDBId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string KRA { get; set; } = null!;

    public double Weightage { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }
    public int UnitId { get; set; }
    public string? Source { get; set; }
    public int PerformanceSettingId { get; set; }
}
