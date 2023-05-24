using System;
using System.Collections.Generic;

#nullable disable

namespace SmartZone.Models
{
    public partial class ImportDetail
    {
        public int TicketDetailsId { get; set; }
        public int? TicketId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? ImportCost { get; set; }
        public decimal? TotalCost { get; set; }

        public virtual Product Product { get; set; }
        public virtual ImportTicket Ticket { get; set; }
    }
}
