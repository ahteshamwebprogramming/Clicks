using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.ExcelUploads;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Payroll;
using System.Data;
using System.Text;

namespace SimpliHR.WebUI.Controllers.Payroll
{
    public class ReportsController : Controller
    {
        private readonly ReportsAPIController _ReportsAPIController;
        public ReportsController(ReportsAPIController reportsAPIController)
        {
            _ReportsAPIController = reportsAPIController;
            
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
        public async Task<IActionResult> ExportBankTransfer(int? Year, int? Month)
        {
            SalaryProcessInputs results = new SalaryProcessInputs();
            DataSet ds = new DataSet();
            results.SalMonths = ((months[])Enum.GetValues(typeof(months))).Select(c => new SalaryMonths() { ID = (int)c, Name = c.ToString() }).ToList();
            // results.SalYears = ((SalYears[])Enum.GetValues(typeof(SalYears))).Select(c => new SalaryYears() { ID = (int)c, Name = c.ToString() }).ToList();
            results.Years = CommonHelper.GetYears(2000, DateTime.Now.Year - 2000 + 1);
            results.Month = Month;
            results.Year = Year;
            using (SqlConnection con = new SqlConnection("Data Source=tcp:mysimplyhr.database.windows.net,1433;Initial Catalog=SimpliHRdb;User ID=SimpliHR;Password=Delta@304020;Encrypt=False;"))
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
        public IActionResult ESICReport()
        {
            return View();
        }
        public IActionResult LWFReport()
        {
            return View();
        }
        public IActionResult PTReport()
        {
            return View();
        }
        public async Task<IActionResult> RegimesSelectionReport()
        {
            RegimeSelectionReportDTO objOutPut = new RegimeSelectionReportDTO();
            // List<RegimeSelectionReport> objResultData = new List<RegimeSelectionReport>();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            objOutPut.GetRegimeSelectionReport = await _ReportsAPIController.RegimeSelection(unitId);
            return View(objOutPut);
        }
        [HttpPost]
        public FileResult ExportRegimesSelectionReport(string RegimeSheet)
        {
            return File(Encoding.ASCII.GetBytes(RegimeSheet), "application/vnd.ms-excel", "RegimesSelectionReport.xls");
        }

        [HttpPost]
        public FileResult ExportBankTransferReport(string BtSheet)
        {
            return File(Encoding.ASCII.GetBytes(BtSheet), "application/vnd.ms-excel", "BankTransferReport.xls");
        }
    }
}
