using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System.Linq.Expressions;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using SimpliHR.Services.DBContext;
using System.Text.RegularExpressions;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Core.Helper;
using Microsoft.AspNetCore.Components.Forms;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class ResourceMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResourceMasterController> _logger;
    private readonly IMapper _mapper;

    public ResourceMasterController(IUnitOfWork unitOfWork, ILogger<ResourceMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetResource(ResourceMasterDTO inputDTO)
    {
        try
        {
            ResourceMasterDTO outputDTO = _mapper.Map<ResourceMasterDTO>(await _unitOfWork.ResourceMaster.GetByIdAsync(inputDTO.ResourceId));
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
            _logger.LogError(ex, $"Error in retriving Resource {nameof(GetResource)}");
            throw;
        }
    }

    [HttpPost(Name = "GetResources")]
    public async Task<IActionResult> GetResources(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<ResourceMasterDTO> outputModel = new List<ResourceMasterDTO>();
            outputModel = _mapper.Map<IList<ResourceMasterDTO>>(await _unitOfWork.ResourceMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId && p.IsActive == true)); ;
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Resources {nameof(GetResources)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveResource(ResourceMasterDTO inputDTO)
    {
        try
        {

            inputDTO.IsActive = true;
            if (ModelState.IsValid)
            {
                Expression<Func<ResourceMaster, bool>> expression = a => a.ResourceName.Trim().Replace(" ", "") == inputDTO.ResourceName.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true; ;
                if (!_unitOfWork.ResourceMaster.Exists(expression))
                {
                    _unitOfWork.ResourceMaster.AddAsync(_mapper.Map<ResourceMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Resource {nameof(SaveResource)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateResource(ResourceMasterDTO inputDTO)
    {
        try
        {

            if (ModelState.IsValid)
            {
                Expression<Func<ResourceMaster, bool>> expression = a => a.ResourceName.Trim().Replace(" ", "") == inputDTO.ResourceName.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.ResourceId != inputDTO.ResourceId && a.IsActive == true;
                if (!_unitOfWork.ResourceMaster.Exists(expression))
                {
                    _unitOfWork.ResourceMaster.Update(_mapper.Map<ResourceMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in Resource updates {nameof(UpdateResource)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteResource(ResourceMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ResourceMaster outputMaster = _mapper.Map<ResourceMaster>(await _unitOfWork.ResourceMaster.GetByIdAsync(inputDTO.ResourceId));
                outputMaster.IsActive = false;
                _unitOfWork.ResourceMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Resource {nameof(DeleteResource)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<ResourceKeyValues> GetResourceKeyValue()
    {
        return (_unitOfWork.ResourceMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new ResourceKeyValues()
                           {
                               ResourceId = p.ResourceId,
                               ResourceName = p.ResourceName
                           })).ToList();
    }




}
