using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Repository;

public interface IEmployeeExitResignationRepository : IDapperRepository<EmployeeExitResignation>
{
    Task<string> SendEmailResignationDetailsReceivedByManager(ExitEmailDTO dto);
    Task<string> SendEmailResignationRequestRejectedByManager(ExitEmailDTO dto);
    Task<string> SendEmailResignationRequestRejectedByAdmin(ExitEmailDTO dto);
    Task<string> SendEmailResignationRequestReceivedByHR_ManagerApproval(ExitEmailDTO dto);
    Task<string> SendEmailResignationRequestReceivedByEmployee_HRApproval(ExitEmailDTO dto);
    Task<string> SendEmailExitInterviewEmailToEmployee(ExitEmailDTO dto);
    Task<string> SendEmailClearanceRequestEmail(ExitEmailDTO dto);
}
