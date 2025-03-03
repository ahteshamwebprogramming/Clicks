using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class Exemptions80DAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<Exemptions80DAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _simpliDbContext;

    public Exemptions80DAPIController(IUnitOfWork unitOfWork, ILogger<Exemptions80DAPIController> logger, IMapper mapper, SimpliDbContext SimpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = SimpliDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> GetExemptions80D()
    {
        try
        {
            var res = _mapper.Map<IList<Exemptions80DDTO>>(await _unitOfWork.Exemptions80D.GetAll(x => x.IsActive == true));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetExemptions80D)}");
            throw;
        }
    }
}
