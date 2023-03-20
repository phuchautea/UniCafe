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
    public class ManageOrderController : BaseController<Order>
    {
        // GET: Admin/ManageOrder
        public ActionResult Index()
        {
            var orders = GetAll().ToList();
            return View(orders);
        }
    }
}