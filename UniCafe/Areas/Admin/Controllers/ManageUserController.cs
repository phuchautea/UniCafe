using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Controllers;
using UniCafe.Data;

namespace UniCafe.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUserController : BaseController<ApplicationUser>
    {
        // GET: Admin/ManageUser
        public ActionResult Index()
        {
            var users = GetAll().ToList();
            return View(users);
        }
    }
}