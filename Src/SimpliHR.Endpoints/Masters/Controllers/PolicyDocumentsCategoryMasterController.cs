using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Masters;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class PolicyDocumentsCategoryMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PolicyDocumentsCategoryMasterController> _logger;
    private readonly IMapper _mapper;

    public PolicyDocumentsCategoryMasterController(IUnitOfWork unitOfWork, ILogger<PolicyDocumentsCategoryMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> GetPolicyDocumentsCategory(PolicyDocumentsCategoryMasterDTO inputDTO)
    {
        try
        {
            PolicyDocumentsCategoryMasterDTO outputDTO = _mapper.Map<PolicyDocumentsCategoryMasterDTO>(await _unitOfWork.PolicyDocumentsCategoryMaster.GetByIdAsync(inputDTO.PolicyDocumentsCategoryId));
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
            _logger.LogError(ex, $"Error in retriving Academic {nameof(GetPolicyDocumentsCategory)}");
            throw;
        }
    }
    [HttpPost(Name = "GetPolicyDocumentsCategories")]
    public async Task<IActionResult> GetPolicyDocumentsCategories(Core.Helper.RequestParams requestParams, int? unitId)
    {
        try
        {
            IList<PolicyDocumentsCategoryMasterDTO> outputModel = new List<PolicyDocumentsCategoryMasterDTO>();
            outputModel = _mapper.Map<IList<PolicyDocumentsCategoryMasterDTO>>(await _unitOfWork.PolicyDocumentsCategoryMaster.GetPagedListWithExpression(requestParams, p => p.UnitId == unitId && p.IsActive == true));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetPolicyDocumentsCategories)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> SavePolicyDocumentsCategory(PolicyDocumentsCategoryMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<PolicyDocumentsCategoryMaster, bool>> expression = a => a.PolicyDocumentsCategory.Trim().Replace(" ", "") == inputDTO.PolicyDocumentsCategory.Trim().Replace(" ", "") && a.IsActive == true && a.UnitId == inputDTO.UnitId;
                if (!_unitOfWork.PolicyDocumentsCategoryMaster.Exists(expression))
                {
                    //var outputDTO = _mapper.Map<AcademicMaster>(inputDTO);
                    _unitOfWork.PolicyDocumentsCategoryMaster.AddAsync(_mapper.Map<PolicyDocumentsCategoryMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Academic {nameof(SavePolicyDocumentsCategory)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdatePolicyDocumentsCategory(PolicyDocumentsCategoryMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<PolicyDocumentsCategoryMaster, bool>> expression = a => a.PolicyDocumentsCategory.Trim().Replace(" ", "") == inputDTO.PolicyDocumentsCategory.Trim().Replace(" ", "") && a.PolicyDocumentsCategoryId != inputDTO.PolicyDocumentsCategoryId && a.IsActive == true && a.UnitId==inputDTO.UnitId;
                if (!_unitOfWork.PolicyDocumentsCategoryMaster.Exists(expression))
                {
                    _unitOfWork.PolicyDocumentsCategoryMaster.Update(_mapper.Map<PolicyDocumentsCategoryMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in Academic updates {nameof(UpdatePolicyDocumentsCategory)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeletePolicyDocumentsCategory(PolicyDocumentsCategoryMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                PolicyDocumentsCategoryMaster outputMaster = _mapper.Map<PolicyDocumentsCategoryMaster>(await _unitOfWork.PolicyDocumentsCategoryMaster.GetByIdAsync(inputDTO.PolicyDocumentsCategoryId));
                outputMaster.IsActive = false;
                _unitOfWork.PolicyDocumentsCategoryMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Academic {nameof(DeletePolicyDocumentsCategory)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<PolicyDocumentCategoryKeyValues> GetPolicyDocumentsCategoryKeyValue(int? UnitId)
    {
        return (_unitOfWork.PolicyDocumentsCategoryMaster.GetAll(p => p.UnitId == UnitId && p.IsActive == true).Result
                           .Select(p => new PolicyDocumentCategoryKeyValues()
                           {
                               PolicyDocumentsCategoryId = p.PolicyDocumentsCategoryId,
                               PolicyDocumentsCategory = p.PolicyDocumentsCategory
                           })).ToList();
    }

}

