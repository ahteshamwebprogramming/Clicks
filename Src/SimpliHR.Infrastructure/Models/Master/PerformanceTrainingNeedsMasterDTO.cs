using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Master;

public class PerformanceTrainingNeedsMasterDTO
{
    public int TrainingNeedsMasterId { get; set; }

    public string? Training { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
    public int UnitId { get; set; }
    public int PerformanceSettingId { get; set; }
}
