using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Data;
using UniCafe.Models;
using UniCafe.Services;

namespace UniCafe.Controllers
{
    public class OrderController : BaseController<Order>
    {
        private readonly BaseController<OrderDetail> _orderDetailBase;
        private readonly CartManager _cartManager;
        public OrderController() {
            _orderDetailBase = new BaseController<OrderDetail>();
            _cartManager = new CartManager();
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray()).ToUpper();
        }
        // GET: Order
        public ActionResult CheckOut()
        {
            var cart = _cartManager.GetCartItems();
            ViewBag.Carts = cart;
            return View();
        }
        [HttpPost]
        public ActionResult CheckOut(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var name = formCollection["name"];
                var phoneNumber = formCollection["phoneNumber"];
                var address = formCollection["address"];
                var payment = formCollection["payment"];
                var note = formCollection["note"];

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(payment))
                {
                    errors.Add("Vui lòng nhập đầy đủ thông tin.");
                }

                if (errors.Count == 0)
                {
                    Order order = new Order();
                    
                    string code = RandomString(12);
                    order.Code = code;
                    order.Name = name;
                    order.PhoneNumber = phoneNumber;
                    order.Address = address;
                    order.Payment = payment;
                    order.Note = note;
                    Add(order);
                    TempData["orderCode"] = code;
                    var cart = _cartManager.GetCartItems();
                    var orderAdded = Context.Orders.FirstOrDefault(x => x.Code == code);
                    foreach (var item in cart) {
                        try
                        {
                            string propertyProduct = "" + item.PropertyProduct.Name + " - " + item.PropertyProduct.Price.ToString("N0") + "đ";
                            string optionProduct = "";
                            foreach (var option in item.Options)
                            {
                                optionProduct += "" + option.Name + " - " + option.Price.ToString("N0") + "đ\n";
                            }

                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.Order = orderAdded;
                            orderDetail.ProductId = item.ProductId;
                            orderDetail.ProductName = item.ProductName;
                            orderDetail.Price = item.Price;
                            orderDetail.Quantity = item.Quantity;
                            orderDetail.PropertyProduct = propertyProduct;
                            orderDetail.OptionProduct = optionProduct;
                            //Context.OrderDetails.Add(orderDetail);
                            //Context.SaveChanges();
                            
                            _orderDetailBase.Add(orderDetail);
                        }
                        catch (Exception exCart)
                        {
                            Debug.WriteLine(exCart);
                        }
                        
                    }
                    return RedirectToAction("CompleteOrder", "Order");
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("CheckOut", "Order");
        }
        [Route("Order/SearchOrder")]
        public ActionResult SearchOrder()
        {
            return View();
        }
        [Route("Order/SearchOrder/{orderCode}")]
        public ActionResult SearchOrder(string orderCode)
        {
            if (orderCode == null)
            {
                return View();
            }
            var order = Context.Orders.FirstOrDefault(p => p.Code == orderCode);
            if(order == null)
            {
                return View();
            }
            return View(order);
        }

        public ActionResult CompleteOrder()
        {
            return View();
        }
    }
}