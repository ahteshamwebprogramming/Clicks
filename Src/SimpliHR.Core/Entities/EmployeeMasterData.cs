using SimpliHR.Infrastructure.Models.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Core.Entities;

public partial class EmployeeMasterData
{
    [Key]
    public int EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public int? ClientId { get; set; }
    public int? UnitId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string EmployeeName { get; set; } = null!;

    public DateTime? Dob { get; set; }
    public DateTime? DOC { get; set; }

    public int? GenderId { get; set; }

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
    public string? ProfileImageExtension { get; set; }

    public int? WorkLocationId { get; set; }

    public DateTime? Doj { get; set; }

    public int? DepartmentId { get; set; }

    public int? JobTitleId { get; set; }

    public int? ManagerId { get; set; }
    public int? HODId { get; set; }

    public string? OfficialEmail { get; set; }
    public string? CTC { get; set; }
    public string? EPFNumber { get; set; }

    public int? IdentityId { get; set; }

    public string? EmployeeStatus { get; set; }

    public int? RoleId { get; set; }

    public int? BandId { get; set; }

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

    public int? InfoFillingStatus { get; set; }

    public int? JoinType { get; set; }

    public bool? IsActive { get; set; }

    public string? PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public DateTime? PassportValidTillDate { get; set; }
    public int? PassportIssueCountryId { get; set; }
    public int? PassportIssueStateId { get; set; }
    public int? PassportIssueCityId { get; set; }
    public string? ESINumber { get; set; }
    public string? UANNumber { get; set; }
    public int? EmploymentType { get; set; }


    public virtual ICollection<EmployeeAcademicDetail> EmployeeAcademicDetails { get; set; } = new List<EmployeeAcademicDetail>();

    public virtual ICollection<EmployeeBankDetail> EmployeeBankDetails { get; set; } = new List<EmployeeBankDetail>();

    public virtual ICollection<EmployeeCertificationDetail> EmployeeCertificationDetails { get; set; } = new List<EmployeeCertificationDetail>();

    public virtual ICollection<EmployeeContactDetail> EmployeeContactDetails { get; set; } = new List<EmployeeContactDetail>();

    public virtual ICollection<EmployeeExperienceDetail> EmployeeExperienceDetails { get; set; } = new List<EmployeeExperienceDetail>();

    public virtual ICollection<EmployeeFamilyDetail> EmployeeFamilyDetails { get; set; } = new List<EmployeeFamilyDetail>();

    public virtual ICollection<EmployeeLeaveHistory> EmployeeLeaveHistories { get; set; } = new List<EmployeeLeaveHistory>();

    //[NavigationProperty]
    //public virtual ICollection<EmployeeTempDocUpload> EmployeeTempDocUploads { get; set; } = new List<EmployeeTempDocUpload>();
    public virtual ICollection<EmployeeUploadDocument> EmployeeUploadDocuments { get; set; } = new List<EmployeeUploadDocument>();

    public virtual ICollection<EmployeeReferenceDetail> EmployeeReferenceDetails { get; set; } = new List<EmployeeReferenceDetail>();

    public virtual ICollection<EmployeeLanguageDetail> EmployeeLanguageDetails { get; set; } = new List<EmployeeLanguageDetail>();

    //public virtual ICollection<EmployeeExitResignation> EmployeeExitResignationDetails { get; set; } = new List<EmployeeExitResignation>();


    public virtual ICollection<EmployeePolicyAcceptance> EmployeePolicyAcceptances { get; set; } = new List<EmployeePolicyAcceptance>();

    //[NavigationProperty]
    //public virtual ICollection<EmployeeTempDocUploadDTO> EmployeeTempDocUploads { get; set; } = new List<EmployeeTempDocUploadDTO>();

    public virtual BloodGroupMaster BloodGroup { get; set; } = null!;

    public virtual ReligionMaster Religion { get; set; } = null!;
    public virtual JobTitleMaster JobTitle { get; set; } = null!;
    public virtual DepartmentMaster Department { get; set; } = null!;
    public virtual WorkLocationMaster WorkLocation { get; set; } = null!;

    public virtual MaritalStatusMaster MaritalStatus { get; set; } = null!;
    public virtual RoleMaster Role { get; set; } = null!;
    public int? ConfirmationPeriod { get; set; }
    public int? NoticePeriod { get; set; }


}
