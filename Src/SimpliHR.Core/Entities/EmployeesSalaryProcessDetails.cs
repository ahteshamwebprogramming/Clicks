using SimpliHR.Infrastructure.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities;

public partial class EmployeesSalaryProcessDetails
    {
    public int EmployeesSalaryProcessId { get; set; }

    public int? EmployeeId { get; set; }

    public int? UnitId { get; set; }

    public string? ComponentType { get; set; }

    public string? ComponentName { get; set; }
    public decimal? AmtPerMonth { get; set; }

    public decimal? AmtWithdrawPerMonth { get; set; }

    public int? ProcessMonth { get; set; }

    public int? ProcessYear { get; set; }

    public DateTime? ProcessDate { get; set; }

    public string? ProcessBy { get; set; }

    public bool? IsVisiblePaySlip { get; set; }

    public bool? IsProcessed { get; set; }

    public double? NoOfDays { get; set; }

    public double? NoOfLeave { get; set; }

    public List<SalaryMonths> SalMonths = new List<SalaryMonths>();
    public List<SalaryYears> SalYears = new List<SalaryYears>();

    //public DataSet objDataSet = new DataSet();

}




