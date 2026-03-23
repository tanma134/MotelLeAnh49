using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotelLeAnh49.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "⚠️ Vui lòng nhập số phòng")]
        [MaxLength(10, ErrorMessage = "❌ Tối đa 10 ký tự")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "⚠️ Vui lòng nhập loại phòng")]
        [MaxLength(50, ErrorMessage = "❌ Tối đa 50 ký tự")]
        public string RoomType { get; set; } = string.Empty;

        [Required(ErrorMessage = "⚠️ Nhập giá qua đêm")]
        [Range(0, double.MaxValue, ErrorMessage = "❌ Giá phải >= 0")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal OvernightPrice { get; set; }

        [Required(ErrorMessage = "⚠️ Nhập giá ngày")]
        [Range(0, double.MaxValue, ErrorMessage = "❌ Giá phải >= 0")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal DayPrice { get; set; }

        [Required(ErrorMessage = "⚠️ Nhập giá giờ đầu")]
        [Range(0, double.MaxValue, ErrorMessage = "❌ Giá phải >= 0")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal FirstHourPrice { get; set; }

        [Required(ErrorMessage = "⚠️ Nhập giá giờ tiếp theo")]
        [Range(0, double.MaxValue, ErrorMessage = "❌ Giá phải >= 0")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal NextHourPrice { get; set; }

        [Required(ErrorMessage = "⚠️ Nhập số khách tối đa")]
        [Range(1, 50, ErrorMessage = "❌ Số khách phải từ 1 - 50")]
        public int MaxGuests { get; set; }

        [Required(ErrorMessage = "⚠️ Nhập phụ thu thêm người")]
        [Range(0, double.MaxValue, ErrorMessage = "❌ Phụ thu phải >= 0")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal ExtraGuestFee { get; set; }

        // Foreign Key
        public int AdminId { get; set; }

        // Navigation
        public Admin? Admin { get; set; }

        public ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();
    }
}