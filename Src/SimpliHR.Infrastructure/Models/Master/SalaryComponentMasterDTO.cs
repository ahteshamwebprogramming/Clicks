using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Infrastructure.Models.Masters;

[Table("SalaryComponentMaster")]
public partial class SalaryComponentMasterDTO
{
    [Key]
    [Column("SalaryComponentID")]
    public int SalaryComponentId { get; set; }
    public string EncryptedId { get; set; }

    [StringLength(50)]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter salary component title"), MaxLength(50, ErrorMessage = "Salary Component cannot exceed 50 characters")]
    public string? SalaryComponentTitle { get; set; }

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter display order")]
    public int? SalaryComponentDisapyOrder { get; set; }

    [StringLength(500)]
    public string? SalaryComponentDec { get; set; }

    public string? SalaryComponentType { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsBasic { get; set; }
    public bool? IsFixed { get; set; }
    public int? UnitId { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [Column("modifedBy")]
    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column("modifiedOn", TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
    public List<RoleMasterDTO>? RoleMasterList { get; set; }
    public string DisplayMessage { get; set; } = string.Empty;
    public List<SalaryComponentMasterDTO>? SalaryComponentMasterList { get; set; }
    public int? HttpStatusCode { get; set; } = 200;

}
