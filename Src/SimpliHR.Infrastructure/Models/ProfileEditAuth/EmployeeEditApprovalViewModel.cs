using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Common;
using SimpliHR.Infrastructure.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProfileEditAuth;

public class EmployeeEditApprovalViewModel
{
    public EmployeeEditTicketViewModel? EmployeeEditTicketDetails { get; set; }
    public List<EmployeeEditInfoViewModel>? employeeEditInfos { get; set; }

    public EmployeeAddDeleteInfoViewModel? employeeAddDeleteInfoViewModel { get; set; }
}

public class EmployeeAddDeleteInfoViewModel
{
    public List<AddDeleteTableActionDTO>? addDeleteTableActionList { get; set; }
    public List<EmployeeContactDetailDTO>? employeeContactDetailList { get; set; }
    public List<EmployeeBankDetailDTO>? employeeBankDetailList { get; set; }
    public List<EmployeeAcademicDTO>? employeeAcademicDetailList { get; set; }
    public List<EmployeeFamilyDetailDTO>? employeeFamilyDetailList { get; set; }
    public List<EmployeeExperienceDetailDTO>? EmployeeExperienceDetailList { get; set; }
    public List<EmployeeCertificationDetailDTO>? EmployeeCertificationDetailList { get; set; }
    public List<EmployeeReferenceDetailDTO>? EmployeeReferenceDetailList { get; set; }
    public List<EmployeeLanguageDetailDTO>? EmployeeLanguageDetailList { get; set; }
}
