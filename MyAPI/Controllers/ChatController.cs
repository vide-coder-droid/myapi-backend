using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using MyAPI.Hubs;
using MyAPI.Services.Chat;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _hub;

    public ChatController(IChatService chatService, IHubContext<ChatHub> hub)
    {
        _chatService = chatService;
        _hub = hub;
    }

    [Authorize]
    [HttpGet("conversations/{userId}")]
    public async Task<IActionResult> GetConversations(Guid userId)
    {
        var result = await _chatService.GetUserConversations(userId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("messages/{conversationId}")]
    public async Task<IActionResult> GetMessages(Guid conversationId)
    {
        var result = await _chatService.GetMessages(conversationId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var result = await _chatService.SendMessage(
            request.ConversationId,
            request.Content
        );

        if (result.Data == null)
            return BadRequest(result);

        var data = result.Data;

        var messageDto = new RoomMessageDto
        {
            ConversationId = data.ConversationId,
            Id = data.Id,
            Content = data.Content,
            CreatedAt = data.CreatedAt,
            SenderId = data.SenderId
        };

        await _hub.Clients.Group(messageDto.ConversationId.ToString())
            .SendAsync("ReceiveRoomMessage", messageDto);

        return Ok(result);
    }
}