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
    public class EmailsModel : PageModel
    {
        private const int PAGESIZE = 10;

        public IActionResult OnGet()
        {
            string admin_id = HttpContext.Session.GetString("admin_id");
            if (admin_id == null || admin_id == "")
            {
                return Redirect("/Admin?err=1");
            }
            string emailId = Request.Query["xemailid"];
            string spage = Request.Query["p"];
            int page = 0;
            if (!int.TryParse(spage, out page))
                page = 0;
            ViewData["emailId"] = emailId;
            ViewData["p"] = page;
            string conn = ConfigurationManager.AppSetting["ConnectionStrings:connString"];
            DBRoutines dbr = new DBRoutines(conn);
            long count = dbr.getCount(emailId);
            ViewData["count"] = count;
            int pages = 0;
            long startPoint = 0;
            long maxLinks = 0;
            string nextSetFrom = null;
            StringBuilder pageLinks = new StringBuilder("");
            long pageCount = count / PAGESIZE;
            if ((pageCount * PAGESIZE) < count)
                pageCount++;
            if (pageCount > 1)
            {
                if (pageCount < 20)
                {// max links per page
                    maxLinks = pageCount;
                    startPoint = 1;
                } else
                {
                    startPoint = ((page / 20) * 20) + 1;
                    if (startPoint < 1)
                        startPoint = 1;
                    maxLinks = startPoint + 20;
                    if (maxLinks > pageCount)
                    {
                        maxLinks = pageCount;
                        nextSetFrom = null;
                    } else
                    {
                        nextSetFrom = maxLinks.ToString();
                    }

                }

                if (page >= 20)
                {
                    pageLinks.Append("<button type='button' class='btn btn-default'  onclick=\"doPaging(" + (startPoint - 20).ToString() + ");\">" + "<< Prev " + "20" + " pages</button>&nbsp;");
                }

                for (long i = startPoint; i <= maxLinks; i++)
                {
                    if (i == page)
                    {
                        pageLinks.Append("<button type='button' class='btn btn-primary' onclick=\"doPaging(" + i.ToString() + ");\">" + i.ToString() + "</button>&nbsp;");
                    } else
                    {
                        pageLinks.Append("<button type='button' class='btn btn-default'  onclick=\"doPaging(" + i.ToString() + ");\">" + i.ToString() + "</button>&nbsp;");

                    }
                }

                if (nextSetFrom != null)
                {
                    pageLinks.Append("<button type='button' class='btn btn-default'  onclick=\"doPaging(" + nextSetFrom + ");\">" + "Next " + "20" + " pages >></button>&nbsp;");
                }
            }

            ViewData["pageLinks"] = pageLinks.ToString();

            int startRec = 0;
            if (page > 0)
                    startRec = PAGESIZE * (page - 1);
            DataTable dt = dbr.getList(emailId, startRec, PAGESIZE);

            ViewData["data"] = dt;

            return Page();
        }
    }
}