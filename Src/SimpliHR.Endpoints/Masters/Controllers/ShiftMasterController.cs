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
public class ShiftMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ShiftMasterController> _logger;
    private readonly IMapper _mapper;

    public ShiftMasterController(IUnitOfWork unitOfWork, ILogger<ShiftMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetShift(ShiftMasterDTO inputDTO)
    {
        try
        {
            ShiftMasterDTO outputDTO = _mapper.Map<ShiftMasterDTO>(await _unitOfWork.ShiftMaster.GetByIdAsync(inputDTO.ShiftId));
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
            _logger.LogError(ex, $"Error in retriving Shift {nameof(GetShift)}");
            throw;
        }
    }

    [HttpPost(Name = "GetShifts")]
    public async Task<IActionResult> GetShifts(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<ShiftMasterDTO> outputModel = new List<ShiftMasterDTO>();
            outputModel = _mapper.Map<IList<ShiftMasterDTO>>(await _unitOfWork.ShiftMaster.GetPagedListWithExpression(requestParams, p => p.UnitId== UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Shifts {nameof(GetShifts)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveShift(ShiftMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<ShiftMaster, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.ShiftCode.Trim().Replace(" ", "") == inputDTO.ShiftCode.Trim().Replace(" ", "") && a.ShiftName.Trim().Replace(" ", "") == inputDTO.ShiftName.Trim().Replace(" ", "") && a.IsActive == true;
                if (!_unitOfWork.ShiftMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<ShiftMaster>(inputDTO);
                    _unitOfWork.ShiftMaster.AddAsync(_mapper.Map<ShiftMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("ADDSUCCESS");
                }
                else
                    return BadRequest("Duplicate Shift found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Shift {nameof(SaveShift)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateShift(ShiftMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<ShiftMaster, bool>> expression = a =>a.UnitId==inputDTO.UnitId && a.ShiftCode.Trim().Replace(" ", "") == inputDTO.ShiftCode.Trim().Replace(" ", "") && a.ShiftName.Trim().Replace(" ", "") == inputDTO.ShiftName.Trim().Replace(" ", "") && a.ShiftId != inputDTO.ShiftId && a.IsActive == true;
                if (!_unitOfWork.ShiftMaster.Exists(expression))
                {
                    _unitOfWork.ShiftMaster.Update(_mapper.Map<ShiftMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("EDITSUCCESS");
                }
                else
                    return BadRequest("Duplicate Shift found");

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Shift updates {nameof(UpdateShift)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteShift(ShiftMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ShiftMaster outputMaster = _mapper.Map<ShiftMaster>(await _unitOfWork.ShiftMaster.GetByIdAsync(inputDTO.ShiftId));
                outputMaster.IsActive = false;
                _unitOfWork.ShiftMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Shift {nameof(DeleteShift)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<ShiftKeyValues> GetShiftKeyValue()
    {
        return (_unitOfWork.ShiftMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new ShiftKeyValues()
                           {
                               ShiftId = p.ShiftId,
                               ShiftName = p.ShiftName
                           })).ToList();
    }

   


}
