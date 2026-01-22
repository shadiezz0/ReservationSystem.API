using Microsoft.AspNetCore.SignalR;

namespace ReservationSystem.API.Hubs
{
    public class NotificationHub:Hub
    {
        //// Client to Server example
        //public async Task SendMessageToAll(string message)
        //{
        //    await Clients.All.SendAsync("ReceiveNotification", new
        //    {
        //        Message = message,
        //        CreatedAt = DateTime.UtcNow
        //    });
        //}

        // Client to Server to specific user
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
