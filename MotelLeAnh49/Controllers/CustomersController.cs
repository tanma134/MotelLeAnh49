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

        public IActionResult Index(string searchCCCD)
        {
            IEnumerable<Customer> data;

            if (!string.IsNullOrEmpty(searchCCCD))
            {
                // Gọi hàm tìm kiếm mới bạn vừa thêm vào Service/Repo
                data = _customerService.SearchByIdentity(searchCCCD);

                // Lưu lại để ô Input không bị mất chữ sau khi Load trang
                ViewData["CurrentSearch"] = searchCCCD;
            }
            else
            {
                // Chạy như cũ nếu không search gì
                data = _customerService.GetAllCustomers();
            }

            return View(data);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Account account, Customer customer)
        {
            // 1. Xóa lỗi mặc định để tự kiểm soát
            ModelState.Clear();

            // 2. Check thủ công các trường bắt buộc & Mật khẩu 6 số
            if (string.IsNullOrEmpty(account.Username))
                ModelState.AddModelError("Account.Username", "Username is required!");

            if (string.IsNullOrEmpty(account.Password) || account.Password.Length < 6)
                ModelState.AddModelError("Account.Password", "Password must be at least 6 characters!");

            if (string.IsNullOrEmpty(customer.Email))
                ModelState.AddModelError("Email", "Email is required!");

            // Nếu có lỗi định dạng ở trên thì trả về luôn, chưa cần xuống DB
            if (!ModelState.IsValid) return View(customer);

            try
            {
                // 3. Chuẩn bị dữ liệu
                account.Email = customer.Email;
                account.Password = _authService.HashPassword(account.Password);
                account.Role = "Customer";
                account.IsActive = true;

                // 4. Gọi Service (Nơi sẽ gọi Repo để check trùng Phone, CCCD, Username)
                _customerService.CreateCustomer(customer, account);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 5. "Bắt" lỗi trùng từ Repo và ném vào đúng ô nhập liệu
                string msg = ex.Message;

                if (msg.Contains("Phone"))
                {
                    ModelState.AddModelError("Phone", msg);
                }
                else if (msg.Contains("Identity")) // Khớp với chữ "Identity Number" trong Exception Repo
                {
                    ModelState.AddModelError("IdentityNumber", msg);
                }
                else if (msg.Contains("Username")) // Giả sử Repo Account cũng throw lỗi "Username exists"
                {
                    ModelState.AddModelError("Account.Username", msg);
                }
                else if (msg.Contains("Email"))
                {
                    // Phải khớp với tên trường trong Model hoặc View của bạn
                    ModelState.AddModelError("Email", "This email address has already been registered!");
                }
                else
                {
                    // Các lỗi hệ thống khác thì hiện trên cùng
                    ModelState.AddModelError("", "System error: " + msg);
                }

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
            // 1. Xóa sạch lỗi tự động của các object liên quan để mình tự kiểm soát
            ModelState.Clear();

            if (id != customer.Id) return NotFound();

            // 2. Check thủ công các trường bắt buộc
            if (string.IsNullOrEmpty(customer.Phone))
                ModelState.AddModelError("Phone", "Phone is require");

            if (string.IsNullOrEmpty(customer.IdentityNumber))
                ModelState.AddModelError("IdentityNumber", "IdentityNumber is require");

            if (ModelState.IsValid)
            {
                try
                {
                    _customerService.UpdateCustomer(customer);
                    TempData["Success"] = "Cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // 3. Phân loại lỗi từ Service quăng lên (Trùng Phone, CCCD...)
                    string msg = ex.Message;
                    if (msg.Contains("Phone"))
                        ModelState.AddModelError("Phone", "Phone Number exists");
                    else if (msg.Contains("Identity"))
                        ModelState.AddModelError("IdentityNumber", "Identity Number exists");
                    else if (msg.Contains("Email"))
                        ModelState.AddModelError("Email", "This email address has already been registered!");
                    else
                        ModelState.AddModelError("", "Lỗi hệ thống: " + msg);
                }
            }

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
