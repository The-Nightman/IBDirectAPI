using IBDirect.API.DTOs;
using IBDirect.API.Entities;
using IBDirect.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        public MessageRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Message> CreateMessage(MessageDto createMessageDto)
        {
            var message = new Message
            {
                Content = createMessageDto.Content,
                DateSent = DateTime.UtcNow,
                SenderId = createMessageDto.SenderId,
                SenderName = createMessageDto.SenderName,
                SenderRole = createMessageDto.SenderRole,
                RecipientId = createMessageDto.RecipientId,
                RecipientName = createMessageDto.RecipientName,
                RecipientRole = createMessageDto.RecipientRole,
                Read = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<UserInboxDto> GetUserInbox(int currentId)
        {
            var unreadChats = await _context.Messages
                .Where(m => m.RecipientId == currentId && !m.Read)
                .GroupBy(m => m.SenderId)
                .Select(
                    g =>
                        new UserUnreadChatsDto
                        {
                            Content = g.First().Content,
                            MostRecent = g.Max(m => m.DateSent),
                            SenderId = g.Key,
                            SenderName = g.First().SenderName,
                            SenderRole = g.First().SenderRole,
                            UnreadMessages = g.Count()
                        }
                )
                .OrderByDescending(m => m.MostRecent)
                .ToListAsync();

            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

            var unreadChatSenderIds = unreadChats.Select(u => u.SenderId).ToList();

            var recentChats = await _context.Messages
                .Where(
                    m =>
                        (m.RecipientId == currentId || m.SenderId == currentId)
                        && m.DateSent >= oneWeekAgo
                        && !unreadChatSenderIds.Contains(
                            m.SenderId == currentId ? m.RecipientId : m.SenderId
                        )
                )
                .GroupBy(m => m.SenderId == currentId ? m.RecipientId : m.SenderId)
                .Select(
                    g =>
                        new UserRecentChatsDto
                        {
                            Content = g.First().Content,
                            MostRecent = g.Max(m => m.DateSent),
                            SenderId = g.Key,
                            SenderName =
                                g.First().SenderId == currentId
                                    ? g.First().RecipientName
                                    : g.First().SenderName,
                            SenderRole =
                                g.First().SenderId == currentId
                                    ? g.First().RecipientRole
                                    : g.First().SenderRole
                        }
                )
                .OrderByDescending(m => m.MostRecent)
                .ToListAsync();

            var inbox = new UserInboxDto { UnreadChats = unreadChats, RecentChats = recentChats };

            return inbox;
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int currentId, int recipientId)
        {
            var messages = await _context.Messages
                .Where(
                    m =>
                        (m.SenderId == currentId && m.RecipientId == recipientId)
                        || (m.SenderId == recipientId && m.RecipientId == currentId)
                )
                .OrderBy(m => m.DateSent)
                .ToListAsync();

            var unreadMessages = messages
                .Where(m => m.RecipientId == currentId && !m.Read)
                .ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.Read = true;
                }

                await _context.SaveChangesAsync();
            }

            return messages;
        }
    }
}
