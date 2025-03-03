using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services
{
    public class ClientIDTypeMappingRepository : GenericRepository<ClientIDTypeMapping>, IClientIDTypeMappingRepository
    {
        public ClientIDTypeMappingRepository(SimpliDbContext dataContext) : base(dataContext)
        {
        }
    }
}
