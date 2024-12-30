using Microsoft.AspNetCore.SignalR;
using PROJECT_SEM3.Dtos;
using PROJECT_SEM3.Services;
using System.Security.Claims;

namespace PROJECT_SEM3.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private static readonly Dictionary<string, string> _userConnections = new();

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                await Clients.Others.SendAsync("UserOnline", userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.Remove(userId);
                await Clients.Others.SendAsync("UserOffline", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            var message = await _messageService.SendMessageAsync(senderId, receiverId, content);

            var messageDto = new ChatMessageDTO
            {
                Id = message.Id,
                SenderId = senderId,
                SenderName = Context.User.Identity.Name,
                ReceiverId = receiverId,
                Content = content,
                SentAt = message.SentAt,
                IsRead = false
            };

            // Gửi tin nhắn đến người nhận nếu họ online
            if (_userConnections.TryGetValue(receiverId, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", messageDto);
            }

            // Gửi xác nhận về cho người gửi
            await Clients.Caller.SendAsync("MessageSent", messageDto);
        }

        public async Task TypingStarted(string receiverId)
        {
            if (_userConnections.TryGetValue(receiverId, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("UserTyping", Context.User.Identity.Name);
            }
        }

        public async Task TypingStopped(string receiverId)
        {
            if (_userConnections.TryGetValue(receiverId, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("UserStoppedTyping", Context.User.Identity.Name);
            }
        }
    }
}