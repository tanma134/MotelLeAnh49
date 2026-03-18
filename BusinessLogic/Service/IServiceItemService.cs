using MotelLeAnh49.Models;
using DataAccess.Models;

namespace BusinessLogic.Interfaces
{
    public interface IServiceItemService
    {
        // CRUD Service Items
        List<ServiceItem> GetAllServices();
        ServiceItem? GetServiceById(int id);
        void CreateService(ServiceItem item);
        void UpdateService(ServiceItem item);
        void DeleteService(int id);

        // Booking Services
        List<BookingServiceItem> GetServicesByBooking(int bookingId);
        void AddServiceToBooking(int bookingId, int serviceId, int quantity);
        void RemoveServiceFromBooking(int id);
        decimal CalculateTotalServiceCost(int bookingId);
    }
}