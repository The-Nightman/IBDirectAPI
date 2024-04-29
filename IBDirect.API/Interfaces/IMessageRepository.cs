using IBDirect.API.DTOs;
using IBDirect.API.Entities;

namespace IBDirect.API.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> CreateMessage(MessageDto createMessageDto);
        Task<UserInboxDto> GetUserInbox(int currentId);
        Task<IEnumerable<Message>> GetMessageThread(int currentId, int otherId);
        Task<IEnumerable<UserUnreadChatsDto>> GetUnreadMessages(int currentId);
        void AddGroup (Group group);
        void RemoveConnection (Connection connection);
        Task<Connection> GetConnection (string connectionId);
        Task<Group> GetMessageGroup (string groupName);
        Task<bool> SaveAllAsync();
    }
}