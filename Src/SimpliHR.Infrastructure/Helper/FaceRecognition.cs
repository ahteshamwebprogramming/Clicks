using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using SimpliHR.Infrastructure.FaceRecognition;
using MXFaceAPICall;
using System.ComponentModel.Design;
using SimpliHR.Infrastructure.Models.Login;
using Microsoft.AspNetCore.Mvc;
using MXFaceAPICall.Model;
using System.Reflection.PortableExecutable;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.Cms;
//using System.Drawing.Common;
//using System.Drawing.Imaging;

namespace SimpliHR.Infrastructure.Helper;

public class FaceRecognition
{
    private string _apiUrl = "https://faceapi.mxface.ai";
    private string _subscriptionKey = "rwiVhVJ0ILmZ2nb5Ew-fjpuKw6JyO2060";
    //public async Task<bool> RecognizeEmployee(string loginPersonImage,string EmployeeCode, string UnitId, string unitImagePath,string profileImageExt)
    //{
    //    string apiToken = "d8c5eeaaf3f648baa6936c3e0afd17d2";
    //    string persionPicPath = loginPersonImage;
    //    LoginDetailDTO loginDetail=new LoginDetailDTO();
    //    //string employeeImage =  $@"{unitImagePath}\employee1.jpg";
    //    if (string.IsNullOrEmpty(profileImageExt))
    //        profileImageExt = "jpg";
    //    else
    //        profileImageExt=profileImageExt.Replace(".", "");
    //    bool faceMatched=false;
    //    string employeeImage = $@"{unitImagePath}\{EmployeeCode}.{profileImageExt}";
    //    //bool checkLiveness = await CheckLiveness(EmployeeCode,loginPersonImage, apiToken);
    //    //if (checkLiveness)
    //    int linenessScore = await MXAPILiveness(loginPersonImage);
    //    if(linenessScore>85)
    //        faceMatched = await MXAPICompare(loginPersonImage, employeeImage);

    //     return faceMatched;
    //    //return checkLiveness;
    //}

    public async Task<string> RecognizeEmployee(string loginPersonImage, string EmployeeCode, string UnitId, string unitImagePath, string profileImageExt)
    {
        string apiToken = "d8c5eeaaf3f648baa6936c3e0afd17d2";
        string returnResult = string.Empty;
        string persionPicPath = loginPersonImage;
        LoginDetailDTO loginDetail = new LoginDetailDTO();
        //string employeeImage =  $@"{unitImagePath}\employee1.jpg";
        if (string.IsNullOrEmpty(profileImageExt))
            profileImageExt = "jpg";
        else
            profileImageExt = profileImageExt.Replace(".", "");
        bool faceMatched = false;
        string fileName = $@"{EmployeeCode}.{profileImageExt}";
        string employeeImage = $@"{unitImagePath}\{fileName}";
        string filePath = unitImagePath + @"\";
        bool compareResult=false;
        string checkLiveness = await CheckLiveness(EmployeeCode, loginPersonImage, apiToken);
        if (checkLiveness.ToLower() == "real")
        {
            compareResult = await CompareFace(loginPersonImage, employeeImage, apiToken);
            if (!compareResult)
            {
                returnResult = "Failed to match with login profile";
            }
        }
        else if(checkLiveness!="")
            returnResult = checkLiveness;
        else
            returnResult = "There is Issue with the Image. Try again!";
        //if (checkLiveness)

        return returnResult;
    }

    public async Task<dynamic> CheckLiveness(string uuid, string file, string apiToken)
    {
        bool liveness = false;
        HttpClientHandler handler = new HttpClientHandler();
        try {
            string fileName = Path.GetFileName(file);
            using (var httpClient = new HttpClient(handler))
            {
                LuxandParams luxandParams = new LuxandParams();
                var livenessURL = @$"https://api.luxand.cloud/photo/liveness/v2";
                //luxandParams.Token = apiToken;
                // luxandParams.photos = loginPersonImage;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.luxand.cloud/photo/liveness/v2");
                request.Headers.Add("token", apiToken);
                //var content = new MultipartFormDataContent();


                using (FileStream stream = File.OpenRead(file))
                {
                    MultipartFormDataContent content = new MultipartFormDataContent
                {
                    { new StreamContent(stream), "photo",fileName }
                };
                    request.Content = content;
                    //HttpResponseMessage responseMessage = await client.PostAsync("https://api.luxand.cloud/photo/liveness/v2", content);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    JObject objResult = JObject.Parse(result);
                    // bool similer = (bool)objResult["similer"];
                    string livenessResult = GetJArrayValue(objResult, "result");
                    if(livenessResult=="")
                        livenessResult = GetJArrayValue(objResult, "message");
                    return livenessResult;
                    
                }

                //var content = new MultipartFormDataContent();
                //content.Add(new StreamContent(File.OpenRead(file)), "photo", fileName);
                //request.Content = content;
                //var response = await client.SendAsync(request);
                //response.EnsureSuccessStatusCode();
                ////var result = JsonConvert.DeserializeObject<OperationResponseObject>(await responseMessage.Content.ReadAsStringAsync());
                //var result = await response.Content.ReadAsStringAsync();
                //return result;
            }

        }
        catch(Exception ex)
        {
            return false;
        }
        

    }

