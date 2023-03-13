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
    public class ManageCategoryController : BaseController<Category>
    {
        // GET: Admin/ManageCategory
        public ActionResult Index()
        {
            var categories = GetAll().ToList();
            return View(categories);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, Category category)
        {
            List<string> errors = new List<string>();
            try
            {
                var name = formCollection["name"];
                var slug = formCollection["slug"];
                var description = formCollection["description"];
                var parentid = formCollection["parentid"];
                var status = formCollection["status"];
                var checkSlug = Context.Categories.Count(x => x.Slug == slug);
                if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên loại sản phẩm");
                }
                if (checkSlug > 0)
                {
                    errors.Add("Slug đã tồn tại");
                }
                if (string.IsNullOrEmpty(description))
                {
                    errors.Add("Chưa nhập mô tả loại sản phẩm");
                }
                if (errors.Count == 0)
                {
                    Add(category);
                }

            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;

            return RedirectToAction("Index", "ManageCategory");
        }
        //public ActionResult Edit(int? Id)
        //{
        //    if (Id == null)
        //    {
        //        return RedirectToAction("Index", "ManageCategory");
        //    }
        //    var checkId = GetById(Id.Value);
        //    if (checkId == null)
        //    {
        //        return RedirectToAction("Index", "ManageCategory");
        //    }
        //    var categories = _categoryRepository.GetAll().ToList();
        //    ViewBag.Categories = categories;
        //    Category category = GetById(Id.Value);
        //    return View(category);
        //}
        //[HttpPost]
        //public ActionResult Edit(FormCollection formCollection)
        //{
        //    List<string> errors = new List<string>();
        //    try
        //    {
        //        var id = formCollection["id"];
        //        var name = formCollection["name"];
        //        var slug = formCollection["slug"];
        //        var description = formCollection["description"];
        //        var status = formCollection["status"];
        //        var checkSlug = Context.Categories.Count(x => x.Slug == slug);
        //        var getCategoryContainsSlug = Context.Categories.FirstOrDefault(x => x.Slug == slug);
        //        if (string.IsNullOrEmpty(name))
        //        {
        //            errors.Add("Chưa nhập tên loại sản phẩm");
        //        }
        //        if (checkSlug > 0 && getCategoryContainsSlug.Id != Convert.ToInt32(id))
        //        {
        //            errors.Add("Slug đã tồn tại");
        //        }
        //        if (string.IsNullOrEmpty(description))
        //        {
        //            errors.Add("Chưa nhập mô tả loại sản phẩm");
        //        }
        //        if (errors.Count == 0)
        //        {
        //            var product = GetById(Int32.Parse(formCollection["Id"]));
        //            product.Name = name;
        //            product.Slug = slug;
        //            product.Description = description;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errors.Add(ex.Message);
        //    }
        //    TempData["Errors"] = errors;
        //    return RedirectToAction("Index", "ManageCategory");
        //}
        //[HttpPost]
        //public ActionResult Delete(int? Id)
        //{
        //    if (Id == null)
        //    {
        //        return Json(new { success = false, message = "ID không hợp lệ" });
        //    }
        //    Category category = GetById(Id.Value);
        //    if (category == null)
        //    {
        //        return Json(new { success = false, message = "Loại sản phẩm không tồn tại" });
        //    }
        //    Remove(category);
        //    return Json(new { success = true, message = "Xóa loại sản phẩm thành công" });
        //}
    }
}