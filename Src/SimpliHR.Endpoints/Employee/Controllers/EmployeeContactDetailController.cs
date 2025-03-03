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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Cors;
namespace SimpliHR.Endpoints.Masters;

[EnableCors()]
[Route("api/[controller]/[action]")]
[ApiController]
public class EmployeeContactDetailController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeContactDetailController> _logger;
    private readonly IMapper _mapper;

    public EmployeeContactDetailController(IUnitOfWork unitOfWork, ILogger<EmployeeContactDetailController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

   
    [HttpPost(Name = "GetEmployeeContacts")]
    public async Task<IActionResult> GetEmployeeContacts(Core.Helper.RequestParams requestParams,int? employeeId)
    {
        try
        {
            IList<EmployeeContactDetailDTO> outputModel = new List<EmployeeContactDetailDTO>();
            outputModel = _mapper.Map<IList<EmployeeContactDetailDTO>>(await _unitOfWork.EmployeeContactDetail.GetPagedList(requestParams)).Where(p => p.IsActive == true && p.EmployeeId == employeeId).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee Cpnntacts {nameof(GetEmployeeContacts)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeContacts(EmployeeContactDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<EmployeeContactDetail, bool>> expression = (a => ((a.Pannumber.Trim() == inputDTO.Pannumber) || (a.EmailId.Trim() == inputDTO.EmailId) || (a.AadharNumber.Trim()==inputDTO.AadharNumber.Trim())));
                //if (!_unitOfWork.EmployeeContactDetail.Exists(expression))
                //{
                //var outputDTO = _mapper.Map<EmployeeContactDetail>(inputDTO);
                inputDTO.IsActive=true;
                int tableID = _unitOfWork.EmployeeContactDetail.Insert(_mapper.Map<EmployeeContactDetail>(inputDTO)).EmployeeContactDetailId;
               // _unitOfWork.Save();
                //int tableID = _unitOfWork.EmployeeContactDetail.GetAll(null,null,null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeContactDetailId);
                //return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, empID.ToString()));
                return Ok(tableID.ToString());
              //  }
                //else
                //    return Ok("Duplicate Email/Aadhar/PAN entry found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SaveEmployeeContacts)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEditEmployeeContacts(EmployeeContactDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<EmployeeContactDetail, bool>> expression = (a => ((a.Pannumber.Trim() == inputDTO.Pannumber) || (a.EmailId.Trim() == inputDTO.EmailId) || (a.AadharNumber.Trim()==inputDTO.AadharNumber.Trim())));
                //if (!_unitOfWork.EmployeeContactDetail.Exists(expression))
                //{
                //var outputDTO = _mapper.Map<EmployeeContactDetail>(inputDTO);
                inputDTO.IsActive = false;
                int tableID = _unitOfWork.EmployeeContactDetail.Insert(_mapper.Map<EmployeeContactDetail>(inputDTO)).EmployeeContactDetailId;
                //_unitOfWork.Save();
                //int tableID = _unitOfWork.EmployeeContactDetail.GetAll(null,null,null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeContactDetailId);
                //return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, empID.ToString()));
                return Ok(tableID.ToString());
                //  }
                //else
                //    return Ok("Duplicate Email/Aadhar/PAN entry found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee {nameof(SaveEmployeeContacts)}");
            throw;
        }
    }

    [HttpPost]
    public string UpdateEmployeeContacts(EmployeeContactDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<EmployeeContactDetail, bool>> expression = a => a.EmployeeContactDetailId == inputDTO.EmployeeContactDetailId && a.EmployeeId != inputDTO.EmployeeId;
                //if (!_unitOfWork.EmployeeContactDetail.Exists(expression))
                //{
                inputDTO.IsActive = true;
                _unitOfWork.EmployeeContactDetail.Update(_mapper.Map<EmployeeContactDetail>(inputDTO));
                _unitOfWork.Save();
                return "Success";
                //}
                //else
                //    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            }
            return "Invalid Model";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee updates {nameof(UpdateEmployeeContacts)}");
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
        var objData = new { @ScreenTab = empTempDoc.ScreenTab, @Id = empTempDoc.EmployeeId, @SessionId = empTempDoc.SessionId, @LoggedInUser = empTempDoc.LoggedInUser };
        _unitOfWork.EmployeeContactDetail.ExecuteRawQuery("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser", new[] { screenTab, id, sessionId, loggedInUser });
        attachmentMsg = "Success";
        return attachmentMsg;
    }

    //[HttpPost]
    //public async Task<IActionResult> DeleteEmployeeContacts(EmployeeContactDetailDTO inputDTO)
    //{
    //    try
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            EmployeeContactDetail outputMaster = _mapper.Map<EmployeeContactDetail>(await _unitOfWork.EmployeeContactDetail.GetByIdAsync(inputDTO.EmployeeId));
    //            outputMaster.IsActive = false;
    //            _unitOfWork.EmployeeContactDetail.Update(outputMaster);
    //            _unitOfWork.Save();
    //            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
    //        }
    //        return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error while deleting Employee {nameof(DeleteEmployeeContacts)}");
    //        throw;
    //    }
    //}

}
