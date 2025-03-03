using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Repository;
using System.Linq.Expressions;
using System.Net;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Helper;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class MaritalStatusMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MaritalStatusMasterController> _logger;
    private readonly IMapper _mapper;

    public MaritalStatusMasterController(IUnitOfWork unitOfWork, ILogger<MaritalStatusMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetMaritalStatus(MaritalStatusMasterDTO inputDTO)
    {
        try
        {
            MaritalStatusMasterDTO outputDTO = _mapper.Map<MaritalStatusMasterDTO>(await _unitOfWork.MaritalStatusMaster.GetByIdAsync(inputDTO.MaritalStatusId));
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
            _logger.LogError(ex, $"Error in retriving Marital Status {nameof(GetMaritalStatus)}");
            throw;
        }
    }

    [HttpPost(Name = "GetMaritalStates")]
    public async Task<IActionResult> GetMaritalStates(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<MaritalStatusMasterDTO> outputModel = new List<MaritalStatusMasterDTO>();
            outputModel = _mapper.Map<IList<MaritalStatusMasterDTO>>(await _unitOfWork.MaritalStatusMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving MaritalStatuss {nameof(GetMaritalStates)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveMaritalStatus(MaritalStatusMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<MaritalStatusMaster, bool>> expression = a => a.MaritalStatusName.Trim().Replace(" ", "") == inputDTO.MaritalStatusName.Trim().Replace(" ", "") && a.IsActive == true;
                if (!_unitOfWork.MaritalStatusMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<MaritalStatusMaster>(inputDTO);
                    _unitOfWork.MaritalStatusMaster.AddAsync(_mapper.Map<MaritalStatusMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Marital Status {nameof(SaveMaritalStatus)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateMaritalStatus(MaritalStatusMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<MaritalStatusMaster, bool>> expression = a => a.MaritalStatusName.Trim().Replace(" ", "") == inputDTO.MaritalStatusName.Trim().Replace(" ", "") && a.MaritalStatusId != inputDTO.MaritalStatusId && a.IsActive == true;
                if (!_unitOfWork.MaritalStatusMaster.Exists(expression))
                {
                    _unitOfWork.MaritalStatusMaster.Update(_mapper.Map<MaritalStatusMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in Marital Status updates {nameof(UpdateMaritalStatus)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMaritalStatus(MaritalStatusMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                MaritalStatusMaster outputMaster = _mapper.Map<MaritalStatusMaster>(await _unitOfWork.MaritalStatusMaster.GetByIdAsync(inputDTO.MaritalStatusId));
                outputMaster.IsActive = false;
                _unitOfWork.MaritalStatusMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Marital Status {nameof(DeleteMaritalStatus)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<MaritalStatusKeyValues> GetMaritalStatusKeyValue()
    {
        return (_unitOfWork.MaritalStatusMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new MaritalStatusKeyValues()
                           {
                               MaritalStatusId = p.MaritalStatusId,
                               MaritalStatusName = p.MaritalStatusName
                           })).ToList();
    }




}
