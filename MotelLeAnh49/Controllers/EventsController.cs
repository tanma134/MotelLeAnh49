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
        public IActionResult Create(Event ev)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");
            if (!ModelState.IsValid)
                return View(ev);
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
        public IActionResult Edit(Event ev)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");
            if (!ModelState.IsValid)
                return View(ev);
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

            var registration = _eventRegistrationService.Register(model.EventId, model.FullName, model.Email, model.Phone);

            model.Title = ev.Title;
            model.EventDate = ev.EventDate;
            model.Location = ev.Location;
            model.RegistrationId = registration.Id;

            return View("JoinSuccess", model);
        }

        // UC-55: CANCEL EVENT REGISTRATION (USER)
        public IActionResult Cancel(int id)
        {
            var registration = _eventRegistrationService.GetById(id);
            if (registration == null)
            {
                return NotFound();
            }

            var ev = _eventService.GetEventById(registration.EventId);
            if (ev == null)
            {
                return NotFound();
            }

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
