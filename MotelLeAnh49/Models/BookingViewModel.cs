using System;
using System.ComponentModel.DataAnnotations;

namespace MotelLeAnh49.Models
{
    public class BookingViewModel
    {
        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        public int Adults { get; set; }

        public int Children { get; set; }
    }
}
