using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Endpoints.StatutoryComponent;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.ITDeclaration;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Infrastructure.Models.StatutoryComponent;
using SimpliHR.Services.DBContext;
using SimpliHR.WebUI.BL;
using SimpliHR.WebUI.Modals.ITDeclarations;
using System.Linq.Expressions;
using SimpliHR.Endpoints.ExcelUploads;
using DocumentFormat.OpenXml.Bibliography;
using SimpliHR.Endpoints.Leave;
using SimpliHR.Infrastructure.Models.Leave;
using Simpli2._0.Pages.PayrollAccount;
using System.IO.Compression;
using System.Diagnostics;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;
using SimpliHR.Infrastructure.Models.KeyValue;

namespace SimpliHR.WebUI.Controllers.Payroll;

public class PayrollAccountController : Controller
{
    private readonly ITDeclarationAPIController _iTDeclarationAPIController;
    private readonly Investments80CAPIController _investments80CAPIController;
    private readonly Exemptions80DAPIController _exemptions80DAPIController;
    private readonly OtherInvestmentsExemptionsAPIController _otherInvestmentsExemptionsAPIController;
    private readonly ExcelUDAPIController _excelUDAPIController;
    private readonly LoanAPIController _loanAPIController;
    private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnv;
    private readonly MastersKeyValueController _masterKeyValue;
    public PayrollAccountController(ITDeclarationAPIController iTDeclarationAPIController, Investments80CAPIController investments80CAPIController, Exemptions80DAPIController exemptions80DAPIController, OtherInvestmentsExemptionsAPIController otherInvestmentsExemptionsAPIController, ExcelUDAPIController excelUDAPIController, LoanAPIController loanAPIController, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnv,MastersKeyValueController masterKeyValue)
    {
        _iTDeclarationAPIController = iTDeclarationAPIController;
        _investments80CAPIController = investments80CAPIController;
        _exemptions80DAPIController = exemptions80DAPIController;
        _otherInvestmentsExemptionsAPIController = otherInvestmentsExemptionsAPIController;
        _excelUDAPIController = excelUDAPIController;
        _loanAPIController = loanAPIController;
        _hostingEnv = hostingEnv;
        _masterKeyValue = masterKeyValue;
    }



