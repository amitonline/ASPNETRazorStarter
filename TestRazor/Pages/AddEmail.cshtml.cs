using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Text;

namespace TestRazor.Pages
{
    public class AddEmailModel : PageModel
    {
        public JsonResult OnPost()
        {
            string retVal = "";
            string email = Request.Form["email"];
            if (email == null || email == "")
            {
                return new JsonResult("Error - invalid email id");
            }

            retVal = "Error - login failed!";
            string conn = ConfigurationManager.AppSetting["ConnectionStrings:connString"];
            DBRoutines dbr = new DBRoutines(conn);
            DataTable dt = dbr.getRowByEmailId(email);
            if (dt == null || dt.Rows.Count == 0)
            {
                EmailData ed = new EmailData();
                ed.email = email;
                ed.name = "";
                ed.vkey = randomDigits(6);
                ed.signup = DateTime.Now;
                ed.verified = 0;

                bool status = dbr.addRow(ed);
                if (!status)
                {
                    retVal = dbr.getError();
                } else
                {
                    // send verification email
                    string link = "http://mlist.test/verify?code=" + ed.vkey;
                    string subject = "Mailing List - Signup Verification";
                    string content = System.IO.File.ReadAllText("mails/verification.html");
                    content = content.Replace("#name#", "Subscriber");
                    content = content.Replace("#verificationlink#", link);


                    string to = ed.email; //To address    
                    string from = "support@mediawarrior.com"; //From address    
                    string smtpServer = ConfigurationManager.AppSetting["smtpServer"];
                    string smtpPort = ConfigurationManager.AppSetting["smtpPort"];
                    string smtpUserId = ConfigurationManager.AppSetting["smtpUserId"];
                    string smtpPwd = ConfigurationManager.AppSetting["smtpPwd"];
                    MailMessage message = new MailMessage(from, to);

                    string mailbody = content;
                    message.Subject = subject;
                    message.Body = mailbody;
                    message.BodyEncoding = Encoding.UTF8;
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", int.Parse(smtpPort)); 
                    System.Net.NetworkCredential basicCredential1 = new  System.Net.NetworkCredential(smtpUserId, smtpPwd);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = basicCredential1;
                    try
                    {
                        client.Send(message);
                        retVal = "";
                    }

                    catch (Exception ex)
                    {
                        throw ex;
                        retVal = ex.Message;
                    }

                }
            } else
            {
                return new JsonResult("Error - This email id is already registered.");

            }
            return new JsonResult(retVal);
        }

        private string randomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }
    }


}