using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class EmployeeKeyValues
{
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeNameCode { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmailId { get; set; }
    public int? DepartmentId { get; set; }
    public int? UnitId { get; set; }
    public int? ManagerId { get; set; }
    public int? HODId { get; set; }
    public DateTime? DOJ { get; set; }
    public DateTime? ExitDate { get; set; }
    public string? EmployeeDeparment { get; set; }

}
