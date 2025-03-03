using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;

namespace SimpliHR.Services.DBContext;

public class UnitOfWork : IUnitOfWork
{
    private readonly SimpliDbContext _context;
    private readonly DapperDBContext _dapperDBContext;
    //public UnitOfWork(DapperDBContext dapperDBContext)
    //{
    //    _dbSetting = dbSetting;
    //    AttendanceHistory = new AttendanceHistoryRepository(_dapperDBContext);
    //    AttendanceRoster = new AttendanceRosterRepository(_dapperDBContext);
    //}
    public UnitOfWork(SimpliDbContext context, DapperDBContext dapperDBContext, IMapper mapper)
    {
        _context = context;
        _dapperDBContext = dapperDBContext;
        LoginDetail = new LoginDetailRepository(_context);
        CountryMaster = new CountryMasterRepository(_context);
        StateMaster = new StateMasterRepository(_context);
        DistrictMaster = new DistrictMasterRepository(_context);
        CityMaster = new CityMasterRepository(_context);
        JobTitleMaster = new JobTitleMasterRepository(_context);
        BankMaster = new BankMasterRepository(_context);
        BandMaster = new BandMasterRepository(_context);
        DepartmentMaster = new DepartmentMasterRepository(_context);
        AcademicMaster = new AcademicMasterRepository(_context);
        IdtypeMaster = new IdtypeMasterRepository(_context);
        MaritalStatusMaster = new MaritalStatusMasterRepository(_context);
        ModuleMaster = new ModuleMasterRepository(_context);
        ReligionMaster = new ReligionMasterRepository(_context);
        RoleMaster = new RoleMasterRepository(_context);
        Client = new ClientRepository(_context);
        ClientSetting = new ClientSettingRepository(_context);
        SalaryComponentMaster = new SalaryComponentMasterRepository(_context);
        BloodGroupMaster = new BloodGroupMasterRepository(_context);
        HolidaysListMaster = new HolidaysListMasterRepository(_context);
        UnitHolidayList = new UnitHolidayListRepository(_context);
        WorkLocationMaster = new WorkLocationMasterRepository(_context);
        ResourceMaster = new ResourceMasterRepository(_context);
        LeaveTypeMaster = new LeaveTypeMasterRepository(_context);
        ShiftMaster = new ShiftMasterRepository(_context);
        EmployeeMaster = new EmployeeMasterRepository(_context, mapper);
        EmployeeAcademicDetail = new EmployeeAcademicDetailRepository(_context);
        EmployeeCertificationDetail = new EmployeeCertificationDetailRepository(_context);
        EmployeeLeaveHistory = new EmployeeLeaveHistoryRepository(_context);
        EmployeeUploadDocument = new EmployeeUploadDocumentRepository(_context);
        EmployeeBankDetail = new EmployeeBankDetailRepository(_context);
        EmployeeFamilyDetail = new EmployeeFamilyDetailRepository(_context);
        EmployeeContactDetail = new EmployeeContactDetailRepository(_context);
        EmployeeExperienceDetail = new EmployeeExperienceDetailRepository(_context);
        EmployeeReferenceDetail = new EmployeeReferenceDetailRepository(_context);
        MenuMaster = new MenuMasterRepository(_context);
        RoleMenuMapping = new RoleMenuMappingRepository(_context);
        AttendanceSetting = new AttendanceSettingRepository(_context);
        WorkFlowSettings = new WorkFlowSettingsRepository(_context);
        UnitMaster = new UnitMasterRepository(_context);
        EmployeeUnitsMapping = new EmployeeUnitsMappingRepository(_context);
        PolicyDocumentsCategoryMaster = new PolicyDocumentsCategoryMasterRepository(_context);
        PolicyDocumentsSubCategoryMaster = new PolicyDocumentsSubCategoryMasterRepository(_context);
        PolicyDocumentsMaster = new PolicyDocumentsMasterRepository(_context);
        EmployeePolicyAcceptance = new EmployeePolicyAcceptanceRepository(_context);
        LeaveCalenderYear = new LeaveCalenderYearRepository(_context);
        LeaveAttribute = new LeaveAttributeRepository(_context);
        AttendanceLateSetting = new AttendanceLateSettingRepository(_context);       
        StatutoryComponent_EPF = new StatutoryComponent_EPFRepository(_context);
        StatutoryComponentsEsi = new StatutoryComponentsEsiRepository(_context);
        StatutoryComponentsLabourWelfareFund = new StatutoryComponentsLabourWelfareFundRepository(_context);
        SalaryTemplate = new SalaryTemplateRepository(_context);
        SalaryTemplateComponentsMapping = new SalaryTemplateComponentsMappingRepository(_context);
        EPFEmployeeMapping = new EPFEmployeeMappingRepository(_context);
        EmployeeLanguageDetail = new EmployeeLanguageDetailRepository(_context);
        ProfessionalTax = new ProfessionalTaxRepository(_context);

        //Dapper
        EmployeeLeaveBalance = new EmployeeLeaveBalanceRepository(_dapperDBContext);
        AttendanceHistory = new AttendanceHistoryRepository(_dapperDBContext);
        AttendanceRoster = new AttendanceRosterRepository(_dapperDBContext);
        ManualPunches = new ManualPunchesRepository(_dapperDBContext);
        PayrollEarningComponent = new PayrollEarningComponentRepository(_dapperDBContext);
        PayrollDeductionComponent = new PayrollDeductionComponentRepository(_dapperDBContext);
        PayrollReimbursementComponent = new PayrollReimbursementComponentRepository(_dapperDBContext);
        EmployeeLeaveDetails = new EmployeeLeaveDetailsRepository(_dapperDBContext);
        EmployeeSalaryTemplateMapping = new EmployeeSalaryTemplateMappingRepository(_dapperDBContext);
        EmployeeSalaryTemplateDetail = new EmployeeSalaryTemplateDetailRepository(_dapperDBContext);
        EmployeesSalaryProcessDetails = new EmployeesSalaryProcessDetailsRepository(_dapperDBContext);
        FaceAttendance = new FaceAttendanceRepository(_dapperDBContext);
        EmployeeSalarySummary = new EmployeeSalarySummaryRepository(_dapperDBContext);
        PaySlipDetails = new PaySlipDetailsRepository(_dapperDBContext);
        PaySlipComponents = new PaySlipComponentsRepository(_dapperDBContext);


        ItDeclaration80Cinvestment = new ItDeclaration80CinvestmentRepository(_context);
        ItDeclaration80Dexemption = new ItDeclaration80DexemptionRepository(_context);
        ItDeclarationHomeLoanDetail = new ItDeclarationHomeLoanDetailRepository(_context);
        ItDeclarationHouseRentDetail = new ItDeclarationHouseRentDetailRepository(_context);
        ItDeclarationLentOutPropertyDetail = new ItDeclarationLentOutPropertyDetailRepository(_context);
        ItDeclarationOtherSourceOfIncome = new ItDeclarationOtherSourceOfIncomeRepository(_context);
        ItDeclarationPreviousEmployement = new ItDeclarationPreviousEmployementRepository(_context);

        OtherInvestmentExemption = new OtherInvestmentExemptionRepository(_context);
        ItDeclarationOtherInvestmentExemption = new ItDeclarationOtherInvestmentExemptionRepository(_context);
        Investment80Cmaster = new Investment80CmasterRepository(_context);
        Exemptions80D = new Exemptions80DRepository(_context);
        EmployeesSalaryDetails = new EmployeesSalaryDetailsRepository(_context);
        ItDeclarationType = new ItDeclarationTypeRepository(_context);
        PageControlKeyValues = new PageControlKeyValueRepository(_context);
        ITaxSlabDetail = new TaxSlabDetailsRepository(_context);
        EmployeeExitInterViewFormMaster = new EmployeeExitInterViewFormMasterRepository(_context);
        LoanMaster = new LoanMasterRepository(_context);
        LoanPaymentDetail = new LoanPaymentDetailRepository(_context);
        LeaveCompOff = new LeaveCompOffRepository(_context);
        PayrollFullnFinalSettings = new PayrollFullnFinalSettingsRepository(_context);
        EmployeeFnFDetails = new EmployeeFnFDetailsRepository(_context);
        EmployeeSettlementSummery = new EmployeeSettlementSummeryRepository(_context);
        ComponentsTaxLimit = new ComponentsTaxLimitRepository(_context);
        BankUnitMaster = new BankUnitMasterRepository(_context);
        LanguageMaster = new LanguageMasterRepository(_context);
        LanguageUnitMaster = new LanguageUnitMasterRepository(_context);
        UnitCityMaster = new UnitCityMasterRepository(_context);
        UnitStateMaster = new UnitStateMasterRepository(_context);
        EmployeeCompOff = new EmployeeCompOffRepository(_context);

        //Dapper

        AttendanceHistory = new AttendanceHistoryRepository(_dapperDBContext);
        AttendanceRoster = new AttendanceRosterRepository(_dapperDBContext);
        ManualPunches = new ManualPunchesRepository(_dapperDBContext);
        PayrollEarningComponent = new PayrollEarningComponentRepository(_dapperDBContext);
        PayrollDeductionComponent = new PayrollDeductionComponentRepository(_dapperDBContext);
        PayrollReimbursementComponent = new PayrollReimbursementComponentRepository(_dapperDBContext);
        EmployeeLeaveDetails = new EmployeeLeaveDetailsRepository(_dapperDBContext);
        EmployeeSalaryTemplateMapping = new EmployeeSalaryTemplateMappingRepository(_dapperDBContext);
        EmployeeSalaryTemplateDetail = new EmployeeSalaryTemplateDetailRepository(_dapperDBContext);
        EmployeesSalaryProcessDetails = new EmployeesSalaryProcessDetailsRepository(_dapperDBContext);
        FaceAttendance = new FaceAttendanceRepository(_dapperDBContext);
        EmployeeSalarySummary = new EmployeeSalarySummaryRepository(_dapperDBContext);
        //EmployeeLeaveDetails = new EmployeeLeaveDetailsRepository(_dapperDBContext);
        EmployeeExitResignation = new EmployeeExitResignationRepository(_dapperDBContext);
        EmployeeExitClearance = new EmployeeExitClearanceRepository(_dapperDBContext);
        EmployeeExitClearanceDetail = new EmployeeExitClearanceDetailRepository(_dapperDBContext);
        EmployeeExitClearanceHeader = new EmployeeExitClearanceHeaderRepository(_dapperDBContext);
        ExitClearanceAssetMapping = new ExitClearanceAssetMappingRepository(_dapperDBContext);
        ExitClearanceMapping = new ExitClearanceMappingRepository(_dapperDBContext);
        ProfileEditAuth = new ProfileEditAuthRepository(_dapperDBContext);
        ProfileField = new ProfileFieldRepository(_dapperDBContext);
        EditEmployeeData = new EditEmployeeDataRepository(_dapperDBContext);
        TicketMaster = new TicketMasterRepository(_dapperDBContext);
        TemplateMasterDynamic = new TemplateMasterDynamicRepository(_dapperDBContext);
        PerformanceSetting = new PerformanceSettingRepository(_dapperDBContext);
        EmployeeValidation = new EmployeeValidationRepository(_dapperDBContext);
        PerformanceSettingMechanism = new PerformanceSettingMechanismRepository(_dapperDBContext);
        PerformanceSettingSkillSetMatrix = new PerformanceSettingSkillSetMatrixRepository(_dapperDBContext);
        RegimeSelectionReport = new RegimeSelectionReportRepository(_dapperDBContext);
        RegimeSelectionReport = new RegimeSelectionReportRepository(_dapperDBContext);
        EmployeeTempDocUpload = new EmployeeTempDocUploadRepository(_dapperDBContext);

        PerformanceKRAMasterDB = new PerformanceKRAMasterDBRepository(_dapperDBContext);
        PerformanceEmployeeTrainingData = new PerformanceEmployeeTrainingDataRepository(_dapperDBContext);
        PerformanceEmployeeKRAData = new PerformanceEmployeeKRADataRepository(_dapperDBContext);
        PerformanceEmployeeData = new PerformanceEmployeeDataRepository(_dapperDBContext);
        PerformanceTrainingNeedsMaster = new PerformanceTrainingNeedsMasterRepository(_dapperDBContext);
        EmployeeSettlementDetails = new EmployeeSettlementDetailslRepository(_dapperDBContext);
        EmployeeValidationMaster = new EmployeeValidationMasterRepository(_dapperDBContext);
        EmployeeAnnouncement = new EmployeeAnnouncementRepository(_dapperDBContext);
        EmployeeAnnouncementFileUpload = new EmployeeAnnouncementFileUploadRepository(_dapperDBContext);
        EmployeeTicketViewModel = new EmployeeTicketViewModelRepository(_dapperDBContext);
        EmployeeNews = new EmployeeNewsRepository(_dapperDBContext);
        EmployeeNewsFileUpload = new EmployeeNewsFileUploadRepository(_dapperDBContext);
        AnnouncementTypeMaster = new AnnouncementTypeMasterRepository(_dapperDBContext);
        NewsCategoryTagMaster = new NewsCategoryTagMasterRepository(_dapperDBContext);
        SurveyPollOption = new SurveyPollOptionRepository(_dapperDBContext);
        SurveyPollsQuestion = new SurveyPollsQuestionRepository(_dapperDBContext);
        QuickAccessList = new QuickAccessListRepository(_dapperDBContext);
        QuickAccessUnitList = new QuickAccessUnitListRepository(_dapperDBContext);
        PollResponse = new PollResponseRepository(_dapperDBContext);

        EmployeeDashboardDetails = new EmployeeDashboardDetailsRepository(_dapperDBContext);
        EmployeeTicketsView = new EmployeeTicketsViewRepository(_dapperDBContext);
        AppMessage = new AppMessageRepository(_dapperDBContext);
        WageBillTrendData = new WageBillTrendDataRepository(_dapperDBContext);

        //PTRole = new PTRoleRepository(_dapperDBContext);
        PTStatus = new PTStatusRepository(_dapperDBContext);
        PTProjectCategory = new PTProjectCategoryRepository(_dapperDBContext);
        PTProject = new PTProjectRepository(_dapperDBContext);
        PTTask = new PTTaskRepository(_dapperDBContext);
        PTProjectMember = new PTProjectMemberRepository(_dapperDBContext);
        PTComment = new PTCommentRepository(_dapperDBContext);
        PTAttachment = new PTAttachmentRepository(_dapperDBContext);
        EmployeeDashboardDetails = new EmployeeDashboardDetailsRepository(_dapperDBContext);
        EmployeeTicketsView = new EmployeeTicketsViewRepository(_dapperDBContext);
        AppMessage = new AppMessageRepository(_dapperDBContext);
        MyMeetings = new MyMeetingsRepository(_dapperDBContext);
        EmployeeDirectory = new EmployeeDirectoryRepository(_dapperDBContext);
        MainDirectory = new MainDirectoryRepository(_dapperDBContext);
        PTMilestones = new PTMilestonesRepository(_dapperDBContext);
        PTDeliverables = new PTDeliverablesRepository(_dapperDBContext);
        PTProjectPriority = new PTProjectPriorityRepository(_dapperDBContext);
        EmployeeDirectoryCardDetails = new EmployeeDirectoryCardDetailsRepository(_dapperDBContext);
        EmployeeLeaveBalanceReport = new EmployeeLeaveBalanceReportRepository(_dapperDBContext);

        ComplaintPriority = new ComplaintPriorityRepository(_dapperDBContext);
        ComplaintComment = new ComplaintCommentRepository(_dapperDBContext);
        ComplaintAttachmentFile = new ComplaintAttachmentFileRepository(_dapperDBContext);
        ComplaintStatus = new ComplaintStatusRepository(_dapperDBContext);
        ComplaintCategory = new ComplaintCategoryRepository(_dapperDBContext);
        Complaint = new ComplaintRepository(_dapperDBContext);

    }
    public ILoginDetailRepository LoginDetail
    { get; private set; }
    public ICountryMasterRepository CountryMaster
    { get; private set; }
    public IStateMasterRepository StateMaster
    { get; private set; }
    public IDistrictMasterRepository DistrictMaster
    { get; private set; }
    public ICityMasterRepository CityMaster
    { get; private set; }
    public IJobTitleMasterRepository JobTitleMaster
    { get; private set; }
    public IBankMasterRepository BankMaster
    { get; private set; }
    public IBandMasterRepository BandMaster
    { get; private set; }
    public IDepartmentMasterRepository DepartmentMaster
    { get; private set; }
    public IAcademicMasterRepository AcademicMaster
    { get; private set; }
    public IIdtypeMasterRepository IdtypeMaster
    { get; private set; }
    public IMaritalStatusMasterRepository MaritalStatusMaster
    { get; private set; }
    public IModuleMasterRepository ModuleMaster
    { get; private set; }
    public IReligionMasterRepository ReligionMaster
    { get; private set; }
    public IRoleMasterRepository RoleMaster
    { get; private set; }
    //public IClientMasterRepository ClientMaster
    //{ get; private set; }

