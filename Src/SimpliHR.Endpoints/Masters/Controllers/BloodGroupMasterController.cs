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

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class BloodGroupMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BloodGroupMasterController> _logger;
    private readonly IMapper _mapper;

    public BloodGroupMasterController(IUnitOfWork unitOfWork, ILogger<BloodGroupMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetBloodGroup(BloodGroupMasterDTO inputDTO)
    {
        try
        {
            BloodGroupMasterDTO outputDTO = _mapper.Map<BloodGroupMasterDTO>(await _unitOfWork.BloodGroupMaster.GetByIdAsync(inputDTO.BloodGroupId));
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
            _logger.LogError(ex, $"Error in retriving BloodGroup {nameof(GetBloodGroup)}");
            throw;
        }
    }

    [HttpPost(Name = "GetBloodGroup")]
    public async Task<IActionResult> GetBloodGroups(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<BloodGroupMasterDTO> outputModel = new List<BloodGroupMasterDTO>();
            outputModel = _mapper.Map<IList<BloodGroupMasterDTO>>(await _unitOfWork.BloodGroupMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving BloodGroup {nameof(GetBloodGroup)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveBloodGroup(BloodGroupMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<BloodGroupMaster, bool>> expression = a => (a.BloodGroupName.Trim().Replace(" ", "") == inputDTO.BloodGroupName.Trim().Replace(" ", "") || a.BloodGroupCode.Trim().Replace(" ", "") == inputDTO.BloodGroupCode.Trim().Replace(" ", "")) && a.IsActive == true;
                if (!_unitOfWork.BloodGroupMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<BloodGroupMaster>(inputDTO);
                    _unitOfWork.BloodGroupMaster.AddAsync(_mapper.Map<BloodGroupMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Duplicate Entry Found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Blood Group {nameof(SaveBloodGroup)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateBloodGroup(BloodGroupMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<BloodGroupMaster, bool>> expression = a => (a.BloodGroupName.Trim().Replace(" ", "") == inputDTO.BloodGroupName.Trim().Replace(" ", "") || a.BloodGroupCode.Trim().Replace(" ", "") == inputDTO.BloodGroupCode.Trim().Replace(" ", "")) && a.BloodGroupId != inputDTO.BloodGroupId && a.IsActive == true;
                if (!_unitOfWork.BloodGroupMaster.Exists(expression))
                {
                    _unitOfWork.BloodGroupMaster.Update(_mapper.Map<BloodGroupMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in Blood Group updates {nameof(UpdateBloodGroup)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBloodGroup(BloodGroupMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                BloodGroupMaster outputMaster = _mapper.Map<BloodGroupMaster>(await _unitOfWork.BloodGroupMaster.GetByIdAsync(inputDTO.BloodGroupId));
                outputMaster.IsActive = false;
                _unitOfWork.BloodGroupMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return BadRequest(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Blood Group {nameof(DeleteBloodGroup)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<BloodGroupKeyValues> GetBloodGroupKeyValue()
    {
        return (_unitOfWork.BloodGroupMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new BloodGroupKeyValues()
                           {
                               BloodGroupId = p.BloodGroupId,
                               BloodGroupName = p.BloodGroupName
                           })).ToList();
    }




}
