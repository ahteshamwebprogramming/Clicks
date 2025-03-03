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
using Microsoft.AspNetCore.Components.Forms;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class JobTitleMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JobTitleMasterController> _logger;
    private readonly IMapper _mapper;

    public JobTitleMasterController(IUnitOfWork unitOfWork, ILogger<JobTitleMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetJobTitle(JobTitleMasterDTO inputDTO)
    {
        try
        {
            JobTitleMasterDTO outputDTO = _mapper.Map<JobTitleMasterDTO>(await _unitOfWork.JobTitleMaster.GetByIdAsync(inputDTO.JobTitleId));

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
            _logger.LogError(ex, $"Error in retriving JobTitle {nameof(GetJobTitle)}");
            throw;
        }
    }

    [HttpPost(Name = "GetJobTitles")]
    public async Task<IActionResult> GetJobTitles(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            IList<JobTitleMasterDTO> outputModel = new List<JobTitleMasterDTO>();
            outputModel = _mapper.Map<IList<JobTitleMasterDTO>>(await _unitOfWork.JobTitleMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == UnitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving JobTitles {nameof(GetJobTitles)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveJobTitle(JobTitleMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<JobTitleMaster, bool>> expression = a => a.JobTitle.Trim().Replace(" ", "") == inputDTO.JobTitle.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                if (!_unitOfWork.JobTitleMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<JobTitleMaster>(inputDTO);
                    _unitOfWork.JobTitleMaster.AddAsync(_mapper.Map<JobTitleMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving JobTitle {nameof(SaveJobTitle)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateJobTitle(JobTitleMasterDTO inputDTO)
    {
        try
        {

            //  inputDTO.IsActive = true;
            // inputDTO.UnitId = unitId;
            if (ModelState.IsValid)
            {
                Expression<Func<JobTitleMaster, bool>> expression = a => a.JobTitle.Trim().Replace(" ", "") == inputDTO.JobTitle.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.JobTitleId != inputDTO.JobTitleId && a.IsActive == true;
                if (!_unitOfWork.JobTitleMaster.Exists(expression))
                {
                    _unitOfWork.JobTitleMaster.Update(_mapper.Map<JobTitleMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in JobTitle updates {nameof(UpdateJobTitle)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteJobTitle(JobTitleMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                JobTitleMaster outputMaster = _mapper.Map<JobTitleMaster>(await _unitOfWork.JobTitleMaster.GetByIdAsync(inputDTO.JobTitleId));
                outputMaster.IsActive = false;
                _unitOfWork.JobTitleMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting JobTitle {nameof(DeleteJobTitle)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<JobTitleKeyValues> GetJobTitleKeyValue()
    {
        return (_unitOfWork.JobTitleMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new JobTitleKeyValues()
                           {
                               JobTitleId = p.JobTitleId,
                               JobTitle = p.JobTitle
                           })).ToList();
    }

}
