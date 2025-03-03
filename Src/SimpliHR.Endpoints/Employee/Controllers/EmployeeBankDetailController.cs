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
public class EmployeeBankDetailController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeBankDetailController> _logger;
    private readonly IMapper _mapper;

    public EmployeeBankDetailController(IUnitOfWork unitOfWork, ILogger<EmployeeBankDetailController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeBankDetail(EmployeeBankDetailDTO inputDTO)
    {
        try
        {
            EmployeeBankDetailDTO outputDTO = _mapper.Map<EmployeeBankDetailDTO>(await _unitOfWork.EmployeeBankDetail.GetByIdAsync(inputDTO.BankDetailId));
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
            _logger.LogError(ex, $"Error in retriving Employee Academic Details {nameof(GetEmployeeBankDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeBankDetail(EmployeeBankDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeBankDetail, bool>> expression = a => a.AccountNo.Trim().Replace(" ", "") == inputDTO.AccountNo && a.IsActive == true;
                 EmployeeBankDetailDTO bankDetailDTO= _mapper.Map<EmployeeBankDetailDTO>(_unitOfWork.EmployeeBankDetail.FindFirstByExpression(expression));
                int BankDetailId = 0;
                if (BankDetailId==0)
                {
                    //var outputDTO = _mapper.Map<EmployeeBankDetail>(inputDTO);
                    inputDTO.IsActive = true;
                    inputDTO = _mapper.Map<EmployeeBankDetailDTO>(_unitOfWork.EmployeeBankDetail.Insert(_mapper.Map<EmployeeBankDetail>(inputDTO)));                   
                    //_unitOfWork.Save();
                    BankDetailId = inputDTO.BankDetailId;
                    return Ok(BankDetailId);
                }
                else
                {
                    UpdateEmployeeBankDetail(bankDetailDTO);
                    inputDTO.BankDetailId = bankDetailDTO.BankDetailId;
                    return Ok("Employee Bank Detail already exists");
                }
                    
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Academic Detail {nameof(SaveEmployeeBankDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEditEmployeeBankDetail(EmployeeBankDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeBankDetail, bool>> expression = a => a.AccountNo.Trim().Replace(" ", "") == inputDTO.AccountNo  && a.IFSCCode.Trim().Replace(" ", "") == inputDTO.IFSCCode && a.IsActive == true && a.EmployeeId==inputDTO.EmployeeId;
                if (!_unitOfWork.EmployeeBankDetail.Exists(expression) && (!string.IsNullOrEmpty(inputDTO.AccountNo.Trim())) && (!string.IsNullOrEmpty(inputDTO.IFSCCode.Trim())))
                {
                    
                    //var outputDTO = _mapper.Map<EmployeeBankDetail>(inputDTO);
                    inputDTO.IsActive = false;
                    inputDTO = _mapper.Map<EmployeeBankDetailDTO>(_unitOfWork.EmployeeBankDetail.AddAsyncGetEntity(_mapper.Map<EmployeeBankDetail>(inputDTO)));
                    _unitOfWork.Save();
                    int bankId = inputDTO.BankDetailId;
                    return Ok(bankId);
                }
                else
                    return Ok("Employee Bank Detail already exists or invalid/no inputs found");
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Academic Detail {nameof(SaveEmployeeBankDetail)}");
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
            _unitOfWork.EmployeeUploadDocument.CallStoredProc("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser,@RefrenceId", new[] { screenTab, id, sessionId, loggedInUser, refId });
            attachmentMsg = "Success";
        }
        catch (Exception ex) { }
        
        
        return attachmentMsg;
    }

    [HttpPost]
    public IActionResult UpdateEmployeeBankDetail(EmployeeBankDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<EmployeeBankDetail, bool>> expression = a => a.AccountNo.Trim().Replace(" ", "") == inputDTO.AccountNo && a.BankDetailId != inputDTO.BankDetailId && a.IsActive == true;
                if (!_unitOfWork.EmployeeBankDetail.Exists(expression))
                {
                    inputDTO.IsActive = true;
                    _unitOfWork.EmployeeBankDetail.Update(_mapper.Map<EmployeeBankDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return Ok("Duplicate Bank Account Number found");

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee Academic Detail updates {nameof(UpdateEmployeeBankDetail)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeBankDetail(EmployeeBankDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeBankDetail outputMaster = _mapper.Map<EmployeeBankDetail>(await _unitOfWork.EmployeeBankDetail.GetByIdAsync(inputDTO.BankDetailId));
                outputMaster.IsActive = false;
                _unitOfWork.EmployeeBankDetail.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Employee Academic Detail {nameof(DeleteEmployeeBankDetail)}");
            throw;
        }
    }

}
