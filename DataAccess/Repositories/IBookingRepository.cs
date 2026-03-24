using MotelLeAnh49.Models;

namespace DataAccess.Repositories
{
    public interface IBookingRepository
    {
        IEnumerable<Booking> GetAll();

        Booking GetById(int id);

        void Add(Booking booking);

        void Update(Booking booking);

        void Delete(int id);

        void Save();
        Task<List<Booking>> GetByCustomerIdAsync(int customerId);
    }
}