    public IClientRepository Client
    { get; private set; }
    public IClientSettingRepository ClientSetting
    { get; private set; }
    public ISalaryComponentMasterRepository SalaryComponentMaster
    { get; private set; }
    public IBloodGroupMasterRepository BloodGroupMaster
    { get; private set; }
    public IHolidaysListMasterRepository HolidaysListMaster
    { get; private set; }

    public IUnitHolidayListRepository UnitHolidayList
    { get; private set; }

    public IWorkLocationMasterRepository WorkLocationMaster
    { get; private set; }
    public IResourceMasterRepository ResourceMaster
    { get; private set; }
    public ILeaveTypeMasterRepository LeaveTypeMaster
    { get; private set; }
    public IShiftMasterRepository ShiftMaster
    { get; private set; }
    public IEmployeeAcademicDetailRepository EmployeeAcademicDetail
    { get; private set; }
    public IEmployeeBankDetailRepository EmployeeBankDetail
    { get; private set; }
    public IEmployeeCertificationDetailRepository EmployeeCertificationDetail
    { get; private set; }
    public IEmployeeLeaveHistoryRepository EmployeeLeaveHistory
    { get; private set; }
    public IEmployeeMasterRepository EmployeeMaster
    { get; private set; }
    public IEmployeeUploadDocumentRepository EmployeeUploadDocument
    { get; private set; }

