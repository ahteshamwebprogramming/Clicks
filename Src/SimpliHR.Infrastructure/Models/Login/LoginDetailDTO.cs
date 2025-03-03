using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.Infrastructure.Models.Login;

public class LoginDetailDTO
{
    public int LoginId { get; set; }

    public int? EmployeeId { get; set; }

    public string? UserName { get; set; }

    public string? MobileNo { get; set; }

    public string? Password { get; set; }
    public string? EncryptedPassword { get; set; }

    public int? ClientId { get; set; }

    public int? LoginType { get; set; }

    public bool? IsActive { get; set; }

    public bool? JoiningMailSent { get; set; }
    public bool? IsPasswordSetByUser { get; set; }
    public virtual ClientDTO? Client { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public string? EncryptedEmpId { get; set; }

    public string DisplayMessage = "_blank";


}
