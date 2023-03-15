using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniCafe.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<OptionProduct> Options { get; set; }
        public PropertyProduct PropertyProduct { get; set; }
    }
    //public class CartOption
    //{
    //    public int Id { get; set; }
    //    public string OptionName { get; set; }
    //    public int OptionId { get; set; }
    //}

    //public class CartProperty
    //{
    //    public int Id { get; set; }
    //    public string PropertyName { get; set; }
    //    public int PropertyId { get; set; }
    //}
}