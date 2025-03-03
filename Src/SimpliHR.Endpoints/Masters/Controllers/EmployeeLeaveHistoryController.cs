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
public class EmployeeLeaveHistoryController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeLeaveHistoryController> _logger;
    private readonly IMapper _mapper;

    public EmployeeLeaveHistoryController(IUnitOfWork unitOfWork, ILogger<EmployeeLeaveHistoryController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetAcademic(EmployeeLeaveHistoryDTO inputDTO)
    {
        try
        {
            EmployeeLeaveHistoryDTO outputDTO = _mapper.Map<EmployeeLeaveHistoryDTO>(await _unitOfWork.EmployeeLeaveHistory.GetByIdAsync(inputDTO.AcademicId));
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
            _logger.LogError(ex, $"Error in retriving Academic {nameof(GetAcademic)}");
            throw;
        }
    }

    [HttpPost(Name = "GetAcademics")]
    public async Task<IActionResult> GetAcademics(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<EmployeeLeaveHistoryDTO> outputModel = new List<EmployeeLeaveHistoryDTO>();
            outputModel = _mapper.Map<IList<EmployeeLeaveHistoryDTO>>(await _unitOfWork.EmployeeLeaveHistory.GetPagedList(requestParams));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetAcademics)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveAcademic(EmployeeLeaveHistoryDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeLeaveHistory, bool>> expression = a => a.AcademicName.Trim().Replace(" ","") == inputDTO.AcademicName;
                if (!_unitOfWork.EmployeeLeaveHistory.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<EmployeeLeaveHistory>(inputDTO);
                    _unitOfWork.EmployeeLeaveHistory.AddAsync(_mapper.Map<EmployeeLeaveHistory>(inputDTO));
                    _unitOfWork.Save();
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                }
                else
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Academic {nameof(SaveAcademic)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateAcademic(EmployeeLeaveHistoryDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeLeaveHistory, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.LeaveId != inputDTO.LeaveId;
                if (!_unitOfWork.EmployeeLeaveHistory.Exists(expression))
                {
                    _unitOfWork.EmployeeLeaveHistory.Update(_mapper.Map<EmployeeLeaveHistory>(inputDTO));
                    _unitOfWork.Save();
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                }
                else
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Academic updates {nameof(UpdateAcademic)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAcademic(EmployeeLeaveHistoryDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeLeaveHistory outputMaster = _mapper.Map<EmployeeLeaveHistory>(await _unitOfWork.EmployeeLeaveHistory.GetByIdAsync(inputDTO.LeaveId));
                //outputMaster.IsActive = false;
                _unitOfWork.EmployeeLeaveHistory.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Academic {nameof(DeleteAcademic)}");
            throw;
        }
    }
}
