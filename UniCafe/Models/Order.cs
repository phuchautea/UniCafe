﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Unity.Policy;

namespace UniCafe.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Payment { get; set; }
        public decimal Total { get; set; }
        public string Note { get; set; }
        public int Paid { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;




    
    }
}