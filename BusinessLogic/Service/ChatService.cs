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

        public ChatService(
     IOpenAIService openAI,
     IChatRepository repo,
     IRoomRepository roomRepo,
     IEventRepository eventRepo,
     IServiceItemRepository serviceItemRepo,
     ICustomerRepository customerRepo)
        {
            _openAI = openAI;
            _repo = repo;
            _roomRepo = roomRepo;
            _eventRepo = eventRepo;
            _serviceItemRepo = serviceItemRepo;
            _customerRepo = customerRepo;
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

Câu hỏi:
{userMessage}

QUY TẮC:
- Nếu có tên khách hàng, BẮT BUỘC phải chào theo tên.
- Không được dùng "anh/chị" chung chung.
- Ví dụ: "Chào anh Vinh..."

Trả lời tự nhiên, thân thiện, bằng tiếng Việt.
Nếu phù hợp hãy gợi ý phòng, dịch vụ hoặc sự kiện.
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