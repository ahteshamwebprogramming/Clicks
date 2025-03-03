using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Services.DBContext;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.Endpoints.Attendance;


[Route("api/[controller]/[action]")]
[ApiController]
public class WorkFlowSettingsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkFlowSettingsAPIController> _logger;
    private readonly IMapper _mapper;
    public WorkFlowSettingsAPIController(IUnitOfWork unitOfWork, ILogger<WorkFlowSettingsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetWorkFlowSettings(WorkFlowSettingsDTO inputDTO)
    {
        try
        {
            WorkFlowSettingsDTO outputDTO = _mapper.Map<WorkFlowSettingsDTO>(await _unitOfWork.WorkFlowSettings.GetByIdAsync(inputDTO.WorkFlowSettingsId));
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
            _logger.LogError(ex, $"Error in retriving WorkFlow Setting {nameof(GetWorkFlowSettings)}");
            throw;
        }
    }


    [HttpPost(Name = "GetWorkFlowSettingList")]
    public async Task<IActionResult> GetWorkFlowSettingList(Core.Helper.RequestParams requestParams, int? UnitId)
    {

        try
        {
            var returnData = _mapper.Map<IList<WorkFlowSettingsDTO>>(await _unitOfWork.WorkFlowSettings.GetAll(requestParams: requestParams,
                                                                                             expression: (p =>p.UnitId== UnitId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                             orderBy: (m => m.OrderBy(x => x.WorkFlowSettingsId))));
            IList<WorkFlowSettingsDTO> outputModel = new List<WorkFlowSettingsDTO>();

            outputModel = returnData.Select(r => new WorkFlowSettingsDTO
            {
                ActionId = r.ActionId,
                WorkFlowSettingsId = r.WorkFlowSettingsId,
                Name = r.Name,
                Authority1 = r.Authority1,
                Authority2 = r.Authority2,
                Authority3 = r.Authority3,
                ModuleId = r.ModuleId,
                ModuleName = r.Module.ModuleName,       
                IsActive = r.IsActive
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Work Locations {nameof(GetWorkFlowSettingList)}");
            throw;
        }
       
    }


    [HttpPost]
    public async Task<IActionResult> GetWorkFlowSettingByID(int inputDTO)
    {
        try
        {
            WorkFlowSettingsDTO outputDTO = _mapper.Map<WorkFlowSettingsDTO>(await _unitOfWork.WorkFlowSettings.GetByIdAsync(inputDTO));
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
            _logger.LogError(ex, $"Error in retriving Get WorkFlow Setting by ID {nameof(GetWorkFlowSettingByID)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult WorkFlowSetting(WorkFlowSettingsDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.WorkFlowSettingsId == 0)
                {
                    Expression<Func<WorkFlowSettings, bool>> expression = a => a.UnitId== inputDTO.UnitId &&  a.Name == inputDTO.Name && a.IsActive == true;
                    if (!_unitOfWork.WorkFlowSettings.Exists(expression))
                    {
                        _unitOfWork.WorkFlowSettings.AddAsync(_mapper.Map<WorkFlowSettings>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("ADDSUCCESS");
                    }
                    else
                        return BadRequest("Duplicate Entry Found");
                }
                else
                {
                    
                    Expression<Func<WorkFlowSettings, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.Name == inputDTO.Name && a.IsActive == true && a.WorkFlowSettingsId != inputDTO.WorkFlowSettingsId;
                    if (!_unitOfWork.WorkFlowSettings.Exists(expression))
                    {
                        _unitOfWork.WorkFlowSettings.Update(_mapper.Map<WorkFlowSettings>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("EDITSUCCESS");
                    }
                    else
                        return BadRequest("Duplicate Entry Found");
                    
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Attendance Setting {nameof(WorkFlowSetting)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteWorkflowSetting(WorkFlowSettingsDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                WorkFlowSettings outputMaster = _mapper.Map<WorkFlowSettings>(await _unitOfWork.WorkFlowSettings.GetByIdAsync(inputDTO.WorkFlowSettingsId));
                outputMaster.IsActive = false;
                _unitOfWork.WorkFlowSettings.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "DELETESUCCESS"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Work setting {nameof(DeleteWorkflowSetting)}");
            throw;
        }
    }
}



