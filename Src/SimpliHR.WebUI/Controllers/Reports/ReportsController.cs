using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SimpliHR.WebUI.Controllers.Reports
{
     //[Authorize(Roles = "Clientadmin")]
    public class ReportsController : Controller
    {

        private readonly ReportsAPIController _ReportsAPIController;
        private readonly MastersKeyValueController _MastersKeyValueController;
        private readonly IConfiguration _configuration;
        public ReportsController(ReportsAPIController reportsAPIController, IConfiguration configuration, MastersKeyValueController MastersKeyValueController)
        {
            _ReportsAPIController = reportsAPIController;
            _MastersKeyValueController = MastersKeyValueController;
            _configuration = configuration;

        }

       
        public async Task<IActionResult> BankTransferReport()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();

            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);

            return View(results);
        }

        [HttpPost]
       // [Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> ExportBankTransfer(int? Year, int? Month)
        {
           // _configuration.GetConnectionString("SimplyConnectionDB")
            SalaryProcessInputs results = new SalaryProcessInputs();
            DataSet ds = new DataSet();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            results.Month = Month;
            results.Year = Year;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_BankTransferReport", con))
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

                }
                results.objBTRDSet = ds;
            }


            return View("BankTransferReport", results);
        }

      //  [Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> ESIReport()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();

            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);

            return View(results);
        }

        [HttpPost]
        //[Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> ExportESI(int? Year, int? Month)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            DataSet ds = new DataSet();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            results.Month = Month;
            results.Year = Year;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_EmployeeESIReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");
                    cmd.Parameters.Add("@Month", SqlDbType.Int).Value = Month;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = Year;

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }

                }
                results.objESIDSet = ds;
            }


            return View("ESIReport", results);
        }

       // [Authorize(Roles = "Clientadmin")]
        public IActionResult LWReport()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();

            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);

            return View(results);
        }

        [HttpPost]
       // [Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> ExportLW(int? Year, int? Month)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            DataSet ds = new DataSet();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            results.Month = Month;
            results.Year = Year;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_EmployeeLWFReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");
                    cmd.Parameters.Add("@Month", SqlDbType.Int).Value = Month;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = Year;

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }

                }
                results.objLWDSet = ds;
            }


            return View("LWReport", results);
        }

     //   [Authorize(Roles = "Clientadmin")]
        public IActionResult ProfTaxReport()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();

            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);

            return View(results);
        }

        [HttpPost]
       // [Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> ExportPT(int? Year, int? Month)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            DataSet ds = new DataSet();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            results.Month = Month;
            results.Year = Year;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_EmployeePTReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");
                    cmd.Parameters.Add("@Month", SqlDbType.Int).Value = Month;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = Year;

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }

                }
                results.objPTDSet = ds;
            }


            return View("ProfTaxReport", results);
        }


      //  [Authorize(Roles = "Clientadmin")]
        public IActionResult ITDeductionReport()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();

            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);

            return View(results);
        }
        [HttpPost]
      //  [Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> ExportIT(int? Year, int? Month)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            DataSet ds = new DataSet();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            results.Month = Month;
            results.Year = Year;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_EmployeeITDeductionReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");
                    cmd.Parameters.Add("@Month", SqlDbType.Int).Value = Month;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = Year;

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }

                }
                results.objITDSet = ds;
            }


            return View("ITDeductionReport", results);
        }

       // [Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> RegimesSelectionReport()
        {
            RegimeSelectionReportDTO objOutPut = new RegimeSelectionReportDTO();
            // List<RegimeSelectionReport> objResultData = new List<RegimeSelectionReport>();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            objOutPut.GetRegimeSelectionReport = await _ReportsAPIController.RegimeSelection(unitId);
            return View(objOutPut);
        }

      //  [Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> RegimeSelectionReport()
        {
            RegimeSelectionReportDTO objOutPut = new RegimeSelectionReportDTO();
            // List<RegimeSelectionReport> objResultData = new List<RegimeSelectionReport>();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            objOutPut.GetRegimeSelectionReport = await _ReportsAPIController.RegimeSelection(unitId);
            return View(objOutPut);
        }


      //  [Authorize(Roles = "Clientadmin")]
        public ActionResult PolicyAcceptanceReport()
        {
            SalaryProcessInputs results = new SalaryProcessInputs();

            //results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            //// results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            //results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_PolicyAcceptanceReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;                 
                    cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");
                   
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }

                }
               // results.objBTRDSet = ds;
            }

            return View(ds);
        }
        //[Authorize(Roles = "Clientadmin")]
        public ActionResult ConsolidatedReport()
        {
           // SalaryProcessInputs results = new SalaryProcessInputs();

            //results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            //// Leave Balance Reportresults.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            //results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            {
                //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
                using (SqlCommand cmd = new SqlCommand("usp_ConsolidatedEmployeeReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(ds);
                    }

                }
                // results.objBTRDSet = ds;
            }

            return View(ds);
        }

        // [Authorize(Roles = "Clientadmin")]
        public async Task<IActionResult> LeaveBalanceReport()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            EmployeeLeaveBalanceInputs results = new EmployeeLeaveBalanceInputs();

            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            results.DepartmentKeyValues = await _MastersKeyValueController.DepartmentKeyValue(x => x.IsActive == true && x.UnitId == unitId);
            results.WorkLocationKeyValues = await _MastersKeyValueController.WorkLocationKeyValue(x => x.IsActive == true && x.UnitId == unitId);

            return View(results);
        }


        public async Task<IActionResult> ExportLeaveBalance(EmployeeLeaveBalanceInputs InputData)
        {
            // _configuration.GetConnectionString("SimplyConnectionDB")
            EmployeeLeaveBalanceInputs results = new EmployeeLeaveBalanceInputs();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            InputData.UnitId = unitId;
            results.GetEmployeeLeaveBalanceReport = await _ReportsAPIController.GetEmployeeLeaveBalanceReport(InputData);
            //  return View(objOutPut);
            //DataSet ds = new DataSet();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            results.DepartmentKeyValues = await _MastersKeyValueController.DepartmentKeyValue(x => x.IsActive == true && x.UnitId == unitId);
            results.WorkLocationKeyValues = await _MastersKeyValueController.WorkLocationKeyValue(x => x.IsActive == true && x.UnitId == unitId);

            results.FromMonth = InputData.FromMonth;
            results.ToMonth = InputData.ToMonth;
            results.FromYear = InputData.FromYear;
            results.ToYear = InputData.ToYear;
            results.DepartmentId = InputData.DepartmentId;
            results.LocationId = InputData.LocationId;
            //using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SimplyConnectionDB")))
            //{
            //    //   string query = "SELECT TOP 10 CustomerID,ContactName,City,Country FROM Customers";
            //    using (SqlCommand cmd = new SqlCommand("usp_BankTransferReport", con))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.Add("@EmpId", SqlDbType.Int).Value = 0;
            //        cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = HttpContext.Session.GetInt32("UnitId");
            //        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = Month;
            //        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = Year;

            //        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            //        {
            //            sda.Fill(ds);
            //        }

            //    }
            //    results.objBTRDSet = ds;
            //}


            return View("LeaveBalanceReport", results);
        }

        [HttpPost]
        //[Authorize(Roles = "Clientadmin")]
        public FileResult ExportRegimesSelectionReport(string RegimeSheet)
        {
            return File(Encoding.ASCII.GetBytes(RegimeSheet), "application/vnd.ms-excel", "RegimesSelectionReport.xls");
        }

        [HttpPost]
       // [Authorize(Roles = "Clientadmin")]
        public FileResult ExportBankTransferReport(string BtSheet)
        {
            return File(Encoding.ASCII.GetBytes(BtSheet), "application/vnd.ms-excel", "BankTransferReport.xls");
        }

        [HttpPost]
       // [Authorize(Roles = "Clientadmin")]
        public FileResult ExportPolicyAcceptanceReport(string PolicySheet)
        {
            return File(Encoding.ASCII.GetBytes(PolicySheet), "application/vnd.ms-excel", "PolicyAcceptanceReport.xls");
        }

        [HttpPost]
       // [Authorize(Roles = "Clientadmin")]
        public FileResult ExportConsolidatedReport(string DataSheet)
        {
            return File(Encoding.ASCII.GetBytes(DataSheet), "application/vnd.ms-excel", "ConsolidatedReport.xls");
        }

        [HttpPost]
      //  [Authorize(Roles = "Clientadmin")]
        public FileResult ExportLeaveBalanceReport(string LeaveSheet)
        {
            return File(Encoding.ASCII.GetBytes(LeaveSheet), "application/vnd.ms-excel", "LeaveBalanceReport.xls");
        }

        [HttpPost]
      //  [Authorize(Roles = "Clientadmin")]
        public FileResult ExportESICReport(string ESISheet)
        {
            return File(Encoding.ASCII.GetBytes(ESISheet), "application/vnd.ms-excel", "ESICReport.xls");
        }

        [HttpPost]
      //  [Authorize(Roles = "Clientadmin")]
        public FileResult ExportLWFReport(string LWFSheet)
        {
            return File(Encoding.ASCII.GetBytes(LWFSheet), "application/vnd.ms-excel", "LWFReport.xls");
        }

        [HttpPost]
       // [Authorize(Roles = "Clientadmin")]
        public FileResult ExportPTReport(string PTSheet)
        {
            return File(Encoding.ASCII.GetBytes(PTSheet), "application/vnd.ms-excel", "PTReport.xls");
        }

        [HttpPost]
       // [Authorize(Roles = "Clientadmin")]
        public FileResult ExportITReport(string ITSheet)
        {
            return File(Encoding.ASCII.GetBytes(ITSheet), "application/vnd.ms-excel", "ITDeductionsReport.xls");
        }

        [HttpPost]
        // [Authorize(Roles = "Clientadmin")]
        public FileResult ExportEmployeeLeaveBalanceReport(string LeaveSheet)
        {
            return File(Encoding.ASCII.GetBytes(LeaveSheet), "application/vnd.ms-excel", "EmployeeLeaveBalanceReport.xls");
        }
    }
}
