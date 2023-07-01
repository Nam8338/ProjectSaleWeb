using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Admin.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Products = new HashSet<Product>();
        }
        [Key]
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Logo { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
