using MotelLeAnh49.Models;

namespace DataAccess.Repositories
{
    public interface IEventRepository
    {
        List<Event> GetAll();
        Event? GetById(int id);
        void Add(Event ev);
        void Update(Event ev);
        void Delete(int id);
        Task<List<Event>> GetUpcomingEventsAsync();

    }
}
