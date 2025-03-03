using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Infrastructure.Models.Masters;

[Table("ResourceMaster")]
public partial class ResourceMasterDTO
{
    [Key]
    [Column("ResourceID")]
    public int ResourceId { get; set; }
    public string EncryptedId { get; set; }

    [StringLength(100)]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter resource"), MaxLength(100, ErrorMessage = "Resource cannot exceed 100 characters")]
    public string? ResourceName { get; set; }

    [StringLength(500)]
    public string? ResourceDesc { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }

    public HttpResponseMessage? HttpMessage { get; set; }
    public List<RoleMasterDTO>? RoleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
   
    public List<ResourceMasterDTO>? ResourceMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;
    public int? UnitId { get; set; }

}