    public async Task<bool> CompareFace(string loginPerson, string profileImage, string apiToken)
    {
        HttpClientHandler handler = new HttpClientHandler();
        //request.Headers.Add("token", "apiToken");
        string fileName1 = Path.GetFileName(loginPerson);
        string fileName2 = Path.GetFileName(profileImage);
        

        using (var httpClient = new HttpClient(handler))
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.luxand.cloud/photo/similarity");
            request.Headers.Add("token", apiToken);
            //var content = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            using (FileStream streamLoginPerson = File.OpenRead(loginPerson))
            {
                content.Add(new StreamContent(File.OpenRead(loginPerson)), "face1", fileName1);
                using (FileStream stream = File.OpenRead(profileImage))
                {
                    content.Add(new StreamContent(File.OpenRead(loginPerson)), "face2", fileName2);
                };
            };
            using (content)
            {
                content.Add(new StringContent("0.8"), "threshold");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //var result = await response.Content.ReadAsStringAsync();
                dynamic result = await response.Content.ReadAsStringAsync();
                JObject objResult = JObject.Parse(result);
                // bool similer = (bool)objResult["similer"];
                string similer = GetJArrayValue(objResult,"similar");
                return Convert.ToBoolean(similer);
            }
            
        }
    }

    public string GetJArrayValue(JObject yourJArray, string key)
    {
        string retValue = string.Empty;
        foreach (KeyValuePair<string, JToken> keyValuePair in yourJArray)
        {
            if (key == keyValuePair.Key)
            {
                 retValue = keyValuePair.Value.ToString();
            }
        }
        return retValue;
    }

    //public async Task<dynamic> CheckLive()
    //{
    //    var client = new HttpClient();
    //    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.luxand.cloud/photo/liveness/v2");
    //    request.Headers.Add("token", "{{api-token}}");
    //    var content = new MultipartFormDataContent();
    //    content.Add(new StreamContent(File.OpenRead("")), "photo", "");
    //    request.Content = content;
    //    var response = await client.SendAsync(request);
    //    response.EnsureSuccessStatusCode();
    //    Console.WriteLine(await response.Content.ReadAsStringAsync());
    //Face Compare
    //var client = new HttpClient();
    //var request = new HttpRequestMessage(HttpMethod.Post, "https://api.luxand.cloud/photo/similarity");
    //request.Headers.Add("token", "{{api-token}}");
    //var content = new MultipartFormDataContent();
    //content.Add(new StreamContent(File.OpenRead("")), "face1", "");
    //content.Add(new StreamContent(File.OpenRead("")), "face2", "");
    //content.Add(new StringContent("0.8"), "threshold");
    //request.Content = content;
    //var response = await client.SendAsync(request);
    //response.EnsureSuccessStatusCode();
    //Console.WriteLine(await response.Content.ReadAsStringAsync());

    //}

    public async Task<int> MXAPILiveness(string image)
    {
        try
        {
            int livenessScore = 95;
            // MXFaceAPI mxfaceAPI = new MXFaceAPI("https://faceapi.mxface.ai", _subscriptionKey);
            MXFaceAPI mxfaceAPI = new MXFaceAPI("https://faceapi.mxface.ai", _subscriptionKey);


            using (var httpClient = new HttpClient())
            {
                APIRequest request = new APIRequest
                {
                    encoded_image = Convert.ToBase64String(System.IO.File.ReadAllBytes(image)),

                };
                string jsonRequest = JsonConvert.SerializeObject(request);
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscriptionKey);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("https://faceapi.mxface.ai/api/v3/face/Liveness", httpContent);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var liveness = JsonConvert.DeserializeObject(apiResponse);

                    //var compareJson = JsonConvert.SerializeObject(compareFaces, Formatting.Indented);
                    //Console.WriteLine(compareJson);
                    //if (!isFaceMatched)
                    //    return isFaceMatched;
                }
                //else
                //{
                //    //Console.WriteLine("Error {0}, {1}", response.StatusCode, apiResponse);
                return livenessScore;
                //}
            }
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<bool> MXAPICompare(string loginPersonImage, string employeeImage)
    {
        try
        {
            bool isFaceMatched = false;
            // MXFaceAPI mxfaceAPI = new MXFaceAPI("https://faceapi.mxface.ai", _subscriptionKey);
            MXFaceAPI mxfaceAPI = new MXFaceAPI("https://faceapi.mxface.ai", _subscriptionKey);


            using (var httpClient = new HttpClient())
            {
                APICompareRequest request = new APICompareRequest
                {
                    encoded_image1 = Convert.ToBase64String(System.IO.File.ReadAllBytes(loginPersonImage)),
                    encoded_image2 = Convert.ToBase64String(System.IO.File.ReadAllBytes(employeeImage)),
                };
                string jsonRequest = JsonConvert.SerializeObject(request);
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscriptionKey);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("/api/v3/face/verify", httpContent);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MatchedFaceResponse compareFaces = JsonConvert.DeserializeObject<MatchedFaceResponse>(apiResponse);
                    foreach (var item in compareFaces.MatchedFaces)
                    {
                        Console.WriteLine("Confidence of match {0}", item.matchResult);
                        if (item.matchResult == 1)
                        {
                            isFaceMatched = true;
                            break;
                        }
                    }
                    //var compareJson = JsonConvert.SerializeObject(compareFaces, Formatting.Indented);
                    //Console.WriteLine(compareJson);
                    if (!isFaceMatched)
                        return isFaceMatched;
                }
                //else
                //{
                //    //Console.WriteLine("Error {0}, {1}", response.StatusCode, apiResponse);
                return isFaceMatched;
                //}
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    //public async Task<bool> Compare(string loginPersonImage,string employeeImage)
    //{
    //    try
    //    {
    //        bool isFaceMatched = false;
    //        // MXFaceAPI mxfaceAPI = new MXFaceAPI("https://faceapi.mxface.ai", _subscriptionKey);
    //        MXFaceAPI mxfaceAPI = new MXFaceAPI("https://faceapi.mxface.ai/api/v3/face/detect", _subscriptionKey); 


    //        using (var httpClient = new HttpClient())
    //        {
    //            APICompareRequest request = new APICompareRequest
    //            {
    //                encoded_image1 = Convert.ToBase64String(System.IO.File.ReadAllBytes(loginPersonImage)),
    //                encoded_image2 = Convert.ToBase64String(System.IO.File.ReadAllBytes(employeeImage)),
    //            };
    //            string jsonRequest = JsonConvert.SerializeObject(request);
    //            httpClient.BaseAddress = new Uri(_apiUrl);
    //            httpClient.DefaultRequestHeaders.Accept.Clear();
    //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    //            httpClient.DefaultRequestHeaders.Add("subscriptionkey", _subscriptionKey);
    //            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
    //            HttpResponseMessage response = await httpClient.PostAsync("/api/v3/face/verify", httpContent);
    //            string apiResponse = await response.Content.ReadAsStringAsync();
    //            if (response.StatusCode == HttpStatusCode.OK)
    //            {
    //                MatchedFaceResponse compareFaces = JsonConvert.DeserializeObject<MatchedFaceResponse>(apiResponse);
    //                foreach (var item in compareFaces.MatchedFaces)
    //                {
    //                    Console.WriteLine("Confidence of match {0}", item.matchResult);
    //                    if (item.matchResult == 1)
    //                    {
    //                        isFaceMatched = true;
    //                        break;
    //                    }
    //                }
    //                //var compareJson = JsonConvert.SerializeObject(compareFaces, Formatting.Indented);
    //                //Console.WriteLine(compareJson);
    //                if (!isFaceMatched)
    //                    return isFaceMatched;
    //            }
    //            //else
    //            //{
    //            //    //Console.WriteLine("Error {0}, {1}", response.StatusCode, apiResponse);
    //            return isFaceMatched;
    //            //}
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return false;
    //    }
    //}

    //public ActionResult Image()
    //{
    //    var bitmap = GetBitmap(); // The method that returns List<Bitmap>
    //    var width = 0;
    //    var height = 0;
    //    foreach (var image in bitmap)
    //    {
    //        width += image.Width;
    //        height = image.Height > height
    //            ? image.Height
    //            : height;
    //    }
    //    var bitmap2 = new Bitmap(width, height);
    //    var g = Graphics.FromImage(bitmap2);
    //    var localWidth = 0;
    //    foreach (var image in bitmap)
    //    {
    //        g.DrawImage(image, localWidth, 0);
    //        localWidth += image.Width;
    //    }

    //    var ms = new MemoryStream();

    //    bitmap2.Save(ms, ImageFormat.Png);
    //    var result = ms.ToArray();
    //    //string base64String = Convert.ToBase64String(result); 
    //    return File(result, "image/jpeg"); //Return as file result
    //    //return base64String;
    //}
    ////this method returns List<Bitmap>
    //public List<Bitmap> GetBitmap()
    //{
    //    var lstbitmap = new List<Bitmap>();
    //    var bitmap = new Bitmap(@"E:\My project\ProjectImage\ProjectImage\BmImage\1525244892128.JPEG");
    //    var bitmap2 = new Bitmap(@"E:\My project\ProjectImage\ProjectImage\BmImage\1525244892204.JPEG");
    //    var bitmap3 = new Bitmap(@"E:\My project\ProjectImage\ProjectImage\BmImage\3.jpg");
    //    lstbitmap.Add(bitmap);
    //    lstbitmap.Add(bitmap2);
    //    lstbitmap.Add(bitmap3);
    //    return lstbitmap;
    //}

}





