using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCafe.Data;

namespace UniCafe.Models
{
    public class Product
    {
        
        //1 sản phẩm -> 1 danh mục
        [Key]
        public int Id { get; set; }
        public Category Category { get; set; }
        public ICollection<OptionProduct> OptionProducts { get; set; }
        public ICollection<PropertyProduct> PropertyProducts { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
    
}
