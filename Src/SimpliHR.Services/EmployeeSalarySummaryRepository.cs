using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;


public class EmployeeSalarySummaryRepository : DapperGenericRepository<EmployeeSalarySummary>, IEmployeeSalarySummaryRepository
    {
        public EmployeeSalarySummaryRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }

