using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Repository;
using System.Linq.Expressions;
using System.Net;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Helper;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class ModuleMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ModuleMasterController> _logger;
    private readonly IMapper _mapper;

    public ModuleMasterController(IUnitOfWork unitOfWork, ILogger<ModuleMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetModule(ModuleMasterDTO inputDTO)
    {
        try
        {
            ModuleMasterDTO outputDTO = _mapper.Map<ModuleMasterDTO>(await _unitOfWork.ModuleMaster.GetByIdAsync(inputDTO.ModuleId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Module {nameof(GetModule)}");
            throw;
        }
    }

    [HttpPost(Name = "GetModules")]
    public async Task<IActionResult> GetModules(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<ModuleMasterDTO> outputModel = new List<ModuleMasterDTO>();
            outputModel = _mapper.Map<IList<ModuleMasterDTO>>(await _unitOfWork.ModuleMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Modules {nameof(GetModules)}");
            throw;
        }
    }
    [HttpPost(Name = "GetAllModules")]
    public async Task<IActionResult> GetAllModules()
    {
        try
        {
            IList<ModuleMasterDTO> outputModel = new List<ModuleMasterDTO>();
            outputModel = _mapper.Map<IList<ModuleMasterDTO>>(await _unitOfWork.ModuleMaster.GetAll(p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Modules {nameof(GetModules)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveModule(ModuleMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<ModuleMaster, bool>> expression = a => (a.ModuleName.Trim().Replace(" ", "") == inputDTO.ModuleName.Trim().Replace(" ", "") || a.ModuleShortName.Trim().Replace(" ", "") == inputDTO.ModuleShortName.Trim().Replace(" ", "")) && a.IsActive == true;
                if (!_unitOfWork.ModuleMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<ModuleMaster>(inputDTO);
                    _unitOfWork.ModuleMaster.AddAsync(_mapper.Map<ModuleMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Duplicate Entry Found");
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Module {nameof(SaveModule)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateModule(ModuleMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<ModuleMaster, bool>> expression = a => (a.ModuleName.Trim().Replace(" ", "") == inputDTO.ModuleName.Trim().Replace(" ", "") || a.ModuleShortName.Trim().Replace(" ", "") == inputDTO.ModuleShortName.Trim().Replace(" ", "")) && a.ModuleId != inputDTO.ModuleId && a.IsActive == true;
                if (!_unitOfWork.ModuleMaster.Exists(expression))
                {
                    _unitOfWork.ModuleMaster.Update(_mapper.Map<ModuleMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Duplicate Entry Found");

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Module updates {nameof(UpdateModule)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteModule(ModuleMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ModuleMaster outputMaster = _mapper.Map<ModuleMaster>(await _unitOfWork.ModuleMaster.GetByIdAsync(inputDTO.ModuleId));
                outputMaster.IsActive = false;
                _unitOfWork.ModuleMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Module {nameof(DeleteModule)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<ModuleKeyValues> GetModuleKeyValue()
    {
        return (_unitOfWork.ModuleMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new ModuleKeyValues()
                           {
                               ModuleId = p.ModuleId,
                               ModuleName = p.ModuleName
                           })).ToList();
    }




}
