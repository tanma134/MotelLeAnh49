using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MotelLeAnh49.Models;

namespace DataAccess.Models
{
    public class EventRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Event")]
        public int EventId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public bool IsCancelled { get; set; }

        public DateTime? CancelledAt { get; set; }

        // Navigation
        public Event Event { get; set; } = null!;
    }
}

