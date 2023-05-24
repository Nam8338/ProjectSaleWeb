using System;
using System.Collections.Generic;

#nullable disable

namespace SmartZone.Models
{
    public partial class TransactStatus
    {
        public TransactStatus()
        {
            Orders = new HashSet<Order>();
        }

        public int TransactionStatusId { get; set; }
        public string Status { get; set; }
        public string Descriptions { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