    [Route("/PayrollAccount/ITDeclarations/{FYRegime}")]
    public async Task<IActionResult> ITDeclarations(string FYRegime)
    {
        string FY = FYRegime.Split("_")[0];
        string Regime = FYRegime.Split("_")[1];
        EmployeeMasterDTO employee = JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee"));
        if (employee == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        if (employee.UnitId == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        if (employee.EmployeeId == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        ITDeclarationsViewModel iTDeclarationsViewModel = await GetDeclarationsForIncomeTax(FY, Regime);
        iTDeclarationsViewModel.FY = FY;
        iTDeclarationsViewModel.Regime = Regime;
        return View(iTDeclarationsViewModel);
    }

    public async Task<ITDeclarationsViewModel> GetDeclarationsForIncomeTax(string FY, string Regime)
    {
        EmployeeMasterDTO employee = JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee"));
        ITDeclarationsViewModel iTDeclarationsViewModel = new ITDeclarationsViewModel();
        var objResHouseRentDetails = await _iTDeclarationAPIController.GetHouseRentDetails(employee.UnitId ?? default(int), employee.EmployeeId, FY, Regime);
        if (objResHouseRentDetails != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResHouseRentDetails).StatusCode == 200)
            {
                var resHouseRentDetails = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResHouseRentDetails).Value;
                if (resHouseRentDetails != null)
                {
                    iTDeclarationsViewModel.ItDeclarationHouseRentDetailList = (List<ItDeclarationHouseRentDetailDTO>)resHouseRentDetails;
                }
            }
        }
        var objResHomeLoneDetails = await _iTDeclarationAPIController.GetHomeLoneDetails(employee.UnitId ?? default(int), employee.EmployeeId, FY, Regime);
        if (objResHomeLoneDetails != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResHomeLoneDetails).StatusCode == 200)
            {
                var resHomeLoneDetails = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResHomeLoneDetails).Value;
                if (resHomeLoneDetails != null)
                {
                    iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO = (ItDeclarationHomeLoanDetailDTO)resHomeLoneDetails;
                }
            }
        }
        var objResLentOutPropertyDetail = await _iTDeclarationAPIController.GetLentOutPropertyDetails(employee.UnitId ?? default(int), employee.EmployeeId, FY, Regime);
        if (objResLentOutPropertyDetail != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResLentOutPropertyDetail).StatusCode == 200)
            {
                var resLentOutPropertyDetails = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResLentOutPropertyDetail).Value;
                if (resLentOutPropertyDetails != null)
                {
                    iTDeclarationsViewModel.ItDeclarationLentOutPropertyDetailList = (List<ItDeclarationLentOutPropertyDetailDTO>)resLentOutPropertyDetails;
                }
            }
        }
        var objResOtherSourcesOfIncomeDetail = await _iTDeclarationAPIController.GetOtherSourcesOfIncomeDetails(employee.UnitId ?? default(int), employee.EmployeeId, FY, Regime);
        if (objResOtherSourcesOfIncomeDetail != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResOtherSourcesOfIncomeDetail).StatusCode == 200)
            {
                var resOtherSourcesOfIncomeDetails = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResOtherSourcesOfIncomeDetail).Value;
                if (resOtherSourcesOfIncomeDetails != null)
                {
                    iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO = (ItDeclarationOtherSourceOfIncomeDTO)resOtherSourcesOfIncomeDetails;
                }
            }
        }
        var objResInvestments80C = await _iTDeclarationAPIController.GetInvestments80C(employee.UnitId ?? default(int), employee.EmployeeId, FY, Regime);
        if (objResInvestments80C != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResInvestments80C).StatusCode == 200)
            {
                var resInvestments80C = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResInvestments80C).Value;
                if (resInvestments80C != null)
                {
                    iTDeclarationsViewModel.ItDeclaration80CinvestmentList = (List<ItDeclaration80CinvestmentDTO>)resInvestments80C;
                }
            }
        }
        var objResExemptions80D = await _iTDeclarationAPIController.GetExemptions80D(employee.UnitId ?? default(int), employee.EmployeeId, FY, Regime);
        if (objResExemptions80D != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResExemptions80D).StatusCode == 200)
            {
                var resExemptions80D = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResExemptions80D).Value;
                if (resExemptions80D != null)
                {
                    iTDeclarationsViewModel.ItDeclaration80DexemptionList = (List<ItDeclaration80DexemptionDTO>)resExemptions80D;
                }
            }
        }
        var objResOtherInvestmentExemption = await _iTDeclarationAPIController.GetOtherInvestmentExemptions(employee.UnitId ?? default(int), employee.EmployeeId, FY, Regime);
        if (objResOtherInvestmentExemption != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResOtherInvestmentExemption).StatusCode == 200)
            {
                var resOtherInvestmentExemption = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResOtherInvestmentExemption).Value;
                if (resOtherInvestmentExemption != null)
                {
                    iTDeclarationsViewModel.ItDeclarationOtherInvestmentExemptionList = (List<ItDeclarationOtherInvestmentExemptionDTO>)resOtherInvestmentExemption;
                }
            }
        }
        var objResPreviousEmploymentDetails = await _iTDeclarationAPIController.GetPreviousEmploymentDetails(employee.UnitId ?? default(int), employee.EmployeeId, FY, Regime);
        if (objResPreviousEmploymentDetails != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResPreviousEmploymentDetails).StatusCode == 200)
            {
                var resPreviousEmploymentDetails = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResPreviousEmploymentDetails).Value;
                if (resPreviousEmploymentDetails != null)
                {
                    iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO = (ItDeclarationPreviousEmployementDTO)resPreviousEmploymentDetails;
                }
            }
        }
        var objResInvestments80CMaster = await _investments80CAPIController.GetInvestments80C();
        if (objResInvestments80CMaster != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResInvestments80CMaster).StatusCode == 200)
            {
                var resInvestments80CMaster = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResInvestments80CMaster).Value;
                if (resInvestments80CMaster != null)
                {
                    iTDeclarationsViewModel.Investment80CmasterDTOs = (List<Investment80CmasterDTO>)resInvestments80CMaster;
                }
            }
        }
        var objResExemptions80DMaster = await _exemptions80DAPIController.GetExemptions80D();
        if (objResExemptions80DMaster != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResExemptions80DMaster).StatusCode == 200)
            {
                var resExemptions80DMaster = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResExemptions80DMaster).Value;
                if (resExemptions80DMaster != null)
                {
                    iTDeclarationsViewModel.exemptions80DDTOs = (List<Exemptions80DDTO>)resExemptions80DMaster;
                }
            }
        }
        var objResOthersExemptionsInvestmentsMaster = await _otherInvestmentsExemptionsAPIController.GetOthersExemptionsInvestments();
        if (objResOthersExemptionsInvestmentsMaster != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResOthersExemptionsInvestmentsMaster).StatusCode == 200)
            {
                var resOthersExemptionsInvestmentsMaster = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResOthersExemptionsInvestmentsMaster).Value;
                if (resOthersExemptionsInvestmentsMaster != null)
                {
                    iTDeclarationsViewModel.otherInvestmentExemptionDTOs = (List<OtherInvestmentExemptionDTO>)resOthersExemptionsInvestmentsMaster;
                }
            }
        }
        return iTDeclarationsViewModel;
    }

    public async Task<IActionResult> GetHouseRentDetailsPartialView([FromBody] ItDeclarationHouseRentDetailDTO itDeclarationHouseRentDetailDTO)
    {
        return PartialView("_ITDeclaration/_HouseRentDetailsForm", itDeclarationHouseRentDetailDTO);
    }
    public async Task<IActionResult> GetInvestments80CPartialView()
    {
        ItDeclaration80CinvestmentDTO itDeclaration80CinvestmentDTO = new ItDeclaration80CinvestmentDTO();

        var objResInvestments80CMaster = await _investments80CAPIController.GetInvestments80C();
        if (objResInvestments80CMaster != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResInvestments80CMaster).StatusCode == 200)
            {
                var resInvestments80CMaster = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResInvestments80CMaster).Value;
                if (resInvestments80CMaster != null)
                {
                    itDeclaration80CinvestmentDTO.Investment80CmasterDTOs = (List<Investment80CmasterDTO>)resInvestments80CMaster;
                }
            }
        }
        return PartialView("_ITDeclaration/_Investments80C", itDeclaration80CinvestmentDTO);
    }
    public async Task<IActionResult> GetExemptions80DPartialView()
    {
        ItDeclaration80DexemptionDTO itDeclaration80DexemptionDTO = new ItDeclaration80DexemptionDTO();

        var objResExemptions80DMaster = await _exemptions80DAPIController.GetExemptions80D();
        if (objResExemptions80DMaster != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResExemptions80DMaster).StatusCode == 200)
            {
                var resExemptions80DMaster = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResExemptions80DMaster).Value;
                if (resExemptions80DMaster != null)
                {
                    itDeclaration80DexemptionDTO.exemptions80DDTOs = (List<Exemptions80DDTO>)resExemptions80DMaster;
                }
            }
        }
        return PartialView("_ITDeclaration/_Exemptions80D", itDeclaration80DexemptionDTO);
    }
    public async Task<IActionResult> GetOtherInvestmentsExemptionsPartialView()
    {
        ItDeclarationOtherInvestmentExemptionDTO itDeclarationOtherInvestmentExemptionDTO = new ItDeclarationOtherInvestmentExemptionDTO();

        var objResOthersExemptionsInvestmentsMaster = await _otherInvestmentsExemptionsAPIController.GetOthersExemptionsInvestments();
        if (objResOthersExemptionsInvestmentsMaster != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)objResOthersExemptionsInvestmentsMaster).StatusCode == 200)
            {
                var resOthersExemptionsInvestmentsMaster = ((Microsoft.AspNetCore.Mvc.ObjectResult)objResOthersExemptionsInvestmentsMaster).Value;
                if (resOthersExemptionsInvestmentsMaster != null)
                {
                    itDeclarationOtherInvestmentExemptionDTO.otherInvestmentExemptionDTOs = (List<OtherInvestmentExemptionDTO>)resOthersExemptionsInvestmentsMaster;
                }
            }
        }
        return PartialView("_ITDeclaration/_OtherInvestmentsExemptions", itDeclarationOtherInvestmentExemptionDTO);
    }

    [Route("/PayrollAccount/ProofSubmission/{FYRegime}")]
    public async Task<IActionResult> ProofSubmission(string FYRegime)
    {
        string FY = FYRegime.Split("_")[0];
        string Regime = FYRegime.Split("_")[1];
        EmployeeMasterDTO employee = JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee"));
        if (employee == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        if (employee.UnitId == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        if (employee.EmployeeId == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        ITDeclarationsViewModel iTDeclarationsViewModel = await GetDeclarationsForIncomeTax(FY, Regime);
        iTDeclarationsViewModel.FY = FY;
        iTDeclarationsViewModel.Regime = Regime;
        return View(iTDeclarationsViewModel);
    }
    public async Task<IActionResult> PayrollAccount()
    {
        string Base64String = string.Empty;
        SalaryProcessInputs input = new SalaryProcessInputs();
        string? employeeId = HttpContext.Session.GetString("EmployeeId");
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        input.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
        // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
        input.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
        IActionResult actionResult = await _loanAPIController.GetEmployeeSanctionLoanDetails(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId, Convert.ToInt32(employeeId));
        //IActionResult actionResult = await _loanAPIController.EmployeeLoan(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 },Convert.ToInt32(employeeId));
        ObjectResult objResult = (ObjectResult)actionResult;
        //List<LoanPaymentDetailDTO> objResultData = (List<LoanPaymentDetailDTO>)objResult.Value;
        List<LoanMasterDTO> objResultData = (List<LoanMasterDTO>)objResult.Value;
        //input.GetLoanPaymentDetails = objResultData;
        input.GetSanctionLoan = objResultData;

        input.Form16Doc.EmployeeId = Convert.ToInt32(employeeId);
        //form16VM.Form16Doc.FinYear = "2024-2025";
        input = await _excelUDAPIController.Form16EmployeeSearch(input);
        foreach (var item in input.Form16DocList)
        {
            Base64String = CommonHelper.GetBase64String("pdf");
            item.AttachmentBase64String = Base64String + Convert.ToBase64String(item.DocAttachment, 0, item.DocAttachment.Length);
        }

        // List<EmployeesSalaryProcessDetails> objResultData = new List<EmployeesSalaryProcessDetails>();

        // //objResultData.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
        // //objResultData.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
        // int _loginId;

        // int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
        // int? unitId = HttpContext.Session.GetInt32("UnitId");
        // input.UnitId = unitId;
        //input.EmployeeId = loginId;
        //// input.EmployeeId = 152;
        // input.Month = 12;
        // input.Year = 2023;
        // objResultData = await _excelUDAPIController.GetSalaryDetails(input);
        return View(input);
    }

    [HttpPost]
    public ActionResult TDSSheet(int? ProcessYear, int? ProcessMonth)
    {
        //SalaryProcessInputs obj = new SalaryProcessInputs();
        //obj.Year = ProcessYear;
        //obj.Month = ProcessMonth;
        HttpContext.Session.SetInt32("Year", ProcessYear ?? default(int));
        HttpContext.Session.SetInt32("Month", ProcessMonth ?? default(int));
        //var urlquerstr = @Url.Action("TDSSheetDownload", "PayrollSalary");
        //  window.open(urlquerstr);

        // return RedirectToAction(urlquerstr);
        var routeValue = new RouteValueDictionary 
            (new { action = "TDSSheetDownload", controller = "PayrollSalary" });
        return RedirectToRoute(routeValue);
        // HttpContext.Current.Response.Write(@"<script type='text/javascript' language='javascript'>window.open('page.html','_blank').focus();</script>");
        // return View("TDSSheet");
    }

        [HttpGet]
    public async Task<List<EmployeesSalaryProcessDetails>> GetSalaryDetails()
    {
        SalaryProcessInputs input = new SalaryProcessInputs();
        List<EmployeesSalaryProcessDetails> objResultData = new List<EmployeesSalaryProcessDetails>();
        int _loginId;

        int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        input.UnitId = unitId;
        input.EmployeeId = loginId;
        input.Month = 12;
        input.Year = 2023;
        objResultData = await _excelUDAPIController.GetSalaryDetails(input);

        return objResultData;
    }


    [HttpPost]
    public IActionResult SaveITDeclarations([FromBody] ITDeclarationsViewModel iTDeclarationsViewModel)
    {
        try
        {
            string error = "";
            string success = "";
            EmployeeMasterDTO employee = JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee"));
            if (employee == null)
            {
                return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
            }
            if (employee.UnitId == null)
            {
                return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
            }
            if (employee.EmployeeId == null)
            {
                return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
            }
            try
            {
                foreach (var item in iTDeclarationsViewModel.ItDeclarationHouseRentDetailList)
                {
                    item.UnitId = employee.UnitId;
                    item.EmployeeId = employee.EmployeeId;
                    item.EmployeeCode = employee.EmployeeCode;
                    item.FY = iTDeclarationsViewModel.FY;
                    item.Regime = iTDeclarationsViewModel.Regime;
                    item.IsActive = true;
                    var res = _iTDeclarationAPIController.SaveHouseRentDetails(item);
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
                    {
                        error += " " + ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString() + ".";
                    }
                }
            }
            catch (Exception exHouseRentDetails)
            {
                error += " " + exHouseRentDetails.Message + ".";
            }
            try
            {
                iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO.UnitId = employee.UnitId;
                iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO.EmployeeId = employee.EmployeeId;
                iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO.EmployeeCode = employee.EmployeeCode;
                iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO.FY = iTDeclarationsViewModel.FY;
                iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO.Regime = iTDeclarationsViewModel.Regime;
                iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO.IsActive = true;
                var res = _iTDeclarationAPIController.SaveHomeLoanDetails(iTDeclarationsViewModel.ItDeclarationHomeLoanDetailDTO);
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
                {
                    error += " " + ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString() + ".";
                }
            }
            catch (Exception exHomeLoadDetails)
            {
                error += " " + exHomeLoadDetails.Message + ".";
            }
            try
            {
                foreach (var item in iTDeclarationsViewModel.ItDeclarationLentOutPropertyDetailList)
                {
                    item.UnitId = employee.UnitId;
                    item.EmployeeId = employee.EmployeeId;
                    item.EmployeeCode = employee.EmployeeCode;
                    item.FY = iTDeclarationsViewModel.FY;
                    item.Regime = iTDeclarationsViewModel.Regime;
                    item.IsActive = true;
                    var res = _iTDeclarationAPIController.SaveLentOutPropertyDetails(item);
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
                    {
                        error += " " + ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString() + ".";
                    }
                }
            }
            catch (Exception exHomeLoadDetails)
            {
                error += " " + exHomeLoadDetails.Message + ".";
            }
            try
            {
                iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO.UnitId = employee.UnitId;
                iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO.EmployeeId = employee.EmployeeId;
                iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO.EmployeeCode = employee.EmployeeCode;
                iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO.FY = iTDeclarationsViewModel.FY;
                iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO.Regime = iTDeclarationsViewModel.Regime;
                iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO.IsActive = true;

                var res = _iTDeclarationAPIController.SaveOtherSourceOfIncomeDetails(iTDeclarationsViewModel.ItDeclarationOtherSourceOfIncomeDTO);
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
                {
                    error += " " + ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString() + ".";
                }
            }
            catch (Exception exHomeLoadDetails)
            {
                error += " " + exHomeLoadDetails.Message + ".";
            }
            try
            {
                foreach (var item in iTDeclarationsViewModel.ItDeclaration80CinvestmentList)
                {
                    item.UnitId = employee.UnitId;
                    item.EmployeeId = employee.EmployeeId;
                    item.EmployeeCode = employee.EmployeeCode;
                    item.FY = iTDeclarationsViewModel.FY;
                    item.Regime = iTDeclarationsViewModel.Regime;
                    item.IsActive = true;
                    var res = _iTDeclarationAPIController.Save80CInvestmentDetails(item);
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
                    {
                        error += " " + ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString() + ".";
                    }
                }
            }
            catch (Exception exHomeLoadDetails)
            {
                error += " " + exHomeLoadDetails.Message + ".";
            }
            try
            {
                foreach (var item in iTDeclarationsViewModel.ItDeclaration80DexemptionList)
                {
                    item.UnitId = employee.UnitId;
                    item.EmployeeId = employee.EmployeeId;
                    item.EmployeeCode = employee.EmployeeCode;
                    item.FY = iTDeclarationsViewModel.FY;
                    item.Regime = iTDeclarationsViewModel.Regime;
                    item.IsActive = true;
                    var res = _iTDeclarationAPIController.Save80DExemptionDetails(item);
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
                    {
                        error += " " + ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString() + ".";
                    }
                }
            }
            catch (Exception exHomeLoadDetails)
            {
                error += " " + exHomeLoadDetails.Message + ".";
            }
            try
            {
                foreach (var item in iTDeclarationsViewModel.ItDeclarationOtherInvestmentExemptionList)
                {
                    item.UnitId = employee.UnitId;
                    item.EmployeeId = employee.EmployeeId;
                    item.EmployeeCode = employee.EmployeeCode;
                    item.FY = iTDeclarationsViewModel.FY;
                    item.Regime = iTDeclarationsViewModel.Regime;
                    item.IsActive = true;
                    var res = _iTDeclarationAPIController.SaveOtherInvestmentExemptionDetails(item);
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
                    {
                        error += " " + ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString() + ".";
                    }
                }
            }
            catch (Exception exHomeLoadDetails)
            {
                error += " " + exHomeLoadDetails.Message + ".";
            }
            try
            {
                iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO.UnitId = employee.UnitId;
                iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO.EmployeeId = employee.EmployeeId;
                iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO.EmployeeCode = employee.EmployeeCode;
                iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO.FY = iTDeclarationsViewModel.FY;
                iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO.Regime = iTDeclarationsViewModel.Regime;
                iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO.IsActive = true;
                var res = _iTDeclarationAPIController.SavePreviousEmployementDetails(iTDeclarationsViewModel.ItDeclarationPreviousEmployementDTO);
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
                {
                    error += " " + ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString() + ".";
                }
            }
            catch (Exception exHomeLoadDetails)
            {
                error += " " + exHomeLoadDetails.Message + ".";
            }
            return Ok(error);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    public async Task<IActionResult> SaveITDeclarationType([FromBody] ItDeclarationTypeDTO inputDTO)
    {
        EmployeeMasterDTO? employee = JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee"));

        if (employee == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        if (employee.UnitId == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        if (employee.EmployeeId == null)
        {
            return StatusCode(StatusCodes.Status419AuthenticationTimeout, "Session Timed Out");
        }
        inputDTO.EmployeeId = employee.EmployeeId;
        inputDTO.UnitId = employee.UnitId;
        inputDTO.EmployeeCode = employee.EmployeeCode;
        inputDTO.IsActive = 1;
        inputDTO.CreatedDate = System.DateTime.Now;

        var res = _iTDeclarationAPIController.SaveItDeclarationType(inputDTO);
        return res;

    }

    public async Task<IActionResult> DeleteHouseRentDetail([FromBody] ItDeclarationHouseRentDetailDTO inputDTO)
    {
        if (inputDTO.ItDeclarationHouseRentDetailsId != 0)
        {
            var res = await _iTDeclarationAPIController.DeleteHouseRentDetail(inputDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> DeleteLentOutPropertyDetail([FromBody] ItDeclarationLentOutPropertyDetailDTO inputDTO)
    {
        if (inputDTO.ItDeclarationLentOutPropertyDetailsId != 0)
        {
            var res = await _iTDeclarationAPIController.DeleteLentOutPropertyDetail(inputDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> DeleteInvestments80C([FromBody] ItDeclaration80CinvestmentDTO inputDTO)
    {
        if (inputDTO.ItDeclaration80CinvestmentsId != 0)
        {
            var res = await _iTDeclarationAPIController.DeleteInvestments80C(inputDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> DeleteExemptions80D([FromBody] ItDeclaration80DexemptionDTO inputDTO)
    {
        if (inputDTO.ItDeclaration80DexemptionsId != 0)
        {
            var res = await _iTDeclarationAPIController.DeleteExemptions80D(inputDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }
    public async Task<IActionResult> DeleteOtherInvestmentsExemptions([FromBody] ItDeclarationOtherInvestmentExemptionDTO inputDTO)
    {
        if (inputDTO.ItDeclarationOtherInvestmentExemptionId != 0)
        {
            var res = await _iTDeclarationAPIController.DeleteOtherInvestmentsExemptions(inputDTO);
            return res;
        }
        else
        {
            return StatusCode(StatusCodes.Status204NoContent, "No Record Selected to delete");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadITDeclartionProof(ITDeclarationProofDocument formData)

    {

        if (formData != null)
        {
            if (formData.ItDeclarationParticular != null)
            {
                if (formData.ItDeclarationParticularId != null)
                {
                    if (formData.DocumentProofFile != null)
                    {
                        if (formData.DocumentProofFile.Length > 0)
                        {
                            formData.DocumentName = Path.GetFileName(formData.DocumentProofFile.FileName);
                            formData.DocumentExtension = Path.GetExtension(formData.DocumentName);
                            using (var target = new MemoryStream())
                            {
                                formData.DocumentProofFile.CopyTo(target);
                                formData.DocumentProofByte = target.ToArray();
                            }
                            var res = await _iTDeclarationAPIController.UploadITDeclarationProof(formData);
                            return res;
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "No document found");
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Please upload proof");
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No record found");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error has occurred. Please refresh the page and try again");
            }
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Some error has occurred. Please refresh the page and try again");
        }

    }


    [HttpGet]
    public async Task<LoanMasterDTO> GetLoanDetails(int SanctionId)
    {
        LoanMasterDTO EmployeeLoanDetail = new LoanMasterDTO();
        // EmployeeLoanDetail = await _loanAPIController.EmployeeLoan(SanctionId);
        string? employeeId = HttpContext.Session.GetString("EmployeeId");
        IActionResult actionResult = await _loanAPIController.EmployeeLoan(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, Convert.ToInt32(employeeId), SanctionId);
        ObjectResult objResult = (ObjectResult)actionResult;
        List<LoanPaymentDetailDTO> objResultData = (List<LoanPaymentDetailDTO>)objResult.Value;
        // List<LoanMasterDTO> objResultData = (List<LoanMasterDTO>)objResult.Value;
        EmployeeLoanDetail.GetLoanPaymentDetails = objResultData;


        return EmployeeLoanDetail;
    }
    [HttpGet]
    public IActionResult Form16Upload()
    {
        Fom16DocViewModel form16VM = new Fom16DocViewModel();
        return View(form16VM);
    }

    [HttpPost]
    public async Task<Fom16DocViewModel> Form16Upload(Form16DocDTO form16DocDTO)
    {
        Fom16DocViewModel form16VM = new Fom16DocViewModel();
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        int employeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        form16DocDTO.CreatedBy = employeeId;
        form16DocDTO.ModifiedBy = employeeId;
        form16DocDTO.CreatedOn = DateTime.Now;
        form16DocDTO.ModifiedOn = DateTime.Now;
        
        form16VM.Form16Doc = form16DocDTO;
        var form16Zip = form16DocDTO.DocAttachmentFile;
        byte[] decodedByteArray;
        if (form16Zip != null)
        {
            if (form16Zip.Length > 0)
            {
                var zipFile = form16Zip.FileName;
                if (Path.GetExtension(zipFile).ToLower() != ".zip" && Path.GetExtension(zipFile).ToLower() != ".rar")
                    form16VM.DisplayMessage = "Not a valid zip file";
                else
                {
                    //System.IO.Compression;
                    string unZipPath = Path.Combine(_hostingEnv.WebRootPath, "Docs", unitId.ToString(), employeeId.ToString());
                    if (Directory.Exists(unZipPath))
                    {
                        Directory.Delete(unZipPath, true);
                    }
                    if (!Directory.Exists(unZipPath))
                        Directory.CreateDirectory(unZipPath);
                    
                    var unZipFilePath = Path.Combine(unZipPath, form16Zip.FileName);
                    using (FileStream fs = System.IO.File.Create(unZipFilePath))
                    {
                        form16Zip.CopyTo(fs);
                    }
                    
                    using (var target = new MemoryStream())
                    {
                        form16Zip.CopyTo(target);
                        form16DocDTO.DocAttachment = target.ToArray();
                        //ZipFile.ExtractToDirectory(unZipFilePath,unZipPath,true);
                        string unZipFilesLocation = Path.Combine(unZipPath, Path.GetFileNameWithoutExtension(zipFile));
                        Directory.CreateDirectory(unZipFilesLocation);
                        using (var archive = ArchiveFactory.Open(unZipFilePath))
                        {
                            archive.WriteToDirectory(unZipFilesLocation);
                        }
                        form16VM = await _excelUDAPIController.SaveForm16Data(unZipFilesLocation, form16VM);
                    }
                }
                    
            }
        }
        return form16VM;
    }


    [HttpGet]
    //public async Task<IActionResult> Form16EmployeeSearch(string eid)
    //{
    //    string Base64String = string.Empty;
    //  //  Fom16DocViewModel form16VM = new Fom16DocViewModel();
    //    SalaryProcessInputs form16VM = new SalaryProcessInputs();
    //    int employeeId;
    //    int? unitId = HttpContext.Session.GetInt32("UnitId");
    //    if (eid != "")
    //    {
    //        employeeId = Convert.ToInt32(CommonHelper.Decrypt(eid));
    //        form16VM.Form16Doc.EmployeeId = employeeId;
    //        //form16VM.Form16Doc.FinYear = "2024-2025";
    //        form16VM = await _excelUDAPIController.Form16EmployeeSearch(form16VM);
    //        foreach (var item in form16VM.Form16DocList)
    //        {
    //            Base64String = CommonHelper.GetBase64String("pdf");
    //            item.AttachmentBase64String = Base64String + Convert.ToBase64String(item.DocAttachment, 0, item.DocAttachment.Length);
    //        }
    //    }
    //    return form16VM;
    //}

    [HttpGet]
    public async Task<IActionResult> Form16Search()
    {
        Fom16DocViewModel form16VM = new Fom16DocViewModel();
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        //int employeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        form16VM.EmployeeKeyValues = await _masterKeyValue.ConfirmedEmployeeKeyValue(x => x.IsActive == true && x.UnitId == unitId);
        return View(form16VM);
    }

    [HttpPost]
    public async Task<Fom16DocViewModel> Form16Search(Form16DocDTO form16DocDTO)
    {
        string Base64String=string.Empty;
        Fom16DocViewModel form16VM = new Fom16DocViewModel();
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        int employeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        form16VM.Form16Doc = form16DocDTO;
        form16VM = await _excelUDAPIController.Form16Search(form16VM);
        foreach(var item in form16VM.Form16DocList)
        {
            Base64String = CommonHelper.GetBase64String("pdf");
            item.AttachmentBase64String = Base64String + Convert.ToBase64String(item.DocAttachment, 0, item.DocAttachment.Length);
        }
            

        return form16VM;
    }

    [HttpPost]
    public async Task<Fom16DocViewModel> DeleteForm16(Form16DocDTO form16DocDTO)
    {
        if(form16DocDTO.FormId!=0)
        {
            await _excelUDAPIController.DeleteForm16(form16DocDTO.FormId);
        }
        string Base64String = string.Empty;
        Fom16DocViewModel form16VM = new Fom16DocViewModel();
        int? unitId = HttpContext.Session.GetInt32("UnitId");
        int employeeId = Convert.ToInt32(HttpContext.Session.GetString("EmployeeId"));
        form16VM.Form16Doc = form16DocDTO;
        form16VM = await _excelUDAPIController.Form16Search(form16VM);
        foreach (var item in form16VM.Form16DocList)
        {
            Base64String = CommonHelper.GetBase64String("pdf");
            item.AttachmentBase64String = Base64String + Convert.ToBase64String(item.DocAttachment, 0, item.DocAttachment.Length);
        }


        return form16VM;
    }

}

