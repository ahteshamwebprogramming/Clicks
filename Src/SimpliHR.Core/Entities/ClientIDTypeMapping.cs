using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Core.Entities
{
    public class ClientIDTypeMapping
    {
        public int ClientIDTypeMappingId { get; set; }

        public int ClientId { get; set; }

        public int IDTypeId { get; set; }
    }
}
