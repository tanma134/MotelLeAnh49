using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Service;
using DataAccess.Models;

namespace MotelLeAnh49.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly EmailService _emailService;

        public AuthController(AuthService authService, EmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        // LOGIN PAGE
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var account = _authService.Login(username, password);

            if (account == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            // Lưu session
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetString("Role", account.Role);
            HttpContext.Session.SetString("FullName", account.Customer.FullName);

            return RedirectToAction("Index", "Home");
        }

        // REGISTER PAGE
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER
        [HttpPost]
        public IActionResult Register(Account account, Customer customer)
        {
            _authService.Register(account, customer);

            string otp = _authService.GenerateOTP();

            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("Email", account.Email);

            // gửi OTP
            _emailService.SendOTP(account.Email, otp);

            return RedirectToAction("VerifyOTP");
        }

        // VERIFY OTP PAGE
        public IActionResult VerifyOTP()
        {
            return View();
        }

        // VERIFY OTP
        [HttpPost]
        public IActionResult VerifyOTP(string otp)
        {
            string? sessionOtp = HttpContext.Session.GetString("OTP");

            if (sessionOtp == null || otp != sessionOtp)
            {
                ViewBag.Error = "OTP không đúng";
                return View();
            }

            // activate account
            string email = HttpContext.Session.GetString("Email");
            _authService.ActivateAccount(email);

            ViewBag.Success = "Xác thực thành công! Đang chuyển đến trang login...";

            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var account = _authService.GetByEmail(email);

            if (account == null)
            {
                ViewBag.Error = "Email không tồn tại";
                return View();
            }

            string otp = _authService.GenerateOTP();

            HttpContext.Session.SetString("ResetOTP", otp);
            HttpContext.Session.SetString("ResetEmail", email);

            _emailService.SendOTP(email, otp);

            return RedirectToAction("VerifyResetOTP");
        }
        public IActionResult VerifyResetOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyResetOTP(string otp)
        {
            string sessionOtp = HttpContext.Session.GetString("ResetOTP");

            if (otp != sessionOtp)
            {
                ViewBag.Error = "OTP không đúng";
                return View();
            }

            return RedirectToAction("ResetPassword");
        }
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string password)
        {
            string email = HttpContext.Session.GetString("ResetEmail");

            _authService.ResetPassword(email, password);

            return RedirectToAction("Login");
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}