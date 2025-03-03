

using Microsoft.Extensions.Configuration;

namespace SimpliHR.Services.DBContext
{
    public class DapperDBSetting
    {
        //private readonly IConfiguration configuration;
       private  readonly string _connectionString;
        public DapperDBSetting(IConfiguration configuration)
        {
            string UserId = System.Configuration.ConfigurationManager.AppSettings["LoginName"];
            ConnectionString = configuration.GetSection("ConnectionStrings:SimplyConnectionDB").Value;
            //  strConnection = ConfigurationManager.ConnectionStrings["SimplyDBConnection"].ConnectionString;
        }
        // var strval = configuration.GetConnectionString("SimplyDBConnection");

        public string ConnectionString { get; set; }

    }
}
