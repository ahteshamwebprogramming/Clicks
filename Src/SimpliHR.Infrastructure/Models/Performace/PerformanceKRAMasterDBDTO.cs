using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PerformanceKRAMasterDBDTO
{
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
