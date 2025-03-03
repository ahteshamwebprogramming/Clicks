using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.KeyValue;

public class EmployeeMastersKeyValues
{
    public ICollection<AcademicKeyValues> AcademicKeyValues { get; set; } = new List<AcademicKeyValues>();
    public ICollection<BandKeyValues> BandKeyValues { get; set; } = new List<BandKeyValues>();
    public ICollection<BankKeyValues> BankKeyValues { get; set; } = new List<BankKeyValues>();
    public ICollection<BloodGroupKeyValues> BloodGroupKeyValues { get; set; } = new List<BloodGroupKeyValues>();
    public ICollection<CountryKeyValues> CountryKeyValues { get; set; } = new List<CountryKeyValues>();
    public ICollection<DepartmentKeyValues> DepartmentKeyValues { get; set; } = new List<DepartmentKeyValues>();
    public ICollection<IdtypeKeyValues> IdtypeKeyValues { get; set; } = new List<IdtypeKeyValues>();
    public ICollection<JobTitleKeyValues> JobTitleKeyValues { get; set; } = new List<JobTitleKeyValues>();
    public ICollection<LeaveTypeKeyValues> LeaveTypeKeyValues { get; set; } = new List<LeaveTypeKeyValues>();
    public ICollection<MaritalStatusKeyValues> MaritalStatusKeyValues { get; set; } = new List<MaritalStatusKeyValues>();
    public ICollection<EmployeeKeyValues> EmployeeKeyValues { get; set; } = new List<EmployeeKeyValues>();

    public ICollection<EmployeeKeyValues> HODKeyValues { get; set; } = new List<EmployeeKeyValues>();
    public ICollection<ReligionKeyValues> ReligionKeyValues { get; set; } = new List<ReligionKeyValues>();

    public ICollection<ResourceKeyValues> ResourceKeyValues { get; set; } = new List<ResourceKeyValues>();

    public ICollection<RoleKeyValues> RoleKeyValues { get; set; } = new List<RoleKeyValues>();

    public ICollection<SalaryComponentKeyValues> SalaryKeyValues { get; set; } = new List<SalaryComponentKeyValues>();

    public ICollection<ShiftKeyValues> ShiftKeyValues { get; set; } = new List<ShiftKeyValues>();
    public ICollection<WorkLocationKeyValues> WorkLocationKeyValues { get; set; } = new List<WorkLocationKeyValues>();

    public ICollection<ClientKeyValues> ClientKeyValues { get; set; } = new List<ClientKeyValues>();

    public ICollection<SBUKeyValues> SBUKeyValues { get; set; } = new List<SBUKeyValues>();

    public ICollection<EmployeeValidationKeyValues> EmployeeValidations { get; set; } = new List<EmployeeValidationKeyValues>();

    public ICollection<LanguageKeyValues> LanguageKeyValue { get; set; } = new List<LanguageKeyValues>();

    public List<int> GetYears(int startYear, int noOfYears)
    {
        return Enumerable.Range(startYear, noOfYears).Reverse().ToList();
    }

}
