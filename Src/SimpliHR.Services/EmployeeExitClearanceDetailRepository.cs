using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;

public class EmployeeExitClearanceDetailRepository : DapperGenericRepository<EmployeeExitClearanceDetail>, IEmployeeExitClearanceDetailRepository
{
	public EmployeeExitClearanceDetailRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
       
    }
}


