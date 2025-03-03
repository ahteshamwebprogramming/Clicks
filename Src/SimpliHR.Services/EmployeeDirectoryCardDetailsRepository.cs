using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Services;

public class EmployeeDirectoryCardDetailsRepository : DapperGenericRepository<EmployeeDirectoryCardDetails>, IEmployeeDirectoryCardDetailsRepository
{
    public EmployeeDirectoryCardDetailsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
