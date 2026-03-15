using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Models;

namespace MotelLeAnh49.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        // ================= ROOM =================
        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        // ============== CUSTOMER =================
        public int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        // ============== CONTACT =================
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        // ============== BOOKING INFO =================
        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        public int Adults { get; set; }

        public int Children { get; set; }

        // ============== STATUS =================
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        // Pending
        // Confirmed
        // Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}