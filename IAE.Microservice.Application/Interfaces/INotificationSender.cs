using System.Threading.Tasks;
using IAE.Microservice.Application.Notifications.Models;

namespace IAE.Microservice.Application.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(Message message);
    }
}
