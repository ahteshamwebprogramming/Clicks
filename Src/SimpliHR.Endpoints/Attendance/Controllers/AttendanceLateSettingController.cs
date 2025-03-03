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
public class AttendanceLateSettingController : ControllerBase
    {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AttendanceLateSettingController> _logger;
    private readonly IMapper _mapper;

    public AttendanceLateSettingController(IUnitOfWork unitOfWork, ILogger<AttendanceLateSettingController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }


    [HttpPost]
    public async Task<IActionResult> GetAttendanceLateSetting(AttendanceLateSettingDTO inputDTO)
    {
        try
        {
            AttendanceLateSettingDTO outputDTO = _mapper.Map<AttendanceLateSettingDTO>(await _unitOfWork.AttendanceLateSetting.GetByIdAsync(inputDTO.LateMasterId));
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
            _logger.LogError(ex, $"Error in retriving Attendance late Setting {nameof(GetAttendanceLateSetting)}");
            throw;
        }
    }

    [HttpPost(Name = "GetAttendanceLateSettingList")]
    public async Task<IActionResult> GetAttendanceLateSettingList(Core.Helper.RequestParams requestParams, int? UnitId)
    {

        try
        {
            var returnData = _mapper.Map<IList<AttendanceLateSettingDTO>>(await _unitOfWork.AttendanceLateSetting.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.UnitId== UnitId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                             orderBy: (m => m.OrderBy(x => x.LateMasterId))));
            IList<AttendanceLateSettingDTO> outputModel = new List<AttendanceLateSettingDTO>();

            outputModel = returnData.Select(r => new AttendanceLateSettingDTO
            {
                LateMasterId = r.LateMasterId,
                LateDuration = r.LateDuration,
                NoOfLate = r.NoOfLate,
                AppliedOn = r.AppliedOn,
                ShowPostLimit = r.ShowPostLimit               
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Work Locations {nameof(GetAttendanceLateSettingList)}");
            throw;
        }
        //try
        //{
        //    IList<AttendanceSettingDTO> outputModel = new List<AttendanceSettingDTO>();
        //    outputModel = _mapper.Map<IList<AttendanceSettingDTO>>(await _unitOfWork.AttendanceSetting.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
        //    return Ok(outputModel);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, $"Error in retriving Attendance Setting {nameof(GetAttendanceSettingList)}");
        //    throw;
        //}
    }

    //[HttpPost]
    //public async Task<IActionResult> GetAttendanceSettingByID(int inputDTO)
    //{
    //    try
    //    {
    //        AttendanceSettingDTO outputDTO = _mapper.Map<AttendanceSettingDTO>(await _unitOfWork.AttendanceSetting.GetByIdAsync(inputDTO));
    //        HttpResponseMessage httpMessage = new HttpResponseMessage();
    //        if (outputDTO == null)
    //        {
    //            httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
    //            outputDTO = CommonHelper.GetClassObject(outputDTO);
    //        }
    //        else
    //            httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

    //        outputDTO.HttpMessage = httpMessage;
    //        return Ok(outputDTO);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Get Attendance Setting by ID {nameof(GetAttendanceSettingByID)}");
    //        throw;
    //    }
    //}

    [HttpPost]
    public IActionResult SaveAttendanceLateSetting(AttendanceLateSettingDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.LateMasterId == 0)
                {
                    Expression<Func<AttendanceLateSetting, bool>> expression = a => a.UnitId== inputDTO.UnitId && a.AppliedOn == inputDTO.AppliedOn && a.IsActive == true;
                    if (!_unitOfWork.AttendanceLateSetting.Exists(expression))
                    {
                        _unitOfWork.AttendanceLateSetting.AddAsync(_mapper.Map<AttendanceLateSetting>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("ADDSUCCESS");
                    }
                    else
                    {
                        return BadRequest("Duplicate Attendance late Setting found for unit");
                    }
                }
                else
                {
                    Expression<Func<AttendanceLateSetting, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.AppliedOn == inputDTO.AppliedOn && a.IsActive == true && a.LateMasterId!= inputDTO.LateMasterId;
                    if (!_unitOfWork.AttendanceLateSetting.Exists(expression))
                    {
                        _unitOfWork.AttendanceLateSetting.Update(_mapper.Map<AttendanceLateSetting>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("EDITSUCCESS");
                    }
                    else
                    {
                        return BadRequest("Duplicate Attendance late Setting found for unit");
                    }
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Attendance Setting {nameof(SaveAttendanceLateSetting)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAttendanceLateSetting(AttendanceLateSettingDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                AttendanceLateSetting outputMaster = _mapper.Map<AttendanceLateSetting>(await _unitOfWork.AttendanceLateSetting.GetByIdAsync(inputDTO.LateMasterId));
                outputMaster.IsActive = false;
                _unitOfWork.AttendanceLateSetting.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "DELETESUCCESS"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Shift {nameof(DeleteAttendanceLateSetting)}");
            throw;
        }
    }

}

