using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Data;
using MotelLeAnh49.Models;

namespace DataAccess.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly MotelDbContext _context;

        public BookingRepository(MotelDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Booking> GetAll()
        {
            return _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Customer)
                .ToList();
        }

        public Booking GetById(int id)
        {
            return _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.Id == id);
        }

        public void Add(Booking booking)
        {
            _context.Bookings.Add(booking);
        }

        public void Update(Booking booking)
        {
            _context.Bookings.Update(booking);
        }

        public void Delete(int id)
        {
            var booking = _context.Bookings.Find(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task<List<Booking>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .Include(b => b.Room) // 🔹 QUAN TRỌNG: Phải eager load Room
                .OrderByDescending(b => b.CheckIn) // Sắp xếp theo ngày nhận phòng mới nhất
                .ToListAsync();
        }
    }
}