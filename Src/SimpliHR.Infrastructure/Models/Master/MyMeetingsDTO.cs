using System.ComponentModel.DataAnnotations;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class MyMeetingsDTO
{
    public int? MeetingID { get; set; }

    public int? EmployeeId { get; set; }

    public int? UnitId { get; set; }

    public string? UserId { get; set; }

    public string? UserPassword { get; set; }
    public string? EncryptedPassword { get; set; }
    public string? UserType { get; set; }

    public bool? IsActive { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<MyMeetingsDTO>? MyMeetingsList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
   

}
