using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;


public class EmployeesSalaryProcessDetailsRepository : DapperGenericRepository<EmployeesSalaryProcessDetails>, IEmployeesSalaryProcessDetailsRepository
    {
        public EmployeesSalaryProcessDetailsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }

