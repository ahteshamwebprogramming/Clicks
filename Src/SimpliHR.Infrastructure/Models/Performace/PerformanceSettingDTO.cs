using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PerformanceSettingDTO
{
    public int PerformanceSettingId { get; set; }

    public int UnitId { get; set; }

    public DateTime ReviewPeriodFrom { get; set; }

    public DateTime ReviewPeriodTo { get; set; }

    public int AssesmentPeriodicity { get; set; }

    public int RollOut { get; set; }

    public int Mechanism { get; set; }

    public bool SettingByManager { get; set; }

    public bool AcceptanceofGoalsByEmployee { get; set; }

    public bool EmployeeRating { get; set; }

    public bool EmployeeRemarks { get; set; }

    public bool EmployeeClosingRemarks { get; set; }

    public bool ManagerRating { get; set; }

    public bool ManagerRemarks { get; set; }

    public bool ManagerClosingRemarks { get; set; }

    public bool TNRByManager { get; set; }

    public bool HODReview { get; set; }

    public bool HODClosingRemarks { get; set; }
    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }
}
