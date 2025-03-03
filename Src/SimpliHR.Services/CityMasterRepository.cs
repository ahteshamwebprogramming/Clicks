using SimpliHR.Services.DBContext;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Services;

public class CityMasterRepository : GenericRepository<CityMaster>, ICityMasterRepository
{
	public CityMasterRepository(SimpliDbContext context): base(context)
	{

	}
}


