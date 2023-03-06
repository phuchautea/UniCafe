using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class UserController : MasterController<ApplicationUser>
    {
        // GET: User
        public ActionResult Index()
        {
            var users = GetAll().ToList();
            return View(users);
        }
    }
}