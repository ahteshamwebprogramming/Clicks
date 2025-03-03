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
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Core.Helper;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class AcademicMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AcademicMasterController> _logger;
    private readonly IMapper _mapper;

    public AcademicMasterController(IUnitOfWork unitOfWork, ILogger<AcademicMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetAcademic(AcademicMasterDTO inputDTO)
    {
        try
        {
            AcademicMasterDTO outputDTO = _mapper.Map<AcademicMasterDTO>(await _unitOfWork.AcademicMaster.GetByIdAsync(inputDTO.AcademicId));
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
            _logger.LogError(ex, $"Error in retriving Academic {nameof(GetAcademic)}");
            throw;
        }
    }

    [HttpPost(Name = "GetAcademics")]
    public async Task<IActionResult> GetAcademics(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<AcademicMasterDTO> outputModel = new List<AcademicMasterDTO>();
            outputModel = _mapper.Map<IList<AcademicMasterDTO>>(await _unitOfWork.AcademicMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetAcademics)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveAcademic(AcademicMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<AcademicMaster, bool>> expression = a => a.AcademicName.Trim().Replace(" ", "") == inputDTO.AcademicName.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.AcademicMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<AcademicMaster>(inputDTO);
                    _unitOfWork.AcademicMaster.AddAsync(_mapper.Map<AcademicMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Duplicate Entry Found");
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Academic {nameof(SaveAcademic)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateAcademic(AcademicMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<AcademicMaster, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.AcademicName.Trim().Replace(" ", "") == inputDTO.AcademicName.Trim().Replace(" ", "") && a.AcademicId != inputDTO.AcademicId && a.IsActive == true;
                if (!_unitOfWork.AcademicMaster.Exists(expression))
                {
                    _unitOfWork.AcademicMaster.Update(_mapper.Map<AcademicMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                    return BadRequest("Duplicate Entry Found");

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Academic updates {nameof(UpdateAcademic)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAcademic(AcademicMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                AcademicMaster outputMaster = _mapper.Map<AcademicMaster>(await _unitOfWork.AcademicMaster.GetByIdAsync(inputDTO.AcademicId));
                outputMaster.IsActive = false;
                _unitOfWork.AcademicMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Academic {nameof(DeleteAcademic)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<AcademicKeyValues> GetAcademicKeyValue()
    {
        return (_unitOfWork.AcademicMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new AcademicKeyValues()
                           {
                               AcademicId = p.AcademicId,
                               AcademicName = p.AcademicName
                           })).ToList();
    }




}
