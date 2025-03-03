using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Helper;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Linq.Expressions;
using System.Net;


namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class BankMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BandMasterController> _logger;
    private readonly IMapper _mapper;

    public BankMasterController(IUnitOfWork unitOfWork, ILogger<BandMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetBank(BankMasterDTO inputDTO)
    {
        try
        {
            BankMasterDTO outputDTO = _mapper.Map<BankMasterDTO>(await _unitOfWork.BankMaster.GetByIdAsync(inputDTO.BankId));
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
            _logger.LogError(ex, $"Error in retriving Bank {nameof(GetBank)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetUnitBank(BankUnitMasterDTO inputDTO)
    {
        try
        {
            BankUnitMasterDTO outputDTO = _mapper.Map<BankUnitMasterDTO>(await _unitOfWork.BankUnitMaster.GetByIdAsync(inputDTO.BankId));
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
            _logger.LogError(ex, $"Error in retriving Bank {nameof(GetBank)}");
            throw;
        }
    }

    [HttpPost(Name = "GetUnitBanks")]
    public async Task<IActionResult> GetUnitBanks(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<BankUnitMasterDTO> outputModel = new List<BankUnitMasterDTO>();
            outputModel = _mapper.Map<IList<BankUnitMasterDTO>>(await _unitOfWork.BankUnitMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Banks {nameof(GetUnitBanks)}");
            throw;
        }
    }


    [HttpPost(Name = "GetBankMaster")]
    public async Task<IActionResult> GetBankMaster(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<BankMasterDTO> outputModel = new List<BankMasterDTO>();
            outputModel = _mapper.Map<IList<BankMasterDTO>>(await _unitOfWork.BankMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Banks {nameof(GetBankMaster)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveUnitBank(BankUnitMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<BankUnitMaster, bool>> expression = a => a.BankName.Trim().Replace(" ","") == inputDTO.BankName.Trim().Replace(" ", "") && a.UnitId==inputDTO.UnitId && a.IsActive==true;
                if (!_unitOfWork.BankUnitMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<BankMaster>(inputDTO);
                    _unitOfWork.BankUnitMaster.AddAsync(_mapper.Map<BankUnitMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving bank {nameof(SaveUnitBank)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult SaveBankMaster(BankMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<BankMaster, bool>> expression = a => a.BankName.Trim().Replace(" ", "") == inputDTO.BankName.Trim().Replace(" ", "") && a.IsActive == true;
                if (!_unitOfWork.BankMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<BankMaster>(inputDTO);
                    _unitOfWork.BankMaster.AddAsync(_mapper.Map<BankMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving bank {nameof(SaveBankMaster)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateBankMaster(BankMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<BankMaster, bool>> expression = a => a.BankName.Trim().Replace(" ", "") == inputDTO.BankName.Trim().Replace(" ", "") && a.BankId != inputDTO.BankId && a.IsActive == true;
                if (!_unitOfWork.BankMaster.Exists(expression))
                {
                    _unitOfWork.BankMaster.Update(_mapper.Map<BankMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in bank updates {nameof(UpdateBankMaster)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult UpdateBank(BankUnitMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<BankUnitMaster, bool>> expression = a => a.BankName.Trim().Replace(" ", "") == inputDTO.BankName.Trim().Replace(" ", "") && a.BankId != inputDTO.BankId && a.IsActive == true;
                if (!_unitOfWork.BankUnitMaster.Exists(expression))
                {
                    _unitOfWork.BankUnitMaster.Update(_mapper.Map<BankUnitMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in bank updates {nameof(UpdateBank)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBank(BankUnitMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                BankUnitMaster outputMaster = _mapper.Map<BankUnitMaster>(await _unitOfWork.BankUnitMaster.GetByIdAsync(inputDTO.BankId));
                outputMaster.IsActive = false;
                _unitOfWork.BankUnitMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting bank {nameof(DeleteBank)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBankMaster(BankMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                BankMaster outputMaster = _mapper.Map<BankMaster>(await _unitOfWork.BankMaster.GetByIdAsync(inputDTO.BankId));
                outputMaster.IsActive = false;
                _unitOfWork.BankMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting bank {nameof(DeleteBankMaster)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<BankKeyValues> GetBankKeyValue()
    {
        return (_unitOfWork.BankMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new BankKeyValues()
                           {
                               BankId = p.BankId,
                               BankName = p.BankName
                           })).ToList();
    }

    //Unit Mapping
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
    public IActionResult SaveUnitBankFromMaster(UnitBankListVM unitBankVM,string employeeId)
    {
        try
        {
            BankUnitMasterDTO inputDTO = new BankUnitMasterDTO();
            if (unitBankVM.BankMasterList.Count >0)
            {
                foreach( var bank in unitBankVM.BankMasterList)
                {
                    BankMasterDTO bankDetails = _mapper.Map<BankMasterDTO>(_unitOfWork.BankMaster.GetAll(null, null, null).Result.Where(a => a.BankId == bank.BankId).FirstOrDefault());
                    inputDTO.UnitId = bank.UnitId;
                    inputDTO.BankName = bankDetails.BankName;
                    inputDTO.IsActive = true;
                    inputDTO.CreatedOn = DateTime.Now;
                    inputDTO.CreatedBy = Convert.ToInt32(employeeId);

                    if (ModelState.IsValid)
                    {
                        Expression<Func<BankUnitMaster, bool>> expression = a => a.BankName.Trim().Replace(" ", "") == inputDTO.BankName.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                        if (!_unitOfWork.BankUnitMaster.Exists(expression))
                        {
                            //var outputDTO = _mapper.Map<BankMaster>(inputDTO);
                            _unitOfWork.BankUnitMaster.AddAsync(_mapper.Map<BankUnitMaster>(inputDTO));
                            _unitOfWork.Save();
                           // return Ok("Success");
                        }
                       // else
                            //return BadRequest("Duplicate Bank Name found");
                    }

                }

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving bank {nameof(SaveBankMaster)}");
            throw;
        }
    }




}
