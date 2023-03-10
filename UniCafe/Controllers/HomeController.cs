using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        public HomeController()
        {
            _context = new ApplicationDbContext();
            _categoryRepository = new Repository<Category>(_context);
            _productRepository = new Repository<Product>(_context);
        }
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        //[Route("Collection/{id}")]
        public ActionResult Collection(string id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Slug == id);
            var products = _context.Products.Where(c => c.Category.Id == category.Id).ToList();
            ViewBag.Products = products;
            ViewBag.Category = category;
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}