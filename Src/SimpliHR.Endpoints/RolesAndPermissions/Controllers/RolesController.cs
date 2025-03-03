using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Services.DBContext;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.Endpoints.RolesAndPermissions;

[EnableCors()]
[Route("api/[controller]/[action]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly SimpliDbContext _simpliDbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RolesController> _logger;
    private readonly IMapper _mapper;

    public RolesController(IUnitOfWork unitOfWork, ILogger<RolesController> logger, IMapper mapper, SimpliDbContext simpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = simpliDbContext;
    }

    //[HttpPost(Name = "GetJobTitles")]
    public async Task<IActionResult> GetJobTitles(Core.Helper.RequestParams requestParams = null)
    {
        try
        {
            IList<JobTitleMasterDTO> outputModel = new List<JobTitleMasterDTO>();
            outputModel = _mapper.Map<IList<JobTitleMasterDTO>>(_unitOfWork.JobTitleMaster.GetAll(null, p => p.IsActive == true).Result.ToList());
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetJobTitles)}");
            throw;
        }
    }
    public async Task<IActionResult> GetJobTitlesUnitWise( int unitId, Core.Helper.RequestParams requestParams = null)
    {
        try
        {
            IList<JobTitleMasterDTO> outputModel = new List<JobTitleMasterDTO>();
            outputModel = _mapper.Map<IList<JobTitleMasterDTO>>(_unitOfWork.JobTitleMaster.GetAll(null, p => p.IsActive == true && p.UnitId == unitId).Result.ToList());
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetJobTitles)}");
            throw;
        }
    }

    //[HttpPost(Name = "GetRoles")]
    public async Task<IActionResult> GetRoles(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<RoleMasterDTO> outputModel = new List<RoleMasterDTO>();
            outputModel = _mapper.Map<IList<RoleMasterDTO>>(_unitOfWork.RoleMaster.GetAll(null, p => p.IsActive == true).Result.ToList());
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetRoles)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRolesUnitWise(Core.Helper.RequestParams requestParams, int unitId)
    {
        try
        {
            IList<RoleMasterDTO> outputModel = new List<RoleMasterDTO>();
            outputModel = _mapper.Map<IList<RoleMasterDTO>>(_unitOfWork.RoleMaster.GetAll(null, p => p.IsActive == true && p.UnitId == unitId).Result.ToList());
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetRoles)}");
            throw;
        }
    }
    public async Task<IActionResult> GetDepartment(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<DepartmentMasterDTO> outputModel = new List<DepartmentMasterDTO>();
            outputModel = _mapper.Map<IList<DepartmentMasterDTO>>(_unitOfWork.DepartmentMaster.GetAll(null, p => p.IsActive == true).Result.ToList());
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetRoles)}");
            throw;
        }
    }
    public async Task<IActionResult> GetDepartmentUnitWise(Core.Helper.RequestParams requestParams, int unitId)
    {
        try
        {
            IList<DepartmentMasterDTO> outputModel = new List<DepartmentMasterDTO>();
            outputModel = _mapper.Map<IList<DepartmentMasterDTO>>(_unitOfWork.DepartmentMaster.GetAll(null, p => p.IsActive == true && p.UnitId == unitId).Result.ToList());
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetRoles)}");
            throw;
        }
    }
    //[HttpPost(Name = "GetMenus")]
    public async Task<IActionResult> GetMenus()
    {
        try
        {
            IList<MenuMasterDTO> outputModel = new List<MenuMasterDTO>();
            outputModel = _mapper.Map<IList<MenuMasterDTO>>(_unitOfWork.MenuMaster.GetAll(p => p.IsActive == 1, orderBy: (m => m.OrderBy(p => p.Sn))).Result.ToList());
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetMenus)}");
            throw;
        }
    }
    //[HttpPost(Name = "GetRoleMenuMappings")]
    public async Task<IActionResult> GetRoleMenuMappings(RoleMenuMappingDTO roleMenuMappingDTO, bool isClient = false, int? clientId = null, int? unitId = null)
    {
        try
        {

            IList<RoleMenuMappingDTO> outputModel = new List<RoleMenuMappingDTO>();
            //outputModel = _mapper.Map<IList<RoleMenuMappingDTO>>(_unitOfWork.RoleMenuMapping.GetAll((new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }), p => p.IsActive == 1 && p.JobTitleId == roleMenuMappingDTO.JobTitleId && p.RoleId == roleMenuMappingDTO.RoleId).Result.ToList());
            //outputModel = _mapper.Map<IList<RoleMenuMappingDTO>>(_unitOfWork.RoleMenuMapping.GetAll((new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }), p => p.IsActive == 1 && p.JobTitleId == roleMenuMappingDTO.JobTitleId && p.DepartmentId == roleMenuMappingDTO.DepartmentId).Result.ToList());

            if (isClient)
            {
                outputModel = _mapper.Map<IList<RoleMenuMappingDTO>>(_unitOfWork.RoleMenuMapping.GetAll(p => p.IsActive == 1 && p.JobTitleId == roleMenuMappingDTO.JobTitleId && p.DepartmentId == roleMenuMappingDTO.DepartmentId && p.RoleId == roleMenuMappingDTO.RoleId, null).Result.ToList());
                return Ok(outputModel);
            }
            else
            {
                outputModel = _mapper.Map<IList<RoleMenuMappingDTO>>(_unitOfWork.RoleMenuMapping.GetAll(p => p.IsActive == 1 && p.JobTitleId == roleMenuMappingDTO.JobTitleId && p.DepartmentId == roleMenuMappingDTO.DepartmentId && p.RoleId == roleMenuMappingDTO.RoleId && p.UnitId == unitId && p.ClientId == clientId, null).Result.ToList());
                return Ok(outputModel);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetRoleMenuMappings)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult DeleteMappingByJobTitleIdRoleId(ListRoleMenuMappingDTOForSave data)
    {
        try
        {
            List<RoleMenuMapping> roleMenus = _unitOfWork.RoleMenuMapping.FindAllByExpression(x => x.JobTitleId == data.JobTitleId && x.DepartmentId == data.DepartmentId && x.RoleId == data.RoleId && x.ClientId == data.ClientId && x.UnitId == data.UnitId).ToList();

            foreach (var roleMenu in roleMenus)
            {
                roleMenu.IsActive = 0;
            }
            _unitOfWork.RoleMenuMapping.UpdateRange(roleMenus);
            _unitOfWork.Save();
            //foreach (var roleMenu in roleMenus)
            //{
            //    roleMenu.IsActive = 0;
            //    _unitOfWork.RoleMenuMapping.Update(roleMenu);
            //    _unitOfWork.Save();
            //}
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee {nameof(DeleteMappingByJobTitleIdRoleId)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult SaveMenuMapping(RoleMenuMappingDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<RoleMenuMapping, bool>> expression = (a => ((a.JobTitleId == inputDTO.JobTitleId && a.RoleId == inputDTO.RoleId && a.DepartmentId == inputDTO.DepartmentId && a.MenuId == inputDTO.MenuId && a.ClientId == inputDTO.ClientId && a.UnitId == inputDTO.UnitId && a.IsActive == 1)));
                if (!_unitOfWork.RoleMenuMapping.Exists(expression))
                {

                    _unitOfWork.RoleMenuMapping.AddAsync(_mapper.Map<RoleMenuMapping>(inputDTO));
                    _unitOfWork.Save();
                }
                return Ok();
            }
            else
            {
                throw new Exception("Not authorised");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SaveMenuMapping)}");
            throw;
        }
    }

}

