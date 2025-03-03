using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.EmployeeSocialActivity;
using SimpliHR.Endpoints.ExcelUploads;
using SimpliHR.Endpoints.Exit;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Payroll;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.ITDeclaration;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Page;
using SimpliHR.Infrastructure.Models.Payroll;
using SimpliHR.Infrastructure.Models.ProjectTracker;
using SimpliHR.WebUI.BL;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace SimpliHR.WebUI.Controllers.ExitManagement
{
    public class ExitManagementController : Controller
    {
        private readonly ExitAPIController _exitAPIController;
        private readonly MastersKeyValueController _mastersKeyValueController;
        private readonly TemplateMasterAPIController _templateMasterAPIController;
        private readonly ClientController _ClientController;
        private readonly EmployeeMasterController _employeeAPIMasterController;
        public ExitManagementController(ExitAPIController exitAPIController, MastersKeyValueController mastersKeyValueController, TemplateMasterAPIController templateMasterAPIController, ClientController ClientManagementController, EmployeeMasterController employeeAPIMasterController)
        {
            _exitAPIController = exitAPIController;
            _mastersKeyValueController = mastersKeyValueController;
            _templateMasterAPIController = templateMasterAPIController;
            _ClientController = ClientManagementController;
            _employeeAPIMasterController = employeeAPIMasterController;
        }

        public async Task<IActionResult> ExitInitiation()
        {
            ExitViewModel exitViewModel = new ExitViewModel();
            if (HttpContext.Session.GetString("employee") != null)
            {
                EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

                int? unitId = empSession.UnitId;

                exitViewModel = await _exitAPIController.EmployeeExitInfo(empSession.EmployeeId);
                exitViewModel.CompanyEmployees = await _mastersKeyValueController.CompleteFilledInfoEmployeeKeyValue(x => x.UnitId == unitId, unitId);
                exitViewModel.ExitReasonList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "Exit" && x.PageName == "ExitInitiation" && x.ControlName == "ReasonForLeaving");
                if (empSession.ClientId != -1)
                {
                    exitViewModel.CompanyEmployees = exitViewModel.CompanyEmployees.Where(x => x.EmployeeId == empSession.EmployeeId).Select(x => x).ToList();
                }
                if (exitViewModel.ResignationDetails.ResignationListId == 0)
                {
                    exitViewModel.Action = "Add";
                    exitViewModel.ResignationDetails.EmployeeId = empSession.EmployeeId;
                    exitViewModel.ResignationDetails.ResignationInitiatedBy = empSession.EmployeeId;
                    exitViewModel.ResignationDetails.UnitId = unitId;
                }
                else
                    exitViewModel.Action = "Edit";
            }
            return View(exitViewModel);
        }

        public async Task<ExitViewModel> SaveResignationDetails(ExitViewModel exitVM)
        {
            string sMsg = await _exitAPIController.ValidateExitInfo(exitVM);
            if (string.IsNullOrEmpty(sMsg))
            {
                //int? unitId = exitVM.ResignationDetails.UnitId;
                //exitVM.ExitReasonList = await _mastersKeyValueController.PageControlKeyValues(x => x.Module == "Exit" && x.PageName == "ExitInitiation" && x.ControlName == "ReasonForLeaving");
                //exitVM.CompanyEmployees = await _mastersKeyValueController.CompleteFilledInfoEmployeeKeyValue(x => x.UnitId == unitId);
                //exitVM.ResignationDetails.EmployeeCode = exitVM.CompanyEmployees.Where(x => x.EmployeeId == exitVM.ResignationDetails.EmployeeId).Select(p => (string.IsNullOrEmpty(p.EmployeeCode) ? "" : p.EmployeeCode)).FirstOrDefault().ToString();

                EmployeeMasterDTO em = new EmployeeMasterDTO();
                int employeeId = exitVM.ResignationDetails.EmployeeId ?? default(int);
                em = await _employeeAPIMasterController.GetEmployeeById(employeeId);

                exitVM.ResignationDetails.EmployeeCode = em.EmployeeCode;
                exitVM.ResignationDetails.CreationDateEmployee = DateTime.Now;
                exitVM.ResignationDetails.IsAllFormalityCompleted = false;
                exitVM.ResignationDetails.IsResignationRolledBack = false;
                exitVM.ResignationDetails.SettlementStatus = 0;
                exitVM.ResignationDetails.InterviewStatus = 0;
                exitVM.ResignationDetails.ClearanceStatus = 0;
                exitVM = await _exitAPIController.SaveResignationDetails(exitVM);
            }
            return exitVM;
        }

        //[HttpGet]
        //[Route("ExitManagement/EmployeeExitInfo/{resignationListId:int}")]
        //public async Task<ActionResult> EmployeeExitInfo(int resignationListId)
        //{

        //    ExitViewModel exitViewModel = new ExitViewModel();


        //    if (resignationListId != null || resignationListId != 0)
        //    {
        //        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));


        //        int? unitId = empSession.UnitId;
        //        exitViewModel.ResignationDetails = await _exitAPIController.EmployeeExitInfo(resignationListId);
        //        exitViewModel.CompanyEmployees = await _mastersKeyValueController.CompleteFilledInfoEmployeeKeyValue(x => x.UnitId == unitId);
        //        if (empSession.ClientId != -1)
        //        {

        //            exitViewModel.CompanyEmployees = exitViewModel.CompanyEmployees.Where(x => x.EmployeeId == exitViewModel.ResignationDetails.EmployeeId).Select(x => x).ToList();
        //        }

        //        exitViewModel.Action = "Edit";
        //    }
        //    return View("ExitInitiation", exitViewModel);
        //}

        public async Task<ExitViewModel> GetResignationList1()
        {
            //EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            //if (empSession == null)
            //{
            //    Error error = new Error();
            //    error.Heading = "Session has expired";
            //    error.Message = "Please login again to resume your session";
            //    error.ButtonMessage = "Go To Login Page";
            //    error.ButtonURL = "/Account/Login";
            //    return View("../Page/Error", error);
            //}
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            ExitViewModel exitViewModel = new ExitViewModel();
            var res = await _exitAPIController.GetResignationList(unitId, 0, "");
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    var resInvestments80CMaster = ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (resInvestments80CMaster != null)
                    {
                        exitViewModel.ResignationList = (List<EmployeeExitResignationDTO>)resInvestments80CMaster;
                    }
                }
            }
            return exitViewModel;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ExitM()
        {
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            ExitViewModel exitViewModel = new ExitViewModel();
            var res = await _exitAPIController.GetResignationListByStatus(empSession.UnitId, empSession.EmployeeId, "Manager", "Pending");
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    exitViewModel.ResignationList = (List<EmployeeExitResignationDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }

                foreach (var item in exitViewModel.ResignationList)
                {
                    item.EncryptedEmployeeId = CommonHelper.EncryptURLHTML(item.EmployeeId.ToString());
                }
            }
            return View(exitViewModel);
        }
        public async Task<IActionResult> ViewEmployeeResignListManagerPartialView([FromBody] EmployeeExitResignationDTO resignationListDTO)
        {
            ExitViewModel exitViewModel = new ExitViewModel();
            try
            {
                EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
                if (empSession == null)
                {
                    Error error = new Error();
                    error.Heading = "Session has expired";
                    error.Message = "Please login again to resume your session";
                    error.ButtonMessage = "Go To Login Page";
                    error.ButtonURL = "/Account/Login";
                    return View("../Page/Error", error);
                }

                var res = await _exitAPIController.GetResignationListByStatus(empSession.UnitId, empSession.EmployeeId, "Manager", resignationListDTO?.Opt);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        exitViewModel.ResignationList = (List<EmployeeExitResignationDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    }

                    foreach (var item in exitViewModel?.ResignationList)
                    {
                        item.EncryptedEmployeeId = CommonHelper.EncryptURLHTML(item?.EmployeeId?.ToString());
                    }
                }
                return PartialView("_manager/_listOfTickets", exitViewModel);
            }
            catch (Exception ex)
            {
                return PartialView("_manager/_listOfTickets", exitViewModel);
            }
        }
        public async Task<IActionResult> ExitA()
        {
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            ExitViewModel exitViewModel = new ExitViewModel();
            //var res = await _exitAPIController.GetResignationList(empSession.UnitId, empSession.EmployeeId, "Admin");
            var res = await _exitAPIController.GetResignationListByStatus(empSession.UnitId, empSession.EmployeeId, "Admin", "Pending");
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    exitViewModel.ResignationList = (List<EmployeeExitResignationDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }

                foreach (var item in exitViewModel.ResignationList)
                {
                    item.EncryptedEmployeeId = CommonHelper.EncryptURLHTML(item.EmployeeId.ToString());
                }
            }
            return View(exitViewModel);
        }
        public async Task<IActionResult> ViewEmployeeResignListAdminPartialView([FromBody] EmployeeExitResignationDTO resignationListDTO)
        {
            ExitViewModel exitViewModel = new ExitViewModel();
            try
            {
                EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
                if (empSession == null)
                {
                    Error error = new Error();
                    error.Heading = "Session has expired";
                    error.Message = "Please login again to resume your session";
                    error.ButtonMessage = "Go To Login Page";
                    error.ButtonURL = "/Account/Login";
                    return View("../Page/Error", error);
                }

                var res = await _exitAPIController.GetResignationListByStatus(empSession.UnitId, empSession.EmployeeId, "Admin", resignationListDTO?.Opt);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        exitViewModel.ResignationList = (List<EmployeeExitResignationDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    }

                    foreach (var item in exitViewModel?.ResignationList)
                    {
                        item.EncryptedEmployeeId = CommonHelper.EncryptURLHTML(item?.EmployeeId?.ToString());
                    }
                }
                return PartialView("_admin/_listOfTickets", exitViewModel);
            }
            catch (Exception ex)
            {
                return PartialView("_admin/_listOfTickets", exitViewModel);
            }
        }


        public async Task<IActionResult> ViewEmployeeResignDetails_Manager([FromBody] EmployeeExitResignationDTO resignationListDTO)
        {
            ExitViewModel exitViewModel = new ExitViewModel();
            try
            {
                var res = await _exitAPIController.GetResignationDetails(resignationListDTO);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        exitViewModel.ResignationDetails = (EmployeeExitResignationDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                        if (exitViewModel.ResignationDetails != null) { exitViewModel.ResignationDetails.encResignationListId = CommonHelper.EncryptURLHTML(exitViewModel.ResignationDetails.ResignationListId.ToString()); }
                        exitViewModel.ExitReasonList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "Exit" && x.PageName == "ResignationViewManagerHR" && x.ControlName == "ReasonForLeaving");
                    }
                }
                return PartialView("_manager/_employeeresignDetailsEmployeeWise", exitViewModel);
            }
            catch (Exception ex)
            {
                return PartialView("_manager/_employeeresignDetailsEmployeeWise", exitViewModel);
            }

        }

        public async Task<IActionResult> SaveResignationDetailsByManager(EmployeeExitResignationDTO inputDTO)
        {
            if (inputDTO != null)
            {

                if (inputDTO.ResignationListId != null)
                {
                    if (inputDTO.DocumentFile != null)
                    {
                        if (inputDTO.DocumentFile.Length > 0)
                        {
                            inputDTO.DocumentName = Path.GetFileName(inputDTO.DocumentFile.FileName);
                            inputDTO.DocumentExtension = Path.GetExtension(inputDTO.DocumentName);
                            using (var target = new MemoryStream())
                            {
                                inputDTO.DocumentFile.CopyTo(target);
                                inputDTO.Document = target.ToArray();
                            }
                        }
                    }
                    var res = await _exitAPIController.SaveResignationDetailsByManager(inputDTO);
                    return res;
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
        public async Task<IActionResult> SaveResignationDetailsByAdmin(EmployeeExitResignationDTO inputDTO)
        {
            if (inputDTO != null)
            {

                if (inputDTO.ResignationListId != null)
                {
                    if (inputDTO.DocumentFileAdmin != null)
                    {
                        if (inputDTO.DocumentFileAdmin.Length > 0)
                        {
                            inputDTO.DocumentNameAdmin = Path.GetFileName(inputDTO.DocumentFileAdmin.FileName);
                            inputDTO.DocumentExtensionAdmin = Path.GetExtension(inputDTO.DocumentNameAdmin);
                            using (var target = new MemoryStream())
                            {
                                inputDTO.DocumentFileAdmin.CopyTo(target);
                                inputDTO.DocumentAdmin = target.ToArray();
                            }
                        }
                    }
                    var res = await _exitAPIController.SaveResignationDetailsByAdmin(inputDTO);
                    return res;
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


        public async Task<IActionResult> ViewEmployeeResignDetails_Admin([FromBody] EmployeeExitResignationDTO resignationListDTO)
        {
            ExitViewModel exitViewModel = new ExitViewModel();
            var res = await _exitAPIController.GetResignationDetails(resignationListDTO);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    exitViewModel.ResignationDetails = (EmployeeExitResignationDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (exitViewModel.ResignationDetails != null) { exitViewModel.ResignationDetails.encResignationListId = CommonHelper.EncryptURLHTML(exitViewModel.ResignationDetails.ResignationListId.ToString()); }
                    exitViewModel.ExitReasonList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "Exit" && x.PageName == "ResignationViewManagerHR" && x.ControlName == "ReasonForLeaving");
                }
            }
            return PartialView("_admin/_employeeresignDetailsEmployeeWise", exitViewModel);
        }
        //public async Task<IActionResult> ViewEmployeeResignDetails_Admin1([FromBody] EmployeeExitResignationDTO resignationListDTO)
        //{
        //    ExitViewModel exitViewModel = new ExitViewModel();
        //    var res = await _exitAPIController.GetResignationDetails(resignationListDTO);
        //    if (res != null)
        //    {
        //        if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //        {
        //            exitViewModel.ResignationDetails = (EmployeeExitResignationDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //        }
        //    }
        //    return PartialView("_admin/_employeeresignDetailsEmployeeWise", exitViewModel);
        //}


        public IActionResult ExitChecklist()
        {
            return View();
        }
        public IActionResult ExitSettlement()
        {
            return View();
        }
        public IActionResult ExitInterview()
        {
            return View();
        }
        public IActionResult ExitRecord()
        {
            return View();
        }

        [HttpGet]
        [Route("/ExitManagement/ExitClearance/{encEmployeeId}")]
        public async Task<IActionResult> ExitClearance(string encEmployeeId, string? encMessageId)
        {
            ExitViewModel exitViewModel = new ExitViewModel();
            int employeeId = 0;
            if (!String.IsNullOrEmpty(encEmployeeId))
            {
                employeeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(encEmployeeId));
            }
            exitViewModel.EmployeeId = employeeId;
            exitViewModel.encMessageId = encMessageId == null ? "" : encMessageId;
            exitViewModel.ExitClearanceStatusList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "Exit" && x.PageName == "ExitClearance" && x.ControlName == "AssetClearanceStatus");
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            //empSession.DepartmentId;
            exitViewModel.LoginUserUnitId = empSession.UnitId;
            exitViewModel.LoginEmployeeId = empSession.EmployeeId;
            exitViewModel.IsClient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
            exitViewModel.LoginUserDepartment = await _exitAPIController.DepartmentIdAssignedToOwner(exitViewModel.EmployeeId ?? default(int), empSession.EmployeeId);

            var res = await _exitAPIController.GetExitClearanceInfo(exitViewModel);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    exitViewModel = (ExitViewModel?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    exitViewModel.SelectedDepartments = string.Join(",", exitViewModel.EmployeeExitClearanceHeaderList.Select(x => x.DepartmentId));
                }
            }
            return View(exitViewModel);
        }


        public async Task<IActionResult> SaveExitClearanceInfo(ExitViewModel exitVM)
        {

            ExitViewModel exitViewModel = exitVM;
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            exitViewModel.ExitClearanceStatusList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "Exit" && x.PageName == "ExitClearance" && x.ControlName == "AssetClearanceStatus");
            //exitViewModel.EmployeeExitClearanceHeaderList.Where(x=>x.ClearanceMappingId in(exitVM.EmployeeExitClearanceHeaderList))
            var res = await _exitAPIController.SaveExitClearanceInfo(exitViewModel);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    exitViewModel = (ExitViewModel?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                }
            }
            return Ok(exitViewModel);
        }

        [HttpGet]
        [Route("/ExitManagement/InitiateExitClearance/{employeeEncId}")]
        public async Task<IActionResult> InitiateExitClearance(string employeeEncId)
        {
            int employeeId = 0;
            try
            {
                employeeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(employeeEncId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            ExitViewModel exitViewModel = new ExitViewModel();
            exitViewModel.EmployeeId = employeeId;
            exitViewModel.encEmployeeId = employeeEncId;
            var res = await _exitAPIController.GetExitClearanceAuthority(exitViewModel);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    exitViewModel = (ExitViewModel?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                }
            }
            return View(exitViewModel);

        }

        public async Task<IActionResult> SaveEmployeeExitClearanceAuthorities(ExitViewModel exitViewModel)
        {
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            exitViewModel.EmployeeExitClearanceList.ForEach(x => x.IsActive = true);
            exitViewModel.EmployeeExitClearanceList.ForEach(x => x.ModifiedBy = empSession.EmployeeCode);
            exitViewModel.EmployeeExitClearanceList.ForEach(x => x.ModifiedOn = DateTime.Now);
            exitViewModel.EmployeeMasterInfo = empSession;
            var res = await _exitAPIController.SaveEmployeeExitClearanceAuthorities(exitViewModel);

            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    exitViewModel = (ExitViewModel?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                }
            }
            return Ok(exitViewModel);

        }
        public IActionResult ExitInterviewForm()
        {
            return View();
        }

        public async Task<IActionResult> SaveExitInterviewFormComponent([FromBody] EmployeeExitInterViewFormMasterDTO inputDTO)
        {
            if (inputDTO != null)
            {
                EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
                int? unitId = empSession.UnitId;
                int? UserId = empSession.EmployeeId;
                inputDTO.UnitId = unitId;
                inputDTO.CreatedBy = UserId.ToString();
                inputDTO.CreatedDate = DateTime.Now;
                inputDTO.IsActive = true;
                if (inputDTO.EmployeeExitInterViewFormMasterId > 0)
                {
                    //if (inputDTO.DocumentFileAdmin != null)
                    //{
                    //    if (inputDTO.DocumentFileAdmin.Length > 0)
                    //    {
                    //        inputDTO.DocumentNameAdmin = Path.GetFileName(inputDTO.DocumentFileAdmin.FileName);
                    //        inputDTO.DocumentExtensionAdmin = Path.GetExtension(inputDTO.DocumentNameAdmin);
                    //        using (var target = new MemoryStream())
                    //        {
                    //            inputDTO.DocumentFileAdmin.CopyTo(target);
                    //            inputDTO.DocumentAdmin = target.ToArray();
                    //        }
                    //    }
                    //}
                    //var res = await _exitAPIController.SaveResignationDetailsByAdmin(inputDTO);
                    //return res;
                    var res = await _exitAPIController.SaveEmployeeExitInterviewFormComponent(inputDTO);
                    return res;

                    //return StatusCode(StatusCodes.Status500InternalServerError, "working on it");
                }
                else
                {
                    var res = await _exitAPIController.SaveEmployeeExitInterviewFormComponent(inputDTO);
                    return res;
                }

            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error has occurred. Please refresh the page and try again");
            }
        }
        public async Task<IActionResult> EmployeeExitInterview(string? encMessageId = null)
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (strEmpSession == null)
            {
                Error error1 = new Error();
                error1.Heading = "Session Expired";
                error1.Message = "You have been logged out. Please login again to continue";
                error1.ButtonMessage = "Login";
                error1.ButtonURL = "Account/Login";
                return View("../Page/Error", error1);
            }

            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(strEmpSession));
            if (empSession == null)
            {
                Error error1 = new Error();
                error1.Heading = "Session Expired";
                error1.Message = "You have been logged out. Please login again to continue";
                error1.ButtonMessage = "Login";
                error1.ButtonURL = "Account/Login";
                return View("../Page/Error", error1);
            }


            var res = await _exitAPIController.ResignationDetailsByEmployeeId(empSession.EmployeeId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    EmployeeExitResignationDTO? employeeExitResignationDTO = (EmployeeExitResignationDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (employeeExitResignationDTO != null)
                    {
                        if (employeeExitResignationDTO.ActivateExitInterview == true && employeeExitResignationDTO.InterviewStatus < 2)
                        {
                            ViewBag.encMessageId = encMessageId == null ? "" : encMessageId;
                            return View();
                        }
                    }
                }
            }
            Error error = new Error();
            error.Heading = "Exit Interview";
            error.Message = "You are not eligible for the exit interview for now";
            error.ButtonMessage = "Go back to the previous page";
            error.ButtonURL = "javascript:history.go(-1)";
            return View("../Page/Error", error);
        }
        public async Task<IActionResult> getInterviewFormComponent()
        {
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int? unitId = empSession.UnitId;
            var res = await _exitAPIController.GetExitInterviewFormComponent(unitId ?? default(int));
            return res;
        }

        public async Task<IActionResult> ExitClearanceMapping()
        {
            string? strEmpSession = HttpContext.Session.GetString("employee");
            if (string.IsNullOrEmpty(strEmpSession))
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            if (empSession == null)
            {
                Error error = new Error();
                error.Heading = "Session has expired";
                error.Message = "Please login again to resume your session";
                error.ButtonMessage = "Go To Login Page";
                error.ButtonURL = "/Account/Login";
                return View("../Page/Error", error);
            }

            ExitViewModel exitViewModel = new ExitViewModel();

            int? unitId = empSession.UnitId;
            exitViewModel.CompanyEmployees = await _mastersKeyValueController.CompleteFilledInfoEmployeeKeyValue(x => x.UnitId == unitId && x.IsActive == true && x.InfoFillingStatus == 1);
            exitViewModel.Departments = await _mastersKeyValueController.DepartmentKeyValueUnitWise(unitId ?? default(int));

            return View(exitViewModel);
        }

        public async Task<IActionResult> SaveClearanceMapping([FromBody] ExitClearanceMappingDTO inputDTO)
        {
            if (inputDTO != null)
            {
                EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
                int? unitId = empSession.UnitId;
                int? UserId = empSession.EmployeeId;
                inputDTO.UnitId = unitId;
                inputDTO.CreatedBy = UserId.ToString();
                inputDTO.IsActive = true;
                var res = await _exitAPIController.SaveClearanceMapping(inputDTO);
                return res;
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error has occurred. Please refresh the page and try again");
            }
        }
        public async Task<IActionResult> DeleteClearanceMapping([FromBody] ExitClearanceMappingDTO inputDTO)
        {
            ExitClearanceMappingDTO e = new ExitClearanceMappingDTO();
            var res = await _exitAPIController.DeleteClearanceMappingById(inputDTO);
            return res;
        }
        public async Task<IActionResult> GetClearanceMappingById([FromBody] ExitClearanceMappingDTO inputDTO)
        {
            ExitClearanceMappingDTO e = new ExitClearanceMappingDTO();
            var res = await _exitAPIController.GetClearanceMappingById(inputDTO);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    e = (ExitClearanceMappingDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                }
            }
            return res;
            //if (inputDTO != null)
            //{
            //    EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            //    int? unitId = empSession.UnitId;
            //    int? UserId = empSession.EmployeeId;
            //    inputDTO.UnitId = unitId;
            //    inputDTO.CreatedBy = UserId.ToString();
            //    inputDTO.IsActive = true;
            //    var res = await _exitAPIController.SaveClearanceMapping(inputDTO);
            //    return res;
            //}
            //else
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError, "Some error has occurred. Please refresh the page and try again");
            //}
        }
        public async Task<IActionResult> ExitClearanceMappingTable()
        {

            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int? unitId = empSession.UnitId;

            var res = await _exitAPIController.GetExitClearanceMappingTable(unitId ?? default(int));

            List<ExitClearanceMappingDTO> dto = new List<ExitClearanceMappingDTO>();

            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (List<ExitClearanceMappingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }


            return PartialView("_exitClearanceMapping/_exitClearanceMappingTable", dto);

        }
        public async Task<IActionResult> ViewExitClearanceAssetMappingPopup([FromBody] ExitClearanceAssetMappingDTO inputDTO)
        {

            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

            inputDTO.UnitId = empSession.UnitId;

            var res = await _exitAPIController.GetExitClearanceAssetMappingTable(inputDTO);

            List<ExitClearanceAssetMappingDTO> dto = new List<ExitClearanceAssetMappingDTO>();

            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (List<ExitClearanceAssetMappingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }


            return PartialView("_exitClearanceMapping/_exitClearanceAssetMapping", dto);

        }
        public async Task<IActionResult> MapAssets([FromBody] ExitViewModel inputDTO)
        {

            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));

            inputDTO.UnitId = empSession.UnitId;

            var res = await _exitAPIController.SaveAssetMapping(inputDTO);

            //List<ExitClearanceAssetMappingDTO> dto = new List<ExitClearanceAssetMappingDTO>();

            //if (res != null)
            //{
            //    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            //    {
            //        dto = (List<ExitClearanceAssetMappingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            //    }
            //}


            //return PartialView("_exitClearanceMapping/_exitClearanceAssetMapping", dto);

            return res;

        }
        public async Task<IActionResult> GetTemplateListExitInterviewPartial()
        {
            List<TemplateMasterDynamicDTO> dTOs = new List<TemplateMasterDynamicDTO>();
            var objRes = await _templateMasterAPIController.GetTemplateListExitInterview();
            if (objRes != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                {
                    var res = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).Value;
                    if (res != null)
                    {
                        dTOs = (List<TemplateMasterDynamicDTO>)res;
                    }
                }
            }
            return PartialView("_exitInterviewForm/_listMasterTemplate", dTOs);
        }
        public async Task<IActionResult> SaveEmployeeExitInterview([FromBody] EmployeeExitResignationDTO inputDTO)
        {
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int EmpId = empSession.EmployeeId;
            if (inputDTO != null)
            {
                inputDTO.EmployeeId = EmpId;
                var res = await _exitAPIController.UpdateEmployeeExitInterviewData(inputDTO);
                return res;
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Posted interview data is not valid");
            }
        }

        public ActionResult ExitInterviewResponses()
        {
            return View();
        }

        public async Task<ActionResult> GetExitInterviewResponsesTable([FromBody] InterviewResponses inputDTO)
        {
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            inputDTO.UnitId = empSession?.UnitId ?? default(int);
            EmployeeExitManagementViewModel outputDTO = new EmployeeExitManagementViewModel();

            var objRes = await _exitAPIController.GetInterviewResponsesList(inputDTO);
            var objRes1 = await _exitAPIController.GetExitInterviewFormComponent(inputDTO.UnitId);

            if (objRes != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                {
                    var res = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).Value;
                    if (res != null)
                    {
                        outputDTO.ExitResignationList = (List<EmployeeExitResignationDTO>)res;
                    }
                }
            }
            if (objRes1 != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes1).StatusCode == 200)
                {
                    var res1 = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes1).Value;
                    if (res1 != null)
                    {
                        outputDTO.ExitInterviewForm = (EmployeeExitInterViewFormMasterDTO)res1;
                    }
                }
            }

            BL.InterViewResponses interViewResponses = new BL.InterViewResponses();

            EmployeeExitManagementViewModel outputData = new EmployeeExitManagementViewModel();

            EmployeeExitManagementViewModel resData = interViewResponses.getInterViewResponses(outputDTO);

            outputData.ResponseComponent = resData.ResponseComponent;
            outputData.HeaderComponents = resData.HeaderComponents;

            return PartialView("_exitInterviewResponses/_list", outputData);
        }

        public async Task<ActionResult> DownloadExcel(DateTime? from, DateTime? to)
        {
            try
            {
                InterviewResponses inputDTO = new InterviewResponses();
                EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
                inputDTO.UnitId = empSession.UnitId ?? default(int);

                inputDTO.LastWorkingDateFrom = from;
                inputDTO.LastWorkingDateTo = to;

                EmployeeExitManagementViewModel outputDTO = new EmployeeExitManagementViewModel();

                var objRes = await _exitAPIController.GetInterviewResponsesList(inputDTO);
                var objRes1 = await _exitAPIController.GetExitInterviewFormComponent(inputDTO.UnitId);

                if (objRes != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                    {
                        var res = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).Value;
                        if (res != null)
                        {
                            outputDTO.ExitResignationList = (List<EmployeeExitResignationDTO>)res;
                        }
                    }
                }
                if (objRes1 != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes1).StatusCode == 200)
                    {
                        var res1 = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes1).Value;
                        if (res1 != null)
                        {
                            outputDTO.ExitInterviewForm = (EmployeeExitInterViewFormMasterDTO)res1;
                        }
                    }
                }

                BL.InterViewResponses interViewResponses = new BL.InterViewResponses();

                EmployeeExitManagementViewModel outputData = new EmployeeExitManagementViewModel();

                EmployeeExitManagementViewModel resData = interViewResponses.getInterViewResponses(outputDTO);

                resData.InterviewFilters = inputDTO;

                XLWorkbook wb = interViewResponses.CreateExcelFile(resData);

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "InterviewResponses.xlsx";

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    var content = MyMemoryStream.ToArray();
                    return File(content, contentType, fileName);
                }
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("/ExitManagement/EmployeeFinalSettlement/{employeeEncId}")]
        public async Task<IActionResult> EmployeeFinalSettlement(string employeeEncId)
        {
            int employeeId = 0;
            try
            {
                employeeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(employeeEncId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            // employeeId = 164;
            int unitId = (int)HttpContext.Session.GetInt32("UnitId");
            ExitViewModel exitViewModel = new ExitViewModel();
            EmployeeFnFDetailslDTO results = await _exitAPIController.GetEmployeeFullnFinalDetails(unitId, employeeId);
            if (results != null)
            {
                exitViewModel.Gratuity = results.Gratuity;
                exitViewModel.LeaveBalance = results.LeaveBalance;
                exitViewModel.NoticePeriod = results.NoticePeriod;

            }
            exitViewModel.EmployeeId = employeeId;
            exitViewModel.ExitClearanceStatusList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "Exit" && x.PageName == "ExitClearance" && x.ControlName == "AssetClearanceStatus");
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            exitViewModel.LoginUserDepartment = empSession.DepartmentId;
            exitViewModel.LoginUserUnitId = empSession.UnitId;
            var res = await _exitAPIController.GetExitClearanceInfo(exitViewModel);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    exitViewModel = (ExitViewModel?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                }
            }
            return View(exitViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> SaveFullnFinalSettlement(EmployeeFnFDetailslDTO inputData)
        {
            ExitViewModel viewModel = new ExitViewModel();
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            inputData.UnitId = unitId;
            inputData.IsMailSent = 0;
            inputData.IsProcess = 0;

            inputData.CreatedOn = DateTime.Now;
            inputData.CreatedBy = Convert.ToInt32(employeeId);

            IActionResult actionResult;
            //  EmployeeFnFDetailslDTO viewModel = new EmployeeFnFDetailslDTO();

            actionResult = await _exitAPIController.EmployeeFullnFinalDetails(inputData);


            ObjectResult objResult = (ObjectResult)actionResult;
            inputData.HttpStatusCode = objResult.StatusCode;

            var objResultData = objResult.Value;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.EmployeeFnFId == 0)
                    inputData.DisplayMessage = "1";
                else
                    inputData.DisplayMessage = "1";

            }
            else
                inputData.DisplayMessage = "2";

            viewModel.DisplayMessage = inputData.DisplayMessage;
            viewModel.NoticePeriod = inputData.NoticePeriod;
            viewModel.LeaveBalance = inputData.LeaveBalance;
            viewModel.Gratuity = inputData.Gratuity;
            viewModel.EmployeeId = inputData.EmployeeId;


            //viewModel.EmployeeId = Convert.ToInt32(employeeId);
            //viewModel.ExitClearanceStatusList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "Exit" && x.PageName == "ExitClearance" && x.ControlName == "AssetClearanceStatus");
            //EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            //viewModel.LoginUserDepartment = empSession.DepartmentId;
            //viewModel.LoginUserUnitId = empSession.UnitId;
            //var res = await _exitAPIController.GetExitClearanceInfo(viewModel);
            //if (res != null)
            //{
            //    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            //    {
            //        viewModel = (ExitViewModel?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

            //    }
            //}


            return View("EmployeeFinalSettlement", viewModel);

        }


        public async Task<IActionResult> FinalSettlementView(int employeeId)
        {
            // employeeId = 164;
            ExitViewModel exitViewModel = new ExitViewModel();
            EmployeeSettlementDetailslDTO objInput = new EmployeeSettlementDetailslDTO();
            exitViewModel.EmployeeId = employeeId;
            exitViewModel.UnitId = HttpContext.Session.GetInt32("UnitId");
            string? clientId = HttpContext.Session.GetString("ClientId");

            exitViewModel.objSettlementDetails = await _exitAPIController.GetEmployeeSettlementDetails(Convert.ToInt32(employeeId), Convert.ToInt32(exitViewModel.UnitId));
            if (exitViewModel.objSettlementDetails.Count > 0)
            {
                objInput = exitViewModel.objSettlementDetails.FirstOrDefault();
                exitViewModel.objPaySlipComponent = await _exitAPIController.GetPaySlipComponents(objInput);
            }

            ClientSettingDTO outputData = new ClientSettingDTO();
            //outputData.ClientId = Convert.ToInt32(clientId);
            outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(clientId));
            exitViewModel.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.ProfileImage, 0, outputData.ProfileImage.Length);


            return View(exitViewModel);

        }

        [HttpPost]
        public FileResult ExportFinalStatement(string FnFSheet)
        {
            return File(Encoding.ASCII.GetBytes(FnFSheet), "application/vnd.ms-excel", "FinalSettlement.xls");
        }

        public async Task<IActionResult> SaveEmployeeOtherPayments(int EmployeeId, int OtherPayments, int OtherDeductions, string Remarks)
        {
            EmployeeSettlementSummeryDTO inputDTO = new EmployeeSettlementSummeryDTO();
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            inputDTO.OtherPayments = OtherPayments;
            inputDTO.OtherDeductions = OtherDeductions;
            inputDTO.EmployeeId = EmployeeId;
            inputDTO.Remarks = Remarks;
            inputDTO.ProcessDate = DateTime.Now;
            inputDTO.IsFixed = false;
            inputDTO.IsMailSent = false;
            inputDTO.ProcessBy = Convert.ToInt32(employeeId);

            inputDTO.UnitId = HttpContext.Session.GetInt32("UnitId");


            var res = await _exitAPIController.SaveEmployeeSettlement(inputDTO, false);
            return res;
        }

        public async Task<IActionResult> ProcessFinalSettlement(int EmployeeId, int OtherPayments, int OtherDeductions, string Remarks)
        {
            EmployeeSettlementSummeryDTO inputDTO = new EmployeeSettlementSummeryDTO();
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            inputDTO.OtherPayments = OtherPayments;
            inputDTO.OtherDeductions = OtherDeductions;
            inputDTO.EmployeeId = EmployeeId;
            inputDTO.Remarks = Remarks;
            inputDTO.ProcessDate = DateTime.Now;
            inputDTO.IsFixed = true;
            inputDTO.IsMailSent = false;
            inputDTO.ProcessBy = Convert.ToInt32(employeeId);

            inputDTO.UnitId = HttpContext.Session.GetInt32("UnitId");


            var res = await _exitAPIController.SaveEmployeeSettlement(inputDTO, true);
            return res;
        }

        [HttpGet]
        [Route("/ExitManagement/SettlementDownload/{employeeEncId}")]
        public async Task<IActionResult> SettlementDownload(string employeeEncId)
        {

            int employeeId = 0;
            try
            {
                employeeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(employeeEncId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            //employeeId = 164;
            ExitViewModel exitViewModel = new ExitViewModel();
            EmployeeSettlementDetailslDTO objInput = new EmployeeSettlementDetailslDTO();
            exitViewModel.EmployeeId = employeeId;
            exitViewModel.UnitId = HttpContext.Session.GetInt32("UnitId");
            string? clientId = HttpContext.Session.GetString("ClientId");

            exitViewModel.objSettlementDetails = await _exitAPIController.GetEmployeeSettlementDetails(Convert.ToInt32(employeeId), Convert.ToInt32(exitViewModel.UnitId));
            if (exitViewModel.objSettlementDetails.Count > 0)
            {
                objInput = exitViewModel.objSettlementDetails.FirstOrDefault();
                exitViewModel.objPaySlipComponent = await _exitAPIController.GetPaySlipComponents(objInput);
            }

            ClientSettingDTO outputData = new ClientSettingDTO();
            //outputData.ClientId = Convert.ToInt32(clientId);
            outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(clientId));
            exitViewModel.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outputData.ProfileImage, 0, outputData.ProfileImage.Length);


            return View(exitViewModel);
        }


        [HttpGet]
        [Route("ExitManagement/DownloadResignationDocumentM/{eFileId}")]
        public async Task<IActionResult> DownloadResignationDocumentM(string eFileId)
        {
            EmployeeExitResignationDTO? inputDTO = new EmployeeExitResignationDTO();
            if (eFileId != null)
            {
                int FileID = inputDTO.ResignationListId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eFileId));
                var res = await _exitAPIController.GetResignationDetails(inputDTO);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    inputDTO = (EmployeeExitResignationDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (inputDTO != null)
                    {
                        var contentType = CommonHelper.getContentTypeByExtesnion(inputDTO.DocumentExtension == null ? "" : inputDTO.DocumentExtension);
                        if (contentType != null && inputDTO.Document != null)
                        {
                            return File(inputDTO.Document, contentType, inputDTO.DocumentName);
                        }
                    }
                }
            }
            return NotFound();
        }

        [HttpGet]
        [Route("ExitManagement/DownloadResignationDocumentA/{eFileId}")]
        public async Task<IActionResult> DownloadResignationDocumentA(string eFileId)
        {
            EmployeeExitResignationDTO? inputDTO = new EmployeeExitResignationDTO();
            if (eFileId != null)
            {
                int FileID = inputDTO.ResignationListId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eFileId));
                var res = await _exitAPIController.GetResignationDetails(inputDTO);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    inputDTO = (EmployeeExitResignationDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (inputDTO != null)
                    {
                        var contentType = CommonHelper.getContentTypeByExtesnion(inputDTO.DocumentExtensionAdmin == null ? "" : inputDTO.DocumentExtensionAdmin);
                        if (contentType != null && inputDTO.DocumentAdmin != null)
                        {
                            return File(inputDTO.DocumentAdmin, contentType, inputDTO.DocumentNameAdmin);
                        }
                    }
                }
            }
            return NotFound();
        }
        public async Task<IActionResult> MarkEmployeeResigned()
        {
            await _exitAPIController.MarkEmployeeResigned();
            return Ok();
        }

        #region Resignation Initiated By Admin
        public async Task<IActionResult> InitiateResignationAdmin()
        {
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            int unitId = empSession?.UnitId ?? default(int);

            ExitViewModel? exitViewModel = new ExitViewModel();

            var res = await _exitAPIController.GetActiveEmployeeForResignationList(unitId);

            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                exitViewModel.CompanyEmployees = (List<EmployeeKeyValues>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            exitViewModel.ExitReasonList = await _mastersKeyValueController.PageControlKeyValue(x => x.Module == "Exit" && x.PageName == "ResignationViewManagerHR" && x.ControlName == "ReasonForLeaving");
            return View(exitViewModel);
        }
        public async Task<IActionResult> EmployeeDetails_InitiateResignationAdmin([FromBody] EmployeeKeyValues inputDTO)
        {
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
            int unitId = empSession?.UnitId ?? default(int);

            ExitViewModel? exitViewModel = new ExitViewModel();
            exitViewModel = await _exitAPIController.EmployeeExitInfo(inputDTO.EmployeeId);
            if (exitViewModel?.ResignationDetails?.ResignationListId == 0)
            {
                exitViewModel.Action = "Add";
                exitViewModel.ResignationDetails.EmployeeId = inputDTO.EmployeeId;
                exitViewModel.ResignationDetails.ResignationInitiatedBy = empSession?.EmployeeId;
                exitViewModel.ResignationDetails.UnitId = unitId;
            }
            else
                exitViewModel.Action = "Edit";

            return Ok(exitViewModel);
        }

        public async Task<IActionResult> SaveEmployeeResignationDetailsByAdmin(EmployeeExitResignationDTO inputDTO)
        {
            if (inputDTO != null)
            {
                EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
                int unitId = empSession?.UnitId ?? default(int);
                ExitViewModel? exitViewModel = new ExitViewModel();
                var employeeInfo = await _exitAPIController.GetEmployeeInfoByEmployeeId(inputDTO.EmployeeId ?? default(int));


                if (employeeInfo != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)employeeInfo).StatusCode == 200)
                {
                    EmployeeMasterDTO? employeeMasterDTO = (EmployeeMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)employeeInfo).Value;
                    inputDTO.ResignationListId = 0;
                    inputDTO.UnitId = employeeMasterDTO?.UnitId ?? default(int);
                    inputDTO.EmployeeCode = employeeMasterDTO?.EmployeeCode;

                    inputDTO.ResignationInitiatedBy = empSession?.EmployeeId;
                    inputDTO.IsAllFormalityCompleted = false;
                    inputDTO.IsResignationRolledBack = false;
                    //inputDTO.ResignationDateManager = inputDTO.ResignationDate;
                    //inputDTO.LastWorkingDateManager = inputDTO.LastWorkingDate;
                    inputDTO.NoticePeriodWaiveOff = false;
                    inputDTO.EligibleToHire = false;
                    inputDTO.ResignationDateAdmin = inputDTO.ResignationDateAdmin;
                    inputDTO.LastWorkingDateAdmin = inputDTO.LastWorkingDateAdmin;
                    inputDTO.NoticePeriodWaiveOffAdmin = inputDTO.NoticePeriodWaiveOffAdmin;
                    inputDTO.EligibleToHireAdmin = inputDTO.EligibleToHireAdmin;
                    inputDTO.AdminApproval = 1;
                    inputDTO.AdminApprovalDate = DateTime.Now;
                    if (inputDTO.ActivateExitInterview == true)
                    {
                        inputDTO.InterviewStatus = 1;
                    }

                    if (inputDTO.DocumentFileAdmin != null)
                    {
                        if (inputDTO.DocumentFileAdmin.Length > 0)
                        {
                            inputDTO.DocumentNameAdmin = Path.GetFileName(inputDTO.DocumentFileAdmin.FileName);
                            inputDTO.DocumentExtensionAdmin = Path.GetExtension(inputDTO.DocumentNameAdmin);
                            using (var target = new MemoryStream())
                            {
                                inputDTO.DocumentFileAdmin.CopyTo(target);
                                inputDTO.DocumentAdmin = target.ToArray();
                            }
                        }
                    }

                    exitViewModel.ResignationDetails = inputDTO;
                    var res = await _exitAPIController.SaveResignationDetailsByAdmin(inputDTO);
                    return Ok(res);
                }
                return BadRequest("Error while saving resignation details");




                //var res = await _exitAPIController.SaveResignationDetailsByAdmin(inputDTO);
                //return res;
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Some error has occurred. Please refresh the page and try again");
            }




        }

        #endregion
    }
}