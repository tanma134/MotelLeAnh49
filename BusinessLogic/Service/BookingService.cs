using BusinessLogic.Interfaces;
using BusinessLogic.Service;
using DataAccess.Repositories;
using MotelLeAnh49.Models;
using System.Text.RegularExpressions;

namespace BusinessLogic.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly EmailService _emailService;

        public BookingService(
            IBookingRepository bookingRepo,
            IRoomRepository roomRepo,
            EmailService emailService)
        {
            _bookingRepo = bookingRepo;
            _roomRepo = roomRepo;
            _emailService = emailService;
        }

        // ──────────────────────────────────────────
        // VALIDATE
        // ──────────────────────────────────────────

        public string? ValidateBooking(Booking booking)
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

            var room = _roomRepo.GetById(booking.RoomId);
            if (room == null)
                return "Phòng không tồn tại";

            int total = booking.Adults + booking.Children;

            if (booking.Adults > room.MaxGuests)
                return $"Phòng chỉ cho tối đa {room.MaxGuests} người lớn";

            if (total > room.MaxGuests + 1)
                return $"Phòng tối đa {room.MaxGuests + 1} người";

            return null;
        }

        // ──────────────────────────────────────────
        // GET
        // ──────────────────────────────────────────

        public List<Booking> GetAll() =>
            _bookingRepo.GetAll().ToList();

        public Booking? GetById(int id) =>
            _bookingRepo.GetById(id);

        public List<Booking> GetByCustomerId(int customerId) =>
            _bookingRepo.GetAll()
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

        // ──────────────────────────────────────────
        // CREATE
        // ──────────────────────────────────────────

        public async Task CreateAsync(Booking booking)
        {
            _bookingRepo.Add(booking);
            _bookingRepo.Save();

            if (!string.IsNullOrEmpty(booking.Email))
            {
                var body = _emailService.BuildBookingTemplate("🎉 Booking Created", booking, "orange");
                await _emailService.SendEmailAsync(booking.Email, "Đặt phòng thành công", body);
            }
        }

        // ──────────────────────────────────────────
        // UPDATE
        // ──────────────────────────────────────────

        public async Task UpdateAsync(Booking booking)
        {
            var old = _bookingRepo.GetById(booking.Id);
            if (old == null) return;

            var oldStatus = old.Status;

            // 🔥 UPDATE TRÊN ENTITY CŨ (EF TRACKING)
            old.FullName = booking.FullName;
            old.Phone = booking.Phone;
            old.Email = booking.Email;
            old.RoomId = booking.RoomId;
            old.Status = booking.Status;
            old.CheckIn = booking.CheckIn;
            old.CheckOut = booking.CheckOut;
            old.Adults = booking.Adults;
            old.Children = booking.Children;

            _bookingRepo.Save(); // đủ rồi, KHÔNG cần Update()

            // ================= EMAIL =================
            bool statusChanged = oldStatus != old.Status;

            if ((statusChanged || !string.IsNullOrEmpty(old.Email)) && !string.IsNullOrEmpty(old.Email))
            {
                string title;
                string color;

                if (statusChanged)
                {
                    (title, color) = old.Status switch
                    {
                        "Confirmed" => ("✅ Booking Confirmed", "green"),
                        "Cancelled" => ("❌ Booking Cancelled", "red"),
                        "Pending" => ("⏳ Booking Pending", "orange"),
                        _ => ($"📝 Booking Updated - {old.Status}", "blue")
                    };
                }
                else
                {
                    title = "📝 Booking Information Updated";
                    color = "blue";
                }

                try
                {
                    var body = _emailService.BuildBookingTemplate(title, old, color);
                    await _emailService.SendEmailAsync(old.Email, title, body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Send email failed: {ex.Message}");
                }
            }
        }

        // ──────────────────────────────────────────
        // DELETE
        // ──────────────────────────────────────────

        public void Delete(int id)
        {
            var booking = _bookingRepo.GetById(id);
            if (booking == null) return;

            _bookingRepo.Delete(id);
            _bookingRepo.Save();
        }

        // ──────────────────────────────────────────
        // CHECK AVAILABILITY
        // ──────────────────────────────────────────

        public bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut, int? ignoreBookingId = null) =>
            !_bookingRepo.GetAll().Any(b =>
                b.RoomId == roomId &&
                b.Status != "Cancelled" &&
                (ignoreBookingId == null || b.Id != ignoreBookingId) &&
                checkIn < b.CheckOut &&
                checkOut > b.CheckIn
            );
    }
}