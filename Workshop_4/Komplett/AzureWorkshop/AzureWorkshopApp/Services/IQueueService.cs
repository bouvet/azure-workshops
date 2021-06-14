using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureWorkshopApp.Services
{
    public interface IQueueService
    {
        Task SendQueueMessage(string queueName, string message);
    }
}
