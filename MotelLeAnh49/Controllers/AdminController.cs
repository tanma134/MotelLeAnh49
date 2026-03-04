using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var admin = _adminService.Login(username, password);

        if (admin == null)
        {
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            return View();
        }

        HttpContext.Session.SetInt32("AdminId", admin.Id);
        HttpContext.Session.SetString("AdminName", admin.FullName ?? "Admin");

        return RedirectToAction("Index", "Rooms");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    public IActionResult TestHash()
    {
        var hash = _adminService.TestHash("1990Huy");
        return Content(hash);
    }
}