using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Master;
using SimpliHR.Infrastructure.Models.Performace;
using System.Collections.Generic;

namespace SimpliHR.Endpoints.Performance;
[Route("api/[controller]")]
[ApiController]
public class PerformanceSettingAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PerformanceSettingAPIController> _logger;
    private readonly IMapper _mapper;
    public PerformanceSettingAPIController(IUnitOfWork unitOfWork, ILogger<PerformanceSettingAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> SavePMSSetting(PerformanceSettingViewModel inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.PerformanceSetting.PerformanceSettingId == 0)
                {

                    List<PerformanceSetting> ecm1 = await _unitOfWork.PerformanceSetting.GetQueryAll("select 1 a from PerformanceSetting where IsActive=1");

                    var ecm = await _unitOfWork.PerformanceSetting.GetFilterAll(x => x.UnitId == inputDTO.PerformanceSetting.UnitId && x.IsActive == true && (
                    (x.ReviewPeriodFrom <= inputDTO.PerformanceSetting.ReviewPeriodFrom && x.ReviewPeriodTo >= inputDTO.PerformanceSetting.ReviewPeriodFrom) ||
                    (x.ReviewPeriodFrom <= inputDTO.PerformanceSetting.ReviewPeriodTo && x.ReviewPeriodTo >= inputDTO.PerformanceSetting.ReviewPeriodTo) ||
                    (x.ReviewPeriodFrom >= inputDTO.PerformanceSetting.ReviewPeriodFrom && x.ReviewPeriodTo <= inputDTO.PerformanceSetting.ReviewPeriodTo)
                    ));

                    if (ecm.Count == 0)
                    {
                        int insertedId = await _unitOfWork.PerformanceSetting.AddAsync(_mapper.Map<PerformanceSetting>(inputDTO.PerformanceSetting));
                        inputDTO.PerformanceSetting.PerformanceSettingId = insertedId;
                        await SavePMSSettingSkillSetMatrix(inputDTO);
                        await SavePMSAverageMethod(inputDTO);
                        _unitOfWork.Save();

                        return Ok("Success");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry Found");
                    }
                }
                else
                {
                    List<PerformanceSetting> ecm1 = await _unitOfWork.PerformanceSetting.GetQueryAll("select 1 a from PerformanceSetting where PerformanceSettingId!=" + inputDTO.PerformanceSetting.PerformanceSettingId + " and IsActive=1");
                    var ecm = await _unitOfWork.PerformanceSetting.GetFilterAll(x => x.PerformanceSettingId != inputDTO.PerformanceSetting.PerformanceSettingId && x.UnitId == inputDTO.PerformanceSetting.UnitId && x.IsActive == true &&
                    (
                    (x.ReviewPeriodFrom <= inputDTO.PerformanceSetting.ReviewPeriodFrom && x.ReviewPeriodTo >= inputDTO.PerformanceSetting.ReviewPeriodFrom)
                    ||
                    (x.ReviewPeriodFrom <= inputDTO.PerformanceSetting.ReviewPeriodTo && x.ReviewPeriodTo >= inputDTO.PerformanceSetting.ReviewPeriodTo)
                    ||
                    (x.ReviewPeriodFrom >= inputDTO.PerformanceSetting.ReviewPeriodFrom && x.ReviewPeriodTo <= inputDTO.PerformanceSetting.ReviewPeriodTo)
                    )
                    );
                    if (ecm.Count == 0)
                    {
                        PerformanceSetting p = await _unitOfWork.PerformanceSetting.FindByIdAsync(inputDTO.PerformanceSetting.PerformanceSettingId);
                        inputDTO.PerformanceSetting.CreatedDate = p.CreatedDate;
                        inputDTO.PerformanceSetting.IsActive = p.IsActive;
                        await _unitOfWork.PerformanceSetting.UpdateAsync(_mapper.Map<PerformanceSetting>(inputDTO.PerformanceSetting));
                        await SavePMSSettingSkillSetMatrix(inputDTO);
                        await SavePMSAverageMethod(inputDTO);
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Duplicate Entry Found");
                    }
                }
            }
            return Ok("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SavePMSSetting)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> SavePMSSettingSkillSetMatrix(PerformanceSettingViewModel inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.PerformanceSetting.PerformanceSettingId != 0)
                {
                    if (inputDTO.PerformanceSettingSkillSetMatrixList != null)
                    {
                        if (inputDTO.PerformanceSettingSkillSetMatrixList.Count > 0)
                        {
                            await DeletePerformanceSettingSkillSetMatrix(inputDTO.PerformanceSetting.PerformanceSettingId);
                            List<PerformanceSettingSkillSetMatrix> listEntity = new List<PerformanceSettingSkillSetMatrix>();
                            foreach (var item in inputDTO.PerformanceSettingSkillSetMatrixList)
                            {
                                PerformanceSettingSkillSetMatrix entity = new PerformanceSettingSkillSetMatrix();
                                entity.PerformanceSettingId = inputDTO.PerformanceSetting.PerformanceSettingId;
                                entity.SoftSkillsWeightage = item.SoftSkillsWeightage;
                                entity.BandId = item.BandId;
                                entity.KRAWeightage = item.KRAWeightage;
                                listEntity.Add(entity);
                                await _unitOfWork.PerformanceSettingSkillSetMatrix.AddAsync(entity);
                                _unitOfWork.Save();
                            }
                            return Ok("Success");

                        }
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to update the settings");
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SavePMSSetting)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> SavePMSAverageMethod(PerformanceSettingViewModel inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.PerformanceSetting.PerformanceSettingId != 0)
                {
                    if (inputDTO.PerformanceSettingMechanismList != null)
                    {
                        if (inputDTO.PerformanceSettingMechanismList.Count > 0)
                        {
                            await DeletePerformanceSettingAverageMethod(inputDTO.PerformanceSetting.PerformanceSettingId);
                            List<PerformanceSettingMechanism> listEntity = new List<PerformanceSettingMechanism>();
                            foreach (var item in inputDTO.PerformanceSettingMechanismList)
                            {
                                PerformanceSettingMechanism entity = _mapper.Map<PerformanceSettingMechanism>(item);
                                entity.PerformanceSettingId = inputDTO.PerformanceSetting.PerformanceSettingId;
                                listEntity.Add(entity);
                                await _unitOfWork.PerformanceSettingMechanism.AddAsync(entity);
                                _unitOfWork.Save();
                            }
                            return Ok("Success");

                        }
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to update the settings");
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SavePMSSetting)}");
            throw;
        }
    }


    public async Task<IActionResult> DeletePerformanceSettingSkillSetMatrix(int PerformanceSettingId)
    {
        try
        {
            string query = "select * from PerformanceSettingSkillSetMatrix where PerformanceSettingId=" + PerformanceSettingId;
            List<PerformanceSettingSkillSetMatrix> dto = await _unitOfWork.PerformanceSettingSkillSetMatrix.GetTableData<PerformanceSettingSkillSetMatrix>(query);
            foreach (PerformanceSettingSkillSetMatrix entity in dto)
            {
                await _unitOfWork.PerformanceSettingSkillSetMatrix.DeleteAsync(entity.PerformanceSettingSkillSetMatrixId);
                _unitOfWork.Save();
            }
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> DeletePerformanceSettingAverageMethod(int PerformanceSettingId)
    {
        try
        {
            string query = "select * from PerformanceSettingMechanism where PerformanceSettingId=" + PerformanceSettingId;
            List<PerformanceSettingMechanism> dto = await _unitOfWork.PerformanceSettingMechanism.GetTableData<PerformanceSettingMechanism>(query);
            foreach (PerformanceSettingMechanism entity in dto)
            {
                await _unitOfWork.PerformanceSettingMechanism.DeleteAsync(entity.PerformanceSettingMechanismId);
                _unitOfWork.Save();
            }
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceSettingSkillSetMatrixList(int PerformanceSettingId)
    {
        try
        {
            string query = "select * from PerformanceSettingSkillSetMatrix where PerformanceSettingId=" + PerformanceSettingId;
            List<PerformanceSettingSkillSetMatrixDTO> dto = await _unitOfWork.PerformanceSettingSkillSetMatrix.GetTableData<PerformanceSettingSkillSetMatrixDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceSettingSkillSetMatrixBandWise(int PerformanceSettingId, int Band)
    {
        try
        {
            string query = "select * from PerformanceSettingSkillSetMatrix where PerformanceSettingId=" + PerformanceSettingId + " and BandId=" + Band + "";
            List<PerformanceSettingSkillSetMatrixDTO> dto = await _unitOfWork.PerformanceSettingSkillSetMatrix.GetTableData<PerformanceSettingSkillSetMatrixDTO>(query);
            if (dto.Count > 0)
            {
                return Ok(dto.FirstOrDefault());
            }
            else
            {
                return BadRequest("Not Found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceSettingAverageMethod(int PerformanceSettingId)
    {
        try
        {
            string query = "select * from PerformanceSettingMechanism where PerformanceSettingId=" + PerformanceSettingId;
            List<PerformanceSettingMechanismDTO> dto = await _unitOfWork.PerformanceSettingMechanism.GetTableData<PerformanceSettingMechanismDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }

    public async Task<IActionResult> PMSList(int UnitId)
    {
        try
        {
            string query = "select   PerformanceSettingId, UnitId, ReviewPeriodFrom, ReviewPeriodTo, (Case when AssesmentPeriodicity=1 then 'Daily' when AssesmentPeriodicity=2 then 'Monthly'  when AssesmentPeriodicity=3 then 'Quarterly'  when AssesmentPeriodicity=4 then 'Half Yearly' when AssesmentPeriodicity=5 then 'Yearly' end )AssesmentPeriodicity, ( Case when RollOut=1 then 'Automatically' when RollOut=2 then 'Manually'  end )RollOut, ( Case when Mechanism=1 then 'Simple Average' when Mechanism=2 then 'Weighed Average'  when Mechanism=3 then 'OKR'  end  ) Mechanism  from performancesetting where isactive=1 and unitId=" + UnitId + " order by ReviewPeriodFrom desc ";

            List<PerformanceSettingWithChildEntity> dto = await _unitOfWork.PerformanceSetting.GetTableData<PerformanceSettingWithChildEntity>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceSettingList(int UnitId)
    {
        try
        {
            string query = "select   PerformanceSettingId, UnitId, ReviewPeriodFrom, ReviewPeriodTo,AssesmentPeriodicity,RollOut,Mechanism  from performancesetting where isactive=1 and unitId=" + UnitId + " order by ReviewPeriodFrom desc ";

            List<PerformanceSettingDTO> dto = await _unitOfWork.PerformanceSetting.GetTableData<PerformanceSettingDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceDatabaseList(int UnitId, string Source, int PerformanceSettingId)
    {
        try
        {
            string query = "select * from PerformanceKRAMasterDB where unitId=" + UnitId + " and Source='" + Source + "' and PerformanceSettingId=" + PerformanceSettingId + "";
            List<PerformanceKRAMasterDBDTO> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<PerformanceKRAMasterDBDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceDatabaseListByEmployee(int UnitId, string EmployeeCode, int PerformanceSettingId)
    {
        try
        {
            string query = "select * from PerformanceKRAMasterDB where unitId=" + UnitId + " and EmployeeCode='" + EmployeeCode + "' and PerformanceSettingId=" + PerformanceSettingId + "";
            List<PerformanceKRAMasterDBDTO> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<PerformanceKRAMasterDBDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceEmployeeKRADataListByEmployee(int UnitId, string EmployeeCode, int PerformanceSettingId, int PerformanceEmployeeDataId)
    {
        try
        {
            string query = @"select PerformanceKRAMasterDBId,EmployeeCode,KRA,Weightage,Source,PerformanceSettingId
            ,(select top 1 EmployeeRating from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)EmployeeRating" +
            ",(select top 1 EmployeeRemarks from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)EmployeeRemarks" +
            ",(select top 1 ManagerRating from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)ManagerRating" +
            ",(select top 1 ManagerRemarks from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)ManagerRemarks" +
            ",(select top 1 WAScore from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)WAScore " +
            "from PerformanceKRAMasterDB p1 where unitId=" + UnitId + " and EmployeeCode='" + EmployeeCode + "' and PerformanceSettingId=" + PerformanceSettingId + " and p1.IsActive=1";
            List<PerformanceEmployeeKRADataViewModel> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<PerformanceEmployeeKRADataViewModel>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceEmployeeKRABehaviouralDataListByEmployee(int UnitId, string EmployeeCode, int PerformanceSettingId, int PerformanceEmployeeDataId, string Type)
    {
        try
        {
            string query = @"select PerformanceKRAMasterDBId,EmployeeCode,KRA,Weightage,Source,PerformanceSettingId
            ,(select top 1 EmployeeRating from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)EmployeeRating" +
            ",(select top 1 EmployeeRemarks from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)EmployeeRemarks" +
            ",(select top 1 ManagerRating from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)ManagerRating" +
            ",(select top 1 ManagerRemarks from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)ManagerRemarks" +
            ",(select top 1 WAScore from [PerformanceEmployeeKRAData] p2 where PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + " and p2.KRA=p1.KRA and Source=p1.Source order by PerformanceEmployeeKRADataId desc)WAScore " +
            "from PerformanceKRAMasterDB p1 where unitId=" + UnitId + " and EmployeeCode='" + EmployeeCode + "' and PerformanceSettingId=" + PerformanceSettingId + " and p1.IsActive=1";
            List<PerformanceEmployeeKRADataViewModel> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<PerformanceEmployeeKRADataViewModel>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }

    public async Task<IActionResult> PerformanceEmployeeTrainingNeedListByEmployee(int UnitId, string EmployeeCode, int PerformanceSettingId, int PerformanceEmployeeDataId)
    {
        try
        {
            string query = @"select p1.TrainingNeedsMasterId ,Training ,(select p2.TrainingType from [PerformanceEmployeeTrainingData] p2 where p1.TrainingNeedsMasterId=p2.TrainingNeedsMasterId and PerformanceEmployeeDataId=" + PerformanceEmployeeDataId + ")TrainingType ,(select p2.TrainingUrgency from [PerformanceEmployeeTrainingData] p2 where p1.TrainingNeedsMasterId=p2.TrainingNeedsMasterId and PerformanceEmployeeDataId= " + PerformanceEmployeeDataId + ")TrainingUrgency from [dbo].[PerformanceTrainingNeedsMaster] p1 where isactive=1 and unitId=" + UnitId + " and PerformanceSettingId=" + PerformanceSettingId + " ";
            List<PerformanceEmployeeTrainingDataDTO> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<PerformanceEmployeeTrainingDataDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceTrainingNeedKeyValues()
    {
        try
        {
            string query = @"select * from PageControlKeyValues where Module='Performance' and PageName='PMSWireFrame'";
            List<PageKeyValues> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<PageKeyValues>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PMSList)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetPMSById(int PerformanceSettingId)
    {
        try
        {
            var res = _mapper.Map<PerformanceSettingDTO>(await _unitOfWork.PerformanceSetting.FindByIdAsync(PerformanceSettingId));
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest("No Data Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Group {nameof(GetPMSById)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetPMSByCurrentDate(DateTime currentDate, int unitId)
    {
        try
        {
            var res = _mapper.Map<PerformanceSettingDTO>(await _unitOfWork.PerformanceSetting.GetFilter(x => x.ReviewPeriodFrom <= currentDate && x.ReviewPeriodTo > currentDate && x.UnitId == unitId && x.IsActive == true));
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest("No Data Found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Group {nameof(GetPMSById)}");
            throw;
        }
    }
    public async Task<IActionResult> UploadKRADB(List<PerformanceKRAMasterDBDTO> data, int unitId, int PerformanceSettingId)
    {
        try
        {
            string res = await _unitOfWork.PerformanceKRAMasterDB.UploadKRADB(data, unitId, PerformanceSettingId);

            if (res == "OK")
            {
                return Ok();
            }
            else
            {
                return BadRequest(res);
            }
            //string sQuery = @"DELETE FROM PerformanceKRAMasterDB WHERE unitID = " + unitId + " and Source='KRA' and PerformanceSettingId=" + PerformanceSettingId + "";
            //bool isSuccess = await _unitOfWork.PerformanceKRAMasterDB.RunSQLCommand(sQuery);
            //if (isSuccess)
            //{
            //    sQuery = @"
            //            insert into PerformanceKRAMasterDB(EmployeeCode,KRA,Weightage,CreatedDate,CreatedBy,IsActive,UnitId,Source,PerformanceSettingId)
            //            values(@EmployeeCode, @KRA,@Weightage,@CreatedDate,@CreatedBy,@IsActive,@UnitId,@Source,@PerformanceSettingId)";
            //    isSuccess = await _unitOfWork.PerformanceKRAMasterDB.ExecuteListData<PerformanceKRAMasterDB>(_mapper.Map<List<PerformanceKRAMasterDB>>(data), sQuery);
            //    _unitOfWork.Save();
            //    //List<EmployeeMasterDTO> employeeCodes = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<EmployeeMasterDTO>("Select EmployeeCode from PerformanceKRAMasterDB group by EmployeeCode having sum(Weightage) < 100");
            //    //
            //    if (isSuccess)
            //    {
            //        return Ok("");
            //    }
            //    else
            //    {
            //        return BadRequest("Error In Uploading KRAs");
            //    }
            //}
            //return BadRequest("Some Error has Occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UploadKRADB)}");
            throw;
        }
    }

    public async Task<IActionResult> CheckWeightageKRA(int unitId, int PerformanceSettingId)
    {
        List<EmployeeMasterDTO> employeeCodes = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<EmployeeMasterDTO>($"Select EmployeeCode from PerformanceKRAMasterDB where UnitId=1 and PerformanceSettingId=1 and Source='KRA' and IsActive=1 group by EmployeeCode having sum(Weightage) < 100");
        if (employeeCodes != null && employeeCodes.Count() > 0)
        {
            return BadRequest(employeeCodes.ToString() + " has weightage that is not equal to 100");
        }
        else
        {

            return Ok("Success");
        }
    }

    public async Task<IActionResult> UploadBehavioralDB(List<PerformanceKRAMasterDBDTO> data, int unitId, int PerformanceSettingId)
    {
        try
        {

            string res = await _unitOfWork.PerformanceKRAMasterDB.UploadBehavioralDB(data, unitId, PerformanceSettingId);

            if (res == "OK")
            {
                return Ok();
            }
            else
            {
                return BadRequest(res);
            }


            //string sQuery = @"DELETE FROM PerformanceKRAMasterDB WHERE unitID = " + unitId + " and Source='Behavioral' and PerformanceSettingId=" + PerformanceSettingId + "";
            //bool isSuccess = await _unitOfWork.PerformanceKRAMasterDB.RunSQLCommand(sQuery);
            //if (isSuccess)
            //{
            //    sQuery = @"
            //            insert into PerformanceKRAMasterDB(EmployeeCode,KRA,Weightage,CreatedDate,CreatedBy,IsActive,UnitId,Source,PerformanceSettingId)
            //            values(@EmployeeCode, @KRA,@Weightage,@CreatedDate,@CreatedBy,@IsActive,@UnitId,@Source,@PerformanceSettingId)";
            //    isSuccess = await _unitOfWork.PerformanceKRAMasterDB.ExecuteListData<PerformanceKRAMasterDB>(_mapper.Map<List<PerformanceKRAMasterDB>>(data), sQuery);

            //}
            //return Ok("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UploadKRADB)}");
            throw;
        }
    }

    public async Task<IActionResult> SavePerformanceEmployeeData(PerformanceEmployeeDataViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.performanceEmployeeDataDTO != null)
                {
                    PerformanceEmployeeDataDTO dto = inputDTO.performanceEmployeeDataDTO;
                    PerformanceEmployeeData res = await _unitOfWork.PerformanceEmployeeData.GetFilter(x => x.EmployeeId == inputDTO.performanceEmployeeDataDTO.EmployeeId && x.PerformanceSettingId == inputDTO.performanceEmployeeDataDTO.PerformanceSettingId);
                    if (res != null)
                    {
                        var performanceData = await _unitOfWork.PerformanceSetting.FindByIdAsync(res.PerformanceSettingId);
                        if (inputDTO.ButtonType != null && inputDTO.ButtonType != "Save")
                        {
                            if (dto.ViewType == "Employee")
                            {
                                res.ModifiedDateEmployee = DateTime.Now;
                                res.ModifiedByEmployee = inputDTO.LoggedInUserId;
                                res.FilledByEmployee = true;
                            }
                            else if (dto.ViewType == "Manager")
                            {
                                res.ModifiedDateManager = DateTime.Now;
                                res.ModifiedByManager = inputDTO.LoggedInUserId;
                                res.FilledByManager = true;

                                if (performanceData.HODClosingRemarks != true && performanceData.HODReview != true)
                                {
                                    res.FilledByHOD = true;
                                }
                            }
                            else if (dto.ViewType == "HOD")
                            {
                                res.ModifiedDateHOD = DateTime.Now;
                                res.ModifiedByHOD = inputDTO.LoggedInUserId;
                                res.FilledByHOD = true;
                            }
                        }

                        res.KRAWeightageTotal = dto.KRAWeightageTotal;
                        res.KRAManagersRatingTotal = dto.KRAManagersRatingTotal;
                        res.KRAScoreTotal = dto.KRAScoreTotal;
                        res.BehaviouralSkillsWeightageTotal = dto.BehaviouralSkillsWeightageTotal;
                        res.BehaviouralSkillsManagersRatingTotal = dto.BehaviouralSkillsManagersRatingTotal;
                        res.BehaviouralSkillsScoreTotal = dto.BehaviouralSkillsScoreTotal;
                        res.ClosingRemarksEmployee = dto.ClosingRemarksEmployee;
                        res.ClosingRemarksManager = dto.ClosingRemarksManager;
                        res.RatingCalculationKRAWeightage = dto.RatingCalculationKRAWeightage;
                        res.RatingCalculationKRAScore = dto.RatingCalculationKRAScore;
                        res.RatingCalculationKRAFinalScore = dto.RatingCalculationKRAFinalScore;
                        res.RatingCalculationBehaviouralSkillsWeightage = dto.RatingCalculationBehaviouralSkillsWeightage;
                        res.RatingCalculationBehaviouralSkillsScore = dto.RatingCalculationBehaviouralSkillsScore;
                        res.RatingCalculationBehaviouralSkillsFinalScore = dto.RatingCalculationBehaviouralSkillsFinalScore;
                        res.RatingCalculationFinalScore = dto.RatingCalculationFinalScore;
                        res.RatingCalculationFinalRating = dto.RatingCalculationFinalRating;
                        res.RatingCalculationFinalRatingId = dto.RatingCalculationFinalRatingId;
                        res.HODFinalRating = dto.HODFinalRating;
                        res.HODFinalRatingId = dto.HODFinalRatingId;
                        res.ClosingRemarksHOD = dto.ClosingRemarksHOD;
                        await _unitOfWork.PerformanceEmployeeData.UpdateAsync(res);
                        _unitOfWork.Save();
                        return Ok(_mapper.Map<PerformanceEmployeeDataDTO>(res));
                    }
                    else
                    {
                        if (dto.ViewType == "Employee")
                        {
                            dto.CreatedDate = DateTime.Now;
                            dto.CreatedBy = inputDTO.LoggedInUserId;
                            if (inputDTO.ButtonType != null && inputDTO.ButtonType != "Save")
                            {
                                dto.FilledByEmployee = true;
                            }
                        }
                        int Id = await _unitOfWork.PerformanceEmployeeData.AddAsync(_mapper.Map<PerformanceEmployeeData>(dto));
                        inputDTO.performanceEmployeeDataDTO.PerformanceEmployeeDataId = Id;
                        return Ok(inputDTO.performanceEmployeeDataDTO);
                    }
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UploadKRADB)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceEmployeeData(int EmployeeId, int PerformanceSettingId)
    {
        try
        {
            PerformanceEmployeeDataDTO dto = _mapper.Map<PerformanceEmployeeDataDTO>(await _unitOfWork.PerformanceEmployeeData.GetFilter(x => x.EmployeeId == EmployeeId && x.PerformanceSettingId == PerformanceSettingId));
            if (dto != null)
            {
                return Ok(dto);
            }
            else
            {
                return BadRequest("No data found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UploadKRADB)}");
            throw;
        }
    }
    public async Task<IActionResult> SavePerformanceEmployeeKRAData(PerformanceEmployeeDataViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.performanceEmployeeDataDTO != null && inputDTO.PerformanceEmployeeKRADatas != null)
                {
                    string sQuery = @"DELETE FROM [PerformanceEmployeeKRAData] WHERE PerformanceEmployeeDataId=" + inputDTO.performanceEmployeeDataDTO.PerformanceEmployeeDataId + "";
                    bool isSuccess = await _unitOfWork.PerformanceEmployeeKRAData.RunSQLCommand(sQuery);
                    if (isSuccess)
                    {
                        sQuery = @"
                        insert into [PerformanceEmployeeKRAData](PerformanceEmployeeDataId,SNo,KRA,Weightage,EmployeeRating,EmployeeRemarks,ManagerRating,ManagerRemarks,WAScore,Source) Values(" + inputDTO.performanceEmployeeDataDTO.PerformanceEmployeeDataId + ",@SNo,@KRA,@Weightage,@EmployeeRating,@EmployeeRemarks,@ManagerRating,@ManagerRemarks,@WAScore,@Source)";
                        isSuccess = await _unitOfWork.PerformanceEmployeeKRAData.ExecuteListData<PerformanceEmployeeKRAData>(_mapper.Map<List<PerformanceEmployeeKRAData>>(inputDTO.PerformanceEmployeeKRADatas), sQuery);
                        if (isSuccess)
                        {
                            return Ok("Success");
                        }
                        else
                        {
                            return BadRequest("Error While saving data");
                        }
                    }
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(UploadKRADB)}");
            throw;
        }
    }

    public async Task<IActionResult> SavePerformanceEmployeeTrainingData(PerformanceEmployeeDataViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.performanceEmployeeDataDTO != null && inputDTO.PerformanceEmployeeTrainingDatas != null)
                {
                    string sQuery = @"DELETE FROM [PerformanceEmployeeTrainingData] WHERE PerformanceEmployeeDataId=" + inputDTO.performanceEmployeeDataDTO.PerformanceEmployeeDataId + "";
                    bool isSuccess = await _unitOfWork.PerformanceEmployeeTrainingData.RunSQLCommand(sQuery);
                    if (isSuccess)
                    {
                        sQuery = @"
                        insert into PerformanceEmployeeTrainingData(TrainingNeedsMasterId,PerformanceEmployeeDataId,TrainingType,TrainingUrgency) Values(@TrainingNeedsMasterId," + inputDTO.performanceEmployeeDataDTO.PerformanceEmployeeDataId + ",@TrainingType,@TrainingUrgency)";
                        isSuccess = await _unitOfWork.PerformanceEmployeeTrainingData.ExecuteListData<PerformanceEmployeeTrainingData>(_mapper.Map<List<PerformanceEmployeeTrainingData>>(inputDTO.PerformanceEmployeeTrainingDatas), sQuery);
                        if (isSuccess)
                        {
                            return Ok("Success");
                        }
                        else
                        {
                            return BadRequest("Error While saving data");
                        }
                    }
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePerformanceEmployeeTrainingData)}");
            throw;
        }
    }

    public async Task<IActionResult> GetMISView(int UnitId, int EmployeeId, int PerformanceSettingId, string Source)
    {
        try
        {
            string query = @"
            select EmployeeId
            ,EmployeeCode
            ,EmployeeName
            ,(select JobTitle from Jobtitlemaster jm where em.JobTitleId = jm.JobTitleId)Designation 
            ,(select DepartmentName from DepartmentMaster jm where em.DepartmentId = jm.DepartmentId)Department 
            ,(select RoleName from RoleMaster jm where em.RoleId=jm.RoleId)[Function] 
            ,(select Location from WorkLocationMaster wm where wm.WorkLocationId = em.WorkLocationId)WorkLocation 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.ManagerId) Manager 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.HODId) HOD 
            ,(Case when((select count(1) from PerformanceSetting where PerformanceSettingId=" + PerformanceSettingId + ") = 0) then 'Goals/KRA''s Setting Due' " +
              "when((select count(1) from[dbo].[PerformanceKRAMasterDB] where EmployeeCode = em.EmployeeCode and PerformanceSettingId=" + PerformanceSettingId + ")= 0) then 'Goals / KRAs Acceptance Due'" +
              "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByHOD = 1) = 1) then 'Completed' " +
              "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByManager=1)=1) then 'HOD''s Assessment Due' " +
              "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByEmployee = 1) = 1) then 'Manager''s Assessment Due' " +
              "else 'Self Assessment Due' end) PMSStatus " +
            ",(select top 1 PerformanceEmployeeDataId from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + ")PerformanceEmployeeDataId " +
            ",(select top 1 Published from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + ")Published " +
            "from EmployeeMaster em where InfoFillingStatus=1 and IsActive=1 and UnitId=" + UnitId + "";

            if (Source == "Employee")
                query += " and em.EmployeeId=" + EmployeeId + "";
            else if (Source == "Manager")
                query += " and em.managerId=" + EmployeeId + "";
            else if (Source == "HOD")
                query += " and em.HODId=" + EmployeeId + "";

            List<MISViewList> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<MISViewList>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> GetMISViewManager(int UnitId, int EmployeeId, int PerformanceSettingId)
    {
        try
        {
            string query = @"
            select EmployeeId
            ,EmployeeCode
            ,EmployeeName
            ,(select JobTitle from Jobtitlemaster jm where em.JobTitleId = jm.JobTitleId)Designation 
            ,(select DepartmentName from DepartmentMaster jm where em.DepartmentId = jm.DepartmentId)Department 
            ,(select RoleName from RoleMaster jm where em.RoleId=jm.RoleId)[Function] 
            ,(select Location from WorkLocationMaster wm where wm.WorkLocationId = em.WorkLocationId)WorkLocation 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.ManagerId) Manager 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.HODId) HOD 
            ,(Case when((select count(1) from PerformanceSetting where PerformanceSettingId=" + PerformanceSettingId + ") = 0) then 'Goals/KRA''s Setting Due' " +
              "when((select count(1) from[dbo].[PerformanceKRAMasterDB] where EmployeeCode = em.EmployeeCode and PerformanceSettingId=" + PerformanceSettingId + ")= 0) then 'Goals / KRAs Acceptance Due'" +
              "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByHOD = 1) = 1) then 'Completed' " +
              "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByManager=1)=1) then 'HOD''s Assessment Due' " +
              "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByEmployee = 1) = 1) then 'Manager''s Assessment Due' " +
              "else 'Self Assessment Due' end) PMSStatus " +
            ",(select top 1 PerformanceEmployeeDataId from PerformanceEmployeeData where EmployeeId = em.EmployeeId)PerformanceEmployeeDataId " +
            ",(select top 1 Published from PerformanceEmployeeData where EmployeeId = em.EmployeeId)Published " +
            "from EmployeeMaster em where InfoFillingStatus=1 and IsActive=1 and UnitId=" + UnitId + " and em.managerId=" + EmployeeId + "";
            List<MISViewList> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<MISViewList>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> GetMISViewHOD(int UnitId, int EmployeeId, int PerformanceSettingId)
    {
        try
        {
            string query = @"
            select EmployeeId
            ,EmployeeCode
            ,EmployeeName
            ,(select JobTitle from Jobtitlemaster jm where em.JobTitleId = jm.JobTitleId)Designation 
            ,(select DepartmentName from DepartmentMaster jm where em.DepartmentId = jm.DepartmentId)Department 
            ,(select RoleName from RoleMaster jm where em.RoleId=jm.RoleId)[Function] 
            ,(select Location from WorkLocationMaster wm where wm.WorkLocationId = em.WorkLocationId)WorkLocation 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.ManagerId) Manager 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.HODId) HOD 
            ,(Case when((select count(1) from PerformanceSetting where PerformanceSettingId=" + PerformanceSettingId + ") = 0) then 'Goals/KRA''s Setting Due' " +
             "when((select count(1) from[dbo].[PerformanceKRAMasterDB] where EmployeeCode = em.EmployeeCode and PerformanceSettingId=" + PerformanceSettingId + ")= 0) then 'Goals / KRAs Acceptance Due'" +
             "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByHOD = 1) = 1) then 'Completed' " +
             "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByManager=1)=1) then 'HOD''s Assessment Due' " +
             "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByEmployee = 1) = 1) then 'Manager''s Assessment Due' " +
             "else 'Self Assessment Due' end) PMSStatus " +
           ",(select top 1 PerformanceEmployeeDataId from PerformanceEmployeeData where EmployeeId = em.EmployeeId)PerformanceEmployeeDataId " +
           ",(select top 1 Published from PerformanceEmployeeData where EmployeeId = em.EmployeeId)Published " +
           "from EmployeeMaster em where InfoFillingStatus=1 and IsActive=1 and UnitId=" + UnitId + " and em.HODId=" + EmployeeId + "";
            List<MISViewList> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<MISViewList>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> GetMISViewClient(int UnitId, int PerformanceSettingId)
    {
        try
        {
            string query = @"
            select EmployeeId
            ,EmployeeCode
            ,EmployeeName
            ,(select JobTitle from Jobtitlemaster jm where em.JobTitleId = jm.JobTitleId)Designation 
            ,(select DepartmentName from DepartmentMaster jm where em.DepartmentId = jm.DepartmentId)Department 
            ,(select RoleName from RoleMaster jm where em.RoleId=jm.RoleId)[Function] 
            ,(select Location from WorkLocationMaster wm where wm.WorkLocationId = em.WorkLocationId)WorkLocation 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.ManagerId) Manager 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.HODId) HOD 
            ,(Case when((select count(1) from PerformanceSetting where PerformanceSettingId=" + PerformanceSettingId + ") = 0) then 'Goals/KRA''s Setting Due' " +
             "when((select count(1) from[dbo].[PerformanceKRAMasterDB] where EmployeeCode = em.EmployeeCode and PerformanceSettingId=" + PerformanceSettingId + ")= 0) then 'Goals / KRAs Acceptance Due'" +
             "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByHOD = 1) = 1) then 'Completed' " +
             "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByManager=1)=1) then 'HOD''s Assessment Due' " +
             "when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and PerformanceSettingId=" + PerformanceSettingId + " and FilledByEmployee = 1) = 1) then 'Manager''s Assessment Due' " +
             "else 'Self Assessment Due' end) PMSStatus " +
           ",(select top 1 PerformanceEmployeeDataId from PerformanceEmployeeData where EmployeeId = em.EmployeeId)PerformanceEmployeeDataId " +
           ",(select top 1 Published from PerformanceEmployeeData where EmployeeId = em.EmployeeId)Published " +
           "from EmployeeMaster em where InfoFillingStatus=1 and IsActive=1 and UnitId=" + UnitId + "";

            List<MISViewList> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<MISViewList>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> GetMISView(int UnitId)
    {
        try
        {
            string query = "select EmployeeId,EmployeeCode,EmployeeName,(select JobTitle from Jobtitlemaster jm where em.JobTitleId = jm.JobTitleId)Designation ,(select DepartmentName from DepartmentMaster jm where em.DepartmentId = jm.DepartmentId)Department ,(select RoleName from RoleMaster jm where em.RoleId=jm.RoleId)[Function] ,(select Location from WorkLocationMaster wm where wm.WorkLocationId = em.WorkLocationId)WorkLocation ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.ManagerId) Manager ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.HODId) HOD ,( Case when((select count(1) from PerformanceSetting where isactive = 1 and ReviewPeriodFrom < getdate() and ReviewPeriodTo > getdate()) = 0) then 'Goals/KRA''s Setting Due' when((select count(1) from[dbo].[PerformanceKRAMasterDB] where EmployeeCode = em.EmployeeCode and PerformanceSettingId = (select PerformanceSettingId from PerformanceSetting where isactive = 1 and ReviewPeriodFrom < getdate() and ReviewPeriodTo > getdate()))= 0) then 'Goals / KRAs Acceptance Due' when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByHOD = 1) = 1) then 'Completed' when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByManager = 1) = 1) then 'HOD''s Assessment Due' when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByEmployee = 1) = 1) then 'Manager''s Assessment Due' else 'Self Assessment Due' end)PMSStatus from EmployeeMaster em where InfoFillingStatus=1 and IsActive=1 and UnitId=" + UnitId + "";
            List<MISViewListViewModel> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<MISViewListViewModel>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> PerformanceReviewFillPMS(int EmployeeId)
    {
        try
        {
            //string query = @"select count(1) from employeemaster em join [PerformanceSetting] ps on em.UnitID = ps.UnitId join [PerformanceKRAMasterDB] pkmdb on em.employeecode = pkmdb.employeecode where em.employeeid = " + EmployeeId + " and ps.isactive = 1 and getdate() between ps.ReviewPeriodFrom and ps.ReviewPeriodTo";
            string query = @"select count(1) from employeemaster em join [PerformanceSetting] ps on em.UnitID = ps.UnitId join [PerformanceKRAMasterDB] pkmdb on em.employeecode = pkmdb.employeecode where em.employeeid = " + EmployeeId + " and ps.isactive = 1 AND CAST(GETDATE() AS DATE) BETWEEN CAST(ps.ReviewPeriodFrom AS DATE) AND CAST(ps.ReviewPeriodTo AS DATE)";
            List<int> res = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<int>(query);
            return Ok(res.Count > 0 ? res[0] == 0 ? false : true : false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPMSReport(int UnitId, int PerformanceSettingId)
    {
        try
        {
            string query = @"select em.EmployeeId,ped.PerformanceEmployeeDataId,ps.PerformanceSettingId,em.EmployeeCode,em.EmployeeName
            ,(select JobTitle from Jobtitlemaster jm where em.JobTitleId = jm.JobTitleId)Designation ,(select DepartmentName from DepartmentMaster jm where em.DepartmentId = jm.DepartmentId)Department 
            ,(select RoleName from RoleMaster jm where em.RoleId=jm.RoleId)[Function] ,(select Location from WorkLocationMaster wm where wm.WorkLocationId = em.WorkLocationId)WorkLocation 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.ManagerId) Manager ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.HODId) HOD
            ,(Case 
                when((select count(1) from PerformanceSetting where isactive = 1 and PerformanceSettingId=ps.PerformanceSettingId) = 0) then 'Goals/KRA''s Setting Due' 
                when((select count(1) from[dbo].[PerformanceKRAMasterDB] where EmployeeCode = em.EmployeeCode and PerformanceSettingId = (select PerformanceSettingId from PerformanceSetting where isactive = 1 and PerformanceSettingId=ps.PerformanceSettingId))= 0) then 'Goals / KRAs Acceptance Due' 
                when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByHOD = 1) = 1) then 'Completed' 
                when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByManager = 1) = 1) then 'HOD''s Assessment Due' 
                when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByEmployee = 1) = 1) then 'Manager''s Assessment Due' 
                else 'Self Assessment Due' end)PMSStatus
            ,ped.RatingCalculationKRAWeightage KRAWeightage 
            ,ped.RatingCalculationKRAScore KRARating
            ,ped.RatingCalculationKRAFinalScore FinalKRARating
            ,ped.RatingCalculationBehaviouralSkillsWeightage BehaviouralWeightage 
            ,ped.RatingCalculationBehaviouralSkillsScore BehaviouralRating
            ,ped.RatingCalculationBehaviouralSkillsFinalScore FinalBehaviouralRating
            ,ped.RatingCalculationFinalScore FinalScore
            ,ped.RatingCalculationFinalRating FinalRatingManager
            ,ped.HODFinalRating FinalRatingHOD
            ,ped.ClosingRemarksEmployee ClosingRemarksEmployee
            ,ped.ClosingRemarksManager ClosingRemarksManager
            ,ped.ClosingRemarksHOD ClosingRemarksHOD 
            from EmployeeMaster em 
            join [dbo].[PerformanceEmployeeData] ped on em.EmployeeId=ped.EmployeeId
            join PerformanceSetting ps on ped.PerformanceSettingId=ps.PerformanceSettingId
            where InfoFillingStatus=1 and em.IsActive=1 and em.UnitId=" + UnitId + " and ps.isactive = 1 and ps.PerformanceSettingId=" + PerformanceSettingId + "";
            List<PMSReportViewModel> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<PMSReportViewModel>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> GetPMSReportByPerformanceSettingId(int UnitId, int PerformanceSettingId)
    {
        try
        {
            string query = @"select em.EmployeeId,ped.PerformanceEmployeeDataId,ps.PerformanceSettingId,em.EmployeeCode,em.EmployeeName
            ,(select JobTitle from Jobtitlemaster jm where em.JobTitleId = jm.JobTitleId)Designation ,(select DepartmentName from DepartmentMaster jm where em.DepartmentId = jm.DepartmentId)Department 
            ,(select RoleName from RoleMaster jm where em.RoleId=jm.RoleId)[Function] ,(select Location from WorkLocationMaster wm where wm.WorkLocationId = em.WorkLocationId)WorkLocation 
            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.ManagerId) Manager ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.HODId) HOD
            ,(Case 
                when((select count(1) from PerformanceSetting where isactive = 1 and ReviewPeriodFrom < getdate() and ReviewPeriodTo > getdate()) = 0) then 'Goals/KRA''s Setting Due' 
                when((select count(1) from[dbo].[PerformanceKRAMasterDB] where EmployeeCode = em.EmployeeCode and PerformanceSettingId = (select PerformanceSettingId from PerformanceSetting where isactive = 1 and ReviewPeriodFrom < getdate() and ReviewPeriodTo > getdate()))= 0) then 'Goals / KRAs Acceptance Due' 
                when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByHOD = 1) = 1) then 'Completed' 
                when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByManager = 1) = 1) then 'HOD''s Assessment Due' 
                when((select count(1) from PerformanceEmployeeData where EmployeeId = em.EmployeeId and FilledByEmployee = 1) = 1) then 'Manager''s Assessment Due' 
                else 'Self Assessment Due' end)PMSStatus
            ,ped.RatingCalculationKRAWeightage KRAWeightage 
            ,ped.RatingCalculationKRAScore KRARating
            ,ped.RatingCalculationKRAFinalScore FinalKRARating
            ,ped.RatingCalculationBehaviouralSkillsWeightage BehaviouralWeightage 
            ,ped.RatingCalculationBehaviouralSkillsScore BehaviouralRating
            ,ped.RatingCalculationBehaviouralSkillsFinalScore FinalBehaviouralRating
            ,ped.RatingCalculationFinalScore FinalScore
            ,ped.RatingCalculationFinalRating FinalRatingManager
            ,ped.HODFinalRating FinalRatingHOD
            ,ped.ClosingRemarksEmployee ClosingRemarksEmployee
            ,ped.ClosingRemarksManager ClosingRemarksManager
            ,ped.ClosingRemarksHOD ClosingRemarksHOD 
            from EmployeeMaster em 
            join [dbo].[PerformanceEmployeeData] ped on em.EmployeeId=ped.EmployeeId
            join PerformanceSetting ps on ped.PerformanceSettingId=ps.PerformanceSettingId
            where InfoFillingStatus=1 and em.IsActive=1 and em.UnitId=" + UnitId + " and ps.isactive = 1 and ps.PerformanceSettingId=" + PerformanceSettingId + "";
            List<PMSReportViewModel> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<PMSReportViewModel>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> GetTrainingNeedReport(int UnitId, int PerformanceSettingId)
    {
        try
        {
            string query = @"select em.EmployeeId,ped.PerformanceEmployeeDataId,ps.PerformanceSettingId,em.EmployeeCode,em.EmployeeName
                            ,(select JobTitle from Jobtitlemaster jm where em.JobTitleId = jm.JobTitleId)Designation 
                            ,(select DepartmentName from DepartmentMaster jm where em.DepartmentId = jm.DepartmentId)Department 
                            ,(select RoleName from RoleMaster jm where em.RoleId=jm.RoleId)[Function] 
                            ,(select Location from WorkLocationMaster wm where wm.WorkLocationId = em.WorkLocationId)WorkLocation 
                            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.ManagerId) Manager 
                            ,(Select EmployeeName from EmployeeMaster em1 where em1.EmployeeId = em.HODId) HOD
                            ,(select Training from [dbo].[PerformanceTrainingNeedsMaster] ptnm where ptnm.IsActive=1 and ptnm.TrainingNeedsMasterId=petd.TrainingNeedsMasterId)TrainingNeed
                            ,(case when petd.TrainingType=1 then 'Functional' when petd.TrainingType=2 then 'Behavioural' else '' end)TrainingType
                            ,(case when petd.TrainingUrgency=1 then 'Urgent' when petd.TrainingUrgency=2 then 'Important' else '' end)Urgency
                            from EmployeeMaster em 
                            join [dbo].[PerformanceEmployeeData] ped on em.EmployeeId=ped.EmployeeId
                            join PerformanceSetting ps on ped.PerformanceSettingId=ps.PerformanceSettingId
                            join [dbo].[PerformanceEmployeeTrainingData] petd on ped.PerformanceEmployeeDataId=petd.PerformanceEmployeeDataId
                            where InfoFillingStatus=1 and em.IsActive=1 and em.UnitId=" + UnitId + " and ps.isactive = 1 and ps.PerformanceSettingId=" + PerformanceSettingId + " and petd.TrainingType != 0";
            List<TrainingNeedReportViewModel> dto = await _unitOfWork.PerformanceKRAMasterDB.GetTableData<TrainingNeedReportViewModel>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> PublishPerformacne(int PerformanceEmployeeDataId)
    {
        try
        {
            PerformanceEmployeeData dto = await _unitOfWork.PerformanceEmployeeData.FindByIdAsync(PerformanceEmployeeDataId);
            if (dto != null)
            {
                dto.Published = true;
                await _unitOfWork.PerformanceEmployeeData.UpdateAsync(dto);
                return Ok("Success");
            }
            return BadRequest("Error occurred while publishing the performace");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetMISView)}");
            throw;
        }
    }
    public async Task<IActionResult> DeletePMSSetting(PerformanceSettingViewModel inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                PerformanceSetting res = await _unitOfWork.PerformanceSetting.FindByIdAsync(inputDTO.PerformanceSettingId ?? default(int));
                res.IsActive = false;
                bool isSuccess = await _unitOfWork.PerformanceSetting.UpdateAsync(res);
                if (isSuccess)
                {
                    return Ok("Deleted");
                }
                else
                {
                    return BadRequest("Error");
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(DeletePMSSetting)}");
            throw;
        }
    }
}
