using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Models.StatutoryComponent;

public interface IUnitOfWork : IDisposable
{
    public ILoginDetailRepository LoginDetail { get; }
    public ICountryMasterRepository CountryMaster { get; }
    public IStateMasterRepository StateMaster { get; }
    public IDistrictMasterRepository DistrictMaster { get; }
    public ICityMasterRepository CityMaster { get; }
    //public IClientRepository ClientMaster { get; }
    public IClientRepository Client { get; }
    public IClientSettingRepository ClientSetting { get; }
    public IJobTitleMasterRepository JobTitleMaster { get; }
    public IBankMasterRepository BankMaster { get; }
    public IBandMasterRepository BandMaster { get; }
    public IDepartmentMasterRepository DepartmentMaster { get; }
    public IAcademicMasterRepository AcademicMaster { get; }
    public IIdtypeMasterRepository IdtypeMaster { get; }
    public IMaritalStatusMasterRepository MaritalStatusMaster { get; }
    public IModuleMasterRepository ModuleMaster { get; }
    public IReligionMasterRepository ReligionMaster { get; }
    public IRoleMasterRepository RoleMaster { get; }
    public ISalaryComponentMasterRepository SalaryComponentMaster { get; }
    public IBloodGroupMasterRepository BloodGroupMaster { get; }
    public IHolidaysListMasterRepository HolidaysListMaster { get; }
    public IUnitHolidayListRepository UnitHolidayList { get; }
    public IWorkLocationMasterRepository WorkLocationMaster { get; }
    public IResourceMasterRepository ResourceMaster { get; }
    public ILeaveTypeMasterRepository LeaveTypeMaster { get; }
    public IShiftMasterRepository ShiftMaster { get; }
    public IEmployeeAcademicDetailRepository EmployeeAcademicDetail { get; }
    public IEmployeeBankDetailRepository EmployeeBankDetail { get; }
    public IEmployeeContactDetailRepository EmployeeContactDetail { get; }
    public IEmployeeCertificationDetailRepository EmployeeCertificationDetail { get; }
    public IEmployeeFamilyDetailRepository EmployeeFamilyDetail { get; }
    public IEmployeeExperienceDetailRepository EmployeeExperienceDetail { get; }
    public IEmployeeLeaveHistoryRepository EmployeeLeaveHistory { get; }
    public IEmployeeMasterRepository EmployeeMaster { get; }
    public IEmployeeUploadDocumentRepository EmployeeUploadDocument { get; }
    public IEmployeeTempDocUploadRepository EmployeeTempDocUpload { get; }
    public IEmployeeReferenceDetailRepository EmployeeReferenceDetail { get; }
    public IAttendanceRosterRepository AttendanceRoster { get; }
    public IAttendanceHistoryRepository AttendanceHistory { get; }
    public IManualPunchesRepository ManualPunches { get; }
    public IMenuMasterRepository MenuMaster { get; }
    public IRoleMenuMappingRepository RoleMenuMapping { get; }

    public IAttendanceSettingRepository AttendanceSetting { get; }

    public IWorkFlowSettingsRepository WorkFlowSettings { get; }
    public IUnitMasterRepository UnitMaster { get; }

    public IEmployeeUnitsMappingRepository EmployeeUnitsMapping { get; }
    public IPolicyDocumentsCategoryMasterRepository PolicyDocumentsCategoryMaster { get; }
    public IPolicyDocumentsSubCategoryMasterRepository PolicyDocumentsSubCategoryMaster { get; }
    public IPolicyDocumentsMasterRepository PolicyDocumentsMaster { get; }
    public IEmployeePolicyAcceptanceRepository EmployeePolicyAcceptance { get; }

    public ILeaveCalenderYearRepository LeaveCalenderYear { get; }
    public ILeaveAttributeRepository LeaveAttribute { get; }
    public IEmployeeLeaveDetailsRepository EmployeeLeaveDetails { get; }
    public IAttendanceLateSettingRepository AttendanceLateSetting { get; }
    public IEmployeeLeaveBalanceRepository EmployeeLeaveBalance { get; }
    public IStatutoryComponent_EPFRepository StatutoryComponent_EPF { get; }
    public IStatutoryComponentsEsiRepository StatutoryComponentsEsi { get; }
    public IStatutoryComponentsLabourWelfareFundRepository StatutoryComponentsLabourWelfareFund { get; }

