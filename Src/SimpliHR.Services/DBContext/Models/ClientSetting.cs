using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Models;

public partial class ClientSetting
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ClientID")]
    public int? ClientId { get; set; }

    public string? ClientLogo { get; set; }

    [StringLength(50)]
    public string? HeaderText { get; set; }

    public bool? IsClientLogo { get; set; }

    [StringLength(50)]
    public string? Footer { get; set; }

    [StringLength(50)]
    public string? ThemeName { get; set; }
}
