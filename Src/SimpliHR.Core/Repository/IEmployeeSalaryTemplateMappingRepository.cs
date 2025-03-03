using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Payroll;

namespace SimpliHR.Core.Repository;


public interface IEmployeeSalaryTemplateMappingRepository : IDapperRepository<EmployeeSalaryTemplateMapping>
{
    public Task<string> SaveEmployeeSalaryTemplateMapping(EmployeeSalaryTemplateMapping employeeSalaryTemplateMapping, string MappingEmployeeIds);
    public Task<string> DeleteEmployeeSalaryTemplateMappingInfo(int employeeSalaryTemplateId);
}

