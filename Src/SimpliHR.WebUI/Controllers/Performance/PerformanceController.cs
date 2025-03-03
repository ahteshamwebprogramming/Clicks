
using Humanizer;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Performance;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.Infrastructure.Models.Performace;
using SimpliHR.Services.DBContext;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SimpliHR.WebUI.Controllers.Performance;

public class PerformanceController : Controller
{
    private readonly MastersKeyValueController _mastersKeyValueController;
    private readonly PerformanceSettingAPIController _performanceSettingAPIController;
    public PerformanceController(MastersKeyValueController mastersKeyValueController, PerformanceSettingAPIController performanceSettingAPIController)
    {
        _mastersKeyValueController = mastersKeyValueController;
        _performanceSettingAPIController = performanceSettingAPIController;
    }

    public async Task<IActionResult> PerformanceSettings(string? ePerformanceSettingId = null)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        int? unitId = empSession.UnitId;
        PerformanceSettingViewModel dto = new PerformanceSettingViewModel();
        dto.Bands = await _mastersKeyValueController.BandKeyValue(x => x.IsActive == true && x.UnitId == unitId);

        if (ePerformanceSettingId != null)
        {
            int PerformanceSettingId = Convert.ToInt32(CommonHelper.Decrypt(ePerformanceSettingId));
            var res = await _performanceSettingAPIController.GetPMSById(PerformanceSettingId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.PerformanceSetting = (PerformanceSettingDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
            List<PerformanceSettingSkillSetMatrixDTO> skill = new List<PerformanceSettingSkillSetMatrixDTO>();
            var resSkill = await _performanceSettingAPIController.PerformanceSettingSkillSetMatrixList(PerformanceSettingId);
            if (resSkill != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    skill = (List<PerformanceSettingSkillSetMatrixDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resSkill).Value;
                }
            }
            dto.PerformanceSettingSkillSetMatrixList = skill;

            var resAverageMethod = await _performanceSettingAPIController.PerformanceSettingAverageMethod(PerformanceSettingId);
            if (resAverageMethod != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.PerformanceSettingMechanismList = (List<PerformanceSettingMechanismDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resAverageMethod).Value;
                }
            }
        }

