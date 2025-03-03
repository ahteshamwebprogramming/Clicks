using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using System.Linq.Expressions;
using System.Net;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Core.Entities;
using System.Collections.Generic;
using SimpliHR.Core.Helper;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class BandMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BandMasterController> _logger;
    private readonly IMapper _mapper;

    public BandMasterController(IUnitOfWork unitOfWork, ILogger<BandMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetBand(BandMasterDTO inputDTO)
    {
        try
        {
            BandMasterDTO outputDTO = _mapper.Map<BandMasterDTO>(await _unitOfWork.BandMaster.GetByIdAsync(inputDTO.BandId));
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
            _logger.LogError(ex, $"Error in retriving Band {nameof(GetBand)}");
            throw;
        }
    }

    [HttpPost(Name = "GetBands")]
    public async Task<IActionResult> GetBands(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<BandMasterDTO> outputModel = new List<BandMasterDTO>();
            outputModel = _mapper.Map<IList<BandMasterDTO>>(await _unitOfWork.BandMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Bands {nameof(GetBands)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveBand(BandMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<BandMaster, bool>> expression = a => (a.Band.Trim().Replace(" ", "") == inputDTO.Band.Trim().Replace(" ", "") || a.BandCode.Trim().Replace(" ", "") == inputDTO.BandCode.Trim().Replace(" ", "")) && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.BandMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<BandMaster>(inputDTO);
                    _unitOfWork.BandMaster.AddAsync(_mapper.Map<BandMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Band {nameof(SaveBand)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateBand(BandMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<BandMaster, bool>> expression = a => (a.Band.Trim().Replace(" ", "") == inputDTO.Band.Trim().Replace(" ", "") || a.BandCode.Trim().Replace(" ", "") == inputDTO.BandCode.Trim().Replace(" ", "")) && a.UnitId == inputDTO.UnitId && a.BandId != inputDTO.BandId && a.IsActive == true;
                if (!_unitOfWork.BandMaster.Exists(expression))
                {
                    _unitOfWork.BandMaster.Update(_mapper.Map<BandMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in Band updates {nameof(UpdateBand)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBand(BandMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                BandMaster outputMaster = _mapper.Map<BandMaster>(await _unitOfWork.BandMaster.GetByIdAsync(inputDTO.BandId));
                outputMaster.IsActive = false;
                _unitOfWork.BandMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Band {nameof(DeleteBand)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<BandKeyValues> GetBandKeyValue()
    {
        return (_unitOfWork.BandMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new BandKeyValues()
                           {
                               BandId = p.BandId,
                               Band = p.Band
                           })).ToList();
    }




}
