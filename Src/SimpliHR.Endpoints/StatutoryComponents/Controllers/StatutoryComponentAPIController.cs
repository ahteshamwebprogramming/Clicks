using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SimpliHR.Services.DBContext;
using SimpliHR.Infrastructure.Models.StatutoryComponent;
using Microsoft.AspNetCore.Components.Forms;
using SimpliHR.Core.Entities;
using System.Linq.Expressions;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Collections.Generic;
using SimpliHR.Infrastructure.Helper;
using System.Net;
using SimpliHR.Infrastructure.Models.Exit;

namespace SimpliHR.Endpoints.StatutoryComponent;

[Route("api/[controller]")]
[ApiController]
public class StatutoryComponentAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatutoryComponentAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _simpliDbContext;
    public StatutoryComponentAPIController(IUnitOfWork unitOfWork, ILogger<StatutoryComponentAPIController> logger, IMapper mapper, SimpliDbContext SimpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = SimpliDbContext;
    }
    [HttpPost]
    public async Task<IActionResult> GetStatutoryComponent(int UnitId)
    {
        try
        {
            StatutoryComponent_EPFDTO outputDTO = _mapper.Map<StatutoryComponent_EPFDTO>(_unitOfWork.StatutoryComponent_EPF.GetAll(x => x.UnitId == UnitId).Result.FirstOrDefault());
            if (outputDTO != null)
            {
                List<EpfemployeeMapping> listEpfEmpMap = _unitOfWork.EPFEmployeeMapping.GetAll(x => x.StatutoryComponentsId == outputDTO.StatutoryComponentsId).Result.ToList();
                outputDTO.MappingEmployeeIds = String.Join(",", listEpfEmpMap.Select(x => x.EmployeeId));
            }
            //else
            //    outputDTO.DisplayMessage = "Employees not found";

            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetStatutoryComponent)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveEmployeeEPFMapping(StatutoryComponent_EPFDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                //_mapper.Map<StatutoryComponent_EPF>(inputDTO);
                var sMsg = _unitOfWork.StatutoryComponent_EPF.SaveEmployeeEPFMapping(_mapper.Map<StatutoryComponent_EPF>(inputDTO), inputDTO.MappingEmployeeIds);
                return Ok("Success");
            }
            else
                return BadRequest("Invalid model SaveEmployeeEPFMapping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveStatutoryComponent)}");
            throw;
        }
    }

    //[HttpGet]
    //[Route("PayrollSalary/GetEPFEmployees/{id}")]
    public async Task<List<EPFEmployeeMappingDTO>> GetEPFEmployees(int id)
    {
        try
        {
            List<EPFEmployeeMappingDTO> epfEmpMapList;
            epfEmpMapList = new List<EPFEmployeeMappingDTO>();
            epfEmpMapList = _mapper.Map<List<EPFEmployeeMappingDTO>>(_unitOfWork.EPFEmployeeMapping.GetAll(r => r.StatutoryComponentsId == id).Result.ToList());
            return epfEmpMapList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveStatutoryComponent)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> SaveStatutoryComponent(StatutoryComponent_EPFDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.StatutoryComponentsId == 0)
                {
                    Expression<Func<StatutoryComponent_EPF, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    if (!_unitOfWork.StatutoryComponent_EPF.Exists(expression))
                    {
                        _unitOfWork.StatutoryComponent_EPF.AddAsync(_mapper.Map<StatutoryComponent_EPF>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate Entry Found");
                }
                else
                {
                    _unitOfWork.StatutoryComponent_EPF.Update(_mapper.Map<StatutoryComponent_EPF>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveStatutoryComponent)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetStatutoryComponentESI(int UnitId)
    {
        try
        {
            StatutoryComponentsEsiDTO outputDTO = _mapper.Map<StatutoryComponentsEsiDTO>(_unitOfWork.StatutoryComponentsEsi.GetAll(x => x.UnitId == UnitId).Result.FirstOrDefault());
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetStatutoryComponent)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetStatutoryComponentLabourWelfareFund(int UnitId)
    {
        try
        {
            StatutoryComponentsLabourWelfareFundDTO outputDTO = _mapper.Map<StatutoryComponentsLabourWelfareFundDTO>(_unitOfWork.StatutoryComponentsLabourWelfareFund.GetAll(x => x.UnitId == UnitId).Result.FirstOrDefault());
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetStatutoryComponent)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveStatutoryComponentsESI(StatutoryComponentsEsiDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.StatutoryComponentsEsiid == 0)
                {
                    Expression<Func<StatutoryComponentsEsi, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    if (!_unitOfWork.StatutoryComponentsEsi.Exists(expression))
                    {
                        _unitOfWork.StatutoryComponentsEsi.AddAsync(_mapper.Map<StatutoryComponentsEsi>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate Entry Found");
                }
                else
                {
                    _unitOfWork.StatutoryComponentsEsi.Update(_mapper.Map<StatutoryComponentsEsi>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveStatutoryComponentsESI)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> SaveStatutoryComponentLabourWelfareFund(StatutoryComponentsLabourWelfareFundDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.StatutoryComponentsLabourWelfareFundId == 0)
                {
                    Expression<Func<StatutoryComponentsLabourWelfareFund, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    if (!_unitOfWork.StatutoryComponentsLabourWelfareFund.Exists(expression))
                    {
                        _unitOfWork.StatutoryComponentsLabourWelfareFund.AddAsync(_mapper.Map<StatutoryComponentsLabourWelfareFund>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate Entry Found");
                }
                else
                {
                    _unitOfWork.StatutoryComponentsLabourWelfareFund.Update(_mapper.Map<StatutoryComponentsLabourWelfareFund>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveStatutoryComponentLabourWelfareFund)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> EnableEPFData(StatutoryComponent_EPFDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                StatutoryComponent_EPF fetchedData = _mapper.Map<StatutoryComponent_EPF>(await _unitOfWork.StatutoryComponent_EPF.GetByIdAsync(inputData.StatutoryComponentsId));
                fetchedData.IsActive = true;
                _unitOfWork.StatutoryComponent_EPF.Update(fetchedData);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent, "Validation Failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(EnableEPFData)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DisableEPFData(StatutoryComponent_EPFDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                StatutoryComponent_EPF fetchedData = _mapper.Map<StatutoryComponent_EPF>(await _unitOfWork.StatutoryComponent_EPF.GetByIdAsync(inputData.StatutoryComponentsId));
                fetchedData.IsActive = false;
                _unitOfWork.StatutoryComponent_EPF.Update(fetchedData);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent, "Validation Failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DisableEPFData)}");
            throw;
        }
    }
    public async Task<IActionResult> EnableESIData(StatutoryComponentsEsiDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                StatutoryComponentsEsi fetchedData = _mapper.Map<StatutoryComponentsEsi>(await _unitOfWork.StatutoryComponentsEsi.GetByIdAsync(inputData.StatutoryComponentsEsiid));
                fetchedData.IsActive = true;
                _unitOfWork.StatutoryComponentsEsi.Update(fetchedData);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent, "Validation Failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(EnableESIData)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DisableESIData(StatutoryComponentsEsiDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                StatutoryComponentsEsi fetchedData = _mapper.Map<StatutoryComponentsEsi>(await _unitOfWork.StatutoryComponentsEsi.GetByIdAsync(inputData.StatutoryComponentsEsiid));
                fetchedData.IsActive = false;
                _unitOfWork.StatutoryComponentsEsi.Update(fetchedData);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent, "Validation Failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DisableEPFData)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> EnableLabourWelfareFundData(StatutoryComponentsLabourWelfareFundDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                StatutoryComponentsLabourWelfareFund fetchedData = _mapper.Map<StatutoryComponentsLabourWelfareFund>(await _unitOfWork.StatutoryComponentsLabourWelfareFund.GetByIdAsync(inputData.StatutoryComponentsLabourWelfareFundId));
                fetchedData.IsActive = true;
                _unitOfWork.StatutoryComponentsLabourWelfareFund.Update(fetchedData);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent, "Validation Failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(EnableLabourWelfareFundData)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DisableLabourWelfareFundData(StatutoryComponentsLabourWelfareFundDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                StatutoryComponentsLabourWelfareFund fetchedData = _mapper.Map<StatutoryComponentsLabourWelfareFund>(await _unitOfWork.StatutoryComponentsLabourWelfareFund.GetByIdAsync(inputData.StatutoryComponentsLabourWelfareFundId));
                fetchedData.IsActive = false;
                _unitOfWork.StatutoryComponentsLabourWelfareFund.Update(fetchedData);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent, "Validation Failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DisableLabourWelfareFundData)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> SaveProfessionalTax(ProfessionalTaxDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.ProfTaxId == 0)
                {
                    Expression<Func<ProfessionalTax, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.StateId==inputDTO.StateId && a.Gender == inputDTO.Gender;
                    if (!_unitOfWork.ProfessionalTax.Exists(expression))
                    {
                        _unitOfWork.ProfessionalTax.AddAsync(_mapper.Map<ProfessionalTax>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                        return BadRequest("Duplicate Entry Found");
                }
                else
                {
                    _unitOfWork.ProfessionalTax.Update(_mapper.Map<ProfessionalTax>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveStatutoryComponentLabourWelfareFund)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> GetProfessionalTax(ProfessionalTaxDTO inputDTO)
    {
        try
        {
            ProfessionalTaxDTO outputDTO = _mapper.Map<ProfessionalTaxDTO>(await _unitOfWork.ProfessionalTax.GetByIdAsync(inputDTO.ProfTaxId));
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
            _logger.LogError(ex, $"Error in retriving Professional Tax {nameof(GetProfessionalTax)}");
            throw;
        }
    }


    [HttpGet(Name = "GetProfessionalTaxes")]
    public async Task<IActionResult> GetProfessionalTaxes(Core.Helper.RequestParams requestParams, int? UnitId)
    {
        try
        {
            // IList<CityMasterDTO> viewModel = new List<CityMasterDTO>();

            var returnData = _mapper.Map<IList<ProfessionalTaxDTO>>(await _unitOfWork.ProfessionalTax.GetAll(requestParams: requestParams,
                                                                                              expression: (p =>p.UnitId== UnitId &&  p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                              orderBy: (m => m.OrderBy(x => x.ProfTaxId))));
            IList<ProfessionalTaxDTO> viewModel = new List<ProfessionalTaxDTO>();

            var data = returnData.Select(r => new ProfessionalTaxDTO
            {
                 Gender=r.Gender,
                 ProfTax = r.ProfTax,
                 MaxSalary = r.MaxSalary,
                 MinSalary = r.MinSalary,
                 ProfTaxId = r.ProfTaxId,
                CountryId = r.Country.CountryId,
                CountryName = r.Country.CountryName,
                StateId = r.State.StateId,
                StateName = r.State.StateName,
                 WEFDate = r.WEFDate
            }).ToList();
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Professional Taxes {nameof(GetProfessionalTaxes)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProfessionalTax(ProfessionalTaxDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ProfessionalTax profTax = _mapper.Map<ProfessionalTax>(await _unitOfWork.ProfessionalTax.GetByIdAsync(inputDTO.ProfTaxId));
                profTax.IsActive = false;
                _unitOfWork.ProfessionalTax.Update(profTax);
                _unitOfWork.Save();
                return Ok(ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success"));
            }
            return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country Professional Tax {nameof(DeleteProfessionalTax)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> SaveComponentsTaxLimit(ComponentsTaxLimitDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Expression<Func<ComponentsTaxLimit, bool>> expression = a => a.GratuityLimit == inputDTO.GratuityLimit;
                ComponentsTaxLimit? objTaxLmt = _unitOfWork.ComponentsTaxLimit.GetWithRawSql("select * from ComponentsTaxLimit").FirstOrDefault();
                if (objTaxLmt == null)
                {
                    _unitOfWork.ComponentsTaxLimit.AddAsync( _mapper.Map<ComponentsTaxLimit>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                {
                    ComponentsTaxLimit? objTaxLimit = _unitOfWork.ComponentsTaxLimit.GetWithRawSql("select * from ComponentsTaxLimit").FirstOrDefault();
                    if (objTaxLimit != null)
                    {
                        objTaxLimit.PFLimit = inputDTO.PFLimit;
                        objTaxLimit.GratuityLimit = inputDTO.GratuityLimit;
                        objTaxLimit.LeaveEncashmentLimit = inputDTO.LeaveEncashmentLimit;
                        objTaxLimit.ModifiedBy = inputDTO.ModifiedBy;
                        objTaxLimit.ModifiedOn = inputDTO.ModifiedOn;
                        _unitOfWork.ComponentsTaxLimit.Update(_mapper.Map<ComponentsTaxLimit>(objTaxLimit));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                    {
                        return BadRequest("failed");
                    }
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Component Tax Limit {nameof(SaveComponentsTaxLimit)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> GetComponentsTaxLimit(ComponentsTaxLimitDTO inputDTO)
    {
        try
        {
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            //inputDTO.SettingId = 0;
            var objComponentsList = await _unitOfWork.ComponentsTaxLimit.GetAll(null, null, null);

            ComponentsTaxLimitDTO objComponents = _mapper.Map<ComponentsTaxLimitDTO>(_unitOfWork.ComponentsTaxLimit.GetAll(null, null, null).Result.FirstOrDefault());
            if (objComponents == null)
                inputDTO.TaxLimitId = 0;
            else
                inputDTO.TaxLimitId = objComponents.TaxLimitId;

            ComponentsTaxLimitDTO outputDTO = _mapper.Map<ComponentsTaxLimitDTO>(await _unitOfWork.ComponentsTaxLimit.GetByIdAsync(inputDTO.TaxLimitId));

            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
            //  LoanPaymentDetailDTO loanDetails = _mapper.Map<LoanPaymentDetailDTO>(_unitOfWork.LoanPaymentDetail.GetAll(null, null, null).Result.Where(a => a.RepaymentId == inputs.RepaymetId).FirstOrDefault());
            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Components Tax Limit {nameof(GetComponentsTaxLimit)}");
            throw;
        }
    }

}
