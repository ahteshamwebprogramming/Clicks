using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class EmployeeSalarySummaryDTO
{
    public int ID { get; set; }

    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }

    public string? EmployeeCode { get; set; }

    public int? UnitId { get; set; }

    public int? DepartmentId { get; set; }

    public decimal? FixedSalary { get; set; }

    public decimal? PayOutSalary { get; set; }

    public decimal? TotalTax { get; set; }

    public decimal? Balance { get; set; }

    public int? ProcessMonth { get; set; }

    public int? ProcessYear { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? CreatedBy { get; set; }

    public double? NoOfDays { get; set; }

    public double? NoOfLeave { get; set; }
    public double? PayOutDays { get; set; }
    public bool? IsProcessed { get; set; }
    public bool? IsFreezed { get; set; }
    public string? Remarks { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
}

