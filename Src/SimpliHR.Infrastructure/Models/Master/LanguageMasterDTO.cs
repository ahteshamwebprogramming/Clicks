using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class LanguageMasterDTO
{
    public int LanguageId { get; set; }
    public string? Language { get; set; }

    public string? EncryptedId { get; set; }
    public bool? IsActive { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<LanguageMasterDTO>? LanguageMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;

}
