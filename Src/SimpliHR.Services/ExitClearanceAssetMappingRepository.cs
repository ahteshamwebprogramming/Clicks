using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;

public class ExitClearanceAssetMappingRepository : DapperGenericRepository<ExitClearanceAssetMapping>, IExitClearanceAssetMappingRepository
{
	public ExitClearanceAssetMappingRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
       
    }
}


