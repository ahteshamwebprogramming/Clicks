using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Employee;

namespace SimpliHR.Services.DBContext;

public partial class SimpliDbContext : DbContext
{
    //static string connection;
    public SimpliDbContext()
    {
        // connection = System.Configuration.ConfigurationManager.ConnectionStrings["SimplyDBConnection"].ConnectionString;
    }

    public SimpliDbContext(DbContextOptions<SimpliDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicMaster> AcademicMasters { get; set; }

    public virtual DbSet<BandMaster> BandMasters { get; set; }

    public virtual DbSet<BankMaster> BankMasters { get; set; }

    public virtual DbSet<BloodGroupMaster> BloodGroupMasters { get; set; }

    public virtual DbSet<CityMaster> CityMasters { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientIDTypeMapping> ClientIDTypeMapping { get; set; }

    public virtual DbSet<ClientModuleMapping> ClientModuleMappings { get; set; }

    public virtual DbSet<ColorTheme> ColorThemes { get; set; }

    public virtual DbSet<CountryMaster> CountryMasters { get; set; }

    public virtual DbSet<DepartmentMaster> DepartmentMasters { get; set; }

    public virtual DbSet<DistrictMaster> DistrictMasters { get; set; }

    public virtual DbSet<ClientSetting> ClientSettings { get; set; }

    public virtual DbSet<EmployeeAcademicDetail> EmployeeAcademicDetails { get; set; }

    public virtual DbSet<EmployeeBankDetail> EmployeeBankDetails { get; set; }

    public virtual DbSet<EmployeeCertificationDetail> EmployeeCertificationDetails { get; set; }

    public virtual DbSet<EmployeeContactDetail> EmployeeContactDetails { get; set; }

    public virtual DbSet<EmployeeExperienceDetail> EmployeeExperienceDetails { get; set; }

    public virtual DbSet<EmployeeFamilyDetail> EmployeeFamilyDetails { get; set; }

    public virtual DbSet<EmployeeLeaveHistory> EmployeeLeaveHistories { get; set; }

    public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }


    //public virtual DbSet<EmployeePersonalDetail> EmployeePersonalDetails { get; set; }

    public virtual DbSet<EmployeeReferenceDetail> EmployeeReferenceDetails { get; set; }

    public virtual DbSet<EmployeeUploadDocument> EmployeeUploadDocuments { get; set; }

    public virtual DbSet<HolidaysListMaster> HolidaysListMasters { get; set; }

    public virtual DbSet<UnitHolidayList> UnitHolidayLists { get; set; }

    public virtual DbSet<IdtypeMaster> IdtypeMasters { get; set; }

    public virtual DbSet<JobTitleMaster> JobTitleMasters { get; set; }

    public virtual DbSet<LeaveTypeMaster> LeaveTypeMasters { get; set; }

    public virtual DbSet<LoginDetail> LoginDetails { get; set; }

    public virtual DbSet<MaritalStatusMaster> MaritalStatusMasters { get; set; }

    public virtual DbSet<EmployeeMaster> Manager { get; set; }

    public virtual DbSet<EmployeeMaster> HOD { get; set; }
    public virtual DbSet<ModuleMaster> ModuleMasters { get; set; }

    public virtual DbSet<ReligionMaster> ReligionMasters { get; set; }

    public virtual DbSet<ResourceMaster> ResourceMasters { get; set; }

    public virtual DbSet<RoleMaster> RoleMasters { get; set; }

    public virtual DbSet<SalaryComponentMaster> SalaryComponentMasters { get; set; }

    public virtual DbSet<ShiftMaster> ShiftMasters { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }

    public virtual DbSet<WorkLocationMaster> WorkLocationMasters { get; set; }
    public virtual DbSet<RoleMenuMapping> RoleMenuMappings { get; set; }
    public virtual DbSet<MenuMaster> MenuMasters { get; set; }

    public virtual DbSet<AttendanceSetting> AttendanceSettings { get; set; }
    public virtual DbSet<WorkFlowSettings> WorkFlowSettings { get; set; }
    public virtual DbSet<UnitMaster> UnitMaster { get; set; }

    public virtual DbSet<EmployeeUnitsMapping> EmployeeUnitsMapping { get; set; }
    public virtual DbSet<PolicyDocumentsCategoryMaster> PolicyDocumentsCategoryMasters { get; set; }

    public virtual DbSet<PolicyDocumentsSubCategoryMaster> PolicyDocumentsSubCategoryMasters { get; set; }

    public virtual DbSet<PolicyDocumentsMaster> PolicyDocumentsMasters { get; set; }
    public virtual DbSet<EmployeePolicyAcceptance> EmployeePolicyAcceptances { get; set; }
    public virtual DbSet<LeaveCalenderYear> LeaveCalenderYears { get; set; }
    public virtual DbSet<LeaveAttribute> LeaveAttributes { get; set; }

    public virtual DbSet<EmployeeLeaveDetails> EmployeeLeaveDetails { get; set; }

    public virtual DbSet<AttendanceLateSetting> AttendanceLateSetting { get; set; }

    public virtual DbSet<EmployeeLeaveBalance> EmployeeLeaveBalance { get; set; }

    public virtual DbSet<StatutoryComponent_EPF> StatutoryComponents_EPF { get; set; }
    public virtual DbSet<StatutoryComponentsEsi> StatutoryComponentsEsis { get; set; }
    public virtual DbSet<StatutoryComponentsLabourWelfareFund> StatutoryComponentsLabourWelfareFunds { get; set; }

    public virtual DbSet<SalaryTemplate> SalaryTemplates { get; set; }

    public virtual DbSet<SalaryTemplateComponentsMapping> SalaryTemplateComponentsMappings { get; set; }

    public virtual DbSet<ItDeclaration80Cinvestment> ItDeclaration80Cinvestments { get; set; }

    public virtual DbSet<ItDeclaration80Dexemption> ItDeclaration80Dexemptions { get; set; }

    public virtual DbSet<ItDeclarationHomeLoanDetail> ItDeclarationHomeLoanDetails { get; set; }

    public virtual DbSet<ItDeclarationHouseRentDetail> ItDeclarationHouseRentDetails { get; set; }

    public virtual DbSet<ItDeclarationLentOutPropertyDetail> ItDeclarationLentOutPropertyDetails { get; set; }

    public virtual DbSet<ItDeclarationOtherSourceOfIncome> ItDeclarationOtherSourceOfIncomes { get; set; }

    public virtual DbSet<ItDeclarationPreviousEmployement> ItDeclarationPreviousEmployements { get; set; }
    public virtual DbSet<ItDeclarationOtherInvestmentExemption> ItDeclarationOtherInvestmentExemptions { get; set; }
    public virtual DbSet<Investment80Cmaster> Investment80Cmasters { get; set; }
    public virtual DbSet<Exemptions80D> Exemptions80Ds { get; set; }
    public virtual DbSet<OtherInvestmentExemption> OtherInvestmentExemptions { get; set; }



    public virtual DbSet<EpfemployeeMapping> EpfemployeeMapping { get; set; }

    public virtual DbSet<EmployeesSalaryDetails> EmployeesSalaryDetails { get; set; }

    public virtual DbSet<EmployeeExitResignation> EmployeeExitResignations { get; set; }

    public virtual DbSet<ItDeclarationType> ItDeclarationTypes { get; set; }

    public virtual DbSet<PageControlKeyValue> PageControlKeyValues { get; set; }
    public virtual DbSet<TaxSlabDetails> TaxSlabDetails { get; set; }

    public virtual DbSet<EmployeeExitInterViewFormMaster> EmployeeExitInterViewFormMasters { get; set; }

    public virtual DbSet<ExitClearanceMapping> ExitClearanceMappings { get; set; }

    public virtual DbSet<ExitClearanceAssetMapping> ExitClearanceAssetMappings { get; set; }

    public virtual DbSet<ProfileEditAuth> ProfileEditAuths { get; set; }

    public virtual DbSet<ProfileField> ProfileFields { get; set; }

    //public virtual DbSet<EditEmployeeData> EditEmployeeData { get; set; }
    public virtual DbSet<TicketMaster> TicketMasters { get; set; }
    public virtual DbSet<TemplateMasterDynamic> TemplateMasterDynamics { get; set; }

    public virtual DbSet<LoanMaster> LoanMasters { get; set; }

    public virtual DbSet<LoanPaymentDetail> LoanPaymentDetails { get; set; }

    public virtual DbSet<LeaveCompOff> LeaveCompOffs { get; set; }
    public virtual DbSet<EmployeeLanguageDetail> EmployeeLanguageDetails { get; set; }
    public virtual DbSet<ProfessionalTax> ProfessionalTaxs { get; set; }
    public virtual DbSet<PayrollFullnFinalSettings> PayrollFullnFinalSettinges { get; set; }
    public virtual DbSet<EmployeeFnFDetails> EmployeeFnFDetailes { get; set; }
    public virtual DbSet<PerformanceSetting> PerformanceSettings { get; set; }

    public virtual DbSet<PerformanceSettingMechanism> PerformanceSettingMechanisms { get; set; }

    public virtual DbSet<PerformanceSettingSkillSetMatrix> PerformanceSettingSkillSetMatrices { get; set; }

    public virtual DbSet<PerformanceKRAMasterDB> PerformanceKRAMasterDBs { get; set; }

    public virtual DbSet<PerformanceEmployeeTrainingData> PerformanceEmployeeTrainingDatas { get; set; }
    public virtual DbSet<PerformanceEmployeeKRAData> PerformanceEmployeeKRADatas { get; set; }
    public virtual DbSet<PerformanceEmployeeData> PerformanceEmployeeDatas { get; set; }
    public virtual DbSet<PerformanceTrainingNeedsMaster> TrainingNeedsMasters { get; set; }
    public virtual DbSet<ComponentsTaxLimit> ComponentsTaxLimits { get; set; }

    public virtual DbSet<EmployeeSettlementSummery> EmployeesSettlementSummery { get; set; }

    public virtual DbSet<EmployeeAnnouncement> EmployeeAnnouncements { get; set; }

    public virtual DbSet<EmployeeAnnouncementFileUpload> EmployeeAnnouncementFileUploads { get; set; }
    public virtual DbSet<BankUnitMaster> BankUnitMasters { get; set; }
    public virtual DbSet<LanguageMaster> LanguagesMaster { get; set; }
    public virtual DbSet<LanguageUnitMaster> LanguagesUnitMaster { get; set; }
    public virtual DbSet<UnitStateMaster> UnitStatesMaster { get; set; }
    public virtual DbSet<UnitCityMaster> UnitCitiesMaster { get; set; }
    public virtual DbSet<EmployeeNews> EmployeeNews { get; set; }
    public virtual DbSet<EmployeeNewsFileUpload> EmployeeNewsFileUploads { get; set; }

    public virtual DbSet<AnnouncementTypeMaster> AnnouncementTypeMasters { get; set; }
    public virtual DbSet<NewsCategoryTagMaster> NewsCategoryTagMasters { get; set; }

    public virtual DbSet<SurveyPollOption> SurveyPollOptions { get; set; }

    public virtual DbSet<SurveyPollsQuestion> SurveyPollsQuestions { get; set; }

    public virtual DbSet<EmployeeCompOff> EmployeeCompOffs { get; set; }
    public virtual DbSet<QuickAccessList> QuickAccessLists { get; set; }
    public virtual DbSet<QuickAccessUnitList> QuickAccessUnitLists { get; set; }

    public DbSet<PollResponse> PollResponses { get; set; }

    public virtual DbSet<AppMessage> AppMessages { get; set; }
    public virtual DbSet<MyMeetings> MyMeetingss { get; set; }
    public virtual DbSet<WageBillTrendData> WageBillTrendDatas { get; set; }
    public virtual DbSet<EmployeeDirectory> EmployeeDirectorys { get; set; }
    public virtual DbSet<MainDirectory> MainDirectorys { get; set; }

    public virtual DbSet<PTProjectCategory> PTProjectCategories { get; set; }
    public virtual DbSet<PTAttachment> PTAttachments { get; set; }
    public virtual DbSet<PTComment> PTComments { get; set; }
    public virtual DbSet<PTProject> PTProjects { get; set; }
    public virtual DbSet<PTProjectMember> PTProjectMembers { get; set; }
    //public virtual DbSet<PTRole> PTRoles { get; set; }
    public virtual DbSet<PTStatus> PTStatuss { get; set; }

    public virtual DbSet<PTDeliverables> PTDeliverabless { get; set; }

    public virtual DbSet<PTMilestones> PTMilestoness { get; set; }
    
    public virtual DbSet<PTTask> PTTasks { get; set; }

    public virtual DbSet<PTProjectPriority> PTProjectPriorities { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{

    //}
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //  => optionsBuilder.UseSqlServer("Data Source=tcp:mysimplyhr.database.windows.net,1433;Initial Catalog=SimpliHRdb;User ID=SimpliHR;Password=Delta@304020;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicMaster>(entity =>
        {
            entity.HasKey(e => e.AcademicId);

            entity.ToTable("AcademicMaster");

            entity.Property(e => e.AcademicCode).HasMaxLength(10);
            entity.Property(e => e.AcademicName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<BandMaster>(entity =>
        {
            entity.HasKey(e => e.BandId);

            entity.ToTable("BandMaster");

            entity.Property(e => e.Band).HasMaxLength(50);
            entity.Property(e => e.BandCode).HasMaxLength(10);
            entity.Property(e => e.BandDesc).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<BankMaster>(entity =>
        {
            entity.HasKey(e => e.BankId);

            entity.ToTable("BankMaster");

            entity.Property(e => e.BankName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<BloodGroupMaster>(entity =>
        {
            entity.HasKey(e => e.BloodGroupId);

            entity.ToTable("BloodGroupMaster");

            entity.Property(e => e.BloodGroupCode).HasMaxLength(50);
            entity.Property(e => e.BloodGroupName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<CityMaster>(entity =>
        {
            entity.HasKey(e => e.CityId);

            entity.ToTable("CityMaster");

            entity.Property(e => e.CityName).HasMaxLength(50);
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modifiedOn");
            entity.Property(e => e.StateId).HasColumnName("StateID");

            entity.HasOne(d => d.Country).WithMany(p => p.CityMasters)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CityMaster_CountryMaster");

            //entity.HasOne(d => d.State).WithMany(p => p.CityMasters)
            //    .HasForeignKey(d => d.StateId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_CityMaster_StateMaster");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK_ClientMaster");

            entity.ToTable("Client");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.ClientLogo).HasMaxLength(100);
            entity.Property(e => e.ClientName).HasMaxLength(50);
            entity.Property(e => e.ColorTheme).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(500);
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DocumentLink).HasMaxLength(100);
            entity.Property(e => e.EmailId)
                .HasMaxLength(100)
                .HasColumnName("EmailID");
            entity.Property(e => e.FooterText).HasMaxLength(500);
            entity.Property(e => e.GSTN)
                .HasMaxLength(50)
                .HasColumnName("GSTN");
            entity.Property(e => e.HeaderText).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.MenuStyle)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValueSql("('V')")
                .IsFixedLength();
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PoliciesLink).HasMaxLength(100);
            entity.Property(e => e.StateId).HasColumnName("StateID");
            entity.Property(e => e.SupportLink).HasMaxLength(100);
        });

        modelBuilder.Entity<ClientIDTypeMapping>(entity =>
        {
            entity.ToTable("ClientIDTypeMapping");

            entity.Property(e => e.ClientIDTypeMappingId).HasColumnName("ClientIDTypeMappingID");
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.IDTypeId).HasColumnName("IDTypeID");
        });



        modelBuilder.Entity<ClientModuleMapping>(entity =>
        {
            entity.HasKey(e => e.ClientModuleMappingId).HasName("PK_ClientSettings");

            entity.ToTable("ClientModuleMapping");

            entity.HasOne(d => d.Client).WithMany(p => p.ClientModuleMappings)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientModuleMapping_ClientMaster");
        });

        modelBuilder.Entity<ClientSetting>(entity =>
        {
            entity.HasKey(e => e.ClientSettingId);

            entity.ToTable("ClientSetting");

            entity.Property(e => e.ClientLogo).HasMaxLength(100);
            entity.Property(e => e.ColorTheme).HasMaxLength(100);
            entity.Property(e => e.DocumentLink).HasMaxLength(100);
            entity.Property(e => e.FooterText).HasMaxLength(500);
            entity.Property(e => e.HeaderText).HasMaxLength(50);
            entity.Property(e => e.MenuStyle)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValueSql("('V')")
                .IsFixedLength();
            entity.Property(e => e.PoliciesLink).HasMaxLength(100);
            entity.Property(e => e.SupportLink).HasMaxLength(100);
            entity.Property(e => e.ModuleIds).HasMaxLength(100);
            entity.Property(e => e.IDTypes).HasMaxLength(100);
            entity.Property(e => e.ProfileImage)
               .HasMaxLength(10)
               .IsUnicode(false);
        });

        modelBuilder.Entity<ColorTheme>(entity =>
        {
            entity.HasKey(e => e.ColorId);

            entity.Property(e => e.ColorTheme1)
                .HasMaxLength(50)
                .HasColumnName("ColorTheme");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<CountryMaster>(entity =>
        {
            entity.HasKey(e => e.CountryId);

            entity.ToTable("CountryMaster");

            entity.Property(e => e.CountryName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<DepartmentMaster>(entity =>
        {
            entity.HasKey(e => e.DepartmentId);

            entity.ToTable("DepartmentMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DepartmentCode).HasMaxLength(10);
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<DistrictMaster>(entity =>
        {
            entity.HasKey(e => e.DistrictId);

            entity.ToTable("DistrictMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DistrictName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Country).WithMany(p => p.DistrictMasters)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DistrictMaster_CountryMaster");

            //entity.HasOne(d => d.State).WithMany(p => p.DistrictMasters)
            //    .HasForeignKey(d => d.StateId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_DistrictMaster_StateMaster");
        });

        //modelBuilder.Entity<EmployeEductionDetail>(entity =>
        //{
        //    entity.HasKey(e => e.EducationD).HasName("PK_EmployeEductionDetails");

        //    entity.ToTable("EmployeEductionDetails#");

        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        //    entity.Property(e => e.EducationName).HasMaxLength(100);
        //    entity.Property(e => e.EducationType).HasMaxLength(100);
        //    entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
        //    entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
        //    entity.Property(e => e.ModifedBy)
        //        .HasMaxLength(50)
        //        .HasColumnName("modifedBy");
        //    entity.Property(e => e.ModifiedOn)
        //        .HasColumnType("datetime")
        //        .HasColumnName("modifiedOn");
        //    entity.Property(e => e.UniversityName).HasMaxLength(100);

        //    entity.HasOne(d => d.Employee).WithMany(p => p.EmployeEductionDetails)
        //        .HasForeignKey(d => d.EmployeeId)
        //        .HasConstraintName("FK_EmployeEductionDetails_EmployeePersonalDetails");
        //});

        modelBuilder.Entity<EmployeeAcademicDetail>(entity =>
        {
            entity.HasKey(e => e.AcademicDetailId).HasName("PK_EmployeeAcademicDetails");

            entity.ToTable("EmployeeAcademicDetail");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");


            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeAcademicDetails)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeA__Emplo__1F63A897");
        });

        modelBuilder.Entity<EmployeeBankDetail>(entity =>
        {
            entity.HasKey(e => e.BankDetailId).HasName("PK_EmployeeBankDetails");

            entity.ToTable("EmployeeBankDetail");
            entity.Property(e => e.AccountNo).HasMaxLength(50);
            entity.Property(e => e.BankId).HasMaxLength(50);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IFSCCode)
                .HasMaxLength(100)
                .HasColumnName("IFSCCode");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeBankDetails)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeB__Emplo__2057CCD0");
        });

        modelBuilder.Entity<EmployeeCertificationDetail>(entity =>
        {
            entity.HasKey(e => e.CertificationDetailId);

            entity.ToTable("EmployeeCertificationDetail");

            entity.Property(e => e.CertificationName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Duration).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeCertificationDetails)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeC__Emplo__214BF109");
        });

        modelBuilder.Entity<EmployeeContactDetail>(entity =>
        {
            entity.ToTable("EmployeeContactDetail");

            entity.Property(e => e.Address1).HasMaxLength(200);
            entity.Property(e => e.Address2).HasMaxLength(200);
            entity.Property(e => e.ContactNum).HasMaxLength(10);
            entity.Property(e => e.ContactType).HasMaxLength(10);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Landmark).HasMaxLength(200);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Pincode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeContactDetails)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeC__Emplo__24285DB4");
        });

        modelBuilder.Entity<EmployeeExperienceDetail>(entity =>
        {
            entity.HasKey(e => e.ExperienceDetailId);

            entity.ToTable("EmployeeExperienceDetail");

            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.LastWorkingDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeExperienceDetails)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeE__Emplo__251C81ED");
        });

        modelBuilder.Entity<EmployeeFamilyDetail>(entity =>
        {
            entity.HasKey(e => e.EmployeeFamilyDetailId);

            entity.ToTable("EmployeeFamilyDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MemberDob)
                .HasColumnType("datetime")
                .HasColumnName("MemberDob");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.MemberName).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Relationship).HasMaxLength(50);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeFamilyDetails)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeF__Emplo__2F9A1060");
        });

        modelBuilder.Entity<EmployeeLeaveHistory>(entity =>
        {
            entity.HasKey(e => e.LeaveId);

            entity.ToTable("EmployeeLeaveHistory");

            entity.Property(e => e.OpeningBalance).HasColumnName("OpeningBalance");
            entity.Property(e => e.Used).HasColumnName("Used");
            entity.Property(e => e.ClosingBalance).HasColumnName("ClosingBalance");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeId");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LeaveReason).HasMaxLength(500);
            entity.Property(e => e.LeaveStatus).HasDefaultValueSql("((1))");
            entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLeaveHistories)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeL__Emplo__27F8EE98");
        });

        modelBuilder.Entity<EmployeeMaster>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);

            entity.ToTable("EmployeeMaster");

            entity.Property(e => e.AadharNumber).HasMaxLength(12);
            entity.Property(e => e.AnnualBasicSalary).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.AnnualCtc)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("AnnualCTC");
            entity.Property(e => e.BasicSalary).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.ContactNo)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Doj)
                .HasColumnType("datetime")
                .HasColumnName("DOJ");
            entity.Property(e => e.DOC)
                .HasColumnType("datetime")
                .HasColumnName("DOC");
            entity.Property(e => e.EmailId).HasMaxLength(50);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(252)
                .HasComputedColumnSql("((((isnull([FirstName],'')+' ')+isnull([MiddleName],''))+' ')+isnull([LastName],''))", false);
            entity.Property(e => e.EmployeeStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FatherName).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GenderId).HasColumnName("GenderID");
            entity.Property(e => e.IdentityId).HasColumnName("IdentityID");
            entity.Property(e => e.InfoFillingStatus).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("IsActive");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.MonthlyCtc)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("MonthlyCTC");
            entity.Property(e => e.OfficialEmail).HasMaxLength(100);
            entity.Property(e => e.OtherCompensation).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Pannumber)
                .HasMaxLength(50)
                .HasColumnName("PANNumber");
            entity.Property(e => e.ProfileImage)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SalaryInHand).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.SpouseName)
                .HasMaxLength(10)
                .IsFixedLength();

            //entity.HasOne(d => d.Department)
            //.HasForeignKey(d => d.DepartmentId);
            //.HasConstraintName("FK__Department_EM__27F8EE98");

        });

        //modelBuilder.Entity<EmployeePersonalDetail>(entity =>
        //{
        //    entity.HasKey(e => e.Empid).HasName("PK_EmployeePersonalDetails");

        //    entity.ToTable("EmployeePersonalDetails#");

        //    entity.Property(e => e.Empid).HasColumnName("EMPID");
        //    entity.Property(e => e.AadharCard).HasMaxLength(20);
        //    entity.Property(e => e.AccountName).HasMaxLength(50);
        //    entity.Property(e => e.AccountNumber).HasMaxLength(50);
        //    entity.Property(e => e.BandId).HasColumnName("BandID");
        //    entity.Property(e => e.BankId).HasColumnName("BankID");
        //    entity.Property(e => e.BloodGroupId).HasColumnName("BloodGroupID");
        //    entity.Property(e => e.ClientId).HasColumnName("ClientID");
        //    entity.Property(e => e.ContactNumber).HasMaxLength(10);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        //    entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
        //    entity.Property(e => e.Dob)
        //        .HasColumnType("date")
        //        .HasColumnName("DOB");
        //    entity.Property(e => e.Doj)
        //        .HasColumnType("date")
        //        .HasColumnName("DOJ");
        //    entity.Property(e => e.EmailId)
        //        .HasMaxLength(100)
        //        .HasColumnName("EmailID");
        //    entity.Property(e => e.EmergencyContactNumber).HasMaxLength(20);
        //    entity.Property(e => e.Empcode)
        //        .HasMaxLength(10)
        //        .HasColumnName("EMPCode");
        //    entity.Property(e => e.EmployementStatusId).HasColumnName("EmployementStatusID");
        //    entity.Property(e => e.EmploymentTypeId).HasColumnName("EmploymentTypeID");
        //    entity.Property(e => e.FatherName).HasMaxLength(100);
        //    entity.Property(e => e.FirstName).HasMaxLength(50);
        //    entity.Property(e => e.GenderId).HasColumnName("GenderID");
        //    entity.Property(e => e.HrmanagerId).HasColumnName("HRManagerID");
        //    entity.Property(e => e.Ifsccode)
        //        .HasMaxLength(10)
        //        .HasColumnName("IFSCCode");
        //    entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
        //    entity.Property(e => e.JobTitleId).HasColumnName("JobTitleID");
        //    entity.Property(e => e.LastName).HasMaxLength(50);
        //    entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
        //    entity.Property(e => e.MaritalId).HasColumnName("MaritalID");
        //    entity.Property(e => e.MiddleName).HasMaxLength(50);
        //    entity.Property(e => e.ModifedBy)
        //        .HasMaxLength(50)
        //        .HasColumnName("modifedBy");
        //    entity.Property(e => e.ModifiedOn)
        //        .HasColumnType("datetime")
        //        .HasColumnName("modifiedOn");
        //    entity.Property(e => e.PanCard).HasMaxLength(10);
        //    entity.Property(e => e.PermanentCityId).HasColumnName("PermanentCityID");
        //    entity.Property(e => e.PermanentCountryId).HasColumnName("PermanentCountryID");
        //    entity.Property(e => e.PermanentDistrictId).HasColumnName("PermanentDistrictID");
        //    entity.Property(e => e.PermanentStateId).HasColumnName("PermanentStateID");
        //    entity.Property(e => e.PresentCityId).HasColumnName("PresentCityID");
        //    entity.Property(e => e.PresentCountryId).HasColumnName("PresentCountryID");
        //    entity.Property(e => e.PresentDistrictId).HasColumnName("PresentDistrictID");
        //    entity.Property(e => e.PresentStateId).HasColumnName("PresentStateID");
        //    entity.Property(e => e.ReligionId).HasColumnName("ReligionID");
        //    entity.Property(e => e.SpouseName).HasMaxLength(100);
        //    entity.Property(e => e.WorkEmailId)
        //        .HasMaxLength(100)
        //        .HasColumnName("WorkEmailID");
        //    entity.Property(e => e.WorkLocationId).HasColumnName("WorkLocationID");

        //    entity.HasOne(d => d.Band).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.BandId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_BandMaster");

        //    entity.HasOne(d => d.Bank).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.BankId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_BankMaster");

        //    entity.HasOne(d => d.BloodGroup).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.BloodGroupId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_BloodGroupMaster");

        //    entity.HasOne(d => d.Client).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.ClientId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_ClientMaster");

        //    entity.HasOne(d => d.Department).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.DepartmentId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_DepartmentMaster");

        //    entity.HasOne(d => d.JobTitle).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.JobTitleId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_JobTitleMaster");

        //    entity.HasOne(d => d.Marital).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.MaritalId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_MaritalStatusMaster");

        //    entity.HasOne(d => d.PermanentCity).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.PermanentCityId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_CityMaster");

        //    entity.HasOne(d => d.PermanentCountry).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.PermanentCountryId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_CountryMaster");

        //    entity.HasOne(d => d.PermanentState).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.PermanentStateId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_StateMaster");

        //    entity.HasOne(d => d.Religion).WithMany(p => p.EmployeePersonalDetails)
        //        .HasForeignKey(d => d.ReligionId)
        //        .HasConstraintName("FK_EmployeePersonalDetails_ReligionMaster");
        //});

        modelBuilder.Entity<EmployeeReferenceDetail>(entity =>
        {
            entity.HasKey(e => e.EmployeeReferenceId);

            entity.ToTable("EmployeeReferenceDetail");

            entity.Property(e => e.ReferenceContactNo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ReferenceDesignation).HasMaxLength(50);
            entity.Property(e => e.HowYouKnow).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ReferenceMobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PersonName).HasMaxLength(100);
            entity.Property(e => e.PresentCompany).HasMaxLength(250);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeReferenceDetails)
                .HasForeignKey(d => d.ReferenceOf)
                .HasConstraintName("FK__EmployeeR__Emplo__4DE98D56");
        });


        modelBuilder.Entity<EmployeeUploadDocument>(entity =>
        {
            entity.HasKey(e => e.UploadDcumentDetailId);

            entity.ToTable("EmployeeUploadDocument");
            entity.Property(e => e.DocumentType).HasMaxLength(20);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DcumentTypeId);
            entity.Property(e => e.EmployeeDocument).HasMaxLength(20);
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("isActive");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeUploadDocuments)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__EmployeeU__Emplo__28ED12D1");
        });

        modelBuilder.Entity<EpfemployeeMapping>(entity =>
        {
            entity.HasKey(e => new { e.StatutoryComponentsId, e.EmployeeId });

            entity.ToTable("EPFEmployeeMapping");
        });

        modelBuilder.Entity<HolidaysListMaster>(entity =>
        {
            entity.HasKey(e => e.HolidayId);

            entity.ToTable("HolidaysListMaster");

            entity.Property(e => e.HolidayId).HasColumnName("HolidayID");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.HolidayDate).HasColumnType("datetime");
            entity.Property(e => e.HolidayName).HasMaxLength(100);
            entity.Property(e => e.HolidayType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });


        modelBuilder.Entity<UnitHolidayList>(entity =>
        {

            entity.HasKey(e => e.UnitHolidayId);
            entity.ToTable("UnitHolidayList");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.HolidayDate).HasColumnType("datetime");
            entity.Property(e => e.HolidayId).HasColumnName("HolidayID");
            entity.Property(e => e.HolidayName).HasMaxLength(100);
            entity.Property(e => e.HolidayType).HasMaxLength(50);
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.UnitHolidayId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<IdtypeMaster>(entity =>
        {
            entity.HasKey(e => e.IdentityId);

            entity.ToTable("IDTypeMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IdentityCode).HasMaxLength(10);
            entity.Property(e => e.IdentityName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<JobTitleMaster>(entity =>
        {
            entity.HasKey(e => e.JobTitleId);

            entity.ToTable("JobTitleMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.JobTitle).HasMaxLength(50);
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<LeaveTypeMaster>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId);

            entity.ToTable("LeaveTypeMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.LeaveType).HasMaxLength(50);
            entity.Property(e => e.ApplicableFor).HasMaxLength(5);
            entity.Property(e => e.IsCompOff).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsPaidLeave).HasDefaultValueSql("((0))");
            entity.Property(e => e.LeaveTypeCode).HasMaxLength(5);
            entity.Property(e => e.MaxLeaveRange).HasColumnName("MaxLeaveRange");
            entity.Property(e => e.MinLeaveRange).HasColumnName("MinLeaveRange");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        //modelBuilder.Entity<LoginDetail>(entity =>
        //{
        //    entity.HasKey(e => e.LoginId);

        //    entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
        //    entity.Property(e => e.Password).HasMaxLength(500);
        //    entity.Property(e => e.UserName).HasMaxLength(50);
        //});
        modelBuilder.Entity<LoginDetail>(entity =>
        {
            entity.HasKey(e => e.LoginId);

            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.UserName).HasMaxLength(50);

            //entity.HasOne(d => d.Client).WithMany(p => p.LoginDetails)
            //    .HasForeignKey(d => d.ClientId)
            //    .HasConstraintName("FK_LoginDetailsClient");
        });

        modelBuilder.Entity<MaritalStatusMaster>(entity =>
        {
            entity.HasKey(e => e.MaritalStatusId);

            entity.ToTable("MaritalStatusMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.MaritalStatusName).HasMaxLength(100);
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<ModuleMaster>(entity =>
        {
            entity.HasKey(e => e.ModuleId);

            entity.ToTable("ModuleMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.ModuleName).HasMaxLength(200);
            entity.Property(e => e.ModuleShortName).HasMaxLength(100);
        });

        modelBuilder.Entity<ReligionMaster>(entity =>
        {
            entity.HasKey(e => e.ReligionId);

            entity.ToTable("ReligionMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.ReligionName).HasMaxLength(100);
        });

        modelBuilder.Entity<ResourceMaster>(entity =>
        {
            entity.HasKey(e => e.ResourceId);

            entity.ToTable("ResourceMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.ResourceDesc).HasMaxLength(500);
            entity.Property(e => e.ResourceName).HasMaxLength(100);
        });

        modelBuilder.Entity<RoleMaster>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("RoleMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy)
                .HasMaxLength(50)
                .HasColumnName("modifedBy");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modifiedOn");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<SalaryComponentMaster>(entity =>
        {
            entity.HasKey(e => e.SalaryComponentId);

            entity.ToTable("SalaryComponentMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy)
                .HasMaxLength(50)
                .HasColumnName("modifedBy");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modifiedOn");
            entity.Property(e => e.SalaryComponentDec).HasMaxLength(500);
            entity.Property(e => e.SalaryComponentTitle).HasMaxLength(50);
        });

        modelBuilder.Entity<ShiftMaster>(entity =>
        {
            entity.HasKey(e => e.ShiftId);

            entity.ToTable("ShiftMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.ShiftCode)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.ShiftName).HasMaxLength(100);
            //entity.Property(e => e.BufferOfInTime).HasColumnName("BufferOfInTime");
            //entity.Property(e => e.BufferOfOutTime).HasColumnName("BufferOfOutTime");
            //entity.Property(e => e.NoOfLateAllowed).HasColumnName("NoOfLateAllowed");
            entity.Property(e => e.IncludeBefore).HasColumnName("IncludeBefore");
            entity.Property(e => e.IncludeAfter).HasColumnName("IncludeAfter");
            //entity.Property(e => e.PolicyId).HasColumnName("PolicyId");
            entity.Property(e => e.UnitId).HasColumnName("UnitId");
            entity.Property(e => e.IsNightShift).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsFlexi).HasDefaultValueSql("((0))");
            entity.Property(e => e.isBufferTimeAllowed).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<StateMaster>(entity =>
        {
            entity.HasKey(e => e.StateId);

            entity.ToTable("StateMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.StateName).HasMaxLength(100);

            entity.HasOne(d => d.Country).WithMany(p => p.StateMasters)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StateMaster_CountryMaster");
        });

        modelBuilder.Entity<WorkLocationMaster>(entity =>
        {
            entity.HasKey(e => e.WorkLocationId);

            entity.ToTable("WorkLocationMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });
        modelBuilder.Entity<MenuMaster>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__MenuMast__C99ED230AE12EA16");

            entity.ToTable("MenuMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Icon)
                .HasMaxLength(100)
                .HasColumnName("icon");
            entity.Property(e => e.MenuName).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PageLink).HasMaxLength(200);
            entity.Property(e => e.Sn).HasColumnName("SN");
            entity.Property(e => e.IsHeading).HasColumnName("IsHeading");

            entity.HasOne(d => d.Module).WithMany(p => p.MenuMasters)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_MenuMasterModuleMaster");
        });
        modelBuilder.Entity<RoleMenuMapping>(entity =>
        {
            entity.HasKey(e => e.RoleMenuMappingId).HasName("PK__RoleMenu__B804E01DC92CCABF");

            entity.ToTable("RoleMenuMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            //entity.Property(e => e.ClientId).hasc("int");

            entity.HasOne(d => d.JobTitle).WithMany(p => p.RoleMenuMappings)
                .HasForeignKey(d => d.JobTitleId)
                .HasConstraintName("FK_RoleMenuMappingJobTitleMaster");

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenuMappings)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK_RoleMenuMappingMenuMaster");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenuMappings)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RoleMenuMappingRoleMaster");

            entity.HasOne(d => d.Department).WithMany(p => p.RoleMenuMappings)
               .HasForeignKey(d => d.DepartmentId)
               .HasConstraintName("FK_RoleMenuMappingDepartmentMaster");
        });


        modelBuilder.Entity<AttendanceSetting>(entity =>
        {
            entity.HasKey(e => e.AttendanceSettingId);

            entity.ToTable("AttendanceSetting");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.MaximumTime).HasColumnName("MaximumTime");
            entity.Property(e => e.MaximumTime).HasColumnName("MaximumTime");
            entity.Property(e => e.ShiftId).HasColumnName("ShiftId");
            entity.Property(e => e.UnitId).HasColumnName("UnitId");
            entity.Property(e => e.LegendType).HasColumnName("LegendType");
        });


        modelBuilder.Entity<WorkFlowSettings>(entity =>
        {
            entity.HasKey(e => e.WorkFlowSettingsId);

            entity.ToTable("WorkFlowSettings");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            //  entity.Property(e => e.LevelId1).HasColumnName("LevelId1");
            // entity.Property(e => e.LevelId2).HasColumnName("LevelId2");
            // entity.Property(e => e.LevelId3).HasColumnName("LevelId3");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ModuleId).HasColumnName("ModuleId");
            entity.Property(e => e.UnitId).HasColumnName("UnitId");
            entity.Property(e => e.Authority1).HasColumnName("Authority1");
            entity.Property(e => e.Authority2).HasColumnName("Authority2");
            entity.Property(e => e.Authority3).HasColumnName("Authority3");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<UnitMaster>(entity =>
        {
            entity.HasKey(e => e.UnitID);

            entity.ToTable("UnitMaster");

            entity.Property(e => e.Address).HasMaxLength(1000);
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .HasMaxLength(100)
                .HasColumnName("EmailID");
            entity.Property(e => e.GSTN)
                .HasMaxLength(50)
                .HasColumnName("GSTN");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.StateId).HasColumnName("StateID");
            entity.Property(e => e.UnitName).HasMaxLength(500);
            entity.Property(e => e.ClientId).HasColumnName("ClientId");
            entity.Property(e => e.PanCard).HasMaxLength(10);
            entity.Property(e => e.TIN).HasMaxLength(50);
            entity.Property(e => e.NoticePeriod).HasColumnName("NoticePeriod");
            entity.Property(e => e.ConfirmationPeriod).HasColumnName("ConfirmationPeriod");
        });

        modelBuilder.Entity<EmployeeUnitsMapping>(entity =>
        {
            entity.HasKey(e => e.EmployeeUnitID);

            entity.ToTable("EmployeeUnitsMapping");

            entity.Property(e => e.ClientID).HasColumnName("ClientID");
            entity.Property(e => e.UnitID).HasColumnName("UnitID");
            entity.Property(e => e.EmployeeUnitID).HasColumnName("EmployeeUnitID");

        });

        modelBuilder.Entity<PolicyDocumentsCategoryMaster>(entity =>
        {
            entity.HasKey(e => e.PolicyDocumentsCategoryId).HasName("PK__PolicyDo__0B624D547114CC40");

            entity.ToTable("PolicyDocumentsCategoryMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PolicyDocumentsCategory).HasMaxLength(100);
        });

        modelBuilder.Entity<PolicyDocumentsSubCategoryMaster>(entity =>
        {
            entity.HasKey(e => e.PolicyDocumentsSubCategoryId).HasName("PK__PolicyDo__BFA09BF3AACB0B20");

            entity.ToTable("PolicyDocumentsSubCategoryMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PolicyDocumentsSubCategory).HasMaxLength(100);

            entity.HasOne(d => d.PolicyDocumentsCategory).WithMany(p => p.PolicyDocumentsSubCategoryMasters)
                .HasForeignKey(d => d.PolicyDocumentsCategoryId)
                .HasConstraintName("FK_PolicyDocumentsSubCategoryMasterPolicyDocumentsCategoryMaster");
        });
        modelBuilder.Entity<PolicyDocumentsMaster>(entity =>
        {
            entity.HasKey(e => e.PolicyDocumentsMasterId).HasName("PK__PolicyDo__C403CAA4C5844BEA");

            entity.ToTable("PolicyDocumentsMaster");

            entity.Property(e => e.PolicyDocumentsMasterId).HasColumnName("PolicyDocumentsMasterId");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PolicyDocument).HasMaxLength(100);
            entity.Property(e => e.PolicyDocumentPath).HasMaxLength(200);

            entity.HasOne(d => d.PolicyDocumentsCategory).WithMany(p => p.PolicyDocumentsMasters)
                .HasForeignKey(d => d.PolicyDocumentsCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PolicyDocumentsMasterPolicyDocumentsCategoryMaster");

            entity.HasOne(d => d.PolicyDocumentsSubCategory).WithMany(p => p.PolicyDocumentsMasters)
                .HasForeignKey(d => d.PolicyDocumentsSubCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PolicyDocumentsMasterPolicyDocumentsSubCategoryMaster");
        });
        modelBuilder.Entity<EmployeePolicyAcceptance>(entity =>
        {
            entity.HasKey(e => e.EmployeePolicyAcceptanceId).HasName("PK__Employee__41C791E18B10D854");

            entity.ToTable("EmployeePolicyAcceptance");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePolicyAcceptances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePolicyAcceptanceEmployeeMasterEmployeeId");

            entity.HasOne(d => d.PolicyDocumentsMaster).WithMany(p => p.EmployeePolicyAcceptances)
                .HasForeignKey(d => d.PolicyDocumentsMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePolicyAcceptancePolicyDocumentsMasterPolicyDocumentsMasterId");
        });

        modelBuilder.Entity<LeaveCalenderYear>(entity =>
        {
            entity.HasKey(e => e.LeaveYearId);

            entity.ToTable("LeaveCalenderYear");

            entity.Property(e => e.CalendarName).HasColumnName("CalendarName");
            entity.Property(e => e.UnitId).HasColumnName("UnitId");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
        });
        modelBuilder.Entity<LeaveAttribute>(entity =>
        {
            entity.HasKey(e => e.LeaveAttributeId).HasName("PK__LeaveAtt__3ACFAA2860DA3428");

            entity.Property(e => e.AccuralType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.CreditLimit)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Ismternity).HasColumnName("ISMternity");
            entity.Property(e => e.Ispaternity).HasColumnName("ISPaternity");

            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<AttendanceLateSetting>(entity =>
        {
            entity.HasKey(e => e.LateMasterId);

            entity.ToTable("AttendanceLateMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LateDuration).HasColumnName("LateDuration");
            entity.Property(e => e.NoOfLate).HasColumnName("NoOfLate");
            entity.Property(e => e.AppliedOn).HasColumnType("AppliedOn");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.CanDeductLeave).HasDefaultValueSql("((1))");
            entity.Property(e => e.Refill).HasDefaultValueSql("((1))");
            entity.Property(e => e.ShowPostLimit).HasColumnName("ShowPostLimit");
            entity.Property(e => e.ActionPostLate).HasColumnName("ActionPostLate");
            entity.Property(e => e.UnitId).HasColumnName("UnitId");
        });

        modelBuilder.Entity<EmployeeLeaveDetails>(entity =>
        {
            entity.HasKey(e => e.LeaveDetailsId);

            entity.ToTable("EmployeeLeaveDetails");

            entity.Property(e => e.NoOfLeave).HasColumnName("NoOfLeave");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeId");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsBillRequired).HasColumnName("IsBillRequired");
            entity.Property(e => e.BillName).HasColumnName("BillName");
            entity.Property(e => e.LeaveStatus).HasDefaultValueSql("((1))");
            entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");
            entity.Property(e => e.ApprovedBy).HasMaxLength(50);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");

            //entity.HasOne(d => d.Employee).WithMany(p => p.)
            //    .HasForeignKey(d => d.EmployeeId)
            //    .HasConstraintName("FK__EmployeeL__Emplo__27F8EE98");
        });

        modelBuilder.Entity<EmployeeLeaveBalance>(entity =>
        {
            entity.HasKey(e => e.LeaveBalanceId);

            entity.ToTable("EmployeeLeaveBalance");

            entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeId");
            entity.Property(e => e.LeaveTypeCode).HasColumnName("LeaveTypeCode");
            entity.Property(e => e.LeaveBalance).HasColumnName("LeaveBalance");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeId");
            entity.Property(e => e.CalenderYearId).HasColumnName("CalenderYearId");
            entity.Property(e => e.OpeningBalance).HasColumnName("OpeningBalance");
            entity.Property(e => e.LeaveBalance).HasColumnName("LeaveBalance");
            entity.Property(e => e.TotalApplied).HasColumnName("TotalApplied");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");



        });
        modelBuilder.Entity<StatutoryComponent_EPF>(entity =>
        {
            entity.HasKey(e => e.StatutoryComponentsId).HasName("PK__Statutor__53BFB08E5EFA74EB");

            entity.Property(e => e.CreatedBy).HasMaxLength(10);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeductionCycle).HasMaxLength(20);
            entity.Property(e => e.Epfnumber)
                .HasMaxLength(20)
                .HasColumnName("EPFNumber");
            entity.Property(e => e.IsCtcinclusionAdminCharges).HasColumnName("IsCTCInclusionAdminCharges");
            entity.Property(e => e.IsCtcinclusionEmployers).HasColumnName("IsCTCInclusionEmployers");
            entity.Property(e => e.IsCtcinclusionEmployersEdli).HasColumnName("IsCTCInclusionEmployersEDLI");
            entity.Property(e => e.IsLopbasedComponentSalary).HasColumnName("IsLOPBasedComponentSalary");
            entity.Property(e => e.IsProrateRestrictedPfwage).HasColumnName("IsProrateRestrictedPFWage");
            entity.Property(e => e.ModifiedBy).HasMaxLength(10);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<StatutoryComponentsEsi>(entity =>
        {
            entity.HasKey(e => e.StatutoryComponentsEsiid).HasName("PK__Statutor__4BEF2F869FF9DD60");

            entity.ToTable("StatutoryComponents_ESI");

            entity.Property(e => e.StatutoryComponentsEsiid).HasColumnName("StatutoryComponents_ESIId");
            entity.Property(e => e.CreatedBy).HasMaxLength(20);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeductionCycle).HasMaxLength(20);
            entity.Property(e => e.Esilimit).HasColumnName("ESILimit");
            entity.Property(e => e.Esinumber)
                .HasMaxLength(20)
                .HasColumnName("ESINumber");
            entity.Property(e => e.IsEmployersContibutionInCtc).HasColumnName("IsEmployersContibutionInCTC");
            entity.Property(e => e.ModifiedBy).HasMaxLength(20);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<StatutoryComponentsLabourWelfareFund>(entity =>
        {
            entity.HasKey(e => e.StatutoryComponentsLabourWelfareFundId).HasName("PK__Statutor__94ADBA30B3B583D6");

            entity.ToTable("StatutoryComponents_LabourWelfareFund");

            entity.Property(e => e.StatutoryComponentsLabourWelfareFundId).HasColumnName("StatutoryComponents_LabourWelfareFundId");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeductionCycle).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<SalaryTemplate>(entity =>
        {
            entity.HasKey(e => e.SalaryTemplateId).HasName("PK__SalaryTe__EF7FA8BF0DB663EB");

            entity.Property(e => e.AnnualCtc).HasColumnName("AnnualCTC");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TemplateName).HasMaxLength(100);
        });

        modelBuilder.Entity<SalaryTemplateComponentsMapping>(entity =>
        {
            entity.HasKey(e => e.SalaryTemplateComponentsMappingId).HasName("PK__SalaryTe__0C9E51138AFE0081");

            entity.ToTable("SalaryTemplateComponentsMapping");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<ItDeclaration80Cinvestment>(entity =>
        {
            entity.HasKey(e => e.ItDeclaration80CinvestmentsId).HasName("PK__ItDeclar__8E8222475284D82F");

            entity.ToTable("ItDeclaration80CInvestments");

            entity.Property(e => e.ItDeclaration80CinvestmentsId).HasColumnName("ItDeclaration80CInvestmentsId");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProofFileExtension).HasMaxLength(150);
            entity.Property(e => e.ProofFileName).HasMaxLength(150);
        });

        modelBuilder.Entity<ItDeclaration80Dexemption>(entity =>
        {
            entity.HasKey(e => e.ItDeclaration80DexemptionsId).HasName("PK__ItDeclar__F7C32214454B9DD8");

            entity.ToTable("ItDeclaration80DExemptions");

            entity.Property(e => e.ItDeclaration80DexemptionsId).HasColumnName("ItDeclaration80DExemptionsId");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProofFileExtension).HasMaxLength(150);
            entity.Property(e => e.ProofFileName).HasMaxLength(150);
        });

        modelBuilder.Entity<ItDeclarationHomeLoanDetail>(entity =>
        {
            entity.HasKey(e => e.ItDeclarationHomeLoanDetailsId).HasName("PK__ItDeclar__4F71009C011D12F0");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.LenderPancard).HasMaxLength(20);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.NameOfTheLender).HasMaxLength(100);
            entity.Property(e => e.ProofFileExtension).HasMaxLength(150);
            entity.Property(e => e.ProofFileName).HasMaxLength(150);
        });

        modelBuilder.Entity<ItDeclarationHouseRentDetail>(entity =>
        {
            entity.HasKey(e => e.ItDeclarationHouseRentDetailsId).HasName("PK__ItDeclar__48CC0B94522BA1DD");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.HouseAddress).HasMaxLength(255);
            entity.Property(e => e.LandlordName).HasMaxLength(100);
            entity.Property(e => e.LandlordPancard).HasMaxLength(20);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.MonthFrom).HasMaxLength(50);
            entity.Property(e => e.MonthTo).HasMaxLength(50);
            entity.Property(e => e.ProofFileExtension).HasMaxLength(15);
            entity.Property(e => e.ProofFileName).HasMaxLength(150);
            entity.Property(e => e.StayingIn).HasMaxLength(50);
            entity.Property(e => e.YearFrom).HasMaxLength(4);
            entity.Property(e => e.YearTo).HasMaxLength(4);
        });

        modelBuilder.Entity<ItDeclarationLentOutPropertyDetail>(entity =>
        {
            entity.HasKey(e => e.ItDeclarationLentOutPropertyDetailsId).HasName("PK__ItDeclar__9C65489B522EEB14");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProofFileExtension).HasMaxLength(150);
            entity.Property(e => e.ProofFileName).HasMaxLength(150);
        });

        modelBuilder.Entity<ItDeclarationOtherSourceOfIncome>(entity =>
        {
            entity.HasKey(e => e.ItDeclarationOtherSourceOfIncomeId).HasName("PK__ItDeclar__B0F4C8FEB85B5864");

            entity.ToTable("ItDeclarationOtherSourceOfIncome");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProofFileExtension).HasMaxLength(150);
            entity.Property(e => e.ProofFileName).HasMaxLength(150);
        });

        modelBuilder.Entity<ItDeclarationPreviousEmployement>(entity =>
        {
            entity.HasKey(e => e.ItDeclarationPreviousEmployementId).HasName("PK__ItDeclar__28E29C32851DDF29");

            entity.ToTable("ItDeclarationPreviousEmployement");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProofFileExtension).HasMaxLength(150);
            entity.Property(e => e.ProofFileName).HasMaxLength(150);
        });

        modelBuilder.Entity<ItDeclarationOtherInvestmentExemption>(entity =>
        {
            entity.HasKey(e => e.ItDeclarationOtherInvestmentExemptionId).HasName("PK__ItDeclar__EC0C5D56D188ED73");

            entity.ToTable("ItDeclarationOtherInvestmentExemption");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProofFileExtension).HasMaxLength(150);
            entity.Property(e => e.ProofFileName).HasMaxLength(150);
        });
        modelBuilder.Entity<Investment80Cmaster>(entity =>
        {
            entity.HasKey(e => e.Investment80Cid).HasName("PK__Investme__2994E3E0E45F9694");

            entity.ToTable("Investment80CMaster");

            entity.Property(e => e.Investment80Cid).HasColumnName("Investment80CId");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Investment80C).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Optgroup)
                .HasMaxLength(50)
                .HasColumnName("optgroup");
        });
        modelBuilder.Entity<Exemptions80D>(entity =>
        {
            entity.HasKey(e => e.Exemptions80Did).HasName("PK__Exemptio__0F770526048608C4");

            entity.ToTable("Exemptions80D");

            entity.Property(e => e.Exemptions80Did).HasColumnName("Exemptions80DId");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Exemptions80D1)
                .HasMaxLength(100)
                .HasColumnName("Exemptions80D");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Optgroup)
                .HasMaxLength(50)
                .HasColumnName("optgroup");
        });
        modelBuilder.Entity<OtherInvestmentExemption>(entity =>
        {
            entity.HasKey(e => e.OtherInvestmentExemptionId).HasName("PK__OtherInv__FCBFB9395BCB17B6");

            entity.ToTable("OtherInvestmentExemption");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Optgroup)
                .HasMaxLength(50)
                .HasColumnName("optgroup");
            entity.Property(e => e.OtherInvestmentExemption1)
                .HasMaxLength(100)
                .HasColumnName("OtherInvestmentExemption");
        });

        modelBuilder.Entity<EmployeesSalaryDetails>(entity =>
        {
            entity.HasKey(e => e.EmployeeSalaryId);

            entity.ToTable("EmployeesSalaryDetails");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeId");
            entity.Property(e => e.UnitId).HasColumnName("UnitId");
            entity.Property(e => e.SalaryComponentId).HasColumnName("SalaryComponentId");
            entity.Property(e => e.SalaryMonth).HasColumnName("SalaryMonth");
            entity.Property(e => e.SalaryComponentType).HasColumnName("SalaryComponentType");
            entity.Property(e => e.ComponentName).HasMaxLength(500);
            entity.Property(e => e.CalculationType).HasMaxLength(5);
            entity.Property(e => e.AmtPerMonth).HasColumnName("AmtPerMonth");
            entity.Property(e => e.PerVal).HasColumnName("PerVal");
            entity.Property(e => e.Processdate).HasColumnType("datetime");
        });
        modelBuilder.Entity<EmployeeExitResignation>(entity =>
        {
            entity.HasKey(e => e.ResignationListId).HasName("PK__Resignat__020CE33A5C376172");

            entity.ToTable("EmployeeExitResignation");

            entity.Property(e => e.CreationDateEmployee).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeComments).HasMaxLength(255);
            entity.Property(e => e.LastWorkingDate).HasColumnType("datetime");
            entity.Property(e => e.ReasonForLeaving).HasMaxLength(100);
            entity.Property(e => e.ResignationDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ItDeclarationType>(entity =>
        {
            entity.HasKey(e => e.ItDeclarationTypeId).HasName("PK__ItDeclar__216757B71E9B301B");

            entity.ToTable("ItDeclarationType");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.Fy)
                .HasMaxLength(10)
                .HasColumnName("FY");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RegimeType).HasMaxLength(10);
        });

        modelBuilder.Entity<PageControlKeyValue>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.ControlName).HasMaxLength(50);
            entity.Property(e => e.KeyName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyValue).HasMaxLength(100);
            entity.Property(e => e.KeyValueId).ValueGeneratedOnAdd();
            entity.Property(e => e.Module).HasMaxLength(50);
            entity.Property(e => e.PageName).HasMaxLength(50);
        });


        modelBuilder.Entity<TaxSlabDetails>(entity =>
        {
            entity.HasKey(e => e.SlabID);

            entity.ToTable("TaxSlabDetails");

            entity.Property(e => e.AmtFrom).HasColumnName("AmtFrom");
            entity.Property(e => e.AmtTo).HasColumnName("AmtTo");
            entity.Property(e => e.CessTax).HasColumnName("CessTax");
            entity.Property(e => e.FY)
                .HasMaxLength(10)
                .HasColumnName("FY");
            entity.Property(e => e.Regime).HasColumnName("Regime");
            entity.Property(e => e.TaxPercentage).HasColumnName("TaxPercentage");
            entity.Property(e => e.UnitId).HasColumnName("UnitId");
        });

        modelBuilder.Entity<EmployeeExitInterViewFormMaster>(entity =>
        {
            entity.HasKey(e => e.EmployeeExitInterViewFormMasterId).HasName("PK__Employee__2D4894DD2C9F4F33");

            entity.ToTable("EmployeeExitInterViewFormMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FormName).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<ExitClearanceAssetMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExitClearanceAssetMapping");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ExitClearanceMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExitClearanceMapping");

            entity.Property(e => e.ClearanceMappingId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProfileEditAuth>(entity =>
        {
            entity.HasKey(e => e.ProfileEditAuthId).HasName("PK__ProfileE__37BDEACE27AB13B2");

            entity.ToTable("ProfileEditAuth");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProfileFieldDisplayName).HasMaxLength(100);
            entity.Property(e => e.ProfileFieldName).HasMaxLength(100);
        });

        modelBuilder.Entity<ProfileField>(entity =>
        {
            entity.HasKey(e => e.ProfileFieldId).HasName("PK__ProfileF__F89E5140EB6C2210");

            entity.ToTable("ProfileField");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProfileFieldName).HasMaxLength(100);
        });

        //modelBuilder.Entity<EditEmployeeData>(entity =>
        //{
        //    entity.HasNoKey();

        //    entity.Property(e => e.ChangeType).HasMaxLength(50);
        //    entity.Property(e => e.ChangeValue).HasMaxLength(255);
        //    entity.Property(e => e.CreatedBy).HasMaxLength(50);
        //    entity.Property(e => e.CreatedOn)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.EmployeeUpdateId).ValueGeneratedOnAdd();
        //    entity.Property(e => e.EntrySource)
        //        .HasMaxLength(20)
        //        .IsUnicode(false);
        //    entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        //    entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        //    entity.Property(e => e.OldValue).HasMaxLength(255);
        //    entity.Property(e => e.Wefdate)
        //        .HasColumnType("date")
        //        .HasColumnName("WEFDate");
        //});
        modelBuilder.Entity<TicketMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TicketMaster");

            entity.Property(e => e.CreatedBy).HasMaxLength(20);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(20);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TicketId).ValueGeneratedOnAdd();
            entity.Property(e => e.TicketSource).HasMaxLength(20);
        });

        modelBuilder.Entity<TemplateMasterDynamic>(entity =>
        {
            entity.HasKey(e => e.TemplateMasterDynamicId).HasName("PK__Template__BE3C38F40EF50124");

            entity.ToTable("TemplateMasterDynamic");

            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FormName).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<EmployeeLanguageDetail>(entity =>
        {
            entity.HasKey(e => e.EmployeeLanguageDetailId).HasName("PK__Employee__CEC840FBDCC9C317");

            entity.ToTable("EmployeeLanguageDetail");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LanguageId).HasColumnName("LanguageId"); ;
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });
        modelBuilder.Entity<PerformanceSetting>(entity =>
        {
            entity.HasKey(e => e.PerformanceSettingId).HasName("PK__Performa__2D7B6A0FFE0EFB1B");

            entity.ToTable("PerformanceSetting");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.HODClosingRemarks).HasColumnName("HODClosingRemarks");
            entity.Property(e => e.HODReview).HasColumnName("HODReview");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ReviewPeriodFrom).HasColumnType("datetime");
            entity.Property(e => e.ReviewPeriodTo).HasColumnType("datetime");
            entity.Property(e => e.TNRByManager).HasColumnName("TNRByManager");
        });

        modelBuilder.Entity<PerformanceSettingMechanism>(entity =>
        {
            entity.HasKey(e => e.PerformanceSettingMechanismId).HasName("PK__Performa__DE1E4EF2A887E5D1");

            entity.ToTable("PerformanceSettingMechanism");

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<PerformanceSettingSkillSetMatrix>(entity =>
        {
            entity.HasKey(e => e.PerformanceSettingSkillSetMatrixId).HasName("PK__Performa__103A228E903923A5");

            entity.ToTable("PerformanceSettingSkillSetMatrix");

            entity.Property(e => e.KRAWeightage).HasColumnName("KRAWeightage");
        });
        modelBuilder.Entity<PerformanceKRAMasterDB>(entity =>
        {
            entity.HasKey(e => e.PerformanceKRAMasterDBId).HasName("PK__Performa__2344762C44DBD291");

            entity.ToTable("PerformanceKRAMasterDB");

            entity.Property(e => e.PerformanceKRAMasterDBId).HasColumnName("PerformanceKRAMasterDBId");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.KRA)
                .HasMaxLength(255)
                .HasColumnName("KRA");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<PerformanceEmployeeTrainingData>(entity =>
        {
            entity.HasKey(e => e.PerformanceEmployeeTrainingDataId).HasName("PK__Performa__D35BF6128EEA8D79");
        });
        modelBuilder.Entity<PerformanceEmployeeKRAData>(entity =>
        {
            entity.HasKey(e => e.PerformanceEmployeeKRADataId).HasName("PK__Performa__2F6645EA8FBA0497");

            entity.ToTable("PerformanceEmployeeKRAData");

            entity.Property(e => e.PerformanceEmployeeKRADataId).HasColumnName("PerformanceEmployeeKRADataId");
            entity.Property(e => e.EmployeeRemarks).HasMaxLength(255);
            entity.Property(e => e.KRA)
                .HasMaxLength(255)
                .HasColumnName("KRA");
            entity.Property(e => e.ManagerRemarks).HasMaxLength(255);
            entity.Property(e => e.SNo).HasColumnName("SNo");
            entity.Property(e => e.Source).HasMaxLength(50);
            entity.Property(e => e.WAScore).HasColumnName("WAScore");
        });
        modelBuilder.Entity<PerformanceEmployeeData>(entity =>
        {
            entity.HasKey(e => e.PerformanceEmployeeDataId).HasName("PK__Performa__B9A15BA90DE01E0E");

            entity.Property(e => e.ClosingRemarksEmployee).HasMaxLength(255);
            entity.Property(e => e.ClosingRemarksHOD)
                .HasMaxLength(255)
                .HasColumnName("ClosingRemarksHOD");
            entity.Property(e => e.ClosingRemarksManager).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(100);
            entity.Property(e => e.FilledByHOD).HasColumnName("FilledByHOD");
            entity.Property(e => e.HODFinalRating)
                .HasMaxLength(100)
                .HasColumnName("HODFinalRating");
            entity.Property(e => e.HODFinalRatingId).HasColumnName("HODFinalRatingId");
            entity.Property(e => e.KRAEmployeeRatingTotal).HasColumnName("KRAEmployeeRatingTotal");
            entity.Property(e => e.KRAManagersRatingTotal).HasColumnName("KRAManagersRatingTotal");
            entity.Property(e => e.KRAWeightageTotal).HasColumnName("KRAWeightageTotal");
            entity.Property(e => e.ModifiedByHOD).HasColumnName("ModifiedByHOD");
            entity.Property(e => e.ModifiedDateEmployee).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateHOD)
                .HasColumnType("datetime")
                .HasColumnName("ModifiedDateHOD");
            entity.Property(e => e.ModifiedDateManager).HasColumnType("datetime");
            entity.Property(e => e.RatingCalculationFinalRating).HasMaxLength(100);
            entity.Property(e => e.RatingCalculationKRAFinalScore).HasColumnName("RatingCalculationKRAFinalScore");
            entity.Property(e => e.RatingCalculationKRAScore).HasColumnName("RatingCalculationKRAScore");
            entity.Property(e => e.RatingCalculationKRAWeightage).HasColumnName("RatingCalculationKRAWeightage");
        });
        modelBuilder.Entity<PerformanceTrainingNeedsMaster>(entity =>
        {
            entity.HasKey(e => e.TrainingNeedsMasterId).HasName("PK__Training__10E23F68081CFA31");

            entity.ToTable("PerformanceTrainingNeedsMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Training).HasMaxLength(255);
        });

        modelBuilder.Entity<EmployeeAnnouncement>(entity =>
        {
            entity.HasKey(e => e.EmployeeAnnouncementId).HasName("PK__Employee__AC9E1FD894F5AF87");

            entity.ToTable("EmployeeAnnouncement");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Departments).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Keywords).HasMaxLength(255);
            entity.Property(e => e.KeywordsRaw).HasMaxLength(255);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
        });
        modelBuilder.Entity<EmployeeAnnouncementFileUpload>(entity =>
        {
            entity.HasKey(e => e.EmployeeAnnouncementFileUploadsId).HasName("PK__Employee__26BE9D00F9FCD3F1");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UploadType).HasMaxLength(20);
            entity.Property(e => e.UploadedFileExtension).HasMaxLength(10);
            entity.Property(e => e.UploadedFileName).HasMaxLength(150);
            entity.Property(e => e.UploadedFilePath).HasMaxLength(255);
        });

        modelBuilder.Entity<EmployeeNews>(entity =>
        {
            entity.HasKey(e => e.EmployeeNewsId).HasName("PK__Employee__8B307E1554721BED");

            entity.Property(e => e.Article).HasMaxLength(255);
            entity.Property(e => e.ArticleLink).HasMaxLength(255);
            entity.Property(e => e.AuthorsName).HasMaxLength(255);
            entity.Property(e => e.AuthorsNameRaw).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Keywords).HasMaxLength(255);
            entity.Property(e => e.KeywordsRaw).HasMaxLength(255);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PublicationName).HasMaxLength(150);
            entity.Property(e => e.Tagging).HasMaxLength(255);
            entity.Property(e => e.TaggingRaw).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<EmployeeNewsFileUpload>(entity =>
        {
            entity.HasKey(e => e.EmployeeNewsFileUploadsId).HasName("PK__Employee__9026DBA24B4B208C");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UploadType).HasMaxLength(20);
            entity.Property(e => e.UploadedFileExtension).HasMaxLength(10);
            entity.Property(e => e.UploadedFileName).HasMaxLength(150);
            entity.Property(e => e.UploadedFilePath).HasMaxLength(255);
        });
        modelBuilder.Entity<AnnouncementTypeMaster>(entity =>
        {
            entity.HasKey(e => e.AnnouncementTypeId).HasName("PK__Announce__CA21331911B5D5FF");

            entity.ToTable("AnnouncementTypeMaster");

            entity.Property(e => e.AnnouncementType).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<NewsCategoryTagMaster>(entity =>
        {
            entity.HasKey(e => e.NewsCategoryTagId).HasName("PK__NewsCate__83239299B448F6EE");

            entity.ToTable("NewsCategoryTagMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.NewsCategoryTag).HasMaxLength(150);
        });
        modelBuilder.Entity<SurveyPollOption>(entity =>
        {
            entity.HasKey(e => e.SurveyPollOptionId).HasName("PK__SurveyPo__778E0D9EF18AF483");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OptionName).HasMaxLength(255);
        });

        modelBuilder.Entity<SurveyPollsQuestion>(entity =>
        {
            entity.HasKey(e => e.SurveyPollQuestionId).HasName("PK__SurveyPo__DE3DCD9CF9696926");

            entity.ToTable("SurveyPollsQuestion");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Question).HasMaxLength(255);
        });
        modelBuilder.Entity<PollResponse>(entity =>
        {
            entity.ToTable("PollResponses");
            entity.HasKey(e => e.PollResponsesId).HasName("PK__PollResp__E6D034B94C088404");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

        });
        modelBuilder.Entity<AppMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.HODId).HasColumnName("HODId");
            entity.Property(e => e.MessageHTML).HasColumnName("MessageHTML");
            entity.Property(e => e.MessageSubject).HasMaxLength(500);
            entity.Property(e => e.MessageType).HasMaxLength(50);
            entity.Property(e => e.PublishEndDate).HasColumnType("datetime");
            entity.Property(e => e.PublishStartDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<WageBillTrendData>(entity =>
        {
            entity.HasKey(e => e.WageBillTrendDataId).HasName("PK__WageBill__BBFEC597425EFD06");
        });
        modelBuilder.Entity<PTAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentID).HasName("PK__PTAttach__442C64DE49E87913");

            entity.ToTable("PTAttachment");

            entity.Property(e => e.AttachmentID).HasColumnName("AttachmentID");
            entity.Property(e => e.CommentID).HasColumnName("CommentID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FileType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });
        modelBuilder.Entity<PTComment>(entity =>
        {
            entity.HasKey(e => e.CommentID).HasName("PK__PTCommen__C3B4DFAA74D4535C");

            entity.ToTable("PTComment");

            entity.Property(e => e.CommentID).HasColumnName("CommentID");
            entity.Property(e => e.CommentDate).HasColumnType("datetime");
            entity.Property(e => e.CommentText).IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TaskID).HasColumnName("TaskID");
            entity.Property(e => e.UserID).HasColumnName("UserID");
        });
        modelBuilder.Entity<PTProject>(entity =>
        {
            entity.HasKey(e => e.ProjectID).HasName("PK__PTProjec__761ABED0B3618E8C");

            entity.ToTable("PTProject");

            entity.Property(e => e.ProjectID).HasColumnName("ProjectID");
            entity.Property(e => e.CategoryID).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PriorityId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProjectName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.StatusID).HasColumnName("StatusID");
        });
        modelBuilder.Entity<PTProjectCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryID).HasName("PK__PTProjec__19093A2BC2C48003");

            entity.ToTable("PTProjectCategory");

            entity.Property(e => e.CategoryID).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PTProjectMember>(entity =>
        {
            entity.HasKey(e => e.ProjectMemberID).HasName("PK__PTProjec__E4E9983CCFB29D95");

            entity.ToTable("PTProjectMember");

            entity.Property(e => e.ProjectMemberID).HasColumnName("ProjectMemberID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectID).HasColumnName("ProjectID");
            entity.Property(e => e.RoleType).HasColumnName("RoleType");
            entity.Property(e => e.UserID).HasColumnName("UserID");
        });
        //modelBuilder.Entity<PTRole>(entity =>
        //{
        //    entity.HasKey(e => e.RoleID).HasName("PK__PTRole__8AFACE3AF65338CD");

        //    entity.ToTable("PTRole");

        //    entity.Property(e => e.RoleID).HasColumnName("RoleID");
        //    entity.Property(e => e.CreatedDate)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        //    entity.Property(e => e.RoleName)
        //        .HasMaxLength(255)
        //        .IsUnicode(false);
        //});

        modelBuilder.Entity<PTStatus>(entity =>
        {
            entity.HasKey(e => e.StatusID).HasName("PK__PTStatus__C8EE2043D45F8203");

            entity.ToTable("PTStatus");

            entity.Property(e => e.StatusID).HasColumnName("StatusID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.StatusName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PTTask>(entity =>
        {
            entity.HasKey(e => e.TaskID).HasName("PK__PTTask__7C6949D12BC2C33E");

            entity.ToTable("PTTask");

            entity.Property(e => e.TaskID).HasColumnName("TaskID");
            entity.Property(e => e.Comments).IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Priority)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProjectID).HasColumnName("ProjectID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.StatusID).HasColumnName("StatusID");
            entity.Property(e => e.TaskDescription).IsUnicode(false);
        });

        modelBuilder.Entity<PTDeliverables>(entity =>
        {
            entity.HasKey(e => e.DeliverableId).HasName("PK__PTDelive__71A9170ED4649737");

            entity.ToTable("PTDeliverables");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeliverableName).HasMaxLength(200);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SNo).HasColumnName("SNo");
        });

        modelBuilder.Entity<PTMilestones>(entity =>
        {
            entity.HasKey(e => e.MilestoneId).HasName("PK__PTMilest__09C48078B1541B80");

            entity.ToTable("PTMilestones");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MilestoneName).HasMaxLength(200);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SNo).HasColumnName("SNo");
        });
        modelBuilder.Entity<PTProjectPriority>(entity =>
        {
            entity.HasKey(e => e.PriorityId).HasName("PK__PTProjec__D0A3D0BEDFE7D5F1");

            entity.ToTable("PTProjectPriority");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Priority).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
