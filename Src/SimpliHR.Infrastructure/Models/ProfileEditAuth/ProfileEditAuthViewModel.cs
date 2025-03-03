using SimpliHR.Infrastructure.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProfileEditAuth;

public class ProfileEditAuthViewModel
{
    public List<ProfileEditAuthDTO> ProfileEditAuthList { get; set; }
    public int UnitId { get; set; }

    public EmployeeMasterDTO EmployeeDetails { get; set; }

    public List<EmployeeEditTicketViewModel>? EmployeeEditTicketListHistory { get; set; }
}
