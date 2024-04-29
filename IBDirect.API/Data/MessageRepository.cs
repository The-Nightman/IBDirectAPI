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

        public async Task<IEnumerable<UserUnreadChatsDto>> GetUnreadMessages(
            int currentId,
            Message newMessage = null
        )
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

            if (
                newMessage != null
                && newMessage.RecipientId == currentId
                && newMessage.Read == false
            )
            {
                var chatToUpdate = unreadChats.FirstOrDefault(
                    c => c.SenderId == newMessage.SenderId
                );
                if (chatToUpdate != null)
                {
                    chatToUpdate.UnreadMessages++;
                    chatToUpdate.Content = newMessage.Content;
                    chatToUpdate.MostRecent = newMessage.DateSent;
                }
                else
                {
                    unreadChats.Add(
                        new UserUnreadChatsDto
                        {
                            Content = newMessage.Content,
                            MostRecent = newMessage.DateSent,
                            SenderId = newMessage.SenderId,
                            SenderName = newMessage.SenderName,
                            SenderRole = newMessage.SenderRole,
                            UnreadMessages = 1
                        }
                    );
                }
            }

            return unreadChats;
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

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(g => g.Connections)
                .FirstOrDefaultAsync(g => g.Name == groupName);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
