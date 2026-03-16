using System.Collections.Generic;
using DataAccess.Models;

namespace BusinessLogic.Service
{
    public interface IEventRegistrationService
    {
        EventRegistration Register(int eventId, string fullName, string email, string phone);
        bool Cancel(int registrationId);
        EventRegistration? GetById(int id);
        List<EventRegistration> GetByEvent(int eventId);
    }
}

