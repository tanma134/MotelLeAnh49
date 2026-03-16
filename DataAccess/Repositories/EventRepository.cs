using MotelLeAnh49.Data;
using MotelLeAnh49.Models;

namespace DataAccess.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly MotelDbContext _context;
        public EventRepository(MotelDbContext context)
        {
            _context = context;
        }
        public List<Event> GetAll()
        {
            return _context.Events
                .OrderBy(e => e.EventDate)
                .ToList();
        }
        public Event? GetById(int id)
        {
            return _context.Events.FirstOrDefault(e => e.EventId == id);
        }
        public void Add(Event ev)
        {
            _context.Events.Add(ev);
            _context.SaveChanges();
        }
        public void Update(Event ev)
        {
            _context.Events.Update(ev);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.EventId == id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                _context.SaveChanges();
            }
        }
    }
}
