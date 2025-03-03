using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class BankMasterDTO
{
    public int BankId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter bank name"), MaxLength(50, ErrorMessage = "Bank cannot exceed 50 characters")]
    public string? BankName { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<BankMasterDTO>? BankMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public int? UnitId { get; set; }
}