    public IEmployeeFamilyDetailRepository EmployeeFamilyDetail
    { get; private set; }
    public IEmployeeContactDetailRepository EmployeeContactDetail
    { get; private set; }
    public IEmployeeExperienceDetailRepository EmployeeExperienceDetail
    { get; private set; }

    public IEmployeeReferenceDetailRepository EmployeeReferenceDetail
    { get; private set; }
    public IAttendanceHistoryRepository AttendanceHistory
    { get; private set; }
    public IAttendanceRosterRepository AttendanceRoster
    { get; private set; }
    public IManualPunchesRepository ManualPunches
    { get; private set; }
    public IMenuMasterRepository MenuMaster
    { get; private set; }

    public IRoleMenuMappingRepository RoleMenuMapping
    { get; private set; }

    public IAttendanceSettingRepository AttendanceSetting
    { get; private set; }

    public IWorkFlowSettingsRepository WorkFlowSettings
    { get; private set; }

    public IUnitMasterRepository UnitMaster
    { get; private set; }

    public IEmployeeUnitsMappingRepository EmployeeUnitsMapping
    { get; private set; }

    public IPolicyDocumentsCategoryMasterRepository PolicyDocumentsCategoryMaster
    { get; private set; }
    public IPolicyDocumentsSubCategoryMasterRepository PolicyDocumentsSubCategoryMaster
    { get; private set; }
    public IPolicyDocumentsMasterRepository PolicyDocumentsMaster
    { get; private set; }
    public IEmployeePolicyAcceptanceRepository EmployeePolicyAcceptance
    { get; private set; }

