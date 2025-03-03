using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services
{
    public class ClientModuleMappingRepository : GenericRepository<ClientModuleMapping>, IClientModuleMappingRepository
    {
        public ClientModuleMappingRepository(SimpliDbContext dataContext) : base(dataContext)
        {
        }
    }
}
