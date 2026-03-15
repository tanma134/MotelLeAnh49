using System.ComponentModel.DataAnnotations;
namespace MotelLeAnh49.Models
{

    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone must start with 0 and have 10 digits")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^\d{9}$|^\d{12}$", ErrorMessage = "Identity number must be 9 or 12 digits")]
        public string IdentityNumber { get; set; }

        public string Address { get; set; }
    }
}
