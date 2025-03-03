
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using SimpliHR.Core.Entities;
using SimpliHR.Endpoints.Attendance;
using SimpliHR.Endpoints.ClientManagement;
using SimpliHR.Endpoints.Leave;
using SimpliHR.Endpoints.Login;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Infrastructure.Models.Login;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Web;

namespace SimpliHR.WebUI.Controllers.ClientManagement
{
    public class ClientManagementController : Controller
    {

        private readonly MastersKeyValueController _masterKeyValueAPIController;
        private readonly ClientController _ClientController;
        private readonly LoginController _loginAPIController;
        private readonly EmployeeMasterController _employeeAPIController;

        private IWebHostEnvironment Environment;

        public ClientManagementController(MastersKeyValueController masterKeyValueAPIController, ClientController ClientManagementController, IWebHostEnvironment _environment, LoginController loginController, EmployeeMasterController employeeAPIController)
        {

            _masterKeyValueAPIController = masterKeyValueAPIController;
            _ClientController = ClientManagementController;
            Environment = _environment;
            _loginAPIController = loginController;
            _employeeAPIController = employeeAPIController;
            // _roleAPIController = roleAPIController;
        }
        public IActionResult Client()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            ClientDTO outputData = new ClientDTO();
            outputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            outputData.ModuleList = await _masterKeyValueAPIController.ModuleKeyValue(p => p.IsActive == true);
            outputData.IDTypeList = await _masterKeyValueAPIController.IdTypeKeyValue(true);



            var enumData = (from ColorThemes e in Enum.GetValues(typeof(ColorThemes))
                            select new
                            {
                                ID = (int)e,
                                Name = e.ToString()
                            }).ToList();
            ViewBag.ColorList = new SelectList(enumData, "ID", "Name");
            if (outputData != null)
            {
                return View(outputData);
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClientUnit()
        {

            UnitMasterDTO outputData = new UnitMasterDTO();

            outputData.lstUnitMaster = await GetClientUnits();

            foreach (var item in outputData.lstUnitMaster)
            {
                item.EunitID = CommonHelper.EncryptURLHTML(item.UnitID.ToString());
            }

            outputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            outputData.ClientList = await _masterKeyValueAPIController.ClientKeyValue(null);


            if (outputData != null)
            {
                //  ViewBag.Div = "List";
                return View(outputData);
            }
            else
            {
                return View();
            }


        }

        public async Task<ActionResult> SetUnit(UnitMasterDTO unitMasterDTO)
        {
            try
            {
                //  HttpContext.Session.SetInt32("RoleId", (int)employeeMasterDTO.RoleId);
                HttpContext.Session.SetInt32("UnitId", unitMasterDTO.UnitID);

                EmployeeMasterDTO? employeeMasterDTO = JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee"));

                employeeMasterDTO.UnitId = unitMasterDTO.UnitID;

                string employee = JsonConvert.SerializeObject(employeeMasterDTO);

                HttpContext.Session.SetString("employee", employee);

                UnitMasterDTO resUnit = await _ClientController.GetClientUnitById(unitMasterDTO.UnitID);
                if (resUnit != null)
                {
                    string unit = JsonConvert.SerializeObject(resUnit);
                    HttpContext.Session.SetString("unit", unit);
                }

                //RoleMasterDTO outputData = new RoleMasterDTO();
                //outputData.RoleId = (int)HttpContext.Session.GetInt32("RoleId");

                //IActionResult actionResult;

                //actionResult = await _roleAPIController.GetRole(outputData);
                //ObjectResult objResult = (ObjectResult)actionResult;
                //var objResultData = (RoleMasterDTO)objResult.Value;
                //var roleType = objResultData.RoleType;
                //if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                //{
                //    return View("Role", objResultData);
                //    //return RedirectToAction("Role","Role", objResultData);
                //}

                return Ok(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        public async Task<IActionResult> ClientUnitsHome()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("employee")))
            {
                return RedirectToAction("Login", "Account");
            }
            EmployeeMasterDTO employeeMasterDTO = JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee"));

            UnitMasterDTO outputData = new UnitMasterDTO();
            string emailId = employeeMasterDTO.EmailId;
            IActionResult actionResult = _ClientController.GetCllientUnits(emailId);
            ObjectResult objResult = (ObjectResult)actionResult;
            List<UnitMasterDTO> objResultData = (List<UnitMasterDTO>)objResult.Value;
            outputData.UnitMasterList = objResultData;
            //   outputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            // outputData.ClientList = await _masterKeyValueAPIController.ClientKeyValue(null);


            if (outputData != null)
            {
                //  ViewBag.Div = "List";
                return View(outputData);
            }
            else
            {
                return View();
            }


        }

        public async Task<IActionResult> UnitDashboard()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("employee")))
            {
                return RedirectToAction("Login", "Account");
            }
            EmployeeMasterDTO employeeMasterDTO = JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee"));

