using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using MimeKit;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Infrastructure.Models.Employee;
using SimpliHR.Infrastructure.Models.KeyValue;
using SimpliHR.Infrastructure.Models.Login;
using System.ComponentModel.Design;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Collections.Specialized;
using System.Web;

namespace SimpliHR.Infrastructure.Helper;

public static class MailHelper
{
    private static string UnitEmailSubject(UnitMasterDTO client, LoginDetailDTO loginDetails)
    {
        string subject = "<p><strong>Dear " + client.ClientName + ",</strong></p>\r\n<p>This is with reference to created admin of&nbsp;<strong>" + client.UnitName + "</strong>  you are requested to kindly fill up the own Employment Details form available on the link below:</p>\r\n<p><a target=\"_blank\" href=\"https://simplihrms.com/\" rel=\"noopener\">https://simplihrms.com/</a></p>\r\n<p>Your login details for filling up the form are as follows:</p>\r\n<ul>\r\n<li>User ID: " + loginDetails.UserName + "</li>\r\n<li>Password: " + CommonHelper.Decrypt(loginDetails.Password) + "</li>\r\n</ul>\r\n<p>We welcome " + client.UnitName + " on board and wish a long and prosperous association with us!</p>\r\n<p>Team &ndash; SimplyHR</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact the Team in case of any queries.</em></p>";
        return subject;
    }
    private static string EmailSubject(EmployeeMasterDTO employee, LoginDetailDTO loginDetails)
    {
        EmployeeMastersKeyValues _masterKeyValues = new EmployeeMastersKeyValues();
        //string subject = "<p><strong>Dear " + employee.FirstName + " " + employee.LastName + ",</strong></p>\r\n<p>This is with reference to your employment with&nbsp;<strong>" + employee.ClientKeyValue.CompanyName + "</strong>&nbsp;as&nbsp;<strong> " + employee.JobTitleKeyValue.JobTitle + " </strong>&nbsp;with effect from&nbsp;<strong>" + (employee.Doj == null ? "" : ((DateTime)employee.Doj).ToString("dd/MM/yyyy")) + "</strong>.</p>\r\n<p>As part of joining formalities, you are requested to kindly fill up the online Employment Details form available on the link below:</p>\r\n<p><a target=\"_blank\" href=\"https://simplihr2uat-web.azurewebsites.net\" rel=\"noopener\">https://simplihr2uat-web.azurewebsites.net\"</a></p>\r\n<p>Your login details for filling up the form are as follows:</p>\r\n<ul>\r\n<li>User ID : " + loginDetails.UserName + "</li>\r\n<li>Password : " + loginDetails.Password + "</li>\r\n</ul>\r\n<p>We welcome you on board and wish you a long and prosperous association with us!</p>\r\n<p>Team &ndash; Human Resources</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact HR Team in case of any queries.</em></p>";
        string subject = "<p><strong>Dear " + employee.FirstName + " " + employee.LastName + ",</strong></p>\r\n<p>This is with reference to your employment with&nbsp;<strong>" + employee.ClientKeyValue.CompanyName + "</strong>&nbsp;as&nbsp;<strong> " + employee.JobTitleKeyValue.JobTitle + " </strong>&nbsp;with effect from&nbsp;<strong>" + (employee.Doj == null ? "" : ((DateTime)employee.Doj).ToString("dd/MM/yyyy")) + "</strong>.</p>\r\n<p>As part of joining formalities, you are requested to kindly fill up the online Employment Details form available on the link below:</p>\r\n<p><a target=\"_blank\" href=\"https://simplihrms.com\" rel=\"noopener\">https://simplihrms.com/\"</a></p>\r\n<p>Your login details for filling up the form are as follows:</p>\r\n<ul>\r\n<li>User ID : " + loginDetails.UserName + "</li>\r\n<li>Password : " + loginDetails.Password + "</li>\r\n</ul>\r\n<p>We welcome you on board and wish you a long and prosperous association with us!</p>\r\n<p>Team &ndash; Human Resources</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact HR Team in case of any queries.</em></p>";
        return subject;
    }
    private static string EmailLoginInductionBody(EmployeeMasterDTO employee, LoginDetailDTO loginDetails)
    {
        EmployeeMastersKeyValues _masterKeyValues = new EmployeeMastersKeyValues();
        //string subject = "<p><strong>Dear " + employee.FirstName + " " + employee.LastName + ",</strong></p>\r\n<p>This is with reference to your employment with&nbsp;<strong>" + employee.ClientKeyValue.CompanyName + "</strong>&nbsp;as&nbsp;<strong> " + employee.JobTitleKeyValue.JobTitle + " </strong>&nbsp;with effect from&nbsp;<strong>" + (employee.Doj == null ? "" : ((DateTime)employee.Doj).ToString("dd/MM/yyyy")) + "</strong>.</p>\r\n<p>As part of joining formalities, you are requested to kindly login to the link below:</p>\r\n<p><a target=\"_blank\" href=\"https://simplihr2uat-web.azurewebsites.net/\" rel=\"noopener\">https://simplihr2uat-web.azurewebsites.net</a></p>\r\n<p>Your login details are as follows:</p>\r\n<ul>\r\n<li>User ID : " + loginDetails.UserName + "</li>\r\n<li>Password : " + loginDetails.Password + "</li>\r\n</ul>\r\n<p>We welcome you on board and wish you a long and prosperous association with us!</p>\r\n<p>Team &ndash; Human Resources</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact HR Team in case of any queries.</em></p>";
        string subject = "<p><strong>Dear " + employee.FirstName + " " + employee.LastName + ",</strong></p>\r\n<p>This is with reference to your employment with&nbsp;<strong>" + employee.ClientKeyValue.CompanyName + "</strong>&nbsp;as&nbsp;<strong> " + employee.JobTitleKeyValue.JobTitle + " </strong>&nbsp;with effect from&nbsp;<strong>" + (employee.Doj == null ? "" : ((DateTime)employee.Doj).ToString("dd/MM/yyyy")) + "</strong>.</p>\r\n<p>As part of joining formalities, you are requested to kindly login to the link below:</p>\r\n<p><a target=\"_blank\" href=\"https://simplihrms.com/\" rel=\"noopener\">https://simplihrms.com</a></p>\r\n<p>Your login details are as follows:</p>\r\n<ul>\r\n<li>User ID : " + loginDetails.UserName + "</li>\r\n<li>Password : " + loginDetails.Password + "</li>\r\n</ul>\r\n<p>We welcome you on board and wish you a long and prosperous association with us!</p>\r\n<p>Team &ndash; Human Resources</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact HR Team in case of any queries.</em></p>";
        return subject;
    }

    public static bool SendMail(string subject, string mailBody, string? receiverMail = null, string? senderMail = null, AlternateView? mailbody = null, string? emailDisplayName = null, int? emailProvider = 0, string? filePath = null, string[]? ccEmails = null)
    {
        if (emailProvider == 1)
            return SentEmailByZeptoSMTP(subject, mailBody, receiverMail, senderMail, mailbody, emailDisplayName, filePath, ccEmails);
        else if (emailProvider == 2)
            return SendMailByGupShapAPI(subject, mailBody, receiverMail, senderMail, mailbody, emailDisplayName, ccEmails);
        else
            return SentEmailByGMailSMTP(subject, mailBody, receiverMail, senderMail, mailbody, emailDisplayName, ccEmails);


    }

    //public static bool MailSend(string subject, string mailBody, string? receiverMail = null, string? senderMail = null, AlternateView? mailbody = null, string? emailDisplayName = null,int? emailProvider=0)
    //{
    //    if(emailProvider==1)
    //    return MailHelper.SentEmailByZeptoSMTP(subject, mailBody, receiverMail, senderMail, null, emailDisplayName);
    //    else
    //    return MailHelper.SentEmailByGMailSMTP(subject, mailBody, receiverMail, senderMail, null, emailDisplayName);

    //}
    private static AlternateView GetEmbeddedImage(String filePath, string Mailbody)
    {
        LinkedResource res = new LinkedResource(filePath);
        res.ContentId = Guid.NewGuid().ToString();
        string htmlBody = @"<img alt='' class='h-auto ms-0 rounded user-profile-img' style='width: 70px;border-radius: 2.375rem !important;' src='cid:" + res.ContentId + @"'/>";
        Mailbody = Mailbody.Replace("#empImage#", htmlBody);
        AlternateView alternateView = AlternateView.CreateAlternateViewFromString(Mailbody, null, MediaTypeNames.Text.Html);
        alternateView.LinkedResources.Add(res);
        return alternateView;
    }
    public static string GetMailTemplate(string fileName)
    {
        string mailContent = string.Empty;
        string filePathName = Path.Combine(Environment.CurrentDirectory, $"MailTemplates\\{fileName}");
        try
        {
            using (StreamReader reader = File.OpenText(filePathName))
            {
                string fileContent = reader.ReadToEnd();
                if (fileContent != null && fileContent != "")
                {
                    return fileContent;
                }
            }
        }
        catch (Exception ex)
        {

        }

        return "";
    }

