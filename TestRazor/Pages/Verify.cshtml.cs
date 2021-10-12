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
    public class VerifyModel : PageModel
    {
        public void OnGet()
        {
            string retVal = "";
            string code = Request.Query["code"];
            if (code == null || code == "")
            {
                ViewData["error"] = "Error - missing verification code";
                return;
            }

            string conn = ConfigurationManager.AppSetting["ConnectionStrings:connString"];
            DBRoutines dbr = new DBRoutines(conn);
            DataTable dt = dbr.getRowByVerifyCode(code);
            if (dt != null && dt.Rows.Count > 0 && (Int16) dt.Rows[0]["verified"] != 1)
            {
                int id = (int) dt.Rows[0]["ID"];
                bool status = dbr.verifyAccount(id);
                if (!status)
                {
                    retVal = dbr.getError();
                }
                else
                {
                    // send confirmation email
                    string subject = "Mailing List - Signup Confirmation";
                    string content = System.IO.File.ReadAllText("mails/confirmation.html");
                    content = content.Replace("#name#", "Subscriber");


                    string to = (string) (string) dt.Rows[0]["email"]; //To address    
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
                    System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential(smtpUserId, smtpPwd);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = basicCredential1;
                    try
                    {
                        //client.Send(message);
                        retVal = "";
                        ViewData["msg"] = "Your account has been verified and a confirmation email sent!";
                    }
                    catch (Exception ex)
                    {
                        ViewData["error"] = ex.Message;
                    }

                }
            }
            else
            {
                ViewData["error"]= "Error - This verification code is invalid or already used.";

            }
             
        }
    }
}