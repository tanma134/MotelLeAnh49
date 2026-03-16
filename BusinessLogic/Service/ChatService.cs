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
                $"Room {r.RoomNumber} - {r.RoomType} - {r.DayPrice} VND"));

            // lấy event
            var events = await _eventRepo.GetUpcomingEventsAsync();
            var eventData = string.Join("\n", events.Select(e =>
                $"{e.Title} - {e.Location} - {e.EventDate:dd/MM/yyyy}"
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