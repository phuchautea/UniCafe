using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;

namespace UniCafe.Models
{
    public class CartItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<OptionProduct> Options { get; set; }
        public PropertyProduct PropertyProduct { get; set; }
    }
}