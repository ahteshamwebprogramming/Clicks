using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using SimpliHR.WebUI.Modals.Masters;
using SimpliHR.Infrastructure.Helper;
using SimpliHR.Endpoints.Masters;
using SimpliHR.Infrastructure.Models.Masters;
using SimpliHR.WebUI.Masters;
using System.Data;
using System.Net.Http.Headers;
using SimpliHR.Infrastructure.Models.Common;

namespace SimpliHR.WebUI.Controllers
{
    public class MastersController : Controller
    {

        //private readonly IConfiguration _configuration;
        //private MastersController(IConfiguration configInfo) 
        //{
        //    configInfo = _configuration;
        //}

        private readonly JobTitleMasterController _jobTitleApiController;
        private readonly RoleMasterController _roleAPIController;
        private readonly SchedularController _SchedularController;
        //private RoleMasterController RoleAPIController { set { this._roleAPIController = value; } }
        //private JobTitleMasterController JobTitleApiController { set { this._jobTitleApiController = value; } }
        public MastersController(JobTitleMasterController jobTitleApiController, RoleMasterController roleAPIController, SchedularController schedularController)
        {
            _jobTitleApiController = jobTitleApiController;
            _roleAPIController = roleAPIController;
            _SchedularController = schedularController;
        }

        public IActionResult BloodGroup()
        {
            return View();
        }
        public IActionResult Academic()
        {
            return View();
        }
        public IActionResult Department()
        {
            return View();
        }

        public IActionResult Banks()
        {
            return View();
        }
        public IActionResult IDType()
        {
            return View();
        }
        public IActionResult Band()
        {
            return View();
        }
        public IActionResult MaritalStatus()
        {
            return View();
        }
        public IActionResult Modules()
        {
            return View();
        }
        public IActionResult Religion()
        {
            return View();
        }
        public IActionResult Resource()
        {
            return View();
        }
        public IActionResult SchedulersEvent()
        {
            SchedularMessageDTO outPut = new SchedularMessageDTO();
            return View(outPut);
        }

        [HttpPost]
        public IActionResult GetSchedulers()
        {
            SchedularMessageDTO outPut = new SchedularMessageDTO();
            var units = HttpContext.Session.GetInt32("UnitId");
            string employeeid = "";
            var results = _SchedularController.SchedularEvent(Convert.ToString(units), employeeid);
            outPut.DisplayMessage ="Success";
            return View("SchedulersEvent", outPut);
        }
        //public IActionResult Role()
        //{
        //    return View();
        //}

        public async Task<IActionResult> Country()
        {
            string baseURL = "https://localhost:7134/api/CountryMaster/";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage getdata = await client.GetAsync("GetCountries");
                if (getdata.IsSuccessStatusCode)
                {
                    string result = getdata.Content.ReadAsStringAsync().Result;
                    Country country = new Country();
                    country.Countries = JsonConvert.DeserializeObject<List<Country>>(result);
                    return View(country);
                }
                else
                {
                    return View();
                }
            }
        }

