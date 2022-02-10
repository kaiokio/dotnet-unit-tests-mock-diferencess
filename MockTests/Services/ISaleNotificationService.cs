using MockTests.Entities;

namespace MockTests.Services
{
    public interface ISaleNotificationService
    {
        void SendNotification(string clientName, string message);
    }
}