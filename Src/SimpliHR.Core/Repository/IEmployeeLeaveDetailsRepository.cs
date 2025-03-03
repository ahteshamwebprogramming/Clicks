using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Leave;

namespace SimpliHR.Core.Repository;

public interface IEmployeeLeaveDetailsRepository : IDapperRepository<EmployeeLeaveDetails>
{

    Task<string> SendLeaveApprovalMail(LeaveAction userAction);
    Task<string> SendLeaveReversalMail(LeaveAction userAction);
    Task<string> SendLeaveRequalizeMail(LeaveAction userAction);
    Task<string> SendLeaveRequalizeTestMail(LeaveAction userAction);
    
}

