using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SimpliHR.Infrastructure.Models.Masters;
public partial class BandMasterDTO
{
    public int BandId { get; set; }
    public string EncryptedId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter band code"), MaxLength(10, ErrorMessage = "Band Code cannot exceed 10 characters")]
    public string? BandCode { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter band name"), MaxLength(50, ErrorMessage = "Band cannot exceed 50 characters")]
    public string? Band { get; set; }
    public string? BandDesc { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<BandMasterDTO>? BandMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public int? UnitId { get; set; }
}
