using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;

public class ClientMasterRepository : GenericRepository<Client>, IClientRepository
{
	public ClientMasterRepository(SimpliDbContext context): base(context)
	{

	}
}


