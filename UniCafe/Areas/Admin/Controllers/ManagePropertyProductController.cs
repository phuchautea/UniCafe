using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using UniCafe.Controllers;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagePropertyProductController : BaseController<PropertyProduct>
    {
        private readonly IRepository<Product> _productRepository;
        public ManagePropertyProductController()
        {
            _productRepository = new Repository<Product>(Context);
        }
        // GET: Admin/ManagePropertyProduct

        public ActionResult Index()
        {
            var product = _productRepository.GetAll().ToList();
            ViewBag.Product = product;
            var listPropertyProduct = GetAll().ToList();
            return View(listPropertyProduct);
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, PropertyProduct propertyProduct)
        {
            List<string> errors = new List<string>();
            try
            {
                var name = formCollection["name"];
                //var slug = formCollection["slug"];
                var price = formCollection["price"];
                var status = formCollection["status"];
                //var checkSlug = Context.PropertyProducts.Count(x => x.Slug == slug);
                if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên thuộc tính");
                }
                //if (checkSlug > 0)
                //{
                //    errors.Add("Slug đã tồn tại");
                //}
                if (Convert.ToDecimal(price) < 0 || string.IsNullOrEmpty(price))
                {
                    errors.Add("Nhập giá ít nhất là 0đ");
                }
                if (errors.Count == 0)
                {
                    var Product_Id = Int32.Parse(formCollection["product_id"]);
                    propertyProduct.Product = _productRepository.GetById(Product_Id);
                    Add(propertyProduct);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;

            return RedirectToAction("Index", "ManagePropertyProduct");
        }
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction("Index", "ManagePropertyProduct");
            }
            var checkId = GetById(Id.Value);
            if (checkId == null)
            {
                return RedirectToAction("Index", "ManagePropertyProduct");
            }
            var product = _productRepository.GetAll().ToList();
            ViewBag.Product = product;
            PropertyProduct propertyProduct = GetById(Id.Value);
            return View(propertyProduct);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var id = formCollection["id"];
                var name = formCollection["name"];
                //var slug = formCollection["slug"];
                var price = formCollection["price"];
                var status = formCollection["status"];
                //var checkSlug = Context.Products.Count(x => x.Slug == slug);
                //var getPropertyProductContainsSlug = Context.Products.FirstOrDefault(x => x.Slug == slug);
                if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên thuộc tính");
                }
                //if (checkSlug > 0 && getPropertyProductContainsSlug.Id != Convert.ToInt32(id))
                //{
                //    errors.Add("Slug đã tồn tại");
                //}
                if (Convert.ToDecimal(price) < 1000 || string.IsNullOrEmpty(price))
                {
                    errors.Add("Nhập giá ít nhất là 1000đ");
                }
                if (errors.Count == 0)
                {
                    var product = _productRepository.GetById(Int32.Parse(formCollection["product_id"]));
                    var propertyProduct = GetById(Int32.Parse(formCollection["Id"]));
                    propertyProduct.Name = name;
                    propertyProduct.Product = product;
                    //propertyProduct.Slug = slug;
                    propertyProduct.Price = Convert.ToDecimal(price); // ép kiểu từ string về decimal
                    propertyProduct.Status = status;
                    propertyProduct.UpdatedAt = DateTime.Now;
                    Update(propertyProduct);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index", "ManagePropertyProduct");
        }
        [HttpPost]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "ID không hợp lệ" });
            }
            PropertyProduct propertyProduct = GetById(Id.Value);
            if (propertyProduct == null)
            {
                return Json(new { success = false, message = "Thuộc tính không tồn tại" });
            }
            Remove(propertyProduct);
            return Json(new { success = true, message = "Xóa thuộc tính thành công" });
        }
    }
}