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
        if (!await _context.Users.AnyAsync(u => u.Id == createMessageDto.SenderId))
        {
            return BadRequest("Sender does not exist");
        }
        else if (!await _context.Users.AnyAsync(u => u.Id == createMessageDto.RecipientId))
        {
            return BadRequest("Recipient does not exist");
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

        return NoContent();
    }
}
