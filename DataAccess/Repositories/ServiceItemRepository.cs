using MotelLeAnh49.Data;
using MotelLeAnh49.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class ServiceItemRepository : IServiceItemRepository
    {
        private readonly MotelDbContext _context;

        public ServiceItemRepository(MotelDbContext context)
        {
            _context = context;
        }

        public List<ServiceItem> GetAll()
        {
            return _context.ServiceItems.ToList();
        }

        public ServiceItem? GetById(int id)
        {
            return _context.ServiceItems.Find(id);
        }

        public void Add(ServiceItem serviceItem)
        {
            _context.ServiceItems.Add(serviceItem);
            _context.SaveChanges();
        }

        public void Update(ServiceItem serviceItem)
        {
            _context.ServiceItems.Update(serviceItem);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var item = _context.ServiceItems.Find(id);
            if (item != null)
            {
                _context.ServiceItems.Remove(item);
                _context.SaveChanges();
            }
        }
        public async Task < List < ServiceItem >> GetAvailableServicesAsync()
{
    return await _context.ServiceItems
        .Where(s => s.IsAvailable)
        .ToListAsync();
    }
}
}