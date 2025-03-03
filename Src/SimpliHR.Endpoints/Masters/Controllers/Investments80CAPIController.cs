using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class Investments80CAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<Investments80CAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _simpliDbContext;

    public Investments80CAPIController(IUnitOfWork unitOfWork, ILogger<Investments80CAPIController> logger, IMapper mapper, SimpliDbContext SimpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = SimpliDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> GetInvestments80C()
    {
        try
        {
            var res = _mapper.Map<IList<Investment80CmasterDTO>>(await _unitOfWork.Investment80Cmaster.GetAll(x => x.IsActive==true));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetInvestments80C)}");
            throw;
        }
    }
}
