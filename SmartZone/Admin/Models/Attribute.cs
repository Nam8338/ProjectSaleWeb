using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Admin.Models
{
    public partial class Attribute
    {
        public Attribute()
        {
            AttributePrices = new HashSet<AttributePrice>();
        }
        [Key]
        public int AttributeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AttributePrice> AttributePrices { get; set; }
    }
}
