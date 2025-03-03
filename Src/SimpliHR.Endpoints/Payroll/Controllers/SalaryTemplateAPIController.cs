using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Infrastructure.Models.StatutoryComponent;
using SimpliHR.Services.DBContext;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.Endpoints.Payroll;

[Route("api/[controller]")]
[ApiController]
public class SalaryTemplateAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SalaryTemplateAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _simpliDbContext;
    public SalaryTemplateAPIController(IUnitOfWork unitOfWork, ILogger<SalaryTemplateAPIController> logger, IMapper mapper, SimpliDbContext SimpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = SimpliDbContext;
    }
    #region SalaryTemplate
    [HttpPost]
    public IActionResult DeleteSalaryTemplateComponentsMappingBySalaryTemplateId(SalaryTemplateDTOForSave data)
    {
        try
        {
            List<SalaryTemplateComponentsMapping> salaryTemplateComponentsMappings = _unitOfWork.SalaryTemplateComponentsMapping.FindAllByExpression(x => x.SalaryTemplateId == data.SalaryTemplateId).ToList();

            foreach (var salaryTemplateComponentsMapping in salaryTemplateComponentsMappings)
            {
                salaryTemplateComponentsMapping.IsActive = false;
            }
            _unitOfWork.SalaryTemplateComponentsMapping.UpdateRange(salaryTemplateComponentsMappings);
            _unitOfWork.Save();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee {nameof(DeleteSalaryTemplateComponentsMappingBySalaryTemplateId)}");
            throw;
        }

    }
    [HttpPost]
    public IActionResult UpdateSalaryTemplate(SalaryTemplateDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<SalaryTemplate, bool>> expression = a => (a.UnitId == inputDTO.UnitId && a.TemplateName == inputDTO.TemplateName && a.IsActive == true) && a.SalaryTemplateId != inputDTO.SalaryTemplateId;
                if (!_unitOfWork.SalaryTemplate.Exists(expression))
                {
                    _unitOfWork.SalaryTemplate.Update(_mapper.Map<SalaryTemplate>(inputDTO));
                    _unitOfWork.Save();
                    return StatusCode(StatusCodes.Status200OK, "Success");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee {nameof(UpdateSalaryTemplate)}");
            throw;
        }

    }
    [HttpPost]
    public IActionResult SaveSalaryTemplateComponentsMapping(SalaryTemplateComponentsMappingDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<SalaryTemplateComponentsMapping, bool>> expression = (a => ((a.SalaryTemplateId == inputDTO.SalaryTemplateId && a.SalaryComponentId == inputDTO.SalaryComponentId && a.IsActive == true)));

                inputDTO.SalaryComponentType = _unitOfWork.SalaryComponentMaster.GetByIdAsync(inputDTO.SalaryComponentId ?? default(int)).Result.SalaryComponentType.FirstOrDefault().ToString();

                if (!_unitOfWork.SalaryTemplateComponentsMapping.Exists(expression))
                {
                    _unitOfWork.SalaryTemplateComponentsMapping.AddAsync(_mapper.Map<SalaryTemplateComponentsMapping>(inputDTO));
                    _unitOfWork.Save();
                }
                return Ok();
            }
            else
            {
                throw new Exception("Not authorised");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SaveSalaryTemplateComponentsMapping)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult SaveSalaryTemplate(SalaryTemplateDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.SalaryTemplateId == 0)
                {
                    Expression<Func<SalaryTemplate, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.TemplateName == inputDTO.TemplateName && a.IsActive == true;
                    if (!_unitOfWork.SalaryTemplate.Exists(expression))
                    {
                        _unitOfWork.SalaryTemplate.AddAsync(_mapper.Map<SalaryTemplate>(inputDTO));
                        _unitOfWork.Save();
                        int Id = _unitOfWork.SalaryTemplate.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.SalaryTemplateId);
                        return Ok(Id.ToString());
                    }
                    else
                        return BadRequest("Duplicate entry found");
                }
                else
                {
                    return BadRequest();
                }

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveSalaryTemplate)}");
            throw;
        }
    }
    [HttpPost(Name = "GetSalaryTemplates")]
    public async Task<IActionResult> GetSalaryTemplates(Core.Helper.RequestParams requestParams, int unitId)
    {
        try
        {
            IList<SalaryTemplateDTO> ViewModel = new List<SalaryTemplateDTO>();
            ViewModel = _mapper.Map<IList<SalaryTemplateDTO>>(await _unitOfWork.SalaryTemplate.GetPagedListWithExpression(requestParams, p => p.IsActive == true && p.UnitId == unitId));

            return Ok(ViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetSalaryTemplates)}");
            throw;
        }
    }


    [HttpPost(Name = "GetSalaryTemplateComponentMapping")]
    public async Task<IActionResult> GetSalaryTemplateComponentMapping(Core.Helper.RequestParams requestParams, int SalaryTemplateId)
    {
        try
        {
            IList<SalaryTemplateComponentsMappingDTO> outputModel = new List<SalaryTemplateComponentsMappingDTO>();
            outputModel = _mapper.Map<IList<SalaryTemplateComponentsMappingDTO>>(await _unitOfWork.SalaryTemplateComponentsMapping.GetPagedListWithExpression(requestParams, p => p.IsActive == true && p.SalaryTemplateId == SalaryTemplateId));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Salary Components {nameof(GetSalaryTemplateComponentMapping)}");
            throw;
        }
    }
    [HttpPost(Name = "GetSalaryTemplateById")]
    public async Task<IActionResult> GetSalaryTemplateById(int SalaryTemplateId)
    {
        try
        {

            SalaryTemplateDTO outputModel = new SalaryTemplateDTO();
            outputModel = _mapper.Map<SalaryTemplateDTO>(await _unitOfWork.SalaryTemplate.GetByIdAsync(SalaryTemplateId));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Salary Components {nameof(GetSalaryTemplateById)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteSalaryTemplate(SalaryTemplateDTO salaryTemplateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                SalaryTemplate salaryTemplate = _mapper.Map<SalaryTemplate>(await _unitOfWork.SalaryTemplate.GetByIdAsync(salaryTemplateDTO.SalaryTemplateId));
                salaryTemplate.IsActive = false;
                _unitOfWork.SalaryTemplate.Update(salaryTemplate);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent, "Validation Failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteSalaryTemplate)}");
            throw;
        }
    }

    #endregion
}
