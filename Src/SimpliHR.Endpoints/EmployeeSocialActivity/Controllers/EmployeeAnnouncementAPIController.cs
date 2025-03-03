using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.Exit;
using SimpliHR.Infrastructure.Models.Master;

namespace SimpliHR.Endpoints.EmployeeSocialActivity;

[Route("api/[controller]")]
[ApiController]
public class EmployeeAnnouncementAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeAnnouncementAPIController> _logger;
    private readonly IMapper _mapper;

    public EmployeeAnnouncementAPIController(IUnitOfWork unitOfWork, ILogger<EmployeeAnnouncementAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> SaveEmployeeAnnouncement(EmployeeAnnouncementDTO inputDTO)
    {
        try
        {
            //int insertedId = await _unitOfWork.PerformanceSetting.AddAsync(_mapper.Map<PerformanceSetting>(inputDTO.PerformanceSetting));
            if (inputDTO != null)
            {
                if (inputDTO.EmployeeAnnouncementId > 0)
                {
                    EmployeeAnnouncement ea = await _unitOfWork.EmployeeAnnouncement.FindByIdAsync(inputDTO.EmployeeAnnouncementId);
                    inputDTO.CreatedDate = ea.CreatedDate;
                    inputDTO.CreatedBy = ea.CreatedBy;
                    inputDTO.UnitId = ea.UnitId;
                    inputDTO.IsActive = ea.IsActive;
                    await _unitOfWork.EmployeeAnnouncement.UpdateAsync(_mapper.Map<EmployeeAnnouncement>(inputDTO));
                    _unitOfWork.Save();
                    EmployeeAnnouncementDTO ead = _mapper.Map<EmployeeAnnouncementDTO>(ea);
                    return Ok(ead);
                }
                else
                {
                    int insertedId = await _unitOfWork.EmployeeAnnouncement.AddAsync(_mapper.Map<EmployeeAnnouncement>(inputDTO));
                    inputDTO.EmployeeAnnouncementId = insertedId;
                    _unitOfWork.Save();
                    return Ok(inputDTO);
                }

            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeAnnouncement)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveSurveyPoll(EmployeeAnnouncementViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                List<SurveyPollViewModel>? surveyPollViewModel = inputDTO.SurveyPolls;
                if (surveyPollViewModel != null && surveyPollViewModel.Any())
                {
                    foreach (var item in surveyPollViewModel)
                    {
                        if (item != null)
                        {
                            if (item.PollQuestion != null)
                            {
                                if (item.PollQuestion.SurveyPollQuestionId > 0)
                                {
                                    await _unitOfWork.SurveyPollsQuestion.UpdateAsync(_mapper.Map<SurveyPollsQuestion>(item.PollQuestion));
                                }
                                else
                                {
                                    item.PollQuestion.EmployeeAnnouncementId = inputDTO.employeeAnnouncementDTO.EmployeeAnnouncementId;
                                    item.PollQuestion.SurveyPollQuestionId = await _unitOfWork.SurveyPollsQuestion.AddAsync(_mapper.Map<SurveyPollsQuestion>(item.PollQuestion));
                                }

                                List<SurveyPollOptionDTO>? options = item.PollOptions;
                                if (options != null && options.Any())
                                {
                                    foreach (var option in options)
                                    {
                                        if (option.SurveyPollOptionId > 0)
                                        {
                                            await _unitOfWork.SurveyPollOption.UpdateAsync(_mapper.Map<SurveyPollOption>(option));
                                        }
                                        else
                                        {
                                            option.QuestionId = item.PollQuestion.SurveyPollQuestionId;
                                            await _unitOfWork.SurveyPollOption.AddAsync(_mapper.Map<SurveyPollOption>(option));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return Ok("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveSurveyPoll)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveEmployeeAnnouncementFile(EmployeeAnnouncementFileUploadDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.UploadType != null && inputDTO.UploadType.Trim().ToUpper() == "UPLOAD")
                {
                    var employeeAnnouncementFile = await _unitOfWork.EmployeeAnnouncementFileUpload.GetFilter(x => x.UploadType == inputDTO.UploadType && x.IsActive == true && x.EmployeeAnnouncementId == inputDTO.EmployeeAnnouncementId);
                    if (employeeAnnouncementFile != null)
                    {
                        inputDTO.EmployeeAnnouncementFileUploadsId = employeeAnnouncementFile.EmployeeAnnouncementFileUploadsId;
                        await _unitOfWork.EmployeeAnnouncementFileUpload.UpdateAsync(_mapper.Map<EmployeeAnnouncementFileUpload>(inputDTO));
                        _unitOfWork.Save();
                        return Ok(inputDTO);
                        //return StatusCode(StatusCodes.Status302Found, inputDTO.UploadedFileName + " has already been uploaded in " + inputDTO.UploadType);
                    }
                    else
                    {
                        int insertedId = await _unitOfWork.EmployeeAnnouncementFileUpload.AddAsync(_mapper.Map<EmployeeAnnouncementFileUpload>(inputDTO));
                        inputDTO.EmployeeAnnouncementFileUploadsId = insertedId;
                        _unitOfWork.Save();
                        return Ok(inputDTO);
                    }
                }
                else
                {
                    var employeeAnnouncementFile = await _unitOfWork.EmployeeAnnouncementFileUpload.GetFilter(x => x.UploadType == inputDTO.UploadType && x.IsActive == true && x.EmployeeAnnouncementId == inputDTO.EmployeeAnnouncementId && x.UploadedFileName == inputDTO.UploadedFileName);
                    if (employeeAnnouncementFile != null)
                    {
                        return StatusCode(StatusCodes.Status302Found, inputDTO.UploadedFileName + " has already been uploaded in " + inputDTO.UploadType);
                    }
                    else
                    {
                        int insertedId = await _unitOfWork.EmployeeAnnouncementFileUpload.AddAsync(_mapper.Map<EmployeeAnnouncementFileUpload>(inputDTO));
                        inputDTO.EmployeeAnnouncementFileUploadsId = insertedId;
                        _unitOfWork.Save();
                        return Ok(inputDTO);
                    }
                }


            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeAnnouncementFile)}");
            throw;
        }
    }
    public async Task<IActionResult> HardDeleteEmployeeAnnouncement(int EmployeeAnnouncementId, List<int> FileUploadIds)
    {
        try
        {
            await _unitOfWork.EmployeeAnnouncement.DeleteAsync(EmployeeAnnouncementId);
            foreach (int FileUploadId in FileUploadIds)
            {
                await _unitOfWork.EmployeeAnnouncementFileUpload.DeleteAsync(FileUploadId);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeAnnouncementFile)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeAnnouncementList(int UnitId)
    {
        try
        {
            string query = $"select EmployeeAnnouncementId,UnitId,AnnouncementType AnnouncementTypeId,(Select atm.AnnouncementType from AnnouncementTypeMaster atm where atm.AnnouncementTypeId=ea.AnnouncementType)AnnouncementType,Title,Description,Poll,Survey,Departments ,DepartmentNames=STUFF((SELECT ', ' + DepartmentName FROM DepartmentMaster b  WHERE b.DepartmentId in (Select value from string_split(ea.Departments,',') ) FOR XML PATH('')), 1, 2, '')  ,KeywordsRaw,Keywords,PublishDate,PublishTime,(case when (Select count(1) from SurveyPollsQuestion spq where IsActive=1 and spq.EmployeeAnnouncementId=ea.EmployeeAnnouncementId)>0 then 1 else 0 end)Poll from EmployeeAnnouncement ea  where UnitId={UnitId} and isactive=1";

            List<EmployeeAnnouncementWithChild> dto = await _unitOfWork.EmployeeAnnouncement.GetTableData<EmployeeAnnouncementWithChild>(query);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }



    public async Task<IActionResult> EmployeeAnnouncementListForDashboard(int UnitId, int DepartmentId, int AnnouncementTypeId)
    {
        try
        {
            DateTime currDate = System.DateTime.Now.Date;
            TimeSpan currTime = System.DateTime.Now.TimeOfDay;
            string filter = "";
            if (AnnouncementTypeId > 0)
            {
                filter = $"and AnnouncementType={AnnouncementTypeId}";
            }
            string query = $"select Top 3 EmployeeAnnouncementId,UnitId,AnnouncementType,Title,Description,KeywordsRaw,Keywords,PublishDate,PublishTime ,(select Top 1 UploadedFilePath+'/'+UploadedFileName from [dbo].[EmployeeAnnouncementFileUploads] enfu where enfu.EmployeeAnnouncementId=ea.EmployeeAnnouncementId and enfu.IsActive=1 and enfu.UploadType='Upload') ImagePath from EmployeeAnnouncement ea  where UnitId={UnitId} and isactive=1 and {DepartmentId} in (Select Value from string_split(ea.Departments,',')) and (CONCAT(PublishDate,' ',PublishTime) <= convert(date,'{currDate.ToString("yyyy-MM-dd")} {currTime}')) {filter} order by PublishDate desc";

            List<EmployeeAnnouncementDTO> dto = await _unitOfWork.EmployeeAnnouncement.GetTableData<EmployeeAnnouncementDTO>(query);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeAnnouncementById(int EmployeeAnnouncementId)
    {
        try
        {
            EmployeeAnnouncementDTO dto = _mapper.Map<EmployeeAnnouncementDTO>(await _unitOfWork.EmployeeAnnouncement.FindByIdAsync(EmployeeAnnouncementId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeAnnouncementFilesByAnnouncementId(int EmployeeAnnouncementId)
    {
        try
        {
            List<EmployeeAnnouncementFileUploadDTO> dto = _mapper.Map<List<EmployeeAnnouncementFileUploadDTO>>(await _unitOfWork.EmployeeAnnouncementFileUpload.GetFilterAll(x => x.IsActive == true && x.EmployeeAnnouncementId == EmployeeAnnouncementId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeAnnouncementFilesByFileId(int EmployeeAnnouncementFileUploadsId)
    {
        try
        {

            EmployeeAnnouncementFileUploadDTO dto = _mapper.Map<EmployeeAnnouncementFileUploadDTO>(await _unitOfWork.EmployeeAnnouncementFileUpload.FindByIdAsync(EmployeeAnnouncementFileUploadsId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }
    public async Task<IActionResult> SurveyPollQuestions(int EmployeeAnnouncementId, int? EmployeeID = null)
    {
        try
        {
            //List<SurveyPollsQuestionDTO> dto1 = _mapper.Map<List<SurveyPollsQuestionDTO>>(await _unitOfWork.SurveyPollsQuestion.GetFilterAll(x => x.IsActive == true && x.EmployeeAnnouncementId == EmployeeAnnouncementId));
            string questionQuery = $"Select (Select count(1) from PollResponses where QuestionId=spq.SurveyPollQuestionId)TotalVotes,* from SurveyPollsQuestion spq where IsActive=1 and EmployeeAnnouncementId={EmployeeAnnouncementId}";
            List<SurveyPollsQuestionDTO> dto = _mapper.Map<List<SurveyPollsQuestionDTO>>(await _unitOfWork.SurveyPollsQuestion.GetTableData<SurveyPollsQuestionDTO>(questionQuery));

            foreach (var item in dto)
            {
                var r = await _unitOfWork.PollResponse.GetFilter(x => x.EmployeeId == EmployeeID && x.QuestionId == item.SurveyPollQuestionId);
                if (r != null)
                {
                    item.Response = r.OptionId;
                }

            }

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }

    public async Task<IActionResult> SurveyPollOptions(int QuestionId)
    {
        try
        {
            List<SurveyPollOptionDTO> dto = _mapper.Map<List<SurveyPollOptionDTO>>(await _unitOfWork.SurveyPollOption.GetFilterAll(x => x.IsActive == true && x.QuestionId == QuestionId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }
    public async Task<IActionResult> SurveyPollResponses(int QuestionId)
    {
        try
        {
            List<PollResponseDTO> dto = _mapper.Map<List<PollResponseDTO>>(await _unitOfWork.PollResponse.GetFilterAll(x => x.QuestionId == QuestionId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteEmployeeAnnouncementFilesById(int EmployeeAnnouncementFileUploadsId)
    {
        try
        {



            EmployeeAnnouncementFileUpload employeeAnnouncementFileUpload = await _unitOfWork.EmployeeAnnouncementFileUpload.FindByIdAsync(EmployeeAnnouncementFileUploadsId);
            if (employeeAnnouncementFileUpload != null)
            {
                employeeAnnouncementFileUpload.IsActive = false;
                await _unitOfWork.EmployeeAnnouncementFileUpload.UpdateAsync(employeeAnnouncementFileUpload);
                _unitOfWork.Save();
                return Ok("Save");
            }
            else
            {
                return BadRequest("File not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteSurveyPollOption(int SurveyPollOptionId)
    {
        try
        {
            SurveyPollOption res = await _unitOfWork.SurveyPollOption.FindByIdAsync(SurveyPollOptionId);
            if (res != null)
            {
                res.IsActive = false;
                await _unitOfWork.SurveyPollOption.UpdateAsync(res);
                _unitOfWork.Save();
                return Ok("Save");
            }
            else
            {
                return BadRequest("File not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementList)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteSurveyPollQuestion(int SurveyPollQuestionId)
    {
        try
        {
            SurveyPollsQuestion res = await _unitOfWork.SurveyPollsQuestion.FindByIdAsync(SurveyPollQuestionId);
            if (res != null)
            {
                res.IsActive = false;
                await _unitOfWork.SurveyPollsQuestion.UpdateAsync(res);
                _unitOfWork.Save();
                return Ok("Save");
            }
            else
            {
                return BadRequest("File not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteSurveyPollQuestion)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteEmployeeAnnouncementById(int EmployeeAnnouncementId)
    {
        try
        {
            EmployeeAnnouncement employeeAnnouncement = await _unitOfWork.EmployeeAnnouncement.FindByIdAsync(EmployeeAnnouncementId);
            if (employeeAnnouncement != null)
            {
                employeeAnnouncement.IsActive = false;
                await _unitOfWork.EmployeeAnnouncement.UpdateAsync(employeeAnnouncement);
                return Ok();
            }
            else
            {
                return BadRequest("Unable to fetch the record");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteEmployeeAnnouncementById)}");
            throw;
        }
    }

    public async Task<IActionResult> EmployeeAnnouncementDetailsById(int EmployeeAnnouncementId, int DepartmentId, int UnitId)
    {
        try
        {
            DateTime currDate = System.DateTime.Now.Date;
            TimeSpan currTime = System.DateTime.Now.TimeOfDay;
            string filter = "";
            if (EmployeeAnnouncementId > 0)
            {
                filter = $"and EmployeeAnnouncementId={EmployeeAnnouncementId}";
            }
            string query = $"select EmployeeAnnouncementId,UnitId,AnnouncementType AnnouncementTypeId,(Select atm.AnnouncementType from AnnouncementTypeMaster atm where atm.AnnouncementTypeId=ea.AnnouncementType)AnnouncementType,Title,Description,KeywordsRaw,Keywords,PublishDate,PublishTime ,(select Top 1 UploadedFilePath+'/'+UploadedFileName from [dbo].[EmployeeAnnouncementFileUploads] enfu where enfu.EmployeeAnnouncementId=ea.EmployeeAnnouncementId and enfu.IsActive=1 and enfu.UploadType='Upload') ImagePath from EmployeeAnnouncement ea  where UnitId={UnitId} and isactive=1 and {DepartmentId} in (Select Value from string_split(ea.Departments,',')) and (CONCAT(PublishDate,' ',PublishTime) <= convert(date,'{currDate.ToString("yyyy-MM-dd")} {currTime}')) {filter} order by PublishDate desc";


            List<EmployeeAnnouncementWithChild> dto = await _unitOfWork.EmployeeNews.GetTableData<EmployeeAnnouncementWithChild>(query);

            if (dto != null && dto.Count() > 0)
            {
                return Ok(dto.FirstOrDefault());
            }
            else
            {
                return BadRequest("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementDetailsById)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeAnnouncementListForViewAll(int UnitId, int DepartmentId, EmployeeAnnouncementViewModel inputDTO)
    {
        try
        {
            string pageDetails = "";
            if (inputDTO != null && inputDTO.PageDetails != null && inputDTO.PageDetails.PageNumber != null && inputDTO.PageDetails.PageSize != null)
            {
                EmployeeAnnouncementPageDetails pd = inputDTO.PageDetails;
                pageDetails = $"OFFSET {(pd.PageNumber - 1) * pd.PageSize} ROWS FETCH NEXT {pd.PageSize} ROWS ONLY";
            }
            DateTime currDate = System.DateTime.Now.Date;
            TimeSpan currTime = System.DateTime.Now.TimeOfDay;
            if (String.IsNullOrEmpty(inputDTO.AnnouncementSearchKeyword))
            {
                inputDTO.AnnouncementSearchKeyword = "";
            }
            string filter = "";

            if (inputDTO.AnnouncementTypeId > 0)
            {
                filter = $" and AnnouncementType={inputDTO.AnnouncementTypeId} ";
            }
            filter += $" and (Title like '%{inputDTO.AnnouncementSearchKeyword}%' or Keywords like '%{inputDTO.AnnouncementSearchKeyword}%')";
            string query = $"select EmployeeAnnouncementId,UnitId,AnnouncementType AnnouncementTypeId,(Select atm.AnnouncementType from AnnouncementTypeMaster atm where atm.AnnouncementTypeId=ea.AnnouncementType)AnnouncementType,Title,Description,KeywordsRaw,Keywords,PublishDate,PublishTime ,(select Top 1 UploadedFilePath+'/'+UploadedFileName from [dbo].[EmployeeAnnouncementFileUploads] enfu where enfu.EmployeeAnnouncementId=ea.EmployeeAnnouncementId and enfu.IsActive=1 and enfu.UploadType='Upload') ImagePath from EmployeeAnnouncement ea  where UnitId={UnitId} and isactive=1 and {DepartmentId} in (Select Value from string_split(ea.Departments,',')) and (CONCAT(PublishDate,' ',PublishTime) <= convert(date,'{currDate.ToString("yyyy-MM-dd")} {currTime}')) {filter} order by PublishDate desc {pageDetails} ;";

            List<EmployeeAnnouncementWithChild> dto = await _unitOfWork.EmployeeNews.GetTableData<EmployeeAnnouncementWithChild>(query);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementListForViewAll)}");
            throw;
        }
    }

    public async Task<int> EmployeeAnnouncementListCountViewAll(int UnitId, int DepartmentId, EmployeeAnnouncementViewModel inputDTO)
    {
        try
        {
            DateTime currDate = System.DateTime.Now.Date;
            TimeSpan currTime = System.DateTime.Now.TimeOfDay;
            if (String.IsNullOrEmpty(inputDTO.AnnouncementSearchKeyword))
            {
                inputDTO.AnnouncementSearchKeyword = "";
            }
            string filter = "";

            if (inputDTO.AnnouncementTypeId > 0)
            {
                filter = $" and AnnouncementType={inputDTO.AnnouncementTypeId} ";
            }
            filter += $" and (Title like '%{inputDTO.AnnouncementSearchKeyword}%' or Keywords like '%{inputDTO.AnnouncementSearchKeyword}%')";
            string query = $"select count(1) from EmployeeAnnouncement ea  where UnitId={UnitId} and isactive=1 and {DepartmentId} in (Select Value from string_split(ea.Departments,',')) and (CONCAT(PublishDate,' ',PublishTime) <= convert(date,'{currDate.ToString("yyyy-MM-dd")} {currTime}')) {filter} ;";

            List<int> dto = await _unitOfWork.EmployeeNews.GetTableData<int>(query);

            if (dto != null && dto.Count > 0)
            {
                return dto.FirstOrDefault();
            }
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeAnnouncementListForViewAll)}");
            throw;
        }
    }
    public async Task<IActionResult> AnnouncementTypesForDashboard(int UnitId)
    {
        try
        {
            string query = $"Select * from AnnouncementTypeMaster where AnnouncementTypeId in (Select Distinct(AnnouncementType) from employeeAnnouncement where AnnouncementType is not null and IsActive=1 and UnitId={UnitId}) and IsACtive=1 and unitId={UnitId}";
            List<AnnouncementTypeMasterDTO> dto = await _unitOfWork.EmployeeNews.GetTableData<AnnouncementTypeMasterDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AnnouncementTypesForDashboard)}");
            throw;
        }
    }
    public async Task<IActionResult> SavePollResponse(PollResponseDTO inputDTO)
    {
        try
        {

            if (inputDTO == null)
            {
                return BadRequest("Invalid Data");
            }
            var res = await _unitOfWork.PollResponse.GetFilter(x => x.QuestionId == inputDTO.QuestionId && x.EmployeeId == inputDTO.EmployeeId);
            if (res == null)
            {
                int inserted = await _unitOfWork.PollResponse.AddAsync(_mapper.Map<PollResponse>(inputDTO));
                if (inserted != 0)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest("Error while saving data");
                }
            }
            else
            {
                await _unitOfWork.PollResponse.UpdateAsync(_mapper.Map<PollResponse>(inputDTO));
                return Ok("");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SavePollResponse)}");
            throw;
        }
    }

}
