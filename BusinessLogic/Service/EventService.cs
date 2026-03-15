using DataAccess.Repositories;
using MotelLeAnh49.Models;

namespace BusinessLogic.Service
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public List<Event> GetAllEvents()
        {
            return _eventRepository.GetAll();
        }
        public Event? GetEventById(int id)
        {
            return _eventRepository.GetById(id);
        }
        public void CreateEvent(Event ev)
        {
            _eventRepository.Add(ev);
        }
        public bool UpdateEvent(Event ev)
        {
            var existing = _eventRepository.GetById(ev.EventId);
            if (existing == null) return false;
            existing.Title = ev.Title;
            existing.Description = ev.Description;
            existing.Location = ev.Location;
            existing.EventDate = ev.EventDate;
            existing.ImageUrl = ev.ImageUrl;
            existing.City = ev.City;
            _eventRepository.Update(existing);
            return true;
        }
        public void DeleteEvent(int id)
        {
            _eventRepository.Delete(id);
        }
    }
}
