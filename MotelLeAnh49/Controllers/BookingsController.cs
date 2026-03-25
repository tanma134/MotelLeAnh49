using BusinessLogic.Interfaces;
using BusinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using MotelLeAnh49.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotelLeAnh49.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        private readonly IServiceItemService _serviceItemService;
        private readonly IHubContext<BookingHub> _hubContext;

        public BookingsController(
            IBookingService bookingService,
            IRoomService roomService,
            IServiceItemService serviceItemService,
            IHubContext<BookingHub> hubContext)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _serviceItemService = serviceItemService;
            _hubContext = hubContext;
        }

        // ──────────────────────────────────────────
        // HELPERS
        // ──────────────────────────────────────────
        private bool IsAdmin() =>
            HttpContext.Session.GetString("Role") == "Admin";

        private int? CurrentCustomerId() =>
            HttpContext.Session.GetInt32("CustomerId");

        private async Task NotifyClientsAsync() =>
            await _hubContext.Clients.All.SendAsync("reloadBooking");

        /// <summary>
        /// Dùng cho UpdateStatus: hiển thị trạng thái phòng dựa trên booking hiện tại,
        /// loại trừ booking đang sửa.
        /// </summary>
        private void PrepareRoomListForUpdate(int selectedRoomId, int excludeBookingId)
        {
            var rooms = _roomService.GetAllRooms();
            var bookings = _bookingService.GetAll();

            ViewBag.RoomList = rooms.Select(r =>
            {
                var conflict = bookings.FirstOrDefault(b =>
                    b.RoomId == r.Id &&
                    b.Status != "Cancelled" &&
                    b.Id != excludeBookingId);

                bool isBooked = conflict != null;

                return new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"Room {r.RoomNumber} {(isBooked ? "(Booked)" : "(Available)")}",
                    Disabled = isBooked,
                    Selected = r.Id == selectedRoomId
                };
            }).ToList();
        }

        /// <summary>
        /// Dùng cho Create (Admin): tất cả phòng đều Available khi load form.
        /// </summary>
        private void PrepareRoomListForCreate(int? selectedRoomId = null)
        {
            ViewBag.RoomList = _roomService.GetAllRooms()
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"Room {r.RoomNumber}",
                    Selected = r.Id == selectedRoomId
                }).ToList();
        }

        // ──────────────────────────────────────────
        // ADMIN — BOOKING LIST
        // ──────────────────────────────────────────
        public IActionResult Index(string keyword, string status, int? roomId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var bookings = _bookingService.GetAll();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                bookings = bookings.Where(b =>
                    b.FullName.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    b.Phone.Contains(kw)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
                bookings = bookings.Where(b => b.Status == status).ToList();

            if (roomId.HasValue)
                bookings = bookings.Where(b => b.RoomId == roomId.Value).ToList();

            ViewBag.RoomList = _roomService.GetAllRooms()
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = "Room " + r.RoomNumber
                }).ToList();

            return View(bookings);
        }

        // ──────────────────────────────────────────
        // ADMIN — BOOKING DETAILS
        // ──────────────────────────────────────────
        public IActionResult Details(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var booking = _bookingService.GetById(id);
            if (booking == null) return NotFound();

            booking.Room ??= _roomService.GetRoomById(booking.RoomId);

            ViewBag.UsedServices = _serviceItemService.GetServicesByBooking(id);
            ViewBag.TotalServiceCost = _serviceItemService.CalculateTotalServiceCost(id);
            ViewBag.AvailableServices = _serviceItemService.GetAllServices()
                .Where(s => s.IsAvailable)
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceItemId.ToString(),
                    Text = $"{s.Name} - {s.Price:N0} VNĐ"
                }).ToList();

            return View(booking);
        }

        // ──────────────────────────────────────────
        // ADMIN — CREATE BOOKING
        // ──────────────────────────────────────────
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            PrepareRoomListForCreate();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            if (booking.CheckOut <= booking.CheckIn)
            {
                ModelState.AddModelError("", "CheckOut phải sau CheckIn");
                PrepareRoomListForCreate(booking.RoomId);
                return View(booking);
            }

            var error = _bookingService.ValidateBooking(booking);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                PrepareRoomListForCreate(booking.RoomId);
                return View(booking);
            }

            if (!_bookingService.IsRoomAvailable(booking.RoomId, booking.CheckIn, booking.CheckOut))
            {
                ModelState.AddModelError("", "Phòng đã được đặt trong khoảng thời gian này");
                PrepareRoomListForCreate(booking.RoomId);
                return View(booking);
            }

            booking.Status = "Confirmed";
            booking.CreatedAt = DateTime.Now;

            await _bookingService.CreateAsync(booking);
            await NotifyClientsAsync();

            TempData["Success"] = "Tạo booking thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ──────────────────────────────────────────
        // ADMIN — UPDATE BOOKING
        // ──────────────────────────────────────────
        public IActionResult UpdateStatus(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var booking = _bookingService.GetById(id);
            if (booking == null) return NotFound();

            PrepareRoomListForUpdate(booking.RoomId, booking.Id);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Booking model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            ModelState.Remove("Room");
            ModelState.Remove("Customer");

            var booking = _bookingService.GetById(model.Id);
            if (booking == null) return NotFound();

            if (booking.Status == "Cancelled")
            {
                TempData["Error"] = "Đơn đã hủy!";
                return RedirectToAction(nameof(Index));
            }

            if (model.CheckOut <= model.CheckIn)
            {
                ModelState.AddModelError("", "CheckOut phải sau CheckIn");
                PrepareRoomListForUpdate(booking.RoomId, booking.Id);
                return View(model);
            }

            var error = _bookingService.ValidateBooking(model);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                PrepareRoomListForUpdate(booking.RoomId, booking.Id);
                return View(model);
            }

            if (!_bookingService.IsRoomAvailable(model.RoomId, model.CheckIn, model.CheckOut, model.Id))
            {
                ModelState.AddModelError("", "Phòng đã có người đặt");
                PrepareRoomListForUpdate(booking.RoomId, booking.Id);
                return View(model);
            }

            

            await _bookingService.UpdateAsync(model);
            await NotifyClientsAsync();

            TempData["Success"] = "Cập nhật booking thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ──────────────────────────────────────────
        // ADMIN — SEARCH (AJAX)
        // ──────────────────────────────────────────
        [HttpGet]
        public IActionResult Search(string keyword, string status)
        {
            if (!IsAdmin()) return Json(new List<object>());

            var bookings = _bookingService.GetAll();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                bookings = bookings.Where(b =>
                    b.FullName?.ToLower().Contains(kw) == true ||
                    b.Phone?.Contains(kw) == true).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
                bookings = bookings.Where(b => b.Status == status).ToList();

            var result = bookings.Select(b => new
            {
                b.Id,
                fullName = b.FullName,
                phone = b.Phone,
                email = b.Email,
                roomNumber = b.Room?.RoomNumber ?? "N/A",
                checkIn = b.CheckIn.ToString("dd/MM/yyyy"),
                checkOut = b.CheckOut.ToString("dd/MM/yyyy"),
                status = b.Status
            });

            return Json(result);
        }

        // ──────────────────────────────────────────
        // CUSTOMER — BOOK ROOM (ĐÃ FIX LỖI ĐĂNG NHẬP)
        // ──────────────────────────────────────────
        public IActionResult BookRoom(int roomId, DateTime? checkIn, DateTime? checkOut)
        {
            var customerId = CurrentCustomerId();

            if (customerId == null)
            {
                // Lưu URL để quay lại sau khi login
                TempData["ReturnUrl"] = Request.Path + Request.QueryString.Value;

                TempData["Error"] = "Vui lòng đăng nhập để đặt phòng!";
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.RoomId = roomId;
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookRoom(Booking booking)
        {
            var customerId = CurrentCustomerId();
            if (customerId == null)
            {
                TempData["Error"] = "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại!";
                return RedirectToAction("Login", "Auth");
            }

            booking.CustomerId = customerId.Value;

            var error = _bookingService.ValidateBooking(booking);
            if (error != null)
            {
                TempData["Error"] = error;
                return View(booking);
            }

            if (!_bookingService.IsRoomAvailable(booking.RoomId, booking.CheckIn, booking.CheckOut))
            {
                TempData["Error"] = "Phòng đã được đặt trong khoảng thời gian này";
                return View(booking);
            }

            booking.Status = "Pending";
            booking.CreatedAt = DateTime.Now;

            await _bookingService.CreateAsync(booking);
            await NotifyClientsAsync();

            TempData["Success"] = "Đặt phòng thành công. Chờ admin duyệt!";
            return RedirectToAction(nameof(History));
        }

        // ──────────────────────────────────────────
        // CUSTOMER — BOOKING HISTORY
        // ──────────────────────────────────────────
        public async Task<IActionResult> History()
        {
            var customerId = CurrentCustomerId();
            if (customerId == null) return RedirectToAction("Login", "Auth");

            var bookings = _bookingService.GetByCustomerId(customerId.Value);

            // Auto cancel pending bookings expired > 1 day
            var expiredPending = bookings
                .Where(b => b.Status == "Pending" && b.CheckIn.AddDays(1) < DateTime.Now)
                .ToList();

            foreach (var b in expiredPending)
            {
                b.Status = "Cancelled";
                await _bookingService.UpdateAsync(b);
            }

            return View(bookings);
        }

        // ──────────────────────────────────────────
        // CUSTOMER — CANCEL BOOKING
        // ──────────────────────────────────────────
        public async Task<IActionResult> Cancel(int id)
        {
            var customerId = CurrentCustomerId();
            if (customerId == null) return RedirectToAction("Login", "Auth");

            var booking = _bookingService.GetById(id);
            if (booking == null || booking.CustomerId != customerId.Value)
                return NotFound();

            if ((booking.CheckIn - DateTime.Now).TotalHours <= 4)
            {
                TempData["Error"] = "Không thể hủy phòng trước giờ nhận phòng 4 tiếng!";
                return RedirectToAction(nameof(History));
            }

            booking.Status = "Cancelled";
            await _bookingService.UpdateAsync(booking);
            await NotifyClientsAsync();

            TempData["Success"] = "Đã hủy đặt phòng thành công!";
            return RedirectToAction(nameof(History));
        }

        // ──────────────────────────────────────────
        // API: LẤY DANH SÁCH NGÀY ĐÃ ĐẶT CỦA PHÒNG
        // ──────────────────────────────────────────
        [HttpGet]
        public IActionResult GetBookedRanges(int roomId)
        {
            var bookings = _bookingService.GetAll()
                .Where(b => b.RoomId == roomId && b.Status != "Cancelled")
                .Select(b => new
                {
                    checkIn = b.CheckIn.ToString("yyyy-MM-dd"),
                    checkOut = b.CheckOut.ToString("yyyy-MM-dd")
                })
                .ToList();

            return Json(bookings);
        }

        // ──────────────────────────────────────────
        // AJAX: LẤY DANH SÁCH PHÒNG THEO NGÀY (DÙNG CHO ADMIN CREATE)
        // ──────────────────────────────────────────
        [HttpGet]
        public IActionResult GetRoomsByDate(DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            var rooms = _roomService.GetAllRooms();

            var result = rooms.Select(r => new
            {
                value = r.Id.ToString(),
                text = $"Room {r.RoomNumber}",
                isBooked = !_bookingService.IsRoomAvailable(r.Id, checkIn, checkOut, excludeBookingId)
            });

            return Json(result);
        }
    }
}