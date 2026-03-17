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
                data = _customerService.SearchByIdentity(searchCCCD);
                ViewData["CurrentSearch"] = searchCCCD;
            }
            else
            {
                data = _customerService.GetAllCustomers();
            }

            return View(data);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Account account, Customer customer)
        {
            ModelState.Clear();

            if (string.IsNullOrEmpty(account.Username))
                ModelState.AddModelError("Account.Username", "Username is required!");

            if (string.IsNullOrEmpty(account.Password) || account.Password.Length < 6)
                ModelState.AddModelError("Account.Password", "Password must be at least 6 characters!");

            if (string.IsNullOrEmpty(customer.Email))
                ModelState.AddModelError("Email", "Email is required!");

            if (!ModelState.IsValid) return View(customer);

            try
            {
                account.Email = customer.Email;
                account.Password = _authService.HashPassword(account.Password);
                account.Role = "Customer";
                account.IsActive = true;

                _customerService.CreateCustomer(customer, account);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                if (msg.Contains("Phone"))
                {
                    ModelState.AddModelError("Phone", msg);
                }
                else if (msg.Contains("Identity"))
                {
                    ModelState.AddModelError("IdentityNumber", msg);
                }
                else if (msg.Contains("Username"))
                {
                    ModelState.AddModelError("Account.Username", msg);
                }
                else if (msg.Contains("Email"))
                {
                    ModelState.AddModelError("Email", "This email address has already been registered!");
                }
                else
                {
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
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer customer)
        {
            ModelState.Clear();

            if (id != customer.Id) return NotFound();

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
            var customer = _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _customerService.DeleteCustomer(id);

                TempData["Success"] = "Đã khóa tài khoản khách hàng thành công!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thực hiện khóa: " + ex.Message);

                var customer = _customerService.GetCustomerById(id);
                return View(customer);
            }
        }
    }
}