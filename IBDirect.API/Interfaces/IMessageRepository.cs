using IBDirect.API.DTOs;
using IBDirect.API.Entities;

namespace IBDirect.API.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> CreateMessage(MessageDto createMessageDto);
        Task<UserInboxDto> GetUserInbox(int currentId);
        Task<IEnumerable<Message>> GetMessageThread(int currentId, int otherId);
    }
}