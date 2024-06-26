using IBDirect.API.Controllers;
using IBDirect.API.Data;
using IBDirect.API.DTOs;
using IBDirect.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Namespace;

[Authorize(Roles = "1,2,3,4,5")]
public class ChatController : BaseApiController
{
    private readonly DataContext _context;

    public ChatController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("create-message")]
    public async Task<ActionResult<Message>> CreateMessage(MessageDto createMessageDto)
    {
        await UsersExist(createMessageDto.SenderId, createMessageDto.RecipientId);

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

        return Ok(message);
    }

    [HttpGet("user-inbox/{currentId}")]
    public async Task<ActionResult<UserInboxDto>> GetUserInbox(int currentId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == currentId))
        {
            return BadRequest("User does not exist");
        }

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
                    && !unreadChatSenderIds.Contains(m.SenderId == currentId ? m.RecipientId : m.SenderId)
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

        var inbox = new UserInboxDto
        { 
            UnreadChats = unreadChats,
            RecentChats = recentChats
        };

        return Ok(inbox);
    }

    [HttpGet("user-thread/{currentId}/{recipientId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetUserThread(
        int currentId,
        int recipientId
    )
    {
        await UsersExist(currentId, recipientId);

        var messages = await _context.Messages
            .Where(
                m =>
                    (m.SenderId == currentId && m.RecipientId == recipientId)
                    || (m.SenderId == recipientId && m.RecipientId == currentId)
            )
            .OrderBy(m => m.DateSent)
            .ToListAsync();

        var unreadMessages = messages.Where(m => m.RecipientId == currentId && !m.Read).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.Read = true;
            }

            await _context.SaveChangesAsync();
        }

        return Ok(messages);
    }

    private async Task<BadRequestObjectResult> UsersExist(int senderId, int recipientId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == senderId))
        {
            return BadRequest("Sender does not exist");
        }
        else if (!await _context.Users.AnyAsync(u => u.Id == recipientId))
        {
            return BadRequest("Recipient does not exist");
        }
        return null;
    }
}