    public ILeaveCalenderYearRepository LeaveCalenderYear
    { get; private set; }
    public ILeaveAttributeRepository LeaveAttribute
    { get; private set; }

    public IEmployeeLeaveDetailsRepository EmployeeLeaveDetails
    { get; private set; }

    public IAttendanceLateSettingRepository AttendanceLateSetting
    { get; private set; }

    public IEmployeeLeaveBalanceRepository EmployeeLeaveBalance
    { get; private set; }
    public IStatutoryComponent_EPFRepository StatutoryComponent_EPF
    { get; private set; }

    public IStatutoryComponentsEsiRepository StatutoryComponentsEsi
    { get; private set; }
    public IStatutoryComponentsLabourWelfareFundRepository StatutoryComponentsLabourWelfareFund
    { get; private set; }

    public IPayrollEarningComponentRepository PayrollEarningComponent
    { get; private set; }
    public ISalaryTemplateRepository SalaryTemplate
    { get; private set; }

    public IEPFEmployeeMappingRepository EPFEmployeeMapping
    { get; private set; }

    public ISalaryTemplateComponentsMappingRepository SalaryTemplateComponentsMapping
    { get; private set; }

    public IPayrollDeductionComponentRepository PayrollDeductionComponent
    { get; private set; }

    public IPayrollReimbursementComponentRepository PayrollReimbursementComponent
    { get; private set; }

