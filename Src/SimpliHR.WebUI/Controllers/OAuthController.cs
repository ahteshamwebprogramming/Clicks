using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Employee;

namespace SimpliHR.WebUI.Controllers
{
    public class OAuthController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnv;

        public OAuthController(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            this.hostingEnv = env;

        }
        public ActionResult callback(string code, string error, string state)
        {
            var logPath = Path.Combine(this.hostingEnv.WebRootPath, "log");
            if (string.IsNullOrWhiteSpace(error))
            {
               
               
                this.GetTokens(code);
            }
            else
            {
               
            }
            return Redirect("/Employee/Dashboard");

        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult GetTokens(string code)
        {
            var logPath = Path.Combine(this.hostingEnv.WebRootPath, "log");
          //  CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "GetTokens");
            EmployeeDashboardVM outputData = new EmployeeDashboardVM();
            string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");
            string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "Tokens.json");

            JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));
            RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();
            objRequest.AddQueryParameter("client_id", credital["client_id"].ToString());
            objRequest.AddQueryParameter("client_secret", credital["client_secret"].ToString());
            objRequest.AddQueryParameter("code", code);
            objRequest.AddQueryParameter("grant_type", "authorization_code");
            objRequest.AddQueryParameter("redirect_uri", "https://simplihrms.com/oauth/callback");
           // CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "GetTokens11");
            var client = new RestClient(new System.Uri("https://oauth2.googleapis.com/token"));
            // var baseUrl = client.Options.BaseUrl;
            var response = client.Post(objRequest);
           // CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "GetTokens22");
            if (response.StatusCode== System.Net.HttpStatusCode.OK)
            {
                System.IO.File.WriteAllText(Tpath, response.Content);

                return Redirect("/Employee/Dashboard");
             
            }
            return RedirectToAction("Dashboard", "Employee");
           
        }


        public ActionResult Outlookcallback(string code, string error, string state)
        {
            string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "OutlookCredits.json");
            string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "TokensOutlook.json");
            var logPath = Path.Combine(this.hostingEnv.WebRootPath, "log");

            if (string.IsNullOrWhiteSpace(error))
            {
                JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));

                RestClient objClient = new RestClient();
                RestRequest objRequest = new RestRequest();

                objRequest.AddQueryParameter("client_id", credital["client_id"].ToString());
                objRequest.AddQueryParameter("client_secret", credital["client_secret"].ToString());
                objRequest.AddQueryParameter("scope", credital["scopes"].ToString());
                objRequest.AddQueryParameter("code", code);
                objRequest.AddQueryParameter("grant_type", "authorization_code");
                objRequest.AddQueryParameter("redirect_uri", credital["redirect_url"].ToString());
            
                var client = new RestClient(new System.Uri("https://login.microsoftonline.com/common/oauth2/v2.0/token"));
                
                var response = client.Post(objRequest);
               
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    System.IO.File.WriteAllText(Tpath, response.Content);

                    return Redirect("/Employee/Dashboard");

                }


            }
            else
            {

            }
            return Redirect("/Employee/Dashboard");

        }

    }
}
