using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Data;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Core.Entities;
using Microsoft.Extensions.Caching.Memory;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using HtmlAgilityPack;
using System.Data.SqlClient;
using SimpliHR.Endpoints.ExcelUploads;
using System.Text;
using System.Diagnostics;
using DocumentFormat.OpenXml.Bibliography;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Infrastructure.Models.ClientManagement;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace SimpliHR.WebUI.Controllers.Payroll
{
    public class PayrollSalaryController : Controller
    {
        private readonly EarningComponentAPIController _payrollController;
        private readonly MastersKeyValueController _mastersKeyValueController;
        private readonly ExcelUDAPIController _excelUDAPIController;
        private readonly EmployeeMasterController _employeeAPIController;
        private readonly ClientController _ClientController;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        public PayrollSalaryController(MastersKeyValueController mastersKeyValueController, EarningComponentAPIController leaveAPIController, ExcelUDAPIController excelUDAPIController, EmployeeMasterController EmployeeAPIController, ClientController ClientManagementController, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _mastersKeyValueController = mastersKeyValueController;
            _payrollController = leaveAPIController;
            _excelUDAPIController = excelUDAPIController;
            _employeeAPIController = EmployeeAPIController;
            _ClientController = ClientManagementController;
            _memoryCache = memoryCache;
            _configuration = configuration;

        }


        public async Task<IActionResult> EmployeeSalaryTemplateMapping()
        {
            EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM;
            employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
            employeeSalaryTemplateMappingVM.ViewScreen = "List";
            employeeSalaryTemplateMappingVM.UnitId = HttpContext.Session.GetInt32("UnitId");
            employeeSalaryTemplateMappingVM.EmployeeKeyValues = await _mastersKeyValueController.EmployeeKeyValue(p => p.IsActive == true && p.InfoFillingStatus == 1);
            employeeSalaryTemplateMappingVM.DepartmentKeyValues = await _mastersKeyValueController.DepartmentKeyValue(x => x.IsActive == true && x.UnitId == employeeSalaryTemplateMappingVM.UnitId);
            employeeSalaryTemplateMappingVM.SalaryTemplateKeyValues = await _mastersKeyValueController.SalaryTemplateKeyValue(p => (p.IsActive == true));

            employeeSalaryTemplateMappingVM = await _payrollController.GetEmployeeSalaryTemplateMappingList(employeeSalaryTemplateMappingVM, 1000, 0);
            if (employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMappingList != null)
            {
                foreach (var item in employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMappingList)
                {
                    item.EncryptedEmployeeSalaryTemplateId = CommonHelper.EncryptURLHTML(item.EmployeeSalaryTemplateId.ToString());
                }
            }
            return View("EmployeeSalaryTemplateMapping", employeeSalaryTemplateMappingVM);
        }

        public async Task<EmployeeSalaryTemplateMappingViewModel> SaveEmployeeSalaryTemplateMapping(EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM)
        {

            //employeeSalaryTemplateMappingVM.EmployeeKeyValues = await _mastersKeyValueController.EmployeeKeyValue();
            //employeeSalaryTemplateMappingVM.DepartmentKeyValues = await _mastersKeyValueController.DepartmentKeyValue();
            employeeSalaryTemplateMappingVM.UnitId = employeeSalaryTemplateMappingVM.UnitId;
            var departmentId = employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMapping.DepartmentId;
            if (employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMapping.MappingEmployeeIds == null)
                employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateMapping.MappingEmployeeIds = string.Join(",", _mastersKeyValueController.EmployeeKeyValue(x => (departmentId == null ? x.IsActive == true : (x.DepartmentId == departmentId && x.IsActive == true)) && x.InfoFillingStatus == 1).Result.Select(x => x.EmployeeId));

            employeeSalaryTemplateMappingVM = await _payrollController.SaveEmployeeSalaryTemplateMapping(employeeSalaryTemplateMappingVM);
            //employeeSalaryTemplateMappingVM.ViewScreen = "Add";
            //employeeSalaryTemplateMappingVM.SalaryTemplateKeyValues = await _mastersKeyValueController.SalaryTemplateKeyValues(p => (p.IsActive == true));
            return employeeSalaryTemplateMappingVM;
        }

        [HttpGet]
        [Route("PayrollSalary/GetEmployeeSalaryTemplateMapping/{id}")]
        public async Task<IActionResult> GetEmployeeSalaryTemplateMapping(string id)
        {
            EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
            string _employeeSalaryTemplateId = CommonHelper.Decrypt(id);
            int employeeSalaryTemplateId;

            if (CommonHelper.IsNumeric(_employeeSalaryTemplateId))
            {
                int.TryParse(_employeeSalaryTemplateId, out employeeSalaryTemplateId);
                employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
                employeeSalaryTemplateMappingVM.UnitId = HttpContext.Session.GetInt32("UnitId");
                employeeSalaryTemplateMappingVM = await _payrollController.GetEmployeeSalaryTemplateMapping(employeeSalaryTemplateId);
            }
            else
                employeeSalaryTemplateMappingVM.DisplayMessage = "Error searching details for given Id. Invalid Inputs to edit";
            employeeSalaryTemplateMappingVM.UnitId = HttpContext.Session.GetInt32("UnitId");
            employeeSalaryTemplateMappingVM.EmployeeKeyValues = await _mastersKeyValueController.EmployeeKeyValue();
            employeeSalaryTemplateMappingVM.DepartmentKeyValues = await _mastersKeyValueController.DepartmentKeyValue();
            employeeSalaryTemplateMappingVM.SalaryTemplateKeyValues = await _mastersKeyValueController.SalaryTemplateKeyValue(p => (p.IsActive == true));
            employeeSalaryTemplateMappingVM = await _payrollController.GetEmployeeSalaryTemplateMappingList(employeeSalaryTemplateMappingVM, 1000, 0);
            employeeSalaryTemplateMappingVM.ViewScreen = "Edit";
            return View("EmployeeSalaryTemplateMapping", employeeSalaryTemplateMappingVM);
        }

        [HttpGet]
        [Route("PayrollSalary/GetEmployeeSalaryTemplateDetail/{id}")]
        public async Task<EmployeeSalaryTemplateMappingViewModel> GetEmployeeSalaryTemplateDetail(string id)
        {
            EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
            string _employeeSalaryTemplateId = CommonHelper.Decrypt(id);
            int employeeSalaryTemplateId;

            if (CommonHelper.IsNumeric(_employeeSalaryTemplateId))
            {
                int.TryParse(_employeeSalaryTemplateId, out employeeSalaryTemplateId);
                employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
                employeeSalaryTemplateMappingVM = await _payrollController.GetEmployeeSalaryTemplateDetail(employeeSalaryTemplateId);
                employeeSalaryTemplateMappingVM.EmployeeKeyValues = await _mastersKeyValueController.EmployeeKeyValue();
                foreach (var item in employeeSalaryTemplateMappingVM.EmployeeSalaryTemplateDetailList)
                    item.EmployeeName = employeeSalaryTemplateMappingVM.EmployeeKeyValues.Where(x => x.EmployeeId == item.EmployeeId).Select(r => r.EmployeeName).FirstOrDefault();
            }
            else
                employeeSalaryTemplateMappingVM.DisplayMessage = "Error searching details for given Id. Invalid Inputs to edit";

            return employeeSalaryTemplateMappingVM;
        }

        [HttpGet]
        [Route("PayrollSalary/DeleteEmployeeSalaryTemplateMappingInfo/{id}")]
        public async Task<string> DeleteEmployeeSalaryTemplateMappingInfo(string id)
        {
            string result = "";
            //EmployeeSalaryTemplateMappingViewModel employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
            string _employeeSalaryTemplateId = CommonHelper.Decrypt(id);
            int employeeSalaryTemplateId;

            if (CommonHelper.IsNumeric(_employeeSalaryTemplateId))
            {
                int.TryParse(_employeeSalaryTemplateId, out employeeSalaryTemplateId);
                //employeeSalaryTemplateMappingVM = new EmployeeSalaryTemplateMappingViewModel();
                //employeeSalaryTemplateMappingVM.UnitId = HttpContext.Session.GetInt32("UnitId");
                result = await _payrollController.DeleteEmployeeSalaryTemplateMappingInfo(employeeSalaryTemplateId);
            }

            return result;
        }

        public async Task<IActionResult> EarningComponent()
        {
            PayrollComponentViewModel payrollEarningVM;
            payrollEarningVM = new PayrollComponentViewModel();
            payrollEarningVM.PayrollEarningComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            payrollEarningVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "E" && x.IsActive == true && x.UnitId == payrollEarningVM.PayrollEarningComponent.UnitId);
            return View("EarningComponent", payrollEarningVM);
        }

        public async Task<IActionResult> EarningComponents()
        {
            PayrollComponentViewModel payrollEarningVM;
            payrollEarningVM = new PayrollComponentViewModel();
            payrollEarningVM.PayrollEarningComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            payrollEarningVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "E" && x.IsActive == true && x.UnitId == payrollEarningVM.PayrollEarningComponent.UnitId);
            return View("EarningComponents", payrollEarningVM);
        }

        public async Task<IActionResult> DeductionComponent()
        {
            PayrollComponentViewModel payrollComponentVM;
            payrollComponentVM = new PayrollComponentViewModel();
            payrollComponentVM.PayrollDeductionComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            payrollComponentVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "D" && x.IsActive == true && x.UnitId == payrollComponentVM.PayrollDeductionComponent.UnitId);
            return View("DeductionComponent", payrollComponentVM);
        }
        public async Task<IActionResult> DeductionComponents()
        {
            PayrollComponentViewModel payrollComponentVM;
            payrollComponentVM = new PayrollComponentViewModel();
            payrollComponentVM.PayrollDeductionComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            payrollComponentVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "D" && x.IsActive == true && x.UnitId == payrollComponentVM.PayrollDeductionComponent.UnitId);
            return View("DeductionComponents", payrollComponentVM);
        }

        public async Task<IActionResult> ReimbursementComponent()
        {
            PayrollComponentViewModel payrollComponentVM;
            payrollComponentVM = new PayrollComponentViewModel();
            payrollComponentVM.PayrollReimbursementComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            payrollComponentVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "R" && x.IsActive == true && x.UnitId == payrollComponentVM.PayrollReimbursementComponent.UnitId);
            return View("ReimbursementComponent", payrollComponentVM);
        }


        [HttpGet]
        [Route("PayrollSalary/EarningComponent/{id:int}")]
        public async Task<IActionResult> EarningComponent(int id)
        {
            PayrollComponentViewModel payrollEarningVM;
            payrollEarningVM = new PayrollComponentViewModel();
            payrollEarningVM = await _payrollController.GetEarningCompnent(id);
            payrollEarningVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "E" && x.IsActive == true);

            return View("EarningComponent", payrollEarningVM);
        }

        [HttpGet]
        [Route("PayrollSalary/EarningComponents/{eid}")]
        public async Task<IActionResult> EarningComponents(string eid)
        {
            PayrollComponentViewModel payrollEarningVM = new PayrollComponentViewModel();
            int id = 0;
            try
            {
                id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eid));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id != 0)
            {
              
              //  payrollEarningVM = new PayrollComponentViewModel();
                payrollEarningVM = await _payrollController.GetEarningCompnent(id);
                payrollEarningVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "E" && x.IsActive == true);
            }
            return View("EarningComponents", payrollEarningVM);
        }

        [HttpGet]
        [Route("PayrollSalary/DeductionComponent/{eid}")]
        public async Task<IActionResult> DeductionComponent(string eid)
        {
            PayrollComponentViewModel payrollComponentVM;
            payrollComponentVM = new PayrollComponentViewModel();
            int id = 0;
            try
            {
                id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eid));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id != 0)
            {
                payrollComponentVM = await _payrollController.GetDeductionCompnent(id);
                payrollComponentVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "D" && x.IsActive == true);
            }
            return View("DeductionComponent", payrollComponentVM);
        }

        [HttpGet]
        [Route("PayrollSalary/DeductionComponents/{eid}")]
        public async Task<IActionResult> DeductionComponents(string eid)
        {
            PayrollComponentViewModel payrollComponentVM;
            payrollComponentVM = new PayrollComponentViewModel();
            int id = 0;
            try
            {
                id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eid));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id != 0)
            {
                payrollComponentVM = await _payrollController.GetDeductionCompnent(id);
                payrollComponentVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "D" && x.IsActive == true);
            }
            return View("DeductionComponents", payrollComponentVM);
        }

        [HttpGet]
        [Route("PayrollSalary/ReimbursementComponent/{eid}")]
        public async Task<IActionResult> ReimbursementComponent(string eid)
        {
            PayrollComponentViewModel payrollComponentVM;
            payrollComponentVM = new PayrollComponentViewModel();
            int id = 0;
            try
            {
                id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eid));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id != 0)
            {
                payrollComponentVM = await _payrollController.GetReimbursementCompnent(id);
                payrollComponentVM.SalaryComponentKeyValues = await _mastersKeyValueController.SalaryCompnentKeyValue(x => x.SalaryComponentType.ToUpper() == "R" && x.IsActive == true);
            }
            return View("ReimbursementComponent", payrollComponentVM);
        }

        public async Task<string> SaveEarningComponent(PayrollComponentViewModel payrollEarningVM)
        {

            string employeeCode = "0";
            payrollEarningVM.PayrollEarningComponent.CreatedBy = employeeCode;
            payrollEarningVM.PayrollEarningComponent.Percentage = 0;
            payrollEarningVM.PayrollEarningComponent.CalculationType = "Exl";
            payrollEarningVM.PayrollEarningComponent.IsSalaryPart = true;
            payrollEarningVM.PayrollEarningComponent.IsProRataCalculation = false;
            payrollEarningVM = await _payrollController.SaveEarningComponent(payrollEarningVM);
            // return RedirectToAction("SalaryComponents");
            //return View("EarningComponent", payrollEarningVM);
            string result = payrollEarningVM.DisplayMessage;
            return result;
        }


        public async Task<string> SaveDeductionComponent(PayrollComponentViewModel payrollComponentVM)
        {
            string employeeCode = "0";
            payrollComponentVM.PayrollDeductionComponent.CreatedBy = employeeCode;
            payrollComponentVM.PayrollDeductionComponent.Percentage = 0;
            payrollComponentVM.PayrollDeductionComponent.CalculationType = "Exl";
            payrollComponentVM = await _payrollController.SaveDeductionComponent(payrollComponentVM);
            string result = payrollComponentVM.DisplayMessage;
            return result;
            //return RedirectToAction("SalaryComponents");
            //return View("DeductionComponent", payrollComponentVM);
        }

        public async Task<string> SaveReimbursementComponent(PayrollComponentViewModel payrollComponentVM)
        {
            string employeeCode = "0";
            payrollComponentVM.PayrollReimbursementComponent.CreatedBy = employeeCode;
            payrollComponentVM = await _payrollController.SaveReimbursementComponent(payrollComponentVM);
            // return RedirectToAction("SalaryComponents");
            //return View("ReimbursementComponent", payrollComponentVM);
            string result = payrollComponentVM.DisplayMessage;
            return result;
        }

        public async Task<PayrollComponentViewModel> GetComponentsList(PayrollComponentViewModel payrollEarningVM)
        {
            int employeeId = 0;
            // IList<PayrollEarningComponentDTO> outputList = new List<PayrollEarningComponentDTO>();
            if (payrollEarningVM.PayrollEarningComponent.UnitId == 0 || payrollEarningVM.PayrollEarningComponent.UnitId == null)
            {
                payrollEarningVM.PayrollEarningComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            }
            payrollEarningVM = await _payrollController.GetAllEarningComponentsList(payrollEarningVM, 1000, 0, 1);
            foreach (var item in payrollEarningVM.PayrollEarningComponentList)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.EarningId.ToString());
            }

            if (payrollEarningVM.PayrollDeductionComponent.UnitId == 0 || payrollEarningVM.PayrollDeductionComponent.UnitId == null)
            {
                payrollEarningVM.PayrollDeductionComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            }
            payrollEarningVM = await _payrollController.GetDeductionComponentsList(payrollEarningVM, 1000, 0);

            foreach (var item in payrollEarningVM.PayrollDeductionComponentList)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.DeductionId.ToString());
            }
            if (payrollEarningVM.PayrollReimbursementComponent.UnitId == 0 || payrollEarningVM.PayrollReimbursementComponent.UnitId == null)
            {
                payrollEarningVM.PayrollReimbursementComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            }
            payrollEarningVM = await _payrollController.GetReimbursementComponentsList(payrollEarningVM, 1000, 0);

            foreach (var item in payrollEarningVM.PayrollReimbursementComponentList)
            {
                item.EncryptedId = CommonHelper.EncryptURLHTML(item.ReimbursementId.ToString());
            }
            //payrollEarningVM.EmployeeMasterKeyValue = (await _mastersKeyValueController.EmployeeKeyValue()).Where(x => x.ManagerId == attendanceHistoryVM.EmployeeId || x.EmployeeId == attendanceHistoryVM.EmployeeId).ToList();
            //payrollEarningVM.DepartmentMasterKeyValue = await _mastersKeyValueController.DepartmentKeyValue();
            //payrollEarningVM.WorkLocationKeyValue = await _mastersKeyValueController.WorkLocationKeyValue();
            //payrollEarningVM.ShiftMasterKeyValue = await _mastersKeyValueController.ShiftKeyValue();
            return payrollEarningVM;
        }

        public async Task<IActionResult> SalaryComponents()
        {
            PayrollComponentViewModel payrollEarningVM;
            payrollEarningVM = new PayrollComponentViewModel();
            payrollEarningVM = await GetComponentsList(payrollEarningVM);
            //// return View("ManageEarningComponents", payrollEarningVM);
            //if (payrollEarningVM != null)
            //{
            //    return this.ExportExcel(payrollEarningVM);
            //}
            return View(payrollEarningVM);
        }

        public async Task<IActionResult> SalaryHeads()
        {
            PayrollComponentViewModel payrollEarningVM;
            payrollEarningVM = new PayrollComponentViewModel();
            payrollEarningVM = await GetComponentsList(payrollEarningVM);
            //// return View("ManageEarningComponents", payrollEarningVM);
            //if (payrollEarningVM != null)
            //{
            //    return this.ExportExcel(payrollEarningVM);
            //}
            return View(payrollEarningVM);
        }
        public async Task<IActionResult> PayScheduler()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();

            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            return View(results);
        }

        public async Task<IActionResult> PaySlip(int? Year, int? Month)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            string? clientId = HttpContext.Session.GetString("ClientId");
            SalaryProcessInputs results = new SalaryProcessInputs();
            results.EmployeeId = Convert.ToInt32(employeeId);
            results.UnitId = HttpContext.Session.GetInt32("UnitId");
            results.Month = Month;
            results.Year = Year;
            results.MonthName = getMonthName((int)Month);
            HttpContext.Session.SetInt32("SalYear", Year ?? default(int));
            HttpContext.Session.SetInt32("SalMonth", Month ?? default(int));
            results.objPaySlip = await _excelUDAPIController.GetPaySlipDetails(Convert.ToInt32(employeeId));
            results.objPaySlipComponent = await _excelUDAPIController.GetPaySlipComponents(results);

            ClientSettingDTO outputData = new ClientSettingDTO();
            //outputData.ClientId = Convert.ToInt32(clientId);
            outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(clientId));
            results.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.ProfileImage, 0, outputData.ProfileImage.Length);

            //results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            //results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            return View(results);
        }

        public async Task<IActionResult> PaySlipDownload()
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            string? clientId = HttpContext.Session.GetString("ClientId");
            SalaryProcessInputs results = new SalaryProcessInputs();
            results.EmployeeId = Convert.ToInt32(employeeId);
            results.UnitId = HttpContext.Session.GetInt32("UnitId");
            results.Month = HttpContext.Session.GetInt32("SalMonth");
            results.Year = HttpContext.Session.GetInt32("SalYear");
            results.MonthName = getMonthName((int)results.Month);
            results.objPaySlip = await _excelUDAPIController.GetPaySlipDetails(Convert.ToInt32(employeeId));
            results.objPaySlipComponent = await _excelUDAPIController.GetPaySlipComponents(results);

            ClientSettingDTO outputData = new ClientSettingDTO();
            //outputData.ClientId = Convert.ToInt32(clientId);
            outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(clientId));
            results.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.ProfileImage, 0, outputData.ProfileImage.Length);

            //results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            //results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            return View(results);
        }

        [HttpPost]
        //[ValidateInput(false)]
        public FileResult ExportPaySlip(string ExportData)
        {
            try
            {
                HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Closed;
                HtmlNode.ElementsFlags["input"] = HtmlElementFlag.Closed;
                HtmlDocument doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(ExportData);
                ExportData = doc.DocumentNode.OuterHtml;

                using (MemoryStream stream = new System.IO.MemoryStream())
                {
                    StringReader reader = new StringReader(ExportData);
                    Document PdfFile = new Document(PageSize.A4);
                    PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                    PdfFile.Open();
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
                    PdfFile.Close();
                    return File(stream.ToArray(), "application/pdf", "Salary_Nov2023.pdf");
                }
            }
            catch (SystemException ex)
            {
                return null;
            }
        }

        public async Task<IActionResult> SalariesList()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            return View(results);
        }
        public async Task<IActionResult> PayRegister()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            return View(results);
        }

        public async Task<IActionResult> SanctionMaster()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            return View(results);
        }
        public ActionResult PayRegisterDownload(int? Year, int? Month)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            DataTable dtDuration = new DataTable();
            dtDuration.Clear();
            dtDuration.Columns.Add("Month");
            dtDuration.Columns.Add("Year");
            DataRow dr = dtDuration.NewRow();
            dr["Month"] = getMonthName((int)Month);
            dr["Year"] = Year;
            dtDuration.Rows.Add(dr);
            DataSet ds = new DataSet();
            // string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_UnitPayRegister", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");
                    cmd.Parameters.Add("@Month", SqlDbType.Int).Value = Month;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = Year;

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    ds.Tables.Add(dtDuration);
                }
            }

            return View(ds);
            //results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            //results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            return View(results);
        }

        static string getMonthName(int month)
        {
            DateTime date = new DateTime(2020, month, 1);

            return date.ToString("MMMM");
        }

        public ActionResult TDSSheetDownload(int? Year, int? Month)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            DataTable dtDuration = new DataTable();
            dtDuration.Clear();
            dtDuration.Columns.Add("Month");
            dtDuration.Columns.Add("Year");
            DataRow dr = dtDuration.NewRow();
            dr["Month"] = getMonthName((int)Month);
            dr["Year"] = Year;
            dtDuration.Rows.Add(dr);

            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            DataSet ds = new DataSet();
            // string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_TDSCalculationC", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = Convert.ToInt32(employeeId);
                    cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");
                    cmd.Parameters.Add("@Month", SqlDbType.Int).Value = Month;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = Year;
                    cmd.Parameters.Add("@FY", SqlDbType.NVarChar).Value = "2023-2024";

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }
                    ds.Tables.Add(dtDuration);
                }
            }
            results.objDataSet = ds;

            //return View(ds);
            //results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            //results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            return View(results);
        }

        [HttpPost]
        public FileResult ExportTaxSheet(string TaxSheet)
        {
            return File(Encoding.ASCII.GetBytes(TaxSheet), "application/vnd.ms-excel", "TaxSheet.xls");
        }
        [HttpPost]
        public FileResult ExportPayRegister(string PayRegister)
        {
            return File(Encoding.ASCII.GetBytes(PayRegister), "application/vnd.ms-excel", "PayRegister.xls");
        }


        [HttpPost]
        public async Task<IActionResult> EmpSalaryFreeze(SalaryParameters userAction)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            //int? unitId = HttpContext.Session.GetInt32("UnitId");
            userAction.UnitId = HttpContext.Session.GetInt32("UnitId");

            // userAction.ProcessYear = 2023;
            // userAction.ProcessMonth = 12;

            SalaryParameters manualPunchVM = new SalaryParameters();
            //   userAction.ApprovedBy = employeeId;
            if (userAction.EmployeeIds != null && userAction.EmployeeIds.Length != 0)
            {
                results.objResultData = await _excelUDAPIController.FreezeSalaryDetails(userAction);

                // manualPunchVM.DisplayMessage = sRetMsg.;
            }
            return View("PayScheduler", results);
            //  return RedirectToAction("PayScheduler", "ExcelUD");
            //else
            //  manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
            // return Ok(manualPunchVM.DisplayMessage);
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployeeSalaries(int? Year, int? Month)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            //  List<EmployeeSalarySummaryDTO> objResultData = new List<EmployeeSalarySummaryDTO>();
            try
            {
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                // results.DepartmentKeyValues = await _mastersKeyValueController.DepartmentKeyValueUnitWise(unitId ??default(int),true);

                results.UnitId = unitId;
                results.Month = Month;
                results.Year = Year;
                results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
                results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
                results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
                //objResultData = EmployeeSalarySummaryDTO.GetSalarySummery(results);
                results.objResultData = await _excelUDAPIController.GetProcessedSalaryDetails(results);

            }
            catch (SystemException ex)
            {
                results.DisplayMessage = ex.Message;
            }

            return View("SalariesList", results);
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

        [HttpPost]
        public async Task<IActionResult> EmployeeSalaryProcessing(int? SelYear, int? SelMonth)
        {

            SalaryProcessInputs results = new SalaryProcessInputs();
            results.Year = SelYear;
            results.Month = SelMonth;
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            //  return View("PayScheduler", results);

            int? unitId = HttpContext.Session.GetInt32("UnitId");
            //  var employeeList = _memoryCache.Get("Employees") as List<EmployeeMasterDTO>;
            //  var employeeList = _GetEmployeeList(;
            results.EmployeeMasterList = await GetEmployeeList();
            // results.EmployeeMasterList = employeeList;
            results.UnitId = unitId;
            results.Year = SelYear;
            results.Month = SelMonth;
            results.FY = "2023-2024";
            string sRetMsg = "0";
            // results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            if (results.UnitId > 0)
            {
                if (results.EmployeeMasterList.Count > 0)
                {
                    var allEmployees = results.EmployeeMasterList.ToList();
                    for (int i = 0; i < allEmployees.Count; i++)
                    {
                        results.EmployeeId = allEmployees[i].EmployeeId;
                        sRetMsg = await _excelUDAPIController.SalaryProcessing(results);
                    }
                }

                results.DisplayMessage = sRetMsg;
                //  results.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, HttpContext.Session.GetInt32("UnitId"));
            }
            else
                results.DisplayMessage = "Failed";        // manualPunchVM.DisplayMessage = "Select punches " + (userAction.ActionType == "A" ? "for approval" : "to be reject");

            return View("PayScheduler", results);

        }

        [HttpGet]
        public async Task<List<EmployeeSalarySummary>> GetSalarySummray(int employeeId)
        {
            SalaryProcessInputs input = new SalaryProcessInputs();
            List<EmployeeSalarySummary> objResultData = new List<EmployeeSalarySummary>();

            int? unitId = HttpContext.Session.GetInt32("UnitId");
            input.UnitId = unitId;
            input.EmployeeId = employeeId;
            input.Month = 12;
            input.Year = 2023;
            objResultData = await _excelUDAPIController.GetSalarySummery(input);

            return objResultData;
        }


    }
}
