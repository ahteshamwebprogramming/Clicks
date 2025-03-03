using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.GoogleCalendar;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SimpliHR.Infrastructure.Models.Employee;

public partial class EmployeeMasterDTO
{
    public int EmployeeId { get; set; }
    public string? EnycEmployeeId { get; set; }

    public int? ClientId { get; set; }
    public int? UnitId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string EmployeeName { get; set; } = null!;

    public DateTime? Dob { get; set; }
    public DateTime? DOC { get; set; }

    public int? GenderId { get; set; }
    public string Gender { get; set; }

    public string? FatherName { get; set; }

    public string? ContactNo { get; set; }

    public string? EmergencyContactPerson { get; set; }

    public string? EmergencyContactNo { get; set; }

    public string? EmergencyContactRelation { get; set; }

    public string? EmailId { get; set; }

    public int? Age { get; set; }

    public string? SpouseName { get; set; }

    public int? ReligionId { get; set; }

    public int? MaritalStatusId { get; set; }

    public string? AadharNumber { get; set; }

    public string? Pannumber { get; set; }

    public int? BloodGroupId { get; set; }
    [MaxLength]
    public byte[]? ProfileImage { get; set; }

    //[Required]
    //[Display(Name = "Image")]
    public IFormFile ProfileImageFile { get; set; }

    public string Base64ProfileImage { get; set; }
    public string? ProfileImageExtension { get; set; }


    public string CurrentShiftDetails { get; set; }
    public int? WorkLocationId { get; set; }

    public string CompanyName { get; set; }
    public string UnitName { get; set; }

    public DateTime? Doj { get; set; }

    public DateTime? ResignationDate { get; set; }
    public DateTime? LastWorkingDate { get; set; }
    public int? ResignationListId { get; set; }
    public int? DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int? JobTitleId { get; set; }
    public string JobTitleName { get; set; }
    public int? BankId { get; set; }
    public int? BankDetailId { get; set; }
    public int? ManagerId { get; set; }
    public string ManagerName { get; set; }
    public int? HODId { get; set; }
    public string? HODName { get; set; }
    public string? OfficialEmail { get; set; }
    public string? CTC { get; set; }
    public string? EPFNumber { get; set; }
    public int? IdentityId { get; set; }

    public string? EmployeeStatus { get; set; }

    public int? RoleId { get; set; }

    public int? BandId { get; set; }
    public int? JoinType { get; set; }
    public decimal? BasicSalary { get; set; }

    public decimal? AnnualBasicSalary { get; set; }

    public decimal? AnnualCtc { get; set; }

    public decimal? MonthlyCtc { get; set; }

    public decimal? OtherCompensation { get; set; }

    public decimal? SalaryInHand { get; set; }

    public DateTime? CreatedOn { get; set; }

    public decimal? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public decimal? ModifiedBy { get; set; }

    public int? InfoFillingStatus { get; set; } = 0;

    public bool? IsActive { get; set; } = true;
    public bool? IsConfirmed { get; set; }
    public string? PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public DateTime? PassportValidTillDate { get; set; }
    public int? PassportIssueCountryId { get; set; }
    public int? PassportIssueStateId { get; set; }
    public int? PassportIssueCityId { get; set; }

    public string? ESINumber { get; set; }
    public string? UANNumber { get; set; }

    public int? EmploymentType { get; set; }
    public bool? Layout { get; set; }

    public int? LanguageId { get; set; } = 0;
    public string? Language { get; set; }
    public int? EmailProvider { get; set; }
    public string? Url { get; set; }
    public int? Action { get; set; }
    public DateTime? LastTimeStamp { get; set; }

    public EmployeeMastersKeyValues? EmployeeMastersKeyValues { get; set; }

    public virtual ICollection<EmployeeAcademicDTO> EmployeeAcademicDetails { get; set; } = new List<EmployeeAcademicDTO>();

    public virtual ICollection<EmployeeBankDetailDTO> EmployeeBankDetails { get; set; } = new List<EmployeeBankDetailDTO>();

    public virtual ICollection<EmployeeCertificationDetailDTO> EmployeeCertificationDetails { get; set; } = new List<EmployeeCertificationDetailDTO>();

    public virtual ICollection<EmployeeContactDetailDTO> EmployeeContactDetails { get; set; } = new List<EmployeeContactDetailDTO>();

