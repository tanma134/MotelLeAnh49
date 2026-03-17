using BusinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace MotelLeAnh49.Controllers
{
    public class CustomerProfileController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerProfileController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult Profile()
        {

            var userId = HttpContext.Session.GetInt32("CustomerId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var customer = _customerService.GetProfile(userId.Value);

            return View(customer);
        }
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("CustomerId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var customer = _customerService.GetProfile(userId.Value);

            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            var userId = HttpContext.Session.GetInt32("CustomerId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Đảm bảo đúng user
            customer.Id = userId.Value;

            try
            {
                // 🆕 Kiểm tra email trùng
                if (_customerService.IsEmailDuplicate(customer.Email, customer.Id))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng bởi khách hàng khác!");
                    return View(customer);
                }

                // 🆕 Kiểm tra phone trùng
                if (_customerService.IsPhoneDuplicate(customer.Phone, customer.Id))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại này đã được sử dụng bởi khách hàng khác!");
                    return View(customer);
                }

                _customerService.UpdateCustomer(customer);
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(customer);
            }
        }
    }
}
