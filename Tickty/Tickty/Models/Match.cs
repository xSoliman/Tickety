using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Tickty.Models
{
    public class Match
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string Referee { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Stadium is required")]
        public int StadiumId { get; set; }
        public Stadium Stadium { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
