using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;


public class PaySlipDetailsRepository : DapperGenericRepository<PaySlipDetails>, IPaySlipDetailsRepository
    {
        public PaySlipDetailsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }

