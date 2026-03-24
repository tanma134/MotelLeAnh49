// BusinessLogic/Service/ChatService.cs
using DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Service
{
    public class ChatService : IChatService
    {
        private readonly IOpenAIService _openAI;
        private readonly IChatRepository _repo;
        private readonly IRoomRepository _roomRepo;
        private readonly IEventRepository _eventRepo;
        private readonly IServiceItemRepository _serviceItemRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IBookingRepository _bookingRepo; // 🔹 THÊM MỚI

        public ChatService(
     IOpenAIService openAI,
     IChatRepository repo,
     IRoomRepository roomRepo,
     IEventRepository eventRepo,
     IServiceItemRepository serviceItemRepo,
     ICustomerRepository customerRepo,
     IBookingRepository bookingRepo)
        {
            _openAI = openAI;
            _repo = repo;
            _roomRepo = roomRepo;
            _eventRepo = eventRepo;
            _serviceItemRepo = serviceItemRepo;
            _customerRepo = customerRepo;
            _bookingRepo = bookingRepo;
        }

        // Step 3: ProcessUserMessage — called by ChatController
        public async Task<string> ProcessUserMessageAsync(string userMessage, int? customerId)
        {
            // 🔹 Rooms
            var rooms = (await _roomRepo.GetAvailableRoomsAsync()).Take(5);
            var roomData = string.Join("\n", rooms.Select(r =>
        $"""
Room {r.RoomNumber}
- Loại phòng: {r.RoomType}
- Giá qua đêm: {r.OvernightPrice} VND
- Giá theo ngày: {r.DayPrice} VND
- Giá giờ đầu tiên: {r.FirstHourPrice} VND
- Giá giờ tiếp theo: {r.NextHourPrice} VND
- Giới hạn khách tối đa: {r.MaxGuests}
- Phí nếu thêm 1 khách: {r.ExtraGuestFee} VND
"""
        ));

            // 🔹 Events
            var events = (await _eventRepo.GetUpcomingEventsAsync()).Take(5);
            var eventData = string.Join("\n", events.Select(e =>
        $"""
Sự kiện: {e.Title}
- Địa điểm: {e.Location}
- Ngày: {e.EventDate:dd/MM/yyyy}
"""
        ));

            // 🔹 Services
            var services = (await _serviceItemRepo.GetAvailableServicesAsync()).Take(5);
            var serviceData = string.Join("\n", services.Select(s =>
        $"""
Dịch vụ: {s.Name}
- Giá: {s.Price} VND
"""
        ));

            // 🔹 Lịch sử chat (CHỈ của user này)
            string historyData = "";

            if (customerId.HasValue)
            {
                var history = await _repo.GetHistoryByCustomerIdAsync(customerId.Value, 5);

                historyData = string.Join("\n", history.Select(h =>
        $"""
User: {h.UserMessage}
AI: {h.AiResponse}
"""
        ));
            }


            // 🔹 THÊM MỚI: Lịch sử đặt phòng
            string bookingData = "";
            if (customerId.HasValue)
            {
                var bookings = await _bookingRepo.GetByCustomerIdAsync(customerId.Value);

                if (bookings != null && bookings.Any())
                {
                    bookingData = "Lịch sử đặt phòng của khách hàng:\n";
                    bookingData += string.Join("\n", bookings.Select(b =>
                        $"""
Đơn đặt phòng #{b.Id}
- Phòng: {b.Room?.RoomNumber ?? "N/A"}
- Loại phòng: {b.Room?.RoomType ?? "N/A"}
- Nhận phòng: {b.CheckIn:dd/MM/yyyy HH:mm}
- Trả phòng: {b.CheckOut:dd/MM/yyyy HH:mm}
- Trạng thái: {b.Status}
- Tên khách: {b.FullName}
- SĐT: {b.Phone}
- Email: {b.Email}
"""
                    ));
                }
                else
                {
                    bookingData = "Khách hàng không có lịch sử đặt phòng.";
                }
            }
            else
            {
                bookingData = "Khách chưa đăng nhập, không có lịch sử đặt phòng.";
            }
            string customerName = "";

            if (customerId.HasValue)
            {
                var customer = await _customerRepo.GetByIdAsync(customerId.Value);
                customerName = customer?.FullName ?? "";
            }
            string greeting = "";

            if (!string.IsNullOrEmpty(customerName))
            {
                greeting = $"Khách hàng tên: {customerName}. Hãy xưng hô phù hợp (anh/chị).";
            }
            // 🔹 Prompt
            var prompt = $"""
Bạn là AI lễ tân của MotelLeAnh49.
{greeting}
Lịch sử hội thoại:
{historyData}

Phòng:
{roomData}

Sự kiện:
{eventData}

Dịch vụ:
{serviceData}

Lịch sử đặt phòng:
{bookingData}
Câu hỏi:
{userMessage}



QUY TẮC:
- Nếu có tên khách hàng, BẮT BUỘC phải chào theo tên.
- Không được dùng "anh/chị" chung chung.
- Ví dụ: "Chào anh Vinh..."
- Nếu khách hỏi về lịch sử đặt phòng mà không có lịch sử, hãy nói rằng khách chưa có đặt phòng nào.
- Khi có lịch sử đặt phòng, có thể tham khảo thông tin để trả lời các câu hỏi liên quan.
- Trả lời tự nhiên, thân thiện, bằng tiếng Việt.
- Nếu phù hợp hãy gợi ý phòng, dịch vụ hoặc sự kiện
""";

            var aiResponse = await _openAI.SendPromptAsync(prompt);

            // 🔹 Lưu DB nếu login
            if (customerId.HasValue)
            {
                await _repo.SaveChatAsync(customerId.Value, userMessage, aiResponse);
            }

            return aiResponse;
        }
    }
}