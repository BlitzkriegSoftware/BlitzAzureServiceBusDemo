using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// Thanks to: https://stackoverflow.com/questions/55650497/creating-an-azure-servicebus-queue-via-code

namespace Blitz.Azure.ServiceBus.Library
{
    public class AzureServiceBusHelper<T> where T : new()
    {
        private readonly string azureServiceBusConnectionString;
        private readonly ILogger logger;

        public delegate T NewMessageDelegate(T model, out bool isOk);
        public event NewMessageDelegate OnNewMessage;

        public delegate string BadMessageDelegate(string message);
        public event BadMessageDelegate OnBadMessage;

        /// <summary>
        /// CTOR (not allowed)
        /// </summary>
        private AzureServiceBusHelper() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="azureServiceBusConnectionString">Azure Service Bus Connection String</param>
        public AzureServiceBusHelper(string azureServiceBusConnectionString, ILogger logger)
        {
            this.azureServiceBusConnectionString = azureServiceBusConnectionString;
            this.logger = logger;
        }

        public IQueueClient MakeClient(string queueName)
        {
            var managementClient = new ManagementClient(this.azureServiceBusConnectionString);

            var allQueues = managementClient.GetQueuesAsync().GetAwaiter().GetResult();

            var foundQueue = allQueues.Where(q => q.Path == queueName.ToLower()).SingleOrDefault();

            if (foundQueue == null)
            {
                managementClient.CreateQueueAsync(queueName);
            }

            return new QueueClient(this.azureServiceBusConnectionString, queueName);
        }

        public async void Enqueue(T model, string queueName, Guid id = default)
        {
            var client = MakeClient(queueName);
            var json = JsonConvert.SerializeObject(model);
            var b = Encoding.UTF8.GetBytes(json);
            var message = new Message(b)
            {
                ContentType = "application/json",
                MessageId = ((id == Guid.Empty) ? Guid.NewGuid() : id).ToString()
            };
            await client.SendAsync(message);
        }

        public void Dequeue(string queueName, NewMessageDelegate goodMessageHandler, BadMessageDelegate badMessageHandler)
        {
            var client = MakeClient(queueName);

            if(goodMessageHandler != null)  this.OnNewMessage += goodMessageHandler;
            if (badMessageHandler != null) this.OnBadMessage += badMessageHandler;

            var messageHandlerOptions = new MessageHandlerOptions((exceptionReceivedEventArgs) =>
            {
                var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
                logger.LogError($"Message handler exception: {exceptionReceivedEventArgs.Exception}, Endpoint: {context.Endpoint}, Entity Path: {context.EntityPath}, Executing Action: {context.Action}");
                return Task.CompletedTask;
            })
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            client.RegisterMessageHandler(async (message, token) =>
            {
                T model = default;
                try
                {
                    byte[] b = message.Body;
                    var json = Encoding.UTF8.GetString(b);
                    model = JsonConvert.DeserializeObject<T>(json);
                    OnNewMessage(model, out bool isOk);
                    if (isOk)
                    {
                        await client.CompleteAsync(message.SystemProperties.LockToken);
                    } else
                    {
                        await client.AbandonAsync(message.SystemProperties.LockToken);
                    }
                }
                catch (Exception ex)
                {
                    OnBadMessage($"Id: {message.MessageId}; CorrId: {message.CorrelationId}; Ex: {ex.Message}");
                    await client.DeadLetterAsync(message.SystemProperties.LockToken);
                }
                
            }, messageHandlerOptions);
        }

    }
}
