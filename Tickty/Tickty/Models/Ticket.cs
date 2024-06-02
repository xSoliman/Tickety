using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Tickty.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Number of tickets is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Ticket count must be a non-negative number")]
        public int TicketCount { get; set; }

        public string Type { get; set; }

        [Required(ErrorMessage = "Match is required")]
        public int MatchId { get; set; }
        public Match Match { get; set; }
        public List<Bill> Bills { get; set; } 
    }
}
