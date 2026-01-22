using Microsoft.AspNetCore.SignalR;
using ReservationSystem.API.Hubs;

namespace ReservationSystem.API.Services
{
    public class SignalRNotificationService: INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task NotifyUserAsync(string userId, string message)
        {
            await _hubContext.Clients
                .User(userId)
                .SendAsync("ReserveNotification", new
                {
                    Message = message,
                    CreatedAt = DateTime.UtcNow
                });
        }

        public async Task NotifyAllAsync(string title, string message)
        {
            await _hubContext.Clients
                .All
                .SendAsync("ReserveNotification", new
                {
                    Title = title,
                    Message = message,
                    CreatedAt = DateTime.UtcNow
                });
        }


    }
}
