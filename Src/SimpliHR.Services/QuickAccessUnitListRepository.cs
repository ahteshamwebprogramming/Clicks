using Dapper;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Attendance;
using SimpliHR.Infrastructure.Models.KeyValueModels;
using SimpliHR.Infrastructure.Models.Leave;
using SimpliHR.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
namespace SimpliHR.Services;

public class QuickAccessUnitListRepository : DapperGenericRepository<QuickAccessUnitList>, IQuickAccessUnitListRepository
{
   
    public QuickAccessUnitListRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }

   
}

