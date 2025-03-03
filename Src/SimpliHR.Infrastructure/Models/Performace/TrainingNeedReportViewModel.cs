using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class TrainingNeedReportListViewModel
{
    public List<TrainingNeedReportViewModel>? TrainingNeedReportList { get; set; }
    public List<PerformanceSettingDTO>? PerformanceSettingList { get; set; }
}
public class TrainingNeedReportViewModel
{
    public int? EmployeeId { get; set; }
    public int? PerformanceEmployeeDataId { get; set; }
    public int? PerformanceSettingId { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
    public string? Designation { get; set; }
    public string? Department { get; set; }
    public string? Function { get; set; }
    public string? WorkLocation { get; set; }
    public string? Manager { get; set; }
    public string? HOD { get; set; }
    public string? TrainingNeed { get; set; }
    public string? TrainingType { get; set; }
    public string? Urgency { get; set; }

}
