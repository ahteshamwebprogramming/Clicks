using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.StatutoryComponent;

public partial class StatutoryComponent_EPFDTO
{
    public int StatutoryComponentsId { get; set; }
    public string StatutoryComponentsIdEnc { get; set; }
    public int UnitId { get; set; }

    public int DepartmentId { get; set; }

    public string DepartmentIds { get; set; }

    public string EmployeeIds { get; set; }

    public string MappingEmployeeIds { get; set; }

    public string? Epfnumber { get; set; }

    public string? DeductionCycle { get; set; }

    public double? EmployeeContributionRate { get; set; }

    public double? EmployerContributionRate { get; set; }

    public bool IsCtcinclusionEmployers { get; set; }

    public bool IsCtcinclusionEmployersEdli { get; set; }

    public bool IsCtcinclusionAdminCharges { get; set; }

    public bool IsEmployeeLevelOverride { get; set; }

    public bool IsProrateRestrictedPfwage { get; set; }

    public bool IsLopbasedComponentSalary { get; set; }

    public bool IsAbryScheme { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string DisplayMessage { get; set; } = "_blank";
    public List<DepartmentKeyValues> DepartmentKeyValues { get; set; }
    public List<EmployeeKeyValues> EmployeeKeyValues { get; set; }

    public List<EPFEmployeeMappingDTO> EpfemployeeMappingList { get; set; }

}
