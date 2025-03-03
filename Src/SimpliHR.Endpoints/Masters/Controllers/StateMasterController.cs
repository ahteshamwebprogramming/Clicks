using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using System.Linq.Expressions;
using System.Net;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Helper;
using System.Linq;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class StateMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StateMasterController> _logger;
    private readonly IMapper _mapper;

    public StateMasterController(IUnitOfWork unitOfWork, ILogger<StateMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetState(StateMasterDTO inputDTO)
    {
        try
        {
           
            StateMasterDTO outputDTO = _mapper.Map<StateMasterDTO>( await _unitOfWork.StateMaster.GetByIdAsync(inputDTO.StateId));
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
            _logger.LogError(ex, $"Error in retriving state {nameof(GetState)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetUnitState(UnitStateMasterDTO inputDTO)
    {
        try
        {

            UnitStateMasterDTO outputDTO = _mapper.Map<UnitStateMasterDTO>(await _unitOfWork.UnitStateMaster.GetByIdAsync(inputDTO.StateId));
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
            _logger.LogError(ex, $"Error in retriving state {nameof(GetState)}");
            throw;
        }
    }

    [HttpPost(Name = "GetStates")]
    public async Task<IActionResult> GetStates(Core.Helper.RequestParams requestParams)
    {
        try
        {
           var returnData = _mapper.Map<IList<StateMasterDTO>>(await _unitOfWork.StateMaster.GetAll(requestParams: requestParams,
                                                                                         expression: (p => p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                         orderBy: (m => m.OrderBy(x => x.StateName))));
            IList<StateMasterDTO> outputModel = new List<StateMasterDTO>();

            outputModel = returnData.Select(r => new StateMasterDTO
            {
                StateId = r.StateId,
                StateName=r.StateName,
                CountryId = r.Country.CountryId,
                CountryName = r.Country.CountryName,
                IsActive = r.IsActive
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetStates)}");
            throw;
        }
    }

    [HttpPost(Name = "GetUnitStates")]
    public async Task<IActionResult> GetUnitStates(Core.Helper.RequestParams requestParams,int? unitId)
    {
        try
        {
            var returnData = _mapper.Map<IList<UnitStateMasterDTO>>(await _unitOfWork.UnitStateMaster.GetAll(requestParams: requestParams,
                                                                                          expression: (p => p.UnitId == unitId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                          orderBy: (m => m.OrderBy(x => x.StateName))));
            IList<UnitStateMasterDTO> outputModel = new List<UnitStateMasterDTO>();

            outputModel = returnData.Select(r => new UnitStateMasterDTO
            {
                StateId = r.StateId,
                StateName = r.StateName,
                CountryId = r.Country.CountryId,
                CountryName = r.Country.CountryName,
                IsActive = r.IsActive
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetUnitStates)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveState(StateMasterDTO stateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<StateMaster, bool>> expression = a => a.StateName.Trim().Replace(" ", "") == stateDTO.StateName.Trim().Replace(" ", "") && a.CountryId == stateDTO.CountryId && a.IsActive==true;
                if (!_unitOfWork.StateMaster.Exists(expression))
                {
                    var stateViewModel = _mapper.Map<StateMaster>(stateDTO);
                    _unitOfWork.StateMaster.AddAsync(stateViewModel);
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
            _logger.LogError(ex, $"Error in saving country {nameof(SaveState)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveUnitState(UnitStateMasterDTO stateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<UnitStateMaster, bool>> expression = a => a.StateName.Trim().Replace(" ", "") == stateDTO.StateName.Trim().Replace(" ", "") && a.CountryId == stateDTO.CountryId && a.UnitId == stateDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.UnitStateMaster.Exists(expression))
                {
                    var stateViewModel = _mapper.Map<UnitStateMaster>(stateDTO);
                    _unitOfWork.UnitStateMaster.AddAsync(stateViewModel);
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
            _logger.LogError(ex, $"Error in saving country {nameof(SaveState)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateState(StateMasterDTO stateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<StateMaster, bool>> expression = a => a.StateName.Trim().Replace(" ", "") == stateDTO.StateName.Trim().Replace(" ", "") && a.StateId != stateDTO.StateId && a.IsActive == true;
                if (!_unitOfWork.StateMaster.Exists(expression))
                {
                    var stateViewModel = _mapper.Map<StateMaster>(stateDTO);
                    _unitOfWork.StateMaster.Update(stateViewModel);
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
            _logger.LogError(ex, $"Error in country updates {nameof(UpdateState)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateUnitState(UnitStateMasterDTO stateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<UnitStateMaster, bool>> expression = a => a.StateName.Trim().Replace(" ", "") == stateDTO.StateName.Trim().Replace(" ", "") && a.StateId != stateDTO.StateId && a.IsActive == true;
                if (!_unitOfWork.UnitStateMaster.Exists(expression))
                {
                    var stateViewModel = _mapper.Map<UnitStateMaster>(stateDTO);
                    _unitOfWork.UnitStateMaster.Update(stateViewModel);
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
            _logger.LogError(ex, $"Error in country updates {nameof(UpdateState)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteState(StateMasterDTO stateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                StateMaster stateMaster = _mapper.Map<StateMaster>(await _unitOfWork.StateMaster.GetByIdAsync(stateDTO.StateId));
                stateMaster.IsActive = false;
                _unitOfWork.StateMaster.Update(stateMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteState)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteUnitState(UnitStateMasterDTO stateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                UnitStateMaster stateMaster = _mapper.Map<UnitStateMaster>(await _unitOfWork.UnitStateMaster.GetByIdAsync(stateDTO.StateId));
                stateMaster.IsActive = false;
                _unitOfWork.UnitStateMaster.Update(stateMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteUnitState)}");
            throw;
        }
    }

    public List<StateMasterDTO> GetStatesCountry(List<StateMasterDTO> inputData)
    {
        var data = (_unitOfWork.CountryMaster.GetAll(p => p.IsActive == true)).Result.ToList();
        List<StateMasterDTO> outputData = inputData;

        foreach (var item in inputData)
        {
            item.CountryName = data.Where(x => x.CountryId == item.CountryId).Select(p => p.CountryName).First();
        }
        return outputData;
    }

    public List<UnitStateMasterDTO> GetUnitStatesCountry(List<UnitStateMasterDTO> inputData)
    {
        var data = (_unitOfWork.CountryMaster.GetAll(p => p.IsActive == true)).Result.ToList();
        List<UnitStateMasterDTO> outputData = inputData;

        foreach (var item in inputData)
        {
            item.CountryName = data.Where(x => x.CountryId == item.CountryId).Select(p => p.CountryName).First();
        }
        return outputData;
    }
    [HttpGet]
    public IEnumerable<StateKeyValues> GetStateKeyValue(int countryId = 0)
    {
        if (countryId == 0)
            return (_unitOfWork.StateMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new StateKeyValues()
                               {
                                   StateId = p.StateId,
                                   StateName = p.StateName
                               })).ToList();
        else
            return (_unitOfWork.StateMaster.GetAll(p => p.IsActive == true && p.CountryId == countryId).Result
                          .Select(p => new StateKeyValues()
                          {
                              StateId = p.StateId,
                              StateName = p.StateName
                          })).ToList();
    }


    [HttpGet]
    public IEnumerable<StateKeyValues> GetUnitStateKeyValue(int countryId = 0,int? unitId=0)
    {
        if (countryId == 0)
            return (_unitOfWork.UnitStateMaster.GetAll(p => p.UnitId== unitId && p.IsActive == true).Result
                               .Select(p => new StateKeyValues()
                               {
                                   StateId = p.StateId,
                                   StateName = p.StateName
                               })).ToList();
        else
            return (_unitOfWork.UnitStateMaster.GetAll(p => p.UnitId == unitId && p.IsActive == true && p.CountryId == countryId).Result
                          .Select(p => new StateKeyValues()
                          {
                              StateId = p.StateId,
                              StateName = p.StateName
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
    public IActionResult SaveUnitStateFromMaster(UnitStateListVM unitStateVM, string employeeId)
    {
        try
        {
            UnitStateMasterDTO inputDTO = new UnitStateMasterDTO();
            if (unitStateVM.UnitStateList.Count > 0)
            {
                foreach (var state in unitStateVM.UnitStateList)
                {
                    StateMasterDTO stateDetails = _mapper.Map<StateMasterDTO>(_unitOfWork.StateMaster.GetAll(null, null, null).Result.Where(a => a.StateId == state.StateId).FirstOrDefault());
                    inputDTO.UnitId = state.UnitId;
                    inputDTO.StateName = stateDetails.StateName;
                    inputDTO.StateParentId = state.StateId;
                    inputDTO.CountryId = stateDetails.CountryId;
                    inputDTO.IsActive = true;
                    inputDTO.CreatedOn = DateTime.Now;
                    inputDTO.CreatedBy = Convert.ToInt32(employeeId);
                    inputDTO.ModifiedOn = DateTime.Now;
                    inputDTO.ModifiedBy = Convert.ToInt32(employeeId);

                    if (ModelState.IsValid)
                    {
                        Expression<Func<UnitStateMaster, bool>> expression = a => a.StateName.Trim().Replace(" ", "") == inputDTO.StateName.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                        if (!_unitOfWork.UnitStateMaster.Exists(expression))
                        {
                            //var outputDTO = _mapper.Map<BankMaster>(inputDTO);
                            _unitOfWork.UnitStateMaster.AddAsync(_mapper.Map<UnitStateMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving state {nameof(SaveUnitStateFromMaster)}");
            throw;
        }
    }

}

