using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Controllers;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageOrderController : BaseController<Order>
    {
        // GET: Admin/ManageOrder
        public ActionResult Index()
        {
            var orders = GetAll().ToList();
            return View(orders);
        }
        //--Create
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, Order order)
        {
            List<string> errors = new List<string>();
            try
            {
                var name = formCollection["name"];
                var phonenumber = formCollection["phonenumber"];
                var address = formCollection["address"];
                var payment = formCollection["payment"];
                var note = formCollection["note"];
                var status = formCollection["status"];
                if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên sản phẩm");
                }
                if (errors.Count == 0)
                {
                    Add(order);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;

            return RedirectToAction("Index", "ManageOrder");
        }
        //----EDIT
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction("Index", "ManageOrder");
            }
            var checkId = GetById(Id.Value);
            if (checkId == null)
            {
                return RedirectToAction("Index", "ManageOrder");
            }
            Order order = GetById(Id.Value);
            return View(order);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var name = formCollection["name"];
                var phonenumber = formCollection["phonenumber"];
                var address = formCollection["address"];
                var payment = formCollection["payment"];
                var note = formCollection["note"];
                var status = formCollection["status"];
                 if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Chưa nhập tên sản phẩm");
                }
                if (errors.Count == 0)
                {
                    var order = GetById(Int32.Parse(formCollection["Id"]));
                    order.Name = name;
                    order.PhoneNumber = phonenumber;
                    order.Address = address;
                    order.Payment = payment;
                    order.Note = note;
                    order.Status = status;
                    order.UpdatedAt = DateTime.Now;
                    Update(order);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index", "ManageOrder");
        }
        //---DELETE
        [HttpPost]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "ID không hợp lệ" });
            }
            Order order = GetById(Id.Value);
            if (order == null)
            {
                return Json(new { success = false, message = "Hóa đơn không tồn tại" });
            }
            Remove(order);
            return Json(new { success = true, message = "Xóa hóa đơn thành công" });
        }
    }
}