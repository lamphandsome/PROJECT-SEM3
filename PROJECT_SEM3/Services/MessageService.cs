using Microsoft.EntityFrameworkCore;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.Models;

namespace PROJECT_SEM3.Services
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(string senderId, string receiverId, string content);
        Task<List<Message>> GetChatHistoryAsync(string userId1, string userId2, int page = 1, int pageSize = 20);
        Task<List<Users>> GetRecentChatsAsync(string userId, int count = 10);
        Task MarkMessageAsReadAsync(int messageId);
        Task<int> GetUnreadMessageCountAsync(string userId);
    }

    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Message> SendMessageAsync(string senderId, string receiverId, string content)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetChatHistoryAsync(string userId1, string userId2, int page = 1, int pageSize = 20)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }

        public async Task<List<Users>> GetRecentChatsAsync(string userId, int count = 10)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .Take(count)
                .Join(_context.Users,
                    id => id,
                    user => user.Id,
                    (id, user) => user)
                .ToListAsync();
        }

        public async Task MarkMessageAsReadAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadMessageCountAsync(string userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .CountAsync();
        }
    }
}
