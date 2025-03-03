using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services
{
    public class PayrollFullnFinalSettingsRepository : GenericRepository<PayrollFullnFinalSettings>, IPayrollFullnFinalSettingsRepository
    {
        public PayrollFullnFinalSettingsRepository(SimpliDbContext dataContext) : base(dataContext)
        {
        }
    }
}
