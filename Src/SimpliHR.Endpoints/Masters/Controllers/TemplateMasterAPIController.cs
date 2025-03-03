using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.ProfileEditAuth;
using System.Linq.Expressions;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class TemplateMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TemplateMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public TemplateMasterAPIController(IUnitOfWork unitOfWork, ILogger<TemplateMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> GetTemplateList()
    {
        try
        {
            List<TemplateMasterDynamic> outputModel = new List<TemplateMasterDynamic>();
            string sQuery = "select * from TemplateMasterDynamic where isactive=1";
            List<TemplateMasterDynamicDTO> dto = await _unitOfWork.TemplateMasterDynamic.GetTableData<TemplateMasterDynamicDTO>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetTemplateList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetTemplateListExitInterview()
    {
        try
        {
            List<TemplateMasterDynamic> outputModel = new List<TemplateMasterDynamic>();
            string sQuery = "select * from TemplateMasterDynamic where isactive=1 and formtype='ExitInterview'";
            List<TemplateMasterDynamicDTO> dto = await _unitOfWork.TemplateMasterDynamic.GetTableData<TemplateMasterDynamicDTO>(sQuery);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetTemplateList)}");
            throw;
        }
    }
    public async Task<IActionResult> GetTemplateFormById(TemplateMasterDynamicDTO inputDTO)
    {
        try
        {
            TemplateMasterDynamicDTO res = _mapper.Map<TemplateMasterDynamicDTO>(await _unitOfWork.TemplateMasterDynamic.FindByIdAsync(inputDTO.TemplateMasterDynamicId));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetTemplateList)}");
            throw;
        }
    }
    public async Task<ActionResult> SaveTemplateFormComponent(TemplateMasterDynamicDTO inputDTO)
    {
        if (inputDTO.TemplateMasterDynamicId > 0)
        {
            List<Core.Entities.TemplateMasterDynamic> ecm = await _unitOfWork.TemplateMasterDynamic.GetQueryAll("select 1 a from Templatemasterdynamic where formname='" + inputDTO.FormName + "' and formtype='" + inputDTO.FormType + "' and isactive=1 and templatemasterdynamicid!=" + inputDTO.TemplateMasterDynamicId + "");
            if (ecm.Count == 0)
            {
                await _unitOfWork.TemplateMasterDynamic.UpdateAsync(_mapper.Map<TemplateMasterDynamic>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry Found");
            }
        }
        else
        {
            List<Core.Entities.TemplateMasterDynamic> ecm = await _unitOfWork.TemplateMasterDynamic.GetQueryAll("select 1 a from Templatemasterdynamic where formname='" + inputDTO.FormName + "' and formtype='" + inputDTO.FormType + "' and isactive=1");

            if (ecm.Count == 0)
            {
                await _unitOfWork.TemplateMasterDynamic.AddAsync(_mapper.Map<TemplateMasterDynamic>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry Found");
            }

        }
    }
}
