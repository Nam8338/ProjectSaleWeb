using System;
using System.Collections.Generic;

#nullable disable

namespace SmartZone.Models
{
    public partial class Attribute
    {
        public Attribute()
        {
            AttributePrices = new HashSet<AttributePrice>();
        }

        public int AttributeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AttributePrice> AttributePrices { get; set; }
    }
}
