using MotelLeAnh49.Models;
using DataAccess.Repositories;
using System.Text.RegularExpressions;

namespace BusinessLogic.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;

        public BookingService(IBookingRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        // ===============================
        // VALIDATE BOOKING
        // ===============================

        public string ValidateBooking(Booking booking)
        {
            if (string.IsNullOrWhiteSpace(booking.FullName))
                return "Vui lòng nhập họ tên";

            if (string.IsNullOrWhiteSpace(booking.Phone))
                return "Vui lòng nhập số điện thoại";

            if (!Regex.IsMatch(booking.Phone, @"^0[0-9]{9}$"))
                return "Số điện thoại phải 10 số";

            if (string.IsNullOrWhiteSpace(booking.Email))
                return "Vui lòng nhập email";

            if (!Regex.IsMatch(booking.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Email không đúng định dạng";

            if (booking.CheckIn < DateTime.Today)
                return "Check-in phải từ hôm nay trở đi";

            if (booking.CheckOut <= booking.CheckIn)
                return "Check-out phải sau Check-in";

            if (booking.Adults <= 0)
                return "Phải có ít nhất 1 người lớn";

            return null;
        }

        // ===============================
        // GET ALL BOOKINGS
        // ===============================

        public List<Booking> GetAll()
        {
            return _bookingRepo.GetAll().ToList();
        }

        // ===============================
        // GET BOOKING BY ID
        // ===============================

        public Booking GetById(int id)
        {
            return _bookingRepo.GetById(id);
        }

        // ===============================
        // GET BOOKINGS BY CUSTOMER
        // ===============================

        public List<Booking> GetByCustomerId(int customerId)
        {
            return _bookingRepo.GetAll()
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
        }

        // ===============================
        // CREATE BOOKING
        // ===============================

        public void Create(Booking booking)
        {
            _bookingRepo.Add(booking);
            _bookingRepo.Save();
        }

        // ===============================
        // UPDATE BOOKING
        // ===============================

        public void Update(Booking booking)
        {
            _bookingRepo.Update(booking);
            _bookingRepo.Save();
        }

        // ===============================
        // DELETE BOOKING
        // ===============================

        public void Delete(int id)
        {
            var booking = _bookingRepo.GetById(id);

            if (booking != null)
            {
                _bookingRepo.Delete(id);
                _bookingRepo.Save();
            }
        }

        // ===============================
        // CHECK ROOM AVAILABLE
        // ===============================

        public bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut, int? ignoreBookingId = null)
        {
            return !_bookingRepo.GetAll().Any(b =>
                b.RoomId == roomId &&
                b.Status != "Cancelled" &&
                (ignoreBookingId == null || b.Id != ignoreBookingId) &&
                checkIn < b.CheckOut &&
                checkOut > b.CheckIn
            );
        }
    }
}