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
public class ReligionMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReligionMasterController> _logger;
    private readonly IMapper _mapper;

    public ReligionMasterController(IUnitOfWork unitOfWork, ILogger<ReligionMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetReligion(ReligionMasterDTO inputDTO)
    {
        try
        {
            ReligionMasterDTO outputDTO = _mapper.Map<ReligionMasterDTO>(await _unitOfWork.ReligionMaster.GetByIdAsync(inputDTO.ReligionId));
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
            _logger.LogError(ex, $"Error in retriving Religion {nameof(GetReligion)}");
            throw;
        }
    }

    [HttpPost(Name = "GetReligions")]
    public async Task<IActionResult> GetReligions(Core.Helper.RequestParams requestParams)
    {
        try
        {
            IList<ReligionMasterDTO> outputModel = new List<ReligionMasterDTO>();
            outputModel = _mapper.Map<IList<ReligionMasterDTO>>(await _unitOfWork.ReligionMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Religions {nameof(GetReligions)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveReligion(ReligionMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<ReligionMaster, bool>> expression = a => a.ReligionName.Trim().Replace(" ","") == inputDTO.ReligionName.Trim().Replace(" ", "") && a.IsActive==true;
                if (!_unitOfWork.ReligionMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<ReligionMaster>(inputDTO);
                    _unitOfWork.ReligionMaster.AddAsync(_mapper.Map<ReligionMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Religion {nameof(SaveReligion)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateReligion(ReligionMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<ReligionMaster, bool>> expression = a => a.ReligionName.Trim().Replace(" ", "") == inputDTO.ReligionName.Trim().Replace(" ", "") && a.ReligionId != inputDTO.ReligionId && a.IsActive == true;
                if (!_unitOfWork.ReligionMaster.Exists(expression))
                {
                    _unitOfWork.ReligionMaster.Update(_mapper.Map<ReligionMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in Religion updates {nameof(UpdateReligion)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReligion(ReligionMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ReligionMaster outputMaster = _mapper.Map<ReligionMaster>(await _unitOfWork.ReligionMaster.GetByIdAsync(inputDTO.ReligionId));
                outputMaster.IsActive = false;
                _unitOfWork.ReligionMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Religion {nameof(DeleteReligion)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<ReligionKeyValues> GetReligionKeyValue()
    {
        return (_unitOfWork.ReligionMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new ReligionKeyValues()
                           {
                               ReligionId = p.ReligionId,
                               ReligionName = p.ReligionName
                           })).ToList();
    }

   


}
