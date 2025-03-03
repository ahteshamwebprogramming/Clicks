using SimpliHR.Infrastructure.Helper;
using SimpliHR.Infrastructure.Models.Login;
using System.Net.Mail;
using System.Net;
using SimpliHR.Infrastructure.Models.ClientManagement;

namespace SimpliHR.WebUI.BL
{
    public class ClientMail
    {
        private string EmailSubject(UnitMasterDTO client, LoginDetailDTO loginDetails)
        {
            string subject = "<p><strong>Dear " + client.ClientName + ",</strong></p>\r\n<p>This is with reference to created admin of&nbsp;<strong>" + client.UnitName + "</strong>  you are requested to kindly fill up the own Employment Details form available on the link below:</p>\r\n<p><a target=\"_blank\" href=\"https://simplihr2.azurewebsites.net/\" rel=\"noopener\">https://simplihr2.azurewebsites.net/</a></p>\r\n<p>Your login details for filling up the form are as follows:</p>\r\n<ul>\r\n<li>User ID: " + loginDetails.UserName + "</li>\r\n<li>Password: " + CommonHelper.Decrypt(loginDetails.Password) + "</li>\r\n</ul>\r\n<p>We welcome " + client.UnitName + " on board and wish a long and prosperous association with us!</p>\r\n<p>Team &ndash; SimplyHR</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact the Team in case of any queries.</em></p>";
            return subject;
        }
        public bool SendJoiningLink(UnitMasterDTO client, LoginDetailDTO loginDetails)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    //Credentials = new NetworkCredential("simplihr97@gmail.com", "simpli2pointo"),
                    Credentials = new NetworkCredential("simplihr97@gmail.com", "sccygsomfgtzmaro"),
                    EnableSsl = true,
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("simplihr97@gmail.com"),
                    Subject = "Client admin login details",
                    Body = EmailSubject(client, loginDetails),
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(client.EmailId);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string RandomString()
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
    }
}
