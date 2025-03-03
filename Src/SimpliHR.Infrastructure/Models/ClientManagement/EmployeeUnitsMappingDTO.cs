using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;

namespace SimpliHR.Infrastructure.Models.ClientManagement
{
    public class ClientModuleMappingDTO
    {
        [Key]
        [Column("ClientModuleMappingID")]
        public int ClientModuleMappingID { get; set; }

        [Column("ClientID")]
        public int ClientId { get; set; }

        public int? ModuleID { get; set; }

        public HttpResponseMessage? HttpMessage { get; set; }
   
       
   
    }
}
