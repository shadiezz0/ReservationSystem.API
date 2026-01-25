using Microsoft.AspNetCore.SignalR;

namespace ReservationSystem.API.Hubs
{
    public class NotificationHub:Hub
    {
        //// Client to Server example
        //public async Task SendMessageToAll(string message)
        //{
        //    await Clients.All.SendAsync("ReseiveNotification", new
        //    {
        //        Message = message,
        //        CreatedAt = DateTime.UtcNow
        //    });
        //}
        // 🔹 لما المستخدم يعمل Connection
        //public override async Task OnConnectedAsync()
        //{
        //    var userId = Context.UserIdentifier; // جاية من JWT

        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        // اختياري – مش ضروري لو هتستخدم Clients.User
        //        await Groups.AddToGroupAsync(Context.ConnectionId, $"USER_{userId}");
        //    }

        //    await base.OnConnectedAsync();
        //}

        // 🔹 (اختياري) Client → Server → Client
        // تستخدمها لو عايز تبعت إشعار من الفرونت نفسه
        public async Task SendMessageToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReserveNotification", new
            {
                Message = message,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
