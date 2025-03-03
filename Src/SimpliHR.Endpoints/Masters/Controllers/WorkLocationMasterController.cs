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
public class WorkLocationMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WorkLocationMasterController> _logger;
    private readonly IMapper _mapper;

    public WorkLocationMasterController(IUnitOfWork unitOfWork, ILogger<WorkLocationMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetWorkLocation(WorkLocationMasterDTO inputDTO)
    {
        try
        {
            WorkLocationMasterDTO outputDTO = _mapper.Map<WorkLocationMasterDTO>(await _unitOfWork.WorkLocationMaster.GetByIdAsync(inputDTO.WorkLocationId));
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
            _logger.LogError(ex, $"Error in retriving WorkLocation {nameof(GetWorkLocation)}");
            throw;
        }
    }

    [HttpPost(Name = "GetWorkLocations")]
    public async Task<IActionResult> GetWorkLocations(Core.Helper.RequestParams requestParams, int unitId)
    {
        try
        {
            var returnData = _mapper.Map<IList<WorkLocationMasterDTO>>(await _unitOfWork.WorkLocationMaster.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive) && p.UnitId == unitId),
                                                                                             orderBy: (m => m.OrderBy(x => x.Location))));
            IList<WorkLocationMasterDTO> outputModel = new List<WorkLocationMasterDTO>();

            outputModel = returnData.Select(r => new WorkLocationMasterDTO
            {
                WorkLocationId = r.WorkLocationId,
                Location = r.Location,
                Address = r.Address,
                Pincode = r.Pincode,
                CountryId = r.Country.CountryId,
                CountryName = r.Country.CountryName,
                StateId = r == null ? 0 : r.State == null ? 0 : r.State.StateId,
                StateName = r == null ? "" : r.State == null ? "" : r.State.StateName,
                CityId = r == null ? 0 : r.City == null ? 0 : r.City.CityId,
                CityName = r == null ? "" : r.City == null ? "" : r.City.CityName,
                IsActive = r.IsActive
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Work Locations {nameof(GetWorkLocations)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveWorkLocation(WorkLocationMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<WorkLocationMaster, bool>> expression = a => a.Location.Trim().Replace(" ", "") == inputDTO.Location.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.WorkLocationMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<WorkLocationMaster>(inputDTO);
                    _unitOfWork.WorkLocationMaster.AddAsync(_mapper.Map<WorkLocationMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Work Location {nameof(SaveWorkLocation)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateWorkLocation(WorkLocationMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<WorkLocationMaster, bool>> expression = a => a.Location.Trim().Replace(" ", "") == inputDTO.Location.Trim().Replace(" ", "") && a.UnitId==inputDTO.UnitId && a.WorkLocationId != inputDTO.WorkLocationId;
                if (!_unitOfWork.WorkLocationMaster.Exists(expression))
                {
                    _unitOfWork.WorkLocationMaster.Update(_mapper.Map<WorkLocationMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in WorkLocation updates {nameof(UpdateWorkLocation)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteWorkLocation(WorkLocationMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                WorkLocationMaster outputMaster = _mapper.Map<WorkLocationMaster>(await _unitOfWork.WorkLocationMaster.GetByIdAsync(inputDTO.WorkLocationId));
                outputMaster.IsActive = false;
                _unitOfWork.WorkLocationMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting WorkLocation {nameof(DeleteWorkLocation)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<WorkLocationKeyValues> GetWorkLocationKeyValue()
    {
        return (_unitOfWork.WorkLocationMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new WorkLocationKeyValues()
                           {
                               WorkLocationId = p.WorkLocationId,
                               Location = p.Location
                           })).ToList();
    }




}
