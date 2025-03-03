using SimpliHR.Services.DBContext;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Services;

public class EmployeeCompOffRepository : GenericRepository<EmployeeCompOff>, IEmployeeCompOffRepository
{
	public EmployeeCompOffRepository(SimpliDbContext context): base(context)
	{

	}
}


