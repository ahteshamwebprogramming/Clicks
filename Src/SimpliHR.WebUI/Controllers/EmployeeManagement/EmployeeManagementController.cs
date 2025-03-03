using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Login;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Login;

namespace SimpliHR.WebUI.Controllers.Employee
{
    public class EmployeeManagementController : Controller
    {
        private readonly MastersKeyValueController _mastersKeyValueController;
        private readonly EmployeeMasterController _EmployeeAPIController;
        private readonly LoginController _loginAPIController;
        public EmployeeManagementController(EmployeeMasterController EmployeeAPIController, MastersKeyValueController mastersKeyValueController,LoginController loginController)
        {
            _EmployeeAPIController = EmployeeAPIController;
            _mastersKeyValueController = mastersKeyValueController;
            _loginAPIController = loginController;
        }
        public IActionResult Employee()
        {
            return View();
        }
        public IActionResult E_Employee()
        {
            return View();
        }
        public async Task<IActionResult> E_Joinee()
        {
            EmployeeMasterDTO outputData = new EmployeeMasterDTO();
            outputData.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue();
            return View("E_Joinee", outputData);
        }
        [HttpPost]
        public async Task<IActionResult> E_Joinee(EmployeeEJoineeDTO inputData)
        {
            inputData.IsActive = true;


            IActionResult actionResult;
            EmployeeMasterDTO viewModel = new EmployeeMasterDTO();
            actionResult = await _EmployeeAPIController.SaveEmployeeEJoinee(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;
            var objResultData = objResult.Value;

            if (objResultData != null)
            {
                viewModel.DisplayMessage = "Employee successfully created";
                //return View("E_Joinee", viewModel);
            }

            LoginDetailDTO loginDetailDTO = new LoginDetailDTO();
            loginDetailDTO.UserName=inputData.EmailId;
            loginDetailDTO.Password = "ABC";

             _loginAPIController.SaveLoginDetail(loginDetailDTO);

            BL.Employee blemp = new BL.Employee();
            UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
            if (blemp.SendJoiningLink(inputData, loginDetailDTO, unit))
            {
                viewModel.DisplayMessage = "Email Sent Successfully";
            }
            else
            {
                viewModel.DisplayMessage = "Unable to send Email";
            }
            viewModel.EmployeeMastersKeyValues = await _mastersKeyValueController.EmployeeMastersKeyValue();
            return View("E_Joinee", viewModel);
        }
        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
