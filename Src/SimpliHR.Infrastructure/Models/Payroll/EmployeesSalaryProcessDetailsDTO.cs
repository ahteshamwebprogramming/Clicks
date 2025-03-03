using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System.Data;


namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class EmployeesSalaryProcessDetailsDTO
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
}
public partial class SalaryMonths
{
    public int ID { get; set; }
    public string? Name { get; set; }
}
public partial class SalaryYears
{
    public int ID { get; set; }
    public string? Name { get; set; }
}
public partial class SalaryProcessInputs
{
    public string? FY { get; set; }

    public int? EmployeeId { get; set; }

    public int? UnitId { get; set; }

    public int? Month { get; set; }

    public string? MonthName { get; set; }

    public int? Year { get; set; }

    public int? Employee { get; set; }

    public string Base64ProfileImage { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
    public ICollection<EmployeeMasterDTO> EmployeeMasterList { get; set; } = new List<EmployeeMasterDTO>();
    public List<SalaryMonths> SalMonths  = new List<SalaryMonths>();
    public List<SalaryYears> SalYears = new List<SalaryYears>();
    public List<int> Years = new List<int>();
    public ICollection<EmployeeSalarySummaryDTO> objResultData { get; set; } = new List<EmployeeSalarySummaryDTO>();
    public ICollection<PaySlipDetailsDTO> objPaySlip { get; set; } = new List<PaySlipDetailsDTO>();
    public ICollection<PaySlipComponentsDTO> objPaySlipComponent { get; set; } = new List<PaySlipComponentsDTO>();
    public EmployeeMastersKeyValues? EmployeeMastersKeyValues { get; set; }
    public ICollection<DepartmentKeyValues>? DepartmentKeyValues { get; set; } 
    public List<LoanPaymentDetailDTO>? GetLoanPaymentDetails { get; set; }
    public List<LoanMasterDTO>? GetSanctionLoan { get; set; }
    public DataSet objDataSet = new DataSet();
    public DataSet objBTRDSet = new DataSet();
    public DataSet objESIDSet = new DataSet();
    public DataSet objLWDSet = new DataSet();
    public DataSet objPTDSet = new DataSet();
    public DataSet objITDSet = new DataSet();

    public Form16DocDTO Form16Doc { get; set; } = new Form16DocDTO();
    public List<Form16DocDTO> Form16DocList { get; set; } = new List<Form16DocDTO>();

    public List<EmployeeKeyValues> EmployeeKeyValues { get; set; }

}

public partial class SalarySummery
{
    public string? FY { get; set; }

    public int? EmployeeId { get; set; }

    public int? UnitId { get; set; }

    public decimal? FixedSalary { get; set; }
    public decimal? PayOutSalary { get; set; }
    public decimal? TotalTax { get; set; }
    public decimal? Balance { get; set; }
    public double? NoOfDays { get; set; }
    public double? NoOfLeave { get; set; }
    public int? ProcessMonth { get; set; }
    public int? ProcessYear { get; set; }


}


public partial class EmployeeSalarySummaries
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
    public string? Remarks { get; set; }
    public string DisplayMessage { get; set; } = "_blank";


}

public partial class EmployeeLeaveBalanceInputs
{
    public int? UnitId { get; set; }
    public int? EmployeeId { get; set; }
    public int? LeaveTypeId { get; set; }
    public int? LocationId { get; set; }
    public int? DepartmentId { get; set; }
    public int? ToYear { get; set; }
    public int? ToMonth { get; set; }
    public int? FromYear { get; set; }
    public int? FromMonth { get; set; }

    public List<SalaryMonths> SalMonths = new List<SalaryMonths>();   
    public List<int> Years = new List<int>();
    public ICollection<DepartmentKeyValues>? DepartmentKeyValues { get; set; }
    public ICollection<WorkLocationKeyValues>? WorkLocationKeyValues { get; set; }

    public List<EmployeeLeaveBalanceReportDTO>? GetEmployeeLeaveBalanceReport { get; set; }


}