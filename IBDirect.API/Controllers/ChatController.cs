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
    public async Task<ActionResult<MessageDto>> CreateMessage(MessageDto createMessageDto)
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

        return NoContent();
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
