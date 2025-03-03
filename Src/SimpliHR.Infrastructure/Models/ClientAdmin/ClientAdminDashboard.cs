using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ClientAdmin;

public class ClientAdminDashboard
{
    public List<EmployeeMasterDTO>? EmployeeList { get; set; }
    public List<BandMasterDTO>? BandMasterList { get; set; }
    public List<EmployeeAcademicDetailDTO>? EmployeeAcademicList { get; set; }
    public List<AcademicMasterDTO>? AcademicMasterList { get; set; }
    public List<EmployeeExperienceDetailDTO>? EmployeeExperienceList { get; set; }
    public List<EmployeeExitResignationDTO>? EmployeeExitList { get; set; }
    public CurrentDateEmployeeStatsDTO? CurrentDateEmployeeStatsDTO { get; set; }
    public List<WorkLocationMasterDTO>? WorkLocations { get; set; }
    public IEnumerable<DepartmentMasterDTO>? Departments { get; set; }
}

public class CurrentDateEmployeeStatsDTO
{
    public double TotalEmployee { get; set; }
    public double TotalLeave { get; set; }
    public double Present { get; set; }
    public double LateComing { get; set; }
    public double Absent { get; set; }
}
