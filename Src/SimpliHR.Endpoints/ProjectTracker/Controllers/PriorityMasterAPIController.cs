using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.ProjectTracker.Masters;
using SimpliHR.Infrastructure.Models.ProjectTracker;

namespace ProjectTracker.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PriorityMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PriorityMasterAPIController> _logger;
    private readonly IMapper _mapper;

    public PriorityMasterAPIController(IUnitOfWork unitOfWork, ILogger<PriorityMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> GetProjectPriorityList(int UnitId)
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetProjectPriorityList)}");
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetPriorities)}");
            throw;
        }
    }
    public async Task<IActionResult> GetProjectPriorityById(int ProjectCategoryId)
    {
        try
        {
            var res = await _unitOfWork.PTProjectPriority.FindByIdAsync(ProjectCategoryId);
            PTProjectPriorityDTO dto = _mapper.Map<PTProjectPriorityDTO>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetProjectPriorityById)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> Save(PTProjectPriorityDTO inputDTO)
    {
        try
        {
            if (inputDTO.Equals(null))
            {
                return BadRequest("Data is invalid");
            }

            if (inputDTO.PriorityId > 0)
            {
                var resExist = await _unitOfWork.PTProjectPriority.GetFilter(x => x.IsActive == true && x.UnitId == inputDTO.UnitId && x.Priority.Trim().ToUpper() == inputDTO.Priority.Trim().ToUpper() && x.PriorityId != inputDTO.PriorityId);
                if (resExist == null)
                {
                    var resProjectPriority = await _unitOfWork.PTProjectPriority.FindByIdAsync(inputDTO.PriorityId);
                    inputDTO.UnitId = resProjectPriority.UnitId;
                    inputDTO.CreatedDate = resProjectPriority.CreatedDate;
                    inputDTO.CreatedBy = resProjectPriority.CreatedBy;
                    await _unitOfWork.PTProjectPriority.UpdateAsync(_mapper.Map<PTProjectPriority>(inputDTO));
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
                var resExist = await _unitOfWork.PTProjectPriority.GetFilter(x => x.IsActive == true && x.UnitId == inputDTO.UnitId && x.Priority.Trim().ToUpper() == inputDTO.Priority.Trim().ToUpper());
                if (resExist == null)
                {
                    int inserted = await _unitOfWork.PTProjectPriority.AddAsync(_mapper.Map<PTProjectPriority>(inputDTO));
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

    public async Task<IActionResult> DeleteProjectPriorityById(int PriorityId)
    {
        try
        {
            var res = await _unitOfWork.PTProjectPriority.FindByIdAsync(PriorityId);
            res.IsActive = false;
            await _unitOfWork.PTProjectPriority.UpdateAsync(res);
            _unitOfWork.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Language {nameof(DeleteProjectPriorityById)}");
            throw;
        }
    }
}
