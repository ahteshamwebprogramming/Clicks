using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Exit;

public class ExitEmailDTO
{
    public int ResignationListId { get; set; }
    public string? Profile { get; set; }

    public int? EmployeeId { get; set; }
    public int? ClientId { get; set; }

    public string? ClientLogo { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
    public string? JobTitle { get; set; }
    public string? DepartmentName { get; set; }
    public string? EmployeeDepartment { get; set; }
    public string? Location { get; set; }

    public int? NoticePeriod { get; set; }

    public DateTime? ResignationDate { get; set; }

    public DateTime? LastWorkingDate { get; set; }
    public DateTime? DOJ { get; set; }
    public DateTime? ResignationDateManager { get; set; }

    public DateTime? LastWorkingDateManager { get; set; }

    public bool NoticePeriodWaiveOff { get; set; }
    public bool EligibleToHire { get; set; }
    [MaxLength]
    public byte[]? Document { get; set; }
    public IFormFile? DocumentFile { get; set; }

    public string? Base64Document { get; set; }

    public string? DocumentName { get; set; }
    public string? DocumentExtension { get; set; }

    public string? ManagerRemarks { get; set; }

    public string ReasonForLeaving { get; set; } = "";
    public string ReasonForLeavingManager { get; set; } = "";
    public string ReasonForLeavingAdmin { get; set; } = "";

    public string EmployeeComments { get; set; }

    public DateTime? ResignationDateAdmin { get; set; }

    public DateTime? LastWorkingDateAdmin { get; set; }

    public bool NoticePeriodWaiveOffAdmin { get; set; }
    public bool EligibleToHireAdmin { get; set; }
    public bool ActivateExitInterview { get; set; }
    public bool ClearanceByPass { get; set; }

    public int ClearanceStatus { get; set; }
    public int InterviewStatus { get; set; }
    public int SettlementStatus { get; set; }
    [MaxLength]
    public byte[]? DocumentAdmin { get; set; }

    public IFormFile? DocumentFileAdmin { get; set; }

    public string? Base64DocumentAdmin { get; set; }

    public string? DocumentNameAdmin { get; set; }
    public string? DocumentExtensionAdmin { get; set; }
    public string? LWDPolicy { get; set; }

    public string? AdminRemarks { get; set; }

    public DateTime? CreationDateEmployee { get; set; }

    public int? ResignationInitiatedBy { get; set; }
    public bool? IsAllFormalityCompleted { get; set; }
    public bool? IsResignationRolledBack { get; set; }
    public int? UnitId { get; set; }

    public string? ExitInterviewData { get; set; }

    public int ManagerApproval { get; set; }
    public int AdminApproval { get; set; }

    public DateTime? ManagerApprovalDate { get; set; }
    public DateTime? AdminApprovalDate { get; set; }
    public DateTime? ExitInterviewSubmissionDate { get; set; }
    public string? TicketId { get; set; }
    public string? Opt { get; set; }

    public int ManagerId { get; set; }
    public String? ManagerEmailId { get; set; }
    public String? EmployeeEmailId { get; set; }
    public String? EmailDisplayName { get; set; }
    public int? EmailProvider { get; set; }
    public string? TLDPath { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerCode { get; set; }
    public string? HREmailId { get; set; }
    //public byte[]? ClientLogo { get; set; }
}
