using Dev_Db.Data;
using Dev_Models.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class ChatHub : Hub
{
    private readonly Dev_DbContext _context;
    private static Dictionary<string, string> _connectedUsers = new Dictionary<string, string>();

    public ChatHub(Dev_DbContext context)
    {
        _context = context;

    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            _connectedUsers[Context.ConnectionId] = userId;
            await Clients.All.SendAsync("UserConnected", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (_connectedUsers.ContainsKey(Context.ConnectionId))
        {
            var userId = _connectedUsers[Context.ConnectionId];
            _connectedUsers.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UserDisconnected", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverId, string message)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(senderId))
        {
            return;
        }

        var chatMessage = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = message,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);

            await Clients.User(receiverId).SendAsync("NewMessageNotification", senderId);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving message: {ex.Message}");
        }
    }

    public async Task GetAvailableUsers()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"GetAvailableUsers called. Current userId: {userId}");
        if (string.IsNullOrEmpty(userId))
        {
            Console.WriteLine("UserId is null or empty - returning");
            return;
        }
        var users = await _context.Users
            .Where(u => u.Id != userId)
            .Select(u => new { u.Id, u.Fullname, u.ProfileImage })
            .ToListAsync();
        Console.WriteLine($"Found {users.Count} users");
        await Clients.Caller.SendAsync("AvailableUsers", users);
    }


    public async Task GetChatHistory(string otherUserId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return;

        var messages = await _context.ChatMessages
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderBy(m => m.Timestamp)
            .Select(m => new { m.SenderId, m.Content, m.Timestamp })
            .ToListAsync();


        await Clients.Caller.SendAsync("ChatHistory", messages, userId);
    }
}
