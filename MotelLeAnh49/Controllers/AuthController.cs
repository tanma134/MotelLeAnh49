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
            {
                ViewBag.Error = "Tài khoản chưa kích hoạt";
                return View();
            }

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

            // ✅ CHECK PHONE
            if (!string.IsNullOrWhiteSpace(customer.Phone)
                && _authService.IsPhoneExist(customer.Phone))
            {
                ViewBag.Error = "Số điện thoại đã được sử dụng";
                return View();
            }

            // ✅ CHECK CCCD / CMND
            if (!string.IsNullOrWhiteSpace(customer.IdentityNumber)
                && _authService.IsIdentityExist(customer.IdentityNumber))
            {
                ViewBag.Error = "CMND/CCCD đã tồn tại";
                return View();
            }

            // 🔥 Normalize nhẹ (tránh lỗi space)
            customer.Phone = customer.Phone?.Trim();
            customer.IdentityNumber = customer.IdentityNumber?.Trim();

            string otp = _authService.GenerateOTP();

            // Lưu OTP
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTPTime", DateTime.Now.ToString());

            // 🔥 Lưu FULL dữ liệu
            HttpContext.Session.SetString("RegUsername", account.Username);
            HttpContext.Session.SetString("RegEmail", account.Email);
            HttpContext.Session.SetString("RegPassword", account.Password);

            HttpContext.Session.SetString("RegFullName", customer.FullName);
            HttpContext.Session.SetString("RegPhone", customer.Phone ?? "");
            HttpContext.Session.SetString("RegAddress", customer.Address ?? "");
            HttpContext.Session.SetString("RegIdentity", customer.IdentityNumber ?? "");

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

            // 🔥 LẤY FULL DATA
            var account = new Account
            {
                Username = HttpContext.Session.GetString("RegUsername"),
                Email = HttpContext.Session.GetString("RegEmail"),
                Password = HttpContext.Session.GetString("RegPassword"),
                Role = "Customer",
                IsActive = true
            };

            var customer = new Customer
            {
                FullName = HttpContext.Session.GetString("RegFullName"),
                Phone = HttpContext.Session.GetString("RegPhone"),
                Address = HttpContext.Session.GetString("RegAddress"),
                IdentityNumber = HttpContext.Session.GetString("RegIdentity"),
                Email = HttpContext.Session.GetString("RegEmail")
            };

            _authService.Register(account, customer);

            // clear session
            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("OTPTime");
            HttpContext.Session.Remove("RegUsername");
            HttpContext.Session.Remove("RegEmail");
            HttpContext.Session.Remove("RegPassword");
            HttpContext.Session.Remove("RegFullName");
            HttpContext.Session.Remove("RegPhone");
            HttpContext.Session.Remove("RegAddress");
            HttpContext.Session.Remove("RegIdentity");

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