    public virtual ICollection<EmployeeExperienceDetailDTO> EmployeeExperienceDetails { get; set; } = new List<EmployeeExperienceDetailDTO>();

    public virtual ICollection<EmployeeFamilyDetailDTO> EmployeeFamilyDetails { get; set; } = new List<EmployeeFamilyDetailDTO>();

    public virtual ICollection<EmployeeLeaveHistoryDTO> EmployeeLeaveHistories { get; set; } = new List<EmployeeLeaveHistoryDTO>();

    public virtual ICollection<EmployeeUploadDocumentDTO> EmployeeUploadDocuments { get; set; } = new List<EmployeeUploadDocumentDTO>();
    public virtual ICollection<EmployeeTempDocUploadDTO> EmployeeTempDocUploads { get; set; } = new List<EmployeeTempDocUploadDTO>();

    public virtual ICollection<EmployeeReferenceDetailDTO> EmployeeReferenceDetails { get; set; } = new List<EmployeeReferenceDetailDTO>();

    public ICollection<EmployeeMasterDTO> EmployeeMasterList { get; set; } = new List<EmployeeMasterDTO>();
    public ICollection<EmployeeLanguageDetailDTO> EmployeeLanguageDetails { get; set; } = new List<EmployeeLanguageDetailDTO>();
    public ICollection<EmployeeExitResignationDTO> EmployeeExitResignationDetails { get; set; } = new List<EmployeeExitResignationDTO>();


    public BloodGroupMasterDTO BloodGroup { get; set; } = null!;
    public ReligionMasterDTO Religion { get; set; } = null!;
    public JobTitleMasterDTO JobTitle { get; set; } = null!;
    public DepartmentMasterDTO Department { get; set; } = null!;
    public JobTitleKeyValues JobTitleKeyValue { get; set; } = null!;
    public ClientKeyValues ClientKeyValue { get; set; } = null!;
    public WorkLocationMasterDTO WorkLocation { get; set; } = null!;
    public MaritalStatusMasterDTO MaritalStatus { get; set; } = null!;
    public RoleMasterDTO Role { get; set; } = null!;

    public HttpResponseMessage? HttpMessage { get; set; }
    public string? DisplayMessage { get; set; } = "_blank";
    public string FormName { get; set; }

    public int? ConfirmationPeriod { get; set; }
    public int? NoticePeriod { get; set; }

    public UnitMasterDTO? UnitMaster { get; set; }

    public string? TicketId { get; set; }
    public string? EntrySource { get; set; }

}

public partial class EmployeeMasterDataDTO
{
    public int EmployeeId { get; set; }
    public string? EnycEmployeeId { get; set; }

    public int? ClientId { get; set; }
    public int? UnitId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string EmployeeName { get; set; } = null!;

    public DateTime? Dob { get; set; }
    public DateTime? DOC { get; set; }

    public int? GenderId { get; set; }
    public string Gender { get; set; }

    public string? FatherName { get; set; }

    public string? ContactNo { get; set; }

    public string? EmailId { get; set; }

    public int? Age { get; set; }

    public string? SpouseName { get; set; }

    public int? ReligionId { get; set; }

    public int? MaritalStatusId { get; set; }

    public string? AadharNumber { get; set; }

    public string? Pannumber { get; set; }

    public int? BloodGroupId { get; set; }
    [MaxLength]
    public byte[]? ProfileImage { get; set; }

    //[Required]
    //[Display(Name = "Image")]
    public IFormFile ProfileImageFile { get; set; }

    public string Base64ProfileImage { get; set; }
    public string? ProfileImageExtension { get; set; }


    public string CurrentShiftDetails { get; set; }
    public int? WorkLocationId { get; set; }
    public int? WorkLocationName { get; set; }
    public string CompanyName { get; set; }
    public string UnitName { get; set; }

    public DateTime? Doj { get; set; }

    public DateTime? ResignationDate { get; set; }
    public int? ResignationListId { get; set; }
    public int? DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int? JobTitleId { get; set; }
    public string JobTitleName { get; set; }

    public int? ManagerId { get; set; }
    public string ManagerName { get; set; }
    public int? HODId { get; set; }
    public int? HODName { get; set; }
    public string? OfficialEmail { get; set; }
    public string? CTC { get; set; }
    public string? EPFNumber { get; set; }
    public int? IdentityId { get; set; }