    public IEmployeeSalaryTemplateMappingRepository EmployeeSalaryTemplateMapping
    { get; private set; }

    public IEmployeeSalaryTemplateDetailRepository EmployeeSalaryTemplateDetail
    { get; private set; }

    //public ISalaryTemplateRepository SalaryTemplate
    //{ get; private set; }

    public IItDeclaration80CinvestmentRepository ItDeclaration80Cinvestment
    { get; private set; }
    public IItDeclaration80DexemptionRepository ItDeclaration80Dexemption
    { get; private set; }
    public IItDeclarationHomeLoanDetailRepository ItDeclarationHomeLoanDetail
    { get; private set; }
    public IItDeclarationHouseRentDetailRepository ItDeclarationHouseRentDetail
    { get; private set; }
    public IItDeclarationLentOutPropertyDetailRepository ItDeclarationLentOutPropertyDetail
    { get; private set; }
    public IItDeclarationOtherSourceOfIncomeRepository ItDeclarationOtherSourceOfIncome
    { get; private set; }
    public IItDeclarationPreviousEmployementRepository ItDeclarationPreviousEmployement
    { get; private set; }

    public IOtherInvestmentExemptionRepository OtherInvestmentExemption
    { get; private set; }
    public IItDeclarationOtherInvestmentExemptionRepository ItDeclarationOtherInvestmentExemption
    { get; private set; }
    public IInvestment80CmasterRepository Investment80Cmaster
    { get; private set; }
    public IExemptions80DRepository Exemptions80D
    { get; private set; }
    public IEmployeesSalaryDetailsRepository EmployeesSalaryDetails
    { get; private set; }