        return View(dto);
    }
    [HttpPost]
    public async Task<IActionResult> SavePMSSetting([FromBody] PerformanceSettingViewModel inputDTO)
    {
        try
        {
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int? unitId = empSession.UnitId;

            if (inputDTO == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Invalid data");
            }

            inputDTO.PerformanceSetting.UnitId = unitId ?? default(int);
            if (unitId == null)
            {
                return StatusCode(StatusCodes.Status203NonAuthoritative, "Session has expired");
            }

            if (inputDTO.PerformanceSetting.PerformanceSettingId == 0)
            {
                inputDTO.PerformanceSetting.CreatedDate = DateTime.Now;
                inputDTO.PerformanceSetting.IsActive = true;
                inputDTO.PerformanceSetting.CreatedBy = empSession.EmployeeCode;
            }
            else
            {
                inputDTO.PerformanceSetting.ModifiedDate = DateTime.Now;
                inputDTO.PerformanceSetting.ModifiedBy = empSession.EmployeeCode;
            }
            var res = await _performanceSettingAPIController.SavePMSSetting(inputDTO);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    return Ok();
                }
                else
                {
                    throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString());
                }
            }
            else
            {
                throw new Exception("Error occurred while creating user");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<IActionResult> ListPMSSetting()
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        int? UnitId = empSession.UnitId;
        PerformanceSettingViewModel dto = new PerformanceSettingViewModel();
        List<PerformanceSettingWithChildEntity> Listps = new List<PerformanceSettingWithChildEntity>();
        var res = await _performanceSettingAPIController.PMSList(UnitId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                Listps = (List<PerformanceSettingWithChildEntity>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        foreach (var item in Listps)
        {
            item.ePerformanceSettingId = CommonHelper.EncryptURLHTML(item.PerformanceSettingId.ToString());
        }
        dto.PerformanceSettingWithChildEntityList = Listps;
        return View(dto);
    }
    public IActionResult PMSMastersPage(string ePerformanceSettingId)
    {
        //if (ePerformanceSettingId != null)
        //{
        //    int PerformanceSettingId = Convert.ToInt32(CommonHelper.Decrypt(ePerformanceSettingId));
        ViewBag.PerformanceSettingId = ePerformanceSettingId;
        return View();
        //}        
    }
    public async Task<IActionResult> PerformanceReview()
    {
        bool fillPMS = false;
        List<MISViewList> mvManager = new List<MISViewList>();
        List<MISViewList> mvHOD = new List<MISViewList>();
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        var resFillPMS = await _performanceSettingAPIController.PerformanceReviewFillPMS(empSession.EmployeeId);
        if (resFillPMS != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resFillPMS).StatusCode == 200)
            {
                fillPMS = (bool)((Microsoft.AspNetCore.Mvc.ObjectResult)resFillPMS).Value;
            }
        }
        ViewBag.FillPMS = fillPMS;
        var resMISViewManager = await _performanceSettingAPIController.GetMISViewManager(empSession.UnitId ?? default(int), empSession.EmployeeId, 8);
        if (resMISViewManager != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resMISViewManager).StatusCode == 200)
            {
                mvManager = (List<MISViewList>)((Microsoft.AspNetCore.Mvc.ObjectResult)resMISViewManager).Value;
            }
        }
        var resMISViewHOD = await _performanceSettingAPIController.GetMISViewHOD(empSession.UnitId ?? default(int), empSession.EmployeeId, 8);
        if (resMISViewHOD != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resMISViewHOD).StatusCode == 200)
            {
                mvHOD = (List<MISViewList>)((Microsoft.AspNetCore.Mvc.ObjectResult)resMISViewHOD).Value;
            }
        }
        ViewBag.FillPMS = fillPMS;
        ViewBag.FillPMSManager = mvManager == null ? false : mvManager.Count == 0 ? false : true;
        ViewBag.FillPMSHOD = mvHOD == null ? false : mvHOD.Count == 0 ? false : true;
        ViewBag.SourceEmployee = CommonHelper.EncryptURLHTML("Employee");
        ViewBag.SourceManager = CommonHelper.EncryptURLHTML("Manager");
        ViewBag.SourceHOD = CommonHelper.EncryptURLHTML("HOD");
        return View();
    }
    public IActionResult PMSMasterKRA(string ePerformanceSettingId)
    {
        if (ePerformanceSettingId != null)
        {
            int PerformanceSettingId = Convert.ToInt32(CommonHelper.Decrypt(ePerformanceSettingId));
            PerformanceSettingDTO dto = new PerformanceSettingDTO();
            dto.PerformanceSettingId = PerformanceSettingId;
            return View(dto);
        }
        return null;
    }
    public IActionResult PMSMasterBehavioural(string ePerformanceSettingId)
    {
        if (ePerformanceSettingId != null)
        {
            int PerformanceSettingId = Convert.ToInt32(CommonHelper.Decrypt(ePerformanceSettingId));
            PerformanceSettingDTO dto = new PerformanceSettingDTO();
            dto.PerformanceSettingId = PerformanceSettingId;
            return View(dto);
        }
        return null;
        //return View();
    }

    public async Task<IActionResult> ViewPMSMasterKRA(string ePerformanceSettingId)
    {
        if (ePerformanceSettingId != null)
        {
            PerformanceSettingViewModel dto = new PerformanceSettingViewModel();
            int PerformanceSettingId = Convert.ToInt32(CommonHelper.Decrypt(ePerformanceSettingId));
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int? UnitId = empSession.UnitId;
            var res = await _performanceSettingAPIController.PerformanceDatabaseList(UnitId ?? default(int), "KRA", PerformanceSettingId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.PerformanceKRAMasterDBList = (List<PerformanceKRAMasterDBDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
            dto.encPerformanceSettingId = ePerformanceSettingId;
            dto.PerformanceSettingId = PerformanceSettingId;
            return View(dto);
        }
        return View();

    }
    public async Task<IActionResult> ViewPMSMasterBehavioural(string ePerformanceSettingId)
    {
        if (ePerformanceSettingId != null)
        {
            PerformanceSettingViewModel dto = new PerformanceSettingViewModel();
            int PerformanceSettingId = Convert.ToInt32(CommonHelper.Decrypt(ePerformanceSettingId));
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            int? UnitId = empSession.UnitId;
            var res = await _performanceSettingAPIController.PerformanceDatabaseList(UnitId ?? default(int), "Behavioral", PerformanceSettingId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.PerformanceKRAMasterDBList = (List<PerformanceKRAMasterDBDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
            dto.encPerformanceSettingId = ePerformanceSettingId;
            dto.PerformanceSettingId = PerformanceSettingId;
            return View(dto);
        }
        else
        {
            return View();
        }

    }
    public async Task<IActionResult> UploadKRAs(IFormFile importFile, int PerformanceSettingId)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null) { return BadRequest("Session may have expired. Please login again."); }

        if (importFile == null) return BadRequest("No File Selected");
        else if (Path.GetExtension(importFile.FileName) != ".csv") return BadRequest("Please Upload CSV file.");
        try
        {
            BL.Performance performance = new BL.Performance();
            var fileData = performance.GetKRADataFromCSVFile(importFile.OpenReadStream(), empSession.EmployeeId, empSession.UnitId ?? default(int), PerformanceSettingId);
            var resUploadKRADB = await _performanceSettingAPIController.UploadKRADB(fileData, empSession.UnitId ?? default(int), PerformanceSettingId);

            return resUploadKRADB;

            //return Ok("Success");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> UploadBehavioral(IFormFile importFile, int PerformanceSettingId)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null) { return BadRequest("Session may have expired. Please login again."); }

        if (importFile == null) return BadRequest("No File Selected");
        else if (Path.GetExtension(importFile.FileName) != ".csv") return BadRequest("Please Upload CSV file.");
        try
        {
            BL.Performance performance = new BL.Performance();
            var fileData = performance.GetBehavoralDataFromCSVFile(importFile.OpenReadStream(), empSession.EmployeeId, empSession.UnitId ?? default(int), PerformanceSettingId);
            var x = await _performanceSettingAPIController.UploadBehavioralDB(fileData, empSession.UnitId ?? default(int), PerformanceSettingId);
            return x;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public ActionResult DownloadTemplateKRA()
    {
        string filePath = "/Template/KRA.csv";
        return File(filePath, "application/octet-stream", "KRA.csv");
        //return File(stream, "application/octet-stream", "Reports.csv");
    }
    public ActionResult DownloadTemplateBehavioral()
    {
        string filePath = "/Template/BehavioralSkills.csv";
        return File(filePath, "application/octet-stream", "BehavioralSkills.csv");
        //return File(stream, "application/octet-stream", "Reports.csv");
    }

    [HttpPost]
    public async Task<IActionResult> SavePerformanceEmployeeData([FromBody] PerformanceEmployeeDataViewModel inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                return BadRequest("Invalid Data");
            }
            EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
            PerformanceEmployeeDataDTO dto = new PerformanceEmployeeDataDTO();
            inputDTO.LoggedInUserId = empSession.EmployeeId;
            var res = await _performanceSettingAPIController.SavePerformanceEmployeeData(inputDTO);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (PerformanceEmployeeDataDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    inputDTO.performanceEmployeeDataDTO.PerformanceEmployeeDataId = dto.PerformanceEmployeeDataId;

                    await _performanceSettingAPIController.SavePerformanceEmployeeKRAData(inputDTO);

                    await _performanceSettingAPIController.SavePerformanceEmployeeTrainingData(inputDTO);
                }
            }

            return Ok(res);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    public async Task<IActionResult> MISView(string? snc = null)
    {
        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            return BadRequest("Session Expired <a href='/Account/Login'>Login here</a>");
        }
        int? UnitId = empSession.UnitId;
        string source = "";
        if (snc != null)
        {
            source = CommonHelper.DecryptURLHTML(snc);
        }

        IActionResult res = null;
        MISViewListViewModel dto = new MISViewListViewModel();

        var resPMS = await _performanceSettingAPIController.PerformanceSettingList(UnitId ?? default(int));
        if (resPMS != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resPMS).StatusCode == 200)
            {
                dto.PerformanceSettingDTOs = (List<PerformanceSettingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resPMS).Value;
            }
        }

        if (dto.PerformanceSettingDTOs == null)
        {
            return View(dto);
        }
        if (dto.PerformanceSettingDTOs.Count == 0)
        {
            return View(dto);
        }
        dto.encSource = snc;
        dto.Source = source;
        return View(dto);
    }

    public async Task<IActionResult> MISView_ListPartialView([FromBody] MISViewList inputDTO)
    {

        EmployeeMasterDTO empSession = (EmployeeMasterDTO)(JsonConvert.DeserializeObject<EmployeeMasterDTO>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            return BadRequest("Session Expired <a href='/Account/Login'>Login here</a>");
        }
        int? UnitId = empSession.UnitId;
        if (inputDTO == null)
        {
            return BadRequest("Invalid Data");
        }

        IActionResult res = null;
        MISViewListViewModel dto = new MISViewListViewModel();

        int PerformanceSettingId = inputDTO.PerformanceSettingId ?? default(int);

        var isClient = HttpContext.Session.GetString("isClient");
        if (isClient == "True" && (inputDTO.Source == ""))
        {
            inputDTO.Source = "ClientAdmin";
            inputDTO.encSource = CommonHelper.EncryptURLHTML(inputDTO.Source);
        }
        res = await _performanceSettingAPIController.GetMISView(UnitId ?? default(int), empSession.EmployeeId, PerformanceSettingId, inputDTO.Source);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.MISViewLists = (List<MISViewList>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

            }
        }
        foreach (var item in dto.MISViewLists)
        {
            item.Source = inputDTO.Source;
            item.encSource = CommonHelper.EncryptURLHTML(inputDTO.Source);
            item.encEmployeeId = CommonHelper.EncryptURLHTML(item.EmployeeId.ToString());
            item.PerformanceSettingId = PerformanceSettingId;
            item.encPerformanceSettingId = CommonHelper.EncryptURLHTML(PerformanceSettingId.ToString());
        }

        return PartialView("_misView/_misViewList", dto);
    }

    public async Task<IActionResult> PMSReport()
    {
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            return BadRequest("Session Expired <a href='/Account/Login'>Login here</a>");
        }
        int? UnitId = empSession.UnitId;
        PMSReportListViewModel dto = new PMSReportListViewModel();

        var resPMS = await _performanceSettingAPIController.PerformanceSettingList(UnitId ?? default(int));
        if (resPMS != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resPMS).StatusCode == 200)
            {
                dto.PerformanceSettingList = (List<PerformanceSettingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resPMS).Value;
            }
        }
        return View(dto);
    }


    public async Task<IActionResult> PMSReport_ListPartialView([FromBody] PMSReportViewModel inputDTO)
    {
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            return BadRequest("Session Expired <a href='/Account/Login'>Login here</a>");
        }
        if (inputDTO.PerformanceSettingId == null)
        {
            return BadRequest("Session Expired <a href='/Account/Login'>Login here</a>");
        }
        int? UnitId = empSession.UnitId;
        PMSReportListViewModel dto = new PMSReportListViewModel();

        //var resPMS = await _performanceSettingAPIController.PerformanceSettingList(UnitId ?? default(int));
        //if (resPMS != null)
        //{
        //    if (((Microsoft.AspNetCore.Mvc.ObjectResult)resPMS).StatusCode == 200)
        //    {
        //        dto.PerformanceSettingList = (List<PerformanceSettingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resPMS).Value;
        //    }
        //}



        var res = await _performanceSettingAPIController.GetPMSReport(UnitId ?? default(int), inputDTO.PerformanceSettingId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.PMSReportList = (List<PMSReportViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_pmsReport/_pmsReportList", dto);
    }
    public async Task<IActionResult> TrainingNeedReport()
    {
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            return BadRequest("Session Expired <a href='/Account/Login'>Login here</a>");
        }
        int? UnitId = empSession.UnitId;
        TrainingNeedReportListViewModel dto = new TrainingNeedReportListViewModel();

        var resTNR = await _performanceSettingAPIController.PerformanceSettingList(UnitId ?? default(int));
        if (resTNR != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resTNR).StatusCode == 200)
            {
                dto.PerformanceSettingList = (List<PerformanceSettingDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resTNR).Value;
            }
        }
        return View(dto);



    }

    public async Task<IActionResult> TrainingNeedReport_ListPartialView([FromBody] TrainingNeedReportViewModel inputDTO)
    {
        EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
        if (empSession == null)
        {
            return BadRequest("Session Expired <a href='/Account/Login'>Login here</a>");
        }
        if (inputDTO.PerformanceSettingId == null)
        {
            return BadRequest("Session Expired <a href='/Account/Login'>Login here</a>");
        }
        int? UnitId = empSession.UnitId;
        TrainingNeedReportListViewModel dto = new TrainingNeedReportListViewModel();


        var res = await _performanceSettingAPIController.GetTrainingNeedReport(UnitId ?? default(int), inputDTO.PerformanceSettingId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.TrainingNeedReportList = (List<TrainingNeedReportViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_trainingNeedReport/_trainingNeedReportList", dto);
    }
    public async Task<IActionResult> PublishPerformace([FromBody] PerformanceEmployeeDataDTO inputDTO)
    {
        try
        {
            var isClient = HttpContext.Session.GetString("isClient");
            if (isClient == "True")
            {
                var res = await _performanceSettingAPIController.PublishPerformacne(inputDTO.PerformanceEmployeeDataId);
                return res;
            }
            else
            {
                return Unauthorized("Unauthorized Access");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> DeletePMSSetting([FromBody] PerformanceSettingViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.encPerformanceSettingId != null)
                {
                    inputDTO.PerformanceSettingId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encPerformanceSettingId));
                    var res = await _performanceSettingAPIController.DeletePMSSetting(inputDTO);
                    return res;
                }
                else
                {
                    return BadRequest("Error while deleting data");
                }
            }
            else
            {
                return BadRequest("Error while deleting data");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
