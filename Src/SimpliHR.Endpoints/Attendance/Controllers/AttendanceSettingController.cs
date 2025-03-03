using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Masters;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.Endpoints.Attendance;

[Route("api/[controller]/[action]")]
[ApiController]
public class AttendanceSettingController : ControllerBase
    {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AttendanceSettingController> _logger;
    private readonly IMapper _mapper;

    public AttendanceSettingController(IUnitOfWork unitOfWork, ILogger<AttendanceSettingController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }


    [HttpPost]
    public async Task<IActionResult> GetAttendanceSetting(AttendanceSettingDTO inputDTO)
    {
        try
        {
            AttendanceSettingDTO outputDTO = _mapper.Map<AttendanceSettingDTO>(await _unitOfWork.AttendanceSetting.GetByIdAsync(inputDTO.AttendanceSettingId));
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
            _logger.LogError(ex, $"Error in retriving Attendance Setting {nameof(GetAttendanceSetting)}");
            throw;
        }
    }

    [HttpPost(Name = "GetAttendanceSettingList")]
    public async Task<IActionResult> GetAttendanceSettingList(Core.Helper.RequestParams requestParams, int? UnitId)
    {

        try
        {
            var returnData = _mapper.Map<IList<AttendanceSettingDTO>>(await _unitOfWork.AttendanceSetting.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.UnitId== UnitId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                             orderBy: (m => m.OrderBy(x => x.ShiftId))));
            IList<AttendanceSettingDTO> outputModel = new List<AttendanceSettingDTO>();

            outputModel = returnData.Select(r => new AttendanceSettingDTO
            {
                AttendanceSettingId = r.AttendanceSettingId,
                MaximumTime = r.MaximumTime,
                MinimumTime = r.MinimumTime,
                LegendType = r.LegendType,
                ShiftName = r.Shift.ShiftName,
                ShiftId = r.ShiftId,
                ShiftCode = r.ShiftCode,
                IsActive = r.IsActive,
                LocationId = r.LocationId,
                UnitId= UnitId,
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Work Locations {nameof(GetAttendanceSettingList)}");
            throw;
        }
        
    }

    [HttpPost]
    public async Task<IActionResult> GetAttendanceSettingByID(int inputDTO)
    {
        try
        {
            AttendanceSettingDTO outputDTO = _mapper.Map<AttendanceSettingDTO>(await _unitOfWork.AttendanceSetting.GetByIdAsync(inputDTO));
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
            _logger.LogError(ex, $"Error in retriving Get Attendance Setting by ID {nameof(GetAttendanceSettingByID)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult AttendanceSetting(AttendanceSettingDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.AttendanceSettingId == 0)
                {
                    Expression<Func<AttendanceSetting, bool>> expression = a => a.UnitId== inputDTO.UnitId && a.LocationId == inputDTO.LocationId && a.ShiftCode == inputDTO.ShiftCode && a.LegendType == inputDTO.LegendType && a.IsActive == true;
                    if (!_unitOfWork.AttendanceSetting.Exists(expression))
                    {
                        _unitOfWork.AttendanceSetting.AddAsync(_mapper.Map<AttendanceSetting>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate Attendance Setting found");
                }
                else
                {
                   // Expression<Func<AttendanceSetting, bool>> expression = a => a.AttendanceSettingId != inputDTO.AttendanceSettingId && a.IsActive == true;
                    //Expression<Func<AttendanceSetting, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.LegendType == inputDTO.LegendType && a.AttendanceSettingId != inputDTO.AttendanceSettingId && a.IsActive == true;
                   // if (!_unitOfWork.AttendanceSetting.Exists(expression))
                   // {
                        _unitOfWork.AttendanceSetting.Update(_mapper.Map<AttendanceSetting>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                   // }
                   // else
                       // return BadRequest("Duplicate Attendance Setting found");

                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Attendance Setting {nameof(AttendanceSetting)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAttendanceSetting(AttendanceSettingDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                AttendanceSetting outputMaster = _mapper.Map<AttendanceSetting>(await _unitOfWork.AttendanceSetting.GetByIdAsync(inputDTO.AttendanceSettingId));
                outputMaster.IsActive = false;
                _unitOfWork.AttendanceSetting.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Shift {nameof(DeleteAttendanceSetting)}");
            throw;
        }
    }

}

