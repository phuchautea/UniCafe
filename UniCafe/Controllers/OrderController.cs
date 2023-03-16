using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Models;

namespace UniCafe.Controllers
{
    public class OrderController : BaseController<Order>
    {
        // GET: Order
        public ActionResult CheckOut()
        {
            return View();
        }
        [HttpPost]

        public ActionResult CheckOut(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var OrderCode = formCollection["OrderCode"];
                var Name = formCollection["Name"];
                var PhoneNumber = formCollection["PhoneNumber"];
                var Email = formCollection["Email"];
                var Address = formCollection["Address"];
                var Payment = formCollection["Payment"];

                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(Payment) || string.IsNullOrEmpty(OrderCode))
                {
                    errors.Add("Vui lòng nhập đầy đủ thông tin.");
                }

                if (errors.Count == 0)
                {

                    Order order = new Order();
                    order.OrderCode = OrderCode;
                    order.Name = Name;
                    order.PhoneNumber = PhoneNumber;
                    order.Email = Email;
                    order.Address = Address;
                    order.Payment = Payment;
                    Add(order);
                    return RedirectToAction("Index", "Home");

                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return View();
        }



        public ActionResult IndexSearchOrder()
        {
            return View();
        }

        private ApplicationDbContext _context;
        public ActionResult SearchOrder(string OrderCode)
        {
            _context = new ApplicationDbContext();
            return View(_context.Orders.FirstOrDefault(p => p.OrderCode == OrderCode));
        }
    }
}