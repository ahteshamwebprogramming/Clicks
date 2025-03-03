using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SimpliHR.Core.Entities;

public partial class ClientSetting
{
     [Key]
    [Column("ClientSettingId")]
    public int ClientSettingId { get; set; }

    public int? ClientId { get; set; }

    public string? ClientLogo { get; set; }

    //[StringLength(50)]
    public string? HeaderText { get; set; }

    public string? FooterText { get; set; }

    //[StringLength(50)]
    public string? ColorTheme { get; set; }

   // [StringLength(500)]
    public string? SupportLink { get; set; }

    //[StringLength(500)]
    public string? PoliciesLink { get; set; }

    //[StringLength(500)]
    public string? DocumentLink { get; set; }

   // [StringLength(5)]
    public string? MenuStyle { get; set; }

   // [StringLength(50)]
    public string? IDTypes { get; set; }

    //[StringLength(50)]
    public string? ModuleIds { get; set; }

    [MaxLength]
    public byte[]? ProfileImage { get; set; }

    public int? EmailProvider { get; set; }
    public string? ClientDomain { get; set; }


}
