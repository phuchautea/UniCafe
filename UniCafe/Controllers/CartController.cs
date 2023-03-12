using Newtonsoft.Json;
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
        private readonly CartManager _cartManager;
        public CartController()
        {
            _context = new ApplicationDbContext();
            _productRepository = new Repository<Product>(_context);
            _cartManager = new CartManager();
        }
        public ActionResult Index()
        {
            var cart = _cartManager.GetCartItems();
            return View(cart);
        }
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            var product = _productRepository.GetById(productId);
            var item = new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                ProductName = product.Name,
                Price = product.Price
            };
            _cartManager.AddToCart(item);

            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult RemoveFromCart(int productId)
        {
            _cartManager.RemoveFromCart(productId);

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