using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpliHR.Core.Entities;
using SimpliHR.Infrastructure.Models.EmployeeSocialActivity;
using SimpliHR.Infrastructure.Models.Master;

namespace SimpliHR.Endpoints.EmployeeSocialActivity;

[Route("api/[controller]")]
[ApiController]
public class EmployeeNewsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeNewsAPIController> _logger;
    private readonly IMapper _mapper;
    public EmployeeNewsAPIController(IUnitOfWork unitOfWork, ILogger<EmployeeNewsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> SaveEmployeeNews(EmployeeNewsDTO inputDTO)
    {
        try
        {
            //int insertedId = await _unitOfWork.PerformanceSetting.AddAsync(_mapper.Map<PerformanceSetting>(inputDTO.PerformanceSetting));
            if (inputDTO != null)
            {
                if (inputDTO.EmployeeNewsId > 0)
                {
                    EmployeeNews ea = await _unitOfWork.EmployeeNews.FindByIdAsync(inputDTO.EmployeeNewsId);
                    inputDTO.CreatedDate = ea.CreatedDate;
                    inputDTO.CreatedBy = ea.CreatedBy;
                    inputDTO.UnitId = ea.UnitId;
                    inputDTO.IsActive = ea.IsActive;
                    var res = await _unitOfWork.EmployeeNews.UpdateAsync(_mapper.Map<EmployeeNews>(inputDTO));
                    _unitOfWork.Save();
                    if (res == false)
                    {
                        return BadRequest("Unable to update data");
                    }
                    EmployeeNewsDTO ead = _mapper.Map<EmployeeNewsDTO>(ea);
                    return Ok(ead);
                }
                else
                {
                    int insertedId = await _unitOfWork.EmployeeNews.AddAsync(_mapper.Map<EmployeeNews>(inputDTO));
                    inputDTO.EmployeeNewsId = insertedId;
                    _unitOfWork.Save();
                    return Ok(inputDTO);
                }

            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeNews)}");
            throw;
        }
    }
    public async Task<IActionResult> SaveEmployeeNewsFile(EmployeeNewsFileUploadDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var employeeNewsFile = await _unitOfWork.EmployeeNewsFileUpload.GetFilter(x => x.UploadType == inputDTO.UploadType && x.IsActive == true && x.EmployeeNewsId == inputDTO.EmployeeNewsId);
                if (employeeNewsFile != null)
                {
                    inputDTO.EmployeeNewsFileUploadsId = employeeNewsFile.EmployeeNewsFileUploadsId;
                    await _unitOfWork.EmployeeNewsFileUpload.UpdateAsync(_mapper.Map<EmployeeNewsFileUpload>(inputDTO));
                    //return StatusCode(StatusCodes.Status302Found, inputDTO.UploadedFileName + " has already been uploaded in " + inputDTO.UploadType);
                    _unitOfWork.Save();
                    return Ok(inputDTO);
                }
                else
                {
                    int insertedId = await _unitOfWork.EmployeeNewsFileUpload.AddAsync(_mapper.Map<EmployeeNewsFileUpload>(inputDTO));
                    inputDTO.EmployeeNewsFileUploadsId = insertedId;
                    _unitOfWork.Save();
                    return Ok(inputDTO);
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeNewsFile)}");
            throw;
        }
    }
    public async Task<IActionResult> HardDeleteEmployeeNews(int EmployeeNewsId, List<int> FileUploadIds)
    {
        try
        {
            await _unitOfWork.EmployeeNews.DeleteAsync(EmployeeNewsId);
            foreach (int FileUploadId in FileUploadIds)
            {
                await _unitOfWork.EmployeeNewsFileUpload.DeleteAsync(FileUploadId);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveEmployeeNewsFile)}");
            throw;
        }
    }

    public async Task<IActionResult> EmployeeNewsDetailsById(int EmployeeNewsId)
    {
        try
        {
            string query = $"select EmployeeNewsId,UnitId,Title,ArticleType,Article,ArticleLink,Description,AuthorsNameRaw,AuthorsName,PublicationName,Tagging,CategoryTags=STUFF((SELECT ', ' + b.NewsCategoryTag FROM NewsCategoryTagMaster b  WHERE b.NewsCategoryTagId in (Select value from string_split(ea.Tagging,',') ) FOR XML PATH('')), 1, 2, ''),KeywordsRaw,Keywords,IsActive,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,(select Top 1 UploadedFilePath+'/'+UploadedFileName from [dbo].[EmployeeNewsFileUploads] enfu where enfu.EmployeeNewsId=ea.EmployeeNewsId and enfu.IsActive=1 ) ImagePath from EmployeeNews ea where EmployeeNewsId={EmployeeNewsId}";

            List<EmployeeNewsDTO> dto = await _unitOfWork.EmployeeNews.GetTableData<EmployeeNewsDTO>(query);

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
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeNewsList(int UnitId)
    {
        try
        {
            string query = $"select EmployeeNewsId,UnitId,Title,ArticleType,Article,ArticleLink,Description,AuthorsNameRaw,AuthorsName,PublicationName,Tagging,CategoryTags=STUFF((SELECT ', ' + b.NewsCategoryTag FROM NewsCategoryTagMaster b  WHERE b.NewsCategoryTagId in (Select value from string_split(ea.Tagging,',') ) FOR XML PATH('')), 1, 2, ''),KeywordsRaw,Keywords,IsActive,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,(select Top 1 UploadedFilePath+'/'+UploadedFileName from [dbo].[EmployeeNewsFileUploads] enfu where enfu.EmployeeNewsId=ea.EmployeeNewsId and enfu.IsActive=1 ) ImagePath from EmployeeNews ea  where UnitId={UnitId} and isactive=1 order by EmployeeNewsId desc";

            List<EmployeeNewsDTO> dto = await _unitOfWork.EmployeeNews.GetTableData<EmployeeNewsDTO>(query);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeNewsListForDashboard(int UnitId, int NewsCategoryTagId)
    {
        try
        {
            string filter = "";
            if (NewsCategoryTagId > 0)
            {
                filter = $"and {NewsCategoryTagId} in (Select Value from string_split(Tagging,','))";
            }
            string query = $"select Top 3 EmployeeNewsId,UnitId,Title,ArticleType,Article,ArticleLink,Description,AuthorsNameRaw,AuthorsName,PublicationName,TaggingRaw,Tagging,KeywordsRaw,Keywords,IsActive,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,(select Top 1 UploadedFilePath+'/'+UploadedFileName from [dbo].[EmployeeNewsFileUploads] enfu where enfu.EmployeeNewsId=ea.EmployeeNewsId and enfu.IsActive=1 ) ImagePath from EmployeeNews ea  where UnitId={UnitId} and isactive=1 {filter} order by EmployeeNewsId desc";

            List<EmployeeNewsDTO> dto = await _unitOfWork.EmployeeNews.GetTableData<EmployeeNewsDTO>(query);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeNewsListForViewAll(int UnitId, EmployeeNewsViewModel inputDTO)
    {
        try
        {
            string pageDetails = "";
            if (inputDTO != null && inputDTO.PageDetails != null && inputDTO.PageDetails.PageNumber != null && inputDTO.PageDetails.PageSize != null)
            {
                EmployeeNewsPageDetails pd = inputDTO.PageDetails;
                pageDetails = $"OFFSET {(pd.PageNumber - 1) * pd.PageSize} ROWS FETCH NEXT {pd.PageSize} ROWS ONLY";
            }
            if (String.IsNullOrEmpty(inputDTO.NewsSearchKeyword))
            {
                inputDTO.NewsSearchKeyword = "";
            }
            string filter = "";
            if (inputDTO.NewsCategoryId > 0)
            {
                filter = $" and {inputDTO.NewsCategoryId} in (Select Value from string_split(Tagging,','))";
            }
            filter += $" and (Title like '%{inputDTO.NewsSearchKeyword}%' or AuthorsName like '%{inputDTO.NewsSearchKeyword}%' or PublicationName like '%{inputDTO.NewsSearchKeyword}%' or Keywords like '%{inputDTO.NewsSearchKeyword}%')";
            string query = $"select EmployeeNewsId,UnitId,Title,ArticleType,Article,ArticleLink,Description,AuthorsNameRaw,AuthorsName,PublicationName,Tagging,CategoryTags=STUFF((SELECT ', ' + b.NewsCategoryTag FROM NewsCategoryTagMaster b  WHERE b.NewsCategoryTagId in (Select value from string_split(ea.Tagging,',') ) FOR XML PATH('')), 1, 2, ''),KeywordsRaw,Keywords,IsActive,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,(select Top 1 UploadedFilePath+'/'+UploadedFileName from [dbo].[EmployeeNewsFileUploads] enfu where enfu.EmployeeNewsId=ea.EmployeeNewsId and enfu.IsActive=1 ) ImagePath from EmployeeNews ea  where UnitId={UnitId} and isactive=1 {filter} order by EmployeeNewsId desc {pageDetails}";

            List<EmployeeNewsDTO> dto = await _unitOfWork.EmployeeNews.GetTableData<EmployeeNewsDTO>(query);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }

    public async Task<int> EmployeeNewsListCountViewAll(int UnitId, EmployeeNewsViewModel inputDTO)
    {
        try
        {
            
            if (String.IsNullOrEmpty(inputDTO.NewsSearchKeyword))
            {
                inputDTO.NewsSearchKeyword = "";
            }
            string filter = "";
            if (inputDTO.NewsCategoryId > 0)
            {
                filter = $" and {inputDTO.NewsCategoryId} in (Select Value from string_split(Tagging,','))";
            }
            filter += $" and (Title like '%{inputDTO.NewsSearchKeyword}%' or AuthorsName like '%{inputDTO.NewsSearchKeyword}%' or PublicationName like '%{inputDTO.NewsSearchKeyword}%' or Keywords like '%{inputDTO.NewsSearchKeyword}%')";
            string query = $"select count(1) from EmployeeNews ea  where UnitId={UnitId} and isactive=1 {filter} ;";

            List<int> dto = await _unitOfWork.EmployeeNews.GetTableData<int>(query);

            if (dto != null && dto.Count > 0)
            {
                return dto.FirstOrDefault();
            }
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> NewsCategoryTagsForDashboard(int UnitId)
    {
        try
        {
            string query = $"Select * from NewsCategoryTagMaster where IsActive=1 and NewsCategoryTagId in (SELECT DISTINCT CAST(value AS INT) AS DistinctInteger FROM EmployeeNews CROSS APPLY STRING_SPLIT(Tagging, ',') where IsActive=1 and UnitId={UnitId})";

            List<NewsCategoryTagMasterDTO> dto = await _unitOfWork.EmployeeNews.GetTableData<NewsCategoryTagMasterDTO>(query);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> AnnouncementsForDashboard(int UnitId)
    {
        try
        {
            string query = $"Select * from AnnouncementTypeMaster where AnnouncementTypeId in (Select Distinct(AnnouncementType) from employeeAnnouncement where AnnouncementType is not null and IsActive=1 and UnitId={UnitId}) and IsACtive=1 and unitId={UnitId}";
            List<AnnouncementTypeMasterDTO> dto = await _unitOfWork.EmployeeNews.GetTableData<AnnouncementTypeMasterDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeNewsById(int EmployeeNewsId)
    {
        try
        {
            EmployeeNewsDTO dto = _mapper.Map<EmployeeNewsDTO>(await _unitOfWork.EmployeeNews.FindByIdAsync(EmployeeNewsId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> EmployeeNewsFilesByNewsId(int EmployeeNewsId)
    {
        try
        {
            List<EmployeeNewsFileUploadDTO> dto = _mapper.Map<List<EmployeeNewsFileUploadDTO>>(await _unitOfWork.EmployeeNewsFileUpload.GetFilterAll(x => x.IsActive == true && x.EmployeeNewsId == EmployeeNewsId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteEmployeeNewsFilesById(int EmployeeNewsFileUploadsId)
    {
        try
        {



            EmployeeNewsFileUpload employeeNewsFileUpload = await _unitOfWork.EmployeeNewsFileUpload.FindByIdAsync(EmployeeNewsFileUploadsId);
            if (employeeNewsFileUpload != null)
            {
                employeeNewsFileUpload.IsActive = false;
                await _unitOfWork.EmployeeNewsFileUpload.UpdateAsync(employeeNewsFileUpload);
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
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(EmployeeNewsList)}");
            throw;
        }
    }
    public async Task<IActionResult> DeleteEmployeeNewsById(int EmployeeNewsId)
    {
        try
        {
            EmployeeNews employeeNews = await _unitOfWork.EmployeeNews.FindByIdAsync(EmployeeNewsId);
            if (employeeNews != null)
            {
                employeeNews.IsActive = false;
                await _unitOfWork.EmployeeNews.UpdateAsync(employeeNews);
                return Ok();
            }
            else
            {
                return BadRequest("Unable to fetch the record");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(DeleteEmployeeNewsById)}");
            throw;
        }
    }
}
