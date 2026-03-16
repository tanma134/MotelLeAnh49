using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using MotelLeAnh49.Data;

namespace DataAccess.Repositories
{
    public class EventRegistrationRepository : IEventRegistrationRepository
    {
        private readonly MotelDbContext _context;

        public EventRegistrationRepository(MotelDbContext context)
        {
            _context = context;
        }

        public EventRegistration Add(EventRegistration registration)
        {
            _context.EventRegistrations.Add(registration);
            _context.SaveChanges();
            return registration;
        }

        public EventRegistration? GetById(int id)
        {
            return _context.EventRegistrations
                .FirstOrDefault(r => r.Id == id);
        }

        public void Update(EventRegistration registration)
        {
            _context.EventRegistrations.Update(registration);
            _context.SaveChanges();
        }

        public List<EventRegistration> GetByEvent(int eventId)
        {
            return _context.EventRegistrations
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.RegisteredAt)
                .ToList();
        }
    }
}

