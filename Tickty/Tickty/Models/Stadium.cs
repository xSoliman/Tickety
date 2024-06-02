using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Tickty.Models
{
    public class Stadium
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Link for location is required")]
        public string GmapLocation { get; set; }
        public List<Match> Matches { get; set; }

        [NotMapped]
        public IFormFile? clientFile { get; set; }
    }
}
