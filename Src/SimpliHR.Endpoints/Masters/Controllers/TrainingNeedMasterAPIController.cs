using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Master;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class TrainingNeedMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TrainingNeedMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public TrainingNeedMasterAPIController(IUnitOfWork unitOfWork, ILogger<TrainingNeedMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> TrainingNeedMasterList(int UnitId, int PerformanceSettingId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                List<PerformanceTrainingNeedsMasterDTO> performanceTrainingNeedsMasterDTOs = _mapper.Map<List<PerformanceTrainingNeedsMasterDTO>>(await _unitOfWork.PerformanceTrainingNeedsMaster.GetFilterAll(x => x.PerformanceSettingId == PerformanceSettingId && x.UnitId == UnitId && x.IsActive==true));
                return Ok(performanceTrainingNeedsMasterDTOs);
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(TrainingNeedMasterList)}");
            throw;
        }
    }
    public async Task<IActionResult> TrainingNeedMasterById(int TrainingNeedsMasterId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                PerformanceTrainingNeedsMasterDTO dto = _mapper.Map<PerformanceTrainingNeedsMasterDTO>(await _unitOfWork.PerformanceTrainingNeedsMaster.FindByIdAsync(TrainingNeedsMasterId));
                if (dto != null)
                {
                    return Ok(dto);
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(TrainingNeedMasterById)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveTrainingNeedMaster(PerformanceTrainingNeedsMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.TrainingNeedsMasterId == 0)
                {

                    List<PerformanceTrainingNeedsMaster> ecm = await _unitOfWork.PerformanceTrainingNeedsMaster.GetQueryAll(@"select 1 a from [dbo].[PerformanceTrainingNeedsMaster] where isactive=1 and unitid=" + inputDTO.UnitId + " and PerformanceSettingId=" + inputDTO.PerformanceSettingId + " and Training='" + inputDTO.Training + "'");
                    if (ecm.Count == 0)
                    {
                        int insertedId = await _unitOfWork.PerformanceTrainingNeedsMaster.AddAsync(_mapper.Map<PerformanceTrainingNeedsMaster>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Already exists");
                    }
                }
                else
                {
                    List<PerformanceTrainingNeedsMaster> ecm = await _unitOfWork.PerformanceTrainingNeedsMaster.GetQueryAll(@"select 1 a from [dbo].[PerformanceTrainingNeedsMaster] where isactive=1 and unitid=" + inputDTO.UnitId + " and PerformanceSettingId=" + inputDTO.PerformanceSettingId + " and TrainingNeedsMasterId !=" + inputDTO.TrainingNeedsMasterId + " and Training='" + inputDTO.Training + "'");
                    if (ecm.Count == 0)
                    {
                        await _unitOfWork.PerformanceTrainingNeedsMaster.UpdateAsync(_mapper.Map<PerformanceTrainingNeedsMaster>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Already exists");
                    }
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveTrainingNeedMaster)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteTrainingNeedMaster(PerformanceTrainingNeedsMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                PerformanceTrainingNeedsMaster res = await _unitOfWork.PerformanceTrainingNeedsMaster.FindByIdAsync(inputDTO.TrainingNeedsMasterId);
                res.IsActive = false;
                bool isSuccess = await _unitOfWork.PerformanceTrainingNeedsMaster.UpdateAsync(res);
                if (isSuccess)
                {
                    return Ok("Deleted");
                }
                else
                {
                    return BadRequest("Error");
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(DeleteTrainingNeedMaster)}");
            throw;
        }
    }
}
