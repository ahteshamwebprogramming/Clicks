using Dapper;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Services.DBContext;
using System.Data;

namespace SimpliHR.Services;

public class PayrollReimbursementComponentRepository : DapperGenericRepository<PayrollReimbursementComponent>, IPayrollReimbursementComponentRepository
{
    public PayrollReimbursementComponentRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
    {

    }
}


