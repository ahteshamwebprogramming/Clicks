using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Helper;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class CountryMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryMasterController> _logger;
    private readonly IMapper _mapper;

    public CountryMasterController(IUnitOfWork unitOfWork, ILogger<CountryMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetCountry(CountryMasterDTO inputDTO)
    {
        try
        {
            CountryMasterDTO outputDTO = _mapper.Map<CountryMasterDTO>(await _unitOfWork.CountryMaster.GetByIdAsync(inputDTO.CountryId));
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
            _logger.LogError(ex, $"Error in retriving employee {nameof(GetCountry)}");
            throw;
        }
    }

    [HttpPost(Name = "GetCountries")]
    public async Task<IActionResult> GetCountries(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<CountryMasterDTO> ViewModel = new List<CountryMasterDTO>();
            ViewModel = _mapper.Map<IList<CountryMasterDTO>>(await _unitOfWork.CountryMaster.GetPagedListWithExpression(requestParams,p=>p.IsActive==true));

            return Ok(ViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetCountries)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveCountry(CountryMasterDTO countryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<CountryMaster, bool>> expression = a => a.CountryName.Trim().Replace(" ", "") == countryDTO.CountryName.Trim().Replace(" ", "") && a.IsActive == true;
                if (!_unitOfWork.CountryMaster.Exists(expression))
                {
                    var countryViewModel = _mapper.Map<CountryMaster>(countryDTO);
                    _unitOfWork.CountryMaster.AddAsync(countryViewModel);
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
            _logger.LogError(ex, $"Error in saving country {nameof(SaveCountry)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateCountry(CountryMasterDTO countryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<CountryMaster, bool>> expression = a => a.CountryName.Trim().Replace(" ", "") == countryDTO.CountryName.Trim().Replace(" ", "") && a.CountryId != countryDTO.CountryId && a.IsActive==true;
                if (!_unitOfWork.CountryMaster.Exists(expression))
                {
                    var countryViewModel = _mapper.Map<CountryMaster>(countryDTO);
                    _unitOfWork.CountryMaster.Update(countryViewModel);
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
            _logger.LogError(ex, $"Error in country updates {nameof(UpdateCountry)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCountry(CountryMasterDTO countryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                CountryMaster countryMaster = _mapper.Map<CountryMaster>(await _unitOfWork.CountryMaster.GetByIdAsync(countryDTO.CountryId));
                countryMaster.IsActive = false;
                _unitOfWork.CountryMaster.Update(countryMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteCountry)}");
            throw;
        }
    }


    [HttpGet]
    public IEnumerable<CountryKeyValues> GetCountryKeyValue()
    {
       
        return (_unitOfWork.CountryMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new CountryKeyValues()
                           {
                               CountryId = p.CountryId,
                               CountryName = p.CountryName
                           })).ToList();
    }

   


}
