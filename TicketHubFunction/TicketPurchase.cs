using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHubFunction
{
    public class TicketPurchase
    {
        //[Required]
        public int ConcertId { get; set; }

        //[Required, EmailAddress]
        public string? Email { get; set; }
        //[Required, MaxLength(255)]
        public string? Name { get; set; }
        //[Required, Phone]
        public string? Phone { get; set; }
        //[Required]
        public int Quantity { get; set; }
        //[Required, CreditCard]
        public string? CreditCard { get; set; }
        //[Required, MaxLength(4)]
        public string? Expiry { get; set; }
        //[Required]
        public string? SecurityCode { get; set; }
        //[Required]
        public string? Address { get; set; }
        //[Required]
        public string? City { get; set; }
        //[Required]
        public string? Province { get; set; }
        //[Required, MaxLength(6)]
        public string? PostalCode { get; set; }
        //[Required]
        public string? Country { get; set; }
    }
}
