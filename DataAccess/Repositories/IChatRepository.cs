using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IChatRepository
    {


        /// <summary>Retrieve full chat history for context.</summary>
        Task<IEnumerable<ChatMessage>> GetHistoryByCustomerIdAsync(int customerId, int limit = 5);

        Task SaveChatAsync(int? customerId, string userMessage, string aiResponse);
    }
}