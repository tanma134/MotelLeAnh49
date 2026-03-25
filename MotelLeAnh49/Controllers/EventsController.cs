using BusinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using MotelLeAnh49.Models;

namespace MotelLeAnh49.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IEventRegistrationService _eventRegistrationService;

        public EventsController(IEventService eventService, IEventRegistrationService eventRegistrationService)
        {
            _eventService = eventService;
            _eventRegistrationService = eventRegistrationService;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetInt32("AdminId") != null;
        }

        // LIST (ADMIN)
        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var events = _eventService.GetAllEvents();
            return View(events);
        }

        // UC-52: VIEW EVENT LIST (GUEST)
        public IActionResult List()
        {
            var events = _eventService.GetAllEvents();
            return View(events);
        }

        // UC-53: VIEW EVENT DETAIL (USER)
        public IActionResult Details(int id)
        {
            var ev = _eventService.GetEventById(id);
            if (ev == null) return NotFound();

            var registrationId = HttpContext.Session.GetInt32("RegisteredEvent_" + id);
            ViewBag.RegistrationId = registrationId;

            return View(ev);
        }

        // UC-49: CREATE
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event ev, IFormFile? ImageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            // ── Validate ngày không được ở quá khứ ──
            if (ev.EventDate <= DateTime.Now)
                ModelState.AddModelError("EventDate", "⚠️ Ngày giờ diễn ra phải ở tương lai.");

            if (!ModelState.IsValid)
                return View(ev);

            // upload ảnh
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/events");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                ev.ImageUrl = "/images/events/" + fileName;
            }

            _eventService.CreateEvent(ev);

            return RedirectToAction(nameof(Index));
        }

        // UC-50: UPDATE
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var ev = _eventService.GetEventById(id);
            if (ev == null) return NotFound();

            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Event ev, IFormFile? ImageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            // ── Validate ngày không được ở quá khứ ──
            if (ev.EventDate <= DateTime.Now)
                ModelState.AddModelError("EventDate", "⚠️ Ngày giờ diễn ra phải ở tương lai.");

            if (!ModelState.IsValid)
            {
                // giữ lại ảnh cũ khi validate fail
                var existing2 = _eventService.GetEventById(ev.EventId);
                ev.ImageUrl = existing2?.ImageUrl;
                return View(ev);
            }

            var existing = _eventService.GetEventById(ev.EventId);
            if (existing == null)
                return NotFound();

            // upload ảnh mới
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/events");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                ev.ImageUrl = "/images/events/" + fileName;
            }
            else
            {
                ev.ImageUrl = existing.ImageUrl;
            }

            if (!_eventService.UpdateEvent(ev))
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // UC-51: DELETE
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            var ev = _eventService.GetEventById(id);
            if (ev == null) return NotFound();

            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            _eventService.DeleteEvent(id);

            return RedirectToAction(nameof(Index));
        }

        // UC-54: JOIN EVENT (USER)
        public IActionResult Join(int id)
        {
            var ev = _eventService.GetEventById(id);
            if (ev == null) return NotFound();

            var model = new JoinEventViewModel
            {
                EventId = ev.EventId,
                Title = ev.Title,
                EventDate = ev.EventDate,
                Location = ev.Location
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(JoinEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ev = _eventService.GetEventById(model.EventId);
            if (ev == null)
            {
                return NotFound();
            }

            var registration = _eventRegistrationService.Register(
                model.EventId,
                model.FullName,
                model.Email,
                model.Phone
            );

            // lưu session
            HttpContext.Session.SetInt32("RegisteredEvent_" + model.EventId, registration.Id);

            model.Title = ev.Title;
            model.EventDate = ev.EventDate;
            model.Location = ev.Location;
            model.RegistrationId = registration.Id;

            return View("JoinSuccess", model);
        }

        // UC-55: CANCEL EVENT REGISTRATION
        public IActionResult Cancel(int id)
        {
            var registration = _eventRegistrationService.GetById(id);
            if (registration == null)
                return NotFound();

            var ev = _eventService.GetEventById(registration.EventId);
            if (ev == null)
                return NotFound();

            var model = new JoinEventViewModel
            {
                EventId = ev.EventId,
                Title = ev.Title,
                EventDate = ev.EventDate,
                Location = ev.Location,
                FullName = registration.FullName,
                Email = registration.Email,
                Phone = registration.Phone,
                RegistrationId = registration.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Cancel")]
        public IActionResult CancelConfirmed(int id)
        {
            var success = _eventRegistrationService.Cancel(id);

            ViewBag.CancelSuccess = success;

            return View("CancelResult");
        }
    }
}
