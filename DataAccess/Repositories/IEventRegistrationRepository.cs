using System.Collections.Generic;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IEventRegistrationRepository
    {
        EventRegistration Add(EventRegistration registration);
        EventRegistration? GetById(int id);
        void Update(EventRegistration registration);
        List<EventRegistration> GetByEvent(int eventId);
    }
}

