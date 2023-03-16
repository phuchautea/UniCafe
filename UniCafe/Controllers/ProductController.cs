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
        private readonly IRepository<OptionProduct> _optionProductRepository;

        public ProductController()
        {
            _categoryRepository = new Repository<Category>(Context);
            _optionProductRepository = new Repository<OptionProduct>(Context);
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
            return View(p);
        }
        //public ActionResult Details(int Id) {
        //    var Product = GetById(Id);
        //    var c = _optionProductRepository.GetAll().ToList();
        //    c = Context.OptionProducts.Where(p => p.Product.Id == Id).ToList();
        //    ViewBag.optionProducts = c;
        //    return View(Product);
        //}
    }
}