using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IChatRepository
    {
        /// <summary>Step 7–12: Persist a chat exchange to the database.</summary>
        Task SaveChatAsync(string userMessage, string aiResponse);

        /// <summary>Retrieve full chat history for context.</summary>
        Task<IEnumerable<ChatMessage>> GetHistoryAsync(int limit = 50);
    }
}