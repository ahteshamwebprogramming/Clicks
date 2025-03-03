using Microsoft.AspNetCore.Http;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.KeyValues;
using SimpliHR.Infrastructure.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.EmployeeSocialActivity;

public class EmployeeAnnouncementViewModel
{
    public List<IFormFile>? Attachment { get; set; }
    public List<IFormFile>? Upload { get; set; }

    public bool ExistingThumbnail { get; set; }

    public string? ThumbnailImage { get; set; }
    public EmployeeAnnouncementDTO? employeeAnnouncementDTO { get; set; }
    public EmployeeAnnouncementWithChild? EmployeeAnnouncementWithChild { get; set; }
    public List<DepartmentKeyValues>? Departments { get; set; }
    public List<AnnouncementTypeKeyValues>? AnnouncementTypes { get; set; }
    public List<AnnouncementTypeMasterDTO>? AnnouncementTypeMasterList { get; set; }

    public List<EmployeeAnnouncementWithChild>? EmployeeAnnouncementWithChildList { get; set; }

    public List<EmployeeAnnouncementFileUploadDTO>? EmployeeAnnouncementFileUploadList { get; set; }

    public List<SurveyPollViewModel>? SurveyPolls { get; set; }

    public string? SerializedSurveyPolls { get; set; }

    public int? AnnouncementTypeId { get; set; }
    public string? AnnouncementSearchKeyword { get; set; }

    public EmployeeAnnouncementPageDetails? PageDetails { get; set; }

}

public class EmployeeAnnouncementPageDetails
{
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
    public int? TotalPages { get; set; }    
    public int? TotalRecords { get; set; }
}

public class SurveyPollViewModel
{
    public SurveyPollsQuestionDTO? PollQuestion { get; set; }
    public List<SurveyPollOptionDTO>? PollOptions { get; set; }
    public List<PollResponseDTO>? PollResponses { get; set; }

}
public class EAVM
{
    public string Name { get; set; }
}
public class EmployeeAnnouncementWithChild
{
    public int EmployeeAnnouncementId { get; set; }
    public string? encEmployeeAnnouncementId { get; set; }

    public int? AnnouncementTypeId { get; set; }
    public string? AnnouncementType { get; set; }
    public int UnitId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Poll { get; set; }

    public bool? Survey { get; set; }

    public string Departments { get; set; } = null!;

    public string DepartmentNames { get; set; } = null!;

    public string KeywordsRaw { get; set; } = null!;

    public string Keywords { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime PublishDate { get; set; }

    public TimeSpan PublishTime { get; set; }

    public string? ImagePath { get; set; }

    public string? Source { get; set; }
}
