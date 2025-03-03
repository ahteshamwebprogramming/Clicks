using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Infrastructure.Models.Complaint;
using SimpliHR.Infrastructure.Models.Master;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class ComplaintMastersAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ComplaintMastersAPIController> _logger;
    private readonly IMapper _mapper;

    public ComplaintMastersAPIController(IUnitOfWork unitOfWork, ILogger<ComplaintMastersAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetComplaintPriorityList(int UnitId)
    {
        try
        {
            var res = await _unitOfWork.ComplaintPriority.GetFilterAll(x => x.IsActive == true && x.UnitId == UnitId);
            List<ComplaintPriorityDTO> dto = _mapper.Map<List<ComplaintPriorityDTO>>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetComplaintPriorityList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetComplaintStatusList(int UnitId)
    {
        try
        {
            var res = await _unitOfWork.ComplaintStatus.GetFilterAll(x => x.IsActive == true && x.UnitId == UnitId);
            List<ComplaintStatusDTO> dto = _mapper.Map<List<ComplaintStatusDTO>>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetComplaintStatusList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetComplaintCategoryList(int UnitId)
    {
        try
        {
            var res = await _unitOfWork.ComplaintCategory.GetFilterAll(x => x.IsActive == true && x.UnitId == UnitId);
            List<ComplaintCategoryDTO> dto = _mapper.Map<List<ComplaintCategoryDTO>>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetComplaintCategoryList)}");
            throw;
        }
    }
}
