using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.IService
{
    public interface INotificationService
    {
        Task NotifyUserAsync(string userId, string message);
        Task NotifyAllAsync(string title, string message);
    }
}
