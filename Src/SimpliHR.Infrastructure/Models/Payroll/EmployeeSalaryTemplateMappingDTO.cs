using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System;
using System.Collections.Generic;

namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class EmployeeSalaryTemplateMappingDTO
{
    public int EmployeeSalaryTemplateId { get; set; }

    public string EncryptedEmployeeSalaryTemplateId { get; set; }

    public string? MappingName { get; set; }

    public int? SalaryTemplateId { get; set; }

    public string? TemplateName { get; set; }

    public int? EmployeesSelection { get; set; }

    public string MappingEmployeeIds { get; set; }

    public int? DepartmentId { get; set; }
    public string DepartmentName { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsActive { get; set; }

    public List<EmployeeSalaryTemplateDetailDTO> EmployeeSalaryTemplateDetailList { get; set; }
}

public class EmployeeSalaryTemplateMappingViewModel
{
    public string ViewScreen { get; set; } = "list";
    public int? UnitId { get; set; }
    public string DisplayMessage { get;set; } = "_blank";
    public List<DepartmentKeyValues> DepartmentKeyValues { get; set; }
    public List<EmployeeKeyValues> EmployeeKeyValues { get; set; }
    public List<SalaryTemplateKeyValues> SalaryTemplateKeyValues { get; set; }

    public EmployeeSalaryTemplateMappingDTO EmployeeSalaryTemplateMapping { get; set; } = new EmployeeSalaryTemplateMappingDTO();
    public List<EmployeeSalaryTemplateMappingDTO> EmployeeSalaryTemplateMappingList { get; set; }

    public EmployeeSalaryTemplateDetailDTO EmployeeSalaryTemplateDetail { get; set; }
    public List<EmployeeSalaryTemplateDetailDTO> EmployeeSalaryTemplateDetailList { get; set; }

}
