using System.Security.Claims;
using IBDirect.API.Data;
using IBDirect.API.DTOs;
using IBDirect.API.Entities;
using IBDirect.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.SignalR
{
    [Authorize(Roles = "1,2,3,4,5")]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly DataContext _context;

        public MessageHub(IMessageRepository messageRepository, DataContext context)
        {
            _messageRepository = messageRepository;
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUserId = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                otherUserId
            );

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageRepository.GetMessageThread(
                int.Parse(
                    Context.User.Claims
                        .LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                        ?.Value
                ),
                int.Parse(otherUserId)
            );

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(MessageDto createMessageDto)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == createMessageDto.SenderId))
            {
                throw new HubException("Sender does not exist");
            }
            else if (!await _context.Users.AnyAsync(u => u.Id == createMessageDto.RecipientId))
            {
                throw new HubException("Recipient does not exist");
            }
            else if (createMessageDto.SenderId == createMessageDto.RecipientId)
            {
                throw new HubException("Sender and recipient cannot be the same");
            }

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

            var groupName = GetGroupName(
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                createMessageDto.RecipientId.ToString()
            );
            await Clients.Group(groupName).SendAsync("NewMessage", message);
        }

        public string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
