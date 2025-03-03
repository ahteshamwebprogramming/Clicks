using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class BankUnitMasterDTO
{
    public int BankId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter bank name"), MaxLength(50, ErrorMessage = "Bank cannot exceed 50 characters")]
    public string? BankName { get; set; }
    public bool? IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<BankUnitMasterDTO>? BankMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public int? UnitId { get; set; }
}


public partial class UnitBankListVM
{
    public int? HttpStatusCode { get; set; } = 200;

    public List<UnitMasterDTO>? Units { get; set; } = new List<UnitMasterDTO>();
    public BankUnitMasterDTO? UnitBank { get; set; } = new BankUnitMasterDTO();
    public List<BankUnitMasterDTO>? UnitBankList { get; set; } = new List<BankUnitMasterDTO>();
    public List<BankMasterDTO>? BankMasterList { get; set; } = new List<BankMasterDTO>();
    public string DisplayMessage { get; set; } = "_blank";
}