    public IEmployeesSalaryProcessDetailsRepository EmployeesSalaryProcessDetails
    { get; private set; }

    public IFaceAttendanceRepository FaceAttendance
    { get; private set; }
    public IEmployeeExitResignationRepository EmployeeExitResignation
    { get; private set; }

    public IEmployeeSalarySummaryRepository EmployeeSalarySummary
    { get; private set; }
    public IItDeclarationTypeRepository ItDeclarationType
    { get; private set; }

    public IPageControlKeyValueRepository PageControlKeyValues
    { get; private set; }


    public ITaxSlabDetailsRepository ITaxSlabDetail
    { get; private set; }
    public IEmployeeExitInterViewFormMasterRepository EmployeeExitInterViewFormMaster
    { get; private set; }

    public IPaySlipDetailsRepository PaySlipDetails
    { get; private set; }

    public IPaySlipComponentsRepository PaySlipComponents
    { get; private set; }
    public IEmployeeExitClearanceRepository EmployeeExitClearance { get; private set; }

    public IEmployeeExitClearanceDetailRepository EmployeeExitClearanceDetail { get; private set; }

    public IEmployeeExitClearanceHeaderRepository EmployeeExitClearanceHeader { get; private set; }

    public IExitClearanceAssetMappingRepository ExitClearanceAssetMapping { get; private set; }

    public IExitClearanceMappingRepository ExitClearanceMapping { get; private set; }
    public IProfileFieldRepository ProfileField { get; private set; }
    public IProfileEditAuthRepository ProfileEditAuth { get; private set; }
    public IEditEmployeeDataRepository EditEmployeeData { get; private set; }
    public ITicketMasterRepository TicketMaster { get; private set; }
    public ITemplateMasterDynamicRepository TemplateMasterDynamic { get; private set; }

    public ILoanMasterRepository LoanMaster { get; private set; }

    public ILoanPaymentDetailRepository LoanPaymentDetail { get; private set; }

    public ILeaveCompOffRepository LeaveCompOff { get; private set; }
    public IEmployeeLanguageDetailRepository EmployeeLanguageDetail { get; private set; }

    public IProfessionalTaxRepository ProfessionalTax { get; private set; }
    public IPerformanceSettingRepository PerformanceSetting { get; private set; }
    public IPerformanceSettingMechanismRepository PerformanceSettingMechanism { get; private set; }
    public IPerformanceSettingSkillSetMatrixRepository PerformanceSettingSkillSetMatrix { get; private set; }
    public IPerformanceKRAMasterDBRepository PerformanceKRAMasterDB { get; private set; }
    public IPerformanceEmployeeTrainingDataRepository PerformanceEmployeeTrainingData { get; private set; }
    public IPerformanceEmployeeKRADataRepository PerformanceEmployeeKRAData { get; private set; }
    public IPerformanceEmployeeDataRepository PerformanceEmployeeData { get; private set; }
    public IPerformanceTrainingNeedsMasterRepository PerformanceTrainingNeedsMaster { get; private set; }

