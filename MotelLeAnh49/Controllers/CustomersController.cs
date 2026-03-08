using BusinessLogic.Service;
using DataAccess.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MotelLeAnh49.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly AuthService _authService;

        public CustomersController(ICustomerService customerService, AuthService authService)
        {
            _customerService = customerService;
            _authService = authService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                context.Result = RedirectToAction("Login", "Admin");
            }

            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            var data = _customerService.GetAllCustomers();
            return View(data);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Account account, Customer customer)
        {
            // 1. Xóa sạch mọi lỗi về validation của các object và email
            ModelState.Clear(); // Cách mạnh tay nhất: Xóa sạch lỗi cũ để mình tự check bằng tay

            // 2. Tự mình check những cái thực sự cần thiết
            if (string.IsNullOrEmpty(account.Username) || string.IsNullOrEmpty(account.Password) || string.IsNullOrEmpty(customer.Email))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ Username, Mật khẩu và Email!");
                return View(customer);
            }

            try
            {
                // 3. Logic xử lý như cũ
                account.Email = customer.Email;
                account.Password = _authService.HashPassword(account.Password);
                account.Role = "Customer";
                account.IsActive = true;

                _customerService.CreateCustomer(customer, account);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                return View(customer);
            }
        }


        // GET: Customer/Edit/1
        public IActionResult Edit(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy ID này trong DB
            }
            return View(customer);
        }

        // POST: Customer/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer customer)
        {

            ModelState.Remove("Account");

            if (id != customer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _customerService.UpdateCustomer(customer);
                    TempData["Success"] = "Cập nhật thông tin khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi DB rồi bro: " + ex.Message);
                }
            }

            // --- ĐOẠN NÀY ĐỂ BẮT BỆNH ---
            // Nếu nó chạy xuống đây, nghĩa là ModelState bị lỗi. 
            // Ta lấy hết lỗi đó bỏ vào ViewBag để cái khung DEBUG ở UI nó hiện lên.
            ViewBag.DebugErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return View(customer);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            // 1. Tìm thông tin khách hàng để hiện lên trang xác nhận
            var customer = _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                // 2. Gọi xuống Service để thực hiện logic "IsActive = false"
                _customerService.DeleteCustomer(id);

                // Thông báo thành công (nếu ba có dùng TempData)
                TempData["Success"] = "Đã khóa tài khoản khách hàng thành công!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Nếu lỗi, quay lại trang xác nhận và hiện thông báo lỗi
                ModelState.AddModelError("", "Lỗi khi thực hiện khóa: " + ex.Message);

                // Cần lấy lại dữ liệu để hiển thị lại View
                var customer = _customerService.GetCustomerById(id);
                return View(customer);
            }
        }
    }
}
