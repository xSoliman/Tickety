using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Tickty.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Your name should only contain letters and spaces.")]
        public string Name { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(255)]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please Enter Valid E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, ErrorMessage = "Password must be at least 8", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; set; }

        [RegularExpression(@"^\+?[0-9]\d{1,14}$", ErrorMessage = "Please enter a valid phone number.")]
        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; }
        public string Role { get; set; }
        public List<Bill> Bills { get; set; } 

    }
}
