using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.ClientManagement
{
    public class ClientIDTypeMappingDTO
    {
        [Key]
        [Column("ClientIDTypeMappingID")]
        public int ClientIDTypeMappingID { get; set; }

        [Column("ClientID")]
        public int ClientId { get; set; }

        public int? IDTypeID { get; set; }

        public HttpResponseMessage? HttpMessage { get; set; }        

    }
}
