using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Services;

public class EmployeeNewsRepository : DapperGenericRepository<EmployeeNews>, IEmployeeNewsRepository
{
    public EmployeeNewsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}
