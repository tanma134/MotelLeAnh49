using DataAccess.Models;
using MotelLeAnh49.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class BookingServiceRepository : IBookingServiceRepository
    {
        private readonly MotelDbContext _context;

        public BookingServiceRepository(MotelDbContext context)
        {
            _context = context;
        }

        public List<BookingServiceItem> GetByBookingId(int bookingId)
        {
            return _context.BookingServiceItems
                .Include(bs => bs.ServiceItem)
                .Where(bs => bs.BookingId == bookingId)
                .ToList();
        }

        public void Add(BookingServiceItem item)
        {
            _context.BookingServiceItems.Add(item);
            _context.SaveChanges();
        }

        public void Remove(int id)
        {
            var item = _context.BookingServiceItems.Find(id);
            if (item != null)
            {
                _context.BookingServiceItems.Remove(item);
                _context.SaveChanges();
            }
        }

        public void UpdateQuantity(int id, int quantity)
        {
            var item = _context.BookingServiceItems.Find(id);
            if (item != null)
            {
                item.Quantity = quantity;
                _context.SaveChanges();
            }
        }
    }
}