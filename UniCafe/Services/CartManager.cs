using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using UniCafe.Models;

namespace UniCafe.Services
{
    public class CartManager
    {
        public const string CartSessionKey = "Cart";
        public bool CompareListOptions(List<OptionProduct> list1, List<OptionProduct> list2)
        {
            bool equal = true;
            if (list1.Count == list2.Count)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (list1[i].Id != list2[i].Id)
                    {
                        equal = false;
                        break;
                    }
                }
            }
            else
            {
                equal = false;
            }
            return equal;
        }

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
            var existingItem = cart.FirstOrDefault(c => c.ProductId == item.ProductId
                                                     && c.PropertyProduct.Id == item.PropertyProduct.Id
                                                     && CompareListOptions(c.Options, item.Options));
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
        }
        public void UpdateCart(IEnumerable<CartItem> cart)
        {
            HttpContext.Current.Session[CartSessionKey] = cart.ToList();
        }

        public void RemoveFromCart(Guid cartItemId)
        {
            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(c => c.Id == cartItemId);
            if (existingItem != null)
            {
                cart.Remove(existingItem);
            }
        }
        public decimal GetCartTotal(IEnumerable<CartItem> cart)
        {
            decimal total = 0;
            foreach (var item in cart)
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }
        public decimal GetTotal()
        {
            var cart = GetCartItems();
            decimal total = 0;

            foreach (var item in cart)
            {
                total += (item.Price + item.PropertyProduct.Price + item.Options.Sum(option => option.Price)) * item.Quantity;
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