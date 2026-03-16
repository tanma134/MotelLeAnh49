using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Data;
using MotelLeAnh49.Models;

namespace MotelLeAnh49.Controllers
{
    public class HomeController : Controller
    {
        private readonly MotelDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MotelDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // TRANG CHỦ HIỂN THỊ DANH SÁCH PHÒNG

        public IActionResult Index()
        {
            var rooms = _context.Rooms
                .Include(r => r.RoomImages)
                .OrderBy(r => r.RoomNumber)
                .ToList();

            var events = _context.Events
                .OrderBy(e => e.EventDate)
                .Take(5)
                .ToList();

            ViewBag.Events = events;

            return View(rooms);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            var room = _context.Rooms
                .Include(r => r.RoomImages)
                .FirstOrDefault(r => r.Id == id);

            if (room == null)
                return NotFound();

            // Lấy CustomerId từ Session
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            // truyền sang View
            ViewBag.CustomerId = customerId;

            return View(room);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
