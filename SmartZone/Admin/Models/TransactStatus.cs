using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Admin.Models
{
    public partial class TransactStatus
    {
        public TransactStatus()
        {
            Orders = new HashSet<Order>();
        }
        [Key]
        public int TransactionStatusId { get; set; }
        public string Status { get; set; }
        public string Descriptions { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
