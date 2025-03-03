using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class QuickAccessListDTO
{
    public int? QuickAccessId { get; set; }
    public string? QuickAccessLogo { get; set; }
    public string? QuickAccessName { get; set; }
    public bool? IsActive { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<QuickAccessListDTO>? QuickAccessLists { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
   

}
