using Microsoft.AspNetCore.Mvc;

using SimpliHR.Endpoints.Leave;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Leave;
using System.Data.Common;
using System.Data;
using System.Net;
using System.Text;
using SimpliHR.WebUI.BL;
using SimpliHR.Core.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using SimpliHR.Infrastructure.Models.Masters;
using iTextSharp.text.pdf.codec.wmf;
using Newtonsoft.Json;
using SimpliHR.Infrastructure.Models.Employee;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Infrastructure.Models.Payroll;

namespace SimpliHR.WebUI.Controllers.Leave
{
    public class LeaveController : Controller
    {
        private readonly LeaveAPIController _leaveAPIController;
        private readonly MastersKeyValueController _mastersKeyValueController;
        private IWebHostEnvironment Environment;
        private readonly ClientController _ClientController;
        private static Random random = new Random();
        public LeaveController(MastersKeyValueController mastersKeyValueController, LeaveAPIController leaveAPIController, IWebHostEnvironment _environment, ClientController ClientManagementController)
        {
            _mastersKeyValueController = mastersKeyValueController;
            _leaveAPIController = leaveAPIController;
            Environment = _environment;
            _ClientController = ClientManagementController;

        }

        public async Task<IActionResult> LeaveYear()
        {

            LeaveCalenderYearDTO outputData = new LeaveCalenderYearDTO();

            if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                outputData.UnitId = 0;
            else
                outputData.UnitId = HttpContext.Session.GetInt32("UnitId");

            outputData.LeaveCalenderYearList = await GetLeaveCalenderList(outputData.UnitId);
            if (outputData != null)
            {
                foreach (var item in outputData.LeaveCalenderYearList)
                {
                    item.EncryptedId = CommonHelper.EncryptURLHTML(item.LeaveYearId.ToString());
                }
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<LeaveCalenderYearDTO>?> GetLeaveCalenderList(int? UnitId)
        {

            IActionResult actionResult = await _leaveAPIController.GetLeaveYearList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, UnitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<LeaveCalenderYearDTO> objResultData = (List<LeaveCalenderYearDTO>)objResult.Value;
            //foreach (var item in objResultData)
            //{
            //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.LeaveTypeId.ToString());
            //}
            return objResultData;
        }


        [HttpPost]
        public async Task<IActionResult> SaveLeaveCalender(LeaveCalenderYearDTO inputData)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Div = "Add";
            //    return View("LeaveType", inputData);
            //}
            inputData.IsActive = true;
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                inputData.UnitId = 0;
            else
                inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult;
            LeaveCalenderYearDTO viewModel = new LeaveCalenderYearDTO();
            //if (inputData.LeaveTypeId == 0)
            //    actionResult = _leaveAPIController.SaveLeaveYear(inputData);
            //else
            actionResult = _leaveAPIController.SaveLeaveYear(inputData);

            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;
            inputData.HttpStatusCode = objResult.StatusCode;

            if (inputData.HttpStatusCode == 200)
            {
                if (inputData.LeaveYearId == 0)
                    inputData.DisplayMessage = "Transaction Successful!";
                else
                    inputData.DisplayMessage = "Transaction Successful!";
                inputData.LeaveYearId = 0;
                inputData.LeaveCalenderYearList = await GetLeaveCalenderList(inputData.UnitId);
            }
            else
                inputData.DisplayMessage = objResultData.ToString();
            viewModel = inputData;
            return View("LeaveYear", viewModel);

        }



        [HttpGet]
        [Route("Leave/GetLeaveCalenderInfo/{LeaveYearId}")]
        public async Task<IActionResult> GetLeaveCalenderInfo(string LeaveYearId)
        {
            int leaveTypeId = 0;
            try
            {
                leaveTypeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(LeaveYearId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (leaveTypeId != 0)
            {
                LeaveCalenderYearDTO outputData = new LeaveCalenderYearDTO();
                outputData.LeaveYearId = leaveTypeId;
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                    outputData.UnitId = 0;
                else
                    outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
                IActionResult actionResult;

                actionResult = await _leaveAPIController.GetLeaveYear(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (LeaveCalenderYearDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    return View("LeaveYear", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.LeaveYearId = 0;
                    objResultData.LeaveCalenderYearList = await GetLeaveCalenderList(outputData.UnitId);
                    objResultData.DisplayMessage = "You cannot edit locked leave. Contact Admin for further details";
                    return View("LeaveYear", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("LeaveYear", "Leave");
        }

        [HttpGet]
        [Route("Leave/DeleteLeaveCalender/{LeaveYearId}")]
        public async Task<IActionResult> DeleteLeaveCalender(string LeaveYearId)
        {
            int leaveTypeId = 0;
            try
            {
                leaveTypeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(LeaveYearId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (leaveTypeId != 0)
            {

                LeaveCalenderYearDTO outputData = new LeaveCalenderYearDTO();
                outputData.LeaveYearId = leaveTypeId;
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                    outputData.UnitId = 0;
                else
                    outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
                IActionResult actionResult;

                actionResult = await _leaveAPIController.DeleteLeaveYear(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.LeaveYearId = 0;
                outputData.LeaveCalenderYearList = await GetLeaveCalenderList(outputData.UnitId);
                outputData.DisplayMessage = "Transaction Successful!";
                return View("LeaveYear", outputData);
                //}
            }
            return RedirectToAction("LeaveYear", "Leave");
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public async Task<IActionResult> LeaveAttributes()
        {
            
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {

                LeaveAttributeDTO leaveAttributeDTO = new LeaveAttributeDTO();
                leaveAttributeDTO.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);
                leaveAttributeDTO.LeaveAttributeList = await GetLeaveAttributeList();
                return View(leaveAttributeDTO);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<List<LeaveAttributeDTO>?> GetLeaveAttributeList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _leaveAPIController.GetLeaveAttributes(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<LeaveAttributeDTO> objResultData = (List<LeaveAttributeDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.Encrypt(Convert.ToString(item.LeaveAttributeId));
            }
           return objResultData;
        }

        [HttpGet]
        public async Task<List<LeaveTypeKeyValues>>? GetLeaveTypeAttributes(int LeaveTypeId)
        {
            return await _mastersKeyValueController.LeaveTypeKeyValue(x => x.LeaveTypeId == LeaveTypeId);
        }

        [HttpPost]
        public async Task<IActionResult> LeaveAttributes(LeaveAttributeDTO inputData)
        {

            LeaveAttributeDTO viewModel = new LeaveAttributeDTO();
            try
            {
                int clientId;

                if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
                {
                    return RedirectToAction("Login", "Account");
                }
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                if (unitId != null)
                {
                    inputData.IsActive = true;
                    inputData.UnitId = unitId;
                    IActionResult actionResult;

                    if (inputData.LeaveAttributeId == 0)
                        //throw new Exception("Testing the error message");
                        actionResult = _leaveAPIController.SaveLeaveAttribute(inputData);
                    else
                        actionResult = _leaveAPIController.UpdateLeaveAttribute(inputData);

                    ObjectResult objResult = (ObjectResult)actionResult;

                    var objResultData = objResult.Value;
                    inputData.HttpStatusCode = objResult.StatusCode;
                    inputData.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);

                    if (inputData.HttpStatusCode == 200)
                    {
                        if (inputData.LeaveAttributeId == 0)
                            inputData.DisplayMessage = "Transaction Successful!";
                        else
                            inputData.DisplayMessage = "Transaction Successful!";
                        inputData.LeaveAttributeId = 0;
                        inputData.LeaveAttributeList = await GetLeaveAttributeList();

                    }
                    else
                        inputData.DisplayMessage = objResultData.ToString();
                    viewModel = inputData;
                    return View("LeaveAttributes", viewModel);
                }

                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", "Invalid Username or Password");
                return View("LeaveAttributes", viewModel);
            }



            //int? unitId = HttpContext.Session.GetInt32("UnitId");
            //if (unitId != null)
            //{
            //    inputData.UnitId=unitId;
            //    IActionResult actionResult = _leaveAPIController.SaveLeaveAttribute(inputData);
            //    ObjectResult objResult = (ObjectResult)actionResult;
            //    var objResultData = objResult.Value;
            //    inputData.HttpStatusCode = objResult.StatusCode;

            //    if (inputData.HttpStatusCode == 200)
            //    {
            //        if (inputData.CountryId == 0)
            //            inputData.DisplayMessage = "Country successfully created";
            //        else
            //            inputData.DisplayMessage = "Country updates completed successfully";
            //        inputData.CountryId = 0;
            //        inputData.CountryMasterList = await GetCountryList();

            //    }
            //    else
            //        inputData.DisplayMessage = objResultData.ToString();
            //    viewModel = inputData;
            //    return View("Country", viewModel);


            //    //return View(objResult);
            //}
            //return View(inputData);
        }

        [HttpGet]
        [Route("Leave/GetLeaveAttributeInfo/{eleaveAttributeId}")]
        public async Task<IActionResult> GetLeaveAttributeInfo(string eleaveAttributeId)
        {
            int leaveAttributeId = 0;
            try
            {
                leaveAttributeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eleaveAttributeId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {
                if (leaveAttributeId != 0)
                {
                    LeaveAttributeDTO outputData = new LeaveAttributeDTO();
                    outputData.LeaveAttributeId = leaveAttributeId;

                    IActionResult actionResult;

                    actionResult = await _leaveAPIController.GetLeaveAttribute(outputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    var objResultData = (LeaveAttributeDTO)objResult.Value;
                    if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                    {
                        objResultData.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);
                        return View("LeaveAttributes", objResultData);
                        //return RedirectToAction("Role","Role", objResultData);
                    }
                    else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                    {
                        objResultData.LeaveAttributeId = 0;
                        objResultData.LeaveAttributeList = await GetLeaveAttributeList();
                        objResultData.DisplayMessage = "You cannot edit locked attribute. Contact Admin for further details";
                        return View("LeaveAttribute", objResultData);
                        //return RedirectToAction("Role", objResultData);
                    }
                }
                return RedirectToAction("LeaveAttribute", "Leave");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpGet]
        [Route("Leave/DeleteLeaveAttribute/{eLeaveAttributeId}")]
        public async Task<IActionResult> DeleteLeaveAttribute(string eLeaveAttributeId)
        {
            int leaveAttributeId = 0;
            try
            {
                leaveAttributeId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eLeaveAttributeId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (leaveAttributeId != 0)
            {

                LeaveAttributeDTO outputData = new LeaveAttributeDTO();
                outputData.LeaveAttributeId = leaveAttributeId;

                IActionResult actionResult;

                actionResult = await _leaveAPIController.DeleteLeaveAttribute(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                int clientId;
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) == false || unitId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                outputData.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);

                return RedirectToAction("LeaveAttributes", "Leave");
                //outputData.CountryId = 0;
                //outputData.CountryMasterList = await GetCountryList();
                //outputData.DisplayMessage = "Country record deactivated successfully";
                //return View("Country", outputData);
                //}
            }
            return RedirectToAction("Country", "Country");
        }

        [HttpPost]
        public async Task<IActionResult> SaveEmployeeLeaveDetails(List<IFormFile> formFile,EmployeeLeaveDetailsDTO inputData)
        {
            //MailHelper.SendMailByGupShapAPI(null,null,null,null,null);

            EmployeeLeaveDetailsDTO viewModel = new EmployeeLeaveDetailsDTO();
            ClientSettingDTO outputData = new ClientSettingDTO();
            UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
            inputData.DisplayName = unit.EmailDisplayName;

            outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));
          inputData.EmailProvider = outputData.EmailProvider;

            // string folderpath = Path.Combine("ClientLogo", outputData.ClientId.ToString(), outputData.ClientLogo);
            string folderpath = "ClientLogo/"+outputData.ClientId.ToString()+"/"+outputData.ClientLogo;
            inputData.Profile = folderpath;
           if (string.IsNullOrEmpty(Convert.ToString(inputData.NoOfLeave)))
                inputData.NoOfLeave = 1;

            string? strEmpSession = HttpContext.Session.GetString("employee");
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            int? genderId = empSession.GenderId;

          //  inputData.Profile = this.Environment.WebRootPath;
            if (formFile.Count > 0)
            {
             //   int? unitId = HttpContext.Session.GetInt32("UnitId");
                //string contentPath = this.Environment.ContentRootPath;

                string path = Path.Combine(this.Environment.WebRootPath, "Bills");

                List<string> uploadedFiles = new List<string>();
                foreach (IFormFile postedFile in formFile)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    fileName = inputData.EmployeeId + '_' + fileName;

                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                        uploadedFiles.Add(fileName);
                        // ViewBag.Message += fileName + ",";
                    }
                    inputData.BillName = fileName;
                    inputData.IsBillRequired = true;
                }
            }
            else
            {
                inputData.BillName = "";
                inputData.IsBillRequired = false;
            }
            inputData.LeaveStatus = 1;
        
            inputData.TicketId = CommonHelper.CreateTicket("Leave", "");

            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            string? clientId = HttpContext.Session.GetString("ClientId");
            inputData.EmployeeId = Convert.ToInt32(employeeId);
            inputData.CreatedBy = employeeId;
            inputData.CreatedOn = DateTime.Now;
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                inputData.UnitId = 0;
            else
                inputData.UnitId = HttpContext.Session.GetInt32("UnitId");
           
           
            int cId= Convert.ToInt32(clientId);
            string sRetMsg  = await _leaveAPIController.SaveEmployeeLeaveDetails(inputData);
                      
            inputData = await _leaveAPIController.GetLeaveforReversal(0, Convert.ToInt32(employeeId), true);

            inputData.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, cId, empSession.UnitId);

            // view
            IActionResult actionResult = await _leaveAPIController.GetEmployeeLeaveSummary(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, Convert.ToInt32(employeeId),genderId);
            ObjectResult objResult = (ObjectResult)actionResult;
            inputData.EmployeeLeaveSummary = (List<EmployeeLeaveBalanceDTO>)objResult.Value;
            inputData.DisplayMessage = sRetMsg;

            viewModel = inputData;
            return View("LeaveApp", viewModel);

        }

        public int GetWorkingDays(DateTime from, DateTime to)
        {
            var totalDays = 0;
            //for (var date = from; date < to; date = date.AddDays(1))
            for (var date = from; date <= to; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    totalDays++;
            }

            return totalDays;
        }
        public static string GenerateTicket(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //[HttpGet]
        //[Route("Leave/GetEmployeeLeaveDetailsInfo/{LeaveDetailsId}")]
        //public async Task<IActionResult> GetEmployeeLeaveDetailsInfo(string LeaveDetailsId)
        //{
        //    int EmpLeaveDetailsId = Convert.ToInt32(CommonHelper.DecryptURLHTML(LeaveDetailsId));
        //    if (EmpLeaveDetailsId != 0)
        //    {
        //        EmployeeLeaveDetailsDTO outputData = new EmployeeLeaveDetailsDTO();
        //        outputData.LeaveDetailsId = EmpLeaveDetailsId;
        //        IActionResult actionResult;

        //        actionResult = await _leaveAPIController.GetLeaveDetailsbyId(outputData);
        //        ObjectResult objResult = (ObjectResult)actionResult;
        //        var objResultData = (LeaveCalenderYearDTO)objResult.Value;
        //        if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
        //        {
        //            return View("LeaveApply", objResultData);
        //            //return RedirectToAction("Role","Role", objResultData);
        //        }
        //        else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
        //        {
        //            objResultData.LeaveYearId = 0;
        //            // objResultData.LeaveCalenderYearList = await GetLeaveCalenderList(outputData.UnitId);
        //            objResultData.DisplayMessage = "You cannot edit locked leave. Contact Admin for further details";
        //            return View("LeaveApply", objResultData);
        //            //return RedirectToAction("Role", objResultData);
        //        }
        //    }
        //    return RedirectToAction("LeaveApply", "Leave");
        //}


        public async Task<IActionResult> LeaveView()
        {

            string? strEmpSession = HttpContext.Session.GetString("employee");
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            int? genderId = empSession.GenderId;
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {
                EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
                EmployeeLeaveDetail = await _leaveAPIController.GetLeaveforReversal(0, Convert.ToInt32(employeeId), true);
              
                EmployeeLeaveDetail.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);


                IActionResult actionResult = await _leaveAPIController.GetEmployeeLeaveSummary(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, Convert.ToInt32(employeeId), genderId);
                ObjectResult objResult = (ObjectResult)actionResult;

                EmployeeLeaveDetail.EmployeeLeaveSummary = (List<EmployeeLeaveBalanceDTO>)objResult.Value;
                // EmployeeLeaveDetail.LeaveDetailsId = 0;

                return View(EmployeeLeaveDetail);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

           
        }
        public async Task<IActionResult> LeaveApply()
        {

           // return View();
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");          
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {

                EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
                EmployeeLeaveDetail.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);
                return View(EmployeeLeaveDetail);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> CompOff()
        {

            // return View();
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {

                LeaveCompOffDTO EmployeeLeaveDetail = new LeaveCompOffDTO();
              //  leaveAttributeDTO.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);
                EmployeeLeaveDetail.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);
                EmployeeLeaveDetail.LeaveCompOffList = await GetLeaveCompOffList();

                foreach (var item in EmployeeLeaveDetail.LeaveCompOffList)
                {
                     item.EncryptedId = CommonHelper.EncryptURLHTML(item.CompOffId.ToString());
                  //  item.CalendarName = EmployeeLeaveDetail.LeaveAttributeKeyValues.LeaveCalenderYearKeyValue.Where(x => x.LeaveYearId == item.CalendarYear).Select(r => r.CalendarName).FirstOrDefault();
                }
                return View(EmployeeLeaveDetail);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> EmployeeCompOff()
        {

            // return View();
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {

                EmployeeCompOffDTO EmployeeLeaveDetail = new EmployeeCompOffDTO();
               // EmployeeLeaveDetail.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);
                EmployeeLeaveDetail.EmployeeCompOffList = await GetEmployeeCompOffList();

                foreach (var item in EmployeeLeaveDetail.EmployeeCompOffList)
                {
                    item.EncryptedId = CommonHelper.EncryptURLHTML(item.CompOffId.ToString());
                  //  item.CalendarName = EmployeeLeaveDetail.LeaveAttributeKeyValues.LeaveCalenderYearKeyValue.Where(x => x.LeaveYearId == item.CalendarYear).Select(r => r.CalendarName).FirstOrDefault();
                }
                return View(EmployeeLeaveDetail);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public async Task<IActionResult> LeaveReverse()
        {
            bool isEmployee = true;
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            EmployeeLeaveDetail = await _leaveAPIController.GetLeaveforReversal(0,Convert.ToInt32(employeeId), isEmployee);

            return View(EmployeeLeaveDetail);
        }

        public async Task<IActionResult> LeaveReversal()
        {
            bool isEmployee = true;
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            EmployeeLeaveDetail = await _leaveAPIController.GetLeaveforReversal(0,Convert.ToInt32(employeeId), isEmployee);

            return View(EmployeeLeaveDetail);
        }

        public async Task<IActionResult> RegularizeLeave()
        {
            int? isAdmin = 0;
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            bool isClient = Convert.ToBoolean(HttpContext.Session.GetString("isClient"));
            if (isClient)
                isAdmin = 1;
            else
                isAdmin = 0;

            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            EmployeeLeaveDetail = await _leaveAPIController.GetLeavePendingForApproval( Convert.ToInt32(employeeId), unitId, isAdmin);

            return View(EmployeeLeaveDetail);
        }
        public async Task<IActionResult> RegularizeCompOff()
        {
           //string? employeeId = HttpContext.Session.GetString("EmployeeId");
            //EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            //EmployeeLeaveDetail = await _leaveAPIController.GetLeavePendingForApproval(Convert.ToInt32(employeeId));
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {

                EmployeeCompOffDTO EmployeeLeaveDetail = new EmployeeCompOffDTO();
                // EmployeeLeaveDetail.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);
                EmployeeLeaveDetail.EmployeeCompOffList = await GetEmployeeCompOffPendingList();

                foreach (var item in EmployeeLeaveDetail.EmployeeCompOffList)
                {
                    item.EncryptedId = CommonHelper.EncryptURLHTML(item.CompOffId.ToString());
                    //  item.CalendarName = EmployeeLeaveDetail.LeaveAttributeKeyValues.LeaveCalenderYearKeyValue.Where(x => x.LeaveYearId == item.CalendarYear).Select(r => r.CalendarName).FirstOrDefault();
                }
                return View(EmployeeLeaveDetail);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
         //   return View(EmployeeLeaveDetail);
        }

        [HttpGet]
        public async Task<EmployeeLeaveStatus> GetLeaveBalance(int leaveTypeId)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            LeaveAttributeDTO inputDTO = new LeaveAttributeDTO();
            EmployeeLeaveStatus OutputDTO = new EmployeeLeaveStatus();
            inputDTO.LeaveTypeId = leaveTypeId;
            //inputDTO.EmployeeId = Convert.ToInt32(employeeId);
            inputDTO.UnitId = unitId;
            IActionResult actionResult;
            actionResult = await _leaveAPIController.GetLeaveBalance(inputDTO, employeeId);
            ObjectResult objResult = (ObjectResult)actionResult;
            //var val1 = (bool)objResult.
            LeaveAttributeDTO objResultData = (LeaveAttributeDTO)objResult.Value;
             OutputDTO.NoOfLeave = objResultData.MinLeaveNoForBill;
            OutputDTO.LeaveBalance = objResultData.LeaveBalance;
            OutputDTO.IsBill = objResultData.IsBillRequired;
            OutputDTO.MinAvailLimit = objResultData.MinAvailLimit;
            OutputDTO.IsHalfDay = objResultData.IsHalfDay;
            OutputDTO.MaxAvailLimit = objResultData.MaxAvailLimit;
            OutputDTO.DisplayMessage = objResultData.DisplayMessage;
            OutputDTO.WeekOffType = objResultData.WeekOffType;
            OutputDTO.InterveningHolidays = objResultData.InterveningHolidays;
            OutputDTO.NoOfMonth = objResultData.NoOfMonth;
            // OutputDTO.IsBill = objResultData.IsBillRequired;
            return OutputDTO;
        }


        [HttpGet]
        public async Task<EmployeeLeaveDetailsDTO> GetLeaveDetails(int LeaveTypeId)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            EmployeeLeaveDetail = await _leaveAPIController.GetEmployeeLeaveDetails(LeaveTypeId, Convert.ToInt32(employeeId));

            return EmployeeLeaveDetail;
        }

        [HttpGet]
        public async Task<EmployeeLeaveDetailsDTO> GetLeaveTicketDetails(string ticketId,string moduleId)
        {
            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            EmployeeLeaveDetail = await _leaveAPIController.GetEmployeeTicketDetails(ticketId, moduleId);

            return EmployeeLeaveDetail;
        }

        [HttpGet]
        public async Task<EmployeeLeaveDetailsDTO> GetPendingLeaveDetails(int LeaveTypeId)
        {
            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            EmployeeLeaveDetail = await _leaveAPIController.GetPendingLeaveDetails(LeaveTypeId);

            return EmployeeLeaveDetail;
        }

        [HttpGet]
        public async Task<EmployeeLeaveDetailsDTO> GetEmployeePendingLeaveDetails(int LeaveTypeId)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            EmployeeLeaveDetail = await _leaveAPIController.GetEmployeePendingLeaveDetails(LeaveTypeId,Convert.ToInt32(employeeId));

            return EmployeeLeaveDetail;
        }
        [HttpPost]
        public async Task<IActionResult> LeaveRegularizeProcessing(LeaveAction userAction)
        {
          //  ClientSettingDTO outputData = new ClientSettingDTO();
            UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
            userAction.DisplayName = unit.EmailDisplayName;

            ClientSettingDTO outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(unit.ClientId));
            userAction.EmailProvider = outputData.EmailProvider;

          //  string folderpath = Path.Combine(this.Environment.WebRootPath, "EmployeePofile");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string folderpath = "ClientLogo/" + outputData.ClientId.ToString() + "/" + outputData.ClientLogo;
            userAction.Profile = folderpath;
            //  string path = Path.Combine(folderpath, Convert.ToString(unitId));
           // userAction.Profile = path;
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            LeaveAction manualPunchVM = new LeaveAction();
            userAction.ApprovedBy = employeeId;
            if (userAction.LeaveIds != null && userAction.LeaveIds.Length != 0)
            {
                string sRetMsg = await _leaveAPIController.LeaveRegularizeProcessing(userAction);
               manualPunchVM.DisplayMessage = sRetMsg;
            }
            else
              manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
            return Ok(manualPunchVM.DisplayMessage);
        }

        [HttpPost]
        public async Task<IActionResult> CompOffRegularizeProcessing(CompOffAction userAction)
        {
           // string folderpath = Path.Combine(this.Environment.WebRootPath, "EmployeePofile");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
          //  string path = Path.Combine(folderpath, Convert.ToString(unitId));
          //  userAction.Profile = path;
            string? employeeId = HttpContext.Session.GetString("EmployeeId");          
            CompOffAction manualPunchVM = new CompOffAction();
            userAction.ApprovedBy = employeeId;
            //userAction.TicketId = CommonHelper.CreateTicket("Comp", "");
            if (userAction.CompOffIds != null && userAction.CompOffIds.Length != 0)
            {
                string sRetMsg = await _leaveAPIController.CompOffRegularizeProcessing(userAction);
                manualPunchVM.DisplayMessage = sRetMsg;
            }
            else
                manualPunchVM.DisplayMessage = "Select compoff " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
            return Ok(manualPunchVM.DisplayMessage);
        }

        [HttpGet]
        [Route("/Leave/LeaveRegularizeProcess/{Ids}&{ua}")]
        public async Task<IActionResult> LeaveRegularizeProcess(string Ids, string ua)
        {

          //  leaveCompOffId = Convert.ToInt32(CommonHelper.DecryptURLHTML(leaveId));
            LeaveAction leaveVM = new LeaveAction();
            leaveVM.ApprovedBy = "0";
            leaveVM.LeaveIds = CommonHelper.DecryptURLHTML(Ids);
            leaveVM.ActionType = ua;
            if (ua.Trim() =="A")
                leaveVM.ActionRemarks = "Approved";
                 else
                leaveVM.ActionRemarks = "Rejected";


            if (leaveVM.LeaveIds != null && leaveVM.LeaveIds.Length != 0)
            {
                string sRetMsg = await _leaveAPIController.LeaveRegularizeProcessing(leaveVM);
                leaveVM.DisplayMessage = sRetMsg;
            }
            else
                leaveVM.DisplayMessage = "Select leave " + (leaveVM.ActionType == "A" ? "for approval" : "to be reject");
            return Ok(leaveVM.DisplayMessage);
        }


        [HttpPost]
        public async Task<IActionResult> LeaveReversalProcessing(LeaveAction userAction)
        {
            int clientId;
            int unitId = (int)HttpContext.Session.GetInt32("UnitId");
            UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
            userAction.DisplayName = unit.EmailDisplayName;

           // inputData.TicketId = CommonHelper.CreateTicket("Leave", "");
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {
                ClientSettingDTO outputData = await _ClientController.GetClientSettingDetails(clientId);
                string folderpath = "ClientLogo/" + outputData.ClientId.ToString() + "/" + outputData.ClientLogo;
                userAction.Profile = folderpath;
                userAction.EmailProvider = outputData.EmailProvider;
            }
            else
            {
                userAction.Profile = "";
            }
            LeaveAction manualPunchVM = new LeaveAction();
            userAction.EmployeeId = Convert.ToInt32(employeeId);
            userAction.ApprovedBy = employeeId;
           // userAction.TicketId= "Leave/" + DateTime.Now.Month + "/" + GenerateTicket(6);
            if (userAction.LeaveIds != null && userAction.LeaveIds.Length != 0)
            {
                string sRetMsg = await _leaveAPIController.LeaveReversalProcessing(userAction);
                manualPunchVM.DisplayMessage = sRetMsg;
            }
            else
                manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
            return Ok(manualPunchVM.DisplayMessage);
        }
        public async Task<IActionResult> LeaveApp()
        {
            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            string? strEmpSession = HttpContext.Session.GetString("employee");
            EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(strEmpSession));
            int? genderId = empSession.GenderId;
           
            // var rndnumbers= "Leave/" + DateTime.Now.Month + "/" + GenerateTicket(6);
            int clientId;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {
                string? employeeId = HttpContext.Session.GetString("EmployeeId");
              

                // Reversal 
                EmployeeLeaveDetail = await _leaveAPIController.GetLeaveforReversal(0,Convert.ToInt32(employeeId), true);

                // History 
               // EmployeeLeaveDetail = await _leaveAPIController.GetEmployeeLeaveHistory(0, Convert.ToInt32(employeeId));

                // Apply
                EmployeeLeaveDetail.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);
                if(genderId==1)
                EmployeeLeaveDetail.LeaveAttributeKeyValues.LeaveTypeKeyValues = EmployeeLeaveDetail.LeaveAttributeKeyValues.LeaveTypeKeyValues.Where(x => x.LeaveTypeCode != "MTL").ToList();
                else
                EmployeeLeaveDetail.LeaveAttributeKeyValues.LeaveTypeKeyValues = EmployeeLeaveDetail.LeaveAttributeKeyValues.LeaveTypeKeyValues.Where(x =>x.LeaveTypeCode != "PTL" ).ToList();
                // view
                IActionResult actionResult = await _leaveAPIController.GetEmployeeLeaveSummary(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, Convert.ToInt32(employeeId), genderId);
                ObjectResult objResult = (ObjectResult)actionResult;               
                EmployeeLeaveDetail.EmployeeLeaveSummary = (List<EmployeeLeaveBalanceDTO>)objResult.Value;
                if(!string.IsNullOrEmpty(empSession.Doj.ToString()))
                {
                    DateTime date = (DateTime)empSession.Doj;
                    string formatted = date.ToString("dd-MMM-yyyy");
                    EmployeeLeaveDetail.DOJ = formatted;
                }
               
                EmployeeLeaveDetail.DOC = "";

                return View(EmployeeLeaveDetail);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public async Task<IActionResult> LeaveCompOff(LeaveCompOffDTO inputData)
        {

            LeaveCompOffDTO viewModel = new LeaveCompOffDTO();
            try
            {
                int clientId;

                if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
                {
                    return RedirectToAction("Login", "Account");
                }
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                if (unitId != null)
                {
                    inputData.IsActive = true;
                    inputData.UnitId = unitId;
                 
                    IActionResult actionResult;

                 //   if (inputData.CompOffId == 0)
                        //throw new Exception("Testing the error message");
                        actionResult = _leaveAPIController.SaveComboff(inputData);
                    //else
                      //  actionResult = _leaveAPIController.UpdateLeaveAttribute(inputData);

                    ObjectResult objResult = (ObjectResult)actionResult;

                    var objResultData = objResult.Value;
                    inputData.HttpStatusCode = objResult.StatusCode;
                    inputData.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);

                    if (inputData.HttpStatusCode == 200)
                    {
                        if (inputData.CompOffId == 0)
                            inputData.DisplayMessage = "Transaction Successful!";
                        else
                            inputData.DisplayMessage = "Transaction Successful!";
                        inputData.CompOffId = 0;
                        inputData.LeaveCompOffList = await GetLeaveCompOffList();
                        foreach (var item in inputData.LeaveCompOffList)
                        {
                            // item.EncryptedId = CommonHelper.EncryptURLHTML(item.SanctionId.ToString());
                           // item.CalendarName = inputData.LeaveAttributeKeyValues.LeaveCalenderYearKeyValue.Where(x => x.LeaveYearId == item.CalendarYear).Select(r => r.CalendarName).FirstOrDefault();
                        }

                    }
                    else
                        inputData.DisplayMessage = objResultData.ToString();
                    viewModel = inputData;
                    return View("CompOff", viewModel);
                }

                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", "Invalid Username or Password");
                return View("CompOff", viewModel);
            }



        }

        [HttpPost]
        public async Task<IActionResult> SaveEmployeeCompOff(EmployeeCompOffDTO inputData)
        {

            EmployeeCompOffDTO viewModel = new EmployeeCompOffDTO();
            try
            {
                string? employeeId = HttpContext.Session.GetString("EmployeeId");
                int clientId;

                if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
                {
                    return RedirectToAction("LoginUser", "Account");
                }
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                if (unitId != null)
                {
                    inputData.Status = 0;
                    inputData.UnitId = unitId;
                    inputData.EmployeeId = Convert.ToInt32(employeeId);
                    inputData.CreatedBy = Convert.ToInt32(employeeId);
                    inputData.CreatedOn = DateTime.Now;
                    if (inputData.CompOffId==0)
                    inputData.TicketId = CommonHelper.CreateTicket("CompOff", "");

                    IActionResult actionResult;

                    //   if (inputData.CompOffId == 0)
                    //throw new Exception("Testing the error message");
                    actionResult = _leaveAPIController.SaveEmployeeComboff(inputData);
                    //else
                    //    actionResult = _leaveAPIController.UpdateLeaveAttribute(inputData);

                    ObjectResult objResult = (ObjectResult)actionResult;

                    var objResultData = objResult.Value;
                    inputData.HttpStatusCode = objResult.StatusCode;
                    //  inputData.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, unitId);

                    if (inputData.HttpStatusCode == 200)
                    {
                        if (inputData.CompOffId == 0)
                            inputData.DisplayMessage = "Transaction Successful!";
                        else
                            inputData.DisplayMessage = "Transaction Successful!";

                        //viewModel.DisplayMessage = inputData.DisplayMessage;
                        inputData.EmployeeCompOffList = await GetEmployeeCompOffList();
                        viewModel = inputData;
                        return View("EmployeeCompOff", viewModel);
                    }

                    else
                    {
                        return RedirectToAction("LoginUser", "Account");
                    }
                }
                return RedirectToAction("LoginUser", "Account");
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", "Invalid Username or Password");
                return View("EmployeeCompOff", viewModel);
            }



        }


        public async Task<List<LeaveCompOffDTO>?> GetLeaveCompOffList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            IActionResult actionResult = await _leaveAPIController.GetComboffList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<LeaveCompOffDTO> objResultData = (List<LeaveCompOffDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.Encrypt(Convert.ToString(item.CompOffId));
            }
            return objResultData;
        }

        public async Task<List<EmployeeCompOffDTO>?> GetEmployeeCompOffList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            IActionResult actionResult = await _leaveAPIController.GetEmployeeComboffList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId,Convert.ToInt32(employeeId));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<EmployeeCompOffDTO> objResultData = (List<EmployeeCompOffDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.Encrypt(Convert.ToString(item.CompOffId));
            }
            return objResultData;
        }

        public async Task<List<EmployeeCompOffDTO>?> GetEmployeeCompOffPendingList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            IActionResult actionResult = await _leaveAPIController.GetEmployeeComboffPendingList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, unitId);
            ObjectResult objResult = (ObjectResult)actionResult;

            List<EmployeeCompOffDTO> objResultData = (List<EmployeeCompOffDTO>)objResult.Value;
            foreach (var item in objResultData)
            {
                item.EncryptedId = CommonHelper.Encrypt(Convert.ToString(item.CompOffId));
            }
            return objResultData;
        }

        [HttpGet]
        [Route("Leave/GetLeaveCompoffInfo/{leaveId}")]
        public async Task<IActionResult> GetLeaveCompoffInfo(string leaveId)
        {
            int clientId;
            int leaveCompOffId = 0;
            try
            {
                leaveCompOffId = Convert.ToInt32(CommonHelper.DecryptURLHTML(leaveId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (leaveCompOffId != 0)
            {
                LeaveCompOffDTO outputData = new LeaveCompOffDTO();
                outputData.CompOffId = leaveCompOffId;

                if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
                {
                    return RedirectToAction("Login", "Account");
                }


                if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                    outputData.UnitId = 0;
                else
                    outputData.UnitId = HttpContext.Session.GetInt32("UnitId");

                IActionResult actionResult;

                actionResult = await _leaveAPIController.GetComboff(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (LeaveCompOffDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    objResultData.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, outputData.UnitId);
                    objResultData.LeaveCompOffList = await GetLeaveCompOffList();
                    return View("CompOff", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.CompOffId = 0;
                    objResultData.LeaveCompOffList = await GetLeaveCompOffList();
                    objResultData.DisplayMessage = "You cannot edit locked leave. Contact Admin for further details";
                    return View("CompOff", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("CompOff", "Leave");
        }

        [HttpGet]
        [Route("Leave/DeleteLeaveCompoff/{LeaveCompId}")]
        public async Task<IActionResult> DeleteLeaveCompoff(string LeaveCompId)
        {
            int clientId; int CompOffId = 0;

            if (!int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId))
            {
                return RedirectToAction("Login", "Account");
            }
         
            try
            {
                CompOffId = Convert.ToInt32(CommonHelper.DecryptURLHTML(LeaveCompId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (CompOffId != 0)
            {

                LeaveCompOffDTO outputData = new LeaveCompOffDTO();
                outputData.CompOffId = CompOffId;
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("UnitId")))
                    outputData.UnitId = 0;
                else
                    outputData.UnitId = HttpContext.Session.GetInt32("UnitId");
                IActionResult actionResult;

                actionResult = await _leaveAPIController.DeleteCompOff(outputData);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = objResult.Value;

                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                outputData.CompOffId = 0;
                outputData.LeaveAttributeKeyValues = await _mastersKeyValueController.LeaveAttributeMastersKeyValue(true, clientId, outputData.UnitId);
                outputData.LeaveCompOffList = await GetLeaveCompOffList();
                outputData.DisplayMessage = "Transaction Successful!";
                return View("CompOff", outputData);
                //}
            }
            return RedirectToAction("CompOff", "Leave");
        }

        [HttpGet]
        public async Task<string> DeleteTicket(int Id,int moduleId)
        {
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            var status = await _leaveAPIController.LeaveCancelTicketRequest(Id, moduleId, employeeId);

            return status;


          //  return RedirectToAction("Ticket", "List");

        }

        [HttpPost]
        public async Task<IActionResult> TicketRegularizeProcessing(LeaveAction userAction)
        {
           // string folderpath = Path.Combine(this.Environment.WebRootPath, "EmployeePofile");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
           // string path = Path.Combine(folderpath, Convert.ToString(unitId));

            int clientId;
            //int unitId = (int)HttpContext.Session.GetInt32("UnitId");
            UnitMasterDTO? unit = JsonConvert.DeserializeObject<UnitMasterDTO?>(HttpContext.Session.GetString("unit"));
            userAction.DisplayName = unit.EmailDisplayName;

            // inputData.TicketId = CommonHelper.CreateTicket("Leave", "");
            string? employeeId = HttpContext.Session.GetString("EmployeeId");
            if (int.TryParse(HttpContext.Session.GetString("ClientId"), out clientId) && unitId != null)
            {
                ClientSettingDTO outputData = await _ClientController.GetClientSettingDetails(clientId);
                string folderpath = "ClientLogo/" + outputData.ClientId.ToString() + "/" + outputData.ClientLogo;
                userAction.Profile = folderpath;
                userAction.EmailProvider = outputData.EmailProvider;
            }
            else
            {
                userAction.Profile = "";
            }

          //  userAction.Profile = path;
           // string? employeeId = HttpContext.Session.GetString("EmployeeId");
            LeaveAction manualPunchVM = new LeaveAction();
            userAction.ApprovedBy = employeeId;
            if (userAction.LeaveIds != null && userAction.LeaveIds.Length != 0)
            {
                string sRetMsg = await _leaveAPIController.TicketRegularizeProcessing(userAction);
                manualPunchVM.DisplayMessage = sRetMsg;
            }
            else
                manualPunchVM.DisplayMessage = "Select leave " + (userAction.ActionType == "A" ? "for approval" : "to be reject");
            return Ok(manualPunchVM.DisplayMessage);
        }


        [HttpGet]
        public async Task<int> GetWorkingnHoliday(string sDate, string eDate,int isCondition)
        {
            int holidayCnt = 0,workingdayCnt=0; string weekend = "";
            if (isCondition == 3)
            {
                holidayCnt = await _leaveAPIController.GetUnitHolidayList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, Convert.ToDateTime(sDate), Convert.ToDateTime(eDate), HttpContext.Session.GetInt32("UnitId"));
               // weekend = _leaveAPIController.GetWeekendDay(HttpContext.Session.GetInt32("UnitId"));

                workingdayCnt = GetWorkingDays(Convert.ToDateTime(sDate), Convert.ToDateTime(eDate));
                workingdayCnt = workingdayCnt - holidayCnt;
            }
            else if (isCondition == 1)
            {
                holidayCnt = await _leaveAPIController.GetUnitHolidayList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, Convert.ToDateTime(sDate), Convert.ToDateTime(eDate), HttpContext.Session.GetInt32("UnitId"));
                // weekend = _leaveAPIController.GetWeekendDay(HttpContext.Session.GetInt32("UnitId"));
                workingdayCnt = holidayCnt;
            }
            else if (isCondition == 2)
            {
               // holidayCnt = await _leaveAPIController.GetUnitHolidayList(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, Convert.ToDateTime(sDate), Convert.ToDateTime(eDate), HttpContext.Session.GetInt32("UnitId"));
              //  weekend = _leaveAPIController.GetWeekendDay(HttpContext.Session.GetInt32("UnitId"));

                workingdayCnt = GetWorkingDays(Convert.ToDateTime(sDate), Convert.ToDateTime(eDate));
               // workingdayCnt = workingdayCnt + 1;
            }

            // ObjectResult objResultH = (ObjectResult)actionResultH;
            //var holidayCnt = objResultH.Value;
            // int days = GetWorkingDays(Convert.ToDateTime(sDate), Convert.ToDateTime(eDate));



            return (workingdayCnt);
        }

        [HttpGet]
        public async Task<int> GetWorkingDay(string sDate, string eDate)
        {

           string holidayCnt =  _leaveAPIController.GetWeekendDay(HttpContext.Session.GetInt32("UnitId"));
            // ObjectResult objResultH = (ObjectResult)actionResultH;
            //var holidayCnt = objResultH.Value;
            int days = GetWorkingDays(Convert.ToDateTime(sDate), Convert.ToDateTime(eDate));



            return days;
        }

        [HttpPost]
        public async Task<EmployeeLeaveDetailsDTO> GetDebitLeaveDetails(EmployeeLeaveBalanceInputs userAction)
        {
            EmployeeLeaveDetailsDTO EmployeeLeaveDetail = new EmployeeLeaveDetailsDTO();
            userAction.UnitId = HttpContext.Session.GetInt32("UnitId");
           EmployeeLeaveDetail = await _leaveAPIController.GetEmployeeDebitDetails(userAction);

            return EmployeeLeaveDetail;
        }

    }
}
