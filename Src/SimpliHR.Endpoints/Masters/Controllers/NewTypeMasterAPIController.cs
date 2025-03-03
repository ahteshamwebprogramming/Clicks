using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Master;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class NewTypeMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NewTypeMasterAPIController> _logger;
    private readonly IMapper _mapper;

    public NewTypeMasterAPIController(IUnitOfWork unitOfWork, ILogger<NewTypeMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
   
    public async Task<IActionResult> GetNewsTypeList(int UnitId)
    {
        try
        {
            var res = await _unitOfWork.NewsCategoryTagMaster.GetFilterAll(x => x.IsActive == true && x.UnitId == UnitId);
            List<NewsCategoryTagMasterDTO> dto = _mapper.Map<List<NewsCategoryTagMasterDTO>>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetNewsTypeList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetNewsTypeById(int NewsCategoryTagId)
    {
        try
        {
            var res = await _unitOfWork.NewsCategoryTagMaster.FindByIdAsync(NewsCategoryTagId);
            NewsCategoryTagMasterDTO dto = _mapper.Map<NewsCategoryTagMasterDTO>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetNewsTypeById)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> Save(NewsCategoryTagMasterDTO inputDTO)
    {
        try
        {
            if (inputDTO.Equals(null))
            {
                return BadRequest("Data is invalid");
            }

            if (inputDTO.NewsCategoryTagId > 0)
            {
                var resExist = await _unitOfWork.NewsCategoryTagMaster.GetFilter(x => x.IsActive == true && x.UnitId == inputDTO.UnitId && x.NewsCategoryTag.Trim().ToUpper() == inputDTO.NewsCategoryTag.Trim().ToUpper() && x.NewsCategoryTagId != inputDTO.NewsCategoryTagId);
                if (resExist == null)
                {
                    var resNewsCategoryTagType = await _unitOfWork.NewsCategoryTagMaster.FindByIdAsync(inputDTO.NewsCategoryTagId);
                    inputDTO.UnitId = resNewsCategoryTagType.UnitId;
                    inputDTO.CreatedDate = resNewsCategoryTagType.CreatedDate;
                    inputDTO.CreatedBy = resNewsCategoryTagType.CreatedBy;
                    await _unitOfWork.NewsCategoryTagMaster.UpdateAsync(_mapper.Map<NewsCategoryTagMaster>(inputDTO));
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
                var resExist = await _unitOfWork.NewsCategoryTagMaster.GetFilter(x => x.IsActive == true && x.UnitId == inputDTO.UnitId && x.NewsCategoryTag.Trim().ToUpper() == inputDTO.NewsCategoryTag.Trim().ToUpper());
                if (resExist == null)
                {
                    await _unitOfWork.NewsCategoryTagMaster.AddAsync(_mapper.Map<NewsCategoryTagMaster>(inputDTO));
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

    public async Task<IActionResult> DeleteNewsTypeById(int NewsCategoryTagId)
    {
        try
        {
            var res = await _unitOfWork.NewsCategoryTagMaster.FindByIdAsync(NewsCategoryTagId);
            res.IsActive = false;
            await _unitOfWork.NewsCategoryTagMaster.UpdateAsync(res);
            _unitOfWork.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetNewsTypeList)}");
            throw;
        }
    }
}
