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
public class EmployeeReferenceDetailController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeReferenceDetailController> _logger;
    private readonly IMapper _mapper;

    public EmployeeReferenceDetailController(IUnitOfWork unitOfWork, ILogger<EmployeeReferenceDetailController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeReferenceDetail(EmployeeReferenceDetailDTO inputDTO)
    {
        try
        {
            EmployeeReferenceDetailDTO outputDTO = _mapper.Map<EmployeeReferenceDetailDTO>(await _unitOfWork.EmployeeReferenceDetail.GetByIdAsync(inputDTO.EmployeeReferenceId));
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
            _logger.LogError(ex, $"Error in retriving Employee Reference Details {nameof(GetEmployeeReferenceDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeReferenceList(Core.Helper.RequestParams requestParams,int? employeeID)
    {
        try
        {
            IList<EmployeeReferenceDetailDTO> outputModel = new List<EmployeeReferenceDetailDTO>();
            outputModel = _mapper.Map<IList<EmployeeReferenceDetailDTO>>(await _unitOfWork.EmployeeReferenceDetail.GetAll(requestParams, p => p.IsActive == true && p.ReferenceOf == employeeID, null)).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees Reference Details {nameof(GetEmployeeReferenceList)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEditEmployeeReferenceDetail(EmployeeReferenceDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeReferenceDetail, bool>> expression = (a => (a.ReferenceOf == inputDTO.ReferenceOf && a.PersonName == inputDTO.PersonName && a.PresentCompany == inputDTO.PresentCompany && a.ReferenceDesignation == inputDTO.ReferenceDesignation && a.IsActive == true));
                if (!_unitOfWork.EmployeeReferenceDetail.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<EmployeeReferenceDetail>(inputDTO);
                    int detailId = _unitOfWork.EmployeeReferenceDetail.Insert(_mapper.Map<EmployeeReferenceDetail>(inputDTO)).EmployeeReferenceId;
                    //_unitOfWork.Save();
                    //int detailId = _unitOfWork.EmployeeReferenceDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeReferenceId);
                    return Ok(detailId);
                }
                else
                    return Ok("Employee Reference Detail already exists");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Reference Detail {nameof(SaveEmployeeReferenceDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeReferenceDetail(EmployeeReferenceDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeReferenceDetail, bool>> expression = (a => (a.ReferenceOf == inputDTO.ReferenceOf && a.PersonName == inputDTO.PersonName && a.PresentCompany == inputDTO.PresentCompany && a.ReferenceDesignation == inputDTO.ReferenceDesignation && a.IsActive == true));
                if (!_unitOfWork.EmployeeReferenceDetail.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<EmployeeReferenceDetail>(inputDTO);
                    int detailId = _unitOfWork.EmployeeReferenceDetail.Insert(_mapper.Map<EmployeeReferenceDetail>(inputDTO)).EmployeeReferenceId;
                    //_unitOfWork.Save();
                    //int detailId = _unitOfWork.EmployeeReferenceDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeReferenceId);
                    return Ok(detailId);
                }
                else
                    return Ok("Employee Reference Detail already exists");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Reference Detail {nameof(SaveEmployeeReferenceDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateEmployeeReferenceDetail(EmployeeReferenceDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeReferenceDetail, bool>> expression = a => a.ReferenceOf == inputDTO.ReferenceOf && a.PersonName == inputDTO.PersonName && a.PresentCompany == inputDTO.PresentCompany && a.ReferenceDesignation == inputDTO.ReferenceDesignation && a.EmployeeReferenceId != inputDTO.EmployeeReferenceId && a.IsActive == true;
                if (!_unitOfWork.EmployeeReferenceDetail.Exists(expression))
                {
                    _unitOfWork.EmployeeReferenceDetail.Update(_mapper.Map<EmployeeReferenceDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return Ok("Employee Reference Detail already exists");

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee Reference Detail updates {nameof(UpdateEmployeeReferenceDetail)}");
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
            _unitOfWork.EmployeeReferenceDetail.ExecuteRawQuery("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser,@RefrenceId", new[] { screenTab, id, sessionId, loggedInUser, refId });
            attachmentMsg = "Success";
        }
        catch (Exception ex) { }

        return attachmentMsg;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeReferenceDetail(EmployeeReferenceDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeReferenceDetail outputMaster = _mapper.Map<EmployeeReferenceDetail>(await _unitOfWork.EmployeeReferenceDetail.GetByIdAsync(inputDTO.EmployeeReferenceId));
                outputMaster.IsActive = false;
                _unitOfWork.EmployeeReferenceDetail.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee Certification Detail {nameof(DeleteEmployeeReferenceDetail)}");
            throw;
        }
    }


}
