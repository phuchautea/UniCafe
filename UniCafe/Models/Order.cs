using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UniCafe.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string Payment { get; set; }
    }
}