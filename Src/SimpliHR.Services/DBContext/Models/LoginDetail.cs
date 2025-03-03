using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

public partial class LoginDetail
{
    [Key]
    [Column("LoginID")]
    public int LoginId { get; set; }

    [Column("EmpID")]
    public int? EmpId { get; set; }

    [StringLength(50)]
    public string? UserName { get; set; }

    [StringLength(500)]
    public string? Password { get; set; }

    public bool? IsActive { get; set; }
}
