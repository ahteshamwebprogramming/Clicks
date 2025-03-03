
namespace SimpliHR.Infrastructure.Models.Employee;

public class EmployeeDirectoryDTO
{
    public int? EmployeeDirectoryId { get; set; }

    public int? ParentDirectoryId { get; set; }
    public int? UnitId { get; set; }
    public int? PositionId { get; set; }
    public string? SearchColumns { get; set; }
    public bool? IsActive { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<EmployeeDirectoryDTO>? EmployeeDirectoryList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}
public partial class EmployeeDirectoryAction
{
    public string? EmployeeDirectoryIds { get; set; }   
    public int? UnitId { get; set; }
    public string? IsActives { get; set; }
    public string? PositionIds { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}
