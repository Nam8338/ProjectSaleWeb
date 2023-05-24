using System;
using System.Collections.Generic;

#nullable disable

namespace SmartZone.Models
{
    public partial class ImportTicket
    {
        public ImportTicket()
        {
            ImportDetails = new HashSet<ImportDetail>();
        }

        public int TicketId { get; set; }
        public DateTime? ImportDate { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? UserId { get; set; }

        public virtual Account User { get; set; }
        public virtual ICollection<ImportDetail> ImportDetails { get; set; }
    }
}
