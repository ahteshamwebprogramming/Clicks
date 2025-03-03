using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.ITDeclaration;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Infrastructure.Models.StatutoryComponent;
using SimpliHR.Services.DBContext;
using SimpliHR.WebUI.Modals.ITDeclarations;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq.Expressions;

namespace SimpliHR.Endpoints.Payroll;

[Route("api/[controller]")]
[ApiController]
public class ITDeclarationAPIController : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ITDeclarationAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _simpliDbContext;

    public ITDeclarationAPIController(IUnitOfWork unitOfWork, ILogger<ITDeclarationAPIController> logger, IMapper mapper, SimpliDbContext SimpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = SimpliDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> GetHouseRentDetails(int UnitId, int EmployeeId, string FY, string Regime)
    {
        try
        {
            var res = _mapper.Map<IList<ItDeclarationHouseRentDetailDTO>>(await _unitOfWork.ItDeclarationHouseRentDetail.GetAll(x => x.UnitId == UnitId && x.EmployeeId == EmployeeId && x.FY == FY && x.Regime == Regime && x.IsActive == true));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetHouseRentDetails)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetHomeLoneDetails(int UnitId, int EmployeeId, string FY, string Regime)
    {
        try
        {
            var res = _mapper.Map<ItDeclarationHomeLoanDetailDTO>(_unitOfWork.ItDeclarationHomeLoanDetail.GetAll(x => x.UnitId == UnitId && x.EmployeeId == EmployeeId && x.FY == FY && x.Regime == Regime && x.IsActive == true).Result.FirstOrDefault());
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetHouseRentDetails)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetLentOutPropertyDetails(int UnitId, int EmployeeId, string FY, string Regime)
    {
        try
        {
            var res = _mapper.Map<IList<ItDeclarationLentOutPropertyDetailDTO>>(await _unitOfWork.ItDeclarationLentOutPropertyDetail.GetAll(x => x.UnitId == UnitId && x.EmployeeId == EmployeeId && x.FY == FY && x.Regime == Regime && x.IsActive == true));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetLentOutPropertyDetails)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetOtherSourcesOfIncomeDetails(int UnitId, int EmployeeId, string FY, string Regime)
    {
        try
        {
            var res = _mapper.Map<ItDeclarationOtherSourceOfIncomeDTO>(_unitOfWork.ItDeclarationOtherSourceOfIncome.GetAll(x => x.UnitId == UnitId && x.EmployeeId == EmployeeId && x.FY == FY && x.Regime == Regime && x.IsActive == true).Result.FirstOrDefault());
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetOtherSourcesOfIncomeDetails)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetInvestments80C(int UnitId, int EmployeeId, string FY, string Regime)
    {
        try
        {
            var res = _mapper.Map<IList<ItDeclaration80CinvestmentDTO>>(await _unitOfWork.ItDeclaration80Cinvestment.GetAll(x => x.UnitId == UnitId && x.EmployeeId == EmployeeId && x.FY == FY && x.Regime == Regime && x.IsActive == true));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetInvestments80C)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetExemptions80D(int UnitId, int EmployeeId, string FY, string Regime)
    {
        try
        {
            var res = _mapper.Map<IList<ItDeclaration80DexemptionDTO>>(await _unitOfWork.ItDeclaration80Dexemption.GetAll(x => x.UnitId == UnitId && x.EmployeeId == EmployeeId && x.FY == FY && x.Regime == Regime && x.IsActive == true));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetExemptions80D)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetOtherInvestmentExemptions(int UnitId, int EmployeeId, string FY, string Regime)
    {
        try
        {
            var res = _mapper.Map<IList<ItDeclarationOtherInvestmentExemptionDTO>>(await _unitOfWork.ItDeclarationOtherInvestmentExemption.GetAll(x => x.UnitId == UnitId && x.EmployeeId == EmployeeId && x.FY == FY && x.Regime == Regime && x.IsActive == true));
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetOtherInvestmentExemptions)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetPreviousEmploymentDetails(int UnitId, int EmployeeId, string FY, string Regime)
    {
        try
        {
            var res = _mapper.Map<ItDeclarationPreviousEmployementDTO>(_unitOfWork.ItDeclarationPreviousEmployement.GetAll(x => x.UnitId == UnitId && x.EmployeeId == EmployeeId && x.FY == FY && x.Regime == Regime && x.IsActive == true).Result.FirstOrDefault());
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(GetPreviousEmploymentDetails)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveItDeclarationType(ItDeclarationTypeDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<ItDeclarationType, bool>> expression = a => a.EmployeeId == inputDTO.EmployeeId && a.Fy == inputDTO.Fy && a.UnitId == inputDTO.UnitId && a.IsActive == 1;
                if (!_unitOfWork.ItDeclarationType.Exists(expression))
                {
                    _unitOfWork.ItDeclarationType.AddAsync(_mapper.Map<ItDeclarationType>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                {
                    //SqlParameter[] parm = new SqlParameter[2];
                    //parm[0] = new SqlParameter("@EmployeeId", inputDTO.EmployeeId);
                    //parm[1] = new SqlParameter("@FY", inputDTO.Fy);

                    ItDeclarationType? itDeclarationType = _unitOfWork.ItDeclarationType.GetWithRawSql("select * from ItDeclarationType where EmployeeId=" + inputDTO.EmployeeId + " and FY='" + inputDTO.Fy + "' and isactive=1").FirstOrDefault();
                    if (itDeclarationType != null)
                    {
                        itDeclarationType.RegimeType = inputDTO.RegimeType;
                        _unitOfWork.ItDeclarationType.Update(_mapper.Map<ItDeclarationType>(itDeclarationType));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                    {
                        return BadRequest("Unable to find the record of this financial year");
                    }
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveItDeclarationType)}");
            throw;
        }
    }


    [HttpPost]
    public IActionResult SaveHouseRentDetails(ItDeclarationHouseRentDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.ItDeclarationHouseRentDetailsId == 0)
                {
                    //Expression<Func<ItDeclarationHouseRentDetail, bool>> expression = a => a.UnitId == empInput.UnitId  ;
                    //if (!_unitOfWork.ItDeclarationHouseRentDetail.Exists(expression))
                    //{
                    _unitOfWork.ItDeclarationHouseRentDetail.AddAsync(_mapper.Map<ItDeclarationHouseRentDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.ItDeclarationHouseRentDetail.Update(_mapper.Map<ItDeclarationHouseRentDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveHouseRentDetails)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult SaveHomeLoanDetails(ItDeclarationHomeLoanDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                if (inputDTO.ItDeclarationHomeLoanDetailsId == 0)
                {
                    //Expression<Func<ItDeclarationHomeLoanDetail, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    //if (!_unitOfWork.ItDeclarationHomeLoanDetail.Exists(expression))
                    //{
                    _unitOfWork.ItDeclarationHomeLoanDetail.AddAsync(_mapper.Map<ItDeclarationHomeLoanDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.ItDeclarationHomeLoanDetail.Update(_mapper.Map<ItDeclarationHomeLoanDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveHomeLoanDetails)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult SaveLentOutPropertyDetails(ItDeclarationLentOutPropertyDetailDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                if (inputDTO.ItDeclarationLentOutPropertyDetailsId == 0)
                {
                    //Expression<Func<ItDeclarationLentOutPropertyDetail, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    //if (!_unitOfWork.ItDeclarationLentOutPropertyDetail.Exists(expression))
                    //{
                    _unitOfWork.ItDeclarationLentOutPropertyDetail.AddAsync(_mapper.Map<ItDeclarationLentOutPropertyDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.ItDeclarationLentOutPropertyDetail.Update(_mapper.Map<ItDeclarationLentOutPropertyDetail>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveLentOutPropertyDetails)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult SaveOtherSourceOfIncomeDetails(ItDeclarationOtherSourceOfIncomeDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                if (inputDTO.ItDeclarationOtherSourceOfIncomeId == 0)
                {
                    //Expression<Func<ItDeclarationOtherSourceOfIncome, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    //if (!_unitOfWork.ItDeclarationOtherSourceOfIncome.Exists(expression))
                    //{
                    _unitOfWork.ItDeclarationOtherSourceOfIncome.AddAsync(_mapper.Map<ItDeclarationOtherSourceOfIncome>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.ItDeclarationOtherSourceOfIncome.Update(_mapper.Map<ItDeclarationOtherSourceOfIncome>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveOtherSourceOfIncomeDetails)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult Save80CInvestmentDetails(ItDeclaration80CinvestmentDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                if (inputDTO.ItDeclaration80CinvestmentsId == 0)
                {
                    //Expression<Func<ItDeclaration80Cinvestment, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    //if (!_unitOfWork.ItDeclaration80Cinvestment.Exists(expression))
                    //{
                    _unitOfWork.ItDeclaration80Cinvestment.AddAsync(_mapper.Map<ItDeclaration80Cinvestment>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.ItDeclaration80Cinvestment.Update(_mapper.Map<ItDeclaration80Cinvestment>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(Save80CInvestmentDetails)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult Save80DExemptionDetails(ItDeclaration80DexemptionDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                if (inputDTO.ItDeclaration80DexemptionsId == 0)
                {
                    //Expression<Func<ItDeclaration80Dexemption, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    //if (!_unitOfWork.ItDeclaration80Dexemption.Exists(expression))
                    //{
                    _unitOfWork.ItDeclaration80Dexemption.AddAsync(_mapper.Map<ItDeclaration80Dexemption>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.ItDeclaration80Dexemption.Update(_mapper.Map<ItDeclaration80Dexemption>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(Save80DExemptionDetails)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveOtherInvestmentExemptionDetails(ItDeclarationOtherInvestmentExemptionDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                if (inputDTO.ItDeclarationOtherInvestmentExemptionId == 0)
                {
                    //Expression<Func<ItDeclarationOtherInvestmentExemption, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    //if (!_unitOfWork.ItDeclarationOtherInvestmentExemption.Exists(expression))
                    //{
                    _unitOfWork.ItDeclarationOtherInvestmentExemption.AddAsync(_mapper.Map<ItDeclarationOtherInvestmentExemption>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.ItDeclarationOtherInvestmentExemption.Update(_mapper.Map<ItDeclarationOtherInvestmentExemption>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveOtherInvestmentExemptionDetails)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult SavePreviousEmployementDetails(ItDeclarationPreviousEmployementDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {

                if (inputDTO.ItDeclarationPreviousEmployementId == 0)
                {
                    //Expression<Func<ItDeclarationPreviousEmployement, bool>> expression = a => a.UnitId == inputDTO.UnitId;
                    //if (!_unitOfWork.ItDeclarationPreviousEmployement.Exists(expression))
                    //{
                    _unitOfWork.ItDeclarationPreviousEmployement.AddAsync(_mapper.Map<ItDeclarationPreviousEmployement>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                    //}
                    //else
                    //    return BadRequest("Duplicate entry found");
                }
                else
                {
                    _unitOfWork.ItDeclarationPreviousEmployement.Update(_mapper.Map<ItDeclarationPreviousEmployement>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }

            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SavePreviousEmployementDetails)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteHouseRentDetail(ItDeclarationHouseRentDetailDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ItDeclarationHouseRentDetail fetchedData = _mapper.Map<ItDeclarationHouseRentDetail>(await _unitOfWork.ItDeclarationHouseRentDetail.GetByIdAsync(inputData.ItDeclarationHouseRentDetailsId));
                fetchedData.IsActive = false;
                _unitOfWork.ItDeclarationHouseRentDetail.Update(fetchedData);
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
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteHouseRentDetail)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteLentOutPropertyDetail(ItDeclarationLentOutPropertyDetailDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ItDeclarationLentOutPropertyDetail fetchedData = _mapper.Map<ItDeclarationLentOutPropertyDetail>(await _unitOfWork.ItDeclarationLentOutPropertyDetail.GetByIdAsync(inputData.ItDeclarationLentOutPropertyDetailsId));
                fetchedData.IsActive = false;
                _unitOfWork.ItDeclarationLentOutPropertyDetail.Update(fetchedData);
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
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteLentOutPropertyDetail)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteInvestments80C(ItDeclaration80CinvestmentDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ItDeclaration80Cinvestment fetchedData = _mapper.Map<ItDeclaration80Cinvestment>(await _unitOfWork.ItDeclaration80Cinvestment.GetByIdAsync(inputData.ItDeclaration80CinvestmentsId));
                fetchedData.IsActive = false;
                _unitOfWork.ItDeclaration80Cinvestment.Update(fetchedData);
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
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteInvestments80C)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteExemptions80D(ItDeclaration80DexemptionDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ItDeclaration80Dexemption fetchedData = _mapper.Map<ItDeclaration80Dexemption>(await _unitOfWork.ItDeclaration80Dexemption.GetByIdAsync(inputData.ItDeclaration80DexemptionsId));
                fetchedData.IsActive = false;
                _unitOfWork.ItDeclaration80Dexemption.Update(fetchedData);
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
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteExemptions80D)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteOtherInvestmentsExemptions(ItDeclarationOtherInvestmentExemptionDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                ItDeclarationOtherInvestmentExemption fetchedData = _mapper.Map<ItDeclarationOtherInvestmentExemption>(await _unitOfWork.ItDeclarationOtherInvestmentExemption.GetByIdAsync(inputData.ItDeclarationOtherInvestmentExemptionId));
                fetchedData.IsActive = false;
                _unitOfWork.ItDeclarationOtherInvestmentExemption.Update(fetchedData);
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
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteOtherInvestmentsExemptions)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> UploadITDeclarationProof(ITDeclarationProofDocument inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputData.ItDeclarationParticular == "HouseRent")
                {
                    ItDeclarationHouseRentDetail fetchedData = _mapper.Map<ItDeclarationHouseRentDetail>(await _unitOfWork.ItDeclarationHouseRentDetail.GetByIdAsync(inputData.ItDeclarationParticularId));
                    fetchedData.ProofFile = inputData.DocumentProofByte;
                    fetchedData.ProofFileName = inputData.DocumentName;
                    fetchedData.ProofFileExtension = inputData.DocumentExtension;
                    _unitOfWork.ItDeclarationHouseRentDetail.Update(fetchedData);
                    _unitOfWork.Save();
                }
                else if (inputData.ItDeclarationParticular == "HomeLoan")
                {
                    ItDeclarationHomeLoanDetail fetchedData = _mapper.Map<ItDeclarationHomeLoanDetail>(await _unitOfWork.ItDeclarationHomeLoanDetail.GetByIdAsync(inputData.ItDeclarationParticularId));
                    fetchedData.ProofFile = inputData.DocumentProofByte;
                    fetchedData.ProofFileName = inputData.DocumentName;
                    fetchedData.ProofFileExtension = inputData.DocumentExtension;
                    _unitOfWork.ItDeclarationHomeLoanDetail.Update(fetchedData);
                    _unitOfWork.Save();
                }
                else if (inputData.ItDeclarationParticular == "LentOutProperty")
                {
                    ItDeclarationLentOutPropertyDetail fetchedData = _mapper.Map<ItDeclarationLentOutPropertyDetail>(await _unitOfWork.ItDeclarationLentOutPropertyDetail.GetByIdAsync(inputData.ItDeclarationParticularId));
                    fetchedData.ProofFile = inputData.DocumentProofByte;
                    fetchedData.ProofFileName = inputData.DocumentName;
                    fetchedData.ProofFileExtension = inputData.DocumentExtension;
                    _unitOfWork.ItDeclarationLentOutPropertyDetail.Update(fetchedData);
                    _unitOfWork.Save();
                }
                else if (inputData.ItDeclarationParticular == "80CInvestments")
                {
                    ItDeclaration80Cinvestment fetchedData = _mapper.Map<ItDeclaration80Cinvestment>(await _unitOfWork.ItDeclaration80Cinvestment.GetByIdAsync(inputData.ItDeclarationParticularId));
                    fetchedData.ProofFile = inputData.DocumentProofByte;
                    fetchedData.ProofFileName = inputData.DocumentName;
                    fetchedData.ProofFileExtension = inputData.DocumentExtension;
                    _unitOfWork.ItDeclaration80Cinvestment.Update(fetchedData);
                    _unitOfWork.Save();
                }
                else if (inputData.ItDeclarationParticular == "80DExemptions")
                {
                    ItDeclaration80Dexemption fetchedData = _mapper.Map<ItDeclaration80Dexemption>(await _unitOfWork.ItDeclaration80Dexemption.GetByIdAsync(inputData.ItDeclarationParticularId));
                    fetchedData.ProofFile = inputData.DocumentProofByte;
                    fetchedData.ProofFileName = inputData.DocumentName;
                    fetchedData.ProofFileExtension = inputData.DocumentExtension;
                    _unitOfWork.ItDeclaration80Dexemption.Update(fetchedData);
                    _unitOfWork.Save();
                }
                else if (inputData.ItDeclarationParticular == "OtherInvestmentsExemptions")
                {
                    ItDeclarationOtherInvestmentExemption fetchedData = _mapper.Map<ItDeclarationOtherInvestmentExemption>(await _unitOfWork.ItDeclarationOtherInvestmentExemption.GetByIdAsync(inputData.ItDeclarationParticularId));
                    fetchedData.ProofFile = inputData.DocumentProofByte;
                    fetchedData.ProofFileName = inputData.DocumentName;
                    fetchedData.ProofFileExtension = inputData.DocumentExtension;
                    _unitOfWork.ItDeclarationOtherInvestmentExemption.Update(fetchedData);
                    _unitOfWork.Save();
                }
                else if (inputData.ItDeclarationParticular == "OtherSourceOfIncome")
                {
                    ItDeclarationOtherSourceOfIncome fetchedData = _mapper.Map<ItDeclarationOtherSourceOfIncome>(await _unitOfWork.ItDeclarationOtherSourceOfIncome.GetByIdAsync(inputData.ItDeclarationParticularId));
                    fetchedData.ProofFile = inputData.DocumentProofByte;
                    fetchedData.ProofFileName = inputData.DocumentName;
                    fetchedData.ProofFileExtension = inputData.DocumentExtension;
                    _unitOfWork.ItDeclarationOtherSourceOfIncome.Update(fetchedData);
                    _unitOfWork.Save();
                }
                else if (inputData.ItDeclarationParticular == "PreviousEmployement")
                {
                    ItDeclarationPreviousEmployement fetchedData = _mapper.Map<ItDeclarationPreviousEmployement>(await _unitOfWork.ItDeclarationPreviousEmployement.GetByIdAsync(inputData.ItDeclarationParticularId));
                    fetchedData.ProofFile = inputData.DocumentProofByte;
                    fetchedData.ProofFileName = inputData.DocumentName;
                    fetchedData.ProofFileExtension = inputData.DocumentExtension;
                    _unitOfWork.ItDeclarationPreviousEmployement.Update(fetchedData);
                    _unitOfWork.Save();
                }
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent, "Validation Failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in country delete {nameof(DeleteOtherInvestmentsExemptions)}");
            throw;
        }
    }

}
