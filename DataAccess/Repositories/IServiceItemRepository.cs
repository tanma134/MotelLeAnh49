using MotelLeAnh49.Models;

namespace DataAccess.Repositories
{
    public interface IServiceItemRepository
    {
        List<ServiceItem> GetAll();
        ServiceItem? GetById(int id);
        void Add(ServiceItem serviceItem);
        void Update(ServiceItem serviceItem);
        void Delete(int id);
        Task<List<ServiceItem>> GetAvailableServicesAsync();
    }

    }