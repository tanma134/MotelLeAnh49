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

        // Step 7: SaveChat — called by ChatService
        public async Task SaveChatAsync(string userMessage, string aiResponse)
        {
            var chat = new ChatMessage
            {
                UserMessage = userMessage,
                AiResponse = aiResponse,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chat);

            // Step 8: ChatRepository → MotelDbContext.SaveChanges()
            // Step 9: MotelDbContext → Database (INSERT)
            // Step 10–12: DB confirms → MotelDbContext → ChatRepository → ChatService
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetHistoryAsync(int limit = 50)
        {
            return await _context.ChatMessages
                .OrderByDescending(c => c.CreatedAt)
                .Take(limit)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}