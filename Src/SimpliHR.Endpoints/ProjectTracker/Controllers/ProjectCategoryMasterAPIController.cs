using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.ProjectTracker;

namespace SimpliHR.Endpoints.ProjectTracker.Masters;

[Route("api/[controller]")]
[ApiController]
public class ProjectCategoryMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectCategoryMasterAPIController> _logger;
    private readonly IMapper _mapper;

    public ProjectCategoryMasterAPIController(IUnitOfWork unitOfWork, ILogger<ProjectCategoryMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> GetProjectCategoryList(int UnitId)
    {
        try
        {
            var res = await _unitOfWork.PTProjectCategory.GetFilterAll(x => x.IsActive == true && x.UnitId == UnitId);
            List<PTProjectCategoryDTO> dto = _mapper.Map<List<PTProjectCategoryDTO>>(res);
            if (dto != null && dto.Count > 0)
            {
                return Ok(dto);
            }
            else
            {
                return BadRequest("No Data Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetProjectCategoryList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetProjectStatusList(int UnitId)
    {
        try
        {
            var res = await _unitOfWork.PTStatus.GetFilterAll(x => x.IsActive == true && x.StatusType == "ProjectStatus");
            List<PTStatusDTO> dto = _mapper.Map<List<PTStatusDTO>>(res);
            if (dto != null && dto.Count > 0)
            {
                return Ok(dto);
            }
            else
            {
                return BadRequest("No Data Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetProjectCategoryList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPriorities(int UnitId)
    {
        try
        {
            var res = await _unitOfWork.PTProjectPriority.GetFilterAll(x => x.IsActive == true && x.UnitId == UnitId);
            List<PTProjectPriorityDTO> dto = _mapper.Map<List<PTProjectPriorityDTO>>(res);
            if (dto != null && dto.Count > 0)
            {
                return Ok(dto);
            }
            else
            {
                return BadRequest("No Data Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetProjectCategoryList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetProjectCategoryById(int ProjectCategoryId)
    {
        try
        {
            var res = await _unitOfWork.PTProjectCategory.FindByIdAsync(ProjectCategoryId);
            PTProjectCategoryDTO dto = _mapper.Map<PTProjectCategoryDTO>(res);
            if (dto != null)
            {
                return Ok(dto);
            }
            else
            {
                return BadRequest("No Data Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetProjectCategoryById)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> Save(PTProjectCategoryDTO inputDTO)
    {
        try
        {
            if (inputDTO.Equals(null))
            {
                return BadRequest("Data is invalid");
            }

            if (inputDTO.CategoryID > 0)
            {
                var resExist = await _unitOfWork.PTProjectCategory.GetFilter(x => x.IsActive == true && x.UnitId == inputDTO.UnitId && x.CategoryName.Trim().ToUpper() == inputDTO.CategoryName.Trim().ToUpper() && x.CategoryID != inputDTO.CategoryID);
                if (resExist == null)
                {
                    var resProjectCategory = await _unitOfWork.PTProjectCategory.FindByIdAsync(inputDTO.CategoryID);
                    inputDTO.UnitId = resProjectCategory.UnitId;
                    inputDTO.CreatedDate = resProjectCategory.CreatedDate;
                    inputDTO.CreatedBy = resProjectCategory.CreatedBy;
                    await _unitOfWork.PTProjectCategory.UpdateAsync(_mapper.Map<PTProjectCategory>(inputDTO));
                    _unitOfWork.Save();
                    return Ok();
                }
                else
                {
                    return BadRequest("Duplicate Entry Found");
                }
            }
            else
            {
                var resExist = await _unitOfWork.PTProjectCategory.GetFilter(x => x.IsActive == true && x.UnitId == inputDTO.UnitId && x.CategoryName.Trim().ToUpper() == inputDTO.CategoryName.Trim().ToUpper());
                if (resExist == null)
                {
                    int inserted = await _unitOfWork.PTProjectCategory.AddAsync(_mapper.Map<PTProjectCategory>(inputDTO));
                    if (inserted == 0)
                    {
                        return BadRequest("Error while saving the data");
                    }
                    _unitOfWork.Save();
                    return Ok();
                }
                else
                {
                    return BadRequest("Duplicate Entry Found");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in NewTypeMasterAPIController  {nameof(Save)}");
            throw;
        }
    }

    public async Task<IActionResult> DeleteProjectCategoryById(int ProjectCategoryId)
    {
        try
        {
            var res = await _unitOfWork.PTProjectCategory.FindByIdAsync(ProjectCategoryId);
            res.IsActive = false;
            await _unitOfWork.PTProjectCategory.UpdateAsync(res);
            _unitOfWork.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Language {nameof(DeleteProjectCategoryById)}");
            throw;
        }
    }
}
