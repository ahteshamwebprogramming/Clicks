using Microsoft.AspNetCore.Http;
using SimpliHR.Infrastructure.Models.KeyValue;

namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class Form16DocDTO
{
    public int FormId { get; set; }

    public string? DocName { get; set; }

    public byte[]? DocAttachment { get; set; }

    public string? AttachmentBase64String { get; set; }
    public IFormFile DocAttachmentFile { get; set; }
    public string? Pannumber { get; set; }

    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? FinYear { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool? IsActive { get; set; }
}
public partial class Fom16DocViewModel
{
    public string DisplayMessage { get; set; } = "_blank";
    public Form16DocDTO Form16Doc { get; set; }= new Form16DocDTO();
    public List<Form16DocDTO> Form16DocList { get; set; } = new List<Form16DocDTO>();
   
    public List<EmployeeKeyValues> EmployeeKeyValues { get; set; }
}
