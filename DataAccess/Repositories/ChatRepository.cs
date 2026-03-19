// DataAccess/Repo/ChatRepository.cs
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Data;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Step 7–12 in the sequence diagram.
    /// Calls MotelDbContext.SaveChangesAsync() which triggers the DB insert.
    /// </summary>
    public class ChatRepository : IChatRepository
    {
        private readonly MotelDbContext _context;

        public ChatRepository(MotelDbContext context)
        {
            _context = context;
        }
       
        public async Task SaveChatAsync(int? customerId, string userMessage, string aiResponse)
        {
            var chat = new ChatMessage
            {
                CustomerId = customerId,
                UserMessage = userMessage,
                AiResponse = aiResponse,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chat);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ChatMessage>> GetHistoryByCustomerIdAsync(int customerId, int limit = 5)
        {
            return await _context.ChatMessages
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.CreatedAt)
                .Take(limit)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}