using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.Master;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Net.Security;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace SimpliHR.Infrastructure.Helper;

public static class CommonHelper
{
    public static T DeepCopy<T>(T obj)
    {
        try
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(memoryStream);
            }
        }
        catch (Exception ex)
        {
            return obj;
        }

    }
    public static T GetClassObject<T>(T t) where T : class, new()
    {
        if (t == null)
        {
            t = new T();
        }
        return t;
    }
    public static string RandomString()
    {
        try
        {
            string s = Guid.NewGuid().ToString("N").ToLower()
                  .Replace("1", "").Replace("o", "").Replace("0", "")
                  .Substring(0, 10);
            return s;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static bool IsNumeric(string Value)
    {
        return decimal.TryParse(Value, out _) || double.TryParse(Value, out _);

    }
    public static string NewLineEntry()
    {
        return "<br>";
    }
    public static DateTime? StringToDateTime(string sDateTime)
    {
        DateTime? returnDateTime = new DateTime();
        try
        {
            returnDateTime = DateTime.Parse(sDateTime);
            //        ,
            //"MM/dd/yyyy HH:mm:ss",
            //CultureInfo.InvariantCulture);
            return returnDateTime;
        }
        catch (Exception ex)
        {
            return returnDateTime;
        }
    }

    public static TimeOnly? StringToTimeOnly(string sTime)
    {
        TimeOnly? returnTime = new TimeOnly();
        try
        {
            returnTime = TimeOnly.ParseExact(sTime,
        "hh:mm:ss tt");
            return returnTime;
        }
        catch (Exception ex)
        {
            return returnTime;
        }
    }

    public static DateOnly? StringToDateOnly(string sDate)
    {
        DateOnly? returnDate = new DateOnly();
        try
        {
            returnDate = DateOnly.ParseExact(sDate,
        "MM/dd/yyyy");
            return returnDate;
        }
        catch (Exception ex)
        {
            return returnDate;
        }
    }
    public static HttpResponseMessage GetHttpResponseMessage<T>(T t, bool? isActive = true)
    {
        if (isActive == false)
            return ClientResponse.GetClientResponse(HttpStatusCode.Locked, "Inactive User");
        else if (t == null)
            return ClientResponse.GetClientResponse(HttpStatusCode.NotFound, "Wrong User Name and Password");
        else
            return ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success");
    }
    public static DataTable ToDataTable<T>(this List<T> items)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);

        //Get all the properties  
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in Props)
        {
            //Defining type of data column gives proper data table   
            var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
            //Setting column names as Property names  
            dataTable.Columns.Add(prop.Name, type);
        }
        foreach (T item in items)
        {
            var values = new object[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows  
                values[i] = Props[i].GetValue(item, null);
            }
            dataTable.Rows.Add(values);
        }
        return dataTable;
    }
    public static string Encrypt(string clearText)
    {
        string origninalText = clearText;
        string EncryptionKey = "MAKV2S54FUCKP99212";
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        if (clearText.Contains('/') || clearText.Contains('+'))
        {
            clearText = clearText.Replace("/", "slash").Replace("+", "plus");
        }
        return clearText;
    }
    public static string EncryptURL(string clearText)
    {
        clearText = Encrypt(clearText);
        clearText = WebUtility.UrlEncode(clearText);
        return clearText;
    }
    public static string EncryptURLHTML(string clearText)
    {
        clearText = Encrypt(clearText);
        //clearText = WebUtility.UrlEncode(clearText);
        //clearText = WebUtility.UrlEncode(clearText);
        //clearText = WebUtility.HtmlEncode(clearText);
        return clearText;
    }
    public static string DecryptURLHTML(string clearText)
    {
        //clearText = WebUtility.HtmlDecode(clearText);
        //clearText = WebUtility.UrlDecode(clearText);
        clearText = Decrypt(clearText);
        return clearText;
    }
    public static string DecryptURL(string clearText)
    {
        clearText = Decrypt(clearText);
        clearText = WebUtility.UrlDecode(clearText);
        return clearText;
    }
    public static string Decrypt(string cipherText)
    {
        cipherText = cipherText.Replace("slash", "/").Replace("plus", "+");
        string EncryptionKey = "MAKV2S54FUCKP99212";
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    public static EmployeeMasterDTO GetEmployeeForSession(EmployeeMasterDTO employeeMasterDTORes)
    {

        EmployeeMasterDTO employeeMasterDTO = new EmployeeMasterDTO();

        employeeMasterDTO.EmployeeId = employeeMasterDTORes.EmployeeId;
        employeeMasterDTO.EnycEmployeeId = employeeMasterDTORes.EnycEmployeeId;
        employeeMasterDTO.ClientId = employeeMasterDTORes.ClientId;
        employeeMasterDTO.UnitId = employeeMasterDTORes.UnitId;
        employeeMasterDTO.EmployeeCode = employeeMasterDTORes.EmployeeCode;
        employeeMasterDTO.FirstName = employeeMasterDTORes.FirstName;
        employeeMasterDTO.MiddleName = employeeMasterDTORes.MiddleName;
        employeeMasterDTO.LastName = employeeMasterDTORes.LastName;
        employeeMasterDTO.EmployeeName = employeeMasterDTORes.EmployeeName;
        employeeMasterDTO.Dob = employeeMasterDTORes.Dob;
        employeeMasterDTO.GenderId = employeeMasterDTORes.GenderId;
        employeeMasterDTO.Gender = employeeMasterDTORes.Gender;
        employeeMasterDTO.FatherName = employeeMasterDTORes.FatherName;
        employeeMasterDTO.ContactNo = employeeMasterDTORes.ContactNo;
        employeeMasterDTO.EmailId = employeeMasterDTORes.EmailId;
        employeeMasterDTO.Age = employeeMasterDTORes.Age;
        employeeMasterDTO.SpouseName = employeeMasterDTORes.SpouseName;
        employeeMasterDTO.ReligionId = employeeMasterDTORes.ReligionId;
        employeeMasterDTO.MaritalStatusId = employeeMasterDTORes.MaritalStatusId;
        employeeMasterDTO.AadharNumber = employeeMasterDTORes.AadharNumber;
        employeeMasterDTO.Pannumber = employeeMasterDTORes.Pannumber;
        employeeMasterDTO.BloodGroupId = employeeMasterDTORes.BloodGroupId;
        employeeMasterDTO.ProfileImage = employeeMasterDTORes.ProfileImage;
        employeeMasterDTO.ProfileImageFile = employeeMasterDTORes.ProfileImageFile;
        employeeMasterDTO.Base64ProfileImage = employeeMasterDTORes.Base64ProfileImage;
        employeeMasterDTO.WorkLocationId = employeeMasterDTORes.WorkLocationId;
        employeeMasterDTO.Doj = employeeMasterDTORes.Doj;
        employeeMasterDTO.DepartmentId = employeeMasterDTORes.DepartmentId;
        employeeMasterDTO.JobTitleId = employeeMasterDTORes.JobTitleId;
        employeeMasterDTO.ManagerId = employeeMasterDTORes.ManagerId;
        employeeMasterDTO.HODId = employeeMasterDTORes.HODId;
        employeeMasterDTO.OfficialEmail = employeeMasterDTORes.OfficialEmail;
        employeeMasterDTO.IdentityId = employeeMasterDTORes.IdentityId;
        employeeMasterDTO.EmployeeStatus = employeeMasterDTORes.EmployeeStatus;
        employeeMasterDTO.RoleId = employeeMasterDTORes.RoleId;
        employeeMasterDTO.BandId = employeeMasterDTORes.BandId;
        employeeMasterDTO.JoinType = employeeMasterDTORes.JoinType;
        employeeMasterDTO.BasicSalary = employeeMasterDTORes.BasicSalary;
        employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.AnnualBasicSalary;
        employeeMasterDTO.AnnualCtc = employeeMasterDTORes.AnnualCtc;
        employeeMasterDTO.MonthlyCtc = employeeMasterDTORes.MonthlyCtc;
        employeeMasterDTO.OtherCompensation = employeeMasterDTORes.OtherCompensation;
        employeeMasterDTO.SalaryInHand = employeeMasterDTORes.SalaryInHand;
        employeeMasterDTO.CreatedOn = employeeMasterDTORes.CreatedOn;
        employeeMasterDTO.CreatedBy = employeeMasterDTORes.CreatedBy;
        employeeMasterDTO.ModifiedOn = employeeMasterDTORes.ModifiedOn;
        employeeMasterDTO.ModifiedBy = employeeMasterDTORes.ModifiedBy;
        employeeMasterDTO.InfoFillingStatus = employeeMasterDTORes.InfoFillingStatus;
        employeeMasterDTO.IsActive = employeeMasterDTORes.IsActive;
        employeeMasterDTO.Layout = employeeMasterDTORes.Layout;
        employeeMasterDTO.ProfileImageExtension = employeeMasterDTORes.ProfileImageExtension;
        //employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
        //employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
        //employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
        //employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
        //employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
        //employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;


        //employeeMasterDTO.EmployeeCode = employeeMasterDTORes.EmployeeCode;
        //employeeMasterDTO.UnitId = employeeMasterDTORes.UnitId;
        //employeeMasterDTO.ClientId = employeeMasterDTORes.ClientId;
        //employeeMasterDTO.EmailId = employeeMasterDTORes.EmailId;
        //employeeMasterDTO.Base64ProfileImage = employeeMasterDTORes.Base64ProfileImage;
        //employeeMasterDTO.ProfileImage = employeeMasterDTORes.ProfileImage;
        //employeeMasterDTO.RoleId = employeeMasterDTORes.RoleId;


        return employeeMasterDTO;
    }

    public static string getContentTypeByExtesnion(string extension)
    {
        string contentType = "";
        switch (extension)
        {
            case ".txt":
                return "text/plain";
            case ".pdf":
                contentType = "application/pdf";
                break;
            case ".xslx":
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                break;
            case ".docx":
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                break;
            case ".doc":
                contentType = "application/msword";
                break;
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".gif":
                return "image/gif";
            case ".mp4":
                return "video/mp4";
            case ".avi":
                return "video/x-msvideo";
            case ".wmv":
                return "video/x-ms-wmv";
            case ".ppt":
            case ".pptx":
                return "application/vnd.ms-powerpoint";
            case ".xls":
                return "application/vnd.ms-excel";
            // and so on
            default:
                throw new ArgumentOutOfRangeException($"Unable to find Content Type for file");
        }
        return contentType;
    }

    public static IEnumerable<DateTime> EachDateBetweenDates(DateTime startDate, DateTime endDate)
    {
        for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
        return date;
    }


    public static void StoreInFolder(IFormFile file, string fileName)
    {

        using (FileStream fs = System.IO.File.Create(fileName))
        {
            file.CopyTo(fs);
            fs.Flush();
        }
    }

    public static ulong GetEpochTime(DateTime? startDateTime)
    {
        const long INIT_DATA_TICKS = 621355968000000000; // 1.1.1970 in ticks
        const double ROUNDINGS_FIX = 0.5;
        TimeSpan dTicks = new TimeSpan(DateTime.UtcNow.Ticks - INIT_DATA_TICKS);
        if (startDateTime != null)
            dTicks = new TimeSpan(startDateTime.Value.ToUniversalTime().Ticks - INIT_DATA_TICKS);
        return (ulong)(dTicks.TotalMilliseconds + ROUNDINGS_FIX);
    }

    public static List<T> ConvertToList<T>(DataTable dt)
    {
        var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
        var properties = typeof(T).GetProperties();
        return dt.AsEnumerable().Select(row =>
        {
            var objT = Activator.CreateInstance<T>();
            foreach (var pro in properties)
            {
                if (columnNames.Contains(pro.Name.ToLower()))
                {
                    try
                    {
                        string x = "";
                        //Convert.ChangeType(row[pro.Name], pro.PropertyType);
                        if (pro.Name.ToString() == "IsActive")
                            x = pro.Name.ToString();
                        if (pro.Name.ToLower() == "percentage")
                            pro.SetValue(objT, (row[pro.Name].ToString() == "" ? null : Convert.ToDecimal(row[pro.Name])));
                        else if (pro.PropertyType.FullName.ToLower().Contains("boolean"))
                            pro.SetValue(objT, (row[pro.Name].ToString() == "" ? null : (row[pro.Name].ToString() == "1" || row[pro.Name].ToString().ToLower() == "true" ? true : false)));
                        else if (row[pro.Name].GetType().Name.ToLower() == "double" && pro.PropertyType.GetType().Name.ToLower() != "double")
                            pro.SetValue(objT, (row[pro.Name] == "" ? null : Convert.ToInt32(row[pro.Name])));
                        else if (pro.PropertyType.GetType().Name.ToLower() == "datetime")
                            pro.SetValue(objT, (row[pro.Name].ToString() == "" ? null : row[pro.Name]));
                        else
                            pro.SetValue(objT, (row[pro.Name].ToString() == "" ? null : row[pro.Name]));

                    }
                    catch (Exception ex)
                    {
                        var x = 0;
                        try
                        {
                            pro.SetValue(objT, (row[pro.Name].ToString() == "" ? null : row[pro.Name]));
                        }
                        catch (Exception ex2) { }

                        //return objT;
                    }
                }
            }
            return objT;
        }).ToList();
    }

    public static List<int> GetYears(int startYear, int noOfYears)
    {
        return Enumerable.Range(startYear, noOfYears).Reverse().ToList();
    }
    public static bool IsBase64String(string s)
    {
        try
        {
            Convert.FromBase64String(s);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
    public static bool IsBase64(this string base64String)
    {

        if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
           || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
            return false;

        try
        {
            Convert.FromBase64String(base64String);
            return true;
        }
        catch (Exception exception)
        {
            // Handle the exception
        }
        return false;
    }

    public async static Task<T> GetInstanceField<T>(Type type, object instance, string fieldName)
    {
        BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Static;

        FieldInfo field = type.GetField(fieldName, bindFlags);
        return (T)field.GetValue(instance);
    }

    public async static Task WriteToFile(string path, string fileName, string message)
    {
        try
        {
            string filePathWithName = @path + "\\" + fileName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePathWithName))
            {
                using (FileStream fs = File.Create(filePathWithName)) ;
            }
            //Pass the filepath and filename to the StreamWriter Constructor
            //StreamWriter sw = new StreamWriter(filePathWithName);
            //Write a line of text
            using (StreamWriter sw = File.AppendText(filePathWithName))
            {
                sw.WriteLineAsync($"======Loged Started At===={DateTime.Now}");
                sw.WriteLineAsync(message);
                sw.WriteLineAsync($"======Loged Ended At===={DateTime.Now}\n");
            }
            //sw.WriteLineAsync($"======Loged Started At===={DateTime.Now}");
            //sw.WriteLine(message);
            //sw.WriteLine($"======Loged Ended At===={DateTime.Now}");
            //Close the file
            //sw.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            Console.WriteLine("Executing finally block.");
        }
    }

    public static string CreateTicket(string moduleCode, string sTicketId)
    {
        string concatCode = $"{moduleCode}/{DateTime.Now.ToString("yyMMdd-HHmmssssff")}/";
        if (sTicketId == null || sTicketId == "")
        {
            Random rnd = new Random();
            sTicketId = concatCode + rnd.Next(1, 1000).ToString(); // creates a 8 digit random no.
        }
        else if (sTicketId.IndexOf(moduleCode) < 0)
        {
            sTicketId = concatCode + sTicketId;
        }
        return sTicketId;
    }

    public static string GetBase64String(string ext)
    {
        string Base64String = string.Empty;
        string imgExtStr = ",png,jpeg,jpg,bmp,gif,tiff,";
        if (imgExtStr.ToLower().Contains("," + ext + ","))
            Base64String = "data:image/png;base64,";
        else if (ext == "pdf")
            Base64String = "data:application/pdf;base64,";
        else if (ext == "csv")
            Base64String = "data:application/octet-stream;charset=utf-8;base64,";
        else if (ext == "txt")
            Base64String = "data:application/octet-stream,";
        return Base64String;
    }

    public static DateTime GetZoneTime(string sZone)
    {
        DateTime dateTime = DateTime.Now; // I am getting date time here
        DateTime utcTime = dateTime.ToUniversalTime(); // From current datetime I am retriving UTC time
        //TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"); // Now I am Getting `IST` time From `UTC`
        TimeZoneInfo userZone = TimeZoneInfo.FindSystemTimeZoneById(sZone);
        DateTime userDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, userZone); // Finally converting it 
        //var hourIST = yourISTTime.Hour; // More granular extraction. 
        return userDateTime;
    }

    public static Stream GenerateStreamFromString(string s)
    {
        //MemoryStream stream = new MemoryStream();
        //StreamWriter writer = new StreamWriter(stream);
        //writer.Write(s);
        //writer.Flush();
        //stream.Position = 0;
        //return stream;
        Stream stream = new MemoryStream();
        String strText = "This is a String that needs to beconvert in stream";
        byte[] byteArray = Encoding.UTF8.GetBytes(strText);
        stream.Write(byteArray, 0, byteArray.Length);
        //set the position at the beginning.
        stream.Position = 0;
        //using (StreamReader sr = new StreamReader(stream))
        //{
        //    string strData;
        //    while ((strData = sr.ReadLine()) != null)
        //    {
        //        Console.WriteLine(strData);
        //    }
        //}
        return stream;
    }

}


public enum ColorThemes : int
{
    Default = 1,
    Blue = 2,
    Green = 3,
    Orange = 4,
    Pink = 5,
    Red = 6
}
public enum Gender : int
{
    Male = 1,
    Female = 2
}

public enum months : int
{
    January = 01,
    February = 02,
    March = 03,
    April = 04,
    May = 05,
    June = 06,
    July = 07,
    August = 08,
    September = 09,
    October = 10,
    November = 11,
    December = 12
}
public enum SalYears : int
{
    Year2023 = 2023,
    Year2024 = 2024
    //FY2025 = 2025,
    //FY2026 = 2026,
    //FY2027 = 2027,
    //FY2028 = 2028,
    //FY2029 = 2029,
    //FY2030 = 2030

}






