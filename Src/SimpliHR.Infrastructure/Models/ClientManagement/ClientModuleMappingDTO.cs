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
    public class EmployeeUnitsMappingDTO
    {
       
        public int EmployeeUnitID { get; set; }
      
        public int ClientID { get; set; }

        public int? UnitID { get; set; }

        public int? EmployeeID { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }

        public string DisplayMessage { get; set; } = "_blank";

    }
}
