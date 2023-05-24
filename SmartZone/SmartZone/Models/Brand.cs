using System;
using System.Collections.Generic;

#nullable disable

namespace SmartZone.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Products = new HashSet<Product>();
        }

        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Logo { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
