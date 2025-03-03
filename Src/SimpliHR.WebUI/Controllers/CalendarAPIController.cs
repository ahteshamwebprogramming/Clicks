using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json.Linq;
using RestSharp;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.GoogleCalendar;
using System.Globalization;
//using static Org.BouncyCastle.Math.EC.ECCurve;
using CalendarView = Microsoft.Exchange.WebServices.Data.CalendarView;
namespace SimpliHR.WebUI.Controllers
{
    public class CalendarAPIController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnv;
        private readonly IConfiguration _config;
        public  CalendarAPIController(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration config)
        {
            this.hostingEnv = env;
            _config = config;
        }
        public ActionResult CalendarAPIEvents()
        {
            string strdate = "06/15/2024 1200";
            strdate = strdate.Substring(0, 10);
            DateTime dd = DateTime.Now.Date;
            DateTime DT = DateTime.ParseExact(strdate, "MM/dd/yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
           // DateTime DT1= DateTime.ParseExact(DateTime.Now.Date.ToString(), "MM/dd/yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
            return View();
        }
        public IActionResult OutlookAPIEvents()
        {
            try
            {
                string url = "https://outlook.office365.com/ews/exchange.asmx";
                string userName = "SimpliHRMS@outlook.com";
                string password = "Delhi@2050";

                ExchangeService _exchangeService = new ExchangeService();
                _exchangeService.Url = new Uri(url);
                _exchangeService.UseDefaultCredentials = true;
                _exchangeService.Credentials = new WebCredentials(userName, password);
                DateTime startDate = DateTime.Now;
                DateTime endDate = startDate.AddDays(30);
                const int num_Apt = 5;
                CalendarFolder calendar = CalendarFolder.Bind(_exchangeService, WellKnownFolderName.Calendar, new PropertySet());
                CalendarView cView = new CalendarView(startDate, endDate, num_Apt);
              
                cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End);
                FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);
                foreach(Appointment a in appointments)
                {
                    string sub = a.Subject;
                    DateTime sDate = a.Start;
                    DateTime eDate = a.End;
                  //  var link = a.JoinOnlineMeetingUrl;

                }
            }
            catch (SystemException ex)
            {

            }
            return View();
           
        }


        public ActionResult OuthRedirect()
        {
            var logPath = Path.Combine(this.hostingEnv.WebRootPath, "log");
           // CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "OuthRedirect");
            string sLogPath = _config.GetValue<string>("LogFilePathName");
            try
            {
                string path = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");

                JObject credital = JObject.Parse(System.IO.File.ReadAllText(path));
                  var client_id = credital["client_id"];              
                var redirectUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                    "scope=https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events&" +
                    "access_type=offline&" +
                    "include_granted_scopes=true&" +
                    "response_type=code&" +
                    "state=there&" +
                    "redirect_uri=https://simplihrms.com/oauth/callback&" +
                    "client_id=" + client_id;
               // CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "OuthRedirect11");
              //  CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", redirectUrl);
                return Redirect(redirectUrl);

            }
            catch (SystemException ex)
            {
                var DisplayMessage = $"Source: {ex.Source}({nameof(OuthRedirect)})\n{ex.Message}";
                CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", DisplayMessage);
                // outputData = null;
                return Redirect("");
            }


        }

        public ActionResult OuthRedirectOutLook()
        {
            var logPath = Path.Combine(this.hostingEnv.WebRootPath, "log");
           // CommonHelper.WriteToFile(logPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", "OuthRedirect");
            string sLogPath = _config.GetValue<string>("LogFilePathName");
            try
            {
                string path = Path.Combine(this.hostingEnv.WebRootPath, "OutlookCredits.json");

                JObject credital = JObject.Parse(System.IO.File.ReadAllText(path));
               
                var redirectUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?" +
                    "&scope="+ credital["scopes"].ToString() +                   
                    "&response_type=code" +
                   "&response_mode=query" +
                    "&state=Simplideveloper" +
                    "&redirect_uri="+ credital["redirect_url"].ToString() +    
                    "&client_id=" + credital["client_id"].ToString();
              
                return Redirect(redirectUrl);

            }
            catch (SystemException ex)
            {
                var DisplayMessage = $"Source: {ex.Source}({nameof(OuthRedirect)})\n{ex.Message}";
                CommonHelper.WriteToFile(sLogPath, $"Log_{DateTime.Today.ToString("dd-MM-yy")}.txt", DisplayMessage);
                // outputData = null;
                return Redirect("");
            }


        }


        public ActionResult RefreshTokens()
        {

            string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");
            string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "Tokens.json");

            JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(Tpath));
            RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();
            objRequest.AddQueryParameter("client_id", credital["client_id"].ToString());
            objRequest.AddQueryParameter("client_secret", credital["client_secret"].ToString());
            objRequest.AddQueryParameter("grant_type", "refresh_token");
            objRequest.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());

            var client = new RestClient(new System.Uri("https://oauth2.googleapis.com/token"));
            // var baseUrl = client.Options.BaseUrl;
            var response = client.Post(objRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject newTokens = JObject.Parse(response.Content);
                newTokens["refresh_token"] = tokens["refresh_token"].ToString();
                System.IO.File.WriteAllText(Tpath, newTokens.ToString());
                RedirectToAction("CalendarAPIEvents", "CalendarAPI");
            }


            return View("ExceptionMessage");
        }

        public ActionResult RefreshOutLookTokens()
        {

            string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "OutlookCredits.json");
            string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "TokensOutlook.json");

            JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(Tpath));
            RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();
            objRequest.AddQueryParameter("client_id", credital["client_id"].ToString());
            objRequest.AddQueryParameter("client_secret", credital["client_secret"].ToString());
            objRequest.AddQueryParameter("grant_type", "refresh_token");
            objRequest.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());
            objRequest.AddQueryParameter("scope", credital["scopes"].ToString());
            objRequest.AddQueryParameter("redirect_uri", credital["redirect_url"].ToString());

            var client = new RestClient(new System.Uri("https://login.microsoftonline.com/common/oauth2/v2.0/token"));
            // var baseUrl = client.Options.BaseUrl;
            var response = client.Post(objRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject newTokens = JObject.Parse(response.Content);
               // newTokens["refresh_token"] = tokens["refresh_token"].ToString();
                System.IO.File.WriteAllText(Tpath, newTokens.ToString());
                RedirectToAction("CalendarAPIEvents", "CalendarAPI");
            }


            return View("ExceptionMessage");
        }

        public ActionResult RevokeTokens()
        {

            // string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");
            string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "Tokens.json");

            //JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(Tpath));
            RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();
            objRequest.AddQueryParameter("token", tokens["access_token"].ToString());


            var client = new RestClient(new System.Uri("https://oauth2.googleapis.com/revoke"));
            // var baseUrl = client.Options.BaseUrl;
            var response = client.Post(objRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                RedirectToAction("CalendarAPIEvents", "CalendarAPI");
            }


            return View("ExceptionMessage");
        }

        public ActionResult GetAllEvents()
        {
            List<GoogleCalendarReqDTO> objlist = new List<GoogleCalendarReqDTO>();
            // string Cpath = Path.Combine(this.hostingEnv.WebRootPath, "CleintCredits.json");
            string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "Tokens.json");

            //JObject credital = JObject.Parse(System.IO.File.ReadAllText(Cpath));
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(Tpath));
            RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();
            objRequest.AddQueryParameter("key", "AIzaSyCDAJbpvP4Vld4NTN0K5E7DpZtZQztoXrE");
            //objRequest.AddQueryParameter("timeMin", DateTime.Now);
            //objRequest.AddQueryParameter("timeMax", DateTime.Now.AddDays(1));
            objRequest.AddHeader("Authorization", "Bearer " + tokens["access_token"]);
            objRequest.AddHeader("Accept", "Application/json");

          

            var client = new RestClient(new System.Uri("https://www.googleapis.com/calendar/v3/calendars/primary/events"));
            // var baseUrl = client.Options.BaseUrl;
            var response = client.Get(objRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            { 
                JObject calendarEvents = JObject.Parse(response.Content);
                var allEvents = calendarEvents["items"].ToObject<IEnumerable<Event>>().TakeLast(5).Where(x=> DateTime.ParseExact(x.Start.DateTimeRaw.Substring(0,10), "MM/dd/yyyy", CultureInfo.CurrentUICulture.DateTimeFormat)>=DateTime.Now.Date);
                //  int? totalEvents= allEvents.
                if (allEvents!=null)
                {
                    foreach (var item in allEvents)
                    {
                        var CalenderEvnt = new GoogleCalendarReqDTO();
                        CalenderEvnt.Summary = item.Summary;
                        CalenderEvnt.StartTime = item.Start.DateTimeRaw;
                        CalenderEvnt.EndTime = item.End.DateTimeRaw;
                       // CalenderEvnt.StartDateTime = DateTime.Parse(item.Start.DateTimeRaw, CultureInfo.CreateSpecificCulture("fr-FR"));
                      //  CalenderEvnt.EndDateTime = Convert.ToDateTime(item.End.DateTimeRaw);
                        CalenderEvnt.Link = item.HtmlLink;

                        objlist.Add(CalenderEvnt);
                    }

                }
                RedirectToAction("CalendarAPIEvents", "CalendarAPI");
            }


           return View("ExceptionMessage");
        }

        public ActionResult GetAllOutLookEvents()
        {
            List<GoogleCalendarReqDTO> objlist = new List<GoogleCalendarReqDTO>();
       
            string Tpath = Path.Combine(this.hostingEnv.WebRootPath, "Tokens.json");

           
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(Tpath));
            RestClient objClient = new RestClient();
            RestRequest objRequest = new RestRequest();            
            objRequest.AddHeader("Authorization", "Bearer " + tokens["access_token"]);      

            var client = new RestClient(new System.Uri("https://www.googleapis.com/calendar/v3/calendars/primary/events"));
            
            var response = client.Get(objRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject calendarEvents = JObject.Parse(response.Content);
                var allEvents = calendarEvents["items"].ToObject<IEnumerable<Event>>().TakeLast(5).Where(x => DateTime.ParseExact(x.Start.DateTimeRaw.Substring(0, 10), "MM/dd/yyyy", CultureInfo.CurrentUICulture.DateTimeFormat) >= DateTime.Now.Date);
                //  int? totalEvents= allEvents.
                if (allEvents != null)
                {
                    foreach (var item in allEvents)
                    {
                        var CalenderEvnt = new GoogleCalendarReqDTO();
                        CalenderEvnt.Summary = item.Summary;
                        CalenderEvnt.StartTime = item.Start.DateTimeRaw;
                        CalenderEvnt.EndTime = item.End.DateTimeRaw;
                        // CalenderEvnt.StartDateTime = DateTime.Parse(item.Start.DateTimeRaw, CultureInfo.CreateSpecificCulture("fr-FR"));
                        //  CalenderEvnt.EndDateTime = Convert.ToDateTime(item.End.DateTimeRaw);
                        CalenderEvnt.Link = item.HtmlLink;

                        objlist.Add(CalenderEvnt);
                    }

                }
                RedirectToAction("CalendarAPIEvents", "CalendarAPI");
            }


            return View("ExceptionMessage");
        }

    }
}
