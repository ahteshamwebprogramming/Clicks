using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class PMSReportListViewModel
{
    public List<PMSReportViewModel>? PMSReportList { get; set; }
    public List<PerformanceSettingDTO>? PerformanceSettingList { get; set; }
}
public class PMSReportViewModel
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
    public string? PMSStatus { get; set; }
    public string? KRAWeightage { get; set; }
    public string? KRARating { get; set; }
    public string? FinalKRARating { get; set; }
    public string? BehaviouralWeightage { get; set; }
    public string? BehaviouralRating { get; set; }
    public string? FinalBehaviouralRating { get; set; }
    public string? FinalScore { get; set; }
    public string? FinalRatingManager { get; set; }
    public string? FinalRatingHOD { get; set; }
    public string? ClosingRemarksEmployee { get; set; }
    public string? ClosingRemarksManager { get; set; }
    public string? ClosingRemarksHOD { get; set; }
}
