using System.Collections.Generic;
using DataAccess.Models;
using DataAccess.Repositories;

namespace BusinessLogic.Service
{
    public class EventRegistrationService : IEventRegistrationService
    {
        private readonly IEventRegistrationRepository _registrationRepository;

        public EventRegistrationService(IEventRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public EventRegistration Register(int eventId, string fullName, string email, string phone)
        {
            var registration = new EventRegistration
            {
                EventId = eventId,
                FullName = fullName,
                Email = email,
                Phone = phone
            };

            return _registrationRepository.Add(registration);
        }

        public bool Cancel(int registrationId)
        {
            var registration = _registrationRepository.GetById(registrationId);
            if (registration == null || registration.IsCancelled)
            {
                return false;
            }

            registration.IsCancelled = true;
            registration.CancelledAt = System.DateTime.UtcNow;
            _registrationRepository.Update(registration);
            return true;
        }

        public EventRegistration? GetById(int id)
        {
            return _registrationRepository.GetById(id);
        }

        public List<EventRegistration> GetByEvent(int eventId)
        {
            return _registrationRepository.GetByEvent(eventId);
        }
    }
}

