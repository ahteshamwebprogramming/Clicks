namespace SimpliHR.WebUI.Modals.Masters;

public class State
{
    public int StateId { get; set; }
    public int CountryId { get; set; }
    public string StateName { get; set; } = null!;
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public List<Country>? Countries { get; set; }
    public List<State>? States { get; set; }
}