            UnitMasterDTO outputData = new UnitMasterDTO();
            string emailId = employeeMasterDTO.EmailId;
            int? clientId = employeeMasterDTO.ClientId;
            IActionResult actionResult = _ClientController.GetCllientUnits(emailId);
            ObjectResult objResult = (ObjectResult)actionResult;
            List<UnitMasterDTO> objResultData = (List<UnitMasterDTO>)objResult.Value;
            outputData.UnitMasterList = objResultData;
            //   outputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            // outputData.ClientList = await _masterKeyValueAPIController.ClientKeyValue(null);
            ClientSettingDTO outData = new ClientSettingDTO();
            //outputData.ClientId = Convert.ToInt32(clientId);
            outData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(clientId));
            outputData.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(outData.ProfileImage, 0, outData.ProfileImage.Length);


            if (outputData != null)
            {
                //  ViewBag.Div = "List";
                return View(outputData);
            }
            else
            {
                return View();
            }


        }
        public async Task<List<UnitMasterDTO>?> GetClientUnits()
        {

            IActionResult actionResult = await _ClientController.GetClientUnits(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<UnitMasterDTO> objResultData = (List<UnitMasterDTO>)objResult.Value;

            //foreach (var item in objResultData)
            //{
            //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.WorkFlowSettingsId.ToString());
            //}
            return objResultData;
        }

        //[HttpPost]
        //public async Task<IActionResult> Add(ClientDTO inputData)
        //{
        //    return View();
        //}

        [HttpGet]
        public async Task<List<StateKeyValues>>? GetCounryStates(int countryId)
        {
            return await _masterKeyValueAPIController.StateKeyValue(true, countryId);
        }

        [HttpGet]
        public async Task<List<CityKeyValues>>? GetStateCities(int stateId)
        {
            return await _masterKeyValueAPIController.CityKeyValue(true, stateId);
        }


        public ActionResult Dashboard()
        {
            ClientDTO outputData = new ClientDTO();
            outputData.lstClient = GetClientsList();

            foreach (var item in outputData.lstClient)
            {
                item.EncClientId = CommonHelper.EncryptURLHTML(item.ClientId.ToString());
            }

            if (outputData != null)
            {
                return View("Dashboard", outputData);
            }
            else
            {
                return View("Dashboard");
            }
        }


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Overview()
        {
            ClientDTO outputData = new ClientDTO();
            outputData.lstClient = GetClientsList();
            outputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            if (outputData != null)
            {
                return View("Overview", outputData);
            }
            else
            {
                return View("Overview");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SettingView()
        {
            ClientSettingDTO outputData = new ClientSettingDTO();
            outputData.lstClientSetting = GetClientSetting(0);

            foreach (var item in outputData.lstClientSetting)
            {
                item.EncryptedClientSettingId = CommonHelper.EncryptURLHTML(item.ClientSettingId.ToString());
            }

            outputData.ModuleList = await _masterKeyValueAPIController.ModuleKeyValue(p => p.IsActive == true);
            outputData.IDTypeList = await _masterKeyValueAPIController.IdTypeKeyValue(true);
            outputData.ClientList = await _masterKeyValueAPIController.ClientKeyValue(null);


            var enumData = (from ColorThemes e in Enum.GetValues(typeof(ColorThemes))
                            select new
                            {
                                ID = (int)e,
                                Name = e.ToString()
                            }).ToList();
            ViewBag.ColorList = new SelectList(enumData, "ID", "Name");

            if (outputData != null)
            {
                ViewBag.ClientSettingID = 0;
                return View("SettingView", outputData);
            }
            else
            {
                return View("SettingView");
            }
        }


        [Authorize(Roles = "Admin")]
        public List<ClientDTO> GetClientsList()
        {
            // List<ClientDTO> outputModel = new List<ClientDTO>();
            ClientDTO outputModel = new ClientDTO();
            outputModel.lstClient = _ClientController.GetAllClient(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            return outputModel.lstClient;
            //return null;
        }

        public async Task<List<ClientDTO>> GetClientList()
        {

            IActionResult actionResult = await _ClientController.GetClients(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 });
            ObjectResult objResult = (ObjectResult)actionResult;

            List<ClientDTO> objResultData = (List<ClientDTO>)objResult.Value;
            return objResultData;
        }
        public List<ClientSettingDTO> GetClientSetting(int ID)
        {
            // List<ClientDTO> outputModel = new List<ClientDTO>();
            ClientSettingDTO outputModel = new ClientSettingDTO();
            outputModel.lstClientSetting = _ClientController.GetClientsSetting(ID);

            foreach (var item in outputModel.lstClientSetting)
            {
                item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
            }

            return outputModel.lstClientSetting;


            //return null;
        }

        [HttpGet]
        [Route("ClientManagement/GetClientSettingById/{eSettingId}")]
        public async Task<IActionResult> GetClientSettingById(string eSettingId)
        {
            int SettingId = 0;
            try
            {
                SettingId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eSettingId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (SettingId != 0)
            {
                // ClientSettingDTO outputModel = new ClientSettingDTO();
                //  outputModel.lstClientSetting = _ClientController.GetClientsSetting(ID);

                IActionResult actionResult;

                actionResult = await _ClientController.GetClientsSettingID(SettingId);
                ObjectResult objResult = (ObjectResult)actionResult;

                var objResultData = (ClientSettingDTO)objResult.Value;

                objResultData.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(objResultData.ProfileImage, 0, objResultData.ProfileImage.Length);

                var stream = new MemoryStream(objResultData.ProfileImage);
                IFormFile file = new FormFile(stream, 0, objResultData.ProfileImage.Length, "name", objResultData.ClientLogo);
                objResultData.ProfileImageFile = file;
                //   ObjectResult objResult = (ObjectResult)actionResult;


                objResultData.ModuleList = await _masterKeyValueAPIController.ModuleKeyValue(p => p.IsActive == true);
                objResultData.IDTypeList = await _masterKeyValueAPIController.IdTypeKeyValue(true);
                objResultData.ClientList = await _masterKeyValueAPIController.ClientKeyValue(null);


                var enumData = (from ColorThemes e in Enum.GetValues(typeof(ColorThemes))
                                select new
                                {
                                    ID = (int)e,
                                    Name = e.ToString()
                                }).ToList();
                ViewBag.ColorList = new SelectList(enumData, "ID", "Name");

                if (objResultData != null)
                {
                    ViewBag.Div = "Add";
                    // ViewBag.ClientSettingID = 0;
                    return View("SettingView", objResultData);
                }
                else
                {
                    return RedirectToAction("SettingView", "ClientManagement");
                }
            }
            else
            {
                return RedirectToAction("SettingView", "ClientManagement");
            }

        }

        //[HttpGet]
        //[Route("ClientManagement/GetClientById/{ID:int}")]
        //public async Task<IActionResult> GetClientById(int ID)
        //{

        //    ClientDTO outputData = new ClientDTO();
        //    outputData.lstClient = _ClientController.GetClientByID(ID);
        //    outputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);




        //    if (outputData != null)
        //    {
        //        ViewBag.Div = "Add";
        //        // ViewBag.ClientSettingID = 0;
        //        return View("Dashboard", outputData);
        //    }
        //    else
        //    {
        //        ViewBag.Div = "List";
        //        return RedirectToAction("Dashboard", "ClientManagement");
        //    }

        //}

        [HttpGet]
        [Route("ClientManagement/GetClientById/{eClientID}")]
        public async Task<IActionResult> GetClientById(string eClientID)
        {
            int clientID = 0;
            try
            {
                clientID = Convert.ToInt32(CommonHelper.DecryptURLHTML(eClientID));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
            if (clientID != 0)
            {
                ClientDTO outputData = new ClientDTO();
                outputData.ClientId = clientID;

                IActionResult actionResult;

                actionResult = await _ClientController.GetClientByID(clientID);
                ObjectResult objResult = (ObjectResult)actionResult;
                var objResultData = (ClientDTO)objResult.Value;
                if (objResultData.HttpMessage.StatusCode == HttpStatusCode.OK)
                {
                    //objResultData.WorkLocationMasterList = await GetWorkLocationList();
                    objResultData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
                    return View("Dashboard", objResultData);
                    //return RedirectToAction("Role","Role", objResultData);
                }
                else if (objResultData.HttpMessage.StatusCode == HttpStatusCode.Locked)
                {
                    objResultData.ClientId = 0;
                    objResultData.lstClient = await GetClientList();
                    //  objResultData.DisplayMessage = "You cannot edit locked work location. Contact Admin for further details";
                    return View("Dashboard", objResultData);
                    //return RedirectToAction("Role", objResultData);
                }
            }
            return RedirectToAction("Dashboard", "ClientManagement");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<object> SaveCllientInfo(ClientDTO inputData)
        {

            //if (!ModelState.IsValid)
            //{
            //   // ViewBag.Div = "Add";
            //    return View("Add", inputData);
            //}
            inputData.IsActive = true;
            IActionResult actionResult;
            dynamic objResultData = null;
            ClientDTO viewModel = new ClientDTO();
            dynamic results = 0;
            // var resltlist;
            string sValidMessages = _ClientController.ValidateClientInfo(inputData);
            if (sValidMessages.IsNullOrEmpty())
            {
                if (inputData.ClientId.ToString().Equals("0"))
                {
                    actionResult = _ClientController.SaveClientDetails(inputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    results = objResult.Value.ToString();
                    if (results.ToUpper() == "SUCCESS")
                    {
                        results = _ClientController.GetClientID();
                        //  string CID = results;
                        // HttpContext.Session.SetString("CID", CID);
                    }

                }
                else
                {

                    actionResult = _ClientController.UpdateClient(inputData);
                    ObjectResult objResult = (ObjectResult)actionResult;
                    results = objResult.Value.ToString();

                    return RedirectToAction("Dashboard", "ClientManagement");
                }
            }
            else
            {
                results = sValidMessages;

            }

            if (inputData.ClientId.ToString().Equals("0"))
                return results;
            else
            {
                inputData.DisplayMessage = results;
                return View("Dashboard", inputData);
            }

        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<object> SaveClientSettingInfo(ClientSettingDTO inputData)
        //public async Task<object> SaveClientSettingInfo(List<IFormFile> formFile, ClientSettingDTO inputData)
        {
            BL.ClientMail blemp = new BL.ClientMail();
            //inputData.IsActive = true;
            IActionResult actionResult;
            ObjectResult objResult = null;
            //if (!ModelState.IsValid)
            //{
            //    // ViewBag.Div = "Add";
            //    return View("Add", inputData);
            //}
            //   ClientSettingDTO inputData = new ClientSettingDTO();
            dynamic results = 0;
            var profileImage = inputData.ProfileImageFile;

            if (profileImage != null)
            {
                if (profileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(profileImage.FileName);
                    inputData.ClientLogo = fileName;
                    // //var fileExtension = Path.GetExtension(fileName);

                    string FilePath = Path.Combine(Environment.WebRootPath, "ClientLogo", inputData.ClientId.ToString());
                    if (!Directory.Exists(FilePath))
                        Directory.CreateDirectory(FilePath);

                    var fPath = Path.Combine(FilePath, fileName);



                    using (var target = new MemoryStream())
                    {
                        profileImage.CopyTo(target);
                        inputData.ProfileImage = target.ToArray();
                       System.IO.File.WriteAllBytes(fPath, target.ToArray());
                    }
                }
                else
                    inputData.ProfileImage = null;
            }
            else
                inputData.ProfileImage = null;

            // string sValidMessages = _ClientController.ValidateClientSettingInfo(inputData);
            //  if (sValidMessages.IsNullOrEmpty())
            //  {
            if (inputData.ClientSettingId.ToString().Equals("0"))
            {
                string sValidMessages = _ClientController.ValidateClientSettingInfo(inputData);
                if (sValidMessages.IsNullOrEmpty())
                {
                    actionResult = _ClientController.SaveClientSettingDetails(inputData);
                    objResult = (ObjectResult)actionResult;
                }

                else
                {
                    results = sValidMessages;

                }
            }
            else
            {

                actionResult = _ClientController.UpdateClientSetting(inputData);
                objResult = (ObjectResult)actionResult;

            }



            results = objResult.StatusCode;

            //  }
            //   else
            //  {
            //   results = sValidMessages;

            //  }

            if (inputData.ClientSettingId.ToString().Equals("0"))
                return results;
            else
            {
                inputData.DisplayMessage = results;
                inputData.ClientSettingId = 0;
                return View("SettingView", inputData);
            }



        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        // public async Task<object> SaveClientConfigurationInfo(ClientSettingDTO inputData)
        public async Task<object> SaveClientConfigurationInfo(List<IFormFile> formFile, ClientSettingDTO inputData)
        {
            BL.ClientMail blemp = new BL.ClientMail();
            //inputData.IsActive = true;
            IActionResult actionResult;
            ObjectResult objResult = null;
           /// inputData.ClientDomain = inputData.ClientDomain;
            //if (!ModelState.IsValid)
            //{
            //    // ViewBag.Div = "Add";
            //    return View("Add", inputData);
            //}
            //   ClientSettingDTO inputData = new ClientSettingDTO();
            dynamic results = 0;
            if (formFile.Count > 0)
            {
                //string wwwPath = this.Environment.WebRootPath;
                //string contentPath = this.Environment.ContentRootPath;

                //string FilePath = Path.Combine(Environment.WebRootPath, "ClientLogo", inputData.ClientId.ToString());
                //if (!Directory.Exists(FilePath))
                //    Directory.CreateDirectory(FilePath);

                //var fPath = Path.Combine(FilePath, fileName);


                string path = Path.Combine(this.Environment.WebRootPath, "ClientLogo");

                List<string> uploadedFiles = new List<string>();
                foreach (IFormFile postedFile in formFile)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    fileName = inputData.ClientId + '_' + fileName;

                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                        uploadedFiles.Add(fileName);
                        // ViewBag.Message += fileName + ",";
                    }
                    inputData.ClientLogo = fileName;
                }
            }

            var profileImage = inputData.ProfileImageFile;
            if (profileImage != null)
            {
                if (profileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(profileImage.FileName);
                    inputData.ClientLogo = fileName;
                    var fileExtension = Path.GetExtension(fileName);

                    string FilePath = Path.Combine(Environment.WebRootPath, "ClientLogo", inputData.ClientId.ToString());
                    if (!Directory.Exists(FilePath))
                        Directory.CreateDirectory(FilePath);

                    var fPath = Path.Combine(FilePath, fileName);

                    using (var target = new MemoryStream())
                    {
                        profileImage.CopyTo(target);
                        inputData.ProfileImage = target.ToArray();
                        System.IO.File.WriteAllBytes(fPath, target.ToArray());
                    }
                }
                else
                {
                    IActionResult actionResultClient = await _ClientController.GetClientsSettingIDAsNoTracking(inputData.ClientId ?? default(int));
                    ObjectResult objResultClient = (ObjectResult)actionResultClient;
                    var objResultClientData = (ClientSettingDTO)objResultClient.Value;

                    inputData.ProfileImage = objResultClientData.ProfileImage;
                }
            }

            if (inputData.ClientSettingId.ToString().Equals("0"))
            {
                string sValidMessages = _ClientController.ValidateClientSettingInfo(inputData);
                if (sValidMessages.IsNullOrEmpty())
                {
                    actionResult = _ClientController.SaveClientSettingDetails(inputData);
                    objResult = (ObjectResult)actionResult;
                }

                else
                {
                    results = sValidMessages;

                }
            }
            else
            {
                if (inputData.ProfileImage == null)
                {
                    IActionResult actionResultClient = await _ClientController.GetClientsSettingIDAsNoTracking(inputData.ClientSettingId);
                    ObjectResult objResultClient = (ObjectResult)actionResultClient;
                    var objResultClientData = (ClientSettingDTO)objResultClient.Value;
                    inputData.ProfileImage = objResultClientData.ProfileImage;
                }
                actionResult = _ClientController.UpdateClientSetting(inputData);
                objResult = (ObjectResult)actionResult;
            }



            results = objResult.StatusCode;

            //  }
            //   else
            //  {
            //   results = sValidMessages;

            //  }

            //if (inputData.ClientSettingId.ToString().Equals("0"))
            //    return RedirectToAction("SettingView", "ClientManagement");
            //else
            //{
            return RedirectToAction("SettingView", "ClientManagement");
            //inputData.DisplayMessage = results;
            //    inputData.ClientSettingId = 0;
            //    return View("SettingView", inputData);
            // }



        }


        public async Task<Object> ResendUnitCreationEmail([FromBody] UnitMasterDTO inputData)
        {
            try
            {
                if (inputData != null)
                {
                    if (inputData.EunitID != null)
                    {
                        ClientSettingDTO outputData = new ClientSettingDTO();
                        inputData.UnitID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputData.EunitID));
                        UnitMasterDTO unitMasterDTO = await _ClientController.GetClientUnitById(inputData.UnitID);
                        LoginDetailDTO loginDetailDTO = await _ClientController.GetClientLoginDetailsByUnitId(inputData.UnitID);
                        outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(unitMasterDTO.ClientId));
                      //  BL.ClientMail blemp = new BL.ClientMail();
                        //blemp.SendJoiningLink(unitMasterDTO, loginDetailDTO);
                        bool mailSent = MailHelper.SendJoiningLink(inputData, loginDetailDTO, outputData.EmailProvider);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<object> SaveClientUnitInfo(UnitMasterDTO inputData)
        {

            // return View();

            BL.ClientMail blemp = new BL.ClientMail();
            //  dynamic results = 0;
            //return null;

            //if (!ModelState.IsValid)
            //{
            //   // ViewBag.Div = "Add";
            //    return View("Add", inputData);
            //}
            inputData.IsActive = true;
            IActionResult actionResult = null;
            dynamic objResultData = null;
            //   ClientDTO viewModel = new ClientDTO();
            dynamic results = 0, UnitId = 0;
            int EmployeeID = 0;
            // var resltlist;


            //string sValidMessages = _ClientController.ValidateClientUnitInfo(inputData);
            //if (sValidMessages.IsNullOrEmpty())
            //{
            //if(inputData.ClientId==0)
            //    inputData.ClientId= Convert.ToInt32(HttpContext.Session["CID"]);


            if (inputData.UnitID.ToString().Equals("0"))
            {
                string sValidMessages = _ClientController.ValidateClientUnitInfo(inputData);
                if (sValidMessages.IsNullOrEmpty())
                {
                    if (inputData.ClientId <= 0)
                    {
                        results = _ClientController.GetClientID();
                        inputData.ClientId = results;
                    }

                    inputData.IsBlock = 0;
                    inputData.CreatedBy = "Admin";
                    inputData.CreatedOn = DateTime.Now;
                    actionResult = _ClientController.SaveClientUnitDetails(inputData);
                }
                else
                {
                    results = sValidMessages;
                    inputData.DisplayMessage = results;
                }
                //   results = objResult.Value.ToString();
                // if (results.ToUpper() == "SUCCESS")
                //  results = _ClientController.GetClientID();
            }
            else
            {
                inputData.ModifiedBy = "Admin";
                inputData.ModifiedOn = DateTime.Now;
                actionResult = _ClientController.UpdateClientUnit(inputData);
                //   ObjectResult objResult = (ObjectResult)actionResult;
                //  results = objResult.Value.ToString();

                return RedirectToAction("ClientUnit", "ClientManagement");
            }
            if (actionResult == null)
            {
                inputData.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
                inputData.ClientList = await _masterKeyValueAPIController.ClientKeyValue(null);
                return View("ClientUnit", inputData);
            }
            ObjectResult objResult = (ObjectResult)actionResult;
            UnitId = objResult.Value;
            #region Hide Code to move in Unit Master section

            if (objResult.StatusCode == 200 && inputData.UnitID.ToString().Equals("0"))
            {

                IActionResult actionEmpIDResult;
                actionEmpIDResult = _ClientController.GeyEmployeeID(inputData.EmailId);

                ObjectResult objEmployeeIdResult = (ObjectResult)actionEmpIDResult;
                dynamic objEmpIdReturn = objEmployeeIdResult.Value;
                if (CommonHelper.IsNumeric(objEmpIdReturn))
                    EmployeeID = Convert.ToInt32(objEmployeeIdResult.Value);
                else
                {
                    EmployeeID = 0;
                }

                //  ClientDTO resltlist = await _ClientController.GetClientDetails(Convert.ToInt32(inputData.ClientId));
                if (EmployeeID == 0)
                {
                    EmployeeMasterDTO inputEmployeeData = new EmployeeMasterDTO();
                    inputEmployeeData.ClientId = inputData.ClientId;
                    inputEmployeeData.JoinType = 1;
                    inputEmployeeData.FirstName = inputData.ContactPerson;
                    inputEmployeeData.OfficialEmail = inputData.EmailId;
                    inputEmployeeData.EmailId = inputData.EmailId;
                    inputEmployeeData.ContactNo = Convert.ToString(inputData.ContactNumber);
                    inputEmployeeData.EmployeeStatus = "Active";
                    inputEmployeeData.IsActive = true;

                    EmployeeMasterDTO defaultmasters = await _ClientController.CreateDefaultDepartmentJobTitleRole(Convert.ToInt32(UnitId));

                    inputEmployeeData.JobTitleId = defaultmasters.JobTitleId;
                    inputEmployeeData.DepartmentId = defaultmasters.DepartmentId;
                    inputEmployeeData.RoleId = defaultmasters.RoleId;

                    //inputEmployeeData.JobTitleId = 5;
                    //inputEmployeeData.DepartmentId = 2;
                    //inputEmployeeData.RoleId = 2;
                    // inputEmployeeData.ManagerId = 22;
                    //_ClientController.CreateDefaultDepartmentJobTitleRole(Convert.ToInt32(UnitId));
                    inputEmployeeData.BandId = 3;
                    inputEmployeeData.GenderId = 1;
                    inputEmployeeData.UnitId = Convert.ToInt32(UnitId);
                    inputEmployeeData.Pannumber = "NA";
                    inputEmployeeData.AadharNumber = "NA";
                    inputEmployeeData.InfoFillingStatus = 1;
                    string sValidEmpMessages = _employeeAPIController.ValidateEmployeeInfo(inputEmployeeData);
                    if (sValidEmpMessages.IsNullOrEmpty())
                    {
                        IActionResult employeectionResult = _employeeAPIController.SaveEmployee(inputEmployeeData);
                        ObjectResult objEmployeeResult = (ObjectResult)employeectionResult;
                        dynamic objReturn = objEmployeeResult.Value;
                        if (CommonHelper.IsNumeric(objReturn))
                        {
                            ClientSettingDTO outputData = new ClientSettingDTO();
                            EmployeeID = Convert.ToInt32(objEmployeeResult.Value);
                            /// saved in Employee Unit Mapping

                            EmployeeUnitsMappingDTO inputEmployeeUnitData = new EmployeeUnitsMappingDTO();
                            inputEmployeeUnitData.ClientID = inputData.ClientId;
                            inputEmployeeUnitData.UnitID = Convert.ToInt32(UnitId);
                            inputEmployeeUnitData.EmployeeID = EmployeeID;
                            IActionResult employeeUnitectionResult = _ClientController.SaveEmployeeUnitMapping(inputEmployeeUnitData);
                            ObjectResult objUnitEmployeeResult = (ObjectResult)employeeUnitectionResult;
                            /// End

                            if (objUnitEmployeeResult.StatusCode == 200)
                            {
                                LoginDetailDTO loginDetailDTO = new LoginDetailDTO();
                                loginDetailDTO.UserName = inputData.EmailId;
                                loginDetailDTO.MobileNo = Convert.ToString(inputData.ContactNumber);
                                loginDetailDTO.Password = CommonHelper.Encrypt(blemp.RandomString());
                                loginDetailDTO.EncryptedPassword = CommonHelper.Encrypt(loginDetailDTO.Password);
                                loginDetailDTO.LoginType = 1;
                                loginDetailDTO.ClientId = inputData.ClientId;
                                loginDetailDTO.EmployeeId = Convert.ToInt32(objEmployeeResult.Value);

                                IActionResult loginactionResult =  await _loginAPIController.SaveLoginDetail(loginDetailDTO);
                                ObjectResult objLoginResult = (ObjectResult)loginactionResult;
                                if (objLoginResult.StatusCode == 200)
                                {
                                    outputData = await _ClientController.GetClientSettingDetails(Convert.ToInt32(inputData.ClientId));
                                    bool mailSent = MailHelper.SendJoiningLink(inputData, loginDetailDTO, outputData.EmailProvider);
                                    if (mailSent)
                                    {
                                        return RedirectToAction("ClientUnit", "ClientManagement");
                                        //  return View("Dashboard");
                                    }
                                    else
                                    {
                                        return View();
                                    }
                                }
                            }

                        }

                    }

                }
                else
                {
                    // saved in Employee Unit Mapping
                    if (CommonHelper.IsNumeric(UnitId))
                    {
                        EmployeeUnitsMappingDTO inputEmployeeUnitData = new EmployeeUnitsMappingDTO();
                        inputEmployeeUnitData.ClientID = inputData.ClientId;
                        inputEmployeeUnitData.UnitID = Convert.ToInt32(UnitId);
                        inputEmployeeUnitData.EmployeeID = EmployeeID;
                        IActionResult employeeUnitectionResult = _ClientController.SaveEmployeeUnitMapping(inputEmployeeUnitData);
                        ObjectResult objUnitEmployeeResult = (ObjectResult)employeeUnitectionResult;
                        // if (objUnitEmployeeResult.StatusCode == 200)
                        //  {
                        return RedirectToAction("ClientUnit", "ClientManagement");
                        //}
                    }
                }
                // End 

                //  }
                return View();
            }
            #endregion

            inputData.DisplayMessage = results;

            if (inputData.UnitID.ToString().Equals("0"))
                // return results;
                //  return View("Add", results);
                return View("ClientUnit", inputData);
            else
            {
                return View("ClientUnit", inputData);
            }

        }





        [HttpGet]
        [Route("ClientManagement/GetClientUnitById/{eUnitId}")]
        public async Task<IActionResult> GetClientUnitById(string eUnitId)
        {
            int unitID = 0;
            try
            {
                unitID = Convert.ToInt32(CommonHelper.DecryptURLHTML(eUnitId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            UnitMasterDTO outputModel = new UnitMasterDTO();
            outputModel = await _ClientController.GetClientUnitById(unitID);
            outputModel.CountryList = await _masterKeyValueAPIController.CountryKeyValue(true);
            outputModel.ClientList = await _masterKeyValueAPIController.ClientKeyValue(null);



            //var enumData = (from ColorThemes e in Enum.GetValues(typeof(ColorThemes))
            //                select new
            //                {
            //                    ID = (int)e,
            //                    Name = e.ToString()
            //                }).ToList();
            //ViewBag.ColorList = new SelectList(enumData, "ID", "Name");

            if (outputModel != null)
            {
                ViewBag.Div = "Add";
                // ViewBag.ClientSettingID = 0;
                return View("ClientUnit", outputModel);
            }
            else
            {
                return RedirectToAction("ClientUnit", "ClientManagement");
            }

        }


        [HttpGet]
        public dynamic GetUnitEmail(int clientId)
        {

            var unitDetails = _ClientController.CheckClientEmailExistorNot(clientId);

            return unitDetails;
        }


        public async Task<Object> BlockClientUnits([FromBody] UnitMasterDTO inputData)
        {
            try
            {
                if (inputData != null)
                {
                    if (inputData.EunitID != null)
                    {
                        IActionResult actionResult = null;
                        inputData.UnitID = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputData.EunitID));
                        //UnitMasterDTO unitMasterDTO = await _ClientController.GetClientUnitById(inputData.UnitID);
                        //unitMasterDTO.IsBlock = 1;

                        //unitMasterDTO.ModifiedBy = "Admin";
                        //unitMasterDTO.ModifiedOn = DateTime.Now;
                        actionResult = _ClientController.BlockUnits(inputData.UnitID, (int)inputData.IsBlock);

                        return RedirectToAction("ClientUnit", "ClientManagement");
                        //LoginDetailDTO loginDetailDTO = await _ClientController.GetClientLoginDetailsByUnitId(inputData.UnitID);
                        //BL.ClientMail blemp = new BL.ClientMail();
                        //blemp.SendJoiningLink(unitMasterDTO, loginDetailDTO);
                    }
                }
                return RedirectToAction("ClientUnit", "ClientManagement");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