    public IPayrollEarningComponentRepository PayrollEarningComponent { get; }
    public IPayrollDeductionComponentRepository PayrollDeductionComponent { get; }
    public IPayrollReimbursementComponentRepository PayrollReimbursementComponent { get; }

    public ISalaryTemplateComponentsMappingRepository SalaryTemplateComponentsMapping { get; }

    public IEmployeeSalaryTemplateDetailRepository EmployeeSalaryTemplateDetail { get; }
    public IEmployeeSalaryTemplateMappingRepository EmployeeSalaryTemplateMapping { get; }
    public ISalaryTemplateRepository SalaryTemplate { get; }
    public IItDeclaration80CinvestmentRepository ItDeclaration80Cinvestment { get; }
    public IItDeclaration80DexemptionRepository ItDeclaration80Dexemption { get; }
    public IItDeclarationHomeLoanDetailRepository ItDeclarationHomeLoanDetail { get; }
    public IItDeclarationHouseRentDetailRepository ItDeclarationHouseRentDetail { get; }
    public IItDeclarationLentOutPropertyDetailRepository ItDeclarationLentOutPropertyDetail { get; }
    public IItDeclarationOtherSourceOfIncomeRepository ItDeclarationOtherSourceOfIncome { get; }
    public IItDeclarationPreviousEmployementRepository ItDeclarationPreviousEmployement { get; }
    public IOtherInvestmentExemptionRepository OtherInvestmentExemption { get; }
    public IItDeclarationOtherInvestmentExemptionRepository ItDeclarationOtherInvestmentExemption { get; }
    public IInvestment80CmasterRepository Investment80Cmaster { get; }
    public IExemptions80DRepository Exemptions80D { get; }
    public IEmployeesSalaryDetailsRepository EmployeesSalaryDetails { get; }

    public IEPFEmployeeMappingRepository EPFEmployeeMapping { get; }

    public IEmployeesSalaryProcessDetailsRepository EmployeesSalaryProcessDetails { get; }
    public IEmployeeExitResignationRepository EmployeeExitResignation { get; }
    public IItDeclarationTypeRepository ItDeclarationType { get; }

    public IFaceAttendanceRepository FaceAttendance { get; }

    public IEmployeeSalarySummaryRepository EmployeeSalarySummary { get; }
    public ITaxSlabDetailsRepository ITaxSlabDetail { get; }

    public IPageControlKeyValueRepository PageControlKeyValues { get; }
    public IEmployeeExitInterViewFormMasterRepository EmployeeExitInterViewFormMaster { get; }

    public IPaySlipDetailsRepository PaySlipDetails { get; }
    public IPaySlipComponentsRepository PaySlipComponents { get; }

    public IEmployeeExitClearanceRepository EmployeeExitClearance { get; }

    public IEmployeeExitClearanceDetailRepository EmployeeExitClearanceDetail { get; }

    public IEmployeeExitClearanceHeaderRepository EmployeeExitClearanceHeader { get; }

    public IExitClearanceAssetMappingRepository ExitClearanceAssetMapping { get; }

    public IExitClearanceMappingRepository ExitClearanceMapping { get; }
    public IProfileEditAuthRepository ProfileEditAuth { get; }
    public IProfileFieldRepository ProfileField { get; }
    public IEditEmployeeDataRepository EditEmployeeData { get; }
    public ITicketMasterRepository TicketMaster { get; }
    public ITemplateMasterDynamicRepository TemplateMasterDynamic { get; }

    public ILoanMasterRepository LoanMaster { get; }

    public ILoanPaymentDetailRepository LoanPaymentDetail { get; }

    public ILeaveCompOffRepository LeaveCompOff { get; }
    public IEmployeeLanguageDetailRepository EmployeeLanguageDetail { get; }

    public IProfessionalTaxRepository ProfessionalTax { get; }
    public IPerformanceSettingRepository PerformanceSetting { get; }
    public IPerformanceSettingMechanismRepository PerformanceSettingMechanism { get; }
    public IPerformanceSettingSkillSetMatrixRepository PerformanceSettingSkillSetMatrix { get; }

