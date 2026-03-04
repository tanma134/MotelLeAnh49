using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using MotelLeAnh49.Models;

public class RoomsController : Controller
{
    private readonly IRoomService _roomService;

    private readonly IWebHostEnvironment _env;

    public RoomsController(IRoomService roomService,
                           IWebHostEnvironment env)
    {
        _roomService = roomService;
        _env = env;
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetInt32("AdminId") != null;
    }

    // ================= LIST =================
    public IActionResult Index()
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Admin");

        var rooms = _roomService.GetAllRooms();
        return View(rooms);
    }

    // ================= CREATE =================
    public IActionResult Create()
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Admin");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Room room, List<IFormFile> Images)
    {
        if (!ModelState.IsValid)
            return View(room);

        int adminId = HttpContext.Session.GetInt32("AdminId").Value;

        var imagePaths = new List<string>();

        if (Images != null && Images.Any())
        {
            string uploadFolder = Path.Combine(_env.WebRootPath, "images/rooms");
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in Images)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                imagePaths.Add("/images/rooms/" + fileName);
            }
        }

        _roomService.CreateRoom(room, adminId, imagePaths);

        return RedirectToAction(nameof(Index));
    }

    // ================= EDIT =================
    public IActionResult Edit(int id)
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Admin");

        var room = _roomService.GetRoomById(id);
        if (room == null) return NotFound();

        return View(room);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
     Room room,
     List<IFormFile> Images,
     List<int> DeletedImageIds)
    {
        if (!ModelState.IsValid)
            return View(room);

        var imagePaths = new List<string>();

        // Upload ảnh mới
        if (Images != null && Images.Any())
        {
            string uploadFolder = Path.Combine(_env.WebRootPath, "images/rooms");
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in Images)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                imagePaths.Add("/images/rooms/" + fileName);
            }
        }

        // Gọi service mới
        if (!_roomService.UpdateRoom(room, imagePaths, DeletedImageIds))
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // ================= DELETE =================
    public IActionResult Delete(int id)
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Admin");

        var room = _roomService.GetRoomById(id);
        if (room == null) return NotFound();

        return View(room);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _roomService.DeleteRoom(id);
        return RedirectToAction(nameof(Index));
    }

    // ================= SEARCH =================
    [HttpPost]
    public IActionResult Search(BookingViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index", "Home");

        var availableRooms = _roomService.SearchAvailableRooms(
            model.CheckIn,
            model.CheckOut,
            model.Adults,
            model.Children
        );

        return View("AvailableRooms", availableRooms);
    }

    // ================= DETAIL =================
    public IActionResult Detail(int id, DateTime? checkIn, DateTime? checkOut)
    {
        var room = _roomService.GetRoomById(id);
        if (room == null) return NotFound();

        bool isAvailable = true;

        if (checkIn.HasValue && checkOut.HasValue)
        {
            isAvailable = _roomService.IsRoomAvailable(
                id,
                checkIn.Value,
                checkOut.Value
            );
        }

        ViewBag.IsAvailable = isAvailable;
        ViewBag.CheckIn = checkIn;
        ViewBag.CheckOut = checkOut;

        return View("~/Views/Home/Detail.cshtml", room);
    }

    // ================= BOOK =================
    [HttpPost]
    public IActionResult Book(int roomId, DateTime checkIn, DateTime checkOut)
    {
        if (!_roomService.BookRoom(roomId, checkIn, checkOut))
        {
            TempData["Error"] = "Phòng đã được đặt trong khoảng thời gian này.";
            return RedirectToAction("Detail", new { id = roomId });
        }

        TempData["Success"] = "Đặt phòng thành công!";
        return RedirectToAction("Detail", new { id = roomId });
    }
}