using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Identity Number (ID/Passport) is required")]
        [MaxLength(50, ErrorMessage = "Identity Number cannot exceed 50 characters")]
        public string IdentityNumber { get; set; }

        // Navigation properties
        public Account Account { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}