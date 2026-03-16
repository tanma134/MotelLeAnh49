// BusinessLogic/Service/ChatService.cs
using DataAccess.Repositories;

namespace BusinessLogic.Service
{
    public class ChatService : IChatService
    {
        private readonly IOpenAIService _openAI;
        private readonly IChatRepository _repo;
        private readonly IRoomRepository _roomRepo;
        public ChatService(
     IOpenAIService openAI,
     IChatRepository repo,
     IRoomRepository roomRepo)
        {
            _openAI = openAI;
            _repo = repo;
            _roomRepo = roomRepo;
        }

        // Step 3: ProcessUserMessage — called by ChatController
        public async Task<string> ProcessUserMessageAsync(string userMessage)
        {
            var rooms = await _roomRepo.GetAvailableRoomsAsync();
            var roomData = string.Join("\n", rooms.Select(r =>
            $"Room {r.RoomNumber} - {r.RoomType} - {r.DayPrice} VND"));

            var prompt = $"""
    Available rooms in MotelLeAnh49:s

    {roomData}

    User question:
    {userMessage}
    """;

            var aiResponse = await _openAI.SendPromptAsync(prompt);

            return aiResponse;
            //// Load history for context
            //var history = await _repo.GetHistoryAsync(limit: 20);
            //var historyTuples = history.SelectMany(m => new[]
            //{
            //("user",      m.UserMessage),
            //("model", m.AiResponse)
            //});

            //// Step 4–6: Call Anthropic API
            //var aiResponse = await _openAI.SendPromptAsync(userMessage, historyTuples);

            //// Step 7–12: Save to DB
            //await _repo.SaveChatAsync(userMessage, aiResponse);

            //// Step 13: Return to ChatController
            //return aiResponse;
        }
    }
}