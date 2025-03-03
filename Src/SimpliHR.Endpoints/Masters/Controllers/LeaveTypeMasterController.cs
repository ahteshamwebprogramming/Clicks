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
public class LeaveTypeMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LeaveTypeMasterController> _logger;
    private readonly IMapper _mapper;

    public LeaveTypeMasterController(IUnitOfWork unitOfWork, ILogger<LeaveTypeMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetLeaveType(LeaveTypeMasterDTO inputDTO)
    {
        try
        {
            LeaveTypeMasterDTO outputDTO = _mapper.Map<LeaveTypeMasterDTO>(await _unitOfWork.LeaveTypeMaster.GetByIdAsync(inputDTO.LeaveTypeId));
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
            _logger.LogError(ex, $"Error in retriving Leave Type {nameof(GetLeaveType)}");
            throw;
        }
    }

    [HttpPost(Name = "GetLeaveTypes")]
    public async Task<IActionResult> GetLeaveTypes(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<LeaveTypeMasterDTO> outputModel = new List<LeaveTypeMasterDTO>();
            outputModel = _mapper.Map<IList<LeaveTypeMasterDTO>>(await _unitOfWork.LeaveTypeMaster.GetPagedListWithExpression(requestParams, p => p.UnitId== UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Leave Types {nameof(GetLeaveTypes)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveLeaveType(LeaveTypeMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<LeaveTypeMaster, bool>> expression = a => a.LeaveTypeCode == inputDTO.LeaveTypeCode && a.UnitId==inputDTO.UnitId && a.IsActive==true;
                if (!_unitOfWork.LeaveTypeMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<LeaveTypeMaster>(inputDTO);
                    _unitOfWork.LeaveTypeMaster.AddAsync(_mapper.Map<LeaveTypeMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Duplicate Leave type found");
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Leave Type {nameof(SaveLeaveType)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateLeaveType(LeaveTypeMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<LeaveTypeMaster, bool>> expression = a => a.LeaveType.Trim().Replace(" ", "") == inputDTO.LeaveType.Trim().Replace(" ", "") && a.LeaveTypeId != inputDTO.LeaveTypeId && a.IsActive == true;
                //if (!_unitOfWork.LeaveTypeMaster.Exists(expression))
                //{
                    _unitOfWork.LeaveTypeMaster.Update(_mapper.Map<LeaveTypeMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                //}
                //else
                //    return BadRequest("Duplicate Leave type found");

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Leave Type updates {nameof(UpdateLeaveType)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLeaveType(LeaveTypeMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                LeaveTypeMaster outputMaster = _mapper.Map<LeaveTypeMaster>(await _unitOfWork.LeaveTypeMaster.GetByIdAsync(inputDTO.LeaveTypeId));
                outputMaster.IsActive = false;
                _unitOfWork.LeaveTypeMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Leave Type {nameof(DeleteLeaveType)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<LeaveTypeKeyValues> GetLeaveTypeKeyValue()
    {
        return (_unitOfWork.LeaveTypeMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new LeaveTypeKeyValues()
                           {
                               LeaveTypeId = p.LeaveTypeId,
                               LeaveType = p.LeaveType
                           })).ToList();
    }

   


}
