namespace SimpliHR.WebUI.Modals.Masters;

public class Country
{
    public int CountryId { get; set; }
    public string CountryName { get; set; } = null!;
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public List<Country> Countries { get; set; }
}
