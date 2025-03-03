using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class MenuMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MenuMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public MenuMasterAPIController(IUnitOfWork unitOfWork, ILogger<MenuMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    [HttpPost(Name = "GetMenuMaster")]
    public async Task<IActionResult> GetMenuMaster(int parentMenuId = -1)
    {
        try
        {
            IList<MenuMasterDTO> outputModel = new List<MenuMasterDTO>();
            if (parentMenuId > -1)
            {
                outputModel = _mapper.Map<IList<MenuMasterDTO>>(await _unitOfWork.MenuMaster.GetAll(p => p.IsActive == 1 && p.ParentMenuId == parentMenuId, orderBy: (m => m.OrderBy(p => p.ParentMenuId))));
            }
            else
            {
                outputModel = _mapper.Map<IList<MenuMasterDTO>>(await _unitOfWork.MenuMaster.GetAll(p => p.IsActive == 1, orderBy: (m => m.OrderBy(p => p.ParentMenuId))));
            }
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetMenuMaster)}");
            throw;
        }
    }
    [HttpPost(Name = "GetMenuById")]
    public async Task<IActionResult> GetMenuById(int MenuId)
    {
        try
        {
            MenuMasterDTO outputModel = new MenuMasterDTO();

            outputModel = _mapper.Map<MenuMasterDTO>(await _unitOfWork.MenuMaster.GetByIdAsync(MenuId));

            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetMenuMaster)}");
            throw;
        }
    }
  
    public async Task<IActionResult> DeleteMenuMaster(int MenuId)
    {
        try
        {

            MenuMaster outputModel = _mapper.Map<MenuMaster>(await _unitOfWork.MenuMaster.GetByIdAsync(MenuId));
            outputModel.IsActive = 0;
            _unitOfWork.MenuMaster.Update(outputModel);
            _unitOfWork.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetMenuMaster)}");
            throw;
        }
    }
    [HttpPost(Name = "SaveMenuMaster")]
    public async Task<IActionResult> SaveMenuMaster(MenuMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.MenuId == 0)
                {
                    //Expression<Func<ItDeclarationHouseRentDetail, bool>> expression = a => a.UnitId == empInput.UnitId  ;
                    //if (!_unitOfWork.ItDeclarationHouseRentDetail.Exists(expression))
                    //{
                    _unitOfWork.MenuMaster.AddAsync(_mapper.Map<MenuMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.MenuMaster.Update(_mapper.Map<MenuMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveMenuMaster)}");
            throw;
        }
    }
}
