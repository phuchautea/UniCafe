using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;

namespace UniCafe.Models
{
    public class CartItem : IEquatable<CartItem>
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<OptionProduct> Options { get; set; }
        public PropertyProduct PropertyProduct { get; set; }

        public bool Equals(CartItem other)
        {
            if (other == null) return false;
            if (this.ProductId != other.ProductId) return false;
            if (this.PropertyProduct.Id != other.PropertyProduct.Id) return false;
            if (this.Options.Count != other.Options.Count) return false;
            foreach (var option in this.Options)
            {
                if (!other.Options.Contains(option)) return false;
            }
            return true;
        }
    }
}