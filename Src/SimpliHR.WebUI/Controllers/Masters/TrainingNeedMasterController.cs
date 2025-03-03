using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Endpoints.MastersKeyValue;
using SimpliHR.Endpoints.Performance;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Performace;
using System.Collections.Generic;

namespace SimpliHR.WebUI.Controllers.Masters;

public class TrainingNeedMasterController : Controller
{
    private readonly TrainingNeedMasterAPIController _trainingNeedMasterAPIController;
    public TrainingNeedMasterController(TrainingNeedMasterAPIController trainingNeedMasterAPIController)
    {
        _trainingNeedMasterAPIController = trainingNeedMasterAPIController;
    }
    public IActionResult TrainingNeedMaster(string ePerformanceSettingId)
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
    public async Task<IActionResult> TrainingNeedMaster_ListPartialView([FromBody] PerformanceSettingDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
                if (empSession != null)
                {
                    var res = await _trainingNeedMasterAPIController.TrainingNeedMasterList(empSession.UnitId ?? default(int), inputDTO.PerformanceSettingId);
                    if (res != null)
                    {
                        if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                        {
                            List<PerformanceTrainingNeedsMasterDTO> dto = (List<PerformanceTrainingNeedsMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                            return PartialView("_trainingNeedMaster/_list", dto);
                        }
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Session has expired. Please login again to continue");
                }
            }
            else
            {
                return BadRequest("Invalid Model");
            }
            return BadRequest("Error Occurred");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<IActionResult> TrainingNeedMaster_AddPartialView([FromBody] PerformanceTrainingNeedsMasterDTO inputDTO)
    {
        try
        {
            PerformanceTrainingNeedsMasterDTO? dto = new PerformanceTrainingNeedsMasterDTO();

            if (inputDTO != null && inputDTO.TrainingNeedsMasterId > 0)
            {
                var res = await _trainingNeedMasterAPIController.TrainingNeedMasterById(inputDTO.TrainingNeedsMasterId);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        dto = (PerformanceTrainingNeedsMasterDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    }
                }
            }
            return PartialView("_trainingNeedMaster/_add", dto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    public async Task<IActionResult> SaveTraining([FromBody] PerformanceTrainingNeedsMasterDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                EmployeeMasterDTO? empSession = (EmployeeMasterDTO?)(JsonConvert.DeserializeObject<EmployeeMasterDTO?>(HttpContext.Session.GetString("employee")));
                if (empSession != null)
                {
                    inputDTO.IsActive = true;
                    inputDTO.UnitId = empSession.UnitId ?? default(int);
                    var res = await _trainingNeedMasterAPIController.SaveTrainingNeedMaster(inputDTO);
                    return res;
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Session has expired. Please login again to continue");
                }
            }
            else
            {
                return BadRequest("Invalid Model");
            }
            return BadRequest("Error Occurred");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    public async Task<IActionResult> DeleteTraining([FromBody] PerformanceTrainingNeedsMasterDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var res = await _trainingNeedMasterAPIController.DeleteTrainingNeedMaster(inputDTO);
                return res;
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
