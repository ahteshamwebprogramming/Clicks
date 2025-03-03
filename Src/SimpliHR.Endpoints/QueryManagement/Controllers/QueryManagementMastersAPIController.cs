using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Complaint;
using SimpliHR.Infrastructure.Models.Master;

namespace SimpliHR.Endpoints.QueryManagement;

[Route("api/[controller]")]
[ApiController]
public class QueryManagementMastersAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QueryManagementMastersAPIController> _logger;
    private readonly IMapper _mapper;
    public QueryManagementMastersAPIController(IUnitOfWork unitOfWork, ILogger<QueryManagementMastersAPIController> logger, IMapper mapper)
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
    public async Task<IActionResult> GetComplaintCategoryById(int Id)
    {
        try
        {
            var res = await _unitOfWork.ComplaintCategory.FindByIdAsync(Id);
            ComplaintCategoryDTO dto = _mapper.Map<ComplaintCategoryDTO>(res);
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetComplaintCategoryById)}");
            throw;
        }
    }
}
