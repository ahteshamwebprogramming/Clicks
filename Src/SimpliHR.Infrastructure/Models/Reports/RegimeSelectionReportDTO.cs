using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class RegimeSelectionReportDTO
{
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
    public string? SelectedRegime { get; set; }
    public string? RegimeType { get; set; }
    public string? CreatedDate { get; set; }
    public List<RegimeSelectionReportDTO>? GetRegimeSelectionReport { get; set; }

}

