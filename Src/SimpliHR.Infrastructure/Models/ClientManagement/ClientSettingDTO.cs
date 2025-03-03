using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using SimpliHR.Infrastructure.Models.KeyValue;

namespace SimpliHR.Infrastructure.Models.ClientManagement
{
    public class ClientSettingDTO
    {
       // [Key]
       // [Column("ClientSettingId")]
        public int ClientSettingId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select client")]
        public int? ClientId { get; set; }

        public string EncryptedClientSettingId { get; set; }
        public string? ClientLogo { get; set; }

        [MaxLength]
        public byte[]? ProfileImage { get; set; }
        public IFormFile ProfileImageFile { get; set; }

        public string Base64ProfileImage { get; set; }

        [StringLength(50)]
        public string? HeaderText { get; set; }

        public string? FooterText { get; set; }

        [StringLength(50)]
        public string? ColorTheme { get; set; }

        [StringLength(500)]
        public string? SupportLink { get; set; }

        [StringLength(500)]
        public string? PoliciesLink { get; set; }

        [StringLength(500)]
        public string? DocumentLink { get; set; }

        [StringLength(5)]
        public string? MenuStyle { get; set; }

        [StringLength(50)]
        public string? IDTypes { get; set; }

        [StringLength(50)]
        public string? ModuleIds { get; set; }
        [StringLength(50)]
        public string? ClientName { get; set; }

        public int? EmailProvider { get; set; }
        public string? ClientDomain { get; set; }
        public string? RoleType { get; set; }
        public List<ClientSettingDTO> lstClientSetting { get; set; }
        public List<IdtypeKeyValues> IDTypeList { get; set; }
       // public IdtypeMasterDTO IDType { get; set; } = null!;
        public List<ModuleKeyValues> ModuleList { get; set; }
        // public ModuleMasterDTO Module { get; set; } = null!;
        public string DisplayMessage { get; set; } = "_blank";
        public List<ClientKeyValues> ClientList { get; set; }
        public ClientDTO Client { get; set; } = null!;
        public HttpResponseMessage? HttpMessage { get; set; }


    }
}
