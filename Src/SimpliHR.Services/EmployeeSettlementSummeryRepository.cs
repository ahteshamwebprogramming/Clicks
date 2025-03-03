using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services
{
    public class EmployeeSettlementSummeryRepository : GenericRepository<EmployeeSettlementSummery>, IEmployeeSettlementSummeryRepository
    {
        public EmployeeSettlementSummeryRepository(SimpliDbContext dataContext) : base(dataContext)
        {
        }
    }
}
