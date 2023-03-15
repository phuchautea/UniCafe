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
    public class ManageOptionProductController : BaseController<OptionProduct>
    {
        private readonly IRepository<Product> _productRepository;
        public ManageOptionProductController()
        {
            _productRepository = new Repository<Product>(Context);
        }
        // GET: Admin/ManageOptionProduct

        public ActionResult Index()
        {
            var product = _productRepository.GetAll().ToList();
            ViewBag.Product = product;
            var listOptionProduct = GetAll().ToList();
            return View(listOptionProduct);
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, OptionProduct optionProduct)
        {
            List<string> errors = new List<string>();
            try
            {
                var name = formCollection["name"];
                //var slug = formCollection["slug"];
                var price = formCollection["price"];
                var status = formCollection["status"];
                //var checkSlug = Context.OptionProducts.Count(x => x.Slug == slug);
                if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên tùy chọn");
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
                    optionProduct.Product = _productRepository.GetById(Product_Id);
                    Add(optionProduct);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;

            return RedirectToAction("Index", "ManageOptionProduct");
        }
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction("Index", "ManageOptionProduct");
            }
            var checkId = GetById(Id.Value);
            if (checkId == null)
            {
                return RedirectToAction("Index", "ManageOptionProduct");
            }
            var product = _productRepository.GetAll().ToList();
            ViewBag.Product = product;
            OptionProduct optionProduct = GetById(Id.Value);
            return View(optionProduct);
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
                //var getOptionProductContainsSlug = Context.Products.FirstOrDefault(x => x.Slug == slug);
                if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên danh mục");
                }
                //if (checkSlug > 0 && getOptionProductContainsSlug.Id != Convert.ToInt32(id))
                //{
                //    errors.Add("Slug đã tồn tại");
                //}
                if (Convert.ToDecimal(price) < 1000 || string.IsNullOrEmpty(price))
                {
                    errors.Add("Nhập giá ít nhất là 1000đ");
                }
                if (errors.Count == 0)
                {
                    var optionProduct = GetById(Int32.Parse(formCollection["Id"]));
                    optionProduct.Name = name;
                    //optionProduct.Slug = slug;
                    optionProduct.Price = Convert.ToDecimal(price); // ép kiểu từ string về decimal
                    optionProduct.Status = status;
                    optionProduct.UpdatedAt = DateTime.Now;
                    Update(optionProduct);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index", "ManageOptionProduct");
        }
        [HttpPost]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "ID không hợp lệ" });
            }
            OptionProduct optionProduct = GetById(Id.Value);
            if (optionProduct == null)
            {
                return Json(new { success = false, message = "Tùy chọn không tồn tại" });
            }
            Remove(optionProduct);
            return Json(new { success = true, message = "Xóa tùy chọn thành công" });
        }
    }
}