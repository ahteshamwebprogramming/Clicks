using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SimpliHR.Core.Repository;
namespace SimpliHR.Services.DBContext;

public class DapperDBContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    public DapperDBContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("SimplyConnectionDB");
    }
    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
