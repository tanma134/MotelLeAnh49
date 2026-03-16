using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotelLeAnh49.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Location { get; set; } = null!;

        [Required]
        public DateTime EventDate { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; } = null!;
    }
}
