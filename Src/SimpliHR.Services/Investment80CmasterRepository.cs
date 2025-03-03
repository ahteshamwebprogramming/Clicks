using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Services;

public class Investment80CmasterRepository : GenericRepository<Investment80Cmaster>, IInvestment80CmasterRepository
{
    public Investment80CmasterRepository(SimpliDbContext context) : base(context)
    {

    }
}
