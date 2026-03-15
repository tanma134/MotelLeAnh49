using System;
using System.ComponentModel.DataAnnotations;

namespace MotelLeAnh49.Models
{
    public class JoinEventViewModel
    {
        [Required]
        public int EventId { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public int? RegistrationId { get; set; }
    }
}

