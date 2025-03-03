using SimpliHR.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Models.StatutoryComponent;

namespace SimpliHR.Core.Repository;

public interface IStatutoryComponent_EPFRepository : IGenericRepository<StatutoryComponent_EPF>
{
    public Task<string> SaveEmployeeEPFMapping(StatutoryComponent_EPF epfEmployeeMapping, string mappedIDs);

}
