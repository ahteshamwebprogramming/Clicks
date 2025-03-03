using Dapper;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Services.DBContext;
using System.Data;

namespace SimpliHR.Services;

public class PayrollDeductionComponentRepository : DapperGenericRepository<PayrollDeductionComponent>, IPayrollDeductionComponentRepository
{
    public PayrollDeductionComponentRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}


