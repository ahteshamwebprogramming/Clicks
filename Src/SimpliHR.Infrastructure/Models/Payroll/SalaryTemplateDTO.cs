using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Payroll;

public class SalaryTemplateDTOForSave
{
    public int[] SalaryComponentIds { get; set; }

    public int? SalaryTemplateId { get; set; }

    public string? TemplateName { get; set; }
    public string? Description { get; set; }
    public double? AnnualCTC { get; set; }

    public List<SalaryComponentMasterDTO> SalaryComponentMasterListDTO { get; set; }

    public List<PayrollEarningComponentDTO> PayrollEarningComponentList { get; set; }
    public List<PayrollDeductionComponentDTO> PayrollDeductionComponentList { get; set; }
    public List<PayrollReimbursementComponentDTO> PayrollReimbursementComponentList { get; set; }
    public int? UnitId { get; set; }
}
public class SalaryTemplateDTO
{
    public int SalaryTemplateId { get; set; }

    public string EncryptedId { get; set; }
    public int? UnitId { get; set; }

    public string? TemplateName { get; set; }

    public string? Description { get; set; }

    public double? AnnualCtc { get; set; }

    public double? CostToCompanyMonthly { get; set; }

    public double? CostToCompanyAnnully { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public bool? IsActive { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public string DisplayMessage { get; set; } = string.Empty;
    public HttpResponseMessage? HttpMessage { get; set; }
}
public class SalaryTemplateComponentsMappingDTO
{
    public int SalaryTemplateComponentsMappingId { get; set; }

    public int? SalaryTemplateId { get; set; }

    public int? SalaryComponentId { get; set; }
    public bool? IsActive { get; set; }
    public int? UnitId { get; set; }
    public string? SalaryComponentType { get; set; }
    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }
}

public class EmployeesSalaryDetailsDTO
{
    public int EmployeeSalaryId { get; set; }

    public int? EmployeeId { get; set; }

    public int? SalaryComponentId { get; set; }
    public string? ComponentName { get; set; }
    public int? UnitId { get; set; }
    public decimal? PerVal { get; set; }
    public decimal? AmtPerMonth { get; set; }
    public decimal? AmtPerYear { get; set; }
    public int? SalaryMonth { get; set; }
    public int? SalaryYear { get; set; }
    public int? Employee { get; set; }
    public string? SalaryComponentType { get; set; }
    public string? ProcessBy { get; set; }
    public string? CalculationType { get; set; }
    public string? TemplateType { get; set; }
    public bool? IsTaxableIncome { get; set; }
    public bool? IsEpfConsidration { get; set; }
    public bool? IsEsiConsidrable { get; set; }
    public bool IsSent { get; set; }
    public DateTime? Processdate { get; set; }
    public string? WEF { get; set; }
    public bool IsCurrent { get; set; }
    public bool? IsVisibleInPaySlip { get; set; }
    public bool IsVariable { get; set; }
    public bool IsArrears { get; set; }
    public decimal? ArrearsAmt { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    // List<Validates> componentValidateList = new List<Validates>();
    public List<ComponentsValidates> ComponentValidateList = new List<ComponentsValidates>();
    public List<SalaryValidates> SalaryValidateList = new List<SalaryValidates>();
    public List<EmployeesSalaryDetailsDTO> SalaryDetails = new List<EmployeesSalaryDetailsDTO>();
    public ICollection<EmployeeMasterDTO> EmployeeMasterList { get; set; } = new List<EmployeeMasterDTO>();
    public EmployeeMastersKeyValues? EmployeeMastersKeyValues { get; set; }

    public List<EmpSalaryComponents> EmpSalaryComponentsList = new List<EmpSalaryComponents>();
    public List<SalaryMonths> SalMonths = new List<SalaryMonths>();
    public List<SalaryYears> SalYears = new List<SalaryYears>();
    public List<int> Years = new List<int>();
}

public class ComponentsValidates
{
    public string? ComponentName { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? UnitId { get; set; }
    public int? SalaryComponentId { get; set; }
}

public class SalaryValidates
{
    public double? TemplateSalValue { get; set; }
    public double? EmployeeSalValue { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeCode { get; set; }
    public int? UnitId { get; set; }
    public int? EmployeeId { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}

public class EmpSalaryComponents
{
    public decimal? FixedSalary { get; set; }
    public decimal? ActualSalary { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeCode { get; set; }
    public int? DepartmentId { get; set; }
    public int? EmployeeId { get; set; }
    public double? WorkingDays { get; set; }
    public double? DaysOfMonth { get; set; }
    public double? ActualworkingDays { get; set; }
    public string? Comments { get; set; }
    public int? SalaryMonth { get; set; }
    public int? SalaryYear { get; set; }
    public int? Employee { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}

public class SalaryParameters
{
    public string? EmployeeIds { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public string? ActualDays { get; set; }
    public string? PayrollRemarks { get; set; }
    public int? ProcessMonth { get; set; }
    public int? ProcessYear { get; set; }
    public string? NoOfDays { get; set; }
    public string? FixedSalary { get; set; }
    public string? PayOutSalary { get; set; }
    public int? UnitId { get; set; }
}

