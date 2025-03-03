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
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Core.Helper;
using SimpliHR.Infrastructure.Models.Employee;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class EmployeeExperienceDetailController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeExperienceDetailController> _logger;
    private readonly IMapper _mapper;

    public EmployeeExperienceDetailController(IUnitOfWork unitOfWork, ILogger<EmployeeExperienceDetailController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeExperienceDetail(EmployeeExperienceDetailDTO inputDTO)
    {
        try
        {
            EmployeeExperienceDetailDTO outputDTO = _mapper.Map<EmployeeExperienceDetailDTO>(await _unitOfWork.EmployeeExperienceDetail.GetByIdAsync(inputDTO.ExperienceDetailId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            //  outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee Experience Details {nameof(GetEmployeeExperienceDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeExperienceList(Core.Helper.RequestParams requestParams, int? employeeID)
    {
        try
        {
            IList<EmployeeExperienceDetailDTO> outputModel = new List<EmployeeExperienceDetailDTO>();
            outputModel = _mapper.Map<IList<EmployeeExperienceDetailDTO>>(await _unitOfWork.EmployeeExperienceDetail.GetAll(requestParams, p => p.IsActive == true && p.EmployeeId == employeeID, null)).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees Experience Details {nameof(GetEmployeeExperienceList)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEditEmployeeExperienceDetail(EmployeeExperienceDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.JoinDate < inputDTO.LastWorkingDate)
                {
                    Expression<Func<EmployeeExperienceDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true && (a.CompanyName == inputDTO.CompanyName || ((inputDTO.JoinDate > a.JoinDate && inputDTO.JoinDate < a.LastWorkingDate) || (inputDTO.LastWorkingDate > a.JoinDate && inputDTO.LastWorkingDate < a.LastWorkingDate)));
                    if (!_unitOfWork.EmployeeExperienceDetail.Exists(expression))
                    {
                        //var outputDTO = _mapper.Map<EmployeeExperienceDetail>(inputDTO);
                        int detailId = _unitOfWork.EmployeeExperienceDetail.Insert(_mapper.Map<EmployeeExperienceDetail>(inputDTO)).ExperienceDetailId;
                        //int detailId = _unitOfWork.EmployeeExperienceDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.ExperienceDetailId);
                        return Ok(detailId);
                    }
                    else
                        return Ok("Employee Experience Detail already exists");
                }
                else
                {
                    return Ok("Joining date cannot be greator then Last Working Date");
                }


            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Experience Detail {nameof(SaveEditEmployeeExperienceDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeExperienceDetail(EmployeeExperienceDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.JoinDate < inputDTO.LastWorkingDate)
                {
                    Expression<Func<EmployeeExperienceDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true && (a.CompanyName == inputDTO.CompanyName || ((inputDTO.JoinDate > a.JoinDate && inputDTO.JoinDate < a.LastWorkingDate) || (inputDTO.LastWorkingDate > a.JoinDate && inputDTO.LastWorkingDate < a.LastWorkingDate) || (inputDTO.JoinDate < a.LastWorkingDate && inputDTO.LastWorkingDate > a.JoinDate)));
                    if (!_unitOfWork.EmployeeExperienceDetail.Exists(expression))
                    {
                        //var outputDTO = _mapper.Map<EmployeeExperienceDetail>(inputDTO);
                        int detailId = _unitOfWork.EmployeeExperienceDetail.Insert(_mapper.Map<EmployeeExperienceDetail>(inputDTO)).ExperienceDetailId;
                        //_unitOfWork.Save();
                        //= _unitOfWork.EmployeeExperienceDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.ExperienceDetailId);
                        return Ok(detailId);
                    }
                    else
                        return Ok("Employee Experience Detail already exists");
                }
                else
                {
                    return Ok("Joining date cannot be greator then Last Working Date");
                }


            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Experience Detail {nameof(SaveEmployeeExperienceDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateEmployeeExperienceDetail(EmployeeExperienceDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.JoinDate < inputDTO.LastWorkingDate)
                {
                    //Expression<Func<EmployeeExperienceDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.CompanyName == inputDTO.CompanyName && a.JoinDate == inputDTO.JoinDate && a.ExperienceDetailId != inputDTO.ExperienceDetailId && a.IsActive == true && (inputDTO.JoinDate > a.JoinDate && inputDTO.JoinDate < a.LastWorkingDate) && (inputDTO.LastWorkingDate > a.JoinDate && inputDTO.LastWorkingDate < a.LastWorkingDate);

                    Expression<Func<EmployeeExperienceDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.ExperienceDetailId != inputDTO.ExperienceDetailId && a.IsActive == true && (a.CompanyName == inputDTO.CompanyName || ((inputDTO.JoinDate > a.JoinDate && inputDTO.JoinDate < a.LastWorkingDate) || (inputDTO.LastWorkingDate > a.JoinDate && inputDTO.LastWorkingDate < a.LastWorkingDate) || (inputDTO.JoinDate < a.LastWorkingDate && inputDTO.LastWorkingDate > a.JoinDate)));
                    if (!_unitOfWork.EmployeeExperienceDetail.Exists(expression))
                    {
                        _unitOfWork.EmployeeExperienceDetail.Update(_mapper.Map<EmployeeExperienceDetail>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return Ok("Employee Experience Detail already exists");
                }
                else
                {
                    return Ok("Joining date cannot be greator then Last Working Date");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee Experience Detail updates {nameof(UpdateEmployeeExperienceDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<string> SaveAttachment(EmployeeTempDocUploadDTO empTempDoc)
    {
        string attachmentMsg = "Fail to save attachment";
        var screenTab = new Microsoft.Data.SqlClient.SqlParameter("@ScreenTab", empTempDoc.ScreenTab);
        var id = new Microsoft.Data.SqlClient.SqlParameter("@Id", empTempDoc.EmployeeId);
        var sessionId = new Microsoft.Data.SqlClient.SqlParameter("@SessionId", empTempDoc.SessionId);
        var loggedInUser = new Microsoft.Data.SqlClient.SqlParameter("@loggedInUser", empTempDoc.LoggedInUser);
        var refId = new Microsoft.Data.SqlClient.SqlParameter("@RefrenceId", empTempDoc.ReferenceId);
        //var objData = new { @ScreenTab = empTempDoc.ScreenTab, @Id = empTempDoc.EmployeeId, @SessionId = empTempDoc.SessionId, @LoggedInUser = empTempDoc.LoggedInUser, @RefrenceId=refId };
        try
        {
            _unitOfWork.EmployeeExperienceDetail.ExecuteRawQuery("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser,@RefrenceId", new[] { screenTab, id, sessionId, loggedInUser, refId });
            attachmentMsg = "Success";
        }
        catch (Exception ex) { }


        return attachmentMsg;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeExperienceDetail(EmployeeExperienceDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeExperienceDetail outputMaster = _mapper.Map<EmployeeExperienceDetail>(await _unitOfWork.EmployeeExperienceDetail.GetByIdAsync(inputDTO.ExperienceDetailId));
                outputMaster.IsActive = false;
                _unitOfWork.EmployeeExperienceDetail.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee Experience Detail {nameof(DeleteEmployeeExperienceDetail)}");
            throw;
        }
    }
}
