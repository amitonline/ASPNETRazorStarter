using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace TestRazor.Pages.admin
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            string error = Request.Query["err"];
            if (error != null && error == "1")
            {
                ViewData["error"] = "Your session is invalid or has timed out";
            }

            
        }

        public IActionResult OnPost()
        {
            string admin = ""; string pwd = "";
            admin = Request.Form["userid"];
            pwd = Request.Form["pwd"];

            if (admin != "admin@mailinglist" && pwd != "leonidas")
            {
                ViewData["error"] = "Invalid login";
                return Page();
            }
            HttpContext.Session.SetString("admin_id", "1");
            return Redirect("/admin/dashboard");

        }
    }
}