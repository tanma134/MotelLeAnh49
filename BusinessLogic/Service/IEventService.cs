using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using MotelLeAnh49.Models;

namespace BusinessLogic.Service
{
    public interface IEventService
    {
        List<Event> GetAllEvents();
        Event? GetEventById(int id);
        void CreateEvent(Event ev);
        bool UpdateEvent(Event ev);
        void DeleteEvent(int id);
    }
}
