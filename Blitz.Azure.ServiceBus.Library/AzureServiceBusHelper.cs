using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Management.ServiceBus;

namespace Blitz.Azure.ServiceBus.Library
{
    public class AzureServiceBusHelper
    {
        private readonly string AzureServiceBusConnectionString;

        /// <summary>
        /// CTOR (not allowed)
        /// </summary>
        private AzureServiceBusHelper() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="azureServiceBusConnectionString">Azure Service Bus Connection String</param>
        public AzureServiceBusHelper(string azureServiceBusConnectionString)
        {
            AzureServiceBusConnectionString = azureServiceBusConnectionString;
        }

        public IQueueClient MakeClient(string queueName)
        {
            return new QueueClient(this.AzureServiceBusConnectionString, queueName);
        }

        public bool CreateIfNotExists(string queueName)
        {

            return true;
        }

        public void Enqueue<T>(T model, string queueName)
        {

        }

        public T Dequeue<T>(string queueName)
        {
            return default;
        }

    }
}
