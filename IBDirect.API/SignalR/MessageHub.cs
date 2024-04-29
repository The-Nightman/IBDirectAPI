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
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(
            IMessageRepository messageRepository,
            DataContext context,
            IHubContext<PresenceHub> presenceHub
        )
        {
            _messageRepository = messageRepository;
            _context = context;
            _presenceHub = presenceHub;
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
            await AddToGroup(groupName);

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

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromGroup();
            await base.OnDisconnectedAsync(exception);
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

            var groupName = GetGroupName(
                Context.User.Claims.LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                createMessageDto.RecipientId.ToString()
            );

            var group = await _messageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(c => c.UserId == message.RecipientId))
            {
                message.Read = true;
            }

            _context.Messages.Add(message);

            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", message);
            }
        }

        public string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        public async Task<bool> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(
                Context.ConnectionId,
                int.Parse(
                    Context.User.Claims
                        .LastOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                        ?.Value
                )
            );

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await _messageRepository.SaveAllAsync();
        }

        public async Task RemoveFromGroup()
        {
            var connection = await _messageRepository.GetConnection(Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            await _messageRepository.SaveAllAsync();
        }
    }
}
