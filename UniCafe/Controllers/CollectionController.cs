using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    //public class CollectionController : Controller
    //{
    //    private readonly ApplicationDbContext _context;
    //    private readonly IRepository<Category> _categoryRepository;
    //    private readonly IRepository<Product> _productRepository;
    //    public CollectionController()
    //    {
    //        _context = new ApplicationDbContext();
    //        _categoryRepository = new Repository<Category>(_context);
    //        _productRepository = new Repository<Product>(_context);
    //    }
    //    GET: Collection
    //   [Route("Collection/{Slug}")]
    //    public ActionResult Index(string Slug)
    //    {
    //        var category = _context.Categories.FirstOrDefault(c => c.Slug == Slug);
    //        var products = _context.Products.Where(c => c.Category.Id == category.Id).ToList();
    //        ViewBag.Products = products;
    //        ViewBag.Category = category;
    //        return View();
    //    }
    //}
}