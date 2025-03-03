using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;


public class PaySlipComponentsRepository : DapperGenericRepository<PaySlipComponents>, IPaySlipComponentsRepository
    {
        public PaySlipComponentsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }

