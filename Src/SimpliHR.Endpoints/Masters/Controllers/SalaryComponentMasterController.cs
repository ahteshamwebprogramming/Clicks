using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System.Linq.Expressions;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using SimpliHR.Services.DBContext;
using System.Text.RegularExpressions;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Core.Helper;
using SimpliHR.Infrastructure.Models.Payroll;

namespace SimpliHR.Endpoints.Masters;

[Route("api/[controller]/[action]")]
[ApiController]
public class SalaryComponentMasterController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SalaryComponentMasterController> _logger;
    private readonly IMapper _mapper;

    public SalaryComponentMasterController(IUnitOfWork unitOfWork, ILogger<SalaryComponentMasterController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetSalaryComponent(SalaryComponentMasterDTO inputDTO)
    {
        try
        {
            SalaryComponentMasterDTO outputDTO = _mapper.Map<SalaryComponentMasterDTO>(await _unitOfWork.SalaryComponentMaster.GetByIdAsync(inputDTO.SalaryComponentId));
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
            _logger.LogError(ex, $"Error in retriving Salary Component {nameof(GetSalaryComponent)}");
            throw;
        }
    }

    [HttpPost(Name = "GetSalaryComponents")]
    public async Task<IActionResult> GetSalaryComponents(Core.Helper.RequestParams requestParams, int? unitId)
    {
        try
        {
            IList<SalaryComponentMasterDTO> outputModel = new List<SalaryComponentMasterDTO>();
            outputModel = _mapper.Map<IList<SalaryComponentMasterDTO>>(await _unitOfWork.SalaryComponentMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true && p.UnitId == unitId));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Salary Components {nameof(GetSalaryComponents)}");
            throw;
        }
    }

    [HttpPost(Name = "GetTaxSlabs")]
    public async Task<IActionResult> GetTaxSlabs(Core.Helper.RequestParams requestParams, int? unitId,int ageGroupId)
    {
        try
        {
            IList<TaxSlabDetailsDTO> outputModel = new List<TaxSlabDetailsDTO>();
            outputModel = _mapper.Map<IList<TaxSlabDetailsDTO>>(await _unitOfWork.ITaxSlabDetail.GetPagedListWithExpression(requestParams, p => p.IsActive == true && p.AgeGroupId == ageGroupId));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Tax Slabs {nameof(GetTaxSlabs)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveTaxSlab(TaxSlabDetailsDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                Expression<Func<TaxSlabDetails, bool>> expression = a => (a.AmtFrom == inputDTO.AmtFrom && a.AmtTo == inputDTO.AmtTo) && a.Regime== inputDTO.Regime && a.IsActive == true ;
                if (!_unitOfWork.ITaxSlabDetail.Exists(expression))
                {
                    _unitOfWork.ITaxSlabDetail.AddAsync(_mapper.Map<TaxSlabDetails>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving tax slab {nameof(SaveTaxSlab)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult UpdateTaxSlab(TaxSlabDetailsDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<SalaryComponentMaster, bool>> expression = a => (a.SalaryComponentTitle.Trim().Replace(" ", "") == inputDTO.SalaryComponentTitle.Trim().Replace(" ", "") || a.SalaryComponentDisapyOrder == inputDTO.SalaryComponentDisapyOrder) && a.SalaryComponentId != inputDTO.SalaryComponentId && a.IsActive == true;
                //if (!_unitOfWork.SalaryComponentMaster.Exists(expression))
                //{
                _unitOfWork.ITaxSlabDetail.Update(_mapper.Map<TaxSlabDetails>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
                //}
                //else
                //    return BadRequest("Duplicate Salary Component/Order found");

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Salary Component updates {nameof(UpdateSalaryComponent)}");
            throw;
        }
    }



    [HttpPost]
    public IActionResult SaveSalaryComponent(SalaryComponentMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
             
                Expression<Func<SalaryComponentMaster, bool>> expression = a => (a.SalaryComponentTitle.Trim().Replace(" ","") == inputDTO.SalaryComponentTitle.Trim().Replace(" ", "") && a.SalaryComponentType == inputDTO.SalaryComponentType) && a.IsActive==true && a.UnitId == inputDTO.UnitId;
                if (!_unitOfWork.SalaryComponentMaster.Exists(expression))
                {
                    _unitOfWork.SalaryComponentMaster.AddAsync(_mapper.Map<SalaryComponentMaster>(inputDTO));
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
            _logger.LogError(ex, $"Error in saving Salary Component {nameof(SaveSalaryComponent)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateSalaryComponent(SalaryComponentMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //Expression<Func<SalaryComponentMaster, bool>> expression = a => (a.SalaryComponentTitle.Trim().Replace(" ", "") == inputDTO.SalaryComponentTitle.Trim().Replace(" ", "") || a.SalaryComponentDisapyOrder == inputDTO.SalaryComponentDisapyOrder) && a.SalaryComponentId != inputDTO.SalaryComponentId && a.IsActive == true;
                //if (!_unitOfWork.SalaryComponentMaster.Exists(expression))
                //{
                    _unitOfWork.SalaryComponentMaster.Update(_mapper.Map<SalaryComponentMaster>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                //}
                //else
                //    return BadRequest("Duplicate Salary Component/Order found");

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Salary Component updates {nameof(UpdateSalaryComponent)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSalaryComponent(SalaryComponentMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                SalaryComponentMaster outputMaster = _mapper.Map<SalaryComponentMaster>(await _unitOfWork.SalaryComponentMaster.GetByIdAsync(inputDTO.SalaryComponentId));
                outputMaster.IsActive = false;
                _unitOfWork.SalaryComponentMaster.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting Salary Component {nameof(DeleteSalaryComponent)}");
            throw;
        }
    }

    [HttpPost]
    public IEnumerable<SalaryComponentKeyValues> GetSalaryComponentKeyValue()
    {
        return (_unitOfWork.SalaryComponentMaster.GetAll(p => p.IsActive == true).Result
                           .Select(p => new SalaryComponentKeyValues()
                           {
                               SalaryComponentId = p.SalaryComponentId,
                               SalaryComponentTitle = p.SalaryComponentTitle
                           })).ToList();
    }


    [HttpPost]
    public async Task<IActionResult> GetTaxSlab(TaxSlabDetailsDTO inputDTO)
    {
        try
        {
            TaxSlabDetailsDTO outputDTO = _mapper.Map<TaxSlabDetailsDTO>(await _unitOfWork.ITaxSlabDetail.GetByIdAsync(inputDTO.SlabID));
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
            _logger.LogError(ex, $"Error in retriving tax slabs {nameof(GetTaxSlab)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteTaxSlab(TaxSlabDetailsDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                TaxSlabDetails outputMaster = _mapper.Map<TaxSlabDetails>(await _unitOfWork.ITaxSlabDetail.GetByIdAsync(inputDTO.SlabID));
                outputMaster.IsActive = false;
                _unitOfWork.ITaxSlabDetail.Update(outputMaster);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting tax slab {nameof(DeleteTaxSlab)}");
            throw;
        }
    }
}
