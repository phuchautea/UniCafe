using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class BlogController : MasterController<Blog>
    {
        // GET: Blog
        public ActionResult Index()
        {
            var b = GetAll().ToList();
            return View(b);
        }
        public ActionResult Readmore(int id) {
            var blog = GetById(id);
            if(blog == null || id==null) {
                return RedirectToAction("Index", "Blog");
            }
            return View(blog);
        }
    }
}