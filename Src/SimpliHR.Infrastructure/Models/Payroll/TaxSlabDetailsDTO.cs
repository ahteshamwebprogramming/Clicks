using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Infrastructure.Models.Payroll;

public partial class TaxSlabDetailsDTO
{
    public int SlabID { get; set; }
    public string EncryptedId { get; set; }
    public decimal? AmtFrom { get; set; }
    public decimal? AmtTo { get; set; }
    public bool? IsActive { get; set; }
    public int? TaxPercentage { get; set; }
    public string? Regime { get; set; }
    public int? CessTax { get; set; }
    public int? UnitId { get; set; }
    public int? AgeGroupId { get; set; }
    public string? FY { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; } = 200;
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<TaxSlabDetailsDTO>? TaxSlabDetailsList { get; set; }
}

