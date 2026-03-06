using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string IdentityNumber { get; set; }

        // Navigation
        public Account Account { get; set; }
    }
}
