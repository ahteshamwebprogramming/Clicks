using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;

public class CountryMasterDTO
{
    public int CountryId { get; set; }
    public string EncryptedCountryId { get; set; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter Country"), MaxLength(50, ErrorMessage = "Country name cannot exceed 50 characters")]
    public string CountryName { get; set; } = null!;
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsIncludes { get; set; } = false;
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<CountryMasterDTO>? CountryMasterList { get; set; }
    public int? HttpStatusCode { get; set; } =200;


}
