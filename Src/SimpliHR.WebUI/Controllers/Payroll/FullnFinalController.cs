using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SimpliHR.Endpoints.ExcelUploads;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Endpoints;
using SimpliHR.WebUI.Controllers.ExcelUploads;
using SimpliHR.Infrastructure.Models.Masters;
using System.Net;
using SimpliHR.Infrastructure.Models.Payroll;

namespace SimpliHR.WebUI.Controllers.Payroll
{
    public class FullnFinalController : Controller
    {
        private readonly ExcelUDAPIController _excelUDAPIController;
        private readonly ILogger<FullnFinalController> _logger;
        public FullnFinalController(ILogger<FullnFinalController> logger, ExcelUDAPIController excelUDAPIController)
        {
            _logger = logger;          
            _excelUDAPIController = excelUDAPIController;
        }
       public async Task<IActionResult> FNFSettings()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            PayrollFullnFinalSettingsDTO outputData = new PayrollFullnFinalSettingsDTO();
            outputData.UnitId = unitId;

            IActionResult actionResult;

            actionResult = await _excelUDAPIController.GetSettlementMaster(outputData);
            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = (PayrollFullnFinalSettingsDTO)objResult.Value;
            if (objResultData.HttpStatusCode !=200)
            {
                objResultData.DisplayMessage = "Records not found";
            //return View("City", objResultData);
            }
            return View("FNFSettings", objResultData);
        }
        public IActionResult EmployeeFinalSettlement()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettlementSettings(PayrollFullnFinalSettingsDTO inputData)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsActive = true;

            if (inputData.SettingId == 0)
            {
                inputData.CreatedOn = DateTime.Now;
                inputData.CreatedBy =  Convert.ToInt32(employeeId);
            }
            else
            {
                inputData.ModifiedOn = DateTime.Now;
                inputData.ModifiedBy = Convert.ToInt32(employeeId);
            }

                IActionResult actionResult;
            PayrollFullnFinalSettingsDTO viewModel = new PayrollFullnFinalSettingsDTO();
            
                actionResult = _excelUDAPIController.SaveSattlementMaster(inputData);
            

            ObjectResult objResult = (ObjectResult)actionResult;
            inputData.HttpStatusCode = objResult.StatusCode;

            var objResultData = objResult.Value;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.SettingId == 0)
                    inputData.DisplayMessage = "Records successfully created";
                else
                    inputData.DisplayMessage = "Records updates completed successfully";
                
            }
            else
                inputData.DisplayMessage = objResultData.ToString();         
            viewModel = inputData;
            return View("FNFSettings", viewModel);

        }
    }
}