    public IEmployeeValidationRepository EmployeeValidation { get; }
    public IPerformanceKRAMasterDBRepository PerformanceKRAMasterDB { get; }
    public IPerformanceEmployeeTrainingDataRepository PerformanceEmployeeTrainingData { get; }
    public IPerformanceEmployeeKRADataRepository PerformanceEmployeeKRAData { get; }
    public IPerformanceEmployeeDataRepository PerformanceEmployeeData { get; }
    public IPerformanceTrainingNeedsMasterRepository PerformanceTrainingNeedsMaster { get; }

    public IRegimeSelectionReportRepository RegimeSelectionReport { get; }

    public IPayrollFullnFinalSettingsRepository PayrollFullnFinalSettings { get; }

    public IEmployeeFnFDetailsRepository EmployeeFnFDetails { get; }

    public IEmployeeSettlementDetailsRepository EmployeeSettlementDetails { get; }

    public IEmployeeSettlementSummeryRepository EmployeeSettlementSummery { get; }

    public IComponentsTaxLimitRepository ComponentsTaxLimit { get; }

    public IEmployeeValidationMasterRepository EmployeeValidationMaster { get; }
    public IEmployeeAnnouncementRepository EmployeeAnnouncement { get; }
    public IEmployeeAnnouncementFileUploadRepository EmployeeAnnouncementFileUpload { get; }

    public IBankUnitMasterRepository BankUnitMaster { get; }

    public ILanguageMasterRepository LanguageMaster { get; }
    public ILanguageUnitMasterRepository LanguageUnitMaster { get; }

    public IUnitStateMasterRepository UnitStateMaster { get; }
    public IUnitCityMasterRepository UnitCityMaster { get; }

    public IEmployeeTicketViewModelRepository EmployeeTicketViewModel { get; }
    public IEmployeeNewsRepository EmployeeNews { get; }
    public IEmployeeNewsFileUploadRepository EmployeeNewsFileUpload { get; }
    public INewsCategoryTagMasterRepository NewsCategoryTagMaster { get; }
    public IAnnouncementTypeMasterRepository AnnouncementTypeMaster { get; }

    public IEmployeeCompOffRepository EmployeeCompOff { get; }

    public ISurveyPollOptionRepository SurveyPollOption { get; }
    public ISurveyPollsQuestionRepository SurveyPollsQuestion { get; }

    public IQuickAccessListRepository QuickAccessList { get; }
    public IQuickAccessUnitListRepository QuickAccessUnitList { get; }

    public IPollResponseRepository PollResponse { get; }
    
    public IEmployeeDashboardDetailsRepository EmployeeDashboardDetails { get; }
    public IEmployeeTicketsViewRepository EmployeeTicketsView { get; }
    public IAppMessageRepository AppMessage { get; }
    public IWageBillTrendDataRepository WageBillTrendData { get; }
    public IPTProjectCategoryRepository PTProjectCategory { get; }
    public IPTAttachmentRepository PTAttachment { get; }
    public IPTCommentRepository PTComment { get; }
    public IPTProjectRepository PTProject { get; }
    public IPTProjectMemberRepository PTProjectMember { get; }
    //public IPTRoleRepository PTRole { get; }
    public IPTStatusRepository PTStatus { get; }
    public IPTTaskRepository PTTask { get; }
    
   

    public IMyMeetingsRepository MyMeetings { get; }

    public IEmployeeDirectoryRepository EmployeeDirectory { get; }
    public IMainDirectoryRepository MainDirectory { get; }
    public IPTMilestonesRepository PTMilestones { get; }
    public IPTDeliverablesRepository PTDeliverables { get; }
    public IPTProjectPriorityRepository PTProjectPriority { get; }
    public IEmployeeDirectoryCardDetailsRepository EmployeeDirectoryCardDetails { get; }

    public IEmployeeLeaveBalanceReportRepository EmployeeLeaveBalanceReport { get; }
    public IComplaintPriorityRepository ComplaintPriority { get; }
    public IComplaintCommentRepository ComplaintComment { get; }
    public IComplaintAttachmentFileRepository ComplaintAttachmentFile { get; }
    public IComplaintStatusRepository ComplaintStatus { get; }
    public IComplaintCategoryRepository ComplaintCategory { get; }
    public IComplaintRepository Complaint { get; }
    int Save();
}
