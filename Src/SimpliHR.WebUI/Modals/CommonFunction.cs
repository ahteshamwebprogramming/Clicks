using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace Simpli2._0.Modals;
public class CommonFunction
{
    //private readonly IConfiguration _configInfo;
    //private readonly ILogger _logger;
    //private readonly string _CurrentEnvApiKey = "ApiDevBaseURL";
    //private CommonFunction(IConfiguration configInfo)
    //{
    //    _configInfo = configInfo;
    //}
    public async Task<HttpResponseMessage> GetApiData(dynamic Data, string actionName, string apiControllers)
    {

        string apiURL = GetApiURL("ApiDevBaseURL", apiControllers);
        HttpResponseMessage responseMessage = new HttpResponseMessage();
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(apiURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
            responseMessage = await client.GetAsync(actionName);
        }
        return responseMessage;
    }
    public String GetApiURL(String configAPIKey,string apiAction)
    {
        string baseURL = _configInfo.GetValue<string>(configAPIKey);
        baseURL = string.Format($"{baseURL}{apiAction}/");
        return baseURL;
    }
}
