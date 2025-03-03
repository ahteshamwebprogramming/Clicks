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
public class EmployeeCertificationDetailController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeCertificationDetailController> _logger;
    private readonly IMapper _mapper;

    public EmployeeCertificationDetailController(IUnitOfWork unitOfWork, ILogger<EmployeeCertificationDetailController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeCertificationDetail(EmployeeCertificationDetailDTO inputDTO)
    {
        try
        {
            EmployeeCertificationDetailDTO outputDTO = _mapper.Map<EmployeeCertificationDetailDTO>(await _unitOfWork.EmployeeCertificationDetail.GetByIdAsync(inputDTO.CertificationDetailId));
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
            _logger.LogError(ex, $"Error in retriving Employee Certification Details {nameof(GetEmployeeCertificationDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeCertificationList(Core.Helper.RequestParams requestParams,int? employeeID)
    {
        try
        {
            IList<EmployeeCertificationDetailDTO> outputModel = new List<EmployeeCertificationDetailDTO>();
            outputModel = _mapper.Map<IList<EmployeeCertificationDetailDTO>>(await _unitOfWork.EmployeeCertificationDetail.GetAll(requestParams, p => p.IsActive == true && p.EmployeeId == employeeID, null)).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees Certification Details {nameof(GetEmployeeCertificationList)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult SaveEditEmployeeCertificationDetail(EmployeeCertificationDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeCertificationDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.CertificationName == inputDTO.CertificationName && a.YearOfCertification == inputDTO.YearOfCertification;
                if (!_unitOfWork.EmployeeCertificationDetail.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<EmployeeCertificationDetail>(inputDTO);
                    int detailId = _unitOfWork.EmployeeCertificationDetail.Insert(_mapper.Map<EmployeeCertificationDetail>(inputDTO)).CertificationDetailId;
                    //_unitOfWork.Save();
                    //int detailId = _unitOfWork.EmployeeCertificationDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.CertificationDetailId);
                    return Ok(detailId);
                }
                else
                    return Ok("Duplicate Entry Found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Certification Detail {nameof(SaveEmployeeCertificationDetail)}");
            throw;
        }
    }



    [HttpPost]
    public IActionResult SaveEmployeeCertificationDetail(EmployeeCertificationDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeCertificationDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.CertificationName == inputDTO.CertificationName && a.YearOfCertification == inputDTO.YearOfCertification;
                if (!_unitOfWork.EmployeeCertificationDetail.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<EmployeeCertificationDetail>(inputDTO);
                    int detailId = _unitOfWork.EmployeeCertificationDetail.Insert(_mapper.Map<EmployeeCertificationDetail>(inputDTO)).CertificationDetailId;
                    //_unitOfWork.Save();
                    //int detailId = _unitOfWork.EmployeeCertificationDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.CertificationDetailId);
                    return Ok(detailId);
                }
                else
                    return Ok("Duplicate Entry Found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Certification Detail {nameof(SaveEmployeeCertificationDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateEmployeeCertificationDetail(EmployeeCertificationDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeCertificationDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.CertificationName == inputDTO.CertificationName && a.YearOfCertification == inputDTO.YearOfCertification && a.CertificationDetailId != inputDTO.CertificationDetailId;
                if (!_unitOfWork.EmployeeCertificationDetail.Exists(expression))
                {
                    _unitOfWork.EmployeeCertificationDetail.Update(_mapper.Map<EmployeeCertificationDetail>(inputDTO));
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
            _logger.LogError(ex, $"Error in Employee Certification Detail updates {nameof(UpdateEmployeeCertificationDetail)}");
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
            _unitOfWork.EmployeeCertificationDetail.ExecuteRawQuery("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser,@RefrenceId", new[] { screenTab, id, sessionId, loggedInUser, refId });
            attachmentMsg = "Success";
        }
        catch (Exception ex) { }


        return attachmentMsg;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeCertificationDetail(EmployeeCertificationDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeCertificationDetail outputMaster = _mapper.Map<EmployeeCertificationDetail>(await _unitOfWork.EmployeeCertificationDetail.GetByIdAsync(inputDTO.CertificationDetailId));
                outputMaster.IsActive = false;
                _unitOfWork.EmployeeCertificationDetail.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee Certification Detail {nameof(DeleteEmployeeCertificationDetail)}");
            throw;
        }
    }

}
