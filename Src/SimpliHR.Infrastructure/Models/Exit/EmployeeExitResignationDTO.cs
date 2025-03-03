using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.KeyValue;
using Microsoft.AspNetCore.Http;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Payroll;

namespace SimpliHR.Infrastructure.Models.Exit;

public class EmployeeExitResignationDTO
{

    public int ResignationListId { get; set; }
    public string? encResignationListId { get; set; }

    public string? EncryptedEmployeeId { get; set; }
    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
    public string? JobTitle { get; set; }
    public string? DepartmentName { get; set; }
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

}

public class ExitViewModel
{
    public int? UnitId { get; set; }
    public int? EmployeeId { get; set; }
    public string? encEmployeeId { get; set; }

    public string? encMessageId { get; set; }
    public int? NoticePeriod { get; set; }
    public int? LeaveBalance { get; set; }
    public string? SelectedDepartments { get; set; }
    public bool? IsClient { get; set; }
    public int? Gratuity { get; set; }
    public int? LoginUserDepartment { get; set; }
    public int? LoginUserUnitId { get; set; }
    public int? LoginEmployeeId { get; set; }
    public string Base64ProfileImage { get; set; }
    public EmployeeMasterDTO EmployeeMasterInfo { get; set; } = new EmployeeMasterDTO();
    public List<EmployeeExitClearanceDTO> EmployeeExitClearanceList { get; set; } = new List<EmployeeExitClearanceDTO>();
    public List<EmployeeExitClearanceDetailDTO> EmployeeExitClearanceDetailList { get; set; } = new List<EmployeeExitClearanceDetailDTO>();
    public List<EmployeeExitClearanceHeaderDTO> EmployeeExitClearanceHeaderList { get; set; } = new List<EmployeeExitClearanceHeaderDTO>();
    public List<ExitClearanceMappingDTO> ExitClearanceMappingList { get; set; } = new List<ExitClearanceMappingDTO>();
    public List<ExitClearanceAssetMappingDTO> ExitClearanceAssetMappingList { get; set; } = new List<ExitClearanceAssetMappingDTO>();


    public List<EmployeeKeyValues>? CompanyEmployees { get; set; } = new List<EmployeeKeyValues>();
    public List<DepartmentKeyValues> Departments { get; set; } = new List<DepartmentKeyValues>();
    public List<PageKeyValues> ExitReasonList { get; set; } = new List<PageKeyValues>();
    public List<PageKeyValues> ExitClearanceStatusList { get; set; } = new List<PageKeyValues>();
    public EmployeeExitResignationDTO? ResignationDetails { get; set; } = new EmployeeExitResignationDTO();
    public List<EmployeeExitResignationDTO>? ResignationList { get; set; } = new List<EmployeeExitResignationDTO>();

    public ICollection<EmployeeSettlementDetailslDTO> objSettlementDetails { get; set; } = new List<EmployeeSettlementDetailslDTO>();
    public ICollection<PaySlipComponentsDTO> objPaySlipComponent { get; set; } = new List<PaySlipComponentsDTO>();
    public string DisplayMessage { get; set; } = string.Empty;
    public string Action { get; set; } = "Add";
}