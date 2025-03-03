using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ProfileEditAuth;

public class EmployeeEditTicketListViewModel
{
    public List<EmployeeEditTicketViewModel>? EmployeeEditTicketListActionNeeded { get; set; }
    public List<EmployeeEditTicketViewModel>? EmployeeEditTicketListApproved { get; set; }
    public List<EmployeeEditTicketViewModel>? EmployeeEditTicketListActionNeededAddDel { get; set; }
    public List<EmployeeEditTicketViewModel>? EmployeeEditTicketListApprovedAddDel { get; set; }
}


public class EmployeeEditTicketViewModel
{
    public string? TicketId { get; set; }
    public string? encTicketId { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? Department { get; set; }
    public string? SourceScreen { get; set; }
    public string? encSourceScreen { get; set; }
}