    public string? EmployeeStatus { get; set; }

    public int? RoleId { get; set; }

    public int? BandId { get; set; }
    public int? JoinType { get; set; }
    public decimal? BasicSalary { get; set; }

    public decimal? AnnualBasicSalary { get; set; }

    public decimal? AnnualCtc { get; set; }

    public decimal? MonthlyCtc { get; set; }

    public decimal? OtherCompensation { get; set; }

    public decimal? SalaryInHand { get; set; }

    public DateTime? CreatedOn { get; set; }

    public decimal? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public decimal? ModifiedBy { get; set; }

    public int? InfoFillingStatus { get; set; } = 0;

    public bool? IsActive { get; set; } = true;

    public string? PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public DateTime? PassportValidTillDate { get; set; }
    public int? PassportIssueCountryId { get; set; }
    public int? PassportIssueStateId { get; set; }
    public int? PassportIssueCityId { get; set; }

    public string? ESINumber { get; set; }
    public string? UANNumber { get; set; }

    public int? EmploymentType { get; set; }
    public bool? Layout { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public string? DisplayMessage { get; set; } = "_blank";
    public string FormName { get; set; }

    public int? ConfirmationPeriod { get; set; }
    public int? NoticePeriod { get; set; }

    public BloodGroupMasterDTO BloodGroup { get; set; } = null!;
    public ReligionMasterDTO Religion { get; set; } = null!;
    public JobTitleMasterDTO JobTitle { get; set; } = null!;
    public DepartmentMasterDTO Department { get; set; } = null!;
    public JobTitleKeyValues JobTitleKeyValue { get; set; } = null!;
    public ClientKeyValues ClientKeyValue { get; set; } = null!;
    public WorkLocationMasterDTO WorkLocation { get; set; } = null!;
    public MaritalStatusMasterDTO MaritalStatus { get; set; } = null!;
    public RoleMasterDTO Role { get; set; } = null!;

    public UnitMasterDTO? UnitMaster { get; set; }

}

public partial class EmployeeMasterDataVM
{
    public ICollection<EmployeeMasterDataDTO> EmployeeMasterList { get; set; } = new List<EmployeeMasterDataDTO>();
}

public partial class EmployeeMasterVM
{
    public int EmployeeId { get; set; }
    public int LogInEmployeeId { get; set; }
    public string? EnycEmployeeId { get; set; }
    public int? ClientId { get; set; }
    public int? UnitId { get; set; }
    public string Opt { get; set; } = "Add";
    public EmployeeMasterDTO EmployeeMaster { get; set; } = new EmployeeMasterDTO();

    //public  List<EmployeeMasterDTO> EmployeeList{ get; set; }=new List<EmployeeMasterDTO>();

    public EmployeeMastersKeyValues? EmployeeMastersKeyValues { get; set; }

    public virtual ICollection<EmployeeAcademicDTO> EmployeeAcademicDetails { get; set; } = new List<EmployeeAcademicDTO>();

    public virtual ICollection<EmployeeBankDetailDTO> EmployeeBankDetails { get; set; } = new List<EmployeeBankDetailDTO>();

    public virtual ICollection<EmployeeCertificationDetailDTO> EmployeeCertificationDetails { get; set; } = new List<EmployeeCertificationDetailDTO>();

    public virtual ICollection<EmployeeContactDetailDTO> EmployeeContactDetails { get; set; } = new List<EmployeeContactDetailDTO>();

    public virtual ICollection<EmployeeExperienceDetailDTO> EmployeeExperienceDetails { get; set; } = new List<EmployeeExperienceDetailDTO>();

    public virtual ICollection<EmployeeFamilyDetailDTO> EmployeeFamilyDetails { get; set; } = new List<EmployeeFamilyDetailDTO>();

    public virtual ICollection<EmployeeLeaveHistoryDTO> EmployeeLeaveHistories { get; set; } = new List<EmployeeLeaveHistoryDTO>();

    public virtual ICollection<EmployeeUploadDocumentDTO> EmployeeUploadDocuments { get; set; } = new List<EmployeeUploadDocumentDTO>();

    public virtual ICollection<EmployeeReferenceDetailDTO> EmployeeReferenceDetails { get; set; } = new List<EmployeeReferenceDetailDTO>();

