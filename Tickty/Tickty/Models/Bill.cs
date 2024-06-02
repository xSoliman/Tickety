using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tickty.Models
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string? QrCode { get; set; }
    }
}