        public async Task<IActionResult> GetCountry(int Id)
        {
            string baseURL = "https://localhost:7134/api/CountryMaster/";
            Country country = new Country();
            country.CountryId = Id;
            country.CountryName = "s";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage getdata = await client.PostAsJsonAsync<Country>("GetCountry", country);
                if (getdata.IsSuccessStatusCode)
                {
                    string result = getdata.Content.ReadAsStringAsync().Result;
                    var res = JsonConvert.DeserializeObject<HttpResponseMessage>(result);
                    if (res.IsSuccessStatusCode)
                    {
                        country = JsonConvert.DeserializeObject<Country>(result);
                        return View("Country", country);
                    }
                    else
                    {

                    }
                    return RedirectToAction("Country", "Masters");
                }
                else
                {
                    return RedirectToAction("Country", "Masters");
                }
            }
            //return RedirectToAction("Country", "Masters");
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry(Country country)
        {
            string baseURL = "https://localhost:7134/api/CountryMaster/";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage getdata = await client.PostAsJsonAsync<Country>("SaveCountry", country);
                if (getdata.IsSuccessStatusCode)
                {
                    string result = getdata.Content.ReadAsStringAsync().Result;
                    var res = JsonConvert.DeserializeObject<HttpResponseMessage>(result);
                    var r = res.IsSuccessStatusCode;
                    return RedirectToAction("Country", "Masters");
                }
                else
                {
                    return RedirectToAction("Country", "Masters");
                }
            }
            //return RedirectToAction("Country", "Masters");
        }


        #region "State,City,District,Role,JobTitle"    

        public async Task<IActionResult> State()
        {
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var client = new HttpClient();
            string baseAddress = "https://localhost:7134/";
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            //var taskResponse = apiList.Select(p => client.GetStringAsync(p));
            var response = await client.PostAsJsonAsync(
                $"api/StateMaster/GetStates", new { PageSize = 10, PageNumber = 1, IsInclude = 0 });
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadAsStringAsync();
            State state = new State();
            state.States = JsonConvert.DeserializeObject<List<State>>(token);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            string apiURL = string.Format($"{baseAddress}api/CountryMaster/GetCountryKeyValue");
            var taskResponse = State_Parallel(apiURL, client);
            var responses = await Task.WhenAll(taskResponse);
            var results = responses.Where(r => r != null).ToList();
            state.Countries = JsonConvert.DeserializeObject<List<Country>>(results[0]);
            return View(state);
        }

        [HttpPost]
        public async Task<IActionResult> AddState(State data)
        {
            string baseURL = "https://localhost:7134/api/StateMaster/";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage getdata = await client.PostAsJsonAsync<State>("SaveState", data);
                if (getdata.IsSuccessStatusCode)
                {
                    string result = getdata.Content.ReadAsStringAsync().Result;
                    var res = JsonConvert.DeserializeObject<HttpResponseMessage>(result);
                    var r = res.IsSuccessStatusCode;
                }
                return RedirectToAction("State", "Masters");
            }
            //return RedirectToAction("Country", "Masters");
        }

        [HttpPost]
        public async Task<IActionResult> GetState(Country country)
        {
            string baseURL = "https://localhost:7134/api/CountryMaster/";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage getdata = await client.PostAsJsonAsync<Country>("GetState", country);
                if (getdata.IsSuccessStatusCode)
                {
                    string result = getdata.Content.ReadAsStringAsync().Result;
                    var res = JsonConvert.DeserializeObject<HttpResponseMessage>(result);
                    var r = res.IsSuccessStatusCode;
                    return View("State", getdata);
                }
                else
                {
                    return RedirectToAction("State", "Masters");
                }
            }
            //return RedirectToAction("Country", "Masters");
        }

        public async Task<IActionResult> Role()
        {
            IActionResult actionResult = await _roleAPIController.GetRoles(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<RoleMasterDTO> objResultData = (List<RoleMasterDTO>)objResult.Value;

            if (objResultData != null)
            {
                return View(objResultData);
            }
            else
            {
                return View();
            }

            var client = new HttpClient();
            string baseAddress = "https://localhost:7134/";
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            //var taskResponse = apiList.Select(p => client.GetStringAsync(p));
            var response = await client.PostAsJsonAsync(
                $"api/RoleMaster/GetRoles", new { PageSize = 100, PageNumber = 1, IsInclude = false });
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadAsStringAsync();
            List<Role> role = new List<Role>();
            role = JsonConvert.DeserializeObject<List<Role>>(token);
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> SaveRole(Role role)
        {
            string baseURL = "https://localhost:7134/";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage getdata = await client.PostAsJsonAsync($"api/RoleMaster/SaveRole", role);
                if (getdata.IsSuccessStatusCode)
                {
                    string result = getdata.Content.ReadAsStringAsync().Result;
                    var res = JsonConvert.DeserializeObject<HttpResponseMessage>(result);
                    var r = res.IsSuccessStatusCode;
                }
                return RedirectToAction("Role", "Masters");
            }
            //return RedirectToAction("Country", "Masters");
        }

        public async Task<IActionResult> JobTitle()
        {
            IActionResult actionResult = await _jobTitleApiController.GetJobTitles(new Core.Helper.RequestParams { PageSize = 100, PageNumber = 1 }, HttpContext.Session.GetInt32("UnitId"));
            ObjectResult objResult = (ObjectResult)actionResult;

            List<JobTitleMasterDTO> objResultData = (List<JobTitleMasterDTO>)objResult.Value;

            if (objResultData != null)
            {
                return View(objResultData);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveJobTitle(JobTitleMasterDTO jobTitleVM)
        {

            IActionResult actionResult = _jobTitleApiController.SaveJobTitle(jobTitleVM);
            ObjectResult objResult = (ObjectResult)actionResult;

            var objResultData = objResult.Value;

            if (objResultData != null)
            {
                return RedirectToAction("JobTitle", "Masters");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> District()
        {
            string baseURLDistrict = "https://localhost:7134/api/DistrictMaster/";
            string baseURLState = "https://localhost:7134/api/StateMaster/";
            string baseURLCountry = "https://localhost:7134/api/CountryMaster/";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var tasks = new[]{
                     State_Parallel(baseURLDistrict+"GetDistricts", client),
                     State_Parallel(baseURLState+"GetStates", client),
                     State_Parallel(baseURLCountry+"GetCountries", client)
                     };
                // Await the completion of all the running tasks. 
                var responses = await Task.WhenAll(tasks);
                var results = responses.Where(r => r != null).ToList();
                District district = new District();
                if (results.Count() == 3)
                {
                    district.Districts = JsonConvert.DeserializeObject<List<District>>(results[0]);
                    district.States = JsonConvert.DeserializeObject<List<State>>(results[1]);
                    district.Countries = JsonConvert.DeserializeObject<List<Country>>(results[2]);
                    return View(district);
                }
                else
                {
                    return View();
                }
            }
        }

        public async Task<IActionResult> City()
        {
            string baseURLCity = "https://localhost:7134/api/CityMaster/";
            string baseURLDistrict = "https://localhost:7134/api/DistrictMaster/";
            string baseURLState = "https://localhost:7134/api/StateMaster/";
            string baseURLCountry = "https://localhost:7134/api/CountryMaster/";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var tasks = new[]{
                     State_Parallel(baseURLCity+"GetCities", client),
                     State_Parallel(baseURLDistrict+"GetDistricts", client),
                     State_Parallel(baseURLState+"GetStates", client),
                     State_Parallel(baseURLCountry+"GetCountries", client)
                     };
                // Await the completion of all the running tasks. 
                var responses = await Task.WhenAll(tasks);
                var results = responses.Where(r => r != null).ToList();
                City city = new City();
                if (results.Count() == 4)
                {
                    city.Cities = JsonConvert.DeserializeObject<List<City>>(results[0]);
                    //city.Districts = JsonConvert.DeserializeObject<List<District>>(results[1]);
                    //city.States = JsonConvert.DeserializeObject<List<State>>(results[2]);
                    //city.Countries = JsonConvert.DeserializeObject<List<Country>>(results[3]);
                    return View(city);
                }
                else
                {
                    return View();
                }
            }
        }

        #endregion



        public async Task<string> State_Parallel(string burl, HttpClient client)
        {
            //GET Method  
            //client.BaseAddress = new Uri(burl);
            using (var response = await client.GetAsync(burl))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            return null;
        }
    }
}
