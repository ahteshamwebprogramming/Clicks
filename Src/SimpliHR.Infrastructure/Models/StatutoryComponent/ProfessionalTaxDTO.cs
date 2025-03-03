using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.StatutoryComponent;

public partial class ProfessionalTaxDTO
{
    public int ProfTaxId { get; set; }

    public decimal? ProfTax { get; set; }

    public int? CountryId { get; set; }
    public string? CountryName { get; set; }
    public int? StateId { get; set; }
    public string? StateName { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }

    public DateTime? WEFDate { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? Gender { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }   
    public int? UnitId { get; set; }
    public string EncryptedId { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public HttpResponseMessage? HttpMessage { get; set; }
   public List<ProfessionalTaxDTO>? ProfessionalTaxList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;

    [ValidateNever]
    public List<CountryKeyValues> CountryList { get; set; }
    [ValidateNever]
    public List<StateKeyValues> StateList { get; set; }

    [ValidateNever]
    public CountryMasterDTO Country { get; set; } = null!;
    [ValidateNever]
    public StateMasterDTO State { get; set; } = null!;

}