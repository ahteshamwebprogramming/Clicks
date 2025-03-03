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

public class EmployeeNewsViewModel
{
    public List<IFormFile>? Attachment { get; set; }
    public List<IFormFile>? Upload { get; set; }

    public bool ExistingThumbnail { get; set; }

    public string? ThumbnailImage { get; set; }
    public EmployeeNewsDTO? employeeNewsDTO { get; set; }

    public List<EmployeeNewsDTO>? EmployeeNewsList { get; set; }

    public List<EmployeeNewsFileUploadDTO>? EmployeeNewsFileUploadList { get; set; }

    public List<NewsCategoryTagKeyValues>? NewsCategoryTags { get; set; }
    public List<NewsCategoryTagMasterDTO>? NewsCategoryTagsList { get; set; }
    public int? NewsCategoryId { get; set; }
    public string? NewsSearchKeyword { get; set; }
    public EmployeeNewsPageDetails? PageDetails { get; set; }
}
public class EmployeeNewsPageDetails
{
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
    public int? TotalPages { get; set; }
    public int? TotalRecords { get; set; }
}