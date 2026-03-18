using BusinessLogic.Interfaces;
using DataAccess.Repositories;
using MotelLeAnh49.Models;
using DataAccess.Models;

namespace BusinessLogic.Services
{
    public class ServiceItemService : IServiceItemService
    {
        private readonly IServiceItemRepository _serviceItemRepository;
        private readonly IBookingServiceRepository _bookingServiceRepository;

        public ServiceItemService(IServiceItemRepository serviceItemRepository, 
                                 IBookingServiceRepository bookingServiceRepository)
        {
            _serviceItemRepository = serviceItemRepository;
            _bookingServiceRepository = bookingServiceRepository;
        }

        public List<ServiceItem> GetAllServices()
        {
            return _serviceItemRepository.GetAll();
        }

        public ServiceItem? GetServiceById(int id)
        {
            return _serviceItemRepository.GetById(id);
        }

        public void CreateService(ServiceItem item)
        {
            _serviceItemRepository.Add(item);
        }

        public void UpdateService(ServiceItem item)
        {
            _serviceItemRepository.Update(item);
        }

        public void DeleteService(int id)
        {
            _serviceItemRepository.Delete(id);
        }

        public List<BookingServiceItem> GetServicesByBooking(int bookingId)
        {
            return _bookingServiceRepository.GetByBookingId(bookingId);
        }

        public void AddServiceToBooking(int bookingId, int serviceId, int quantity)
        {
            var service = _serviceItemRepository.GetById(serviceId);
            if (service != null)
            {
                var bookingService = new BookingServiceItem
                {
                    BookingId = bookingId,
                    ServiceItemId = serviceId,
                    Quantity = quantity,
                    PriceAtBooking = service.Price
                };
                _bookingServiceRepository.Add(bookingService);
            }
        }

        public void RemoveServiceFromBooking(int id)
        {
            _bookingServiceRepository.Remove(id);
        }

        public decimal CalculateTotalServiceCost(int bookingId)
        {
            var services = _bookingServiceRepository.GetByBookingId(bookingId);
            return services.Sum(s => s.PriceAtBooking * s.Quantity);
        }
    }
}