    public IEmployeeValidationRepository EmployeeValidation
    { get; private set; }
    public IRegimeSelectionReportRepository RegimeSelectionReport { get; private set; }

    public IPayrollFullnFinalSettingsRepository PayrollFullnFinalSettings { get; private set; }

    public IEmployeeTempDocUploadRepository EmployeeTempDocUpload { get; private set; }

    public IEmployeeFnFDetailsRepository EmployeeFnFDetails { get; private set; }

    public IEmployeeSettlementDetailsRepository EmployeeSettlementDetails { get; private set; }
    public IEmployeeSettlementSummeryRepository EmployeeSettlementSummery { get; private set; }

    public IComponentsTaxLimitRepository ComponentsTaxLimit { get; private set; }

    public IEmployeeValidationMasterRepository EmployeeValidationMaster { get; private set; }
    public IEmployeeAnnouncementRepository EmployeeAnnouncement { get; private set; }
    public IEmployeeAnnouncementFileUploadRepository EmployeeAnnouncementFileUpload { get; private set; }

    public IBankUnitMasterRepository BankUnitMaster { get; private set; }

    public ILanguageMasterRepository LanguageMaster { get; private set; }

    public ILanguageUnitMasterRepository LanguageUnitMaster { get; private set; }
    public IUnitCityMasterRepository UnitCityMaster { get; private set; }
    public IUnitStateMasterRepository UnitStateMaster { get; private set; }

    public IEmployeeTicketViewModelRepository EmployeeTicketViewModel
    { get; private set; }
    public IEmployeeNewsFileUploadRepository EmployeeNewsFileUpload { get; private set; }
    public IEmployeeNewsRepository EmployeeNews { get; private set; }
    public INewsCategoryTagMasterRepository NewsCategoryTagMaster { get; private set; }
    public IAnnouncementTypeMasterRepository AnnouncementTypeMaster { get; private set; }
    public ISurveyPollOptionRepository SurveyPollOption { get; private set; }
    public ISurveyPollsQuestionRepository SurveyPollsQuestion { get; private set; }
    public IEmployeeCompOffRepository EmployeeCompOff { get; private set; }
    public IQuickAccessListRepository QuickAccessList { get; private set; }
    public IQuickAccessUnitListRepository QuickAccessUnitList { get; private set; }
    public IPollResponseRepository PollResponse { get; private set; }


    public IEmployeeDashboardDetailsRepository EmployeeDashboardDetails { get; private set; }
    public IEmployeeTicketsViewRepository EmployeeTicketsView { get; private set; }
    public IAppMessageRepository AppMessage { get; private set; }
    public IWageBillTrendDataRepository WageBillTrendData { get; private set; }

    //public IPTRoleRepository PTRole { get; private set; }
    public IPTStatusRepository PTStatus { get; private set; }
    public IPTProjectCategoryRepository PTProjectCategory { get; private set; }
    public IPTProjectRepository PTProject { get; private set; }
    public IPTTaskRepository PTTask { get; private set; }
    public IPTProjectMemberRepository PTProjectMember { get; private set; }
    public IPTCommentRepository PTComment { get; private set; }
    public IPTAttachmentRepository PTAttachment { get; private set; }    


    public IMyMeetingsRepository MyMeetings { get; private set; }

    public IEmployeeDirectoryRepository EmployeeDirectory { get; private set; }
    public IMainDirectoryRepository MainDirectory { get; private set; }
    public IEmployeeDirectoryCardDetailsRepository EmployeeDirectoryCardDetails { get; private set; }
    public IPTMilestonesRepository PTMilestones { get; private set; }
    public IPTDeliverablesRepository PTDeliverables { get; private set; }
    public IPTProjectPriorityRepository PTProjectPriority { get; private set; }

    public IEmployeeLeaveBalanceReportRepository EmployeeLeaveBalanceReport { get; private set; }

    public IComplaintPriorityRepository ComplaintPriority { get; private set; }
    public IComplaintCommentRepository ComplaintComment { get; private set; }
    public IComplaintAttachmentFileRepository ComplaintAttachmentFile { get; private set; }
    public IComplaintStatusRepository ComplaintStatus { get; private set; }
    public IComplaintCategoryRepository ComplaintCategory { get; private set; }
    public IComplaintRepository Complaint { get; private set; }

    public void Dispose()
    {
        try { _context.Dispose(); }
        catch { }
    }

    public int Save()
    {
        return _context.SaveChanges();
    }
}

