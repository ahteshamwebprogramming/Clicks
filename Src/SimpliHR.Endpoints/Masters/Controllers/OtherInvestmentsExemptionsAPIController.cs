using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class OtherInvestmentsExemptionsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OtherInvestmentsExemptionsAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _simpliDbContext;

    public OtherInvestmentsExemptionsAPIController(IUnitOfWork unitOfWork, ILogger<OtherInvestmentsExemptionsAPIController> logger, IMapper mapper, SimpliDbContext SimpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = SimpliDbContext;
    }
    [HttpPost]
    public async Task<IActionResult> GetOthersExemptionsInvestments()
    {
        try
        {
            var res = _mapper.Map<IList<OtherInvestmentExemptionDTO>>(await _unitOfWork.OtherInvestmentExemption.GetAll(x => x.IsActive == true));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetOthersExemptionsInvestments)}");
            throw;
        }
    }
}
