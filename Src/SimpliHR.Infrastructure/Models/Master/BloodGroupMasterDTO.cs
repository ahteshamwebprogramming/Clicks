using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpliHR.Infrastructure.Models.Masters;

[Table("BloodGroupMaster")]
public partial class BloodGroupMasterDTO
{
    public int BloodGroupId { get; set; }

    public string EncryptedBloodGroupId { get; set; }
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter blood group code"), MaxLength(50, ErrorMessage = "Blood Group Code cannot exceed 50 characters")]
    public string? BloodGroupCode { get; set; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter blood group"), MaxLength(50, ErrorMessage = "Blood Group cannot exceed 50 characters")]
    public string? BloodGroupName { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public List<BloodGroupMasterDTO>? BloodGroupMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;

    public int? HttpStatusCode { get; set; } = 200;

}
