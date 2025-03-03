using Microsoft.AspNetCore.Mvc;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Core.Entities;
using AutoMapper;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Dapper;
using System.Data;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Services.DBContext;
using System.Data.Entity;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Helper;
using Newtonsoft.Json;

namespace SimpliHR.Endpoints.MastersKeyValue;

public class MastersKeyValueController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MastersKeyValueController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _context;

    public MastersKeyValueController(IUnitOfWork unitOfWork, ILogger<MastersKeyValueController> logger, IMapper mapper, SimpliDbContext context)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet]
    public async Task<EmployeeMastersKeyValues> EmployeeMastersKeyValue(bool? isActive = true, int? unitId = null, int? clientId = null)
    {
        var keyValueTasks = new List<Task>();
        EmployeeMastersKeyValues employeeMastersKeyValues = new EmployeeMastersKeyValues();
        employeeMastersKeyValues.AcademicKeyValues = await AcademicKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.BandKeyValues = await BandKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.BankKeyValues = await BankKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.BloodGroupKeyValues = await BloodGroupKeyValue(isActive);
        employeeMastersKeyValues.EmployeeKeyValues = await CompleteFilledInfoEmployeeKeyValue((p => p.IsActive == true && p.InfoFillingStatus == 1 && p.EmployeeStatus.ToUpper() != "TERMINATED" && p.UnitId == unitId));
        employeeMastersKeyValues.CountryKeyValues = await CountryKeyValue(isActive);
        employeeMastersKeyValues.DepartmentKeyValues = await DepartmentKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.IdtypeKeyValues = await IdTypeKeyValue(isActive);
        employeeMastersKeyValues.JobTitleKeyValues = await JobTitleKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.LeaveTypeKeyValues = await LeaveTypeKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.MaritalStatusKeyValues = await MaritalStatusKeyValue(isActive);
        employeeMastersKeyValues.ReligionKeyValues = await ReligionKeyValue(isActive);
        employeeMastersKeyValues.ResourceKeyValues = await ResourceKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.RoleKeyValues = await RoleKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.SalaryKeyValues = await SalaryKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.ShiftKeyValues = await ShiftKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.WorkLocationKeyValues = await WorkLocationKeyValue((p => p.IsActive == true && p.UnitId == unitId), (m => m.OrderBy(x => x.Location)));
        employeeMastersKeyValues.LanguageKeyValue = await LanguageKeyValue(p => p.IsActive == true && p.UnitId == unitId);

        return employeeMastersKeyValues;
    }
    [HttpGet]
    public async Task<EmployeeMastersKeyValues> EmployeeMastersKeyValueExcludingEmployeeKey(bool? isActive = true, int? unitId = null, int? clientId = null)
    {
        var keyValueTasks = new List<Task>();
        EmployeeMastersKeyValues employeeMastersKeyValues = new EmployeeMastersKeyValues();
        employeeMastersKeyValues.AcademicKeyValues = await AcademicKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.BandKeyValues = await BandKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.BankKeyValues = await BankKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.BloodGroupKeyValues = await BloodGroupKeyValue(isActive);
        //employeeMastersKeyValues.EmployeeKeyValues = await CompleteFilledInfoEmployeeKeyValue((p => p.IsActive == true && p.InfoFillingStatus == 1 && p.EmployeeStatus.ToUpper() != "TERMINATED" && p.UnitId == unitId));
        employeeMastersKeyValues.CountryKeyValues = await CountryKeyValue(isActive);
        employeeMastersKeyValues.DepartmentKeyValues = await DepartmentKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.IdtypeKeyValues = await IdTypeKeyValue(isActive);
        employeeMastersKeyValues.JobTitleKeyValues = await JobTitleKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.LeaveTypeKeyValues = await LeaveTypeKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.MaritalStatusKeyValues = await MaritalStatusKeyValue(isActive);
        employeeMastersKeyValues.ReligionKeyValues = await ReligionKeyValue(isActive);
        employeeMastersKeyValues.ResourceKeyValues = await ResourceKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.RoleKeyValues = await RoleKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.SalaryKeyValues = await SalaryKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.ShiftKeyValues = await ShiftKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.WorkLocationKeyValues = await WorkLocationKeyValue((p => p.IsActive == true && p.UnitId == unitId), (m => m.OrderBy(x => x.Location)));
        employeeMastersKeyValues.LanguageKeyValue = await LanguageKeyValue(p => p.IsActive == true && p.UnitId == unitId);
        //employeeMastersKeyValues.EmployeeValidations = await EmployeeValidationKeyValue((p => p.IsActive == true && p.ClientId == clientId));

        return employeeMastersKeyValues;
    }

    [HttpGet]
    public async Task<EmployeeMastersKeyValues> DeptMastersKeyValue(bool? isActive = true, int? unitId = null)
    {
        var keyValueTasks = new List<Task>();
        EmployeeMastersKeyValues employeeMastersKeyValues = new EmployeeMastersKeyValues();
        //employeeMastersKeyValues.AcademicKeyValues = await AcademicKeyValue(isActive);
        //employeeMastersKeyValues.BandKeyValues = await BandKeyValue(isActive);
        //employeeMastersKeyValues.BankKeyValues = await BankKeyValue(isActive);
        //employeeMastersKeyValues.BloodGroupKeyValues = await BloodGroupKeyValue(isActive);
        //employeeMastersKeyValues.EmployeeKeyValues = await CompleteFilledInfoEmployeeKeyValue((p => p.IsActive == true && p.InfoFillingStatus == 1 && p.EmployeeStatus.ToUpper() != "TERMINATED" && p.UnitId == unitId));
        //employeeMastersKeyValues.CountryKeyValues = await CountryKeyValue(isActive);
        employeeMastersKeyValues.DepartmentKeyValues = await DepartmentKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        //employeeMastersKeyValues.IdtypeKeyValues = await IdTypeKeyValue(isActive);
        //employeeMastersKeyValues.JobTitleKeyValues = await JobTitleKeyValue(p => p.IsActive == true);
        //employeeMastersKeyValues.LeaveTypeKeyValues = await LeaveTypeKeyValue(isActive);
        //employeeMastersKeyValues.MaritalStatusKeyValues = await MaritalStatusKeyValue(isActive);
        //employeeMastersKeyValues.ReligionKeyValues = await ReligionKeyValue(isActive);
        //employeeMastersKeyValues.ResourceKeyValues = await ResourceKeyValue(isActive);
        //employeeMastersKeyValues.RoleKeyValues = await RoleKeyValue(isActive);
        //employeeMastersKeyValues.SalaryKeyValues = await SalaryKeyValue(isActive);
        //employeeMastersKeyValues.ShiftKeyValues = await ShiftKeyValue(isActive);
        //employeeMastersKeyValues.WorkLocationKeyValues = await WorkLocationKeyValue(isActive);

        return employeeMastersKeyValues;
    }




    [HttpGet]
    public async Task<AttendanceMastersKeyValues> AttendanceMastersKeyValues(bool? isActive = true, string sUnitId = "",bool isclient = false,int employeeId=0)
    {
        int iUnitId;
        int.TryParse(sUnitId, out iUnitId);
        var keyValueTasks = new List<Task>();
        AttendanceMastersKeyValues mastersKeyValues = new AttendanceMastersKeyValues();
        mastersKeyValues.DepartmentKeyValues = await DepartmentKeyValue(x => x.IsActive == true && x.UnitId == iUnitId);
        string unitIds = sUnitId;
        //Expression<Func<ShiftMaster, bool>>? expression = p => ((unitIds != "" ? unitIds.Contains("," + p.UnitId.ToString() + ",") : p.UnitId == p.UnitId) && (p.IsActive == true));
        Expression<Func<ShiftMaster, bool>>? expression = p => p.UnitId == (string.IsNullOrEmpty(unitIds) ? p.UnitId : iUnitId) && (p.IsActive == true);
        mastersKeyValues.ShiftKeyValues = await ShiftKeyValue(expression);
        
 
        if (isclient)
            mastersKeyValues.EmployeeKeyValues = (await EmployeeKeyValue(x => x.IsActive == true && x.EmployeeStatus=="Active" && x.InfoFillingStatus == 1 && x.UnitId == iUnitId)).ToList();
        else
            mastersKeyValues.EmployeeKeyValues = (await EmployeeKeyValue(x => x.IsActive == true && x.EmployeeStatus == "Active" && x.InfoFillingStatus == 1 && x.EmployeeStatus.ToUpper() != "TERMINATED" && x.UnitId == iUnitId && (x.HODId == employeeId || x.ManagerId == employeeId || x.EmployeeId == employeeId))).ToList();
        
        mastersKeyValues.WorkLocationKeyValues = await WorkLocationKeyValue((p => p.IsActive == true && p.UnitId == iUnitId), (m => m.OrderBy(x => x.Location)));
        return mastersKeyValues;
    }

    [HttpGet]
    public async Task<EmployeeMastersKeyValues> E_JoineeEmployeeMastersKeyValue(bool? isActive = true, int? unitId = null)
    {
        var keyValueTasks = new List<Task>();
        EmployeeMastersKeyValues employeeMastersKeyValues = new EmployeeMastersKeyValues();
        employeeMastersKeyValues.DepartmentKeyValues = await DepartmentKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.JobTitleKeyValues = await JobTitleKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        employeeMastersKeyValues.EmployeeKeyValues = await EmployeeKeyValue((p => p.IsActive == true && p.InfoFillingStatus == 1 && p.EmployeeStatus.ToUpper() != "TERMINATED" && p.UnitId == unitId));
        //employeeMastersKeyValues.SBUKeyValues = await EmployeeKeyValue((p => p.IsActive == true && p.InfoFillingStatus == 1 && p.EmployeeStatus.ToUpper() != "TERMINATED"));
        return employeeMastersKeyValues;
    }
    [HttpGet]
    public async Task<LeaveAttributeMasterKeyValues> LeaveAttributeMastersKeyValue(bool? isActive = true, int? clientId = null, int? UnitId = null)
    {
        var keyValueTasks = new List<Task>();
        LeaveAttributeMasterKeyValues leaveAttributeMasterKeyValues = new LeaveAttributeMasterKeyValues();
        leaveAttributeMasterKeyValues.LeaveCalenderYearKeyValue = await LeaveCalenderYearKeyValue((p => p.IsActive == true && p.UnitId == UnitId));
        leaveAttributeMasterKeyValues.PolicyDocumentKeyValues = await PolicyDocumentKeyValue((p => p.IsActive == true && p.ClientId == clientId && p.UnitId == UnitId));
        leaveAttributeMasterKeyValues.LeaveTypeKeyValues = await LeaveTypeKeyValue((p => p.IsActive == true && p.UnitId == UnitId));
        return leaveAttributeMasterKeyValues;
    }

    //[HttpGet]
    //public async Task<List<EmployeeKeyValues>>? EmployeeKeyValue(Expression<Func<EmployeeMaster, bool>>? expression = null)
    //{
    //    //p => ((isActive != null ? p.IsActive == isActive : true
    //    var dept = DepartmentKeyValue(x => x.IsActive == true).Result;
    //    return (await _unitOfWork.EmployeeMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.EmployeeName))).ConfigureAwait(true))
    //                   .Select(p => new EmployeeKeyValues()
    //                   {
    //                       EmployeeId = p.EmployeeId,
    //                       EmployeeName = p.EmployeeName,
    //                       DepartmentId = p.DepartmentId,
    //                       UnitId = p.UnitId,
    //                       ManagerId = p.ManagerId,
    //                       HODId = p.HODId,
    //                       EmployeeDeparment = (p.DepartmentId == null ? "" : dept.Where(r => r.DepartmentId == (p.DepartmentId == null ? 0 : p.DepartmentId)).Select(x => x.DepartmentName).FirstOrDefault().ToString())

    //                   }).ToList();
    //}


    [HttpGet]
    public async Task<List<EmployeeKeyValues>>? EmployeeKeyValue(Expression<Func<EmployeeMaster, bool>>? expression = null)
    {
        //p => ((isActive != null ? p.IsActive == isActive : true
        //var dept = DepartmentKeyValue(x => x.IsActive == true).Result;
        //return (await _unitOfWork.EmployeeMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.EmployeeName))).ConfigureAwait(true))
        return (_context.EmployeeMasters.Where(expression).Include(a => a.Department)
                       .Select(p => new EmployeeKeyValues()
                       {
                           EmployeeId = p.EmployeeId,
                           EmployeeName = p.EmployeeName,
                           EmployeeNameCode = p.EmployeeName + $" ({p.EmployeeCode})",
                           DepartmentId = p.DepartmentId,
                           UnitId = p.UnitId,
                           ManagerId = p.ManagerId,
                           HODId = p.HODId,
                           EmployeeDeparment = p.Department.DepartmentName,
                           DOJ = p.Doj,
                           ExitDate = _context.EmployeeExitResignations.FirstOrDefault(r => r.EmployeeId == p.EmployeeId  && r.AdminApproval==1).LastWorkingDateAdmin
                           != null ? _context.EmployeeExitResignations.FirstOrDefault(r => r.EmployeeId == p.EmployeeId).LastWorkingDateAdmin : null
                           //_context.EmployeeExitResignations.FirstOrDefault(r => r.EmployeeId == p.EmployeeId).LastWorkingDateManager != null ?
                           //_context.EmployeeExitResignations.FirstOrDefault(r => r.EmployeeId == p.EmployeeId).LastWorkingDateManager :
                           //_context.EmployeeExitResignations.FirstOrDefault(r => r.EmployeeId == p.EmployeeId).LastWorkingDate
                       })).ToList();
    }

    [HttpGet]
    public async Task<List<EmployeeKeyValues>>? ConfirmedEmployeeKeyValue(Expression<Func<EmployeeMaster, bool>>? expression = null)
    {
        return (await EmployeeKeyValue(expression)).OrderBy(x => x.EmployeeName).ToList();
        //var dept = DepartmentKeyValue(x => x.IsActive == true).Result;
        //return (await _unitOfWork.EmployeeMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.EmployeeName))).ConfigureAwait(true))
        //               .Select(p => new EmployeeKeyValues()
        //               {
        //                   EmployeeId = p.EmployeeId,
        //                   EmployeeName = p.EmployeeName,
        //                   DepartmentId = p.DepartmentId,
        //                   EmployeeCode = p.EmployeeCode,
        //                   UnitId = p.UnitId,
        //                   ManagerId = p.ManagerId,
        //                   HODId = p.HODId,
        //                   EmployeeDeparment = (p.DepartmentId == null ? "" : dept.Where(r => r.DepartmentId == (p.DepartmentId == null ? 0 : p.DepartmentId)).Select(x => x.DepartmentName).FirstOrDefault().ToString()),
        //               }).ToList();
    }

    //[HttpGet]
    //public async Task<List<EmployeeKeyValues>>? ConfirmedEmployeeKeyValue(Expression<Func<EmployeeMaster, bool>>? expression = null)
    //{
    //    //p => ((isActive != null ? p.IsActive == isActive : true
    //    //expression = expression + " AND p.InfoFillingStatus=1 AND p.IsConfirmed=1";
    //    var dept = DepartmentKeyValue(x => x.IsActive == true).Result;
    //    return (await _unitOfWork.EmployeeMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.EmployeeName))).ConfigureAwait(true))
    //                   .Select(p => new EmployeeKeyValues()
    //                   {
    //                       EmployeeId = p.EmployeeId,
    //                       EmployeeName = p.EmployeeName,
    //                       DepartmentId = p.DepartmentId,
    //                       EmployeeCode = p.EmployeeCode,
    //                       UnitId = p.UnitId,
    //                       ManagerId = p.ManagerId,
    //                       HODId = p.HODId,
    //                       EmployeeDeparment = (p.DepartmentId == null ? "" : dept.Where(r => r.DepartmentId == (p.DepartmentId == null ? 0 : p.DepartmentId)).Select(x => x.DepartmentName).FirstOrDefault().ToString()),
    //                   }).ToList();
    //}

    [HttpGet]
    public async Task<List<EmployeeKeyValues>>? CompleteFilledInfoEmployeeKeyValue(Expression<Func<EmployeeMaster, bool>>? expression = null, int? UnitId = null)
    {
        //p => ((isActive != null ? p.IsActive == isActive : true
        //expression = expression + " AND p.InfoFillingStatus=1";
        //var dept = DepartmentKeyValue(x => x.IsActive == true).Result;

        if (UnitId == null)
        {

            return (await EmployeeKeyValue(expression)).OrderBy(x => x.EmployeeName).ToList();

            //return (await _unitOfWork.EmployeeMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.EmployeeName))).ConfigureAwait(true)).Where(x => x.InfoFillingStatus == 1)
            //               .Select(p => new EmployeeKeyValues()
            //               {
            //                   EmployeeId = p.EmployeeId,
            //                   EmployeeName = p.EmployeeName,
            //                   DepartmentId = p.DepartmentId,
            //                   EmployeeCode = p.EmployeeCode,
            //                   UnitId = p.UnitId,
            //                   ManagerId = p.ManagerId,
            //                   HODId = p.HODId,
            //                   EmployeeDeparment = (p.DepartmentId == null ? "" : dept.Where(r => r.DepartmentId == (p.DepartmentId == null ? 0 : p.DepartmentId)).Select(x => x.DepartmentName).FirstOrDefault().ToString()),
            //               }).ToList();
        }
        else
        {
            List<EmployeeKeyValues> employeeKeyValues = new List<EmployeeKeyValues>();
            string sQuery = $"select EmployeeId ,EmployeeName ,DepartmentId ,EmployeeCode ,UnitId ,ManagerId ,HODId ,(Select DepartmentName from DepartmentMaster dm where dm.DepartmentId=em.DepartmentId )EmployeeDeparment  from employeemaster em where unitid={UnitId} and infofillingstatus=1 order by employeename";
            employeeKeyValues = await _unitOfWork.ProfileField.GetTableData<EmployeeKeyValues>(sQuery);
            return employeeKeyValues;
        }
    }



    [HttpGet]
    public async Task<List<AcademicKeyValues>>? AcademicKeyValue(Expression<Func<AcademicMaster, bool>>? expression = null)
    {
        return (await _unitOfWork.AcademicMaster.GetAll(expression).ConfigureAwait(true))
                       .Select(p => new AcademicKeyValues()
                       {
                           AcademicId = p.AcademicId,
                           AcademicName = p.AcademicName
                       }).ToList();
    }
    //[HttpGet]
    //public async Task<List<AcademicKeyValues>>? SBUKeyValue(bool? isActive = true)
    //{
    //    return (await _unitOfWork..GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
    //                   .Select(p => new AcademicKeyValues()
    //                   {
    //                       AcademicId = p.AcademicId,
    //                       AcademicName = p.AcademicName
    //                   }).ToList();
    //}

    [HttpGet]
    public async Task<List<BandKeyValues>>? BandKeyValue(Expression<Func<BandMaster, bool>>? expression = null)
    {

        return (await _unitOfWork.BandMaster.GetAll(expression).ConfigureAwait(true))
                        .Select(p => new BandKeyValues()
                        {
                            BandId = p.BandId,
                            Band = p.Band
                        }).ToList();
    }

    [HttpGet]
    public async Task<List<BankKeyValues>>? BankKeyValue(Expression<Func<BankUnitMaster, bool>>? expression = null)
    {

        return (await _unitOfWork.BankUnitMaster.GetAll(expression).ConfigureAwait(true))
                      .Select(p => new BankKeyValues()
                      {
                          BankId = p.BankId,
                          BankName = p.BankName
                      }).ToList();
    }

    [HttpGet]
    public async Task<List<BloodGroupKeyValues>> BloodGroupKeyValue(bool? isActive = true)
    {
        return (await _unitOfWork.BloodGroupMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
                      .Select(p => new BloodGroupKeyValues()
                      {
                          BloodGroupId = p.BloodGroupId,
                          BloodGroupName = p.BloodGroupCode
                      }).ToList();

    }

    [HttpGet]
    public async Task<List<CityKeyValues>> CityKeyValue(bool? isActive = true, int stateId = 0)
    {
        List<CityKeyValues> cities = new List<CityKeyValues>();
       
        if (stateId > 0)
            cities= (await _unitOfWork.CityMaster.GetAll(p => ((isActive != true ? p.IsActive == isActive : true) && p.StateId == stateId)).ConfigureAwait(true))
                      .Select(p => new CityKeyValues()
                      {
                          CityId = p.CityId,
                          CityName = p.CityName
                      }).ToList();
        //else
        //    return (await _unitOfWork.CityMaster.GetAll(p => ((isActive != true ? p.IsActive == isActive : true))).ConfigureAwait(true))
        //             .Select(p => new CityKeyValues()
        //             {
        //                 CityId = p.CityId,
        //                 CityName = p.CityName
        //             }).ToList();
        return cities;
    }

    [HttpGet]
    public async Task<List<CountryKeyValues>> CountryKeyValue(bool? isActive = true)
    {
        return (await _unitOfWork.CountryMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
                   .Select(p => new CountryKeyValues()
                   {
                       CountryId = p.CountryId,
                       CountryName = p.CountryName
                   }).ToList();


    }

    [HttpGet]
    public async Task<List<DepartmentKeyValues>> DepartmentKeyValue(Expression<Func<DepartmentMaster, bool>>? expression = null)
    {

        return (await _unitOfWork.DepartmentMaster.GetAll(expression).ConfigureAwait(true))
                  .Select(p => new DepartmentKeyValues()
                  {
                      DepartmentId = p.DepartmentId,
                      DepartmentName = p.DepartmentName
                  }).ToList();
    }

    [HttpGet]
    public async Task<List<DepartmentKeyValues>> DepartmentKeyValueUnitWise(int unitId, bool? isActive = true)
    {

        return (await _unitOfWork.DepartmentMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true)) && p.UnitId == unitId).ConfigureAwait(true))
                  .Select(p => new DepartmentKeyValues()
                  {
                      DepartmentId = p.DepartmentId,
                      DepartmentName = p.DepartmentName
                  }).ToList();
    }
    [HttpGet]
    public async Task<List<AnnouncementTypeKeyValues>> AnnouncementTypeKeyValueUnitWise(int unitId, bool? isActive = true)
    {

        return (await _unitOfWork.AnnouncementTypeMaster.GetFilterAll(p => ((isActive != null ? p.IsActive == isActive : true)) && p.UnitId == unitId).ConfigureAwait(true))
                  .Select(p => new AnnouncementTypeKeyValues()
                  {
                      encAnnouncementTypeId = CommonHelper.EncryptURLHTML(p.AnnouncementTypeId.ToString()),
                      AnnouncementTypeId = p.AnnouncementTypeId,
                      AnnouncementType = p.AnnouncementType
                  }).ToList();
    }
    [HttpGet]
    public async Task<List<NewsCategoryTagKeyValues>> NewsCategoryTagKeyValueUnitWise(int unitId, bool? isActive = true)
    {

        return (await _unitOfWork.NewsCategoryTagMaster.GetFilterAll(p => ((isActive != null ? p.IsActive == isActive : true)) && p.UnitId == unitId).ConfigureAwait(true))
                  .Select(p => new NewsCategoryTagKeyValues()
                  {
                      encNewsCategoryTagId = CommonHelper.EncryptURLHTML(p.NewsCategoryTagId.ToString()),
                      NewsCategoryTagId = p.NewsCategoryTagId,
                      NewsCategoryTag = p.NewsCategoryTag
                  }).ToList();
    }

    [HttpGet]
    public async Task<List<DistrictKeyValues>> DistrictKeyValue(bool? isActive = true)
    {

        return (await _unitOfWork.DistrictMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
                 .Select(p => new DistrictKeyValues()
                 {
                     DistrictId = p.DistrictId,
                     DistrictName = p.DistrictName
                 }).ToList();

    }


    [HttpGet]
    public async Task<List<IdtypeKeyValues>> IdTypeKeyValue(bool? isActive = true)
    {
        return (await _unitOfWork.IdtypeMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
               .Select(p => new IdtypeKeyValues()
               {
                   IdentityId = p.IdentityId,
                   IdentityName = p.IdentityName
               }).ToList();

    }

    public async Task<List<JobTitleKeyValues>> JobTitleKeyValue(Expression<Func<JobTitleMaster, bool>>? expression = null)
    {
        return (await _unitOfWork.JobTitleMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.JobTitle))).ConfigureAwait(true))
              .Select(p => new JobTitleKeyValues()
              {
                  JobTitleId = p.JobTitleId,
                  JobTitle = p.JobTitle
              }).ToList();

    }

    [HttpGet]
    public async Task<List<ClientKeyValues>> ClientKeyValue(Expression<Func<Client, bool>>? expression = null)
    {
        return (await _unitOfWork.Client.GetAll(expression, orderBy: (m => m.OrderBy(x => x.ClientName))).ConfigureAwait(true))
              .Select(p => new ClientKeyValues()
              {
                  ClientId = p.ClientId,
                  ClientName = p.ClientName,
                  CompanyName = p.CompanyName
              }).ToList();

    }

    [HttpGet]
    public async Task<List<LeaveTypeKeyValues>> LeaveTypeKeyValue(bool? isActive = true)
    {
        return (await _unitOfWork.LeaveTypeMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
              .Select(p => new LeaveTypeKeyValues()
              {
                  LeaveTypeId = p.LeaveTypeId,
                  LeaveType = p.LeaveType
              }).ToList();
    }

    [HttpGet]
    public async Task<List<MaritalStatusKeyValues>> MaritalStatusKeyValue(bool? isActive = true)
    {
        return (await _unitOfWork.MaritalStatusMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
             .Select(p => new MaritalStatusKeyValues()
             {
                 MaritalStatusId = p.MaritalStatusId,
                 MaritalStatusName = p.MaritalStatusName
             }).ToList();

    }
    [HttpGet]
    public async Task<List<ReligionKeyValues>> ReligionKeyValue(bool? isActive = true)
    {
        return (await _unitOfWork.ReligionMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
             .Select(p => new ReligionKeyValues()
             {
                 ReligionId = p.ReligionId,
                 ReligionName = p.ReligionName
             }).ToList();

    }

    [HttpGet]
    public async Task<List<ModuleKeyValues>> ClientSelectedModules(int unitId, Expression<Func<ModuleMaster, bool>>? expression = null, Func<IQueryable<ModuleMaster>, IOrderedQueryable<ModuleMaster>> orderBy = null)
    {
        var clientId = (await _unitOfWork.UnitMaster.GetAll(r => r.UnitID == unitId)).Select(r => r.ClientId).FirstOrDefault();
        string moduleIds = (await _unitOfWork.ClientSetting.GetAll(r => r.ClientId == clientId)).Select(r => r.ModuleIds).FirstOrDefault();
        moduleIds = $",{moduleIds},";
        return (await _unitOfWork.ModuleMaster.GetAll(r => r.IsActive == true && moduleIds.Contains("," + Convert.ToString(r.ModuleId) + ","), orderBy).ConfigureAwait(true))
             .Select(p => new ModuleKeyValues()
             {
                 ModuleId = p.ModuleId,
                 ModuleName = p.ModuleName
             }).ToList();
    }

    [HttpGet]
    public async Task<List<ModuleKeyValues>> ModuleKeyValue(Expression<Func<ModuleMaster, bool>>? expression = null, Func<IQueryable<ModuleMaster>, IOrderedQueryable<ModuleMaster>> orderBy = null)
    {
        return (await _unitOfWork.ModuleMaster.GetAll(expression, orderBy).ConfigureAwait(true))
             .Select(p => new ModuleKeyValues()
             {
                 ModuleId = p.ModuleId,
                 ModuleName = p.ModuleName
             }).ToList();
    }

    [HttpGet]
    public async Task<List<ResourceKeyValues>> ResourceKeyValue(Expression<Func<ResourceMaster, bool>>? expression = null)
    {
        return (await _unitOfWork.ResourceMaster.GetAll(expression).ConfigureAwait(true))
             .Select(p => new ResourceKeyValues()
             {
                 ResourceId = p.ResourceId,
                 ResourceName = p.ResourceName
             }).ToList();

    }
    //[HttpGet]
    //public async Task<List<ResourceKeyValues>> ResourceKeyValue(Expression<Func<EmployeeMaster, bool>>? expression = null)
    //{
    //    return (await _unitOfWork.ResourceMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
    //         .Select(p => new ResourceKeyValues()
    //         {
    //             ResourceId = p.ResourceId,
    //             ResourceName = p.ResourceName
    //         }).ToList();

    //}

    [HttpGet]
    public async Task<List<RoleKeyValues>> RoleKeyValue(Expression<Func<RoleMaster, bool>>? expression = null)
    {
        return (await _unitOfWork.RoleMaster.GetAll(expression).ConfigureAwait(true))
             .Select(p => new RoleKeyValues()
             {
                 RoleId = p.RoleId,
                 RoleName = p.RoleName
             }).ToList();

    }


    public async Task<List<SalaryComponentKeyValues>> SalaryKeyValue(Expression<Func<SalaryComponentMaster, bool>>? expression = null)
    {
        return (await _unitOfWork.SalaryComponentMaster.GetAll(expression).ConfigureAwait(true))
             .Select(p => new SalaryComponentKeyValues()
             {
                 SalaryComponentId = p.SalaryComponentId,
                 SalaryComponentTitle = p.SalaryComponentTitle
             }).ToList();

    }

    //public async Task<List<ShiftKeyValues>> ShiftKeyValue(bool? isActive = true, string unitIds = "")
    //{
    //    if (unitIds != "")
    //        unitIds = "," + unitIds + ",";
    //    return (await _unitOfWork.ShiftMaster.GetAll(p => ((unitIds != "" ? unitIds.Contains("," + p.UnitId.ToString() + ",") : p.UnitId == p.UnitId) && (isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
    //        .Select(p => new ShiftKeyValues()
    //        {
    //            ShiftId = p.ShiftId,
    //            ShiftCode = p.ShiftCode,
    //            ShiftName = p.ShiftName + " ( " + p.InTime + " - " + p.OutTime + " )"
    //        }).ToList();

    //}

    public async Task<List<ShiftKeyValues>> ShiftKeyValue(Expression<Func<ShiftMaster, bool>>? expression = null, Func<IQueryable<ShiftMaster>, IOrderedQueryable<ShiftMaster>> orderBy = null, string unitIds = "")
    {
        if (unitIds != "")
            unitIds = "," + unitIds + ",";
        //return (await _unitOfWork.ShiftMaster.GetAll(p => ((unitIds != "" ? unitIds.Contains("," + p.UnitId.ToString() + ",") : p.UnitId == p.UnitId) && (isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
        return (await _unitOfWork.ShiftMaster.GetAll(expression, orderBy).ConfigureAwait(true))
        .Select(p => new ShiftKeyValues()
        {
            ShiftId = p.ShiftId,
            ShiftCode = p.ShiftCode,
            ShiftName = p.ShiftName + " ( " + p.InTime + " - " + p.OutTime + " )"
        }).ToList();

    }
    public async Task<List<ShiftKeyValues>> ShiftKeyValue(Expression<Func<ShiftMaster, bool>>? expression = null)
    {

        return (await _unitOfWork.ShiftMaster.GetAll(expression).ConfigureAwait(true))
            .Select(p => new ShiftKeyValues()
            {
                ShiftId = p.ShiftId,
                ShiftCode = p.ShiftCode,
                ShiftName = p.ShiftName + " ( " + p.InTime + " - " + p.OutTime + " )"
            }).ToList();

    }

    public async Task<List<StateKeyValues>> StateKeyValue(bool? isActive = true, int countryID = 0)
    {
        List<StateKeyValues> states = new List<StateKeyValues>();
        if (countryID > 0)

            states = (await _unitOfWork.StateMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true) && p.CountryId == countryID)).ConfigureAwait(true))
             .Select(p => new StateKeyValues()
             {
                 StateId = p.StateId,
                 StateName = p.StateName
             }).ToList();
        //else
        //    return (await _unitOfWork.StateMaster.GetAll(p => ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
        //   .Select(p => new StateKeyValues()
        //   {
        //       StateId = p.StateId,
        //       StateName = p.StateName
        //   }).ToList();
        return states;
    }

    public async Task<List<StateKeyValues>> UnitStateKeyValue(bool? isActive = true, int countryID = 0, int? unitId = 0)
    {
        if (countryID > 0)
            return (await _unitOfWork.UnitStateMaster.GetAll(p => p.UnitId == unitId && ((isActive != null ? p.IsActive == isActive : true) && p.CountryId == countryID)).ConfigureAwait(true))
             .Select(p => new StateKeyValues()
             {
                 StateId = p.StateId,
                 StateName = p.StateName
             }).ToList();
        else
            return (await _unitOfWork.UnitStateMaster.GetAll(p => p.UnitId == unitId && ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
           .Select(p => new StateKeyValues()
           {
               StateId = p.StateId,
               StateName = p.StateName
           }).ToList();
    }

    [HttpGet]
    public async Task<List<WorkLocationKeyValues>> WorkLocationKeyValue(Expression<Func<WorkLocationMaster, bool>>? expression = null, Func<IQueryable<WorkLocationMaster>, IOrderedQueryable<WorkLocationMaster>> orderBy = null)
    {
        return (await _unitOfWork.WorkLocationMaster.GetAll(expression, orderBy).ConfigureAwait(true))
             .Select(p => new WorkLocationKeyValues()
             {
                 WorkLocationId = p.WorkLocationId,
                 Location = p.Location
             }).ToList();

    }


    [HttpGet]
    public async Task<List<LeaveCalenderYearKeyValues>> LeaveCalenderYearKeyValue(Expression<Func<LeaveCalenderYear, bool>>? expression = null)
    {
        return (await _unitOfWork.LeaveCalenderYear.GetAll(expression, orderBy: (m => m.OrderBy(x => x.LeaveYearId))).ConfigureAwait(true))
              .Select(p => new LeaveCalenderYearKeyValues()
              {
                  LeaveYearId = p.LeaveYearId,
                  CalendarName = p.CalendarName
              }).ToList();

    }
    [HttpGet]
    public async Task<List<PolicyDocumentKeyValues>> PolicyDocumentKeyValue(Expression<Func<PolicyDocumentsMaster, bool>>? expression = null)
    {
        return (await _unitOfWork.PolicyDocumentsMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.PolicyDocumentsMasterId))).ConfigureAwait(true))
              .Select(p => new PolicyDocumentKeyValues()
              {
                  PolicyDocumentsMasterId = p.PolicyDocumentsMasterId,
                  PolicyDocument = p.PolicyDocument
              }).ToList();

    }

    [HttpGet]
    public async Task<List<LeaveTypeKeyValues>> LeaveTypeKeyValue(Expression<Func<LeaveTypeMaster, bool>>? expression = null)
    {
        return (await _unitOfWork.LeaveTypeMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.LeaveTypeId))).ConfigureAwait(true))
              .Select(p => new LeaveTypeKeyValues()
              {
                  LeaveTypeId = p.LeaveTypeId,
                  LeaveType = p.LeaveType + "(" + p.LeaveTypeCode + ")",
                  LeaveTypeCode = p.LeaveTypeCode,
                  MaxLeaveRange = p.MaxLeaveRange,
                  MinLeaveRange = p.MinLeaveRange
              }).ToList();
    }

    [HttpGet]
    public async Task<List<SalaryComponentKeyValues>> SalaryCompnentKeyValue(Expression<Func<SalaryComponentMaster, bool>>? expression = null)
    {
        return (await _unitOfWork.SalaryComponentMaster.GetAll(expression, orderBy: (m => m.OrderBy(x => x.SalaryComponentTitle))).ConfigureAwait(true))
              .Select(p => new SalaryComponentKeyValues()
              {
                  SalaryComponentId = p.SalaryComponentId,
                  SalaryComponentTitle = p.SalaryComponentTitle,
              }).ToList();
    }

    [HttpGet]
    public async Task<List<SalaryTemplateKeyValues>> SalaryTemplateKeyValue(Expression<Func<SalaryTemplate, bool>>? expression = null)
    {
        return (await _unitOfWork.SalaryTemplate.GetAll(expression, orderBy: (m => m.OrderBy(x => x.TemplateName))).ConfigureAwait(true))
              .Select(p => new SalaryTemplateKeyValues()
              {
                  SalaryTemplateId = p.SalaryTemplateId,
                  TemplateName = p.TemplateName,
                  UnitId = p.UnitId,
              }).ToList();
    }

    [HttpGet]
    public async Task<List<PageKeyValues>> PageControlKeyValue(Expression<Func<PageControlKeyValue, bool>>? expression = null)
    {
        return (await _unitOfWork.PageControlKeyValues.GetAll(expression, orderBy: (m => m.OrderBy(x => x.KeyValue))).ConfigureAwait(true))
              .Select(p => new PageKeyValues()
              {
                  KeyValueId = p.KeyValueId,
                  KeyName = p.KeyName,
                  KeyValue = p.KeyValue,
                  Module = p.Module,
                  PageName = p.PageName,
                  ControlName = p.ControlName
              }).OrderBy(x => x.KeyValueId).ToList();
    }


    [HttpGet]
    public async Task<List<EmployeeValidationKeyValues>> EmployeeValidationKeyValue(List<EmployeeValidationDTO> empValidationList)
    {
        return empValidationList.Select(p => new EmployeeValidationKeyValues()
        {
            ScreenName = p.ScreenName,
            ScreenTab = p.ScreenTab,
            TabSequence = p.TabSequence,
            FieldName = p.FieldName,
            DisplayText = p.DisplayText,
            EmployeeValidationId = p.EmployeeValidationId,
            AddAttachment = p.AddAttachment,
            EditAttachment = p.EditAttachment,
            IsMandatory = p.IsMandatory
        }).OrderBy(x => x.EmployeeValidationId).ToList();
    }

    [HttpGet]
    public async Task<List<EmployeeValidationKeyValues>> EmployeeValidationKeyValue(string screenName, string? screenTab = "", int? clientId = 0, int? unitId = 0)
    {

        List<EmployeeValidationDTO> empValidationList = new List<EmployeeValidationDTO>();
        var parms = new DynamicParameters();
        parms.Add(@"@unitId", unitId, DbType.Int32);
        parms.Add(@"@ClientId", clientId, DbType.Int32);
        parms.Add(@"@ScreenName", screenName, DbType.String);
        parms.Add(@"@ScreenTab", screenTab, DbType.String);
        empValidationList = _mapper.Map<List<EmployeeValidationDTO>>(await _unitOfWork.EmployeeValidation.GetSPData("GetEmployeeValidation", parms));
        return (await EmployeeValidationKeyValue(empValidationList));
    }

    [HttpGet]
    public async Task<List<LanguageKeyValues>> LanguageKeyValue(Expression<Func<LanguageUnitMaster, bool>>? expression = null, Func<IQueryable<LanguageUnitMaster>, IOrderedQueryable<LanguageUnitMaster>> orderBy = null)
    {
        return (await _unitOfWork.LanguageUnitMaster.GetAll(expression, orderBy))
        .Select(p => new LanguageKeyValues()
        {
            LanguageId = p.LanguageId,
            Language = p.Language,
            UnitId = p.UnitId
        }).OrderBy(x => x.Language).ToList();
    }

    //[HttpGet]
    //public async Task<List<EmployeeValidationKeyValues>> EmployeeValidationKeyValue(Expression<Func<EmployeeValidation, bool>>? expression = null, Expression<Func<EmployeeValidation, bool>> orderBy = null)
    //{
    //    return (await _unitOfWork.EmployeeValidation.GetFilterAll(expression).ConfigureAwait(true))
    //         .Select(p => new EmployeeValidationKeyValues()
    //         {
    //             ScreenName = p.ScreenName,
    //             ScreenTab = p.ScreenTab,
    //             TabSequence = p.TabSequence,
    //             FieldName = p.FieldName,
    //             DisplayText = p.DisplayText,
    //             EmployeeValidationId = p.EmployeeValidationId,
    //             AddAttachment = p.AddAttachment,
    //             EditAttachment = p.EditAttachment,
    //             IsMandatory = p.IsMandatory
    //         }).OrderBy(x => x.EmployeeValidationId).ToList();
    //}

}
