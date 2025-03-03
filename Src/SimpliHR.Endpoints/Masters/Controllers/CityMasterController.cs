using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Repository;
using System.Linq.Expressions;
using AutoMapper;
using SimpliHR.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Services.DBContext;
using System.Linq;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class CityMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CityMasterController> _logger;
    private readonly IMapper _mapper;

    public CityMasterController(IUnitOfWork unitOfWork, ILogger<CityMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetCity(CityMasterDTO inputDTO)
    {
        try
        {
            CityMasterDTO outputDTO = _mapper.Map<CityMasterDTO>(await _unitOfWork.CityMaster.GetByIdAsync(inputDTO.CityId));
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
            _logger.LogError(ex, $"Error in retriving employee {nameof(GetCity)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetUnitCity(UnitCityMasterDTO inputDTO)
    {
        try
        {
            UnitCityMasterDTO outputDTO = _mapper.Map<UnitCityMasterDTO>(await _unitOfWork.UnitCityMaster.GetByIdAsync(inputDTO.CityId));
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
            _logger.LogError(ex, $"Error in retriving employee {nameof(GetUnitCity)}");
            throw;
        }
    }

    [HttpGet(Name = "GetCities")]
    public async Task<IActionResult> GetCities(Core.Helper.RequestParams requestParams)
    {
        try
        {
           // IList<CityMasterDTO> viewModel = new List<CityMasterDTO>();

           var returnData = _mapper.Map<IList<CityMasterDTO>>(await _unitOfWork.CityMaster.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                             orderBy: (m => m.OrderBy(x => x.CityName))));
            IList<CityMasterDTO> viewModel = new List<CityMasterDTO>();

            var data = returnData.Select(r => new CityMasterDTO
            {
                CityId = r.CityId,
                CityName = r.CityName,
                CountryId = r.Country.CountryId,
                CountryName = r.Country.CountryName,
                StateId = r.State.StateId,
                StateName = r.State.StateName
            }).ToList();
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetCities)}");
            throw;
        }
    }


    [HttpGet(Name = "GetUnitCities")]
    public async Task<IActionResult> GetUnitCities(Core.Helper.RequestParams requestParams, int? unitId)
    {
        try
        {
            // IList<CityMasterDTO> viewModel = new List<CityMasterDTO>();

            var returnData = _mapper.Map<IList<UnitCityMasterDTO>>(await _unitOfWork.UnitCityMaster.GetAll(requestParams: requestParams,
                                                                                              expression: (p =>p.UnitId==unitId &&  p.IsActive == true),
                                                                                              orderBy: (m => m.OrderBy(x => x.CityName))));
           IList<UnitCityMasterDTO> viewModel = new List<UnitCityMasterDTO>();

            viewModel = returnData.Select(r => new UnitCityMasterDTO
            {
                CityId = r.CityId,
                CityName = r.CityName,
                CountryId = r.Country.CountryId,
                CountryName = r.Country.CountryName,
                StateId = r.State.StateId,
                StateName = r.State.StateName
            }).ToList();
            return Ok(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetUnitCities)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveCity(CityMasterDTO cityDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<CityMaster, bool>> expression = a => a.CityName.Replace(" ", "").Trim().ToLower() == cityDTO.CityName.Replace(" ", "").Trim().ToLower() && a.StateId == cityDTO.StateId && a.IsActive==true;
                if (!_unitOfWork.CityMaster.Exists(expression))
                {
                    var cityViewModel = _mapper.Map<CityMaster>(cityDTO);
                    _unitOfWork.CityMaster.AddAsync(cityViewModel);
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
            _logger.LogError(ex, $"Error in saving country {nameof(SaveCity)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveUnitCity(UnitCityMasterDTO cityDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<UnitCityMaster, bool>> expression = a => a.CityName.Replace(" ", "").Trim().ToLower() == cityDTO.CityName.Replace(" ", "").Trim().ToLower() && a.StateId == cityDTO.StateId && a.UnitId == cityDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.UnitCityMaster.Exists(expression))
                {
                    var cityViewModel = _mapper.Map<UnitCityMaster>(cityDTO);
                    _unitOfWork.UnitCityMaster.AddAsync(cityViewModel);
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
            _logger.LogError(ex, $"Error in saving country {nameof(SaveUnitCity)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateCity(CityMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<CityMaster, bool>> expression = a => a.CityName.Replace(" ", "").Trim().ToLower() == inputDTO.CityName.Replace(" ", "").Trim().ToLower() && a.StateId == inputDTO.StateId && a.IsActive == true && a.CityId!=inputDTO.CityId;
                if (!_unitOfWork.CityMaster.Exists(expression))
                {
                    var cityViewModel = _mapper.Map<CityMaster>(inputDTO);
                    _unitOfWork.CityMaster.Update(cityViewModel);
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
            _logger.LogError(ex, $"Error in country updates {nameof(UpdateCity)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateUnitCity(UnitCityMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<UnitCityMaster, bool>> expression = a => a.CityName.Replace(" ", "").Trim().ToLower() == inputDTO.CityName.Replace(" ", "").Trim().ToLower() && a.StateId == inputDTO.StateId && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.UnitCityMaster.Exists(expression))
                {
                    var cityViewModel = _mapper.Map<UnitCityMaster>(inputDTO);
                    _unitOfWork.UnitCityMaster.Update(cityViewModel);
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
            _logger.LogError(ex, $"Error in country updates {nameof(UpdateCity)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCity(CityMasterDTO cityDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                CityMaster cityMaster = _mapper.Map<CityMaster>(await _unitOfWork.CityMaster.GetByIdAsync(cityDTO.CityId));
                cityMaster.IsActive = false;
                _unitOfWork.CityMaster.Update(cityMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteCity)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteUnitCity(UnitCityMasterDTO cityDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                UnitCityMaster cityMaster = _mapper.Map<UnitCityMaster>(await _unitOfWork.UnitCityMaster.GetByIdAsync(cityDTO.CityId));
                cityMaster.IsActive = false;
                _unitOfWork.UnitCityMaster.Update(cityMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteUnitCity)}");
            throw;
        }
    }
    public List<CityMasterDTO> GetCityState(List<CityMasterDTO> inputData)
    {
        var dataState = (_unitOfWork.StateMaster.GetAll(null, null)).Result.ToList();

        List<CityMasterDTO> outputData = inputData;

        foreach (var item in outputData)
        {
            item.StateName = dataState.Where(x => x.StateId == item.StateId).Select(p => p.StateName).First();
        }
        return outputData;
    }

    public List<UnitCityMasterDTO> GetUnitCityState(List<UnitCityMasterDTO> inputData)
    {
        var dataState = (_unitOfWork.UnitStateMaster.GetAll(null, null)).Result.ToList();

        List<UnitCityMasterDTO> outputData = inputData;

        foreach (var item in outputData)
        {
            item.StateName = dataState.Where(x => x.StateId == item.StateId).Select(p => p.StateName).First();
        }
        return outputData;
    }
    public List<CityMasterDTO> GetCityCountry(List<CityMasterDTO> inputData)
    {
        var dataCountry = (_unitOfWork.CountryMaster.GetAll(null, null,orderBy: (m => m.OrderBy(x => x.CountryName)))).Result.ToList();
        List<CityMasterDTO> outputData = inputData;

        foreach (var item in outputData)
        {
            item.CountryName = dataCountry.Where(x => x.CountryId == item.CountryId).Select(p => p.CountryName).First();
        }
        return outputData;
    }

    [HttpPost]
    public IEnumerable<CityKeyValues> GetCityKeyValue()
    {
        return (_unitOfWork.CityMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new CityKeyValues()
                           {
                               CityId = p.CityId,
                               CityName = p.CityName
                           })).ToList();
    }

    [HttpPost]
    public IEnumerable<CityKeyValues> GetUnitCityKeyValue(int? unitId)
    {
        return (_unitOfWork.UnitCityMaster.GetAll(p => p.UnitId== unitId && p.IsActive == true).Result
                           .Select(p => new CityKeyValues()
                           {
                               CityId = p.CityId,
                               CityName = p.CityName
                           })).ToList();
    }

    public async Task<List<UnitMasterDTO>> GetClientUnits(int clientId)
    {
        try
        {
            List<UnitMasterDTO> outputDTO = _mapper.Map<List<UnitMasterDTO>>(_unitOfWork.UnitMaster.Find(x => x.ClientId == clientId && x.IsActive == true));
            return outputDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting HolidaysList {nameof(GetClientUnits)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult SaveUnitCityFromMaster(UnitCityListVM unitCityVM, string employeeId)
    {
        try
        {
            UnitCityMasterDTO inputDTO = new UnitCityMasterDTO();
            if (unitCityVM.UnitCityList.Count > 0)
            {
                foreach (var city in unitCityVM.UnitCityList)
                {
                    CityMasterDTO cityDetails = _mapper.Map<CityMasterDTO>(_unitOfWork.CityMaster.GetAll(null, null, null).Result.Where(a => a.CityId == city.CityId).FirstOrDefault());
                    inputDTO.UnitId = city.UnitId;
                    inputDTO.StateId = cityDetails.StateId;
                    inputDTO.CityName = cityDetails.CityName;
                    inputDTO.CityParentId = city.CityId;
                    inputDTO.CountryId = cityDetails.CountryId;
                    inputDTO.IsActive = true;
                    inputDTO.CreatedOn = DateTime.Now;
                    inputDTO.CreatedBy = Convert.ToInt32(employeeId);
                    inputDTO.ModifiedOn = DateTime.Now;
                    inputDTO.ModifiedBy = Convert.ToInt32(employeeId);

                    if (ModelState.IsValid)
                    {
                        Expression<Func<UnitCityMaster, bool>> expression = a => a.CityName.Trim().Replace(" ", "") == inputDTO.CityName.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                        if (!_unitOfWork.UnitCityMaster.Exists(expression))
                        {
                            //var outputDTO = _mapper.Map<BankMaster>(inputDTO);
                            _unitOfWork.UnitCityMaster.AddAsync(_mapper.Map<UnitCityMaster>(inputDTO));
                            _unitOfWork.Save();
                            // return Ok("Success");
                        }
                        // else
                        //return BadRequest("Duplicate Bank Name found");
                    }
                    else
                        return BadRequest("Invalid Model");

                }
                return Ok("Success");
            }
            return Ok("Success");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving Unit City {nameof(SaveUnitCityFromMaster)}");
            throw;
        }
    }
}
