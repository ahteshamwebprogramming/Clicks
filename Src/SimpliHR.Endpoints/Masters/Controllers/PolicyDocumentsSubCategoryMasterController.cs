using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Masters;
using System.Data;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class PolicyDocumentsSubCategoryMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PolicyDocumentsSubCategoryMasterController> _logger;
    private readonly IMapper _mapper;

    public PolicyDocumentsSubCategoryMasterController(IUnitOfWork unitOfWork, ILogger<PolicyDocumentsSubCategoryMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> GetPolicyDocumentsSubCategory(PolicyDocumentsSubCategoryMasterDTO inputDTO)
    {
        try
        {
            PolicyDocumentsSubCategoryMasterDTO outputDTO = _mapper.Map<PolicyDocumentsSubCategoryMasterDTO>(await _unitOfWork.PolicyDocumentsSubCategoryMaster.GetByIdAsync(inputDTO.PolicyDocumentsSubCategoryId));
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
            _logger.LogError(ex, $"Error in retriving Academic {nameof(GetPolicyDocumentsSubCategory)}");
            throw;
        }

    }
    [HttpPost(Name = "GetPolicyDocumentsSubCategories")]
    public async Task<IActionResult> GetPolicyDocumentsSubCategories(Core.Helper.RequestParams requestParams, int? unitId)
    {
        try
        {
            var returnData = _mapper.Map<IList<PolicyDocumentsSubCategoryMasterDTO>>(await _unitOfWork.PolicyDocumentsSubCategoryMaster.GetAll(requestParams: requestParams,
                                                                                          expression: (p => p.UnitId == unitId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                          orderBy: (m => m.OrderBy(x => x.PolicyDocumentsSubCategory))));
            IList<PolicyDocumentsSubCategoryMasterDTO> outputModel = new List<PolicyDocumentsSubCategoryMasterDTO>();

            outputModel = returnData.Select(r => new PolicyDocumentsSubCategoryMasterDTO
            {
                PolicyDocumentsSubCategoryId = r.PolicyDocumentsSubCategoryId,
                PolicyDocumentsSubCategory = r.PolicyDocumentsSubCategory,
                PolicyDocumentsCategoryId = r.PolicyDocumentsCategory.PolicyDocumentsCategoryId,
                PolicyDocumentsCategoryName = r.PolicyDocumentsCategory.PolicyDocumentsCategory,
                IsActive = r.IsActive
            }).ToList();
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving countries {nameof(GetPolicyDocumentsSubCategories)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SavePolicyDocumentsSubCategory(PolicyDocumentsSubCategoryMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<PolicyDocumentsSubCategoryMaster, bool>> expression = a => a.PolicyDocumentsSubCategory.Trim().Replace(" ", "") == inputDTO.PolicyDocumentsSubCategory.Trim().Replace(" ", "") && a.PolicyDocumentsCategoryId == inputDTO.PolicyDocumentsCategoryId && a.IsActive == true && a.UnitId == inputDTO.UnitId;
                if (!_unitOfWork.PolicyDocumentsSubCategoryMaster.Exists(expression))
                {
                    var subCategoryViewModel = _mapper.Map<PolicyDocumentsSubCategoryMaster>(inputDTO);
                    _unitOfWork.PolicyDocumentsSubCategoryMaster.AddAsync(subCategoryViewModel);
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
            _logger.LogError(ex, $"Error in saving country {nameof(SavePolicyDocumentsSubCategory)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdatePolicyDocumentsSubCategory(PolicyDocumentsSubCategoryMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<PolicyDocumentsSubCategoryMaster, bool>> expression = a => a.PolicyDocumentsSubCategory.Trim().Replace(" ", "") == inputDTO.PolicyDocumentsSubCategory.Trim().Replace(" ", "") && a.PolicyDocumentsSubCategoryId != inputDTO.PolicyDocumentsSubCategoryId && a.IsActive == true && a.UnitId == inputDTO.UnitId;
                if (!_unitOfWork.PolicyDocumentsSubCategoryMaster.Exists(expression))
                {
                    var subCategoryViewModel = _mapper.Map<PolicyDocumentsSubCategoryMaster>(inputDTO);
                    _unitOfWork.PolicyDocumentsSubCategoryMaster.Update(subCategoryViewModel);
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
            _logger.LogError(ex, $"Error in country updates {nameof(UpdatePolicyDocumentsSubCategory)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeletePolicyDocumentsSubCategory(PolicyDocumentsSubCategoryMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                PolicyDocumentsSubCategoryMaster subCategoryMaster = _mapper.Map<PolicyDocumentsSubCategoryMaster>(await _unitOfWork.PolicyDocumentsSubCategoryMaster.GetByIdAsync(inputDTO.PolicyDocumentsSubCategoryId));
                subCategoryMaster.IsActive = false;
                _unitOfWork.PolicyDocumentsSubCategoryMaster.Update(subCategoryMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeletePolicyDocumentsSubCategory)}");
            throw;
        }
    }

    [HttpGet]
    public IEnumerable<PolicyDocumentSubCategoryKeyValues> GetPolicyDocumentsSubCategoryKeyValue(int policyDocumentsCategoryId = 0)
    {
        if (policyDocumentsCategoryId == 0)
            return (_unitOfWork.PolicyDocumentsSubCategoryMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new PolicyDocumentSubCategoryKeyValues()
                               {
                                   PolicyDocumentsSubCategoryId = p.PolicyDocumentsSubCategoryId,
                                   PolicyDocumentsSubCategory = p.PolicyDocumentsSubCategory
                               })).ToList();
        else
            return (_unitOfWork.PolicyDocumentsSubCategoryMaster.GetAll(p => p.IsActive == true && p.PolicyDocumentsCategoryId == policyDocumentsCategoryId).Result
                          .Select(p => new PolicyDocumentSubCategoryKeyValues()
                          {
                              PolicyDocumentsSubCategoryId = p.PolicyDocumentsSubCategoryId,
                              PolicyDocumentsSubCategory = p.PolicyDocumentsSubCategory
                          })).ToList();
    }
    public List<PolicyDocumentsSubCategoryMasterDTO> GetPolicyDocumentsSubCategory_PolicyDocumentsCategory(List<PolicyDocumentsSubCategoryMasterDTO> inputData)
    {
        var data = (_unitOfWork.PolicyDocumentsCategoryMaster.GetAll(p => p.IsActive == true)).Result.ToList();
        List<PolicyDocumentsSubCategoryMasterDTO> outputData = inputData;

        foreach (var item in inputData)
        {
            item.PolicyDocumentsCategoryName = data.Where(x => x.PolicyDocumentsCategoryId == item.PolicyDocumentsCategoryId).Select(p => p.PolicyDocumentsCategory).First();
        }
        return outputData;
    }



}

