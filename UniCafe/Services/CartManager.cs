using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniCafe.Models;

namespace UniCafe.Services
{
    public class CartManager
    {
        public const string CartSessionKey = "Cart";

        public List<CartItem> GetCartItems()
        {
            var cart = HttpContext.Current.Session[CartSessionKey] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                HttpContext.Current.Session[CartSessionKey] = cart;
            }
            return cart;
        }
        public void AddToCart(CartItem item)
        {
            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
            }
        }

        public decimal GetTotal()
        {
            var cart = GetCartItems();
            decimal total = 0;

            foreach (var item in cart)
            {
                total += item.Price * item.Quantity;
            }

            return total;
        }

        public void ClearCart()
        {
            var cart = GetCartItems();
            cart.Clear();
        }
    }

}