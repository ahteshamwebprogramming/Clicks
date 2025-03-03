using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Services;

public class ItDeclaration80CinvestmentRepository : GenericRepository<ItDeclaration80Cinvestment>, IItDeclaration80CinvestmentRepository
{
    public ItDeclaration80CinvestmentRepository(SimpliDbContext context) : base(context)
    {

    }
}
