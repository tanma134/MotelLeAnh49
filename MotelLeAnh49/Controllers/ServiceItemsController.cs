using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using MotelLeAnh49.Models;

namespace MotelLeAnh49.Controllers
{
    public class ServiceItemsController : Controller
    {
        private readonly IServiceItemService _serviceItemService;

        public ServiceItemsController(IServiceItemService serviceItemService)
        {
            _serviceItemService = serviceItemService;
        }

        // GET: ServiceItems
        public IActionResult Index()
        {
            var services = _serviceItemService.GetAllServices();
            return View(services);
        }

        // GET: ServiceItems/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceItem serviceItem, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/services");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    serviceItem.ImageUrl = "/images/services/" + fileName;
                }

                _serviceItemService.CreateService(serviceItem);
                return RedirectToAction(nameof(Index));
            }

            return View(serviceItem);
        }

        // GET: ServiceItems/Edit/5
        public IActionResult Edit(int id)
        {
            var service = _serviceItemService.GetServiceById(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceItem serviceItem, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/services");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    serviceItem.ImageUrl = "/images/services/" + fileName;
                }

                _serviceItemService.UpdateService(serviceItem);
                return RedirectToAction(nameof(Index));
            }

            return View(serviceItem);
        }

        // GET: ServiceItems/Delete/5
        public IActionResult Delete(int id)
        {
            var service = _serviceItemService.GetServiceById(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _serviceItemService.DeleteService(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: ServiceItems/AddToBooking
        [HttpPost]
        public IActionResult AddToBooking(int bookingId, int serviceItemId, int quantity)
        {
            _serviceItemService.AddServiceToBooking(bookingId, serviceItemId, quantity);
            return RedirectToAction("Details", "Bookings", new { id = bookingId });
        }
        
        [HttpPost]
        public IActionResult RemoveFromBooking(int id, int bookingId)
        {
            _serviceItemService.RemoveServiceFromBooking(id);
            return RedirectToAction("Details", "Bookings", new { id = bookingId });
        }
    }
}