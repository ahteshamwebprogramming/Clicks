using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;

public class EmployeeExitClearanceRepository : DapperGenericRepository<EmployeeExitClearance>, IEmployeeExitClearanceRepository
{
	public EmployeeExitClearanceRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {
       
    }
}


