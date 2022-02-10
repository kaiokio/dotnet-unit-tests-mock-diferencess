using MockTests.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockTests.Services
{
    public class SaleNotificationService : ISaleNotificationService
    {
        public void SendNotification(string clientName, string message)
        {
            Console.WriteLine($"Notification sent. Clientname: {clientName}, Message: {message}");
        }
    }
}
