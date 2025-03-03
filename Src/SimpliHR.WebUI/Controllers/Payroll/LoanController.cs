using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.ExcelUploads;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Net;
using System.Text;
namespace SimpliHR.WebUI.Controllers.Payroll;

public class LoanController : Controller
    {
    private readonly LoanAPIController _loanAPIController;
    private readonly EmployeeMasterController _employeeAPIController;
    public LoanController(LoanAPIController loanAPIController, EmployeeMasterController EmployeeAPIController)
    {
        _loanAPIController = loanAPIController;
        _employeeAPIController = EmployeeAPIController;

    }

    public async Task<IActionResult> LoanSanctionMaster()
    {
        LoanMasterDTO results = new LoanMasterDTO();
        results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
        // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
        results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
        results.EmployeeMasterList = await GetEmployeeList();
        results.GetSanctionLoan = await GetSanctionLoan();
        results.GetLoanPaymentDetails = null;
        foreach (var item in results.GetSanctionLoan)
        {
            // item.EncryptedId = CommonHelper.EncryptURLHTML(item.SanctionId.ToString());
            item.Employee = results.EmployeeMasterList.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.EmployeeName).FirstOrDefault();
        }

        return View(results);
    }

    public async Task<IActionResult> LoanRepayment()
    {
        LoanMasterDTO results = new LoanMasterDTO();
        results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
        // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
        results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
        results.EmployeeMasterList = await GetEmployeeList();
        results.GetSanctionLoan = await GetSanctionLoan();
        results.GetLoanPaymentDetails = null;
        foreach (var item in results.GetSanctionLoan)
        {
            // item.EncryptedId = CommonHelper.EncryptURLHTML(item.SanctionId.ToString());
            item.Employee = results.EmployeeMasterList.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.EmployeeName).FirstOrDefault();
        }

        return View(results);
    }
    [HttpPost]
    public async Task<IActionResult> ManageLoans(LoanMasterDTO inputDTO)
    {
        try
        {
            LoanMasterDTO loanDTO = new LoanMasterDTO();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            if (unitId != null)
            {
               DateTime sDate = new DateTime((int)inputDTO.RepaymentStartYear, (int)inputDTO.RepaymentStartMonth, 1);
               DateTime cDate = sDate.AddMonths((int)inputDTO.RepaymentTenure);
             //  DateOnly date = new DateOnly(inputDTO.)
                inputDTO.UnitId = unitId ?? default(int);
                inputDTO.Status = 1;
                inputDTO.RepaymentStopYear = cDate.Year;
                inputDTO.RepaymentStopMonth = cDate.Month;
                inputDTO.ClosingBalance = inputDTO.SanctionAmount;
                inputDTO.InstallmentMode = 1;
                inputDTO.IsActive = true;
                if (inputDTO.SanctionId != null)
                {
                    if (inputDTO.SanctionId > 0)
                    {
                        
                                         
                        inputDTO.ModifiedOn = System.DateTime.Now;
                        inputDTO.ModifiedBy = employeeId;                       
                       
                        var resSalaryTemplateUpdate = _loanAPIController.UpdateSanctionLoan(inputDTO);
                        if (resSalaryTemplateUpdate != null)
                        {
                            ObjectResult objResult = (ObjectResult)resSalaryTemplateUpdate;
                            var objResultData = objResult.Value;
                            if (objResult.StatusCode == 200)
                                loanDTO.DisplayMessage = "Records updates completed successfully";
                            else
                                loanDTO.DisplayMessage = objResultData.ToString();
                            // if (inputData.LeaveYearId == 0)

                        }
                        else
                        {
                            loanDTO.DisplayMessage = "Some error has occurred. Please contact administrator";
                            //throw new Exception("Some error has occurred. Please contact administrator");
                        }
                    }
                    else
                    {
                       
                       // inputDTO.Status = 1;
                        inputDTO.CreatedOn = System.DateTime.Now;
                        inputDTO.CreatedBy = employeeId;   
                        
                       // inputDTO.IsActive = true;
                        var resSalaryTemplateSave = _loanAPIController.SaveSanctionLoan(inputDTO);
                        if (resSalaryTemplateSave != null)
                        {
                            ObjectResult objResult = (ObjectResult)resSalaryTemplateSave;

                            var objResultData = objResult.Value;
                           if(objResult.StatusCode==200)                           
                            loanDTO.DisplayMessage = "Records successfully saved";
                           else
                             loanDTO.DisplayMessage = objResultData.ToString();
                            
                        }
                        else
                        {
                            loanDTO.DisplayMessage = "Some error has occurred. Please contact administrator";
                         // throw new Exception("Some error has occurred. Please contact administrator");
                        }
                    }
                }
                else
                {
                    loanDTO.DisplayMessage = "Some error has occurred. Please contact administrator";
                }

                loanDTO.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
                // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
                loanDTO.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
                loanDTO.EmployeeMasterList = await GetEmployeeList();
                loanDTO.GetSanctionLoan = await GetSanctionLoan();
              //  loanDTO.GetLoanPaymentDetails = await GetEmployeeLoan();

                foreach (var item in loanDTO.GetSanctionLoan)
                {
                   item.Employee = loanDTO.EmployeeMasterList.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.EmployeeName).FirstOrDefault();
                }

                return View("LoanSanctionMaster", loanDTO);
            }
            else
            {
                return RedirectToAction("/Account/Login");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }

    }

    public async Task<List<EmployeeMasterDTO>?> GetEmployeeList()
    {
        int _loginId;

        int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
        bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
        IActionResult actionResult = await _employeeAPIController.GetEmployeesForClient(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"), isclient, loginId);
        ObjectResult objResult = (ObjectResult)actionResult;

        List<EmployeeMasterDTO> objResultData = (List<EmployeeMasterDTO>)objResult.Value;
        return objResultData;
    }

    public async Task<List<LoanMasterDTO>?> GetSanctionLoan()
    {

        IActionResult actionResult = await _loanAPIController.GetSanctionLoanDetails(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
        ObjectResult objResult = (ObjectResult)actionResult;

        List<LoanMasterDTO> objResultData = (List<LoanMasterDTO>)objResult.Value;
       
        foreach (var item in objResultData)
        {
            item.EncryptedId = CommonHelper.EncryptURLHTML(item.SanctionId.ToString());
           // item.Employee = employees.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.Employee).FirstOrDefault();
            // item.Employee = employeeSalaryTemplateMappingVM.EmployeeKeyValues.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.EmployeeName).FirstOrDefault();
        }
        return objResultData;
    }
    [HttpPost]
    public async Task<IActionResult> GetEmployeeLoan(int? Year, int? Month, int? Employee,int? Status)
    {

        LoanMasterDTO loanDTO = new LoanMasterDTO();
        IActionResult actionResult = await _loanAPIController.GetRepaymentLoanDetails(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"), Employee, Month, Year, Status);
        ObjectResult objResult = (ObjectResult)actionResult;

        loanDTO.RepaymentStartYear = Year;
        loanDTO.RepaymentStartMonth = Month;
        loanDTO.EmployeeId = Employee;
        loanDTO.Status = Status;
        loanDTO.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
        // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
        loanDTO.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
        loanDTO.EmployeeMasterList = await GetEmployeeList();
        loanDTO.GetSanctionLoan = await GetSanctionLoan();
        //  loanDTO.GetLoanPaymentDetails = await GetEmployeeLoan();

        foreach (var item in loanDTO.GetSanctionLoan)
        {
            item.Employee = loanDTO.EmployeeMasterList.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.EmployeeName).FirstOrDefault();
        }



        List<LoanPaymentDetailDTO> objResultData = (List<LoanPaymentDetailDTO>)objResult.Value;
        loanDTO.GetLoanPaymentDetails = objResultData;
        foreach (var item in objResultData)
        {
            item.EncryptedId = CommonHelper.EncryptURLHTML(item.RepaymentId.ToString());
            // item.Employee = employees.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.Employee).FirstOrDefault();
            // item.Employee = employeeSalaryTemplateMappingVM.EmployeeKeyValues.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.EmployeeName).FirstOrDefault();
        }
        // return objResultData;
        return View("LoanRepayment", loanDTO);
    }

    [HttpPost]
    public async Task<IActionResult> RepaymentLoanAmountUpdate(RepaymentLoanInputs userAction)
    {
        string? employeeId = HttpContext.Session.GetString("EmployeeId");
       // var sRetMsg = null;
        //int? unitId = HttpContext.Session.GetInt32("UnitId");
        // userAction.UnitId = HttpContext.Session.GetInt32("UnitId");
        userAction.ModifiedBy = employeeId;
        userAction.ModifiedOn = DateTime.Now;

        //  SalaryParameters manualPunchVM = new SalaryParameters();
        //   userAction.ApprovedBy = employeeId;
       
            var sRetMsg = await _loanAPIController.UpdateRepaymentLoan(userAction);
            // manualPunchVM.DisplayMessage = sRetMsg.;
        

        return sRetMsg;
        //else
        //  manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
        // return Ok(manualPunchVM.DisplayMessage);
    }

    [HttpGet]
    [Route("Loan/GetEmployeeLoanInfo/{eloanId}")]
    public async Task<IActionResult> GetEmployeeLoanInfo(string eloanId)
    {
        int sanctionId = 0;
        try
        {
            sanctionId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eloanId));
        }
        catch (Exception ex)
        {
            return RedirectToAction("Login", "Account");
        }
        if (sanctionId != 0)
        {
            LoanMasterDTO outputData = new LoanMasterDTO();
            outputData.SanctionId = sanctionId;

            IActionResult actionResult;

            actionResult = await _loanAPIController.GetEmployeeLoan(outputData);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = (LoanMasterDTO)objResult.Value;
            if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
            {
                return View("LoanSanctionMaster", objResultData);
                //return RedirectToAction("Role","Role", objResultData);
            }
            
        }
        return RedirectToAction("LoanSanctionMaster", "Loan");
    }

    [HttpPost]
    public FileResult ExportLoanSanction(string LoanSanction)
    {
        return File(Encoding.ASCII.GetBytes(LoanSanction), "application/vnd.ms-excel", "LoanSanctionList.xls");
    }
}

