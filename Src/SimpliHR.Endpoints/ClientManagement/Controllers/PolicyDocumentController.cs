using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Masters;
using System.Linq.Expressions;
using System.Net;

namespace SimpliHR.Endpoints.ClientManagement;

[Route("api/[controller]")]
[ApiController]
public class PolicyDocumentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PolicyDocumentController> _logger;
    private readonly IMapper _mapper;
    public PolicyDocumentController(IUnitOfWork unitOfWork, ILogger<PolicyDocumentController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> PolicyDocument(PolicyDocumentsMasterDTO inputDTO)
    {
        try
        {
            PolicyDocumentsMasterDTO outputDTO = _mapper.Map<PolicyDocumentsMasterDTO>(await _unitOfWork.PolicyDocumentsMaster.GetByIdAsync(inputDTO.PolicyDocumentsMasterId));
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
            _logger.LogError(ex, $"Error in retriving Client {nameof(PolicyDocument)}");
            throw;
        }
    }

    [HttpPost]
    public bool PolicyDocumentExists(PolicyDocumentsMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<PolicyDocumentsMaster, bool>> expression = a => a.PolicyDocumentsCategoryId == inputDTO.PolicyDocumentsCategoryId && a.PolicyDocumentsSubCategoryId == inputDTO.PolicyDocumentsSubCategoryId && a.PolicyDocument == inputDTO.PolicyDocument && a.ClientId == inputDTO.ClientId && a.UnitId == inputDTO.UnitId && a.IsActive == true;

                if (!_unitOfWork.PolicyDocumentsMaster.Exists(expression))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving client {nameof(SavePolicyDocumentDetails)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SavePolicyDocumentDetails(PolicyDocumentsMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<PolicyDocumentsMaster, bool>> expression = a => a.PolicyDocumentsCategoryId == inputDTO.PolicyDocumentsCategoryId && a.PolicyDocumentsSubCategoryId == inputDTO.PolicyDocumentsSubCategoryId && a.PolicyDocument == inputDTO.PolicyDocument && a.ClientId == inputDTO.ClientId && a.UnitId == inputDTO.UnitId && a.IsActive == true;

                if (!_unitOfWork.PolicyDocumentsMaster.Exists(expression))
                {
                    _unitOfWork.PolicyDocumentsMaster.AddAsync(_mapper.Map<PolicyDocumentsMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                {
                    return BadRequest("Duplicate Entry Found");
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving client {nameof(SavePolicyDocumentDetails)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeletePolicyDocument(PolicyDocumentsMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                PolicyDocumentsMaster outputMaster = _mapper.Map<PolicyDocumentsMaster>(await _unitOfWork.PolicyDocumentsMaster.GetByIdAsync(inputDTO.PolicyDocumentsMasterId));
                outputMaster.IsActive = false;
                _unitOfWork.PolicyDocumentsMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Academic {nameof(DeletePolicyDocument)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetPolicyDocumentByID(int inputDTO)
    {
        try
        {
            PolicyDocumentsMasterDTO outputDTO = _mapper.Map<PolicyDocumentsMasterDTO>(await _unitOfWork.PolicyDocumentsMaster.GetByIdAsync(inputDTO));
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
            _logger.LogError(ex, $"Error in retriving Get client by ID {nameof(GetPolicyDocumentByID)}");
            throw;
        }
    }

    [HttpPost(Name = "GetPolicyDocuments")]
    public async Task<IActionResult> GetPolicyDocuments(Core.Helper.RequestParams requestParams, int? clientId = null, int? unitId = null)
    {
        try
        {
            IList<PolicyDocumentsMasterDTO> outputModel = new List<PolicyDocumentsMasterDTO>();
            if (clientId != null)
                outputModel = _mapper.Map<IList<PolicyDocumentsMasterDTO>>(await _unitOfWork.PolicyDocumentsMaster.GetAll(p => p.IsActive == true && p.ClientId == clientId && p.UnitId == unitId, null));
            else
                outputModel = _mapper.Map<IList<PolicyDocumentsMasterDTO>>(await _unitOfWork.PolicyDocumentsMaster.GetAll(p => p.IsActive == true, null));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employees {nameof(GetPolicyDocuments)}");
            throw;
        }
    }

    [HttpGet]
    public async Task<List<PolicyDocumentCategoryKeyValues>>? PolicyDocumentCategoryKeyValues(bool? isActive = true, int? unitId = null)
    {
        return (await _unitOfWork.PolicyDocumentsCategoryMaster.GetAll(p =>p.UnitId== unitId && ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
                       .Select(p => new PolicyDocumentCategoryKeyValues()
                       {
                           PolicyDocumentsCategoryId = p.PolicyDocumentsCategoryId,
                           PolicyDocumentsCategory = p.PolicyDocumentsCategory
                       }).ToList();
    }
    [HttpGet]
    public async Task<List<PolicyDocumentSubCategoryKeyValues>>? PolicyDocumentSubCategoryKeyValues(bool? isActive = true, int? categoryId = null)
    {
        return (await _unitOfWork.PolicyDocumentsSubCategoryMaster.GetAll(p => p.PolicyDocumentsCategoryId == categoryId && ((isActive != null ? p.IsActive == isActive : true))).ConfigureAwait(true))
                       .Select(p => new PolicyDocumentSubCategoryKeyValues()
                       {
                           PolicyDocumentsSubCategoryId = p.PolicyDocumentsSubCategoryId,
                           PolicyDocumentsSubCategory = p.PolicyDocumentsSubCategory
                       }).ToList();
    }

}
