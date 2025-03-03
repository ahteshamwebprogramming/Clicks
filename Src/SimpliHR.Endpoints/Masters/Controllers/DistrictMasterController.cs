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
public class DistrictMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DistrictMasterController> _logger;
    private readonly IMapper _mapper;

    public DistrictMasterController(IUnitOfWork unitOfWork, ILogger<DistrictMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetDistrict(DistrictMasterDTO inputDTO)
    {
        try
        {
            DistrictMasterDTO outputDTO = _mapper.Map<DistrictMasterDTO>(await _unitOfWork.DistrictMaster.GetByIdAsync(inputDTO.DistrictId));
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
            _logger.LogError(ex, $"Error in retriving employee {nameof(GetDistrict)}");
            throw;
        }
    }

    [HttpGet(Name = "GetDistricts")]
    public async Task<IActionResult> GetDistricts(Core.Helper.RequestParams requestParams)
    {
        try
        {


            //IList<DistrictMasterDTO> countryViewModel = _mapper.Map<IList<DistrictMasterDTO>>(_unitOfWork.DistrictMaster.GetAll());
            IList<DistrictMasterDTO> viewModel = new List<DistrictMasterDTO>();
            viewModel = _mapper.Map<IList<DistrictMasterDTO>>(await _unitOfWork.DistrictMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true)); //q => q.Include(x => x.Country):null)
            return Ok(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetDistricts)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveDistrict(DistrictMasterDTO districtDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<DistrictMaster, bool>> expression = a => a.DistrictName.Replace(" ", "").Trim().ToLower() == districtDTO.DistrictName.Replace(" ", "").Trim().ToLower() && a.StateId == districtDTO.StateId;
                if (!_unitOfWork.DistrictMaster.Exists(expression))
                {
                    var districtViewModel = _mapper.Map<DistrictMaster>(districtDTO);
                    _unitOfWork.DistrictMaster.AddAsync(districtViewModel);
                    _unitOfWork.Save();
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                }
                else
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving country {nameof(SaveDistrict)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateDistrict(DistrictMasterDTO districtDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<DistrictMaster, bool>> expression = a => a.DistrictName.Replace(" ", "").Trim().ToLower() == districtDTO.DistrictName.Replace(" ", "").Trim().ToLower() && a.StateId == districtDTO.StateId;
                if (!_unitOfWork.DistrictMaster.Exists(expression))
                {
                    var stateViewModel = _mapper.Map<DistrictMaster>(districtDTO);
                    _unitOfWork.DistrictMaster.Update(stateViewModel);
                    _unitOfWork.Save();
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
                }
                else
                    return Ok(ClientResponse.GetClientResponse(HttpStatusCode.Conflict, "Duplicate records found"));

            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country updates {nameof(UpdateDistrict)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDistrict(DistrictMasterDTO districtDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                DistrictMaster districtMaster = _mapper.Map<DistrictMaster>(await _unitOfWork.DistrictMaster.GetByIdAsync(districtDTO.DistrictId));
                districtMaster.IsActive = false;
                _unitOfWork.DistrictMaster.Update(districtMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteDistrict)}");
            throw;
        }
    }

    public List<DistrictMasterDTO> GetDistrictStates(List<DistrictMasterDTO> inputData,int countryId)
    {
        var data = (_unitOfWork.StateMaster.GetAll(p => p.CountryId==countryId)).Result.ToList();
        List<DistrictMasterDTO> outputData = inputData;

        foreach (var item in inputData)
        {
           item.StateName = data.Where(x => x.StateId == item.StateId).Select(p=>p.StateName).First();
        }
        return outputData;
    }

    [HttpPost]
    public IEnumerable<DistrictKeyValues> GetDistrictKeyValue()
    {
        return (_unitOfWork.DistrictMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new DistrictKeyValues()
                           {
                               DistrictId = p.DistrictId,
                               DistrictName = p.DistrictName
                           })).ToList();
    }

}
