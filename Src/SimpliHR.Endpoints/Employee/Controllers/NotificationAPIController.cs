using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Employee;

namespace SimpliHR.Endpoints.Employee;

[Route("api/[controller]")]
[ApiController]
public class NotificationAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationAPIController> _logger;
    private readonly IMapper _mapper;
    public NotificationAPIController(IUnitOfWork unitOfWork, ILogger<NotificationAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetNotifications(int EmployeeId)
    {
        try
        {

            List<AppMessageDTO> appMessageDTOs = _mapper.Map<List<AppMessageDTO>>(await _unitOfWork.AppMessage.GetFilterAll(x => x.TargetEmployeeId == EmployeeId && x.IsActive == true));
            return Ok(appMessageDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetNotifications)}");
            throw;
        }
    }
    public async Task<IActionResult> ClearNotification(int Id)
    {
        try
        {
            AppMessage appMessage = await _unitOfWork.AppMessage.FindByIdAsync(Id);
            if (appMessage != null)
            {
                appMessage.IsActive = false;
                appMessage.IsViewed = true;
                await _unitOfWork.AppMessage.UpdateAsync(appMessage);
                //_unitOfWork.Save();
                return Ok(appMessage);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetNotifications)}");
            throw;
        }
    }

}
