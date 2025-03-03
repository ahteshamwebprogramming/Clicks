using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class QuickAccessUnitListDTO
{
    public int? QuickAccessId { get; set; }
    public string? QuickAccessLink { get; set; }
    public string? QuickAccessLogo { get; set; }
    public string? QuickAccessName { get; set; }
    public int? ParentQuickAccessId { get; set; }
    public int? UnitId { get; set; }
    public int? PositionId { get; set; }
    public bool? IsActive { get; set; }  
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<QuickAccessUnitListDTO>? QuickAccessUnitLists { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
  

}

public partial class QuickAccessAction
{
    public string? QuickAccessIds { get; set; }
    public string? PositionIds { get; set; }
    public string? Links { get; set; }
    public int? UnitId { get; set; }
    public string? IsActives { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
}
