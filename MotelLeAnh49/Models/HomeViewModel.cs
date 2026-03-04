using System.Collections.Generic;

namespace MotelLeAnh49.Models
{
    public class HomeViewModel
    {
        public BookingViewModel Booking { get; set; }

        public List<Room> Rooms { get; set; }
    }
}