    public static bool SendLoginDetailMail(EmployeeMasterDTO employee, LoginDetailDTO loginDetails, UnitMasterDTO unit, int? emailProvider = 0, string? filePath = null)
    {
        try
        {
            string subject = "Login Induction Mail";
            string body = EmailLoginInductionBody(employee, loginDetails);
            //string receiverEmail = employee.EmailId == null ? "" : employee.EmailId;
            string receiverEmail = loginDetails.UserName == null ? "" : loginDetails.UserName;
            // receiverEmail = "mhasif78@gmail.com";
            string senderEmail = "simplihr97@gmail.com";
            string emailDisplayName = unit == null ? "" : unit.EmailDisplayName == null ? "" : unit.EmailDisplayName.Trim();
            //bool res = MailHelper.SendMail(subject, body, receiverEmail, senderEmail, null, emailDisplayName);
            //  bool res = MailHelper.SentEmailByZeptoSMTP(subject, body, receiverEmail, senderEmail, null, emailDisplayName);
            if (employee.EmailProvider == 1)
                return SentEmailByZeptoSMTP(subject, body, receiverEmail, senderEmail, null, emailDisplayName, filePath);
            else if (employee.EmailProvider == 2)
                return SendMailByGupShapAPI(subject, body, receiverEmail, senderEmail, null, emailDisplayName);
            else
                return SentEmailByGMailSMTP(subject, body, receiverEmail, senderEmail, null, emailDisplayName);

            // return res;


        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public static bool SendJoiningLink(UnitMasterDTO client, LoginDetailDTO loginDetails, int? emailProvider = 0)
    {
        try
        {
            string subject = "Login Induction Mail";
            string body = UnitEmailSubject(client, loginDetails);
            //string receiverEmail = employee.EmailId == null ? "" : employee.EmailId;
            string receiverEmail = loginDetails.UserName == null ? "" : loginDetails.UserName;
            // receiverEmail = "mhasif78@gmail.com";
            string senderEmail = "simplihr97@gmail.com";
            string emailDisplayName = client == null ? "" : client.EmailDisplayName == null ? "" : client.EmailDisplayName.Trim();
            //bool res = MailHelper.SendMail(subject, body, receiverEmail, senderEmail, null, emailDisplayName);
            //  bool res = MailHelper.SentEmailByZeptoSMTP(subject, body, receiverEmail, senderEmail, null, emailDisplayName);
            if (emailProvider == 1)
                return SentEmailByZeptoSMTP(subject, body, receiverEmail, senderEmail, null, emailDisplayName, "");
            else if (emailProvider == 2)
                return SendMailByGupShapAPI(subject, body, receiverEmail, senderEmail, null, emailDisplayName);
            else
                return SentEmailByGMailSMTP(subject, body, receiverEmail, senderEmail, null, emailDisplayName);

            // return res;


        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public static bool SentEmailByZeptoSMTP(string subject, string mailBody, string? receiverMail = null, string? senderMail = null, AlternateView? mailbody = null, string? emailDisplayName = null, string? filePath = null, string[]? ccEmails = null)
    {

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailDisplayName, "support@simplihrms.in"));
        message.To.Add(new MailboxAddress("", receiverMail));
        //  message.To.Add(new MailboxAddress("", "mrigaj.g@kaam.com"));

        // message.Cc.Add(new MailboxAddress("Asif", "mhasif78@gmail.com"));
        if (ccEmails != null)
        {
            if (ccEmails.Length > 0)
            {
                foreach (var ccEmail in ccEmails)
                {
                    if (!String.IsNullOrEmpty(ccEmail))
                    {
                        message.Cc.Add(new MailboxAddress("", ccEmail));
                    }
                }
            }
        }

        message.Subject = subject;
        // message.
        message.Body = new TextPart("html")
        {
            Text = mailBody
        };
        var client = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            client.Connect("smtp.zeptomail.in", 587, false);
            client.Authenticate("emailapikey", "PHtE6r1cRunsiW4u9RMBtvS9E8agNop8/LgyJVNOuI0UD/NWHU0HqIt6lz/k+R4vV6ZKRfWamotv57Odt+rTdjvlMjkfXmqyqK3sx/VYSPOZsbq6x00ctlodck3bXIHrdtZo0ifVuN7fNA==");
            client.Send(message);
            client.Disconnect(true);
            return true;
        }
        catch (Exception e)
        {
            return false;
            // Console.Write(e.Message);
        }

    }

    public static bool SentEmailByGMailSMTP(string subject, string mailBody, string? receiverMail = null, string? senderMail = null, AlternateView? mailbody = null, string? emailDisplayName = null, string[]? ccEmails = null)
    {
        try
        {
            if (senderMail == null || receiverMail == null || mailBody == null)
            {
                return false;
            }
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("simplihr97@gmail.com", "sccygsomfgtzmaro"),
                EnableSsl = true,
            };


            var mailMessage = new MailMessage
            {
                From = emailDisplayName == null ? new MailAddress(senderMail) : emailDisplayName.Trim() == "" ? new MailAddress(senderMail) : new MailAddress(senderMail, emailDisplayName),
                Subject = subject,
                IsBodyHtml = true,
            };
            mailMessage.AlternateViews.Add(mailbody);
            // mailMessage.Body = mailBody;
            mailMessage.To.Add(receiverMail);
            if (ccEmails != null)
            {
                if (ccEmails.Length > 0)
                {
                    foreach (var ccEmail in ccEmails)
                    {
                        if (!String.IsNullOrEmpty(ccEmail))
                        {
                            mailMessage.CC.Add(ccEmail);
                        }
                    }
                }
            }
            mailMessage.CC.Add("simplihr01@gmail.com");
            smtpClient.Send(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    public static bool SendMailByGupShapAPI(string subject, string mailBody, string? receiverMail = null, string? senderMail = null, AlternateView? mailbody = null, string? emailDisplayName = null, string[]? ccEmails = null)
    {
        string gupshupAPIURL = "https://enterprise.webaroo.com/GatewayAPI/rest";
        NameValueCollection reqParams = new NameValueCollection();
        reqParams.Add("userid", "2000700660");
        reqParams.Add("password", "mDTHV2");
        reqParams.Add("method", "EMS_POST_CAMPAIGN");
        reqParams.Add("v", "1.1");
        reqParams.Add("format", "json");
        reqParams.Add("name", "Team Simpli");
        reqParams.Add("recipients", receiverMail);
        if (ccEmails != null)
        {
            if (ccEmails.Length > 0)
            {
                foreach (var ccEmail in ccEmails)
                {
                    if (!String.IsNullOrEmpty(ccEmail))
                    {
                        reqParams.Add("recipients", ccEmail);
                    }
                }
            }
        }
        // reqParams.Add("recipients", "mohd.asif@kabirtechnocrats.com");
        reqParams.Add("content_type", "text/html");
        reqParams.Add("subject", subject);
        reqParams.Add("content", HttpUtility.UrlEncode(mailBody));
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        NameValueCollection fileParams = new NameValueCollection();


        string reqResponse = APIClient.UploadFilesToRemoteUrl(gupshupAPIURL, reqParams, fileParams);
        return true;
    }
}