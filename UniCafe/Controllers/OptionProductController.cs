using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class OptionProductController : MasterController<OptionProduct>
    {
        private readonly IRepository<Product> _productRepository;
        public OptionProductController()
        {
            _productRepository = new Repository<Product>(Context);
        }
        // GET: OptionProduct
        public ActionResult Index()
        {
            var OptionProducts = GetAll().ToList();
            var Products = _productRepository.GetAll().ToList();
            ViewBag.Products = Products;
            return View(OptionProducts);
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, OptionProduct optionProduct) {
            List<string> errors = new List<string>();
            try
            {
                var Product_Id = Int32.Parse(formCollection["Product_Id"]);
                var product = _productRepository.GetById(Product_Id);
                optionProduct.Product = product;
                Add(optionProduct);
            }
            catch(Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index", "OptionProduct");
        }
        public ActionResult Edit(int Id)
        {
            OptionProduct optionProduct = GetById(Id);
            var products = _productRepository.GetAll().ToList();
            ViewBag.Products = products;
            return View(optionProduct);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {
            var optionProduct = GetById(Int32.Parse(formCollection["Id"]));
            optionProduct.Name = formCollection["Name"];
            optionProduct.Slug = formCollection["Slug"];
            optionProduct.Price = Decimal.Parse(formCollection["Price"]);
            optionProduct.Status = formCollection["Status"];
            optionProduct.UpdatedAt = DateTime.Now;
            var Product_Id = Int32.Parse(formCollection["Product_Id"]);
            Product product = _productRepository.GetById(Product_Id);
            optionProduct.Product = product;
            Update(optionProduct);
            return RedirectToAction("Index", "OptionProduct");
        }
        public ActionResult Delete(int Id)
        {
            OptionProduct optionRroduct = GetById(Id);
            return View(optionRroduct);
        }
        [HttpPost]
        public ActionResult Delete(OptionProduct optionRroduct)
        {
            Remove(optionRroduct);
            return RedirectToAction("Index", "OptionProduct");
        }
    }
}
