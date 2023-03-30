using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniCafe.Areas.Admin.Controllers
{
    public class ManageBlogController : Controller
    {
        // GET: Admin/ManageBlog
        public ActionResult Index()
        {
            return View();
        }
    }
}