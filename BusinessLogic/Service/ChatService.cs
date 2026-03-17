// BusinessLogic/Service/ChatService.cs
using DataAccess.Repositories;

namespace BusinessLogic.Service
{
    public class ChatService : IChatService
    {
        private readonly IOpenAIService _openAI;
        private readonly IChatRepository _repo;
        private readonly IRoomRepository _roomRepo;
        private readonly IEventRepository _eventRepo;
        public ChatService(
     IOpenAIService openAI,
     IChatRepository repo,
     IRoomRepository roomRepo,
     IEventRepository eventRepo)
        {
            _openAI = openAI;
            _repo = repo;
            _roomRepo = roomRepo;
            _eventRepo = eventRepo;
        }

        // Step 3: ProcessUserMessage — called by ChatController
        public async Task<string> ProcessUserMessageAsync(string userMessage)
        {
            // lấy phòng
            var rooms = await _roomRepo.GetAvailableRoomsAsync();
            var roomData = string.Join("\n", rooms.Select(r =>
 $"""
Room {r.RoomNumber}
- Loại phòng: {r.RoomType}
- Giá qua đêm: {r.OvernightPrice} VND
- Giá theo ngày: {r.DayPrice} VND
- Giá giờ đầu: {r.FirstHourPrice} VND
- Giá giờ tiếp theo: {r.NextHourPrice} VND
- Số khách tối đa: {r.MaxGuests}
- Phí thêm khách: {r.ExtraGuestFee} VND
"""
 ));

            // lấy event
            var events = await _eventRepo.GetUpcomingEventsAsync();
            var eventData = string.Join("\n", events.Select(e =>
   $"""
Sự kiện: {e.Title}
- Mô tả: {e.Description}
- Địa điểm: {e.Location}
- Thành phố: {e.City}
- Ngày diễn ra: {e.EventDate:dd/MM/yyyy}
"""
   ));

            var prompt = $"""
Available rooms in MotelLeAnh49:

{roomData}

Upcoming events in Can Tho:

{eventData}

User question:
{userMessage}

Answer in Vietnamese and recommend rooms or events if relevant.
""";

            var aiResponse = await _openAI.SendPromptAsync(prompt);

            return aiResponse;
        }
    }
}