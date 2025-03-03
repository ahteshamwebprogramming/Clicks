using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class AnnouncementTypeMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AnnouncementTypeMasterAPIController> _logger;
    private readonly IMapper _mapper;

    public AnnouncementTypeMasterAPIController(IUnitOfWork unitOfWork, ILogger<AnnouncementTypeMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }


    public async Task<IActionResult> GetAnnouncementTypeList(int UnitId)
    {
        try
        {
            var res = await _unitOfWork.AnnouncementTypeMaster.GetFilterAll(x => x.IsActive == true && x.UnitId == UnitId);
            List<AnnouncementTypeMasterDTO> dto = _mapper.Map<List<AnnouncementTypeMasterDTO>>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetAnnouncementTypeList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAnnouncementTypeById(int AnnouncementTypeId)
    {
        try
        {
            var res = await _unitOfWork.AnnouncementTypeMaster.FindByIdAsync(AnnouncementTypeId);
            AnnouncementTypeMasterDTO dto = _mapper.Map<AnnouncementTypeMasterDTO>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetAnnouncementTypeList)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> Save(AnnouncementTypeMasterDTO inputDTO)
    {
        try
        {
            if (inputDTO.Equals(null))
            {
                return BadRequest("Data is invalid");
            }

            if (inputDTO.AnnouncementTypeId > 0)
            {
                var resExist = await _unitOfWork.AnnouncementTypeMaster.GetFilter(x => x.IsActive == true && x.UnitId == inputDTO.UnitId && x.AnnouncementType.Trim().ToUpper() == inputDTO.AnnouncementType.Trim().ToUpper() && x.AnnouncementTypeId != inputDTO.AnnouncementTypeId);
                if (resExist == null)
                {
                    var resAnnoncementType = await _unitOfWork.AnnouncementTypeMaster.FindByIdAsync(inputDTO.AnnouncementTypeId);
                    inputDTO.UnitId = resAnnoncementType.UnitId;
                    inputDTO.CreatedDate = resAnnoncementType.CreatedDate;
                    inputDTO.CreatedBy = resAnnoncementType.CreatedBy;
                    await _unitOfWork.AnnouncementTypeMaster.UpdateAsync(_mapper.Map<AnnouncementTypeMaster>(inputDTO));
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
                var resExist = await _unitOfWork.AnnouncementTypeMaster.GetFilter(x => x.IsActive == true && x.UnitId == inputDTO.UnitId && x.AnnouncementType.Trim().ToUpper() == inputDTO.AnnouncementType.Trim().ToUpper());
                if (resExist == null)
                {
                    await _unitOfWork.AnnouncementTypeMaster.AddAsync(_mapper.Map<AnnouncementTypeMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in AnnouncementTypeMasterAPIController  {nameof(Save)}");
            throw;
        }
    }

    public async Task<IActionResult> DeleteAnnouncementTypeById(int AnnouncementTypeId)
    {
        try
        {
            var res = await _unitOfWork.AnnouncementTypeMaster.FindByIdAsync(AnnouncementTypeId);
            res.IsActive = false;
            await _unitOfWork.AnnouncementTypeMaster.UpdateAsync(res);
            _unitOfWork.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetAnnouncementTypeList)}");
            throw;
        }
    }
}
