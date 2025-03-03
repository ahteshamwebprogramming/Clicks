using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProjectTracker;

public class PTEmployees
{
    public int EmployeeId { get; set; }
    public string? EnycEmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string? EmployeeCode { get; set; }
    public string? Department { get; set; }
}
