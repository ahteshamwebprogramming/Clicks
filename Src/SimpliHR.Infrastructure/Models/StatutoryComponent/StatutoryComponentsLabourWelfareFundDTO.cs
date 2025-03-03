using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.StatutoryComponent;

public class StatutoryComponentsLabourWelfareFundDTO
{
    public int StatutoryComponentsLabourWelfareFundId { get; set; }

    public int UnitId { get; set; }

    public int DepartmentId { get; set; }

    public string DepartmentIds { get; set; }

    public string EmployeeIds { get; set; }

    public double? EmployeesContribution { get; set; }

    public double? EmployersContribution { get; set; }

    public string? DeductionCycle { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }
    public UnitMasterDTO? SelectedUnit { get; set; } = new UnitMasterDTO();

    public string DisplayMessage { get; set; } = "_blank";
    public List<DepartmentKeyValues> DepartmentKeyValues { get; set; }
    public List<EmployeeKeyValues> EmployeeKeyValues { get; set; }

}
