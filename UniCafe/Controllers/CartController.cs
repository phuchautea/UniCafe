﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UniCafe.Data;
using UniCafe.Models;
using UniCafe.Services;

namespace UniCafe.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<PropertyProduct> _propertyProductRepositoy;
        private readonly IRepository<OptionProduct> _optionProductRepositoy;
        private readonly CartManager _cartManager;
        public CartController()
        {
            _context = new ApplicationDbContext();
            _productRepository = new Repository<Product>(_context);
            _propertyProductRepositoy = new Repository<PropertyProduct>(_context);
            _optionProductRepositoy = new Repository<OptionProduct>(_context);
            _cartManager = new CartManager();
        }
        public ActionResult Index()
        {
            var cart = _cartManager.GetCartItems();
            if (cart.Count() < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(cart);
        }
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity, List<int> optionProductIds, int propertyId)
        {
            var product = _productRepository.GetById(productId);
            var propertyProduct = _propertyProductRepositoy.GetById(propertyId);
            List<OptionProduct> optionProducts = new List<OptionProduct>();
            if(optionProductIds != null)
            {
                foreach (var optionProductId in optionProductIds)
                {
                    optionProducts.Add(_optionProductRepositoy.GetById(optionProductId));
                }
            }
            if(quantity <= 0)
            {
                return Json(new { status = false, message = "Số lượng không hợp lệ" });
            }
            var item = new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                ProductName = product.Name,
                Price = product.Price,
                Options = optionProducts,
                PropertyProduct = propertyProduct
            };
            _cartManager.AddToCart(item);
            return Json(new { status = true, message = "Thêm vào giỏ hàng thành công" });
        }
        [HttpPost]
        public ActionResult RemoveFromCart(Guid cartItemId)
        {
            _cartManager.RemoveFromCart(cartItemId);

            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult ClearCart()
        {
            _cartManager.ClearCart();

            return Json(new { success = true });
        }


    }

}