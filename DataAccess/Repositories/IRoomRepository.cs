
using MotelLeAnh49.Models;

namespace DataAccess.Repositories
{
    public interface IRoomRepository
    {
        List<Room> GetAll();
        Room? GetById(int id);
        void Add(Room room);
        void Update(Room room);
        void Delete(int id);
        List<Room> SearchAvailable(DateTime checkIn, DateTime checkOut, int totalGuests);
        bool IsAvailable(int roomId, DateTime checkIn, DateTime checkOut);
        Task<List<Room>> GetAvailableRoomsAsync();
    }
}