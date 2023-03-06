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
        //private readonly ApplicationDbContext _context;
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ProductController()
        {
            //_context = new ApplicationDbContext();
            //_unitOfWork = new UnitOfWork(_context);
            //_productRepository = new Repository<Product>(_context);
            _categoryRepository = new Repository<Category>(Context);
        }
        //public void AddProduct(Product product)
        //{
        //    _productRepository.Add(product);
        //    _unitOfWork.BeginTransaction();
        //    try
        //    {
        //        _unitOfWork.Commit();
        //    }
        //    catch
        //    {
        //        _unitOfWork.Rollback();
        //        throw;
        //    }
        //}
        //public IEnumerable<Product> GetAllProducts()
        //{
        //    return _productRepository.GetAll();
        //}
        //public IEnumerable<Category> GetAllCategories()
        //{
        //    return _categoryRepository.GetAll();
        //}
        //public Category GetCategoryById(int Id)
        //{
        //    return _categoryRepository.GetById(Id);
        //}
        //public Product GetProductById(int Id)
        //{
        //    return _productRepository.GetById(Id);
        //}

        //public void UpdateProduct(Product Product)
        //{
        //    _productRepository.Update(Product);

        //    _unitOfWork.BeginTransaction();
        //    try
        //    {
        //        _unitOfWork.Commit();
        //    }
        //    catch
        //    {
        //        _unitOfWork.Rollback();
        //        throw;
        //    }
        //}

        //public void DeleteProduct(Product product)
        //{
        //    _productRepository.Remove(product);
        //    _unitOfWork.BeginTransaction();
        //    try
        //    {
        //        _unitOfWork.Commit();
        //    }
        //    catch
        //    {
        //        _unitOfWork.Rollback();
        //        throw;
        //    }
        //}


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
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, Product product)
        {
            var Category_Id = Int32.Parse(formCollection["Category_Id"]);
            product.Category = _categoryRepository.GetById(Category_Id);
            Add(product);
            return RedirectToAction("Index", "Product");
        }
        public ActionResult Edit(int Id)
        {
            Product p = GetById(Id);
            var c = _categoryRepository.GetAll().ToList();
            ViewBag.Categories = c;
            return View(p);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {
            var product = GetById(Int32.Parse(formCollection["Id"]));
            product.Slug = formCollection["Slug"];
            product.Name = formCollection["Name"];
            product.Image = formCollection["Image"];
            product.Status = formCollection["Status"];
            product.UpdatedAt = DateTime.Now;
            product.Description = formCollection["Description"];
            var Category_Id = Int32.Parse(formCollection["Category_Id"]);
            Category category = _categoryRepository.GetById(Category_Id);
            product.Category = category;
            Update(product);
            return RedirectToAction("Index", "Product");
        }
        public ActionResult Delete(int Id)
        {
            Product product = GetById(Id);
            return View(product);
        }
        [HttpPost]
        public ActionResult Delete(Product product)
        {
            Remove(product);
            return RedirectToAction("Index", "Product");
        }
    }
}