using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PerformanceSettingViewModel
{
    public string ViewType { get; set; }

    public List<BandKeyValues>? Bands { get; set; }
    public PerformanceSettingDTO? PerformanceSetting { get; set; }
    public List<PerformanceSettingWithChildEntity>? PerformanceSettingWithChildEntityList { get; set; }

    public List<PerformanceSettingMechanismDTO>? PerformanceSettingMechanismList { get; set; }
    public List<PerformanceSettingSkillSetMatrixDTO>? PerformanceSettingSkillSetMatrixList { get; set; }

    public PerformanceSettingSkillSetMatrixDTO? PerformanceSettingSkillSetMatrix { get; set; }
    public int? PerformanceSettingId { get; set; }
    public string? encPerformanceSettingId { get; set; }

    public int EmployeeId { get; set; }
    public string? EmployeeCode { get; set; }
    public List<PerformanceKRAMasterDBDTO>? PerformanceKRAMasterDBList { get; set; }
    public List<PerformanceKRAMasterDBDTO>? PerformanceBehavioralMasterDBList { get; set; }

    public List<PerformanceEmployeeKRADataViewModel>? PerformanceEmployeeKRAList { get; set; }
    public List<PerformanceEmployeeKRADataViewModel>? PerformanceEmployeeBehavioralList { get; set; }

    public List<PerformanceEmployeeKRADataViewModel>? PerformanceEmployeeKRADataVMList { get; set; }

    public PerformanceEmployeeDataDTO? PerformanceEmployeeData { get; set; }

    public List<PerformanceEmployeeTrainingDataDTO>? PerformanceEmployeeTrainingDataList { get; set; }
    public List<PageKeyValues>? PageKeyValueTrainingTypeList { get; set; }
    public List<PageKeyValues>? PageKeyValueTrainingUrgencyList { get; set; }
}
public class PerformanceEmployeeKRADataViewModel
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
    public int? EmployeeRating { get; set; }

    public string? EmployeeRemarks { get; set; }

    public int? ManagerRating { get; set; }

    public string? ManagerRemarks { get; set; }

    public double? WAScore { get; set; }


}