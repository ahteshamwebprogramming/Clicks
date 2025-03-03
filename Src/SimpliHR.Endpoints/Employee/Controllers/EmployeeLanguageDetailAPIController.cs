using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Services.DBContext;
using System.Linq.Expressions;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class EmployeeLanguageDetailAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeLanguageDetailAPIController> _logger;
    private readonly IMapper _mapper;
    public EmployeeLanguageDetailAPIController(IUnitOfWork unitOfWork, ILogger<EmployeeLanguageDetailAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> GetEmployeeLanguageDetail(EmployeeLanguageDetailDTO inputDTO)
    {
        try
        {
            EmployeeLanguageDetailDTO outputDTO = _mapper.Map<EmployeeLanguageDetailDTO>(await _unitOfWork.EmployeeLanguageDetail.GetByIdAsync(inputDTO.EmployeeLanguageDetailId));
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
            _logger.LogError(ex, $"Error in retriving Employee Experience Details {nameof(GetEmployeeLanguageDetail)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetEmployeeLanguageList(SimpliHR.Core.Helper.RequestParams requestParams, int? employeeID)
    {
        try
        {
            IList<EmployeeLanguageDetailDTO> outputModel = new List<EmployeeLanguageDetailDTO>();
            outputModel = _mapper.Map<IList<EmployeeLanguageDetailDTO>>(await _unitOfWork.EmployeeLanguageDetail.GetAll(requestParams, p => p.IsActive == true && p.EmployeeId == employeeID, null)).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees Experience Details {nameof(GetEmployeeLanguageList)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEditEmployeeLanguageDetail(EmployeeLanguageDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                Expression<Func<EmployeeLanguageDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true && a.LanguageId == inputDTO.LanguageId;
                if (!_unitOfWork.EmployeeLanguageDetail.Exists(expression))
                {
                    int detailId = _unitOfWork.EmployeeLanguageDetail.Insert(_mapper.Map<EmployeeLanguageDetail>(inputDTO)).EmployeeLanguageDetailId;
                    //_unitOfWork.Save();
                    //int detailId = _unitOfWork.EmployeeLanguageDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeLanguageDetailId);
                    return Ok(detailId);
                }
                else
                    return BadRequest("Employee Language already exists");
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Experience Detail {nameof(SaveEditEmployeeLanguageDetail)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveEmployeeLanguageDetail(EmployeeLanguageDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                Expression<Func<EmployeeLanguageDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true && a.LanguageId == inputDTO.LanguageId;
                if (!_unitOfWork.EmployeeLanguageDetail.Exists(expression))
                {
                    int detailId = _unitOfWork.EmployeeLanguageDetail.Insert(_mapper.Map<EmployeeLanguageDetail>(inputDTO)).EmployeeLanguageDetailId;
                    //_unitOfWork.Save();
                    //int detailId = _unitOfWork.EmployeeLanguageDetail.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.EmployeeLanguageDetailId);
                    return Ok(detailId);
                }
                else
                    return BadRequest("Employee Language already exists");
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Employee Experience Detail {nameof(SaveEmployeeLanguageDetail)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult UpdateEmployeeLanguageDetail(EmployeeLanguageDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                Expression<Func<EmployeeLanguageDetail, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.LanguageId == inputDTO.LanguageId && a.IsActive == true && a.EmployeeLanguageDetailId != inputDTO.EmployeeLanguageDetailId;
                if (!_unitOfWork.EmployeeLanguageDetail.Exists(expression))
                {
                    _unitOfWork.EmployeeLanguageDetail.Update(_mapper.Map<EmployeeLanguageDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Language already exists");


            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee Experience Detail updates {nameof(UpdateEmployeeLanguageDetail)}");
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
            _unitOfWork.EmployeeLanguageDetail.ExecuteRawQuery("EXEC SaveAttachmentFromTemp @ScreenTab,@Id,@SessionId,@LoggedInUser,@RefrenceId", new[] { screenTab, id, sessionId, loggedInUser, refId });
            attachmentMsg = "Success";
        }
        catch (Exception ex) { }


        return attachmentMsg;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmployeeLanguageDetail(EmployeeLanguageDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                EmployeeLanguageDetail dto = await _unitOfWork.EmployeeLanguageDetail.GetByIdAsync(inputDTO.EmployeeLanguageDetailId);
                dto.IsActive = false;
                _unitOfWork.EmployeeLanguageDetail.Update(_mapper.Map<EmployeeLanguageDetail>(dto));
                _unitOfWork.Save();
                return Ok("Success");
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Employee Experience Detail updates {nameof(UpdateEmployeeLanguageDetail)}");
            throw;
        }
    }
}

