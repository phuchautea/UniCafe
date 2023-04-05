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
        private readonly IRepository<PropertyProduct> _propertyProductRepository;
        private readonly IRepository<OptionProduct> _optionProductRepository;
        private readonly CartManager _cartManager;
        public CartController()
        {
            _context = new ApplicationDbContext();
            _productRepository = new Repository<Product>(_context);
            _propertyProductRepository = new Repository<PropertyProduct>(_context);
            _optionProductRepository = new Repository<OptionProduct>(_context);
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
            var propertyProduct = _propertyProductRepository.GetById(propertyId);
            List<OptionProduct> optionProducts = new List<OptionProduct>();
            if(optionProductIds != null)
            {
                foreach (var optionProductId in optionProductIds)
                {
                    optionProducts.Add(_optionProductRepository.GetById(optionProductId));
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
        public JsonResult UpdateCart(Guid itemId, int quantity)
        {
            // Get the cart and find the item to update
            var cart = _cartManager.GetCartItems();
            var item = cart.FirstOrDefault(x => x.Id == itemId);
            // Check if the item was found
            if (item == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            // Update the item properties
            item.Quantity = quantity;

            // Update the cart
            _cartManager.UpdateCart(cart);

            return Json(new { success = true, message = "Item updated" });
        }
        [HttpGet]
        public JsonResult GetTotal()
        {
            int total = 0;
            // Get the cart and find the item to update
            var cart = _cartManager.GetCartItems();
            foreach(var item in cart)
            {
                total += item.Quantity;
            }
            return Json(new { success = true, total = total }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateCart1(int productId, int quantity, List<int> optionProductIds, int propertyId)
        {
            //var item = new CartItem()
            //{
            //    Id = model.Id,
            //    ProductId = model.ProductId,
            //    PropertyProduct = new PropertyProductModel()
            //    {
            //        Id = model.PropertyProductId,
            //        Name = model.PropertyName,
            //        Price = model.PropertyPrice
            //    },
            //    Options = model.Options,
            //    Quantity = model.Quantity,
            //    Price = model.Price
            //};

            //var cartManager = new CartManager();
            //cartManager.UpdateCart(item);

            return RedirectToAction("Index");
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