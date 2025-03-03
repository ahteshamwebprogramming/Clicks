
namespace SimpliHR.Infrastructure.Models.Employee;

public class MainDirectoryDTO
{
    public int? DirectoryId { get; set; }
    public string? DirectoryColumns { get; set; }
    public bool? IsActive { get; set; }
    public int? PositionId { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<MainDirectoryDTO>? MainDirectoryList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}


public class members
{
    public int? id { get; set; }
    public string? EmployeeName { get; set; }
    public string? JobTitle { get; set; }
    public string? Base64ProfileImage { get; set; }
    public byte[]? ProfileImage { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<members>? membersList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}

//public class EmployeeCard
//{
//    public int? ID { get; set; }
//    public string? DisplayName { get; set; }
//    public string? ColumnValue { get; set; }
//    public string? Base64ProfileImage { get; set; }
//    public byte[]? ProfileImage { get; set; }
//    public int? IsOptional { get; set; }
//    public List<EmployeeCard>? EmployeeCardDetails { get; set; }
  
//}
