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
public class IdtypeMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IdtypeMasterController> _logger;
    private readonly IMapper _mapper;

    public IdtypeMasterController(IUnitOfWork unitOfWork, ILogger<IdtypeMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetIdtype(IdtypeMasterDTO inputDTO)
    {
        try
        {
            IdtypeMasterDTO outputDTO = _mapper.Map<IdtypeMasterDTO>(await _unitOfWork.IdtypeMaster.GetByIdAsync(inputDTO.IdentityId));
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
            _logger.LogError(ex, $"Error in retriving Idtype {nameof(GetIdtype)}");
            throw;
        }
    }

    [HttpPost(Name = "GetIdtypes")]
    public async Task<IActionResult> GetIdtypes(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<IdtypeMasterDTO> outputModel = new List<IdtypeMasterDTO>();
            outputModel = _mapper.Map<IList<IdtypeMasterDTO>>(await _unitOfWork.IdtypeMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Identity {nameof(GetIdtypes)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveIdtype(IdtypeMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<IdtypeMaster, bool>> expression = a => a.IdentityName.Trim().Replace(" ", "") == inputDTO.IdentityName.Trim().Replace(" ", "") && a.IsActive == true;
                if (!_unitOfWork.IdtypeMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<IdtypeMaster>(inputDTO);
                    _unitOfWork.IdtypeMaster.AddAsync(_mapper.Map<IdtypeMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Idtype {nameof(SaveIdtype)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateIdtype(IdtypeMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<IdtypeMaster, bool>> expression = a => a.IdentityName.Trim().Replace(" ", "") == inputDTO.IdentityName.Trim().Replace(" ", "") && a.IdentityId != inputDTO.IdentityId && a.IsActive == true;
                if (!_unitOfWork.IdtypeMaster.Exists(expression))
                {
                    _unitOfWork.IdtypeMaster.Update(_mapper.Map<IdtypeMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in Idtype updates {nameof(UpdateIdtype)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteIdtype(IdtypeMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                IdtypeMaster outputMaster = _mapper.Map<IdtypeMaster>(await _unitOfWork.IdtypeMaster.GetByIdAsync(inputDTO.IdentityId));
                outputMaster.IsActive = false;
                _unitOfWork.IdtypeMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Idtype {nameof(DeleteIdtype)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<IdtypeKeyValues> GetIdtypeKeyValue()
    {
        return (_unitOfWork.IdtypeMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new IdtypeKeyValues()
                           {
                               IdentityId = p.IdentityId,
                               IdentityName = p.IdentityName
                           })).ToList();
    }




}
