using System.ComponentModel.DataAnnotations;

    namespace MotelLeAnh49.Models
    {
        public class Admin
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [MaxLength(50)]
            public string Username { get; set; }

            [Required]
            [MaxLength(255)]
            public string Password { get; set; } // demo: plain text (đồ án OK)

            [MaxLength(100)]
            public string FullName { get; set; }

            public bool IsActive { get; set; } = true;

            // Navigation
            public ICollection<Room> Rooms { get; set; }
        }
    }

