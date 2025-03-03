using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;

public class ExitClearanceMappingRepository : DapperGenericRepository<ExitClearanceMapping>, IExitClearanceMappingRepository
{
	public ExitClearanceMappingRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
       
    }
}


