using System.Net.Mail;
using System.Net;
using SimpliHR.Infrastructure.Models.Employee;
using Microsoft.AspNetCore.Components.Forms;
using crypto;
using SimpliHR.Infrastructure.Models.Login;
using SimpliHR.Infrastructure.Helper;
using System.Drawing;
using Newtonsoft.Json;
using SimpliHR.Infrastructure.Models.ClientManagement;
using SimpliHR.Core.Entities;

namespace SimpliHR.WebUI.BL
{
    public class Employee
    {
        private string EmailSubject(EmployeeEJoineeDTO employee, LoginDetailDTO loginDetails)
        {
            string subject = "<p><strong>Dear " + employee.FirstName + " " + employee.LastName + ",</strong></p>\r\n<p>This is with reference to your employment with&nbsp;<strong>" + employee.ClientKeyValue.CompanyName + "</strong>&nbsp;as&nbsp;<strong> " + employee.JobTitle + " </strong>&nbsp;with effect from&nbsp;<strong>" + (employee.Doj == null ? "" : ((DateTime)employee.Doj).ToString("dd/MM/yyyy")) + "</strong>.</p>\r\n<p>As part of joining formalities, you are requested to kindly fill up the online Employment Details form available on the link below:</p>\r\n<p><a target=\"_blank\" href=\"https://simplihrms.com/simplihr.newjoinee/" + CommonHelper.EncryptURLHTML(employee.EmployeeId.ToString()) + "\" rel=\"noopener\">https://simplihrms.com/simplihr.newjoinee/" + CommonHelper.EncryptURLHTML(employee.EmployeeId.ToString()) + "</a></p>\r\n<p>Your login details for filling up the form are as follows:</p>\r\n<ul>\r\n<li>User ID: " + loginDetails.UserName + "</li>\r\n<li>Password: " + loginDetails.Password + "</li>\r\n</ul>\r\n<p>We welcome you on board and wish you a long and prosperous association with us!</p>\r\n<p>Team &ndash; Human Resources</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact HR Team in case of any queries.</em></p>";
            return subject;
        }
        public bool SendJoiningLink(EmployeeEJoineeDTO employee, LoginDetailDTO loginDetails, UnitMasterDTO? unit)
        {
            try
            {
                string subject = "Online Submission Of Employment Details";
                string body = EmailSubject(employee, loginDetails);
                string receiverEmail = employee.EmailId == null ? "" : employee.EmailId;
                string senderEmail = "simplihr97@gmail.com";
                string emailDisplayName = unit == null ? "" : unit.EmailDisplayName == null ? "" : unit.EmailDisplayName.Trim();
               bool res = MailHelper.SendMail(subject, body, receiverEmail, senderEmail, null, emailDisplayName, employee.EmailProvider);
              //  bool res = true;
                return res;


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
                    Subject = "Online Submission Of Employment Details",
                    Body = EmailSubject(employee, loginDetails),
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(employee.EmailId);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string SendResetEmailLinkBody(EmployeeMasterDTO employee)
        {
            string b = @"<p>Hi " + employee.EmployeeName + "</p>\r\n<p>Below is the link to reset your password. Please make sure you do not share this email with any one.</p>\r\n<p><a href=\"https://simplihrms.com/simplihr.resetpassword/" + CommonHelper.EncryptURLHTML(employee.EmployeeId.ToString()) + "\">https://simplihrms.com/simplihr.resetpassword/" + CommonHelper.EncryptURLHTML(employee.EmployeeId.ToString()) + "</a></p>\r\n<p>Thanks and Regards</p>\r\n<p>SimpliHR Support Team</p>";
            //string b = @"<p>Hi " + employee.EmployeeName + "</p>\r\n<p>Below is the link to reset your password. Please make sure you do not share this email with any one.</p>\r\n<p><a href=\"https://localhost:7151/simplihr.resetpassword/" + CommonHelper.EncryptURLHTML(employee.EmployeeId.ToString()) + "\">https://simplihrms.com/simplihr.resetpassword/" + CommonHelper.EncryptURLHTML(employee.EmployeeId.ToString()) + "</a></p>\r\n<p>Thanks and Regards</p>\r\n<p>SimpliHR Support Team</p>";
            return b;
        }
        public bool SendResetEmailLink(EmployeeMasterDTO employee, UnitMasterDTO? unit)
        {
            try
            {
                string subject = "Password reset link";
                string body = SendResetEmailLinkBody(employee);
                string receiverEmail = employee.EmailId == null ? "" : employee.EmailId;
                string senderEmail = "simplihr97@gmail.com";
                string emailDisplayName = unit == null ? "" : unit.EmailDisplayName == null ? "" : unit.EmailDisplayName.Trim();
                bool res = MailHelper.SendMail(subject, body, receiverEmail, senderEmail, null, emailDisplayName, employee.EmailProvider);
                return res;
                //var smtpClient = new SmtpClient("smtp.gmail.com")
                //{
                //    Port = 587,
                //    //Credentials = new NetworkCredential("simplihr97@gmail.com", "simpli2pointo"),
                //    Credentials = new NetworkCredential("simplihr97@gmail.com", "sccygsomfgtzmaro"),
                //    EnableSsl = true,
                //};
                //var mailMessage = new MailMessage
                //{
                //    From = new MailAddress("simplihr97@gmail.com"),
                //    Subject = "Password reset link",
                //    Body = SendResetEmailLinkBody(employee),
                //    IsBodyHtml = true,
                //};
                //mailMessage.To.Add(employee.EmailId);
                //smtpClient.Send(mailMessage);
                //return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string SendChangePwdEmailBody(LoginDetailDTO loginDetails)
        {
            string subject = "<p><strong>Dear User,</strong></p>\r\n<p>This is with reference to your employment with&nbsp;</p>\r\n<p>Your new login details as follows:</p>\r\n<ul>\r\n<li>User ID: " + loginDetails.UserName + "</li>\r\n<li>Password: " + loginDetails.Password + "</li>\r\n</ul>\r\n<p>We welcome you on board and wish you a long and prosperous association with us!</p>\r\n<p>Team &ndash; Human Resources</p>\r\n<p><em>This is an auto-generated e-mail. Kindly contact HR Team in case of any queries.</em></p>";
            return subject;
        }
        public bool SendChangePwdEmailLink(LoginDetailDTO employee, UnitMasterDTO? unit,int? EmailProvider=0)
        {
            try
            {
                //employee.UserName = "mhasif78@gmail.com";

                string subject = "Change Password";
                string body = SendChangePwdEmailBody(employee);
                string receiverEmail = employee.UserName == null ? "" : employee.UserName;
                string senderEmail = "simplihr97@gmail.com";
                string emailDisplayName = unit == null ? "" : unit.EmailDisplayName == null ? "" : unit.EmailDisplayName.Trim();
                bool res = MailHelper.SendMail(subject, body, receiverEmail, senderEmail, null, emailDisplayName, EmailProvider);
                return res;
                //var smtpClient = new SmtpClient("smtp.gmail.com")
                //{
                //    Port = 587,
                //    //Credentials = new NetworkCredential("simplihr97@gmail.com", "simpli2pointo"),
                //    Credentials = new NetworkCredential("simplihr97@gmail.com", "sccygsomfgtzmaro"),
                //    EnableSsl = true,
                //};
                //var mailMessage = new MailMessage
                //{
                //    From = new MailAddress("simplihr97@gmail.com"),
                //    Subject = "Password reset link",
                //    Body = SendResetEmailLinkBody(employee),
                //    IsBodyHtml = true,
                //};
                //mailMessage.To.Add(employee.EmailId);
                //smtpClient.Send(mailMessage);
                //return true;
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
