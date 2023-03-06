using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class PropertyProductController : MasterController<PropertyProduct>
    {
        private readonly IRepository<Product> _productRepository;
        public PropertyProductController()
        {
            _productRepository = new Repository<Product>(Context);
        }
        // GET: PropertyProduct
        public ActionResult Index()
        {
            var PropertyProducts = GetAll().ToList();
            var Products = _productRepository.GetAll().ToList();
            ViewBag.Products = Products;
            return View(PropertyProducts);
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, PropertyProduct propertyProduct)
        {
            List<string> errors = new List<string>();
            try
            {
                var Product_Id = Int32.Parse(formCollection["Product_Id"]);
                var product = _productRepository.GetById(Product_Id);
                propertyProduct.Product = product;
                Add(propertyProduct);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index", "PropertyProduct");
        }
        public ActionResult Edit(int Id)
        {
            PropertyProduct propertyProduct = GetById(Id);
            var products = _productRepository.GetAll().ToList();
            ViewBag.Products = products;
            return View(propertyProduct);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {
            var propertyProduct = GetById(Int32.Parse(formCollection["Id"]));
            propertyProduct.Name = formCollection["Name"];
            propertyProduct.Slug = formCollection["Slug"];
            propertyProduct.Price = Decimal.Parse(formCollection["Price"]);
            propertyProduct.Status = formCollection["Status"];
            propertyProduct.UpdatedAt = DateTime.Now;
            var Product_Id = Int32.Parse(formCollection["Product_Id"]);
            Product product = _productRepository.GetById(Product_Id);
            propertyProduct.Product = product;
            Update(propertyProduct);
            return RedirectToAction("Index", "PropertyProduct");
        }
        public ActionResult Delete(int Id)
        {
            PropertyProduct propertyProduct = GetById(Id);
            return View(propertyProduct);
        }
        [HttpPost]
        public ActionResult Delete(PropertyProduct propertyProduct)
        {
            Remove(propertyProduct);
            return RedirectToAction("Index", "PropertyProduct");
        }
    }
}
