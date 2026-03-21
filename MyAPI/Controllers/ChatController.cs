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
        var claim = User.FindFirst("id");

        if (claim == null)
            return Unauthorized("Token không có id");

        if (!Guid.TryParse(claim.Value, out var senderId))
            return Unauthorized("id không hợp lệ");

        var result = await _chatService.SendMessage(
            senderId,
            request.ConversationId,
            request.Content
        );

        if (result.Data == null)
            return BadRequest(result);

        var data = result.Data;

        var messageDto = new RoomMessageDto
        {
            ConversationId = ((dynamic)data).conversationId,
            Id = ((dynamic)data).id,
            Content = ((dynamic)data).content,
            CreatedAt = ((dynamic)data).createdAt
        };

        await _hub.Clients.Group(messageDto.ConversationId.ToString())
            .SendAsync("ReceiveRoomMessage", messageDto);

        return Ok(result);
    }
}