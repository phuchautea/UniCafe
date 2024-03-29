﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using UniCafe.Data;
using UniCafe.Models;
using UniCafe.Services;
using Unity.Policy;
using System.Web.Helpers;

namespace UniCafe.Controllers
{
    public class OrderController : BaseController<Order>
    {
        private readonly CartManager _cartManager;
        public OrderController() {
            _cartManager = new CartManager();
        }
        public static bool ValidateVNPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("+84", "0");
            Regex regex = new Regex(@"^(0)(86|96|97|98|32|33|34|35|36|37|38|39|91|94|83|84|85|81|82|90|93|70|79|77|76|78|92|56|58|99|59|55|87)\d{7}$");
            return regex.IsMatch(phoneNumber);
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray()).ToUpper();
        }
        public void SendEmail(string email, string name, string orderCode)
        {
            var fromAddress = new MailAddress("hphcplnh7@gmail.com", "UniCafe");
            var toAddress = new MailAddress(email, name);
            const string fromPassword = "phhrqqdawnyalkpr";
            string subject = "Thông tin đơn hàng UniCafe #" + orderCode + "";
            const string emailBody = "<div class='text-center'>" +
                                "<p>Bạn đã đặt hàng thành công</p>" +
                                "<p>Cảm ơn bạn vì giữa muôn vàn sự lựa chọn đã chọn <b>UniCafe</b></p>" +
                                "<p>Xem chi tiết đơn đặt hàng của bạn <a href='https://unicafe.phuchautea.com/Order/SearchOrder/{0}'>tại đây</a></p>";
            string body = string.Format(emailBody, orderCode);
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
                ViewBag.Message = "Email sent successfully.";
            }
        }

        // GET: Order
        public ActionResult CheckOut()
        {
            var cart = _cartManager.GetCartItems();
            if(cart.Count() < 1){
                return RedirectToAction("Index", "Home");
            }
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
                var email = formCollection["email"];
                var phoneNumber = formCollection["phoneNumber"];
                var address = formCollection["address"];
                var payment = formCollection["payment"];
                var note = formCollection["note"];
                var UserId = formCollection["UserId"];

                if (string.IsNullOrEmpty(name))
                {
                    errors.Add("Vui lòng nhập tên.");
                }

                if (string.IsNullOrEmpty(address))
                {
                    errors.Add("Vui lòng nhập địa chỉ.");
                }

                if(ValidateVNPhoneNumber(phoneNumber) != true){
                    errors.Add("Số điện thoại không hợp lệ.");
                }

                switch (payment)
                {
                    case "cash":
                    case "momo":
                    case "vnpay":
                        break;
                    default:
                        errors.Add("Phương thức thanh toán không hợp lệ.");
                        break;
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
                    order.UserId = UserId;
                    order.Status = "1";
                    
                    //Add(order);
                    Session["orderCode"] = code;
                    // Lấy tổng tiền từ giỏ hàng
                    var cart = _cartManager.GetCartItems();
                    decimal totalOrder = 0;
                    foreach (var item in cart)
                    {
                        var itemTotal = item.Price; // giá sản phẩm
                        itemTotal += item.PropertyProduct.Price; // giá của thuộc tính (size)
                        foreach (var option in item.Options)
                        {
                            itemTotal += option.Price; // giá của tùy chọn (topping)
                        }
                        totalOrder += itemTotal;
                    }
                    order.Total = totalOrder;
                    Session["order"] = order;
                    Session["email"] = email;
                    switch (payment)
                    {
                        case "momo":
                            return RedirectToAction("MomoPay", "Pay");
                        case "vnpay":
                            return RedirectToAction("VNPay", "Pay");
                        default:
                            totalOrder = 0;
                            foreach (var item in cart)
                            {
                                var itemTotal = item.Price;
                                itemTotal += item.PropertyProduct.Price;
                                string propertyProduct = "" + item.PropertyProduct.Name + " - " + item.PropertyProduct.Price.ToString("N0") + "đ";
                                string optionProduct = "";
                                foreach (var option in item.Options)
                                {
                                    itemTotal += option.Price;
                                    optionProduct += "" + option.Name + " - " + option.Price.ToString("N0") + "đ\n";
                                }
                                OrderDetail orderDetail = new OrderDetail();
                                orderDetail.Order = order;
                                orderDetail.ProductId = item.ProductId;
                                orderDetail.ProductName = item.ProductName;
                                orderDetail.Price = item.Price;
                                orderDetail.Total = itemTotal;
                                orderDetail.Quantity = item.Quantity;
                                orderDetail.PropertyProduct = propertyProduct;
                                orderDetail.OptionProduct = optionProduct;
                                totalOrder += itemTotal;
                                Context.OrderDetails.Add(orderDetail);
                                Context.SaveChanges();
                            }
                            //Cập nhật tổng số tiền
                            order.Total = totalOrder;
                            Update(order);
                            _cartManager.ClearCart();
                            SendEmail(Session["email"].ToString(), order.Name, order.Code);
                            break;
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
            //var find =  from o in Context.Orders join od in Context.OrderDetails on o.Id equals od.Id where o.Code== orderCode
            //            select o;


            if (order != null)
            {
                // Xóa OrderCode
                Session["orderCode"] = null;
                var orderDetails = Context.OrderDetails.Where(p => p.Order.Code == orderCode).ToList();
                ViewBag.OrderDetails = orderDetails;
                return View(order);
            }
            return View();
        }

        public ActionResult CompleteOrder()
        {
            return View();
        }
    }
}