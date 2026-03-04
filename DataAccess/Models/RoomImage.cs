using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotelLeAnh49.Models
{
    public class RoomImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; }   // /images/rooms/p101_1.jpg

        [ForeignKey("Room")]
        public int RoomId { get; set; }

        public Room Room { get; set; }
    }
}