    public ICollection<EmployeeMasterDTO> EmployeeMasterList { get; set; } = new List<EmployeeMasterDTO>();
    public ICollection<EmployeeLanguageDetailDTO> EmployeeLanguageDetails { get; set; } = new List<EmployeeLanguageDetailDTO>();
    public ICollection<EmployeeExitResignationDTO> EmployeeExitResignationDetails { get; set; } = new List<EmployeeExitResignationDTO>();


    public BloodGroupMasterDTO BloodGroup { get; set; } = null!;
    public ReligionMasterDTO Religion { get; set; } = null!;
    public JobTitleMasterDTO JobTitle { get; set; } = null!;
    public DepartmentMasterDTO Department { get; set; } = null!;
    public JobTitleKeyValues JobTitleKeyValue { get; set; } = null!;
    public ClientKeyValues ClientKeyValue { get; set; } = null!;
    public WorkLocationMasterDTO WorkLocation { get; set; } = null!;
    public MaritalStatusMasterDTO MaritalStatus { get; set; } = null!;
    public RoleMasterDTO Role { get; set; } = null!;
    public HttpResponseMessage? HttpMessage { get; set; }
    public string? DisplayMessage { get; set; } = "_blank";
    public string FormName { get; set; }
    public string? TicketId { get; set; }
    public int? ConfirmationPeriod { get; set; }
    public int? NoticePeriod { get; set; }

    public UnitMasterDTO? UnitMaster { get; set; }

    public int? InfoFillingStatus { get; set; } = 0;

    public EmployeeTabFillingStatus EmployeeTabFillingStatusData { get; set; } = new EmployeeTabFillingStatus();
}

public partial class EmployeeTabFillingStatus
{
    public bool PersonalInformation { get; set; }
    public bool ProfilePicture { get; set; }
    public bool JobInformation { get; set; }
    public bool CurrentAddress { get; set; }
    public bool PermanentAddress { get; set; }
    public bool FamilyDetails { get; set; }
    public bool AcademicDetails { get; set; }
    public bool BankDetails { get; set; }
    public bool CertificationDetails { get; set; }
    public bool ReferenceDetails { get; set; }
    public bool ExperiencesBackgroud { get; set; }    
    public bool PassportDetails { get; set; }
    public bool Language { get; set; }
}

public partial class EmployeeDashboardVM
{


    public EmployeeMasterDTO EmployeeMaster { get; set; } = new EmployeeMasterDTO();

    public AttendanceHistoryViewModel AttendanceHistory { get; set; } = new AttendanceHistoryViewModel();
    public List<EmployeeLeaveBalanceDTO>? EmployeeLeaveSummary { get; set; } = null;

    public List<EmployeeCompOffDTO>? EmployeeCompOffList { get; set; } = null;
    // public List<EmployeeLeaveDetailsDTO>? EmployeeLeaveList { get; set; } = null;
    public EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();

    public ManualPunchesViewModel manualPunchVM = new ManualPunchesViewModel();
   public List<EmployeeDashboardDetailsDTO>? EmployeeBirthDays { get; set; } = null;
    // public List<EmployeeMasterDTO>? EmployeeBirthDays { get; set; } = null;

   // public List<EmployeeMasterDTO>? EmployeeOnBoardings { get; set; } = null;
    public List<EmployeeDashboardDetailsDTO>? EmployeeOnBoardings { get; set; } = null;
    //public List<EmployeeMasterDTO>? EmployeeAnnivesary { get; set; } = null;
    public List<EmployeeDashboardDetailsDTO>? EmployeeAnnivesary { get; set; } = null;
    public List<EmployeeNewsDTO>? EmployeeNews { get; set; }
    public List<EmployeeAnnouncementDTO>? EmployeeAnnouncements { get; set; }
    public List<NewsCategoryTagMasterDTO>? NewsCategoryTags { get; set; }
    public List<AnnouncementTypeMasterDTO>? AnnouncementTypes { get; set; }
   // public List<GoogleCalendarReqDTO>? GoogleEvents { get; set; }
   public List<GoogleCalendarReqDTO> GoogleEvents = new List<GoogleCalendarReqDTO>();
    public QuickAccessUnitListDTO QuickAccessDetail = new QuickAccessUnitListDTO();
    public string? Source { get; set; }
    public string? CurrentDate { get; set; }
    public decimal? TotalLeaveBalance { get; set; }
    public decimal? TotalAvailed { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}