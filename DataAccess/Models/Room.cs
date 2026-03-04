using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotelLeAnh49.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string RoomType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,0)")]
        public decimal OvernightPrice { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal DayPrice { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal FirstHourPrice { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal NextHourPrice { get; set; }

        public int MaxGuests { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal ExtraGuestFee { get; set; }

        // Foreign Key
        public int AdminId { get; set; }

        // Navigation (nullable)
        public Admin? Admin { get; set; }

        public ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();
    }
}