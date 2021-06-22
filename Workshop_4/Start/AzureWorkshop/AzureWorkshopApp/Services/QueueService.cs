using Azure.Storage.Queues;
using AzureWorkshopApp.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AzureWorkshopApp.Services
{
    public class QueueService : IQueueService
    {
        private readonly AzureStorageConfig _storageConfig;

        public QueueService(IOptions<AzureStorageConfig> storageConfig)
        {
            _storageConfig = storageConfig != null ? storageConfig.Value : throw new ArgumentNullException(nameof(storageConfig));
        }

        public async Task SendQueueMessage(string queueName, string message)
        {
            QueueServiceClient queueServiceClient = new QueueServiceClient(_storageConfig.ConnectionString, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

            var queueClient = queueServiceClient.GetQueueClient(queueName);

            await queueClient.CreateIfNotExistsAsync();

            await queueClient.SendMessageAsync(message);
        }
    }
}
