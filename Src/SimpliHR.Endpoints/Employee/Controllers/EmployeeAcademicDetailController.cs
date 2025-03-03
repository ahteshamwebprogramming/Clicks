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
using SimpliHR.Endpoints;

[Route("api/[controller]/[action]")]
[ApiController]
public class EmployeeAcademicDetailController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeAcademicDetailController> _logger;
    private readonly IMapper _mapper;

    public EmployeeAcademicDetailController(IUnitOfWork unitOfWork, ILogger<EmployeeAcademicDetailController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeAcademicDetail(EmployeeAcademicDTO inputDTO)
    {
        try
        {
            EmployeeAcademicDTO outputDTO = _mapper.Map<EmployeeAcademicDTO>(await _unitOfWork.EmployeeAcademicDetail.GetByIdAsync(inputDTO.AcademicDetailId));
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
            _logger.LogError(ex, $"Error in retriving Employee Academic Details {nameof(GetEmployeeAcademicDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeAcademicList(SimpliHR.Core.Helper.RequestParams requestParams,int? employeeID)
    {
        try
        {
            IList<EmployeeAcademicDTO> outputModel = new List<EmployeeAcademicDTO>();
            outputModel = _mapper.Map<IList<EmployeeAcademicDTO>>(await _unitOfWork.EmployeeAcademicDetail.GetAll(requestParams, p => p.IsActive == true && p.EmployeeId == employeeID,null)).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees Academic Details {nameof(GetEmployeeAcademicList)}");
            throw;
        }
    }

    public IActionResult SaveEditEmployeeAcademicDetail(EmployeeAcademicDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeAcademicDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.AcademicId == inputDTO.AcademicId && a.PassingYear == inputDTO.PassingYear && a.IsActive == true;
                if (!_unitOfWork.EmployeeAcademicDetail.Exists(expression))
                {
                    int detailId = _unitOfWork.EmployeeAcademicDetail.Insert(_mapper.Map<EmployeeAcademicDetail>(inputDTO)).AcademicDetailId;
                    
                    return Ok(detailId);
                }
                else
                    return Ok("Duplicate Entry Found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Academic Detail {nameof(SaveEditEmployeeAcademicDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeAcademicDetail(EmployeeAcademicDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeAcademicDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.AcademicId == inputDTO.AcademicId && a.PassingYear == inputDTO.PassingYear && a.IsActive==true;
                if (!_unitOfWork.EmployeeAcademicDetail.Exists(expression))
                {
                   int detailId =  _unitOfWork.EmployeeAcademicDetail.Insert(_mapper.Map<EmployeeAcademicDetail>(inputDTO)).AcademicDetailId;
                   // _unitOfWork.Save();
                    //detailId = _unitOfWork.EmployeeAcademicDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.AcademicDetailId);
                    return Ok(detailId);
                }
                else
                    return BadRequest("Duplicate Entry Found");
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Academic Detail {nameof(SaveEmployeeAcademicDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateEmployeeAcademicDetail(EmployeeAcademicDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeAcademicDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.AcademicId == inputDTO.AcademicId && a.PassingYear == inputDTO.PassingYear && a.AcademicDetailId != inputDTO.AcademicDetailId && a.IsActive == true;
                if (!_unitOfWork.EmployeeAcademicDetail.Exists(expression))
                {
                    _unitOfWork.EmployeeAcademicDetail.Update(_mapper.Map<EmployeeAcademicDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return Ok("Employee Academic Detail already exists");

            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee Academic Detail updates {nameof(UpdateEmployeeAcademicDetail)}");
            throw;
        }
    }

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
            _unitOfWork.EmployeeAcademicDetail.ExecuteRawQuery("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser,@RefrenceId", new[] { screenTab, id, sessionId, loggedInUser, refId });
            attachmentMsg = "Success";
        }
        catch (Exception ex) { }


        return attachmentMsg;
    }


    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeAcademicDetail(EmployeeAcademicDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeAcademicDetail outputMaster = _mapper.Map<EmployeeAcademicDetail>(await _unitOfWork.EmployeeAcademicDetail.GetByIdAsync(inputDTO.AcademicDetailId));
                outputMaster.IsActive = false;
                _unitOfWork.EmployeeAcademicDetail.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee Academic Detail {nameof(DeleteEmployeeAcademicDetail)}");
            throw;
        }
    }

}
