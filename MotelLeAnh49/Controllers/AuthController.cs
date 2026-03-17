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

            if (!account.IsActive)
                return null;

            // Lưu session
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetString("Role", account.Role);
            HttpContext.Session.SetString("FullName", account.Customer.FullName);
            HttpContext.Session.SetString("Email", account.Email);

            HttpContext.Session.SetInt32("CustomerId", account.Customer.Id);

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
            if (_authService.IsUsernameExist(account.Username))
            {
                ViewBag.Error = "Username đã tồn tại";
                return View();
            }

            if (_authService.IsEmailExist(account.Email))
            {
                ViewBag.Error = "Email đã được sử dụng";
                return View();
            }

            string otp = _authService.GenerateOTP();

            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTPTime", DateTime.Now.ToString());

            HttpContext.Session.SetString("RegUsername", account.Username);
            HttpContext.Session.SetString("RegEmail", account.Email);
            HttpContext.Session.SetString("RegPassword", account.Password);
            HttpContext.Session.SetString("RegFullName", customer.FullName);

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
            string? otpTimeString = HttpContext.Session.GetString("OTPTime");

            if (sessionOtp == null || otpTimeString == null)
            {
                ViewBag.Error = "OTP không hợp lệ";
                return View();
            }

            DateTime otpTime = DateTime.Parse(otpTimeString);

            if ((DateTime.Now - otpTime).TotalMinutes > 5)
            {
                ViewBag.Error = "OTP đã hết hạn";
                return View();
            }

            if (otp != sessionOtp)
            {
                ViewBag.Error = "OTP không đúng";
                return View();
            }

            // lấy dữ liệu đăng ký
            var username = HttpContext.Session.GetString("RegUsername");
            var email = HttpContext.Session.GetString("RegEmail");
            var password = HttpContext.Session.GetString("RegPassword");
            var fullName = HttpContext.Session.GetString("RegFullName");

            var account = new Account
            {
                Username = username,
                Email = email,
                Password = password
            };

            var customer = new Customer
            {
                FullName = fullName
            };

            _authService.Register(account, customer);

            // clear session
            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("OTPTime");
            HttpContext.Session.Remove("RegUsername");
            HttpContext.Session.Remove("RegEmail");
            HttpContext.Session.Remove("RegPassword");
            HttpContext.Session.Remove("RegFullName");

            return RedirectToAction("Login");
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