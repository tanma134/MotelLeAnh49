using MotelLeAnh49.Models;

namespace BusinessLogic.Service
{
    public interface IBookingService
    {
        List<Booking> GetAll();

        Booking GetById(int id);

        List<Booking> GetByCustomerId(int customerId);

        void Create(Booking booking);

        void Update(Booking booking);

        void Delete(int id);

        bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut, int? ignoreBookingId = null);
        string ValidateBooking(Booking booking);
    }
}