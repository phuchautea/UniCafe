using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Controllers;
using UniCafe.Models;

namespace UniCafe.Areas.Admin.Controllers
{
    public class ManageBlogController : BaseController<Blog>
    {
        // GET: Admin/ManageBlog
        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var blogs = GetAll().ToList();
            return View(blogs);
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, Blog blog)
        {
            List<string> errors = new List<string>();
            try
            {
                var Title = formCollection["Title"];
                var Content = formCollection["Content"];
                var CreatedDate = DateTime.Now;
                var Published = formCollection["Published"];
                var Author = formCollection["Author"];
                var Image = formCollection["Image"];
                if (string.IsNullOrEmpty(Title))
                {
                    errors.Add("Chưa nhập tiêu đề của Blog");
                }
                if (string.IsNullOrEmpty(Author))
                {
                    errors.Add("Tác giả không được để trống");
                }
                if (errors.Count == 0)
                {
                    Add(blog);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;

            return RedirectToAction("Index", "ManageBlog");
        }
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction("Index", "ManageBlog");
            }
            var checkId = GetById(Id.Value);
            if (checkId == null)
            {
                return RedirectToAction("Index", "ManageBlog");
            }
            Blog blog = GetById(Id.Value);
            return View(blog);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var  Title = formCollection["Title"];
                var Content = formCollection["Content"];
                var Published = true;
                if (formCollection["Published"] == null)
                {

                     Published = false;
                }
                var Author = formCollection["Author"];
                var Image = formCollection["Image"];
                if (string.IsNullOrEmpty(Title))
                {
                    errors.Add("Chưa nhập tiêu đề");
                }
                if (string.IsNullOrEmpty(Content))
                {
                    errors.Add("Chưa nhập nội dung sản phẩm");
                }
                if (errors.Count == 0)
                {
                    var blog = GetById(Int32.Parse(formCollection["Id"]));
                    blog.Title = Title;
                    blog.Content = Content;
                    blog.Published = Published;
                    blog.Author = Author;
                    blog.Image = Image;
                    Update(blog);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index", "ManageBlog");
        }
        [HttpPost]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "ID không hợp lệ" });
            }
            Blog blog = GetById(Id.Value);
            if (blog == null)
            {
                return Json(new { success = false, message = "Blog không tồn tại" });
            }
            Remove(blog);
            return Json(new { success = true, message = "Xóa thành công" });
        }
    }
}
