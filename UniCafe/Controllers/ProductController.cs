using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;
using System.Runtime.Caching;

namespace UniCafe.Controllers
{
    public class ProductController : MasterController<Product>
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<PropertyProduct> _propertyProductRepositoy;
        private readonly IRepository<OptionProduct> _optionProductRepositoy;
        public ProductController()
        {
            _categoryRepository = new Repository<Category>(Context);
            _propertyProductRepositoy = new Repository<PropertyProduct>(Context);
            _optionProductRepositoy = new Repository<OptionProduct>(Context);
        }
        /// <summary>
        ///  MEMORY CACHE
        /// </summary>
        /// <returns></returns>
        private static MemoryCache _productCache = new MemoryCache("ProductCache");
        public List<Product> GetProducts()
        {
            var cacheKey = "ProductCache";
            var productList = _productCache.Get(cacheKey) as List<Product>;

            if (productList == null)
            {
                // Nếu danh sách sản phẩm chưa có trong cache, truy vấn từ database và lưu vào cache
                productList = GetAll().ToList(); // lấy danh sách sản phẩm từ database
                //productList = Context.Products.Where(s => s.Name == "123").ToList();
                var cachePolicy = new CacheItemPolicy();
                cachePolicy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1); // Thời gian sống của cache: 1 phút
                _productCache.Set(cacheKey, productList, cachePolicy);
            }
            return productList;
        }
        public ActionResult Index()
        {
            //var p = GetProducts();
            var p = GetAll().ToList();
            var c = _categoryRepository.GetAll().ToList();
            ViewBag.Categories = c;
            ViewBag.PropertyProducts = _propertyProductRepositoy.GetAll().ToList();
            ViewBag.OptionProducts = _optionProductRepositoy.GetAll().ToList();
            return View(p);
        }
        [Route("Product/{Slug}")]
        public ActionResult Details(string Slug)
        {
            try
            {
                ViewBag.PropertyProducts = Context.PropertityProducts.Where(x => x.Product.Slug == Slug).ToList();
                ViewBag.OptionProducts = Context.OptionProducts.Where(x => x.Product.Slug == Slug).ToList();
                var product = Context.Products.FirstOrDefault(x => x.Slug == Slug);
                return View(product);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Product");
            }
            return RedirectToAction("Index", "Product");
        }
    }
}