
namespace SimpliHR.Infrastructure.Models.Leave;

public partial class EmployeeCompOffDTO
{

    public int CompOffId { get; set; }
    public string? CompOffType { get; set; }
    public DateTime? CompOffDate { get; set; }
    public string? EncryptedId { get; set; }
    public string? Remarks { get; set; }
    public string? TicketId { get; set; }
    public int? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
    public int? ApprovedBy { get; set; }

    public DateTime? CreatedOn { get; set; }
    public int? UnitId { get; set; }

    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeCode { get; set; }
    public string? Location { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = "_blank";
    public List<EmployeeCompOffDTO>? EmployeeCompOffList { get; set; }
   
    public int? HttpStatusCode { get; set; } = 200;

}

