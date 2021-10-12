using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace TestRazor.Pages.admin
{
    public class DashboardModel : PageModel
    {
        public IActionResult OnGet()
        {
            string admin_id = HttpContext.Session.GetString("admin_id");
            if (admin_id == null || admin_id == "")
            {
                return Redirect("/Admin?err=1");
            }
            return Page();
        }
    }
}