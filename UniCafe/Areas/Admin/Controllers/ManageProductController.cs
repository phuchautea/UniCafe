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
    public class ManageProductController : BaseController<Product>
    {
        private readonly IRepository<Category> _categoryRepository;
        public ManageProductController()
        {
            _categoryRepository = new Repository<Category>(Context);
        }
        // GET: Admin/ManageProduct
        public ActionResult Index()
        {
            var categories = _categoryRepository.GetAll().ToList();
            ViewBag.Categories = categories;
            var listProduct = GetAll().ToList();
            return View(listProduct);
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, Product product)
        {
            List<string> errors = new List<string>();
            try
            {
                var name = formCollection["name"];
                var slug = formCollection["slug"];
                var price = formCollection["price"];
                var description = formCollection["description"];
                var image = formCollection["image"];
                var status = formCollection["status"];
                var checkSlug = Context.Products.Count(x => x.Slug == slug);
                if(string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên sản phẩm");
                }
                if (checkSlug > 0)
                {
                    errors.Add("Slug đã tồn tại");
                }
                if (string.IsNullOrEmpty(description))
                {
                    errors.Add("Chưa nhập mô tả sản phẩm");
                }
                if(Convert.ToDecimal(price) < 1000 || string.IsNullOrEmpty(price))
                {
                    errors.Add("Nhập giá ít nhất là 1000đ");
                }
                if (errors.Count == 0) {
                    var Category_Id = Int32.Parse(formCollection["category_id"]);
                    product.Category = _categoryRepository.GetById(Category_Id);
                    Add(product);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            
            return RedirectToAction("Index", "ManageProduct");
        }
        public ActionResult Edit(int? Id)
        {
            if(Id == null)
            {
                return RedirectToAction("Index", "ManageProduct");
            }
            var checkId = GetById(Id.Value);
            if(checkId == null)
            {
                return RedirectToAction("Index", "ManageProduct");
            }
            var categories = _categoryRepository.GetAll().ToList();
            ViewBag.Categories = categories;
            Product product = GetById(Id.Value);
            return View(product);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var id = formCollection["id"];
                var name = formCollection["name"];
                var slug = formCollection["slug"];
                var price = formCollection["price"];
                var description = formCollection["description"];
                var image = formCollection["image"];
                var status = formCollection["status"];
                var checkSlug = Context.Products.Count(x => x.Slug == slug);
                var getProductContainsSlug = Context.Products.FirstOrDefault(x => x.Slug == slug);
                if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên sản phẩm");
                }
                if (checkSlug > 0 && getProductContainsSlug.Id != Convert.ToInt32(id))
                {
                    errors.Add("Slug đã tồn tại");
                }
                if (string.IsNullOrEmpty(description))
                {
                    errors.Add("Chưa nhập mô tả sản phẩm");
                }
                if (Convert.ToDecimal(price) < 1000 || string.IsNullOrEmpty(price))
                {
                    errors.Add("Nhập giá ít nhất là 1000đ");
                }
                if (errors.Count == 0)
                {
                    var product = GetById(Int32.Parse(formCollection["Id"]));
                    product.Name = name;
                    product.Slug = slug;
                    product.Price = Convert.ToDecimal(price);
                    product.Description = description;
                    product.Image = image;
                    product.Status = status;
                    product.UpdatedAt = DateTime.Now;

                    var Category_Id = Int32.Parse(formCollection["category_id"]);
                    product.Category = _categoryRepository.GetById(Category_Id);
                    Update(product);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index", "ManageProduct");
        }
        [HttpPost]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "ID không hợp lệ" });
            }
            Product product = GetById(Id.Value);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }
            Remove(product);
            return Json(new { success = true, message = "Xóa sản phẩm thành công" });
        }
    }
}
//Areas