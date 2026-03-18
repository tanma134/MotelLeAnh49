using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MotelLeAnh49.Models;

namespace DataAccess.Models
{
    public class BookingServiceItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;

        [Required]
        public int ServiceItemId { get; set; }

        [ForeignKey("ServiceItemId")]
        public ServiceItem ServiceItem { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal PriceAtBooking { get; set; } // Lưu giá tại thời điểm đặt để tránh thay đổi sau này

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}