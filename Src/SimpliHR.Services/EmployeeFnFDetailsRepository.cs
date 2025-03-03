using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services
{
    public class EmployeeFnFDetailsRepository : GenericRepository<EmployeeFnFDetails>, IEmployeeFnFDetailsRepository
    {
        public EmployeeFnFDetailsRepository(SimpliDbContext dataContext) : base(dataContext)
        {
        }
    }
}
