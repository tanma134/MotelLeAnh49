
using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Data;
using MotelLeAnh49.Models;

namespace DataAccess.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly MotelDbContext _context;

        public RoomRepository(MotelDbContext context)
        {
            _context = context;
        }

        public List<Room> GetAll()
        {
            return _context.Rooms
                .Include(r => r.Admin)
                .Include(r => r.RoomImages)
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        public Room? GetById(int id)
        {
            return _context.Rooms
                .Include(r => r.RoomImages)
                .FirstOrDefault(r => r.Id == id);
        }

        public void Add(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
        }

        public void Update(Room room)
        {
            _context.Rooms.Update(room);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var room = _context.Rooms
                .Include(r => r.RoomImages)
                .FirstOrDefault(r => r.Id == id);

            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
        }

        public List<Room> SearchAvailable(DateTime checkIn, DateTime checkOut, int totalGuests)
        {
            return _context.Rooms
                .Where(r =>
                    r.MaxGuests >= totalGuests &&
                    !_context.Bookings.Any(b =>
                        b.RoomId == r.Id &&
                        b.CheckIn < checkOut &&
                        b.CheckOut > checkIn
                    )
                )
                .ToList();
        }

        public bool IsAvailable(int roomId, DateTime checkIn, DateTime checkOut)
        {
            return !_context.Bookings.Any(b =>
                b.RoomId == roomId &&
                b.CheckIn < checkOut &&
                b.CheckOut > checkIn
            );
        }

        public async Task<List<Room>> GetAvailableRoomsAsync()
        {
            var now = DateTime.Now;

            return await _context.Rooms
                .Where(r => !_context.Bookings
                    .Any(b => b.RoomId == r.Id && b.CheckOut > now))
                .ToListAsync();
        }
    }
}