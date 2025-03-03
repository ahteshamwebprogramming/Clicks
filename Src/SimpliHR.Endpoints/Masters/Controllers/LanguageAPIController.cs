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
using static SimpliHR.Infrastructure.Models.Masters.LanguageUnitMasterDTO;


namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class LanguageAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LanguageAPIController> _logger;
    private readonly IMapper _mapper;

    public LanguageAPIController(IUnitOfWork unitOfWork, ILogger<LanguageAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetLanguageMaster(LanguageMasterDTO inputDTO)
    {
        try
        {
            LanguageMasterDTO outputDTO = _mapper.Map<LanguageMasterDTO>(await _unitOfWork.LanguageMaster.GetByIdAsync(inputDTO.LanguageId));
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetLanguageMaster)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetUnitLanguageMaster(LanguageUnitMasterDTO inputDTO)
    {
        try
        {
            LanguageUnitMasterDTO outputDTO = _mapper.Map<LanguageUnitMasterDTO>(await _unitOfWork.LanguageUnitMaster.GetByIdAsync(inputDTO.LanguageId));
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
            _logger.LogError(ex, $"Error in retriving Language {nameof(GetUnitLanguageMaster)}");
            throw;
        }
    }

    [HttpPost(Name = "GetUnitLanguageList")]
    public async Task<IActionResult> GetUnitLanguageList(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<LanguageUnitMasterDTO> outputModel = new List<LanguageUnitMasterDTO>();
            outputModel = _mapper.Map<IList<LanguageUnitMasterDTO>>(await _unitOfWork.LanguageUnitMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving GetUnitLanguageList {nameof(GetUnitLanguageList)}");
            throw;
        }
    }


    [HttpPost(Name = "GetLanguageMasterList")]
    public async Task<IActionResult> GetLanguageMasterList(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<LanguageMasterDTO> outputModel = new List<LanguageMasterDTO>();
            outputModel = _mapper.Map<IList<LanguageMasterDTO>>(await _unitOfWork.LanguageMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving GetLanguageMasterList {nameof(GetLanguageMasterList)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveUnitLanguage(LanguageUnitMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<LanguageUnitMaster, bool>> expression = a => a.Language.Trim().Replace(" ","") == inputDTO.Language.Trim().Replace(" ", "") && a.UnitId==inputDTO.UnitId && a.IsActive==true;
                if (!_unitOfWork.LanguageUnitMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<BankMaster>(inputDTO);
                    _unitOfWork.LanguageUnitMaster.AddAsync(_mapper.Map<LanguageUnitMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving bank {nameof(SaveUnitLanguage)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult SaveLanguageMaster(LanguageMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<LanguageMaster, bool>> expression = a => a.Language.Trim().Replace(" ", "") == inputDTO.Language.Trim().Replace(" ", "") && a.IsActive == true;
                if (!_unitOfWork.LanguageMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<BankMaster>(inputDTO);
                    _unitOfWork.LanguageMaster.AddAsync(_mapper.Map<LanguageMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving language {nameof(SaveLanguageMaster)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateLanguageMaster(LanguageMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<LanguageMaster, bool>> expression = a => a.Language.Trim().Replace(" ", "") == inputDTO.Language.Trim().Replace(" ", "") && a.LanguageId != inputDTO.LanguageId && a.IsActive == true;
                if (!_unitOfWork.LanguageMaster.Exists(expression))
                {
                    _unitOfWork.LanguageMaster.Update(_mapper.Map<LanguageMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in language updates {nameof(UpdateLanguageMaster)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult UpdateUnitLanguage(LanguageUnitMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<LanguageUnitMaster, bool>> expression = a => a.Language.Trim().Replace(" ", "") == inputDTO.Language.Trim().Replace(" ", "") && a.LanguageId != inputDTO.LanguageId && a.IsActive == true;
                if (!_unitOfWork.LanguageUnitMaster.Exists(expression))
                {
                    _unitOfWork.LanguageUnitMaster.Update(_mapper.Map<LanguageUnitMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in bank updates {nameof(UpdateUnitLanguage)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUnitLanguage(LanguageUnitMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                LanguageUnitMaster outputMaster = _mapper.Map<LanguageUnitMaster>(await _unitOfWork.LanguageUnitMaster.GetByIdAsync(inputDTO.LanguageId)) ;
                outputMaster.IsActive = false;
                _unitOfWork.LanguageUnitMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting bank {nameof(DeleteUnitLanguage)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLanguageMaster(LanguageMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                LanguageMaster outputMaster = _mapper.Map<LanguageMaster>(await _unitOfWork.LanguageMaster.GetByIdAsync(inputDTO.LanguageId)) ;
                outputMaster.IsActive = false;
                _unitOfWork.LanguageMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting language {nameof(DeleteLanguageMaster)}");
            throw;
        }
    }

    //[HttpPost]
    //public IEnumerable<BankKeyValues> GetBankKeyValue()
    //{
    //    return (_unitOfWork.BankMaster.GetAll(p => p.IsActive == true).Result
    //                       .Select(p => new BankKeyValues()
    //                       {
    //                           BankId = p.BankId,
    //                           BankName = p.BankName
    //                       })).ToList();
    //}

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
    public IActionResult SaveUnitLanguageFromMaster(UnitLanguageListVM unitLanguageVM,string employeeId)
    {
        try
        {
            LanguageUnitMasterDTO inputDTO = new LanguageUnitMasterDTO();
            if (unitLanguageVM.UnitLanguageList.Count >0)
            {
                foreach( var lang in unitLanguageVM.UnitLanguageList)
                {
                    LanguageMasterDTO languageDetails = _mapper.Map<LanguageMasterDTO>(_unitOfWork.LanguageMaster.GetAll(null, null, null).Result.Where(a => a.LanguageId == lang.LanguageId).FirstOrDefault());
                    inputDTO.UnitId = lang.UnitId;
                    inputDTO.Language = languageDetails.Language;
                    inputDTO.LanguageParentId = lang.LanguageId;
                    inputDTO.IsActive = true;
                    inputDTO.CreatedOn = DateTime.Now;
                    inputDTO.CreatedBy = Convert.ToInt32(employeeId);
                    inputDTO.ModifiedOn = DateTime.Now;
                    inputDTO.ModifiedBy = Convert.ToInt32(employeeId);

                    if (ModelState.IsValid)
                    {
                        Expression<Func<LanguageUnitMaster, bool>> expression = a => a.Language.Trim().Replace(" ", "") == inputDTO.Language.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                        if (!_unitOfWork.LanguageUnitMaster.Exists(expression))
                        {
                            //var outputDTO = _mapper.Map<BankMaster>(inputDTO);
                            _unitOfWork.LanguageUnitMaster.AddAsync(_mapper.Map<LanguageUnitMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving language {nameof(SaveUnitLanguageFromMaster)}");
            throw;
        }
    }




}
