using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(SimpliDbContext dataContext) : base(dataContext)
        {
        }
    }
}
