using System;
using System.Collections.Generic;

#nullable disable

namespace SmartZone.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? TransactionStatusId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsPaid { get; set; }
        public string Note { get; set; }
        public decimal? TotalMoney { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual TransactStatus TransactionStatus { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
