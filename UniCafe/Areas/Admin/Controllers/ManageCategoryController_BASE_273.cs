using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Controllers;
using UniCafe.Models;

namespace UniCafe.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageCategoryController : BaseController<Category>
    {
        // GET: Admin/ManageCategory
        public ActionResult Index()
        {
            var categories = GetAll().ToList();
            return View(categories);
        }
    }
}