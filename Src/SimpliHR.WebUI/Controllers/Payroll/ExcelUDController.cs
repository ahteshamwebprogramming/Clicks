
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints;
using SimpliHR.Endpoints.Leave;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Endpoints.ExcelUploads;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.WebUI.BL;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using SimpliHR.Webui.Modals.Account;
//using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SimpliHR.WebUI.Controllers.ExcelUploads
{
    public class ExcelUDController : Controller
    {
        private readonly EmployeeAttendanceController _employeeAttendanceAPIController;
        private readonly EmployeeMasterController _employeeAPIController;
        private readonly MastersKeyValueController _mastersKeyValueController;
        private readonly ExcelUDAPIController _excelUDAPIController;
        private readonly EarningComponentAPIController _payrollController;
        private readonly IConfiguration _config;
        private IWebHostEnvironment _environment;
        private readonly ILogger<ExcelUDController> _logger;
        private readonly IMemoryCache _memoryCache;
        public ExcelUDController(EmployeeMasterController EmployeeAPIController, EarningComponentAPIController PayrollAPIController, IWebHostEnvironment environment, ILogger<ExcelUDController> logger, IMemoryCache memoryCache, ExcelUDAPIController excelUDAPIController, MastersKeyValueController mastersKeyValueController, EmployeeAttendanceController employeeAttendanceAPIController, IConfiguration config)
        {
            _employeeAPIController = EmployeeAPIController;
            _mastersKeyValueController = mastersKeyValueController;
            _payrollController = PayrollAPIController;
            _environment = environment;
            _logger = logger;
            _memoryCache = memoryCache;
            _excelUDAPIController = excelUDAPIController;
            _employeeAttendanceAPIController = employeeAttendanceAPIController;
            _config = config;
        }
        public async Task<IActionResult> ExcelDownload()
        {
            EmployeesSalaryDetailsDTO objresult = new EmployeesSalaryDetailsDTO();
            //PayrollComponentViewModel payrollEarningVM = new PayrollComponentViewModel();
            //int _loginId;

            //int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
            //bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
            //IActionResult actionResult = await _employeeAPIController.GetEmployeesForClient(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"), isclient, loginId);
            //ObjectResult objResult = (ObjectResult)actionResult;

            //List<EmployeeMasterDTO> objResultData = (List<EmployeeMasterDTO>)objResult.Value;
            //if (objResultData != null)
            //{
            //    payrollEarningVM.PayrollEarningComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
            //    payrollEarningVM = await _payrollController.GetEarningComponentsList(payrollEarningVM, 1000, 0);
            //    return this.ExportExcel(objResultData, payrollEarningVM);
            //}
            //   return objResultData;

            return View(objresult);
        }

        public async Task<IActionResult> SalaryDashboard()
        {
            EmployeesSalaryDetailsDTO objresult = new EmployeesSalaryDetailsDTO();
            decimal? totalSalaries = 0, totalActualSalary = 0, totalDaysOfMonth = 0;
            float? totalWorkingDays = 0, TotalActualDays = 0, totalHalfDay = 0;
            string? employeeName = "", employeeCode = "", comments = "";
            int? departmentId = 0;
            const int Oct = 7;
            int? UnitId = HttpContext.Session.GetInt32("UnitId");
            objresult.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            //  objresult.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            objresult.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            objresult.EmployeeMasterList = await GetEmployeeList();
            return View(objresult);
        }

        public async Task<IActionResult> SalaryTemplates()
        {
            EmployeesSalaryDetailsDTO objresult = new EmployeesSalaryDetailsDTO();

            objresult.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            objresult.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            objresult.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            //objresult.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, HttpContext.Session.GetInt32("UnitId"));
            // objresult.EmployeeMasterList = await GetEmployeeList();

            return View(objresult);
        }

        [HttpGet]
        public async Task<IActionResult> GetExcel(int isFixed)
        {
            //return null;
            try
            {
                EmployeesSalaryDetailsDTO objOutput = new EmployeesSalaryDetailsDTO();
                PayrollComponentViewModel payrollEarningVM = new PayrollComponentViewModel();

                int _loginId;
                int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
                bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
                IActionResult actionResult = await _employeeAPIController.GetEmployeesForClient(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"), isclient, loginId);
                ObjectResult objResult = (ObjectResult)actionResult;

                List<EmployeeMasterDTO> objResultData = (List<EmployeeMasterDTO>)objResult.Value;
                if (objResultData != null)
                {
                    payrollEarningVM.PayrollEarningComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
                    payrollEarningVM = await _payrollController.GetEarningComponentsList(payrollEarningVM, 1000, 0, isFixed);
                    var file = this.ExportExcel(objResultData, payrollEarningVM, isFixed);
                    if (file != null)
                        return file;
                    else
                    {
                        objOutput.DisplayMessage = "Employees not found";
                        return View("SalaryTemplates", objOutput);
                    }

                }
                return BadRequest(); //Or some other relevant response.
            }
            catch (SystemException ex)
            {
                return null;
            }
        }
        public FileStreamResult ExportExcel(List<EmployeeMasterDTO> EmployeesData, PayrollComponentViewModel Earnings, int isFixed)
        {

            try
            {
                var fileName = "EmployeeSalaryTemplate.xlsx";
                var unitId = HttpContext.Session.GetInt32("UnitId");
                string folderpath = Path.Combine(this._environment.WebRootPath, "SalaryTemplates");
                string path = Path.Combine(folderpath, Convert.ToString(unitId));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                var memoryStream = new MemoryStream();
                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheetExist = package.Workbook.Worksheets.SingleOrDefault(x => x.Name == "SalaryTemplate");
                    if (worksheetExist != null)
                        package.Workbook.Worksheets.Delete(worksheetExist);

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("SalaryTemplate");
                    int currentRowNo = 1;
                    int totalRows = Earnings.PayrollEarningComponentList.Count;
                    int k = 2;
                    if (EmployeesData.Count > 0)
                    {
                        worksheet.Cells[1, 1].Value = "EmployeeCode";
                        worksheet.Cells[1, 2].Value = "EmployeeName";
                        foreach (var column in Earnings.PayrollEarningComponentList)
                        {
                            worksheet.Cells[currentRowNo, k + 1].Value = column.NameInPaySlip;
                            k++;
                        }
                        if (isFixed == 1)
                            worksheet.Cells[currentRowNo, k + 1].Value = "WEF";
                    }
                    int columnCount = Earnings.PayrollEarningComponentList.Count + 3;
                    for (int i = 1; i <= columnCount; i++)
                        worksheet.Column(i).AutoFit();

                    worksheet.Row(1).Style.Font.Bold = true;
                    k = 0;
                    foreach (var row in EmployeesData)
                    {
                        worksheet.Cells[currentRowNo + 1, k + 1].Value = row.EmployeeCode;
                        worksheet.Cells[currentRowNo + 1, k + 2].Value = row.EmployeeName;
                        //  k++;
                        currentRowNo++;
                    }
                    //  Path.Combine(path, fileName)
                    // string path = Path.Combine(path, fileName);
                    package.SaveAs(new FileInfo(Path.Combine(path, fileName)));
                    //package.Save();
                }

                memoryStream.Position = 0;
                var contentType = "application/octet-stream";

                // return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                return File(memoryStream, contentType, fileName);
            }
            catch (SystemException ex)
            {

            }

            return null;
        }

        [HttpGet]
        public EmployeesSalaryDetailsDTO SalaryProcessing()
        {
            try
            {
                EmployeesSalaryDetailsDTO objresult = new EmployeesSalaryDetailsDTO();
                var unitId = HttpContext.Session.GetInt32("UnitId");
                var validateSalaryList = _memoryCache.Get("Validate") as List<SalaryValidates>;
                var salaryList = _memoryCache.Get("Salary") as List<EmployeesSalaryDetailsDTO>;
                var listOfSalary = validateSalaryList.Where(x => x.UnitId.Equals(unitId) && x.DisplayMessage == "1").ToList();
                if (listOfSalary.Count > 0)
                {

                    for (int i = 0; i < listOfSalary.Count; i++)
                    {
                        var salaryDetails = salaryList.Where(x => x.UnitId.Equals(unitId) && x.EmployeeId.Equals(listOfSalary[i].EmployeeId)).ToList();
                        if (salaryDetails.Count > 0)
                        {
                            for (int j = 0; j < salaryDetails.Count; j++)
                            {
                                salaryDetails[j].Processdate = DateTime.UtcNow;
                                salaryDetails[j].IsCurrent = true;
                                salaryDetails[j].ProcessBy = HttpContext.Session.GetString("EmployeeId");
                                IActionResult employeeSalaryResult = _excelUDAPIController.SaveEmployeeSalaryDetails(salaryDetails[j]);
                                ObjectResult objUnitEmployeeResult = (ObjectResult)employeeSalaryResult;
                                /// End

                                if (objUnitEmployeeResult.StatusCode == 200)
                                {
                                    objresult.DisplayMessage = "1";
                                }
                            }
                        }
                    }

                }
                return objresult;
            }
            catch (SystemException ex)
            {
                _logger.LogError(ex, $"Error during inserting the salary details into DB {nameof(SalaryProcessing)}");
                throw;
            }
            return null;
        }

        public ActionResult Download()
        {
            string path = Path.Combine(this._environment.WebRootPath, "SalaryTemplates");
            string fullPath = @"C:\\simplyhr\\EmployeeSalaryTemplate.xlsx";
            //var fullPath = path + "\\100EmployeeSalaryTemplate.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fullPath, contentType, Path.GetFileName(fullPath));
        }

        public async Task<IActionResult> UploadSalaryTemplate(List<IFormFile> formFile, string isFixed, int Year, int Month)
        {
            int cTypeId = Convert.ToInt32(isFixed);
            if (cTypeId == 2)
                cTypeId = 1;

            var employeesList = new List<EmployeesSalaryDetailsDTO>();
            List<EmployeesSalaryDetailsDTO>? componentList = new List<EmployeesSalaryDetailsDTO>();

            EmployeesSalaryDetailsDTO objresult = new EmployeesSalaryDetailsDTO();
            EmployeesSalaryDetailsDTO objSalary = null;

            // return View("SalaryTemplates", objresult);
            try
            {

                if (formFile.Count > 0)
                {
                    PayrollComponentViewModel payrollEarningVM = new PayrollComponentViewModel();
                    payrollEarningVM.PayrollEarningComponent.UnitId = HttpContext.Session.GetInt32("UnitId");
                    payrollEarningVM = await _payrollController.GetEarningComponentsList(payrollEarningVM, 1000, 0, cTypeId);
                    var unitId = HttpContext.Session.GetInt32("UnitId");
                    string fileName = null;
                    //string path = Path.Combine(this._environment.WebRootPath, "SalaryTemplates");
                    string folderpath = Path.Combine(this._environment.WebRootPath, "SalaryTemplates");
                    string path = Path.Combine(folderpath, Convert.ToString(unitId));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    List<string> uploadedFiles = new List<string>();
                    foreach (IFormFile postedFile in formFile)
                    {
                        fileName = Path.GetFileName(postedFile.FileName);
                        // fileName = unitId + '_' + fileName;

                        using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        {
                            postedFile.CopyTo(stream);
                            // uploadedFiles.Add(fileName);
                            // ViewBag.Message += fileName + ",";
                        }

                    }
                    var fullPath = path + "/" + fileName;
                    using var package = new ExcelPackage(fullPath);
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    var currentSheet = package.Workbook.Worksheets;



                    var workSheet = currentSheet.First();
                    // var findval = workSheet.Select(row);
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    var noCol = noOfCol;
                    if (isFixed == "1")
                        noCol = noOfCol - 1;

                    objresult.DisplayMessage = "";

                    // listing all earnnig components and validation
                    for (int rowIterator = 3; rowIterator <= noCol; rowIterator++)
                    {
                        var component = new EmployeesSalaryDetailsDTO
                        {
                            ComponentName = workSheet.Cells[1, rowIterator].Value?.ToString()

                        };

                        //var componentId = payrollEarningVM.PayrollEarningComponentList.Where(x => x.NameInPaySlip.Equals(component.ComponentName)).Select(i => i.SalaryComponentId).FirstOrDefault();
                        var componentsDetails = payrollEarningVM.PayrollEarningComponentList.Where(x => x.NameInPaySlip.Equals(component.ComponentName)).FirstOrDefault();
                        component.SalaryComponentId = componentsDetails.SalaryComponentId;
                        component.IsEpfConsidration = componentsDetails.IsEpfConsidration;
                        component.IsEsiConsidrable = componentsDetails.IsEsiConsidrable;
                        component.IsVisibleInPaySlip = componentsDetails.IsVisibleInPaySlip;
                        component.IsTaxableIncome = componentsDetails.IsTaxableIncome;

                        if (payrollEarningVM.PayrollEarningComponentList.Find(x => x.NameInPaySlip == component.ComponentName) == null)
                        {

                            var validate = new ComponentsValidates
                            {
                                ComponentName = component.ComponentName,
                                UnitId = unitId,
                                SalaryComponentId = component.SalaryComponentId,
                                DisplayMessage = "Component does not exist/ matched"

                            };
                            objresult.DisplayMessage = "Component";
                            objresult.ComponentValidateList.Add(validate);
                            // ComponentValidateList.Add(validate);
                            // component.DisplayMessage += component.ComponentName + " Component does not exist";
                        }

                        componentList.Add(component);
                        // TempData["Components"] = componentList;

                    }


                    //   _memoryCache.Set("Components", componentList);
                    // End listing all earnnig components

                    // Getting salary details of employees listed
                    if (objresult.ComponentValidateList.Count <= 0 || objresult.ComponentValidateList == null)
                    {
                        objresult.DisplayMessage = "";
                        int _loginId;
                        int? loginId = int.TryParse(HttpContext.Session.GetString("EmployeeId"), out _loginId) == true ? _loginId : null;
                        bool isclient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
                        IActionResult actionResult = await _employeeAPIController.GetEmployeesForClient(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"), isclient, loginId);
                        ObjectResult objResult = (ObjectResult)actionResult;

                        List<EmployeeMasterDTO> objResultData = (List<EmployeeMasterDTO>)objResult.Value;
                        //DateTime crrdt = DateTime.Now;
                        if (objResultData != null && objResultData.Count > 0)
                        {
                            foreach (var row in objResultData)
                            {

                                foreach (var worksheetCell in workSheet.Cells)
                                {
                                    if (worksheetCell.Value.ToString().Equals(row.EmployeeCode))
                                    {

                                        double empSalaryDetails = 0;
                                        string? WEF = null;

                                        var fRow = worksheetCell.Start.Row;

                                        for (int i = 3; i <= noCol; i++)
                                        {
                                            objSalary = new EmployeesSalaryDetailsDTO();
                                            objSalary.EmployeeId = row.EmployeeId;
                                            objSalary.CalculationType = "Flat";
                                            objSalary.PerVal = 0;
                                            objSalary.SalaryComponentType = "E";
                                            objSalary.UnitId = unitId;
                                            objSalary.SalaryMonth = Month;
                                            objSalary.SalaryYear = Year;
                                            objSalary.TemplateType = "Exl";
                                            if (Convert.ToInt32(isFixed) == 0)
                                                objSalary.IsVariable = true;
                                            else
                                                objSalary.IsVariable = false;

                                            var salaryComponentId = componentList[i - 3].SalaryComponentId;
                                            objSalary.SalaryComponentId = salaryComponentId;
                                            objSalary.ComponentName = componentList[i - 3].ComponentName;
                                            objSalary.IsEpfConsidration = componentList[i - 3].IsEpfConsidration;
                                            objSalary.IsEsiConsidrable = componentList[i - 3].IsEsiConsidrable;
                                            objSalary.IsVisibleInPaySlip = componentList[i - 3].IsVisibleInPaySlip;
                                            objSalary.IsTaxableIncome = componentList[i - 3].IsTaxableIncome;


                                            float f;
                                            if (float.TryParse(Convert.ToString(workSheet.Cells[fRow, i].Value), out f))
                                            {
                                                if (isFixed == "2")
                                                    objSalary.ArrearsAmt = Convert.ToDecimal(workSheet.Cells[fRow, i].Value);
                                                else
                                                    objSalary.AmtPerMonth = Convert.ToDecimal(workSheet.Cells[fRow, i].Value);

                                                empSalaryDetails += Convert.ToDouble(workSheet.Cells[fRow, i].Value);

                                            }
                                            if (isFixed == "1")
                                                objSalary.WEF = Convert.ToString(workSheet.Cells[fRow, noOfCol].Value);

                                            employeesList.Add(objSalary);
                                        }

                                        var Salary = new SalaryValidates
                                        {
                                            EmployeeName = row.EmployeeName,
                                            EmployeeCode = row.EmployeeCode,
                                            //EmployeeSalValue = Math.Round(mCTC, 2),
                                            EmployeeSalValue = 0,
                                            TemplateSalValue = empSalaryDetails,
                                            UnitId = unitId,
                                            EmployeeId = row.EmployeeId,
                                            DisplayMessage = "1"

                                        };
                                        objresult.DisplayMessage = "Salary";
                                        objresult.SalaryValidateList.Add(Salary);

                                    }

                                }
                            }
                            _memoryCache.Set("Validate", objresult.SalaryValidateList);
                            _memoryCache.Set("Salary", employeesList);

                            var results = SalaryTemplateUpload();
                            if (results.DisplayMessage == "1")
                            {
                                objresult.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
                                objresult.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
                                objresult.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
                                objresult.DisplayMessage = "Salary template successfully processed";
                                //objresult.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue(true, HttpContext.Session.GetInt32("UnitId"));
                                //objresult.EmployeeMasterList = await GetEmployeeList();
                                return View("SalaryTemplates", objresult);
                                // return RedirectToAction("SalaryDashboard", "ExcelUD");
                            }
                            else
                            {
                                return View("SalaryTemplates", objresult);
                            }

                        }

                        // End Getting salary details of employees listed
                    }
                }
                else
                {
                    objresult.DisplayMessage = "File does not exist";
                }
                //  RedirectToAction("SalaryDashboard", "ExcelUD");
                return View("SalaryTemplates", objresult);
            }
            catch (SystemException ex)
            {
                _logger.LogError(ex, $"Error during uploading the salary {nameof(UploadSalaryTemplate)}");
                throw;
            }

            //  return View("SalaryDashboard", objresult);
        }

        public EmployeesSalaryDetailsDTO SalaryTemplateUpload()
        {
            try
            {
                EmployeesSalaryDetailsDTO objresult = new EmployeesSalaryDetailsDTO();
                var unitId = HttpContext.Session.GetInt32("UnitId");
                var validateSalaryList = _memoryCache.Get("Validate") as List<SalaryValidates>;
                var salaryList = _memoryCache.Get("Salary") as List<EmployeesSalaryDetailsDTO>;
                var listOfSalary = validateSalaryList.Where(x => x.UnitId.Equals(unitId) && x.DisplayMessage == "1").ToList();
                if (listOfSalary.Count > 0)
                {

                    for (int i = 0; i < listOfSalary.Count; i++)
                    {
                        var salaryDetails = salaryList.Where(x => x.UnitId.Equals(unitId) && x.EmployeeId.Equals(listOfSalary[i].EmployeeId)).ToList();
                        if (salaryDetails.Count > 0)
                        {
                            for (int j = 0; j < salaryDetails.Count; j++)
                            {
                                salaryDetails[j].Processdate = DateTime.UtcNow;
                                salaryDetails[j].IsCurrent = true;
                                salaryDetails[j].ProcessBy = HttpContext.Session.GetString("EmployeeId");
                                //salaryDetails[j].IsTaxableIncome = true;
                                //salaryDetails[j].IsEpfConsidration = true;
                                //salaryDetails[j].IsEsiConsidrable = true;
                                //salaryDetails[j].IsVisibleInPaySlip = true;
                                salaryDetails[j].AmtPerYear = 0;
                                // salaryDetails[j].IsVariable = false;
                                if (string.IsNullOrEmpty(Convert.ToString(salaryDetails[j].AmtPerMonth)))
                                {
                                    salaryDetails[j].AmtPerMonth = 0;
                                    salaryDetails[j].IsArrears = true;
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(salaryDetails[j].ArrearsAmt)))
                                {
                                    salaryDetails[j].ArrearsAmt = 0;
                                    salaryDetails[j].IsArrears = false;
                                }

                                IActionResult employeeSalaryResult = _excelUDAPIController.SaveEmployeeSalaryDetails(salaryDetails[j]);
                                ObjectResult objUnitEmployeeResult = (ObjectResult)employeeSalaryResult;

                                /// End

                                if (objUnitEmployeeResult.StatusCode == 200)
                                {
                                    objresult.DisplayMessage = "1";
                                }
                            }
                        }
                    }

                }
                return objresult;
            }
            catch (SystemException ex)
            {
                _logger.LogError(ex, $"Error during inserting the salary details into DB {nameof(SalaryProcessing)}");
                throw;
            }
            return null;
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


        public async Task<List<EmployeesSalaryDetailsDTO>> GetEmployeeSalaryDetails(int employeeId, int? Year, int? Month)
        {
            EmployeesSalaryDetailsDTO EmployeeSalaryDetails = new EmployeesSalaryDetailsDTO();
            //  EmployeeLeaveDetail = await _leaveAPIController.GetEmployeeLeaveDetails(LeaveTypeId);
            //  EmployeeLeaveDetail.lstUnitMaster = await GetClientUnits();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _excelUDAPIController.GetEmployeeSalaryDetails(employeeId, unitId, Year, Month);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<EmployeesSalaryDetailsDTO> objResultData = (List<EmployeesSalaryDetailsDTO>)objResult.Value;
            //  EmployeeSalaryDetails.SalaryDetails = objResultData;
            return objResultData;
        }


        [HttpGet]
        public async Task<List<EmployeesSalaryDetailsDTO>> GetSalaryDetails(int employeeId)
        {
            EmployeesSalaryDetailsDTO EmployeeSalaryDetails = new EmployeesSalaryDetailsDTO();
            //  EmployeeLeaveDetail = await _leaveAPIController.GetEmployeeLeaveDetails(LeaveTypeId);
            //  EmployeeLeaveDetail.lstUnitMaster = await GetClientUnits();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _excelUDAPIController.GetEmployeeSalaryDetails(employeeId, unitId, 2023, 11);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<EmployeesSalaryDetailsDTO> objResultData = (List<EmployeesSalaryDetailsDTO>)objResult.Value;
            //  EmployeeSalaryDetails.SalaryDetails = objResultData;
            return objResultData;
        }

        [HttpPost]
        public async Task<IActionResult> EmpSalaryProcessing(SalaryParameters userAction)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            //int? unitId = HttpContext.Session.GetInt32("UnitId");
            userAction.UnitId = HttpContext.Session.GetInt32("UnitId");
            // userAction.ProcessYear = 2023;
            // userAction.ProcessMonth = 12;

            SalaryParameters manualPunchVM = new SalaryParameters();
            //   userAction.ApprovedBy = employeeId;
            if (userAction.EmployeeIds != null && userAction.EmployeeIds.Length != 0)
            {
                var sRetMsg = await _excelUDAPIController.UpdateSalaryDetails(userAction);
                manualPunchVM.DisplayMessage = "Successed";
            }

            // return RedirectToAction("SalaryDashboard", "ExcelUD");
            //else
            //  manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
            return Ok(manualPunchVM.DisplayMessage);
        }


        //[HttpPost]
        //public async Task<IActionResult> EmpSalaryFreeze(SalaryParameters userAction)
        //{
        //    string? employeeId = HttpContext.Session.GetString("EmployeeId");
        //    //int? unitId = HttpContext.Session.GetInt32("UnitId");
        //    userAction.UnitId = HttpContext.Session.GetInt32("UnitId");
        //    // userAction.ProcessYear = 2023;
        //    // userAction.ProcessMonth = 12;

        //    SalaryParameters manualPunchVM = new SalaryParameters();
        //    //   userAction.ApprovedBy = employeeId;
        //    if (userAction.EmployeeIds != null && userAction.EmployeeIds.Length != 0)
        //    {
        //        var sRetMsg = await _excelUDAPIController.FreezeSalaryDetails(userAction);
        //        // manualPunchVM.DisplayMessage = sRetMsg.;
        //    }

        //    return RedirectToAction("PayScheduler", "ExcelUD");
        //    //else
        //    //  manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
        //    // return Ok(manualPunchVM.DisplayMessage);
        //}


        [HttpPost]
        public async Task<IActionResult> GetEmployeeSalaries(int? Year, int? Month, int? Employee)
        {

            EmployeesSalaryDetailsDTO objresult = new EmployeesSalaryDetailsDTO();
            decimal? totalSalaries = 0, totalActualSalary = 0, totalDaysOfMonth = 0;
            float? totalWorkingDays = 0, TotalActualDays = 0, totalHalfDay = 0;
            string? employeeName = "", employeeCode = "", comments = "";
            int? departmentId = 0, isFreezed = 0;
            // const int Oct = 7;
            try
            {
                var firstDayOfMonth = new DateTime((int)Year, (int)Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                objresult.SalaryYear = Year;
                objresult.SalaryMonth = Month;
                objresult.Employee = Employee;
                objresult.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
                objresult.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
                objresult.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
                List<EmployeeSalarySummary> objResultData = new List<EmployeeSalarySummary>();
                SalaryProcessInputs input = new SalaryProcessInputs();
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                AttendanceHistoryViewModel attendanceHistoryVM = new AttendanceHistoryViewModel();
                attendanceHistoryVM.StartDate = firstDayOfMonth;
                attendanceHistoryVM.EndDate = lastDayOfMonth;
                //int? totalDays = DateTime.DaysInMonth(2023, Oct);
                int? totalDays = DateTime.DaysInMonth((int)Year, (int)Month);
                totalDaysOfMonth = Convert.ToDecimal(totalDays);
                objresult.EmployeeMastersKeyValues = await _mastersKeyValueController.DeptMastersKeyValue(true, HttpContext.Session.GetInt32("UnitId"));
                //objresult.DepartmentKeyValues = await _mastersKeyValueController.DepartmentKeyValueUnitWise(unitId ?? default(int), true);
                objresult.EmployeeMasterList = await GetEmployeeList();
                if (objresult.EmployeeMasterList.Count > 0)
                {
                    List<EmployeeMasterDTO> allEmployees = new List<EmployeeMasterDTO>();
                    if (Employee > 0)
                        allEmployees = objresult.EmployeeMasterList.Where(x => x.EmployeeId == Employee).ToList();
                    else
                        allEmployees = objresult.EmployeeMasterList.ToList();

                    for (int i = 0; i < allEmployees.Count; i++)
                    {
                        totalSalaries = 0; totalActualSalary = 0; comments = "";
                        totalWorkingDays = 0; TotalActualDays = 0; totalHalfDay = 0;
                        employeeName = ""; employeeCode = ""; isFreezed = 0;
                        attendanceHistoryVM.EmployeeId = allEmployees[i].EmployeeId;
                        employeeName = allEmployees[i].EmployeeName;
                        employeeCode = allEmployees[i].EmployeeCode;
                        departmentId = allEmployees[i].DepartmentId;
                       
                        input.UnitId = unitId;
                        input.EmployeeId = allEmployees[i].EmployeeId;
                        input.Month = Month;
                        input.Year = Year;
                        input.Employee = Employee;
                        objResultData = await _excelUDAPIController.GetSalarySummery(input);
                        // if (objResultData.Count > 0)
                        //{
                        // if (objResultData[0].IsFreezed == false)
                        //{
                        if (objResultData.Count > 0)
                        {
                            if (objResultData[0].IsFreezed == false)
                            {

                                if (string.IsNullOrEmpty(Convert.ToString(objResultData[0].NoOfDays)))
                                    objResultData[0].NoOfDays = 0;

                                if (objResultData[0].NoOfDays <= 0)
                                {
                                    attendanceHistoryVM = await _employeeAttendanceAPIController.GetEmployeeAttendance(attendanceHistoryVM, 1000, 0); // for pick the all working days
                                    if (attendanceHistoryVM != null)
                                    {
                                        totalHalfDay = attendanceHistoryVM.HalfDays;
                                        totalWorkingDays = attendanceHistoryVM.PresentDays + (totalHalfDay / 2);
                                        TotalActualDays = attendanceHistoryVM.PresentDays + (totalHalfDay / 2);

                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(Convert.ToString(objResultData[0].PayOutDays)))
                                        objResultData[0].PayOutDays = objResultData[0].NoOfDays;

                                    totalWorkingDays = (float)objResultData[0].NoOfDays;

                                }


                                TotalActualDays = (float)objResultData[0].PayOutDays;
                                comments = objResultData[0].Remarks;
                            }
                            else
                                isFreezed = 1;
                        }
                        else
                        {
                            attendanceHistoryVM = await _employeeAttendanceAPIController.GetEmployeeAttendance(attendanceHistoryVM, 1000, 0); // for pick the all working days
                            if (attendanceHistoryVM != null)
                            {
                                totalHalfDay = attendanceHistoryVM.HalfDays;
                                totalWorkingDays = attendanceHistoryVM.PresentDays + (totalHalfDay / 2);
                                TotalActualDays = attendanceHistoryVM.PresentDays + (totalHalfDay / 2);

                            }
                        }

                        if (isFreezed == 0)
                        {
                            var employeeSalary = GetEmployeeSalaryDetails(attendanceHistoryVM.EmployeeId, input.Year, input.Month).Result.ToList();
                            if (employeeSalary.Count > 0)
                            {
                                for (int x = 0; x < employeeSalary.Count; x++)
                                {
                                    totalSalaries = totalSalaries + (employeeSalary[x].AmtPerMonth + employeeSalary[x].ArrearsAmt);
                                }
                                if (TotalActualDays > 0)
                                {
                                    totalActualSalary = (totalSalaries / totalDaysOfMonth) * Convert.ToDecimal(TotalActualDays);
                                    totalActualSalary = Math.Round((decimal)totalActualSalary, 2);
                                }
                                else
                                    totalActualSalary = 0;
                            }

                            var empSalary = new EmpSalaryComponents
                            {
                                ActualSalary = totalActualSalary,
                                Comments = comments,
                                EmployeeCode = employeeCode,
                                EmployeeName = employeeName,
                                EmployeeId = attendanceHistoryVM.EmployeeId,
                                FixedSalary = totalSalaries,
                                WorkingDays = totalWorkingDays,
                                ActualworkingDays = TotalActualDays,
                                DepartmentId = departmentId,
                                DaysOfMonth = (double)totalDaysOfMonth,
                                SalaryMonth = Month,
                                SalaryYear = Year,
                                Employee = Employee,
                                DisplayMessage = "1"
                            };

                            if (empSalary.FixedSalary > 0)
                                objresult.EmpSalaryComponentsList.Add(empSalary);

                            
                        }
                    }
                   

                }


            }
            catch (SystemException ex)
            {
                objresult.DisplayMessage = "Error Occured";
            }
            //  return View(objresult);
            //attendanceHistoryVM = await GetEmployeeAttendanceList(attendanceHistoryVM);
            return View("SalaryDashboard", objresult);
        }

        public IActionResult EmployeeDataBulkUploadAdd()
        {
            EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
            return View(employeeMasterDTO);
        }
        public IActionResult EmployeeDataBulkUploadEdit()
        {
            EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
            return View(employeeMasterDTO);
        }

        [HttpPost]
        public async Task<IActionResult> UploadEmployeeDataToAdd()
        {
            EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
            try
            {
                //var x = 0;
                //var y = x / 0;
                var fromFiles = HttpContext.Request.Form.Files;
                if(!string.IsNullOrEmpty(fromFiles[0].FileName))
                {
                    string uploadpath = _environment.WebRootPath;
                    int? UnitId = HttpContext.Session.GetInt32("UnitId");
                    string dest_path = Path.Combine(uploadpath, $@"EmployeeProfile\BulkUpload\{UnitId.ToString()}\Add");

                    if (!Directory.Exists(dest_path))
                    {
                        Directory.CreateDirectory(dest_path);
                    }
                    string sourcefile = Path.GetFileName(fromFiles[0].FileName);
                    string path = Path.Combine(dest_path, sourcefile);

                    using (FileStream filestream = new FileStream(path, FileMode.Create))
                    {
                        fromFiles[0].CopyTo(filestream);
                    }

                    employeeMasterDTO.DisplayMessage = await _excelUDAPIController.SaveExcelDataToEmployee(path);
                }
                else
                {
                    employeeMasterDTO.DisplayMessage = "Please select the template";
                }
                
               
            }
            catch (Exception ex)
            {

                employeeMasterDTO.DisplayMessage = $"Source: {ex.Source}({nameof(UploadEmployeeDataToAdd)})\n{ex.Message}";
                string sLogPath = _config.GetValue<string>("LogFilePathName");
                CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", employeeMasterDTO.DisplayMessage);
               
            }
            return View("EmployeeDataBulkUploadAdd", employeeMasterDTO);
        }
        public async Task<IActionResult> UploadEmployeeDataToEdit()
        {
            var fromFiles = HttpContext.Request.Form.Files;
            string uploadpath = _environment.WebRootPath;
            int? UnitId = HttpContext.Session.GetInt32("UnitId");
            string dest_path = Path.Combine(uploadpath, $@"EmployeeProfile\BulkUpload\{UnitId.ToString()}\Edit");

            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }
            string sourcefile = Path.GetFileName(fromFiles[0].FileName);
            string path = Path.Combine(dest_path, sourcefile);

            using (FileStream filestream = new FileStream(path, FileMode.Create))
            {
                fromFiles[0].CopyTo(filestream);
            }

            EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();
            bool IsSuccess = await _excelUDAPIController.EditExcelDataOfEmployee(path);
            if (IsSuccess)
                employeeMasterDTO.DisplayMessage = "Success";
            return View("EmployeeDataBulkUploadEdit", employeeMasterDTO);
            //return path;
        }

        //public void ImportCustomer(DataTable customer)
        //{


        //        //using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(scon))
        //        //{
        //        //    sqlBulkCopy.DestinationTableName = "Customers";


        //    //    sqlBulkCopy.ColumnMappings.Add("FirstName", "firstName");
        //    //    sqlBulkCopy.ColumnMappings.Add("LastName", "lastName");
        //    //    sqlBulkCopy.ColumnMappings.Add("job", "job");
        //    //    sqlBulkCopy.ColumnMappings.Add("amount", "amount");
        //    //    sqlBulkCopy.ColumnMappings.Add("hiredate", "tdate");

        //    //    scon.Open();
        //    //    sqlBulkCopy.WriteToServer(customer);
        //    //    scon.Close();
        //    //}


        //}

    }
}
