using SimpliHR.Services.DBContext;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Entities;
using SimpliHR.Core.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpliHR.Infrastructure.Helper;
using System.Data;
using Dapper;
using SimpliHR.Infrastructure.Models.KeyValue;

namespace SimpliHR.Services;

public class EmployeeValidationRepository : DapperGenericRepository<EmployeeValidation>, IEmployeeValidationRepository
{
	public EmployeeValidationRepository(DapperDBContext dapperDBContext): base(dapperDBContext)
	{
       
    }

    //public async Task<List<EmployeeValidation>> GetClientValidation(string? screenName, string? screenTab, int clientId = 0)
    //{
    //    List<EmployeeValidation> empValidationList = new List<EmployeeValidation>();
    //    try
    //    {
    //        var parms = new DynamicParameters();
    //        parms.Add(@"@ClientId", clientId, DbType.Int32);
    //        parms.Add(@"@ScreenName", screenName, DbType.String);
    //        parms.Add(@"@ScreenTab", screenTab, DbType.String);
    //        empValidationList = await GetSPData<EmployeeValidation>("GetEmployeeValidation", parms);
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return empValidationList;
    //}
}


