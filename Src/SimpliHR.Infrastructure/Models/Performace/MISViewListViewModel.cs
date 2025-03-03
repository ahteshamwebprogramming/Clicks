using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Performace;

public class MISViewListViewModel
{
    public string? encEmployeeId { get; set; }
    public string? encSource { get; set; }
    public string? Source { get; set; }    
    public List<MISViewList>? MISViewLists { get; set; }
    public List<PerformanceSettingDTO>? PerformanceSettingDTOs { get; set; }
}
public class MISViewList
{
    public int EmployeeId { get; set; }
    public string? encEmployeeId { get; set; }
    public string? encSource { get; set; }
    public string? Source { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
    public string? Designation { get; set; }
    public string? Department { get; set; }
    public string? Function { get; set; }
    public string? WorkLocation { get; set; }
    public string? Manager { get; set; }
    public string? HOD { get; set; }
    public string? PMSStatus { get; set; }
    public int? PerformanceEmployeeDataId { get; set; }
    public int? Published { get; set; }
    public int? PerformanceSettingId { get; set; }
    public string? encPerformanceSettingId { get; set; }

}
