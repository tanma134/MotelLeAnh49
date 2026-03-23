using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using MotelLeAnh49.Models;
using Microsoft.AspNetCore.SignalR;
using MotelLeAnh49.Hubs;
public class RoomsController : Controller
{
    private readonly IRoomService _roomService;

    private readonly IWebHostEnvironment _env;
    private readonly IHubContext<RoomHub> _hubContext;

    public RoomsController(IRoomService roomService,
                        IWebHostEnvironment env,
                        IHubContext<RoomHub> hubContext)
    {
        _roomService = roomService;
        _env = env;
        _hubContext = hubContext;
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
    public async Task<IActionResult> Create(Room room, List<IFormFile> Images)
    {
        if (!ModelState.IsValid)
            return View(room);

        int adminId = HttpContext.Session.GetInt32("AdminId").Value;

        _roomService.CreateRoom(room, adminId, Images, _env.WebRootPath);

        await _hubContext.Clients.All.SendAsync("RoomUpdated");

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

        if (!_roomService.UpdateRoom(room, Images, DeletedImageIds, _env.WebRootPath))
            return NotFound();

        await _hubContext.Clients.All.SendAsync("RoomUpdated");

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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _roomService.DeleteRoom(id);

        await _hubContext.Clients.All.SendAsync("RoomUpdated");

        return RedirectToAction(nameof(Index));
    }

    // ================= SEARCH =================
    [HttpPost]
    public IActionResult Search(BookingViewModel model)
    {
        if (model.CheckIn.Date < DateTime.Today)
            return BadRequest("Ngày nhận phòng không hợp lệ");

        if (model.CheckOut <= model.CheckIn)
            return BadRequest("Ngày trả phòng phải sau ngày nhận phòng");

        var rooms = _roomService.SearchAvailableRooms(
            model.CheckIn,
            model.CheckOut,
            model.Adults,
            model.Children
        );

        return PartialView("_AvailableRooms", rooms);
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

}