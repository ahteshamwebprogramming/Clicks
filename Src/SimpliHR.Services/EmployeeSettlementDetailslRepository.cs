using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services
{
    public class EmployeeSettlementDetailslRepository : DapperGenericRepository<EmployeeSettlementDetails>, IEmployeeSettlementDetailsRepository
    {
        public EmployeeSettlementDetailslRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {
        }
    }
}
