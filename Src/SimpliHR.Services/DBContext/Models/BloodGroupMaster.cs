using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

[Table("BloodGroupMaster")]
public partial class BloodGroupMaster
{
    [Key]
    [Column("BloodGroupID")]
    public int BloodGroupId { get; set; }

    [StringLength(50)]
    public string? BloodGroupCode { get; set; }

    [StringLength(50)]
    public string? BloodGroupName { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    public string? ModifedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedOn { get; set; }
}
