using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;


public class EmployeeTicketViewModelRepository : DapperGenericRepository<EmployeeTicketViewModel>, IEmployeeTicketViewModelRepository
{
        public EmployeeTicketViewModelRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }

