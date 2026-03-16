using BusinessLogic.Interfaces;
using BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotelLeAnh49.Models;

namespace MotelLeAnh49.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        private readonly EmailService _emailService;

        public BookingsController(
            IBookingService bookingService,
            IRoomService roomService,
            EmailService emailService)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _emailService = emailService;
        }

        // ==============================
        // CUSTOMER BOOK ROOM PAGE
        // ==============================
        public IActionResult BookRoom(int roomId)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để đặt phòng!";
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.RoomId = roomId;
            return View();
        }

        // ==============================
        // CUSTOMER CREATE BOOKING
        // ==============================
        [HttpPost]
        public IActionResult BookRoom(Booking booking)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                TempData["Error"] = "Phiên đăng nhập hết hạn!";
                return RedirectToAction("Login", "Auth");
            }

            booking.CustomerId = customerId.Value;

            var error = _bookingService.ValidateBooking(booking);
            if (error != null)
            {
                TempData["Error"] = error;
                return View(booking);
            }

            bool available = _bookingService.IsRoomAvailable(
                booking.RoomId,
                booking.CheckIn,
                booking.CheckOut
            );

            if (!available)
            {
                TempData["Error"] = "Phòng đã được đặt trong khoảng thời gian này";
                return View(booking);
            }

            booking.Status = "Pending";
            booking.CreatedAt = DateTime.Now;

            _bookingService.Create(booking);

            TempData["Success"] = "Đặt phòng thành công. Chờ admin duyệt.";

            return RedirectToAction("History");
        }

        // ==============================
        // CUSTOMER BOOKING HISTORY
        // ==============================
        public IActionResult History()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var bookings = _bookingService.GetByCustomerId(customerId.Value);

            foreach (var booking in bookings)
            {
                if (booking.Status == "Pending" && booking.CheckIn.AddDays(1) < DateTime.Now)
                {
                    booking.Status = "Cancelled";
                    _bookingService.Update(booking);
                }
            }

            return View(bookings);
        }

        // ==============================
        // CUSTOMER CANCEL BOOKING
        // ==============================
        public IActionResult Cancel(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var booking = _bookingService.GetById(id);

            if (booking == null || booking.CustomerId != customerId.Value)
            {
                return NotFound();
            }

            var timeLeft = booking.CheckIn - DateTime.Now;

            if (timeLeft.TotalHours <= 4)
            {
                TempData["Error"] = "Không thể hủy phòng trước giờ nhận phòng 4 tiếng";
                return RedirectToAction("History");
            }

            booking.Status = "Cancelled";
            _bookingService.Update(booking);

            TempData["Success"] = "Đã hủy đặt phòng thành công";

            return RedirectToAction("History");
        }

        // ==============================
        // ADMIN BOOKING LIST
        // ==============================
        public IActionResult Index(string keyword, string status, int? roomId)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            var bookings = _bookingService.GetAll();

            if (!string.IsNullOrEmpty(keyword))
            {
                bookings = bookings
                    .Where(b => b.FullName.Contains(keyword) ||
                                b.Phone.Contains(keyword))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                bookings = bookings
                    .Where(b => b.Status == status)
                    .ToList();
            }

            if (roomId != null)
            {
                bookings = bookings
                    .Where(b => b.RoomId == roomId)
                    .ToList();
            }

            ViewBag.RoomList = _roomService.GetAllRooms()
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = "Room " + r.RoomNumber
                }).ToList();

            return View(bookings);
        }

        // ==============================
        // ADMIN CREATE BOOKING
        // ==============================
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            var rooms = _roomService.GetAllRooms();
            var bookings = _bookingService.GetAll();

            ViewBag.RoomList = rooms.Select(r =>
            {
                var booked = bookings
                    .FirstOrDefault(b => b.RoomId == r.Id && b.Status != "Cancelled");

                bool isBooked = booked != null;

                return new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"Room {r.RoomNumber} {(isBooked ? "(Booked)" : "(Available)")}",
                    Disabled = isBooked
                };
            }).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            var error = _bookingService.ValidateBooking(booking);

            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(booking);
            }

            bool available = _bookingService.IsRoomAvailable(
                booking.RoomId,
                booking.CheckIn,
                booking.CheckOut);

            if (!available)
            {
                ModelState.AddModelError("", "Phòng đã được đặt trong khoảng thời gian này");
                return View(booking);
            }

            booking.Status = "Confirmed";
            booking.CreatedAt = DateTime.Now;

            _bookingService.Create(booking);

            return RedirectToAction("Index");
        }

        // ==============================
        // ADMIN UPDATE BOOKING
        // ==============================
        public IActionResult UpdateStatus(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            var booking = _bookingService.GetById(id);

            if (booking == null)
                return NotFound();

            var rooms = _roomService.GetAllRooms();
            var bookings = _bookingService.GetAll();

            ViewBag.RoomList = rooms.Select(r =>
            {
                var booked = bookings
                    .FirstOrDefault(b => b.RoomId == r.Id && b.Status != "Cancelled");

                bool isBooked = booked != null && booked.Id != booking.Id;

                return new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"Room {r.RoomNumber} {(isBooked ? "(Booked)" : "(Available)")}",
                    Disabled = isBooked && r.Id != booking.RoomId
                };
            }).ToList();

            return View(booking);
        }

        
        [HttpPost]
        public IActionResult UpdateStatus(Booking model)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Admin");
            }

            var booking = _bookingService.GetById(model.Id);

            if (booking == null)
                return NotFound();

            bool available = _bookingService.IsRoomAvailable(
                model.RoomId,
                model.CheckIn,
                model.CheckOut,
                model.Id
            );

            if (!available)
            {
                ModelState.AddModelError("", "Phòng đã được đặt trong khoảng thời gian này");

                var rooms = _roomService.GetAllRooms();

                ViewBag.RoomList = rooms.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = "Room " + r.RoomNumber
                }).ToList();

                return View(model);
            }

            booking.FullName = model.FullName;
            booking.Phone = model.Phone;
            booking.Email = model.Email;
            booking.RoomId = model.RoomId;
            booking.Status = model.Status;
            booking.CheckIn = model.CheckIn;
            booking.CheckOut = model.CheckOut;

            _bookingService.Update(booking);

            return RedirectToAction("Index");
        }
    }
}