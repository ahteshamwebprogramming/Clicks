using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Services;

public class PTMilestonesRepository : DapperGenericRepository<PTMilestones>, IPTMilestonesRepository
{
    public PTMilestonesRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
    }
}
