using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Services.DBContext;

namespace SimpliHR.Services;


public class RegimeSelectionReportRepository : DapperGenericRepository<RegimeSelectionReport>, IRegimeSelectionReportRepository
    {
        public RegimeSelectionReportRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }

