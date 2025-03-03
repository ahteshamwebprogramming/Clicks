using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Services.DBContext;
using System.Drawing;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
namespace SimpliHR.Endpoints.Payroll;

[Route("api/[controller]")]
[ApiController]
public class LoanAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoanAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _simpliDbContext;
    public LoanAPIController(IUnitOfWork unitOfWork, ILogger<LoanAPIController> logger, IMapper mapper, SimpliDbContext SimpliDbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _simpliDbContext = SimpliDbContext;
    }

    [HttpPost]
    public IActionResult SaveSanctionLoan(LoanMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.SanctionId == 0)
                {
                    var Details = _unitOfWork.LoanMaster.GetAll(null, null, null).Result.Where(x => x.EmployeeId == inputDTO.EmployeeId && x.Status==0 && x.IsActive == false).FirstOrDefault();
                    if(Details !=null)
                    {
                        DateTime lDate = new DateTime((int)Details.RepaymentStopYear, (int)Details.RepaymentStopMonth, 1);
                        DateTime sDate = new DateTime((int)inputDTO.RepaymentStartYear, (int)inputDTO.RepaymentStartMonth, 1);
                        if(sDate<= lDate)
                        {
                            return BadRequest("Loan start month and year should be more than last closing date");
                        }
                    }

                    Expression<Func<LoanMaster, bool>> expression = a => a.UnitId == inputDTO.UnitId && a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true;
                    if (!_unitOfWork.LoanMaster.Exists(expression))
                    {
                        _unitOfWork.LoanMaster.AddAsync(_mapper.Map<LoanMaster>(inputDTO));
                        _unitOfWork.Save();
                        int Id = _unitOfWork.LoanMaster.GetAll(null, null, null).Result.DefaultIfEmpty().Max(r => r == null ? 0 : r.SanctionId);
                        if (Id > 0)
                        {
                            LoanPaymentDetailDTO objLoanPayment = new LoanPaymentDetailDTO();
                            objLoanPayment.SanctionId = Id;
                            objLoanPayment.Deduction = inputDTO.MonthlyInstallment;
                            objLoanPayment.OpeningBalance = inputDTO.SanctionAmount;
                            objLoanPayment.ClosingBalance = (inputDTO.SanctionAmount - inputDTO.MonthlyInstallment);
                            objLoanPayment.RepaymentMonth = inputDTO.RepaymentStartMonth;
                            objLoanPayment.RepaymentYear = inputDTO.RepaymentStartYear;
                            objLoanPayment.EmployeeId = inputDTO.EmployeeId;
                            objLoanPayment.PendingInstallment = inputDTO.RepaymentTenure;
                            objLoanPayment.Status = 1;
                            objLoanPayment.IsActive = true;
                            objLoanPayment.CreatedBy = inputDTO.CreatedBy;
                            objLoanPayment.CreatedOn = inputDTO.CreatedOn;

                            //Expression<Func<LoanPaymentDetail, bool>> expression1 = a => a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true;
                            //if (!_unitOfWork.LoanPaymentDetail.Exists(expression1))
                            //{
                                _unitOfWork.LoanPaymentDetail.AddAsync(_mapper.Map<LoanPaymentDetail>(objLoanPayment));
                                _unitOfWork.Save();
                           // }

                        }
                        return Ok("Saved");
                    }
                    else
                        return BadRequest("Duplicate entry found");
                }
                else
                {
                    return BadRequest();
                }

            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save loan sanction {nameof(SaveSanctionLoan)}");
            throw;
        }
    }

    [HttpPost]
    public IActionResult UpdateSanctionLoan(LoanMasterDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Expression<Func<LoanMaster, bool>> expression = a => (a.UnitId == inputDTO.UnitId && a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true) && a.SanctionId != inputDTO.SanctionId;
                if (!_unitOfWork.LoanMaster.Exists(expression))
                {
                    _unitOfWork.LoanMaster.Update(_mapper.Map<LoanMaster>(inputDTO));
                    _unitOfWork.Save();
                    return StatusCode(StatusCodes.Status200OK, "Success");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while update Employee {nameof(UpdateSanctionLoan)}");
            throw;
        }

    }

    public async Task<IActionResult> GetSanctionLoanDetails(Core.Helper.RequestParams requestParams, int? unitId)
    {
        try
        {
            // IList<LoanMasterDTO> ViewModel = new List<LoanMasterDTO>();
            var returnData = _mapper.Map<IList<LoanMasterDTO>>(await _unitOfWork.LoanMaster.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.UnitId == unitId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
                                                                                             orderBy: (m => m.OrderBy(x => x.SanctionId))));



            var data = returnData.Select(r => new LoanMasterDTO
            {
                SanctionId = r.SanctionId,
                EmployeeId = r.EmployeeId,
                SanctionAmount = r.SanctionAmount,
                MonthlyInstallment = r.MonthlyInstallment,
                RepaymentTenure = r.RepaymentTenure,
                RepaymentStartMonth = r.RepaymentStartMonth,
                RepaymentStartYear = r.RepaymentStartYear,
                ClosingBalance = r.ClosingBalance,
                RepaymentStopMonth = r.RepaymentStopMonth,
                RepaymentStopYear = r.RepaymentStopYear,
                Month = getFullName((int)r.RepaymentStartMonth),
                StopMonth = getFullName((int)r.RepaymentStopMonth)
            }).ToList();

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Sanction Loan {nameof(GetSanctionLoanDetails)}");
            throw;
        }
    }



    public async Task<IActionResult> GetEmployeeSanctionLoanDetails(Core.Helper.RequestParams requestParams, int? unitId, int? employeeId)
    {
        try
        {
            // IList<LoanMasterDTO> ViewModel = new List<LoanMasterDTO>();
            var returnData = _mapper.Map<IList<LoanMasterDTO>>(await _unitOfWork.LoanMaster.GetAll(requestParams: requestParams,
                                                                                             expression: (p => p.UnitId == unitId && p.EmployeeId== employeeId),
                                                                                             orderBy: (m => m.OrderBy(x => x.SanctionId))));



            var data = returnData.Select(r => new LoanMasterDTO
            {
                SanctionId = r.SanctionId,
                Status = r.Status,
                SanctionAmount = r.SanctionAmount,
                MonthlyInstallment = r.MonthlyInstallment,
                RepaymentTenure = r.RepaymentTenure,
                RepaymentStartMonth = r.RepaymentStartMonth,
                RepaymentStartYear = r.RepaymentStartYear,
                ClosingBalance = r.ClosingBalance,
                RepaymentStopMonth = r.RepaymentStopMonth,
                RepaymentStopYear = r.RepaymentStopYear,
                Month = getFullName((int)r.RepaymentStartMonth),
                StopMonth = getFullName((int)r.RepaymentStopMonth)
            }).ToList();

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Sanction Loan {nameof(GetSanctionLoanDetails)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRepaymentLoanDetails(Core.Helper.RequestParams requestParams, int? unitId, int? employeeId, int? month, int? year, int? status)
    {
        try
        {
            //IList<LoanPaymentDetailDTO> returnData;

            //if (month > 0 && year > 0)
            //{
            //    returnData = _mapper.Map<IList<LoanPaymentDetailDTO>>(await _unitOfWork.LoanPaymentDetail.GetAll(requestParams: requestParams,
            //                                                                                               expression: (p => p.EmployeeId == employeeId && p.RepaymentMonth == month && p.RepaymentYear == year && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
            //                                                                                               orderBy: (m => m.OrderBy(x => x.RepaymentId))));
            //}
            //else
            //{
            //    if (status > 0)
            //        returnData = _mapper.Map<IList<LoanPaymentDetailDTO>>(await _unitOfWork.LoanPaymentDetail.GetAll(requestParams: requestParams,
            //                                                                                                   expression: (p => p.EmployeeId == employeeId && p.Status == status && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
            //                                                                                                   orderBy: (m => m.OrderBy(x => x.RepaymentId))));
            //    else
            //        returnData = _mapper.Map<IList<LoanPaymentDetailDTO>>(await _unitOfWork.LoanPaymentDetail.GetAll(requestParams: requestParams,
            //                                                                                                                   expression: (p => p.EmployeeId == employeeId && p.IsActive == (requestParams.IsActive == null ? true : requestParams.IsActive)),
            //                                                                                                                   orderBy: (m => m.OrderBy(x => x.RepaymentId))));
            //}

            var results = ( from loan in  _unitOfWork.LoanMaster.GetAll(null, null, null).Result
                           join details in _unitOfWork.LoanPaymentDetail.GetAll(null, null, null).Result on loan.SanctionId equals details.SanctionId
                           where (loan.EmployeeId == employeeId && loan.IsActive==true)
                           select new LoanPaymentDetailDTO
                           {
                               OpeningBalance = details.OpeningBalance,
                                Deduction= details.Deduction,
                                 ClosingBalance= details.ClosingBalance,
                                 Remarks= details.Remarks,
                                 Status= details.Status,
                               EmployeeId = details.EmployeeId,
                               Month = getFullName((int)details.RepaymentMonth),
                               RepaymentYear = details.RepaymentYear,
                               RepaymentId = details.RepaymentId
                           }).ToList();


            //var data = returnData.Select(r => new LoanPaymentDetailDTO
            //{
            //    RepaymentId = r.RepaymentId,
            //    EmployeeId = r.EmployeeId,
            //    OpeningBalance = r.OpeningBalance,
            //    Deduction = r.Deduction,
            //    ClosingBalance = r.ClosingBalance,
            //    RepaymentMonth = r.RepaymentMonth,
            //    Month = getFullName((int)r.RepaymentMonth),
            //    RepaymentYear = r.RepaymentYear,
            //    Status = r.Status,
            //    Remarks = r.Remarks
            //}).ToList();

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Sanction Loan {nameof(GetSanctionLoanDetails)}");
            throw;
        }
    }

    static string getFullName(int month)
    {
        DateTime date = new DateTime(2020, month, 1);

        return date.ToString("MMM");
    }
    [HttpPost]
    public async Task<IActionResult> UpdateRepaymentLoan(RepaymentLoanInputs inputs)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Expression<Func<LoanPaymentDetail, bool>> expression = a => (a.IsActive == true) && a.RepaymentId != inputs.RepaymetId;
                // if (!_unitOfWork.LoanPaymentDetail.Exists(expression))
                // {
                LoanPaymentDetailDTO loanDetails = _mapper.Map<LoanPaymentDetailDTO>(_unitOfWork.LoanPaymentDetail.GetAll(null, null, null).Result.Where(a => a.RepaymentId == inputs.RepaymetId).FirstOrDefault());
                // LoanPaymentDetailDTO inputDTO = new LoanPaymentDetailDTO();
                loanDetails.RepaymentId = (int)inputs.RepaymetId;
                loanDetails.Deduction = inputs.Deduction;
                loanDetails.Remarks = inputs.Remarks;
                loanDetails.ModifiedBy = inputs.ModifiedBy;
                loanDetails.ModifiedOn = inputs.ModifiedOn;
                loanDetails.ClosingBalance = loanDetails.OpeningBalance - inputs.Deduction;
                _unitOfWork.LoanPaymentDetail.Update(_mapper.Map<LoanPaymentDetail>(loanDetails));
                _unitOfWork.Save();

                int? SanctionId = _unitOfWork.LoanPaymentDetail.GetAll(null, null, null).Result.Where(x => x.RepaymentId == inputs.RepaymetId).Select(x=>x.SanctionId).FirstOrDefault();
                if (SanctionId > 0)
                {
                    var Details = _unitOfWork.LoanMaster.GetAll(null, null, null).Result.Where(x => x.SanctionId == SanctionId).FirstOrDefault();
                    //LoanMasterDTO outputDTO = _mapper.Map<LoanMasterDTO>(await _unitOfWork.LoanMaster.GetByIdAsync((int)SanctionId));
                    LoanMasterDTO objLoanPayment = new LoanMasterDTO();
                    // objLoanPayment.SanctionId = Id;
                    objLoanPayment.SanctionId = (int)SanctionId;
                    objLoanPayment.ClosingBalance = loanDetails.ClosingBalance;
                    objLoanPayment.SanctionAmount = Details.SanctionAmount;
                    objLoanPayment.MonthlyInstallment = Details.MonthlyInstallment;
                    objLoanPayment.UnitId = Details.UnitId;
                    objLoanPayment.EmployeeId = Details.EmployeeId;
                    objLoanPayment.RepaymentStartMonth = Details.RepaymentStartMonth;
                    objLoanPayment.RepaymentStopMonth = Details.RepaymentStopMonth;
                    objLoanPayment.RepaymentStartYear = Details.RepaymentStartYear;
                    objLoanPayment.RepaymentStopYear = Details.RepaymentStopYear;
                    objLoanPayment.RepaymentTenure = Details.RepaymentTenure;
                    objLoanPayment.Status = Details.Status;
                    objLoanPayment.IsActive = Details.IsActive;
                    objLoanPayment.InstallmentMode = Details.Status;
                    objLoanPayment.ModifiedBy = inputs.ModifiedBy;
                    objLoanPayment.ModifiedOn = inputs.ModifiedOn;
                    objLoanPayment.CreatedBy = Details.CreatedBy;
                    objLoanPayment.CreatedOn = Details.CreatedOn;
                    if (loanDetails.ClosingBalance <= 0)
                    {
                        objLoanPayment.Status = 0;
                        objLoanPayment.IsActive = false;
                        objLoanPayment.RepaymentStopYear = DateTime.Now.Year;
                        objLoanPayment.RepaymentStopMonth = DateTime.Now.Month;
                    }
                       

                    //outputDTO.ClosingBalance= loanDetails.ClosingBalance;
                    //outputDTO.ModifiedBy = inputs.ModifiedBy;
                    //outputDTO.ModifiedOn = inputs.ModifiedOn;
                    //Expression<Func<LoanPaymentDetail, bool>> expression1 = a => a.EmployeeId == inputDTO.EmployeeId && a.IsActive == true;
                    //if (!_unitOfWork.LoanPaymentDetail.Exists(expression1))
                    //{
                    _unitOfWork.LoanMaster.Update(_mapper.Map<LoanMaster>(objLoanPayment));
                    _unitOfWork.Save();
                    //}

                }


                return StatusCode(StatusCodes.Status200OK, "Success");
                // }
                //  else
                // {
                return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry");
                // }
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while update Employee {nameof(UpdateSanctionLoan)}");
            throw;
        }

    }



    public async Task<IActionResult> EmployeeLoan(Core.Helper.RequestParams requestParams, int? employeeId, int? SanctionId)
    {
        try
        {


            var returnData = _mapper.Map<IList<LoanPaymentDetailDTO>>(await _unitOfWork.LoanPaymentDetail.GetAll(requestParams: requestParams,
                                                                                                                        expression: (p => p.EmployeeId == employeeId && p.SanctionId== SanctionId),
                                                                                                                        orderBy: (m => m.OrderBy(x => x.RepaymentId))));


            var data = returnData.Select(r => new LoanPaymentDetailDTO
            {
                RepaymentId = r.RepaymentId,
                EmployeeId = r.EmployeeId,
                OpeningBalance = r.OpeningBalance,
                Deduction = r.Deduction,
                ClosingBalance = r.ClosingBalance,
                RepaymentMonth = r.RepaymentMonth,
                Month = getFullName((int)r.RepaymentMonth),
                RepaymentYear = r.RepaymentYear,
                Status = r.Status,
                Remarks = r.Remarks
            }).ToList();

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving employee Loan {nameof(EmployeeLoan)}");
            throw;
        }

    }

    [HttpPost]
    public async Task<IActionResult> GetEmployeeLoan(LoanMasterDTO inputDTO)
    {
        try
        {
            LoanMasterDTO outputDTO = _mapper.Map<LoanMasterDTO>(await _unitOfWork.LoanMaster.GetByIdAsync(inputDTO.SanctionId));
            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);
            //  LoanPaymentDetailDTO loanDetails = _mapper.Map<LoanPaymentDetailDTO>(_unitOfWork.LoanPaymentDetail.GetAll(null, null, null).Result.Where(a => a.RepaymentId == inputs.RepaymetId).FirstOrDefault());
            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee Loan {nameof(GetEmployeeLoan)}");
            throw;
        }
    }
}

