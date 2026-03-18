using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBookingServiceRepository
    {
        List<BookingServiceItem> GetByBookingId(int bookingId);
        void Add(BookingServiceItem item);
        void Remove(int id);
        void UpdateQuantity(int id, int quantity);
    }
}